/******************************************************************************
 * File Name:    FileIOManager.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This file contains the singleton FileIOManager class
 *               responsible for all I/O operations on files
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Serilog;
using ReforgerServerApp.Utils;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Microsoft.Win32;

namespace ReforgerServerApp.Managers
{
    /// <summary>
    /// Manager of all things Files and IO
    /// </summary>
    internal class FileIOManager
    {
        private static FileIOManager?   INSTANCE;
        private readonly string         m_legacyModDatabaseFile = "./mod_database.txt";
        private string                  m_steamCmdFile;
        private string                  m_installDir;
        private FileIOManager()
        {
            bool modDatabaseExists = File.Exists(ToolPropertiesManager.GetInstance().GetToolProperties().modDatabaseFile);

            if (!modDatabaseExists && File.Exists(m_legacyModDatabaseFile) &&
                Utilities.DisplayConfirmationMessage(Constants.MIGRATE_LEGACY_MOD_DB_PROMPT_STR, true))
            {
                MigrateLegacyModDatabase(m_legacyModDatabaseFile);
            }

            if (modDatabaseExists)
            {
                ReadModsDatabase();
            }

            if (File.Exists(ToolPropertiesManager.GetInstance().GetToolProperties().installDirectoryFile))
            {
                using StreamReader sr = File.OpenText(ToolPropertiesManager.GetInstance().GetToolProperties().installDirectoryFile);
                m_installDir = sr.ReadToEnd();
                m_steamCmdFile = $"{m_installDir}\\steamcmd\\steamcmd.exe";
            }
            else
            {
                m_installDir = string.Empty;
                m_steamCmdFile = string.Empty;
            }
        }

        public static FileIOManager GetInstance()
        {
            INSTANCE ??= new FileIOManager();
            return INSTANCE;
        }

        public string GetInstallDirectory() { return m_installDir; }
        public string GetSteamCmdFile() { return m_steamCmdFile; }
        public string GetAbsolutePathToServerFile() { return $"{m_installDir}{Constants.SERVER_JSON_STR}"; }

        public bool IsSteamCMDInstalled() { return File.Exists(m_steamCmdFile); }

        /// <summary>
        /// Write the available and enabled mods from the ListBoxes
        /// </summary>
        public void WriteModsDatabase()
        {
            var enabled        = ConfigurationManager.GetInstance().GetEnabledMods();
            var available      = ConfigurationManager.GetInstance().GetAvailableMods();
            List<Mod> combined = new();

            combined.AddRange(enabled);
            combined.AddRange(available);

            File.WriteAllText(
                ToolPropertiesManager.GetInstance().GetToolProperties().modDatabaseFile,
                Utilities.GetFormattedJsonString(combined, new JsonUtils.ModConverter())
            );
        }

        /// <summary>
        /// Read the Mods Database file
        /// This method also calls the AlphabetiseModListBox method so the ListBoxes are always 
        /// displaying the mods in alphabetical order.
        /// </summary>
        public void ReadModsDatabase()
        {
            Log.Information("FileIOManager - Reading mod database...");
            using StreamReader sr = File.OpenText(ToolPropertiesManager.GetInstance().GetToolProperties().modDatabaseFile);
            string json = sr.ReadToEnd().Trim();
            Mod[] loadedMods = JsonSerializer.Deserialize<Mod[]>(json)!;
            foreach (Mod mod in loadedMods)
            {
                Log.Information("FileIOManager - Loading mod {mod}...", mod.name);
                if (mod.version == null)
                {
                    Log.Information("FileIOManager - No version defined, defaulting to latest");
                    mod.version = "latest";
                }
                if (!ConfigurationManager.GetInstance().GetAvailableMods().Contains(mod))
                {
                    ConfigurationManager.GetInstance().GetAvailableMods().Add(mod);
                }
            }
            ConfigurationManager.GetInstance().AlphabetiseModLists();
        }

        public static void SaveModsListToFile()
        {
            using System.Windows.Forms.SaveFileDialog sfd = new();
            sfd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            sfd.Filter = "JSON (*.json)|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ConfigurationManager.GetInstance().CreateConfiguration();
                File.WriteAllText(sfd.FileName, ConfigurationManager.GetInstance().GetServerConfiguration().ModsAsJsonString());
            }
        }

        /// <summary>
        /// Save Configuration to JSON file
        /// </summary>
        public static void SaveConfigurationToFile()
        {
            using System.Windows.Forms.SaveFileDialog sfd = new();
            sfd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            sfd.Filter = "JSON (*.json)|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ConfigurationManager.GetInstance().CreateConfiguration();
                SaveConfigurationToFile(sfd.FileName);
            }
        }

        /// <summary>
        /// Save Configuration to JSON file
        /// </summary>
        /// <param name="path">File path to save to</param>
        /// <returns>True if file was saved successfully, false otherwise</returns>
        public static bool SaveConfigurationToFile(string path)
        {
            try
            {
                Log.Information("FileIOManager - Saving config to {path}", path);
                ConfigurationManager.GetInstance().CreateConfiguration();
                File.WriteAllText(path, ConfigurationManager.GetInstance().GetServerConfiguration().AsJsonString());
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save config to {path}", path);
                Utilities.DisplayErrorMessage($"An error occurred while trying to write server configuration.", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Load Configuration from JSON file
        /// </summary>
        public static void LoadConfigurationFromFile()
        {
            using System.Windows.Forms.OpenFileDialog ofd = new();
            ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            ofd.Filter = "JSON (*.json)|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                using StreamReader sr = File.OpenText(filePath);
                ConfigurationManager.GetInstance().PopulateServerConfiguration(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Load legacy Mod Database file (from <= 0.8.3) and convert to JSON format
        /// Note that this will replace the existing current-format mod
        /// database
        /// </summary>
        /// <param name="path">File path of legacy mod database</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool MigrateLegacyModDatabase(string path)
        {
            Log.Information("FileIOManager - Migrating legacy mod database...");
            using StreamReader sr   = File.OpenText(path);
            List<string> legacyMods = new(sr.ReadToEnd().Split(Environment.NewLine));
            foreach (string s in legacyMods)
            {
                string[] splitMod = s.Split(',');

                if (splitMod.Length < 2)
                {
                    Utilities.DisplayErrorMessage("Importing legacy mods failed.", "At least one legacy mod was in an invalid format.");
                    return false;
                }

                if (splitMod.Length > 2)
                {
                    string versString = splitMod[2].Trim();
                    if (versString != null && !versString.Equals("latest"))
                    {
                        ConfigurationManager.GetInstance()
                                            .GetAvailableMods()
                                            .Add(new Mod(splitMod[0].Trim(), splitMod[1].Trim(), splitMod[2].Trim()));
                    } else
                    {
                        ConfigurationManager.GetInstance()
                                            .GetAvailableMods()
                                            .Add(new Mod(splitMod[0].Trim(), splitMod[1].Trim()));
                    }
                }
            }
            GetInstance().WriteModsDatabase();
            if (DeleteFile(path))
            {
                MessageBox.Show("Legacy Mod Database successfully migrated");
            }
            return true;
        }

        /// <summary>
        /// Download Steam CMD
        /// </summary>
        public void DownloadSteamCMD()
        {
            string path = string.Empty;
            using FolderBrowserDialog fbd = new();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                Log.Information("FileIOManager - Downloading SteamCMD to {path}...", fbd.SelectedPath);
                m_installDir = fbd.SelectedPath;
                m_steamCmdFile = $"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe";
                File.WriteAllText(ToolPropertiesManager.GetInstance().GetToolProperties().installDirectoryFile, m_installDir);
            }

            using WebClient client = new();
            client.DownloadFileCompleted += (s, e) =>
            {
                if (File.Exists($"{m_installDir}\\steamcmd.zip"))
                {
                    Log.Information("FileIOManager - Extracting SteamCMD...");
                    ZipFile.ExtractToDirectory($"{m_installDir}\\steamcmd.zip", $"{m_installDir}\\steamcmd");
                }
            };
            client.DownloadFileAsync(
                new Uri($"{ToolPropertiesManager.GetInstance().GetToolProperties().steamCmdDownloadUrl}/steamcmd.zip"),
                    $"{m_installDir}\\steamcmd.zip");
        }

        /// <summary>
        /// Check our version against the version.txt file in the GitHub repository.
        /// Show a dialog prompting the user to update if we are out of date.
        /// If there is no internet connection, or this simply fails, 
        /// warn the user that we couldn't successfully check for updates.
        /// </summary>
        public static void CheckForUpdates()
        {
            Log.Information("FileIOManager - Checking for updates...");
            string latestVersionString;
            WebClient wc = new WebClient();

            // Add headers to impersonate a web browser. Some web sites 
            // will not respond correctly without these headers
            wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
            wc.Headers.Add("Accept", "*/*");
            wc.Headers.Add("Accept-Language", "en-gb,en;q=0.5");
            wc.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            // Disable caching to avoid retrieval of an old version number
            wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            try
            {
                latestVersionString = wc.DownloadString($"{ToolPropertiesManager.GetInstance().GetToolProperties().updateRepositoryUrl}/main/version.txt");

                var checkedVersion = new Version(latestVersionString);
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var result = checkedVersion.CompareTo(currentVersion);
                if (result > 0)
                {
                    Log.Information("FileIOManager - There is a new version of the tool available (current: {currentVer}, new: {newVer}", currentVersion, checkedVersion);
                    DialogResult dr = MessageBox.Show("There is an update available for the Arma Reforger Dedicated Server Tool." +
                        "\r\nWould you like to get the latest version now?\r\n\r\nOur version: " + currentVersion +
                        "\r\nLatest version: " + checkedVersion, "Arma Reforger Dedicated Server Tool - Update available", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Process.Start("explorer", $"{ToolPropertiesManager.GetInstance().GetToolProperties().releaseRepositoryUrl}");
                        Environment.Exit(0);
                    }
                }
            }
            catch (WebException e)
            {
                Log.Error(e, "FileIOManager - Failed to check for updates");
                Utilities.DisplayErrorMessage($"Unable to check for updates," +
                    " you may not be using the latest version of the Arma Reforger Dedicated Server Tool.\r\nPlease consider checking your internet connection.", e.Message);
            }
        }

        /// <summary>
        /// Check if Visual C++ Runtime is installed (required for the Arma Reforger server), 
        /// if not, displays a prompt to install it or closes the application
        /// </summary>
        public static void CheckForVCRedist()
        {
            bool installed = false;

            string registryKey = @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\X64";

            using RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);

            if (key != null)
            {
                object value = key.GetValue("Installed");
                installed = value != null && (int) value == 1;
            }

            if (!installed)
            {
                Log.Information("FileIOManager - Visual C++ Runtime was not found on your system. Arma Reforger Dedicated Server requires it to function");
                DialogResult dr = MessageBox.Show("Visual C++ Runtime was not found and is required for the server to start." +
                        "\r\nWould you like to install it?" +
                        "\r\n\r\nSelecting Yes will close the application and open your browser. Selecting No will simply close the application.", 
                            "Arma Reforger Dedicated Server Tool - Visual C++ Runtime not found", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    Process.Start("explorer", "https://aka.ms/vs/17/release/vc_redist.x64.exe");
                }
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Delete Server Files
        /// </summary>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public bool DeleteServerFiles()
        {
            string msg = 
                "You are about to delete SteamCMD and all Arma Reforger server files" + Environment.NewLine +
                "ALL files will be deleted in the path:" + Environment.NewLine +
                m_installDir + Environment.NewLine +
                "Do you want to continue?";

            DialogResult result = MessageBox.Show(msg, "Delete Server Files", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {
                Directory.Delete(m_installDir, true);
                m_installDir = string.Empty;
                DeleteFile(ToolPropertiesManager.GetInstance().GetToolProperties().installDirectoryFile);
                MessageBox.Show("Server files deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return !Directory.Exists(m_installDir);
            }

            return false;
        }

        /// <summary>
        /// Locate Server Files
        /// </summary>
        /// <returns>True if the Server and SteamCMD executables are found, false otherwise</returns>
        public bool LocateServerFiles()
        {
            string path = string.Empty;
            using FolderBrowserDialog fbd = new();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                if (File.Exists($"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe") &&
                    File.Exists($"{fbd.SelectedPath}\\arma_reforger\\ArmaReforgerServer.exe"))
                {
                    m_installDir = fbd.SelectedPath;
                    m_steamCmdFile = $"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe";
                    File.WriteAllText(ToolPropertiesManager.GetInstance().GetToolProperties().installDirectoryFile, m_installDir);
                    return true;
                }
                else
                {
                    MessageBox.Show("Arma Reforger Server Files could not be located." +
                        "\r\nPlease confirm the chosen path or download the files to start.", "Warning", MessageBoxButtons.OK);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Convenience method for deleting the 'server.json' file and recreating it
        /// </summary>
        /// <returns>True if the operation completed successfully, false otherwise</returns>
        public bool ResetServerFile()
        {
            return DeleteFile(GetAbsolutePathToServerFile());
        }

        /// <summary>
        /// Convenience method for wrapping the File.Delete method
        /// Handles errors and will do nothing if the file does not exist
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if deletion successful, false otherwise</returns>
        public static bool DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                Utilities.DisplayErrorMessage($"An error occurred while attempting to delete file '{path}'.", ex.Message);
                return false;
            }
        }
    }
}
