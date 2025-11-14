/******************************************************************************
 * File Name:    ServerStatusParser.cs
 * Project:      Longbow
 * Description:  Utility class for parsing server logs and keeping track of 
 *               server statistics
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Shapes;

namespace Longbow.Utils
{
  internal class ServerStatusEventArgs : EventArgs
  {
    private const string UNKNOWN_STR = "Unknown";
    public double LastFPS { set; get; }
    public long LastMem { set; get; }
    public int LastPlayerCount { set; get; }
    public string LastPingSite { set; get; }
    public string LastJoinCode { set; get; }
    public string LastIP { set; get; }
    public int LastPort { set; get; }
    public string LastRconIP { set; get; }
    public int LastRconPort { set; get; }
    public DateTime LastUpdate { set; get; }

    public ServerStatusEventArgs()
    {
      LastIP = UNKNOWN_STR;
      LastRconIP = UNKNOWN_STR;
      LastPingSite = UNKNOWN_STR;
      LastJoinCode = UNKNOWN_STR;
    }
  }

  internal partial class ServerStatusParser
  {
    private const string STATS_REGEX = @"FPS:\s*(?<fps>[\d\.]+).*?Mem:\s*(?<mem>\d+)\s*kB.*?Player:\s*(?<player>\d+)";
    [GeneratedRegex(STATS_REGEX)]
    private static partial Regex StatsRegex();

    private const string PING_SITE_REGEX = @"Ping Site:\s*(?<site>.*)";
    [GeneratedRegex(PING_SITE_REGEX)]
    private static partial Regex PingSiteRegex();

    private const string JOIN_CODE_REGEX = @"Direct Join Code:\s*(?<code>\d+)";
    [GeneratedRegex(JOIN_CODE_REGEX)]
    private static partial Regex JoinCodeRegex();

    private const string RCON_REGEX = @"Ip address=(?<ip>[\d\.]+) and Port=(?<port>\d+)";
    [GeneratedRegex(RCON_REGEX)]
    private static partial Regex RconRegex();

    private const string ADDRESS_REGEX = @"Server registered with address:\s*(?<ip>[\d\.]+):(?<port>\d+)";
    [GeneratedRegex(ADDRESS_REGEX)]
    private static partial Regex AddressRegex();

    public delegate void UpdateServerStatusEventHandler(object sender, ServerStatusEventArgs e);
    public event UpdateServerStatusEventHandler UpdateServerStatus;

    /// <summary>
    /// Sender for the 'UpdateServerStatus' Event
    /// </summary>
    /// <param name="e">Arguments to pass to the GUI to update server status</param>
    protected virtual void OnUpdateServerStatusEvent(ServerStatusEventArgs e)
    {
      UpdateServerStatus?.Invoke(this, e);
    }

    private bool TryAndParseStats(string logLine, ServerStatusEventArgs e)
    {
      Match match = StatsRegex().Match(logLine);

      if (match.Success)
      {
        // Access the values by their group name
        string fps = match.Groups["fps"].Value;
        string mem = match.Groups["mem"].Value;
        string player = match.Groups["player"].Value;

        // Use TryParse for safe conversion
        if (double.TryParse(fps, out double fpsValue))
        {
          e.LastFPS = fpsValue;
        }

        if (long.TryParse(mem, out long memValue))
        {
          e.LastMem = memValue;
        }

        if (int.TryParse(player, out int playerValue))
        {
          e.LastPlayerCount = playerValue;
        }

        OnUpdateServerStatusEvent(e);
      }

      return match.Success;
    }

    private bool TryAndParsePingSite(string logLine, ServerStatusEventArgs e)
    {
      Match match = PingSiteRegex().Match(logLine);

      if (match.Success)
      {
        e.LastPingSite = match.Groups["site"].Value.Trim();
        OnUpdateServerStatusEvent(e);
      }

      return match.Success;
    }

    private bool TryAndParseJoinCode(string logLine, ServerStatusEventArgs e)
    {
      Match match = JoinCodeRegex().Match(logLine);

      if (match.Success)
      {
        e.LastJoinCode = match.Groups["code"].Value;
        OnUpdateServerStatusEvent(e);
      }

      return match.Success;
    }

    private bool TryAndParseRcon(string logLine, ServerStatusEventArgs e)
    {
      Match match = RconRegex().Match(logLine);

      if (match.Success)
      {
        string ip = match.Groups["ip"].Value;
        string port = match.Groups["port"].Value;

        e.LastRconIP = ip;

        if (int.TryParse(port, out int portValue))
        {
          e.LastRconPort = portValue;
        }
        OnUpdateServerStatusEvent(e);
      }

      return match.Success;
    }

    private bool TryAndParseAddress(string logLine, ServerStatusEventArgs e)
    {
      Match match = AddressRegex().Match(logLine);

      if (match.Success)
      {
        e.LastIP = match.Groups["ip"].Value;
        string port = match.Groups["port"].Value;

        if (int.TryParse(port, out int portValue))
        {
          e.LastPort = portValue;
        }
        OnUpdateServerStatusEvent(e);
      }

      return match.Success;
    }

    public void ParseServerStatus(string logLine)
    {
      ServerStatusEventArgs e = new();
      e.LastUpdate = DateTime.Now;
      Debug.WriteLine($"DEBUG_REGEX_INPUT: |{logLine}|");
      TryAndParseAddress(logLine, e);
      TryAndParseRcon(logLine, e);
      TryAndParseJoinCode(logLine, e);
      TryAndParsePingSite(logLine, e);
      TryAndParseStats(logLine, e);
    }
  }
}
