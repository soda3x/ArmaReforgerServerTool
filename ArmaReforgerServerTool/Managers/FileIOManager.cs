using ReforgerServerApp.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReforgerServerApp.Managers
{
    /// <summary>
    /// Manager of all things Files and IO
    /// </summary>
    internal class FileIOManager
    {
        private static FileIOManager?   m_instance;
        private readonly string         m_modDatabaseFile  = "./mod_database.txt";
        private readonly string         m_installDirFile   = "./install_directory.txt";
        private string                  m_steamCmdFile;
        private string                  m_installDir;
        private FileIOManager()
        {
            if (File.Exists(m_modDatabaseFile))
            {
                ReadModsDatabase();
            }

            if (File.Exists(m_installDirFile))
            {
                using StreamReader sr = File.OpenText(m_installDirFile);
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
            m_instance ??= new FileIOManager();
            return m_instance;
        }

        public string GetInstallDirectory() { return m_installDir; }
        public string GetModDatabaseFile() { return m_modDatabaseFile; }
        public string GetSteamCmdFile() {  return m_steamCmdFile; }
        public string GetInstallDirFile() {  return m_installDirFile; }

        public bool IsSteamCMDInstalled() {  return File.Exists(m_steamCmdFile); }

        /// <summary>
        /// Write the available and enabled mods from the ListBoxes to the mod_database.txt file so that on next open, 
        /// they can be imported without the need for re-adding.
        /// The mods are stored in a comma separated format of "MOD_ID,MOD_NAME" on each line.
        /// </summary>
        public void WriteModsDatabase()
        {
            var enabled        = ConfigurationManager.GetInstance().GetEnabledMods();
            var available      = ConfigurationManager.GetInstance().GetAvailableMods();
            List<Mod> combined = new();

            combined.AddRange(enabled);
            combined.AddRange(available);

            File.WriteAllText(m_modDatabaseFile, JsonSerializer.Serialize(combined));
        }

        /// <summary>
        /// Read the mods_database.txt file.
        /// This method also calls the AlphabetiseModListBox method so the ListBoxes are always 
        /// displaying the mods in alphabetical order.
        /// </summary>
        public void ReadModsDatabase()
        {
            using StreamReader sr = File.OpenText(m_modDatabaseFile);
            string json = sr.ReadToEnd().Trim();
            List<Mod> loadedMods = JsonSerializer.Deserialize<List<Mod>>(json)!;
            foreach (Mod mod in loadedMods)
            {
                if (!ConfigurationManager.GetInstance().GetAvailableMods().Contains(mod))
                {
                    ConfigurationManager.GetInstance().GetAvailableMods().Add(mod);
                }
            }
            ConfigurationManager.GetInstance().AlphabetiseModLists();
        }

        /// <summary>
        /// Save Configuration to JSON file
        /// </summary>
        public static void SaveConfigurationToFile()
        {
            using SaveFileDialog sfd = new();
            sfd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            sfd.Filter = "JSON (*.json)|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ConfigurationManager.GetInstance().CreateConfiguration();
                File.WriteAllText(sfd.FileName, ConfigurationManager.GetInstance().GetServerConfiguration().AsJsonString());
            }
        }

        /// <summary>
        /// Load Configuration from JSON file
        /// </summary>
        public static void LoadConfigurationFromFile()
        {
            using OpenFileDialog ofd = new();
            ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            ofd.Filter = "Properties files (*.json)|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                using StreamReader sr = File.OpenText(filePath);
                ConfigurationManager.GetInstance().PopulateServerConfiguration(sr.ReadToEnd());
            }
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
                m_installDir = fbd.SelectedPath;
                m_steamCmdFile = $"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe";
                File.WriteAllText(m_installDirFile, m_installDir);
            }

            using WebClient client = new();
            client.DownloadFileCompleted += (s, e) =>
            {
                if (File.Exists($"{m_installDir}\\steamcmd.zip"))
                {
                    ZipFile.ExtractToDirectory($"{m_installDir}\\steamcmd.zip", $"{m_installDir}\\steamcmd");
                }
            };
            client.DownloadFileAsync(new Uri("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip"), $"{m_installDir}\\steamcmd.zip");
        }

        /// <summary>
        /// Check our version against the version.txt file in the GitHub repository.
        /// Show a dialog prompting the user to update if we are out of date.
        /// If there is no internet connection, or this simply fails, 
        /// warn the user that we couldn't successfully check for updates.
        /// </summary>
        public static void CheckForUpdates()
        {
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
                latestVersionString = wc.DownloadString("https://raw.githubusercontent.com/soda3x/ArmaReforgerServerTool/main/version.txt");

                var checkedVersion = new Version(latestVersionString);
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var result = checkedVersion.CompareTo(currentVersion);
                if (result > 0)
                {
                    DialogResult dr = MessageBox.Show("There is an update available for the Arma Reforger Dedicated Server Tool." +
                        "\r\nWould you like to get the latest version now?\r\n\r\nOur version: " + currentVersion +
                        "\r\nLatest version: " + checkedVersion, "Arma Reforger Dedicated Server Tool - Update available", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Process.Start("explorer", "https://github.com/soda3x/ArmaReforgerServerTool/releases");
                    }
                }
            }
            catch (WebException)
            {
                MessageBox.Show("Unable to check for updates," +
                    " you may not be using the latest version of the Arma Reforger Dedicated Server Tool.", "Arma Reforger Dedicated Server Tool");
            }
        }

        /// <summary>
        /// Delete Server Files
        /// </summary>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public bool DeleteServerFiles()
        {
            DialogResult result = MessageBox.Show("You are about to delete SteamCMD and all Arma Reforger server files," +
                " are you sure you would like to do this?", "Warning", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Directory.Delete(m_installDir, true);
                m_installDir = string.Empty;
                File.Delete(m_installDirFile);
                MessageBox.Show("Server files deleted.", "Warning", MessageBoxButtons.OK);
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
                    File.WriteAllText(m_installDirFile, m_installDir);
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
    }
}
