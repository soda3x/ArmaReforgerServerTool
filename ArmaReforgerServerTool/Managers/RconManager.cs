/******************************************************************************
 * File Name:    RconManager.cs
 * Project:      Longbow
 * Description:  Singleton that sends BattlEye RCON commands to the running
 *               Arma Reforger dedicated server (UDP-based protocol).
 *
 * Author:       Bradley Newman
 ******************************************************************************/

using Longbow.Managers;
using Serilog;
using System.Net.Sockets;
using System.Text;

namespace ReforgerServerApp.Managers
{
  internal class RconManager
  {
    private static RconManager? m_instance;
    private byte m_sequenceNumber;

    public static RconManager GetInstance()
    {
      m_instance ??= new RconManager();
      return m_instance;
    }

    private RconManager() { }

    /// <summary>
    /// Broadcast a message to all connected players via BattlEye RCON.
    /// Silently no-ops when RCON is not enabled in the server configuration.
    /// </summary>
    public async Task SendBroadcastAsync(string message)
    {
      var config = ConfigurationManager.GetInstance().GetServerConfiguration();
      if (!config.rconEnabled || config.root.rcon == null)
      {
        Log.Debug("RconManager - RCON not enabled, skipping broadcast.");
        return;
      }

      var rcon = config.root.rcon;
      string host = string.IsNullOrEmpty(rcon.address) || rcon.address == "0.0.0.0"
          ? "127.0.0.1" : rcon.address;

      try
      {
        using UdpClient udp = new();
        udp.Client.ReceiveTimeout = 3000;
        udp.Connect(host, rcon.port);

        await udp.SendAsync(BuildPacket(0x00, Encoding.UTF8.GetBytes(rcon.password)));
        UdpReceiveResult loginResp = await udp.ReceiveAsync().WaitAsync(TimeSpan.FromSeconds(3));

        if (loginResp.Buffer.Length < 9 || loginResp.Buffer[8] != 0x01)
        {
          Log.Warning("RconManager - RCON login failed (wrong password or server not ready).");
          return;
        }

        string cmd = $"say -1 {message}";
        byte[] cmdPayload = new byte[1 + Encoding.UTF8.GetByteCount(cmd)];
        cmdPayload[0] = m_sequenceNumber++;
        Encoding.UTF8.GetBytes(cmd, 0, cmd.Length, cmdPayload, 1);
        await udp.SendAsync(BuildPacket(0x01, cmdPayload));

        Log.Debug("RconManager - Broadcast sent: \"{message}\"", message);
      }
      catch (Exception ex)
      {
        Log.Warning("RconManager - Broadcast failed: {msg}", ex.Message);
      }
    }

    private static byte[] BuildPacket(byte type, byte[] payload)
    {
      // BattlEye packet: BE(2) + CRC32(4) + 0xFF(1) + type(1) + payload
      byte[] packet = new byte[8 + payload.Length];
      packet[0] = 0x42; // 'B'
      packet[1] = 0x45; // 'E'
      packet[6] = 0xFF;
      packet[7] = type;
      Array.Copy(payload, 0, packet, 8, payload.Length);

      uint crc = ComputeCrc32(packet, 6, packet.Length - 6);
      packet[2] = (byte)(crc & 0xFF);
      packet[3] = (byte)((crc >> 8) & 0xFF);
      packet[4] = (byte)((crc >> 16) & 0xFF);
      packet[5] = (byte)((crc >> 24) & 0xFF);
      return packet;
    }

    private static uint ComputeCrc32(byte[] data, int offset, int length)
    {
      uint crc = 0xFFFFFFFF;
      for (int i = offset; i < offset + length; i++)
      {
        crc ^= data[i];
        for (int j = 0; j < 8; j++)
          crc = (crc & 1) == 1 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
      }
      return ~crc;
    }
  }
}
