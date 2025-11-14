/******************************************************************************
 * File Name:    FileIOManager.cs
 * Project:      Longbow
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
using Longbow.Managers;

namespace ReforgerServerApp.Managers
{
  /// <summary>
  /// Manager of all things Files and IO
  /// </summary>
  internal class FileIOManager
  {
    private static FileIOManager? INSTANCE;
    private readonly string m_legacyModDatabaseFile = "./mod_database.txt";
    private string m_steamCmdFile;
    private string m_installDir;
    private string m_savesPath;
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

      if (SavedStateManager.GetInstance().GetSavedState().serverLocation != null)
      {
        m_installDir = SavedStateManager.GetInstance().GetSavedState().serverLocation;
        m_steamCmdFile = $"{m_installDir}\\steamcmd\\steamcmd.exe";
        m_savesPath = $"{m_installDir}\\saves\\profile\\.save\\game";
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
      var enabled = ConfigurationManager.GetInstance().GetEnabledMods();
      var available = ConfigurationManager.GetInstance().GetAvailableMods();
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

    /// <summary>
    /// Save Enabled Mods List to JSON file
    /// </summary>
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
    /// Load Mods List JSON from file and populate enabled mods list.
    /// Mods currently in the enabled mods list will be moved back to available mods first to ensure they're not lost.
    /// If a mod in the mod list is already in the available mods list, it will be moved from there so no duplicates exist.
    /// </summary>
    public static void LoadModsListFromFile()
    {
      using System.Windows.Forms.OpenFileDialog ofd = new();
      ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
      ofd.Filter = "JSON (*.json)|*.json";
      if (ofd.ShowDialog() == DialogResult.OK)
      {
        string filePath = ofd.FileName;
        using StreamReader sr = File.OpenText(filePath);
        string modsJsonString = sr.ReadToEnd();
        if (modsJsonString == null)
        {
          return;
        }
        JsonSerializerOptions options = new();
        options.Converters.Add(new JsonUtils.ModConverter());
        try
        {
          List<Mod> modsToImport = JsonSerializer.Deserialize<List<Mod>>(modsJsonString, options);
          if (modsToImport != null && modsToImport.Count > 0)
          {
            ConfigurationManager.GetInstance().ImportModsList(modsToImport);
          }
        }
        catch (JsonException je)
        {
          Utilities.DisplayErrorMessage("Failed to import mods list", $"Failed to import mods list, the mods list may be malformed.\r\n\r\n{je.Message}");
        }
      }
    }

    /// <summary>
    /// Write the State file to disk
    /// </summary>
    public void WriteStateFile()
    {
      File.WriteAllText(
        SavedStateManager.GetInstance().GetSavedStateFile(),
        SavedStateManager.GetInstance().GetSavedState().AsJsonString());
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
      using StreamReader sr = File.OpenText(path);
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
          }
          else
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
    public async Task DownloadSteamCMD()
    {
      string path = string.Empty;
      using FolderBrowserDialog fbd = new();
      DialogResult result = fbd.ShowDialog();
      if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
      {
        Log.Information("FileIOManager - Downloading SteamCMD to {path}...", fbd.SelectedPath);
        m_installDir = fbd.SelectedPath;
        m_steamCmdFile = $"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe";
        m_savesPath = $"{m_installDir}\\saves\\profile\\.save\\game";
        SavedStateManager.GetInstance().GetSavedState().serverLocation = m_installDir;
      }

      string steamCmdUrl = $"{ToolPropertiesManager.GetInstance().GetToolProperties().steamCmdDownloadUrl}/steamcmd.zip";
      string zipFilePath = Path.Combine(m_installDir, "steamcmd.zip");
      string extractPath = Path.Combine(m_installDir, "steamcmd");

      try
      {
        using HttpClient client = new();
        Log.Debug("FileIOManager - Downloading SteamCMD...");
        byte[] fileBytes = await client.GetByteArrayAsync(steamCmdUrl);
        await File.WriteAllBytesAsync(zipFilePath, fileBytes);

        if (File.Exists(zipFilePath))
        {
          Log.Debug("FileIOManager - Extracting SteamCMD...");
          ZipFile.ExtractToDirectory(zipFilePath, extractPath);
          DeleteFile(zipFilePath);
        }
        return;
      }
      catch (Exception ex)
      {
        Utilities.DisplayErrorMessage("Failed to download or extract SteamCMD.", $"Failed to download or extract SteamCMD: {ex.Message}");
        Log.Error($"FileIOManager - Failed to download or extract SteamCMD: {ex.Message}");
      }
    }

    /// <summary>
    /// Install the NoBackendScenarioLoader mod into the addons directory
    /// </summary>
    public void InstallNoBackendScenarioLoader()
    {
      ZipFile.ExtractToDirectory("Resources\\NoBackendScenarioLoader_6324F7124A9768FB.zip",
          $"{m_installDir}\\addons", true);
    }

    /// <summary>
    /// Check our version against the version.txt file in the GitHub repository.
    /// Show a dialog prompting the user to update if we are out of date.
    /// If there is no internet connection, or this simply fails, 
    /// warn the user that we couldn't successfully check for updates.
    /// </summary>
    public static async Task CheckForUpdates()
    {
      Log.Information("FileIOManager - Checking for updates...");
      string latestVersionString;

      using HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
      client.DefaultRequestHeaders.Add("Accept", "*/*");
      client.DefaultRequestHeaders.Add("Accept-Language", "en-gb,en;q=0.5");
      client.DefaultRequestHeaders.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");

      try
      {
        string updateUrl = $"{ToolPropertiesManager.GetInstance().GetToolProperties().updateRepositoryUrl}/main/version.txt";
        latestVersionString = await client.GetStringAsync(updateUrl);

        var checkedVersion = new Version(latestVersionString.Trim());
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var result = checkedVersion.CompareTo(currentVersion);

        if (result > 0)
        {
          Log.Information("FileIOManager - There is a new version of the tool available (current: {currentVer}, new: {newVer}", currentVersion, checkedVersion);
          DialogResult dr = MessageBox.Show(
          $"There is an update available for the Longbow.\r\n" +
          $"Would you like to get the latest version now?\r\n\r\n" +
          $"Our version: {currentVersion}\r\nLatest version: {checkedVersion}",
          "Longbow - Update available",
          MessageBoxButtons.YesNo);

          if (dr == DialogResult.Yes)
          {
            Process.Start(new ProcessStartInfo
            {
              FileName = ToolPropertiesManager.GetInstance().GetToolProperties().releaseRepositoryUrl,
              UseShellExecute = true
            });
            Environment.Exit(0);
          }
        }
      }
      catch (HttpRequestException e)
      {
        Log.Error(e, "FileIOManager - Failed to check for updates");
        Utilities.DisplayErrorMessage(
            "Unable to check for updates, you may not be using the latest version of the Longbow.\r\n" +
            "Please consider checking your internet connection.",
            e.Message);
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
                    "Longbow - Visual C++ Runtime not found", MessageBoxButtons.YesNo);
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
        DeleteFile(SavedStateManager.GetInstance().GetSavedState().serverLocation);
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
          m_savesPath = $"{m_installDir}\\saves\\profile\\.save\\game";
          SavedStateManager.GetInstance().GetSavedState().serverLocation = m_installDir;
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

    public string GetSavesPath()
    {
      return m_savesPath;
    }

    /// <summary>
    /// Get a dictionary of saved games, keyed on their name and their value being their paths
    /// </summary>
    /// <returns>Dictionary of Saved Games</returns>
    public Dictionary<string, string> GetSavedGames()
    {
      if (!Directory.Exists(m_savesPath))
      {
        Directory.CreateDirectory(m_savesPath);
        Log.Debug("FileIOManager - Saves path doesn't exist, created it...");
      }

      var savedGames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      foreach (var path in Directory.EnumerateFiles(m_savesPath, "*.json", SearchOption.AllDirectories))
      {
        var name = Path.GetFileNameWithoutExtension(path);

        // salta il file speciale ".LatestSave"
        if (string.Equals(name, ".LatestSave", StringComparison.OrdinalIgnoreCase))
          continue;

        // in caso di nomi duplicati in cartelle diverse, l'ultimo incontrato sovrascrive il precedente
        savedGames[name] = path;
      }

      return savedGames;
    }

    /// <summary>
    /// Rename File given a full path to original and new file names
    /// </summary>
    /// <param name="origName"></param>
    /// <param name="newName"></param>
    /// <returns>New file path as string if successful, otherwise returns original file path</returns>
    public string RenameFile(string origName, string newName)
    {
      try
      {
        if (File.Exists(origName))
        {
          File.Move(origName, newName);
          return newName;
        }
        return origName;
      }
      catch (IOException ex)
      {
        Log.Error($"FileIOManager - An error occurred when attempting to rename file {origName} - {ex.Message}");
        Utilities.DisplayErrorMessage($"Failed to rename {origName}", ex.Message);
        return origName;
      }
    }
  }
}
