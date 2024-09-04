/******************************************************************************
 * File Name:    ToolProperties.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This file contains the ToolProperties class, a model
 *               representing the server tool properties file
 * 
 * Author:       Kye Seyhun
 ******************************************************************************/

using Serilog;
using Serilog.Events;
using System.Text.Json;

namespace ReforgerServerApp.Models
{
    /// <summary>
    /// This class represents the structure of the properties file containing the application settings.
    /// </summary>
    internal class ToolProperties
    {
        private static readonly List<string> DEFAULT_SCENARIOS = new List<string>
        {
            "{ECC61978EDCC2B5A}Missions/23_Campaign.conf",
            "{59AD59368755F41A}Missions/21_GM_Eden.conf",
            "{94FDA7451242150B}Missions/103_Arland_Tutorial.conf",
            "{2BBBE828037C6F4B}Missions/22_GM_Arland.conf",
            "{C700DB41F0C546E1}Missions/23_Campaign_NorthCentral.conf",
            "{28802845ADA64D52}Missions/23_Campaign_SWCoast.conf",
            "{DAA03C6E6099D50F}Missions/24_CombatOps.conf",
            "{C41618FD18E9D714}Missions/23_Campaign_Arland.conf",
            "{DFAC5FABD11F2390}Missions/26_CombatOpsEveron.conf"
        };

        private static readonly string DEFAULT_MOD_DATABASE_FILE            = "./mod_database.json";
        private static readonly string DEFAULT_INSTALL_DIR_FILE             = "./install_directory.txt";
        private static readonly string DEFAULT_UPDATE_REPOSITORY            = "https://raw.githubusercontent.com/soda3x/ArmaReforgerServerTool";
        private static readonly string DEFAULT_BUG_REPORT_REPOSITORY        = "https://github.com/soda3x/ArmaReforgerServerTool/issues";
        private static readonly bool   DEFAULT_CHECK_FOR_UPDATES_ON_STARTUP = true;
        private static readonly string DEFAULT_STEAMCMD_DOWNLOAD_URL        = "https://steamcdn-a.akamaihd.net/client/installer";
        private static readonly string DEFAULT_ARMA_WORKSHOP_URL            = "https://reforger.armaplatform.com/workshop";
        private static readonly string DEFAULT_LOG_FILE                     = "logs/ardst.log";
        private static readonly string DEFAULT_MINIMUM_LOG_LEVEL            = "Debug";

        public List<string> defaultScenarios { get; }
        public string modDatabaseFile { get; }
        public string installDirectoryFile { get; }
        public string updateRepositoryUrl { get; }
        public string bugReportUrl { get; }
        public bool checkForUpdatesOnStartup { get; }
        public string steamCmdDownloadUrl { get; }
        public string armaWorkshopUrl { get; }
        public string logFile { get; }
        public string minimumLogLevel { get; }

        /// <summary>
        /// Constructs an instance of the Tool Properties model
        /// </summary>
        /// <param name="defaultScenarios"></param>
        /// <param name="modDatabaseFile"></param>
        /// <param name="installDirectoryFile"></param>
        /// <param name="updateRepositoryUrl"></param>
        /// <param name="bugReportUrl"></param>
        /// <param name="checkForUpdatesOnStartup"></param>
        /// <param name="steamCmdDownloadUrl"></param>
        /// <param name="armaWorkshopUrl"></param>
        /// <param name="logFile"></param>
        /// <param name="minimumLogLevel"></param>
        public ToolProperties(List<string> defaultScenarios, string modDatabaseFile, string installDirectoryFile,
            string updateRepositoryUrl, string bugReportUrl, bool checkForUpdatesOnStartup,
            string steamCmdDownloadUrl, string armaWorkshopUrl, string logFile, string minimumLogLevel)
        {
            this.defaultScenarios = defaultScenarios;
            this.modDatabaseFile = modDatabaseFile;
            this.installDirectoryFile = installDirectoryFile;
            this.updateRepositoryUrl = updateRepositoryUrl;
            this.bugReportUrl = bugReportUrl;
            this.checkForUpdatesOnStartup = checkForUpdatesOnStartup;
            this.steamCmdDownloadUrl = steamCmdDownloadUrl;
            this.armaWorkshopUrl = armaWorkshopUrl;
            this.logFile = logFile;
            this.minimumLogLevel = minimumLogLevel;
        }


        public static ToolProperties Default => new(DEFAULT_SCENARIOS, DEFAULT_MOD_DATABASE_FILE, DEFAULT_INSTALL_DIR_FILE,
            DEFAULT_UPDATE_REPOSITORY, DEFAULT_BUG_REPORT_REPOSITORY, DEFAULT_CHECK_FOR_UPDATES_ON_STARTUP,
            DEFAULT_STEAMCMD_DOWNLOAD_URL, DEFAULT_ARMA_WORKSHOP_URL, DEFAULT_LOG_FILE, DEFAULT_MINIMUM_LOG_LEVEL);

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
