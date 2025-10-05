/******************************************************************************
 * File Name:    ToolProperties.cs
 * Project:      Longbow
 * Description:  This file contains the ToolProperties class, a model
 *               representing the server tool properties file
 * 
 * Author:       Kye Seyhun
 ******************************************************************************/

using Longbow.Models;
using ReforgerServerApp.Utils;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ReforgerServerApp.Utils.JsonUtils;

namespace ReforgerServerApp.Models
{
  /// <summary>
  /// This class represents the structure of the properties file containing the application settings.
  /// </summary>
  [JsonConverter(typeof(ToolProperitesConverter))]
  internal class ToolProperties
  {
    private static readonly List<Scenario> DEFAULT_SCENARIOS =
        [
            new("Conflict - Everon", "{ECC61978EDCC2B5A}Missions/23_Campaign.conf"),
            new("Training", "{002AF7323E0129AF}Missions/Tutorial.conf"),
            new("Game Master - Everon", "{59AD59368755F41A}Missions/21_GM_Eden.conf"),
            new("Game Master - Arland", "{2BBBE828037C6F4B}Missions/22_GM_Arland.conf"),
            new("Game Master - Kolguyev", "{F45C6C15D31252E6}Missions/27_GM_Cain.conf"),
            new("Conflict - Northern Everon", "{C700DB41F0C546E1}Missions/23_Campaign_NorthCentral.conf"),
            new("Conflict - Southern Everon", "{28802845ADA64D52}Missions/23_Campaign_SWCoast.conf"),
            new("Conflict - Western Everon", "{94992A3D7CE4FF8A}Missions/23_Campaign_Western.conf"),
            new("Conflict - Montignac", "{FDE33AFE2ED7875B}Missions/23_Campaign_Montignac.conf"),
            new("Combat Ops - Arland", "{DAA03C6E6099D50F}Missions/24_CombatOps.conf"),
            new("Conflict - Arland", "{C41618FD18E9D714}Missions/23_Campaign_Arland.conf"),
            new("Combat Ops - Everon", "{DFAC5FABD11F2390}Missions/26_CombatOpsEveron.conf"),
            new("Capture & Hold - Briars", "{3F2E005F43DBD2F8}Missions/CAH_Briars_Coast.conf"),
            new("Capture & Hold - Montfort Castle", "{F1A1BEA67132113E}Missions/CAH_Castle.conf"),
            new("Capture & Hold - Concrete Plant", "{589945FB9FA7B97D}Missions/CAH_Concrete_Plant.conf"),
            new("Capture & Hold - Almara Factory", "{9405201CBD22A30C}Missions/CAH_Factory.conf"),
            new("Capture & Hold - Simon's Wood", "{1CD06B409C6FAE56}Missions/CAH_Forest.conf"),
            new("Capture & Hold - Le Moule", "{7C491B1FCC0FF0E1}Missions/CAH_LeMoule.conf"),
            new("Capture & Hold - Camp Blake", "{6EA2E454519E5869}Missions/CAH_Military_Base.conf"),
            new("Capture & Hold - Morton", "{2B4183DF23E88249}Missions/CAH_Morton.conf"),
            new("Elimination", "{C47A1A6245A13B26}Missions/SP01_ReginaV2.conf"),
            new("Air Support", "{0648CDB32D6B02B3}Missions/SP02_AirSupport.conf"),
            new("Conflict: HQ Commander - Everon", "{0220741028718E7F}Missions/23_Campaign_HQC_Everon.conf"),
            new("Conflict: HQ Commander - Arland", "{68D1240A11492545}Missions/23_Campaign_HQC_Arland.conf"),
            new("Conflict: HQ Commander - Kolguyev", "{BB5345C22DD2B655}Missions/23_Campaign_HQC_Cain.conf"),
            new("Operation Omega 01: Over The Hills And Far Away", "{10B8582BAD9F7040}Missions/Scenario01_Intro.conf"),
            new("Operation Omega 02: Radio Check", "{1D76AF6DC4DF0577}Missions/Scenario02_Steal.conf"),
            new("Operation Omega 03: Light In The Dark", "{D1647575BCEA5A05}Missions/Scenario03_Villa.conf"),
            new("Operation Omega 04: Red Silence", "{6D224A109B973DD8}Missions/Scenario04_Sabotage.conf"),
            new("Operation Omega 05: Cliffhanger", "{FA2AB0181129CB16}Missions/Scenario05_Hill.conf"),
            new("Combat Ops - Kolguyev", "{CB347F2F10065C9C}Missions/CombatOpsCain.conf")
        ];

    private static readonly string DEFAULT_MOD_DATABASE_FILE = "./mod_database.json";
    private static readonly string DEFAULT_UPDATE_REPOSITORY = "https://raw.githubusercontent.com/soda3x/ArmaReforgerServerTool";
    private static readonly string DEFAULT_RELEASES_REPOSITORY = "https://github.com/soda3x/ArmaReforgerServerTool/releases/latest";
    private static readonly string DEFAULT_BUG_REPORT_REPOSITORY = "https://github.com/soda3x/ArmaReforgerServerTool/issues";
    private static readonly bool DEFAULT_CHECK_FOR_UPDATES_ON_STARTUP = true;
    private static readonly string DEFAULT_STEAMCMD_DOWNLOAD_URL = "https://steamcdn-a.akamaihd.net/client/installer";
    private static readonly string DEFAULT_ARMA_WORKSHOP_URL = "https://reforger.armaplatform.com/workshop";
    private static readonly string DEFAULT_LOG_FILE = "logs/longbow.log";
    private static readonly string DEFAULT_MINIMUM_LOG_LEVEL = "Debug";
    private static readonly int DEFAULT_AUTO_RESTART_TIME_MS = 2000;

    public List<Scenario> defaultScenarios { get; set; }
    public string modDatabaseFile { get; set; }
    public string updateRepositoryUrl { get; set; }
    public string releaseRepositoryUrl { get; set; }
    public string bugReportUrl { get; set; }
    public bool checkForUpdatesOnStartup { get; set; }
    public string steamCmdDownloadUrl { get; set; }
    public string armaWorkshopUrl { get; set; }
    public string logFile { get; set; }
    public string minimumLogLevel { get; set; }
    public int autoRestartTime_ms { get; set; }

    /// <summary>
    /// Constructs an instance of the Tool Properties model
    /// </summary>
    /// <param name="defaultScenarios"></param>
    /// <param name="modDatabaseFile"></param>
    /// <param name="installDirectoryFile"></param>
    /// <param name="updateRepositoryUrl"></param>
    /// <param name="releaseRepositoryUrl"></param>
    /// <param name="bugReportUrl"></param>
    /// <param name="checkForUpdatesOnStartup"></param>
    /// <param name="steamCmdDownloadUrl"></param>
    /// <param name="armaWorkshopUrl"></param>
    /// <param name="logFile"></param>
    /// <param name="minimumLogLevel"></param>
    /// <param name="autoRestartTime_ms"></param>
    public ToolProperties(List<Scenario> defaultScenarios, string modDatabaseFile,
        string updateRepositoryUrl, string releaseRepositoryUrl, string bugReportUrl, bool checkForUpdatesOnStartup,
        string steamCmdDownloadUrl, string armaWorkshopUrl, string logFile, string minimumLogLevel, int autoRestartTime_ms)
    {
      this.defaultScenarios = defaultScenarios;
      this.modDatabaseFile = modDatabaseFile;
      this.updateRepositoryUrl = updateRepositoryUrl;
      this.releaseRepositoryUrl = releaseRepositoryUrl;
      this.bugReportUrl = bugReportUrl;
      this.checkForUpdatesOnStartup = checkForUpdatesOnStartup;
      this.steamCmdDownloadUrl = steamCmdDownloadUrl;
      this.armaWorkshopUrl = armaWorkshopUrl;
      this.logFile = logFile;
      this.minimumLogLevel = minimumLogLevel;
      this.autoRestartTime_ms = autoRestartTime_ms;
    }


    public static ToolProperties Default => new(DEFAULT_SCENARIOS, DEFAULT_MOD_DATABASE_FILE,
        DEFAULT_UPDATE_REPOSITORY, DEFAULT_RELEASES_REPOSITORY, DEFAULT_BUG_REPORT_REPOSITORY, DEFAULT_CHECK_FOR_UPDATES_ON_STARTUP,
        DEFAULT_STEAMCMD_DOWNLOAD_URL, DEFAULT_ARMA_WORKSHOP_URL, DEFAULT_LOG_FILE, DEFAULT_MINIMUM_LOG_LEVEL, DEFAULT_AUTO_RESTART_TIME_MS);

    /// <summary>
    /// Display <c>ToolProperties</c> in readable Json format.
    /// </summary>
    /// <returns>Json string representation of the <c>ToolProperties</c></returns>
    public string AsJsonString()
    {
      return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Set the Serilog Minimum Log level from a string
    /// </summary>
    /// <param name="levelStr">string representation of min log level</param>
    /// <returns>LoggerConfiguration object with minimum log level set</returns>
    public static LoggerConfiguration SetMinimumLogLevel(string levelStr)
    {
      LoggerConfiguration lc = new LoggerConfiguration();
      switch (levelStr)
      {
        case "Verbose":
          lc.MinimumLevel.Is(LogEventLevel.Verbose);
          break;
        case "Information":
          lc.MinimumLevel.Is(LogEventLevel.Information);
          break;
        case "Error":
          lc.MinimumLevel.Is(LogEventLevel.Error);
          break;
        case "Fatal":
          lc.MinimumLevel.Is(LogEventLevel.Fatal);
          break;
        case null:
        default:
          lc.MinimumLevel.Is(LogEventLevel.Debug);
          break;
      }
      return lc;
    }
  }
}
