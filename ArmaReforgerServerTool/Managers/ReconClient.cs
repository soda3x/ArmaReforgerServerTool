using Force.Crc32;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Longbow.Managers
{
  public class ReconClient
  {

    private UdpClient m_udpClient;
    private string m_password;
    private readonly CancellationTokenSource m_receiveCts = new();

    private byte m_sequenceNumber = 0;
    private bool m_isConnected = false;

    private readonly ConcurrentDictionary<byte, TaskCompletionSource<string>> m_pendingCommands = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> m_recurringTasks = new();

    public event Action<string> OnServerMessage;
    public event Action OnDisconnected;

    public bool IsConnected => m_isConnected;

    public ReconClient(string address, int port, string password)
    {
      m_udpClient = new UdpClient();
      try
      {
        m_udpClient.Connect(address, port);
      } catch (SocketException)
      {
        throw new SocketException();
      }
      m_password = password;
    }

    public async Task<bool> ConnectAsync()
    {
      using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
      try
      {
        bool success = await SendAndAwaitLoginAsync(m_password, cts.Token);
        if (success)
        {
          m_isConnected = true;
          _ = ReceiveLoopAsync(m_receiveCts.Token);
          _ = SendCommandAsync("players"); // Immediately request the server's player info
          StartHeartbeat();
          Log.Information("ReCON Client - Connected successfully.");
          return true;
        }
      }
      catch (OperationCanceledException)
      {
        Log.Error("ReCON Client - Login timed out.");
      }

      m_isConnected = false;
      return false;
    }

    public async Task<string> SendCommandAsync(string command)
    {
      if (!m_isConnected)
        throw new InvalidOperationException("Not connected to RCON.");

      byte seq = GetNextSequence();
      byte[] commandBytes = Encoding.ASCII.GetBytes(command);

      byte[] payload = new byte[2 + commandBytes.Length];
      payload[0] = 0x01;
      payload[1] = seq;
      Array.Copy(commandBytes, 0, payload, 2, commandBytes.Length);

      var tcs = new TaskCompletionSource<string>();
      m_pendingCommands[seq] = tcs;

      await SendPacketAsync(payload);

      var timeoutTask = Task.Delay(3000);
      var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

      m_pendingCommands.TryRemove(seq, out _);

      if (completedTask == timeoutTask)
      {
        Debug.WriteLine($"Command '{command}' timed out waiting for 0x01 response.");
        return string.Empty;
      }

      return await tcs.Task;
    }

    public void StartRecurringCommand(string taskId, string command, TimeSpan interval, Action<string> onResult)
    {
      if (m_recurringTasks.ContainsKey(taskId))
        return;

      var cts = new CancellationTokenSource();
      m_recurringTasks[taskId] = cts;

      _ = Task.Run(async () =>
      {
        using var timer = new PeriodicTimer(interval);
        try
        {
          while (await timer.WaitForNextTickAsync(cts.Token))
          {
            if (!m_isConnected)
              break;
            string result = await SendCommandAsync(command);
            onResult?.Invoke(result);
          }
        }
        catch (OperationCanceledException) { /* Task stopped naturally */ }
        catch (Exception ex)
        {
          Log.Error($"ReCON Client - Recurring task {taskId} failed: {ex.Message}");
        }
      }, cts.Token);
    }

    public void StopRecurringCommand(string taskId)
    {
      if (m_recurringTasks.TryRemove(taskId, out var cts))
      {
        cts.Cancel();
        cts.Dispose();
      }
    }

    private void StartHeartbeat()
    {
      var cts = new CancellationTokenSource();
      m_recurringTasks["Heartbeat"] = cts;

      _ = Task.Run(async () =>
      {
        try
        {
          while (!cts.Token.IsCancellationRequested)
          {
            await Task.Delay(40000, cts.Token); // 40 seconds
            if (m_isConnected)
            {
              byte seq = GetNextSequence();
              // Spec: 0x01 | seq | [nothing]
              byte[] heartbeatPayload = new byte[] { 0x01, seq };
              await SendPacketAsync(heartbeatPayload);
            }
          }
        }
        catch (OperationCanceledException) { /* Clean exit */ }
        catch (Exception ex)
        {
          Log.Error($"Heartbeat failed: {ex.Message}");
        }
      }, cts.Token);
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
      try
      {
        while (!token.IsCancellationRequested)
        {
          var result = await m_udpClient.ReceiveAsync(token);
          byte[] data = result.Buffer;

          if (data.Length < 9)
            continue;

          byte packetType = data[7];
          byte seq = data[8];
          string content = Encoding.ASCII.GetString(data, 9, data.Length - 9);

          Debug.WriteLine($"Received: Type {packetType}, Seq {seq}, Content Length {content.Length}");

          if (packetType == 0x02)
          {
            await SendAckAsync(packetType, seq);
          }

          // Handle the actual content
          if (packetType == 0x01)
          {
            if (m_pendingCommands.TryGetValue(seq, out var tcs))
            {
              tcs.TrySetResult(content);
            }
            if (content.Length > 0)
            {
              OnServerMessage?.Invoke(content);
            }
          }
          else if (packetType == 0x02 && content.Length > 0)
          {
            OnServerMessage?.Invoke(content);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error($"ReCON Client - Loop Error: {ex.Message}");
      }
    }

    private async Task SendAckAsync(byte type, byte seq)
    {
      byte[] ackPayload = new byte[] { type, seq };
      await SendPacketAsync(ackPayload);
    }

    private async Task<bool> SendAndAwaitLoginAsync(string password, CancellationToken token)
    {
      byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
      byte[] loginPayload = new byte[1 + passwordBytes.Length];
      loginPayload[0] = 0x00;
      Array.Copy(passwordBytes, 0, loginPayload, 1, passwordBytes.Length);

      await SendPacketAsync(loginPayload);

      var result = await m_udpClient.ReceiveAsync(token);
      byte[] data = result.Buffer;

      // 0x00 is the login response, 0x01 indicates success
      if (data.Length >= 9 && data[7] == 0x00 && data[8] == 0x01)
      {
        return true;
      }

      Log.Warning("ReCON Client - Login failed. Server returned: {Hex}", BitConverter.ToString(data));
      return false;
    }

    private async Task SendPacketAsync(byte[] payload)
    {
      uint crc32 = Crc32Algorithm.Compute(payload);

      using var ms = new MemoryStream();
      ms.Write(new byte[] { 0x42, 0x45 }, 0, 2); // Header Prefix (BE)
      ms.Write(BitConverter.GetBytes(crc32), 0, 4); // CRC32 of the payload
      ms.Write(new byte[] { 0xFF }, 0, 1); // 0xFF
      ms.Write(payload, 0, payload.Length);   // Payload

      await m_udpClient.SendAsync(ms.ToArray(), (int) ms.Length);
    }

    private byte GetNextSequence()
    {
      byte current = m_sequenceNumber;
      m_sequenceNumber = (byte) ((m_sequenceNumber + 1) % 256);
      return current;
    }

    public void Disconnect()
    {
      if (!m_isConnected)
        return;

      m_receiveCts.Cancel();

      m_isConnected = false;

      foreach (var tcs in m_pendingCommands.Values)
      {
        tcs.TrySetCanceled();
      }
      m_pendingCommands.Clear();

      foreach (var cts in m_recurringTasks.Values)
      {
        cts.Cancel();
      }
      m_recurringTasks.Clear();

      OnDisconnected?.Invoke();

      Log.Information("ReCON Client - Disconnected from RCON server.");
    }

    public void Dispose()
    {
      m_isConnected = false;
      m_receiveCts.Cancel();
      m_receiveCts.Dispose();
      foreach (var cts in m_recurringTasks.Values)
        cts.Cancel();
      m_recurringTasks.Clear();
      m_udpClient?.Dispose();
    }
  }
}
