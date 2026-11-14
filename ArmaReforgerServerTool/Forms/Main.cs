/******************************************************************************
 * File Name:    Main.cs
 * Project:      Longbow
 * Description:  This is the Main Form
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Longbow.Components.ui;
using Longbow.Forms;
using Longbow.Managers;
using Longbow.Models;
using Longbow.Utils;
using ReforgerServerApp.Components;
using ReforgerServerApp.Managers;
using ReforgerServerApp.Models;
using ReforgerServerApp.Utils;
using Serilog;
using System.ComponentModel;

namespace ReforgerServerApp
{
  public partial class Main : Form
  {
    private BindingSource m_availableModsBindingSource;
    private BindingSource m_enabledModsBindingSource;
    private ServerStatusParser m_serverStatusParser;
    private List<LanguagePackMeta> m_availableLanguages = [];

    public Main()
    {
      InitializeComponent();

      CreateServerParameterControls();
      CreateAdvancedServerParameterControls();

      serverRunningLabel.Text = string.Empty;

      modsSearchTB.PlaceholderText = Strings.Get(Constants.MAIN_MODS_SEARCH_PLACEHOLDER_STR);

      ProcessManager.GetInstance().UpdateGuiControlsEvent += HandleUpdateGuiControlsEvent;
      ProcessManager.GetInstance().UpdateSteamCmdLogEvent += HandleUpdateSteamCmdLogEvent;
      ProcessManager.GetInstance().UpdateServerStatusEvent += HandleServerStatusEvent;
      ConfigurationManager.GetInstance().UpdateScenarioIdFromLoadedConfigEvent += HandleUpdateScenarioIdFromLoadedConfigEvent;
      SavedStateManager.GetInstance().SetMainReference(this);

      useUpnp.Checked = SavedStateManager.GetInstance().GetLoadedAdvancedSettings().GetValueOrDefault("useUpnp", SavedState.DEFAULT_USE_UPNP).Enabled;
      NetworkManager.GetInstance().useUPnP = useUpnp.Checked;

      useExperimentalCheckBox.Checked = SavedStateManager.GetInstance().GetLoadedAdvancedSettings().GetValueOrDefault("useExperimental", SavedState.DEFAULT_USE_EXPERIMENTAL).Enabled;

      keepServerUpdated.Checked = SavedStateManager.GetInstance().GetLoadedAdvancedSettings().GetValueOrDefault("keepServerUpdated", SavedState.DEFAULT_KEEP_SERVER_UPDATED).Enabled;
      ProcessManager.GetInstance().KeepServerUpdated = keepServerUpdated.Checked;

      // Create tooltips
      CreateTooltips();

      loadedScenarioLabel.Text = Strings.Get(Constants.MAIN_NO_SCENARIO_CHOSEN_MESSAGE_STR);

      UpdateSteamCmdInstallStatus();

      m_serverStatusParser = new();
      m_serverStatusParser.UpdateServerStatus += HandleServerStatusEvent;

      HandleServerStatusEvent(this, new());


      m_availableModsBindingSource = new()
      {
        DataSource = ConfigurationManager.GetInstance().GetAvailableMods()
      };

      m_enabledModsBindingSource = new()
      {
        DataSource = ConfigurationManager.GetInstance().GetEnabledMods()
      };

      ResetModFilters();

      ConfigurationManager.GetInstance().AlphabetiseModLists();

      if (ToolPropertiesManager.GetInstance().GetToolProperties().checkForUpdatesOnStartup)
      {
        _ = FileIOManager.CheckForUpdates();
      }
      else
      {
        Log.Information("Main - Skipping update check, checkForUpdatesOnStartup is false in properties.json");
      }

      FileIOManager.CheckForVCRedist();

      // FIXME: Disable the Save Manager for now
      loadSaveGameBtn.Visible = false;
      loadSaveGameBtn.Enabled = false;
      ToolTip loadsaveGameBtnTooltip = new();
      loadsaveGameBtnTooltip.SetToolTip(loadSaveGameBtn, Strings.Get(Constants.MAIN_LOAD_SAVE_GAME_TOOLTIP_STR));

      copyAddressBtn.Enabled = false;
      copyRconAddressBtn.Enabled = false;
      copyJoinCodeBtn.Enabled = false;

      logLevelComboBox.SelectedIndex = 0;

      InitialiseLanguageComboBox();

      chartFps.Units = "FPS";
      chartMem.Units = "GB";

      steamCmdLog.ScrollBars = ScrollBars.Vertical;

      steamCmdLog.ReadOnly = true;

      string lastConfigPath = SavedStateManager.GetInstance().GetSavedState().lastLoadedConfig;

      if (!string.IsNullOrWhiteSpace(lastConfigPath))
      {
        if (!FileIOManager.LoadConfigurationFromFile(lastConfigPath))
        {
          Utilities.DisplayErrorMessage(Strings.Get(Constants.MAIN_FAILED_TO_OPEN_LAST_CONFIG_STR),
              Strings.Get(Constants.MAIN_FAILED_TO_OPEN_LAST_CONFIG_DETAIL_STR, ("path", lastConfigPath)));
          SavedStateManager.GetInstance().GetSavedState().lastLoadedConfig = SavedState.Default.lastLoadedConfig;
        }
        else
        {
          Text = $"Longbow Arma Dedicated Server Tool - {lastConfigPath}";
        }
      }

      ThemeManager.GetInstance().ConfigureTheme(this);
      LocalisationManager.GetInstance().ApplyStrings(this, translationKeyProvider1);
    }

    /// <summary>
    /// Initialise tool tips for certain UI elements.
    /// </summary>
    private void CreateTooltips()
    {
      ToolTip enableAllModsToolTip = new();
      enableAllModsToolTip.SetToolTip(enableAllModsBtn, Strings.Get(Constants.ENABLE_ALL_MODS_STR));
      ToolTip disableAllModsToolTip = new();
      disableAllModsToolTip.SetToolTip(disableAllModsBtn, Strings.Get(Constants.DISABLE_ALL_MODS_STR));
      ToolTip enableModToolTip = new();
      enableModToolTip.SetToolTip(addToEnabledBtn, Strings.Get(Constants.ENABLE_MOD_STR));
      ToolTip disableModToolTip = new();
      disableModToolTip.SetToolTip(removeFromEnabledBtn, Strings.Get(Constants.DISABLE_MOD_STR));
      ToolTip useUpnpToolTip = new();
      useUpnpToolTip.SetToolTip(useUpnp, Strings.Get(Constants.USE_UPNP_STR));
      ToolTip exportModsToolTip = new();
      exportModsToolTip.SetToolTip(exportModsBtn, Strings.Get(Constants.EXPORT_MODS_STR));
      ToolTip importModsToolTip = new();
      exportModsToolTip.SetToolTip(importModsBtn, Strings.Get(Constants.IMPORT_MODS_STR));
      ToolTip startServerToolTip = new();
      startServerToolTip.SetToolTip(startServerBtn, Strings.Get(Constants.START_SERVER_STR));
      ToolTip downloadToolTip = new();
      downloadToolTip.SetToolTip(downloadSteamCmdBtn, Strings.Get(Constants.DOWNLOAD_SERVER_FILES_STR));
      ToolTip locateServerToolTip = new();
      locateServerToolTip.SetToolTip(locateServerFilesBtn, Strings.Get(Constants.LOCATE_SERVER_FILES_STR));
      ToolTip deleteServerToolTip = new();
      deleteServerToolTip.SetToolTip(deleteServerFilesBtn, Strings.Get(Constants.DELETE_SERVER_FILES_STR));
      ToolTip useExperimentalToolTip = new();
      useExperimentalToolTip.SetToolTip(useExperimentalCheckBox, Strings.Get(Constants.USE_EXPERIMENTAL_STR));
      ToolTip keepServerUpdatedToolTip = new();
      keepServerUpdatedToolTip.SetToolTip(keepServerUpdated, Strings.Get(Constants.KEEP_SERVER_UPDATED_STR));
    }

    /// <summary>
    /// Populates the language combo box from every language pack LocalisationManager can find
    /// (embedded + on-disk), and selects whichever one is currently active in properties.json.
    /// </summary>
    private void InitialiseLanguageComboBox()
    {
      m_availableLanguages = LocalisationManager.GetInstance().AvailableLanguages();

      languageComboBox.Items.Clear();
      languageComboBox.Items.AddRange([.. m_availableLanguages.Select(l => (object) $"{l.NativeName} ({l.Code})")]);

      string currentCode = ToolPropertiesManager.GetInstance().GetToolProperties().language;
      int currentIndex = m_availableLanguages.FindIndex(l => l.Code == currentCode);
      languageComboBox.SelectedIndex = currentIndex >= 0 ? currentIndex : 0;
    }

    /// <summary>
    /// Persists the chosen language to properties.json. Doesn't apply it live - the language
    /// is only loaded at startup (see i18n-system.md section 5), so this just tells the user
    /// a restart is needed.
    /// </summary>
    private void LanguageComboBoxSelectedIndexChanged(object sender, EventArgs e)
    {
      if (languageComboBox.SelectedIndex < 0 || languageComboBox.SelectedIndex >= m_availableLanguages.Count)
      {
        return;
      }

      string selectedCode = m_availableLanguages[languageComboBox.SelectedIndex].Code;
      if (selectedCode == ToolPropertiesManager.GetInstance().GetToolProperties().language)
      {
        return;
      }

      ToolPropertiesManager.GetInstance().GetToolProperties().language = selectedCode;
      ToolPropertiesManager.GetInstance().SaveToolProperties();

      MessageBox.Show(
          Strings.Get(Constants.LANGUAGE_CHANGE_RESTART_REQUIRED_STR),
          Strings.Get(Constants.INFO_MESSAGEBOX_TITLE_STR));
    }

    /// <summary>
    /// This method is used to control the state of the controls used to Download SteamCMD and start the server.
    /// If SteamCMD is detected, The message telling the user to Download SteamCMD is hidden, 
    /// the Download button is disabled and the Start Server button is enabled.
    /// </summary>
    private void UpdateSteamCmdInstallStatus()
    {
      if (steamCmdAlert.InvokeRequired)
      {
        steamCmdAlert.Invoke(new Action(() => UpdateSteamCmdInstallStatus()));
      }
      else
      {
        if (FileIOManager.GetInstance().IsSteamCMDInstalled())
        {
          steamCmdAlert.Text = Strings.Get(Constants.MAIN_STEAMCMD_FOUND_MESSAGE_STR, ("installDir", FileIOManager.GetInstance().GetInstallDirectory()));
          downloadSteamCmdBtn.Enabled = false;
          startServerBtn.Enabled = true;
          deleteServerFilesBtn.Enabled = true;
          loadSaveGameBtn.Enabled = true;
        }
        else
        {
          steamCmdAlert.Text = Strings.Get(Constants.MAIN_STEAMCMD_NOT_FOUND_MESSAGE_STR);
          startServerBtn.Enabled = false;
          downloadSteamCmdBtn.Enabled = true;
          deleteServerFilesBtn.Enabled = false;
          loadSaveGameBtn.Enabled = false;
        }
      }
    }

    /// <summary>
    /// Get the Enabled Mods ListBox
    /// </summary>
    /// <returns>enabledMods ListBox</returns>
    public FluentListBox GetEnabledModsList()
    {
      return enabledMods;
    }

    /// <summary>
    /// Get the Available Mods ListBox
    /// </summary>
    /// <returns>availableMods ListBox</returns>
    public FluentListBox GetAvailableModsList()
    {
      return availableMods;
    }

    /// <summary>
    /// Show the Mod Dialog when the "Add Mod" button is pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddModBtnPressed(object sender, EventArgs e)
    {
      AddModDialog addModDialog = new(this);
      addModDialog.ShowDialog();
    }

    /// <summary>
    /// Event Handler for when the Selected Mod changes in the Available Mods
    /// list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AvailableModsSelectedIndexChanged(object sender, EventArgs e)
    {
      // If the server is running, we don't want to be able to edit / remove mods
      if (string.IsNullOrWhiteSpace(serverRunningLabel.Text))
      {
        editModBtn.Enabled = availableMods.SelectedItem != null;
        removeModBtn.Enabled = availableMods.SelectedItem != null;
      }
    }

    /// <summary>
    /// Event Handler for when the Edit Mod button is pressed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditModBtnPressed(object sender, EventArgs e)
    {
      if (availableMods.SelectedItem != null)
      {
        AddModDialog addModDialog = new(this, (Mod)availableMods.SelectedItem, availableMods.SelectedIndex);
        addModDialog.ShowDialog();
      }
    }

    /// <summary>
    /// Remove the selected mod(s) from the Available Mods ListBox when the "Remove Mod" button is pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveSelectedModBtnPressed(object sender, EventArgs e)
    {
      Mod[] modsToDelete = new Mod[GetAvailableModsList().SelectedItems.Count];
      GetAvailableModsList().SelectedItems.CopyTo(modsToDelete, 0);

      var availableMods = ConfigurationManager.GetInstance().GetAvailableMods();
      bool hasDeletedAtLeastOne = false;
      GetAvailableModsList().BeginUpdate();
      foreach (Mod mod in modsToDelete)
      {
        if (availableMods.Remove(mod))
          hasDeletedAtLeastOne = true;
      }

      if (hasDeletedAtLeastOne)
      {
        FileIOManager.GetInstance().WriteModsDatabase();
      }
      GetAvailableModsList().EndUpdate();
    }

    /// <summary>
    /// When the "Add to Enabled Mods" button (which currently looks like '>') is pressed, 
    /// remove the entry from the Available Mods ListBox and add the entry to the Enabled Mods ListBox.
    /// This method also calls the AlphabetiseModListBox method so the ListBoxes are always 
    /// displaying the mods in alphabetical order.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddToEnabledModsBtnPressed(object sender, EventArgs e)
    {
      Mod[] modsToMove = new Mod[GetAvailableModsList().SelectedItems.Count];
      GetAvailableModsList().BeginUpdate();
      GetEnabledModsList().BeginUpdate();
      GetAvailableModsList().SelectedItems.CopyTo(modsToMove, 0);
      foreach (Mod mod in modsToMove)
      {
        // Move mod from Available Mods -> Enabled Mods
        ConfigurationManager.MoveMod(mod, ConfigurationManager.GetInstance().GetAvailableMods(),
                                        ConfigurationManager.GetInstance().GetEnabledMods());
      }
      ConfigurationManager.GetInstance().AlphabetiseModLists();
      ResetModFilters();
      GetAvailableModsList().EndUpdate();
      GetEnabledModsList().EndUpdate();
    }

    /// <summary>
    /// When the "Remove From Enabled Mods" button (which currently looks like '<') is pressed, 
    /// remove the entry from the Enabled Mods ListBox and add the entry to the Available Mods ListBox.
    /// This method also calls the AlphabetiseModListBox method so the ListBoxes are always 
    /// displaying the mods in alphabetical order.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemovedFromEnabledModsBtnPressed(object sender, EventArgs e)
    {
      Mod[] modsToMove = new Mod[GetEnabledModsList().SelectedItems.Count];
      GetAvailableModsList().BeginUpdate();
      GetEnabledModsList().BeginUpdate();
      GetEnabledModsList().SelectedItems.CopyTo(modsToMove, 0);
      foreach (Mod mod in modsToMove)
      {
        // Move mod from Enabled Mods -> Available Mods
        ConfigurationManager.MoveMod(mod, ConfigurationManager.GetInstance().GetEnabledMods(),
                                        ConfigurationManager.GetInstance().GetAvailableMods());
      }
      ConfigurationManager.GetInstance().AlphabetiseModLists();
      ResetModFilters();
      GetAvailableModsList().EndUpdate();
      GetEnabledModsList().EndUpdate();
    }

    /// <summary>
    /// Event handler for when the Mod Position Up button is pressed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MoveEnabledModPositionUpBtnPressed(object sender, EventArgs e)
    {
      if ((Mod) GetEnabledModsList().SelectedItem != null)
      {
        GetEnabledModsList().BeginUpdate();
        Mod m = (Mod)GetEnabledModsList().SelectedItem;

        // Set move backward to true as moving position 'up' actually means moving the mod earlier in the list
        Utilities.MoveItem(ConfigurationManager.GetInstance().GetEnabledMods(), m, true);

        // Re-select the mod so we can do multiple moves in a row if we like
        GetEnabledModsList().SelectedItems.Clear();
        GetEnabledModsList().SelectedItem = m;
        GetEnabledModsList().EndUpdate();
      }
    }

    /// <summary>
    /// Event handler for when the Mod Position Down button is pressed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MoveEnabledModPositionDownBtnPressed(object sender, EventArgs e)
    {
      if ((Mod) GetEnabledModsList().SelectedItem != null)
      {
        GetEnabledModsList().BeginUpdate();
        Mod m = (Mod)GetEnabledModsList().SelectedItem;

        // Move forward is the default, this will mean moving the mod later in the list
        Utilities.MoveItem(ConfigurationManager.GetInstance().GetEnabledMods(), m);

        // Re-select the mod so we can do multiple moves in a row if we like
        GetEnabledModsList().SelectedItems.Clear();
        GetEnabledModsList().SelectedItem = m;
        GetEnabledModsList().EndUpdate();
      }
    }

    /// <summary>
    /// Save the server settings to a JSON file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveSettingsToFileBtnPressed(object sender, EventArgs e)
    {
      FileIOManager.SaveConfigurationToFile();
    }

    /// <summary>
    /// Load the server settings from a JSON file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadSettingsFromFileBtnPressed(object sender, EventArgs e)
    {
      ResetModFilters();
      FileIOManager.LoadConfigurationFromFile();
    }

    /// <summary>
    /// Handler for the "Download" button under the "Server Management" tab.
    /// This method will allow the user to pick a destination for SteamCMD and the Arma Reforger 
    /// Server files before downloading SteamCMD in the chosen directory.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadSteamCmdBtnPressed(object sender, EventArgs e)
    {
      Task steamCmdTask = FileIOManager.GetInstance().DownloadSteamCMD();
      steamCmdTask.ContinueWith(t =>
      {
        UpdateSteamCmdInstallStatus();
      });
      FileIOManager.GetInstance().InstallNoBackendScenarioLoader();
    }

    /// <summary>
    /// This is the handler for the Start Server Button. This is also used for the automatic server restart functionality.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartServerBtnPressed(object sender, EventArgs e)
    {
      AdvancedServerParameterTime? autoRestartDaily =
          ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary()["autoRestartDaily"] as AdvancedServerParameterTime;

      if (autoRestartDaily == null)
      {
        Log.Error("Main - Failed to start server due to issues with auto restart logic. Cannot continue.");
        return;
      }
      // If we are starting the server for the first time and using the automatic restart functionality, configure the timer
      if (autoRestartDaily.Checked() && !ProcessManager.GetInstance().IsServerUsingTimer())
      {
        CreateLaunchArguments();
        ProcessManager.GetInstance().ConfigureAutomaticRestartTask();
      }

      // The user is turning the server off manually
      else if (autoRestartDaily.Checked() && ProcessManager.GetInstance().IsServerUsingTimer())
      {
        ProcessManager.GetInstance().CancelAutomaticRestartTask();
      }

      // User just normally pressed the button
      else if (!autoRestartDaily.Checked() && !ProcessManager.GetInstance().IsServerUsingTimer())
      {
        CreateLaunchArguments();
        ProcessManager.GetInstance().StartStopServer();
      }
    }

    /// <summary>
    /// Handler for the Enable All Mods Button (displayed as '>>' on the GUI).
    /// Adds all mods from the Available Mods list to the Enabled Mods list and then alphabetises the order.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EnableAllModsBtnPressed(object sender, EventArgs e)
    {
      foreach (Mod m in ConfigurationManager.GetInstance().GetAvailableMods())
      {
        if (!ConfigurationManager.GetInstance().GetEnabledMods().Contains(m))
        {
          ConfigurationManager.GetInstance().GetEnabledMods().Add(new(m));
        }
      }
      ConfigurationManager.GetInstance().GetAvailableMods().Clear();
      ConfigurationManager.GetInstance().AlphabetiseModLists();
      ResetModFilters();
    }

    /// <summary>
    /// Handler for the Disable All Mods Button (displayed as '<<' on the GUI).
    /// Adds all mods from the Enabled Mods list to the Available Mods list and then alphabetises the order.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisableAllModsBtnPressed(object sender, EventArgs e)
    {
      foreach (Mod m in ConfigurationManager.GetInstance().GetEnabledMods())
      {
        if (!ConfigurationManager.GetInstance().GetAvailableMods().Contains(m))
        {
          ConfigurationManager.GetInstance().GetAvailableMods().Add(new(m));
        }
      }
      ConfigurationManager.GetInstance().GetEnabledMods().Clear();
      ConfigurationManager.GetInstance().AlphabetiseModLists();
      ResetModFilters();
    }

    /// <summary>
    /// Handler for the about button, displays information about the program itself.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AboutBtnPressed(object sender, EventArgs e)
    {
      AboutBox ab = new();
      ab.ShowDialog();
    }

    /// <summary>
    /// Handler for the "Delete Server Files" button.
    /// Deletes all server files and references to them.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteServerFilesBtnPressed(object sender, EventArgs e)
    {
      if (FileIOManager.GetInstance().DeleteServerFiles())
      {
        UpdateSteamCmdInstallStatus();
      }
    }

    /// <summary>
    /// Handler for the "Locate Server Files" button.
    /// Allows the user to navigate to a directory and set the Server Files installation directory without downloading the files.
    /// Useful for moving installation directories around.
    /// Informs the user if the server files were not located.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LocateServerFilesBtnPressed(object sender, EventArgs e)
    {
      if (FileIOManager.GetInstance().LocateServerFiles())
      {
        UpdateSteamCmdInstallStatus();
      }
      FileIOManager.GetInstance().InstallNoBackendScenarioLoader();
    }

    /// <summary>
    /// Enable / Disable Server Configuration Fields
    /// </summary>
    /// <param name="enabled"></param>
    private void EnableServerFields(bool enabled)
    {
      foreach (KeyValuePair<string, ServerParameter> param in
          ConfigurationManager.GetInstance().GetServerParametersDictionary())
      {
        param.Value.SetFieldEnabled(enabled);
      }

      foreach (KeyValuePair<string, AdvancedServerParameter> param in
          ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary())
      {
        param.Value.SetEnabled(enabled);
      }

      enableAllModsBtn.Enabled = enabled;
      addToEnabledBtn.Enabled = enabled;
      disableAllModsBtn.Enabled = enabled;
      removeFromEnabledBtn.Enabled = enabled;
      loadSettingsBtn.Enabled = enabled;
      saveSettingsBtn.Enabled = enabled;
      addModBtn.Enabled = enabled;
      removeModBtn.Enabled = enabled;
      editModBtn.Enabled = enabled;
      deleteServerFilesBtn.Enabled = enabled;
      locateServerFilesBtn.Enabled = enabled;
      logLevelComboBox.Enabled = enabled;
      scenarioSelectBtn.Enabled = enabled;
      editMissionHeaderBtn.Enabled = enabled;
      useExperimentalCheckBox.Enabled = enabled;
      useUpnp.Enabled = enabled;
      moveModPosUpBtn.Enabled = enabled;
      moveModPosDownBtn.Enabled = enabled;
      loadSaveGameBtn.Enabled = enabled;
      keepServerUpdated.Enabled = enabled;
      importModsBtn.Enabled = enabled;
      exportModsBtn.Enabled = enabled;

      // The clipboard buttons are the opposite
      copyAddressBtn.Enabled = !enabled;
      copyRconAddressBtn.Enabled = !enabled;
      copyJoinCodeBtn.Enabled = !enabled;
    }

    /// <summary>
    /// Handler for the "Clear Log" button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearLogBtnPressed(object sender, EventArgs e)
    {
      steamCmdLog.Text = string.Empty;
    }

    /// <summary>
    /// Handler for when the Scenario Select button is pressed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ScenarioSelectBtnClicked(object sender, EventArgs e)
    {
      SpawnScenarioSelect();
    }

    /// <summary>
    /// Little hacky method for refreshing the Scenario ID value, 
    /// this is called from within the Scenario Selector form
    /// At some stage I should probably make this whole thing MVC
    /// </summary>
    public void RefreshLoadedScenario()
    {
      loadedScenarioLabel.Text = ConfigurationManager.GetInstance().GetServerConfiguration().root.game.scenarioId;
    }

    /// <summary>
    /// Logic for starting the Scenario Select form
    /// </summary>
    private void SpawnScenarioSelect()
    {
      ScenarioSelector scenarioSelector = new(this);
      scenarioSelector.ShowDialog();
    }

    private void SpawnSaveSelect()
    {
      SaveSelector saveSelector = new();
      saveSelector.ShowDialog();
    }

    private void EditMissionHeaderBtnClicked(object sender, EventArgs e)
    {
      TextInputForm tif = new("Edit Mission Header", ConfigurationManager.GetInstance().GetServerConfiguration().MissionHeaderAsJsonString());
      tif.ShowDialog();
      ConfigurationManager.GetInstance().GetServerConfiguration().SetMissionHeaderFromJson(tif.GetText());
    }

    private void EditAdminsListBtnClicked(object sender, EventArgs e)
    {
      ListForm lf = new("Edit Admins", ConfigurationManager.GetInstance().GetServerConfiguration().root.game.admins);
      lf.ShowDialog();
      ConfigurationManager.GetInstance().GetServerConfiguration().root.game.admins = lf.GetItems();
    }

    /// <summary>
    /// Populate Config Managers Server Parameter UI controls to easily retrieve values and send them to the model
    /// </summary>
    private void CreateServerParameterControls()
    {
      ServerParameterString serverName = new()
      {
        ParameterName = "name",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_NAME_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_NAME_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(serverName);
      ServerParameterString serverPassword = new()
      {
        ParameterName = "password",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PASSWORD_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_PASSWORD_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(serverPassword);
      ServerParameterString adminPassword = new()
      {
        ParameterName = "passwordAdmin",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PASSWORD_ADMIN_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_ADMIN_PASSWORD_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(adminPassword);
      ServerParameterList admins = new()
      {
        ParameterName = "admins",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_ADMINS_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_ADMINS_TOOLTIP_STR),
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.game.admins
      };
      serverParameters.Controls.Add(admins);
      ServerParameterBool modsRequiredByDefault = new()
      {
        ParameterName = "modsRequiredByDefault",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_MODS_REQUIRED_BY_DEFAULT_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_MODS_REQUIRED_BY_DEFAULT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(modsRequiredByDefault);
      ServerParameterNumeric maxPlayers = new()
      {
        ParameterName = "maxPlayers",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_MAX_PLAYERS_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Game.MIN_PLAYERS,
        ParameterMax = Game.MAX_PLAYERS,
        ParameterValue = Game.DEFAULT_PLAYERS,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_MAX_PLAYERS_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(maxPlayers);
      ServerParameterBool visible = new()
      {
        ParameterName = "visible",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_VISIBLE_LABEL_STR),
        ParameterValue = Game.DEFAULT_VISIBLE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_VISIBLE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(visible);
      ServerParameterString bindAddress = new()
      {
        ParameterName = "bindAddress",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_BIND_ADDRESS_LABEL_STR),
        ParameterValue = Root.DEFAULT_BIND_ADDRESS,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_BIND_ADDRESS_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(bindAddress);
      ServerParameterNumeric bindPort = new()
      {
        ParameterName = "bindPort",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_BIND_PORT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = Root.DEFAULT_PORT,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_BIND_PORT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(bindPort);
      ServerParameterString publicAddress = new()
      {
        ParameterName = "publicAddress",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PUBLIC_ADDRESS_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_PUBLIC_ADDRESS_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(publicAddress);
      ServerParameterNumeric publicPort = new()
      {
        ParameterName = "publicPort",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PUBLIC_PORT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = Root.DEFAULT_PORT,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_PUBLIC_PORT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(publicPort);
      ServerParameterString a2sAddress = new()
      {
        ParameterName = "address",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_ADDRESS_LABEL_STR),
        ParameterValue = A2S.DEFAULT_ADDRESS,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_A2S_ADDRESS_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(a2sAddress);
      ServerParameterNumeric a2sPort = new()
      {
        ParameterName = "port",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PORT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = A2S.DEFAULT_PORT,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_A2S_PORT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(a2sPort);
      ServerParameterBool rconEnabled = new()
      {
        ParameterName = "rconEnabled",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_ENABLED_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_ENABLE_RCON_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(rconEnabled);
      ServerParameterString rconAddress = new()
      {
        ParameterName = "rconAddress",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_ADDRESS_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_ADDRESS_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(rconAddress);
      ServerParameterNumeric rconPort = new()
      {
        ParameterName = "rconPort",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_PORT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = Rcon.DEFAULT_PORT,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_PORT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(rconPort);
      ServerParameterString rconPassword = new()
      {
        ParameterName = "rconPassword",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_PASSWORD_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_PASSWORD_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(rconPassword);
      ServerParameterNumeric rconMaxClients = new()
      {
        ParameterName = "rconMaxClients",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_MAX_CLIENTS_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_MAX_CLIENTS_TOOLTIP_STR),
        ParameterIncrement = 1,
        ParameterMin = Rcon.MIN_CLIENTS,
        ParameterMax = Rcon.MAX_CLIENTS,
        ParameterValue = Rcon.DEFAULT_CLIENTS
      };
      serverParameters.Controls.Add(rconMaxClients);
      ServerParameterSelect rconPermission = new()
      {
        ParameterName = "rconPermission",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_PERMISSION_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_PERMISSION_TOOLTIP_STR),
        ParameterValue = Rcon.PERMISSIONS
      };
      rconPermission.ParameterValueSelection(Rcon.DEFAULT_PERMISSION);
      serverParameters.Controls.Add(rconPermission);
      ServerParameterList rconWhitelist = new()
      {
        ParameterName = "rconWhitelist",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_WHITELIST_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_WHITELIST_TOOLTIP_STR),
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.rcon.whitelist
      };
      serverParameters.Controls.Add(rconWhitelist);
      ServerParameterList rconBlacklist = new()
      {
        ParameterName = "rconBlacklist",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_RCON_BLACKLIST_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_RCON_BLACKLIST_TOOLTIP_STR),
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.rcon.blacklist
      };
      serverParameters.Controls.Add(rconBlacklist);
      ServerParameterNumeric playerSaveTime = new()
      {
        ParameterName = "playerSaveTime",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PLAYER_SAVE_TIME_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Operating.MIN_PLAYER_SAVE_TIME,
        ParameterMax = Operating.MAX_PLAYER_SAVE_TIME,
        ParameterValue = Operating.DEFAULT_PLAYER_SAVE_TIME,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_PLAYER_SAVE_TIME_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(playerSaveTime);
      ServerParameterNumeric serverMaxViewDistance = new()
      {
        ParameterName = "serverMaxViewDistance",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = GameProperties.MIN_SERVER_VIEW_DISTANCE,
        ParameterMax = GameProperties.MAX_SERVER_VIEW_DISTANCE,
        ParameterValue = GameProperties.DEFAULT_SERVER_VIEW_DISTANCE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(serverMaxViewDistance);
      ServerParameterNumeric serverMinGrassDistance = new()
      {
        ParameterName = "serverMinGrassDistance",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = GameProperties.MIN_SERVER_GRASS_DISTANCE,
        ParameterMax = GameProperties.MAX_SERVER_GRASS_DISTANCE,
        ParameterValue = GameProperties.DEFAULT_SERVER_GRASS_DISTANCE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(serverMinGrassDistance);
      ServerParameterNumeric networkViewDistance = new()
      {
        ParameterName = "networkViewDistance",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_NETWORK_VIEW_DISTANCE_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = GameProperties.MIN_NETWORK_VIEW_DISTANCE,
        ParameterMax = GameProperties.MAX_NETWORK_VIEW_DISTANCE,
        ParameterValue = GameProperties.DEFAULT_NETWORK_VIEW_DISTANCE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_NETWORK_VIEW_DISTANCE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(networkViewDistance);
      ServerParameterBool disableThirdPerson = new()
      {
        ParameterName = "disableThirdPerson",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_DISABLE_THIRD_PERSON_LABEL_STR),
        ParameterValue = GameProperties.DEFAULT_DISABLE_THIRD_PERSON,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DISABLE_THIRD_PERSON_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(disableThirdPerson);
      ServerParameterBool fastValidation = new()
      {
        ParameterName = "fastValidation",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_FAST_VALIDATION_LABEL_STR),
        ParameterValue = GameProperties.DEFAULT_FAST_VALIDATION,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_FAST_VALIDATION_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(fastValidation);
      ServerParameterBool battlEye = new()
      {
        ParameterName = "battlEye",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_BATTL_EYE_LABEL_STR),
        ParameterValue = GameProperties.DEFAULT_BATTLE_EYE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_BATTLEYE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(battlEye);
      ServerParameterBool lobbyPlayerSynchronise = new()
      {
        ParameterName = "lobbyPlayerSynchronise",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_LOBBY_PLAYER_SYNCHRONISE_LABEL_STR),
        ParameterValue = Operating.DEFAULT_LOBBY_PLAYER_SYNCHRONISE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_LOBBY_PLAYER_SYNC_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(lobbyPlayerSynchronise);
      ServerParameterBool vonDisableUI = new()
      {
        ParameterName = "VONDisableUI",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_VON_DISABLE_UI_LABEL_STR),
        ParameterValue = GameProperties.DEFAULT_VON_DISABLE_UI,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_VON_DISABLE_UI_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(vonDisableUI);
      ServerParameterBool vonDisableDirectSpeechUI = new()
      {
        ParameterName = "VONDisableDirectSpeechUI",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_LABEL_STR),
        ParameterValue = GameProperties.DEFAULT_VON_DISABLE_DIRECT_SPEECH_UI,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(vonDisableDirectSpeechUI);
      ServerParameterBool vonCanTransmitCrossFaction = new()
      {
        ParameterName = "VONCanTransmitCrossFaction",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_VON_CAN_TRANSMIT_CROSS_FACTION_LABEL_STR),
        ParameterValue = GameProperties.DEFAULT_VON_CAN_TRANSMIT_CROSS_FACTION,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_VON_CAN_TRANSMIT_ACROSS_FACTION_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(vonCanTransmitCrossFaction);
      ServerParameterBool crossPlatform = new()
      {
        ParameterName = "crossPlatform",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_CROSS_PLATFORM_LABEL_STR),
        ParameterValue = Game.DEFAULT_CROSS_PLATFORM,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_CROSS_PLATFORM_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(crossPlatform);
      ServerParameterNumeric aiLimit = new()
      {
        ParameterName = "aiLimit",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_AI_LIMIT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Operating.MIN_AI_LIMIT,
        ParameterMax = Operating.MAX_AI_LIMIT,
        ParameterValue = Operating.DEFAULT_AI_LIMIT,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_AI_LIMIT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(aiLimit);
      ServerParameterNumeric slotReservationTimeout = new()
      {
        ParameterName = "slotReservationTimeout",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = Operating.MIN_SLOT_RESERVATION_TIMEOUT,
        ParameterMax = Operating.MAX_SLOT_RESERVATION_TIMEOUT,
        ParameterValue = Operating.DEFAULT_SLOT_RESERVATION_TIMEOUT,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(slotReservationTimeout);
      ServerParameterBool toggleDisableNavmeshStreaming = new()
      {
        ParameterName = "toggleDisableNavmeshStreaming",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_TOGGLE_DISABLE_NAVMESH_STREAMING_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DISABLE_NAVMESH_STREAMING_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(toggleDisableNavmeshStreaming);
      ServerParameterList disableNavmeshStreaming = new()
      {
        ParameterName = "disableNavmeshStreaming",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_DISABLE_NAVMESH_STREAMING_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DISABLE_SPECIFIC_NAVMESH_STREAMING_TOOLTIP_STR),
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.operating.disableNavmeshStreaming
      };
      serverParameters.Controls.Add(disableNavmeshStreaming);
      ServerParameterBool disableServerShutdown = new()
      {
        ParameterName = "disableServerShutdown",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_LABEL_STR),
        ParameterValue = Operating.DEFAULT_DISABLE_SERVER_SHUTDOWN,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(disableServerShutdown);
      ServerParameterBool disableCrashReporter = new()
      {
        ParameterName = "disableCrashReporter",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_DISABLE_CRASH_REPORTER_LABEL_STR),
        ParameterValue = Operating.DEFAULT_DISABLE_CRASH_REPORTER,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DISABLE_CRASH_REPORT_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(disableCrashReporter);
      ServerParameterBool disableAI = new()
      {
        ParameterName = "disableAI",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_DISABLE_AI_LABEL_STR),
        ParameterValue = Operating.DEFAULT_DISABLE_AI,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DISABLE_AI_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(disableAI);
      ServerParameterNumeric joinQueueMaxSize = new()
      {
        ParameterName = "maxSize",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_MAX_SIZE_LABEL_STR),
        ParameterValue = JoinQueue.DEFAULT_MAX_SIZE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_JOIN_QUEUE_MAX_SIZE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(joinQueueMaxSize);
      ServerParameterBool persistence = new()
      {
        ParameterName = "persistenceEnabled",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_PERSISTENCE_ENABLED_LABEL_STR),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_ENABLE_PERSISTENCE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(persistence);
      ServerParameterNumeric autoSaveInterval = new()
      {
        ParameterName = "autoSaveInterval",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_AUTO_SAVE_INTERVAL_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_AUTOSAVE_INTERVAL_MINS,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_AUTO_SAVE_INTERVAL_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(autoSaveInterval);
      ServerParameterNumeric saveRetention = new()
      {
        ParameterName = "saveRetention",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_SAVE_RETENTION_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_SAVE_RETENTION_VALUE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_SAVE_RETENTION_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(saveRetention);
      ServerParameterBool loadSessionSave = new()
      {
        ParameterName = "loadSessionSave",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_LOAD_SESSION_SAVE_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_LOAD_SESSION_SAVE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_LOAD_SESSION_SAVE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(loadSessionSave);
      ServerParameterBool keepSessionSave = new()
      {
        ParameterName = "keepSessionSave",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_KEEP_SESSION_SAVE_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_KEEP_SESSION_SAVE,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_KEEP_SESSION_SAVE_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(keepSessionSave);
      ServerParameterNumeric hiveId = new()
      {
        ParameterName = "hiveId",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_HIVE_ID_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_HIVE_ID,
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_HIVE_ID_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(hiveId);
      ServerParameterText databases = new()
      {
        ParameterName = "databases",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_DATABASES_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_DATABASES.ToString(),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_DATABASES_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(databases);
      ServerParameterText storages = new()
      {
        ParameterName = "storages",
        ParameterFriendlyName = Strings.Get(Constants.SERVER_PARAM_STORAGES_LABEL_STR),
        ParameterValue = Persistence.DEFAULT_STORAGES.ToString(),
        ParameterTooltip = Strings.Get(Constants.SERVER_PARAM_STORAGES_TOOLTIP_STR)
      };
      serverParameters.Controls.Add(storages);

      foreach (ServerParameter param in serverParameters.Controls)
      {
        ConfigurationManager.GetInstance().GetServerParametersDictionary()[param.ParameterName] = param;
      }
    }

    private void CreateAdvancedServerParameterControls()
    {
      Dictionary<string, AdvancedSetting> loadedSettings = SavedStateManager.GetInstance().GetSavedState().advancedSettings;

      AdvancedServerParameterNumeric limitServerMaxFPS = new()
      {
        ParameterName = "maxFPS",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_MAX_FPS_LABEL_STR),
        ParameterMin = 1,
        ParameterMax = 1000,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "maxFPS").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_MAX_FPS_DESCRIPTION_STR)
      };
      limitServerMaxFPS.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "maxFPS").Enabled;
      advancedParametersPanel.Controls.Add(limitServerMaxFPS);

      AdvancedServerParameterTime autoRestartTime = new()
      {
        ParameterName = "autoRestartDaily",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_AUTO_RESTART_DAILY_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_AUTO_RESTART_DAILY_DESCRIPTION_STR),
        ParameterMin = DateTime.Today,
        ParameterMax = DateTime.Today.AddDays(1).AddMinutes(-1),
        ParameterValue = DateTime.Today
      };
      advancedParametersPanel.Controls.Add(autoRestartTime);

      AdvancedServerParameterBool addonsRepair = new()
      {
        ParameterName = "addonsRepair",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_ADDONS_REPAIR_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_ADDONS_REPAIR_DESCRIPTION_STR)
      };
      addonsRepair.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "addonsRepair").Enabled;
      advancedParametersPanel.Controls.Add(addonsRepair);

      AdvancedServerParameterBool autoRestartOnCrash = new()
      {
        ParameterName = "autoRestartOnCrash",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_AUTO_RESTART_ON_CRASH_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_AUTO_RESTART_ON_CRASH_DESCRIPTION_STR)
      };
      autoRestartOnCrash.CheckBox.CheckedChanged += AutoRestartOnCrashCheckChanged;
      autoRestartOnCrash.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "autoRestartOnCrash").Enabled;
      advancedParametersPanel.Controls.Add(autoRestartOnCrash);

      AdvancedServerParameterNumeric autoReload = new()
      {
        ParameterName = "autoreload",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_AUTORELOAD_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_AUTORELOAD_DESCRIPTION_STR),
        ParameterMin = 1,
        ParameterMax = int.MaxValue,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "autoreload").Value
      };
      autoReload.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "autoreload").Enabled;
      advancedParametersPanel.Controls.Add(autoReload);

      AdvancedServerParameterString loadSessionSave = new()
      {
        ParameterName = "loadSessionSave",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_LOAD_SESSION_SAVE_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_LOAD_SESSION_SAVE_DESCRIPTION_STR),
        ParameterPlaceholder = Strings.Get(Constants.ADVANCED_PARAM_LOAD_SESSION_SAVE_PLACEHOLDER_STR)
      };
      bool loadSessionSavedEnabled = SavedStateManager.GetSetting(loadedSettings, "loadSessionSave").Enabled;
      loadSessionSave.CheckBox.Checked = loadSessionSavedEnabled;
      if (loadSessionSavedEnabled)
      {
        loadSessionSave.ParameterValue = SavedStateManager.GetSetting(loadedSettings, "loadSessionSave").Value;
      }
      advancedParametersPanel.Controls.Add(loadSessionSave);

      AdvancedServerParameterBool noBackend = new()
      {
        ParameterName = "noBackend",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_NO_BACKEND_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_NO_BACKEND_DESCRIPTION_STR)
      };
      noBackend.CheckBox.CheckedChanged += NoBackendCheckChanged;
      noBackend.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "noBackend").Enabled;
      advancedParametersPanel.Controls.Add(noBackend);

      AdvancedServerParameterBool autoShutdown = new()
      {
        ParameterName = "autoShutdown",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_AUTO_SHUTDOWN_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_AUTO_SHUTDOWN_DESCRIPTION_STR)
      };
      autoShutdown.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "autoShutdown").Enabled;
      advancedParametersPanel.Controls.Add(autoShutdown);

      AdvancedServerParameterBool logVoting = new()
      {
        ParameterName = "logVoting",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_LOG_VOTING_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_LOG_VOTING_DESCRIPTION_STR)
      };
      logVoting.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "logVoting").Enabled;
      advancedParametersPanel.Controls.Add(logVoting);

      AdvancedServerParameterNumeric overridePort = new()
      {
        ParameterName = "bindPort",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_BIND_PORT_LABEL_STR),
        ParameterMin = 1,
        ParameterMax = 65535,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "bindPort").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_BIND_PORT_DESCRIPTION_STR)
      };
      overridePort.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "bindPort").Enabled;
      advancedParametersPanel.Controls.Add(overridePort);

      AdvancedServerParameterNumeric networkDynamicSim = new()
      {
        ParameterName = "nds",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_NDS_LABEL_STR),
        ParameterMin = 0,
        ParameterMax = 2,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "nds").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_NDS_DESCRIPTION_STR)
      };
      networkDynamicSim.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "nds").Enabled;
      advancedParametersPanel.Controls.Add(networkDynamicSim);

      AdvancedServerParameterNumeric spatialMapRes = new()
      {
        ParameterName = "nwkResolution",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_NWK_RESOLUTION_LABEL_STR),
        ParameterMin = 100,
        ParameterMax = 1000,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "nwkResolution").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_NWK_RESOLUTION_DESCRIPTION_STR)
      };
      spatialMapRes.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "nwkResolution").Enabled;
      advancedParametersPanel.Controls.Add(spatialMapRes);

      AdvancedServerParameterNumeric staggeringBudget = new()
      {
        ParameterName = "staggeringBudget",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_STAGGERING_BUDGET_LABEL_STR),
        ParameterMin = 1,
        ParameterMax = 10201,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "staggeringBudget").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_STAGGERING_BUDGET_DESCRIPTION_STR)
      };
      staggeringBudget.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "staggeringBudget").Enabled;
      advancedParametersPanel.Controls.Add(staggeringBudget);

      AdvancedServerParameterNumeric streamingBudget = new()
      {
        ParameterName = "streamingBudget",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_STREAMING_BUDGET_LABEL_STR),
        ParameterMin = 100,
        ParameterMax = 10201,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "streamingBudget").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_STREAMING_BUDGET_DESCRIPTION_STR)
      };
      streamingBudget.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "streamingBudget").Enabled;
      advancedParametersPanel.Controls.Add(streamingBudget);

      AdvancedServerParameterNumeric streamsDelta = new()
      {
        ParameterName = "streamsDelta",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_STREAMS_DELTA_LABEL_STR),
        ParameterMin = 1,
        ParameterMax = 1000,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "streamsDelta").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_STREAMS_DELTA_DESCRIPTION_STR)
      };
      streamsDelta.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "streamsDelta").Enabled;
      advancedParametersPanel.Controls.Add(streamsDelta);

      AdvancedServerParameterNumeric rplTimeoutMs = new()
      {
        ParameterName = "rpl-timeout-ms",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_RPL_TIMEOUT_MS_LABEL_STR),
        ParameterMin = 1,
        ParameterMax = int.MaxValue,
        ParameterIncrement = 1,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "rpl-timeout-ms").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_RPL_TIMEOUT_MS_DESCRIPTION_STR)
      };
      rplTimeoutMs.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "rpl-timeout-ms").Enabled;
      advancedParametersPanel.Controls.Add(rplTimeoutMs);

      AdvancedServerParameterBool aiPartialSim = new()
      {
        ParameterName = "aiPartialSim",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_AI_PARTIAL_SIM_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_AI_PARTIAL_SIM_DESCRIPTION_STR)
      };
      aiPartialSim.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "aiPartialSim").Enabled;
      advancedParametersPanel.Controls.Add(aiPartialSim);

      AdvancedServerParameterBool createDB = new()
      {
        ParameterName = "createDB",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_CREATE_DB_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_CREATE_DB_DESCRIPTION_STR)
      };
      createDB.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "createDB").Enabled;
      advancedParametersPanel.Controls.Add(createDB);

      AdvancedServerParameterString debugger = new()
      {
        ParameterName = "debugger",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_DEBUGGER_LABEL_STR),
        ParameterPlaceholder = "127.0.0.1",
        Description = Strings.Get(Constants.ADVANCED_PARAM_DEBUGGER_DESCRIPTION_STR)
      };
      bool debuggerEnabled = SavedStateManager.GetSetting(loadedSettings, "debugger").Enabled;
      debugger.CheckBox.Checked = debuggerEnabled;
      if (debuggerEnabled)
      {
        debugger.ParameterValue = SavedStateManager.GetSetting(loadedSettings, "debugger").Value;
      }
      advancedParametersPanel.Controls.Add(debugger);

      AdvancedServerParameterNumeric debuggerPort = new()
      {
        ParameterName = "debuggerPort",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_DEBUGGER_PORT_LABEL_STR),
        ParameterIncrement = 1,
        ParameterMin = 1,
        ParameterMax = 65535,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "debuggerPort").Value,
        Description = Strings.Get(Constants.ADVANCED_PARAM_DEBUGGER_PORT_DESCRIPTION_STR)
      };
      debuggerPort.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "debuggerPort").Enabled;
      advancedParametersPanel.Controls.Add(debuggerPort);

      AdvancedServerParameterBool disableShadersBuild = new()
      {
        ParameterName = "disableShadersBuild",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_DISABLE_SHADERS_BUILD_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_DISABLE_SHADERS_BUILD_DESCRIPTION_STR)
      };
      disableShadersBuild.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "disableShadersBuild").Enabled;
      advancedParametersPanel.Controls.Add(disableShadersBuild);

      AdvancedServerParameterBool generateShaders = new()
      {
        ParameterName = "generateShaders",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_GENERATE_SHADERS_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_GENERATE_SHADERS_DESCRIPTION_STR)
      };
      generateShaders.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "generateShaders").Enabled;
      advancedParametersPanel.Controls.Add(generateShaders);

      AdvancedServerParameterBool rplEncodeAsLongJobs = new()
      {
        ParameterName = "rplEncodeAsLongJobs",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_RPL_ENCODE_AS_LONG_JOBS_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_RPL_ENCODE_AS_LONG_JOBS_DESCRIPTION_STR)
      };
      rplEncodeAsLongJobs.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "rplEncodeAsLongJobs").Enabled;
      advancedParametersPanel.Controls.Add(rplEncodeAsLongJobs);

      AdvancedServerParameterNumeric jobsysShortWorkerCount = new()
      {
        ParameterName = "jobsysShortWorkerCount",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_JOBSYS_SHORT_WORKER_COUNT_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_JOBSYS_SHORT_WORKER_COUNT_DESCRIPTION_STR),
        ParameterMin = 1,
        ParameterMax = Utilities.GetNumberAvailableThreads(),
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "jobsysShortWorkerCount").Value
      };
      jobsysShortWorkerCount.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "jobsysShortWorkerCount").Enabled;
      advancedParametersPanel.Controls.Add(jobsysShortWorkerCount);

      AdvancedServerParameterNumeric jobsysLongWorkerCount = new()
      {
        ParameterName = "jobsysLongWorkerCount",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_JOBSYS_LONG_WORKER_COUNT_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_JOBSYS_LONG_WORKER_COUNT_DESCRIPTION_STR),
        ParameterMin = 1,
        ParameterMax = Utilities.GetNumberAvailableThreads(),
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "jobsysLongWorkerCount").Value
      };
      jobsysLongWorkerCount.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "jobsysLongWorkerCount").Enabled;
      advancedParametersPanel.Controls.Add(jobsysLongWorkerCount);

      AdvancedServerParameterNumeric freezeCheck = new()
      {
        ParameterName = "freezeCheck",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_FREEZE_CHECK_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_FREEZE_CHECK_DESCRIPTION_STR),
        ParameterIncrement = 1,
        ParameterMin = 0,
        ParameterMax = 600,
        ParameterValue = SavedStateManager.GetSetting(loadedSettings, "freezeCheck").Value
      };
      freezeCheck.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "freezeCheck").Enabled;
      advancedParametersPanel.Controls.Add(freezeCheck);

      AdvancedServerParameterEnumerated freezeCheckMode = new()
      {
        ParameterName = "freezeCheckMode",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_FREEZE_CHECK_MODE_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_FREEZE_CHECK_MODE_DESCRIPTION_STR),
        ParameterAvailableValues = new List<string>() {"crash", "minidump", "kill"}
      };
      bool freezeCheckModeEnabled = SavedStateManager.GetSetting(loadedSettings, "freezeCheckMode").Enabled;
      freezeCheckMode.CheckBox.Checked = freezeCheckModeEnabled;
      if (freezeCheckModeEnabled)
      {
        freezeCheckMode.ParameterValue = SavedStateManager.GetSetting(loadedSettings, "freezeCheckMode").Value;
      }
      advancedParametersPanel.Controls.Add(freezeCheckMode);

      AdvancedServerParameterBool forceDisableNightGrain = new()
      {
        ParameterName = "forceDisableNightGrain",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_FORCE_DISABLE_NIGHT_GRAIN_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_FORCE_DISABLE_NIGHT_GRAIN_DESCRIPTION_STR),
      };
      forceDisableNightGrain.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "forceDisableNightGrain").Enabled;
      advancedParametersPanel.Controls.Add(forceDisableNightGrain);

      AdvancedServerParameterBool keepSessionSave = new()
      {
        ParameterName = "keepSessionSave",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_KEEP_SESSION_SAVE_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_KEEP_SESSION_SAVE_DESCRIPTION_STR)
      };
      keepSessionSave.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "keepSessionSave").Enabled;
      advancedParametersPanel.Controls.Add(keepSessionSave);

      AdvancedServerParameterString playerLimits = new()
      {
        ParameterName = "playerLimits",
        ParameterFriendlyName = Strings.Get(Constants.ADVANCED_PARAM_PLAYER_LIMITS_LABEL_STR),
        Description = Strings.Get(Constants.ADVANCED_PARAM_PLAYER_LIMITS_DESCRIPTION_STR),
        ParameterPlaceholder = Strings.Get(Constants.ADVANCED_PARAM_PLAYER_LIMITS_PLACEHOLDER_STR),
        ParameterValue = SavedStateManager.GetSetting(loadedSettings,"playerLimits").Value
      };
      playerLimits.CheckBox.Checked = SavedStateManager.GetSetting(loadedSettings, "playerLimits").Enabled;
      advancedParametersPanel.Controls.Add(playerLimits);

      foreach (AdvancedServerParameter param in advancedParametersPanel.Controls)
      {
        ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary()[param.ParameterName] = param;
      }
    }

    private void NoBackendCheckChanged(object? sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().noBackend =
          ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary()["noBackend"].Checked();

      if (ConfigurationManager.GetInstance().noBackend)
      {
        bool enableNoBackend = Utilities.DisplayConfirmationMessage(Strings.Get(Constants.MAIN_ENABLE_NO_BACKEND_CONFIRM_STR), true);

        if (!enableNoBackend)
        {
          ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary()["noBackend"].CheckBox.Checked = false;
        }
      }
    }

    private void AutoRestartOnCrashCheckChanged(object? sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().autoRestartOnCrash =
          ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary()["autoRestartOnCrash"].Checked();
    }

    /// <summary>
    /// Event Handler for the 'UpdateSteamCmdLog' event
    /// This method is called twice if the call came from a non-UI thread
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">contains the line to append to the Steam CMD log</param>
    private void HandleUpdateSteamCmdLogEvent(object sender, SteamCmdLogEventArgs e)
    {
      if (steamCmdLog.InvokeRequired)
      {
        steamCmdLog.Invoke(new Action(() => HandleUpdateSteamCmdLogEvent(sender, e)));
      }
      else
      {
        steamCmdLog.AppendText(e.line);

        if (steamCmdLog.Lines.Length > 500)
        {
          steamCmdLog.SuspendLayout();

          bool wasReadOnly = steamCmdLog.ReadOnly;
          if (wasReadOnly)
          {
            steamCmdLog.ReadOnly = false;
          }

          int linesToRemove = steamCmdLog.Lines.Length - 500;

          // Get the exact character index where the cutoff happens
          int cutoffIndex = steamCmdLog.GetFirstCharIndexFromLine(linesToRemove);

          // Highlight the oldest text at the top and delete it
          steamCmdLog.Select(0, cutoffIndex);
          steamCmdLog.SelectedText = string.Empty;

          if (wasReadOnly)
          {
            steamCmdLog.ReadOnly = true;
          }

          // Move the caret back to the bottom
          steamCmdLog.SelectionStart = steamCmdLog.Text.Length;
          steamCmdLog.ScrollToCaret();

          steamCmdLog.ResumeLayout();
        }

        // Update the Server Status
        m_serverStatusParser.ParseServerStatus(e.line);
      }
    }

    private void HandleServerStatusEvent(object sender, ServerStatusEventArgs e)
    {
      string serverOfflineString = Strings.Get(Constants.MAIN_SERVER_OFFLINE_STR);
      if (!e.ServerOnline)
      {
        serverAddressStatusLabel.Text = serverOfflineString;
        rconAddressStatusLabel.Text = serverOfflineString;
        pingSiteStatusLabel.Text = serverOfflineString;
        joinCodeStatusLabel.Text = serverOfflineString;
        playerCountStatusLabel.Text = serverOfflineString;
        fpsLabel.Text = Strings.Get(Constants.MAIN_FPS_OFFLINE_STR);
        memLabel.Text = Strings.Get(Constants.MAIN_MEMORY_OFFLINE_STR);
        chartFps.Clear();
        chartMem.Clear();
        flagStatusPB.Image = null;
        return;
      }
      if (e.LastFPS > 0 || e.LastMem > 0)
      {
        UpdatePerformanceGraphs(e.LastFPS, e.LastMem, e.LastUpdate);
      }
      serverAddressStatusLabel.Text = $"{e.LastIP}:{e.LastPort}";
      rconAddressStatusLabel.Text = $"{e.LastRconIP}:{e.LastRconPort}";

      if (!e.LastPingSite.Equals("Unknown"))
      {
        string pingSite = e.LastPingSite.Replace("_", " ").ToUpper();
        flagStatusPB.Image = FlagUtils.ChooseFlag(pingSite);
      }

      pingSiteStatusLabel.Text = string.Concat(e.LastPingSite.Substring(0, 1).ToUpper(), e.LastPingSite.AsSpan(1));
      joinCodeStatusLabel.Text = e.LastJoinCode;

      playerCountStatusLabel.Text = Strings.Get(Constants.MAIN_PLAYER_COUNT_STATUS_STR, ("count", e.LastPlayerCount));
    }

    private void UpdatePerformanceGraphs(double fps, long memoryKb, DateTime time)
    {
      double memoryGb = Math.Round(memoryKb / 1024.0 / 1024.0, 2);

      if (this.InvokeRequired)
      {
        this.BeginInvoke(new Action(() => UpdatePerformanceGraphs(fps, memoryKb, time)));
        return;
      }

      chartFps.AddDataPoint((float) fps);
      chartMem.AddDataPoint((float) memoryGb);

      fpsLabel.Text = Strings.Get(Constants.MAIN_FPS_STATUS_STR, ("fps", fps));
      memLabel.Text = Strings.Get(Constants.MAIN_MEMORY_STATUS_STR, ("memoryGb", memoryGb));
    }

    /// <summary>
    /// Event Handler for the 'UpdateGuiControls' event
    /// This method is called twice if the call came from a non-UI thread
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">contains values to update various GUI controls with</param>
    private void HandleUpdateGuiControlsEvent(object sender, GuiModelEventArgs e)
    {
      if (startServerBtn.InvokeRequired)
      {
        // We can invoke with any GUI element here
        startServerBtn.Invoke(new Action(() => HandleUpdateGuiControlsEvent(sender, e)));
      }
      else
      {
        startServerBtn.Enabled = e.startServerBtnEnabled;
        serverRunningLabel.Text = e.serverRunningLabelText;
        startServerBtn.IconChar = e.buttonIconChar;
        EnableServerFields(e.enableServerFields);
      }
    }

    /// <summary>
    /// Event Handler for the 'UpdateScenarioIdFromLoadedConfigEvent' event
    /// This method is called twice if the call came from a non-UI thread
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">contains values to update various GUI controls with</param>
    private void HandleUpdateScenarioIdFromLoadedConfigEvent(object sender, ScenarioIdEventArgs e)
    {
      if (loadedScenarioLabel.InvokeRequired)
      {
        loadedScenarioLabel.Invoke(new Action(() =>
            HandleUpdateScenarioIdFromLoadedConfigEvent(sender, e)));
      }
      else
      {
        if (string.IsNullOrWhiteSpace(e.scenarioId))
        {
          loadedScenarioLabel.Text = Strings.Get(Constants.MAIN_NO_SCENARIO_ID_CHOSEN_MESSAGE_STR);
        }
        else
        {
          loadedScenarioLabel.Text = e.scenarioId;
        }
      }
    }

    /// <summary>
    /// Create a string with all requried launch arguments
    /// </summary>
    /// <returns>String containing launch arguments for the Reforger Server</returns>
    private void CreateLaunchArguments()
    {
      // Begin by setting the mandatory parameters
      LaunchArguments args = new()
      {
        // Config will be placed in <server-files-dir>/server.json, wrap in quotes to capture potential spaces in paths
        config = new("config", $"\"{FileIOManager.GetInstance().GetInstallDirectory()}\\server.json\""),
        // Saves etc. will be placed in <server-files-dir>/saves/, wrap in quotes to capture potential spaces in paths
        profile = new("profile", $"\"{FileIOManager.GetInstance().GetInstallDirectory()}\\saves\""),
        // Addons will be placed in <server-files-dir>/addons/, wrap in quotes to capture potentional spaces in paths
        addonsDir = new("addonsDir", $"\"{FileIOManager.GetInstance().GetInstallDirectory()}\\addons\""),
        // Log performance stats every 5 seconds (represented in ms)
        logStats = new("logStats", $"{Convert.ToString(5000)}"),
        logLevel = new("logLevel", $"{logLevelComboBox.Text}")
      };

      var advParams = ConfigurationManager.GetInstance().GetAdvancedServerParametersDictionary();

      if (advParams["loadSessionSave"].Checked())
      {
        // As no parameter is also valid, check if there is a value
        string loadSessionSaveVal = Convert.ToString(advParams["loadSessionSave"].ParameterValue);
        if (String.IsNullOrWhiteSpace(loadSessionSaveVal))
        {
          args.loadSessionSave = new("loadSessionSave");
        }
        else
        {
          // Need to wrap the save in quotes as it allows spaces
          args.loadSessionSave = new("loadSessionSave", $"\"{loadSessionSaveVal}\"");
        }
      }

      if (advParams["maxFPS"].Checked())
      {
        args.maxFPS = new("maxFPS", Convert.ToString(advParams["maxFPS"].ParameterValue));
      }

      if (advParams["bindPort"].Checked())
      {
        args.bindPort = new("bindPort", Convert.ToString(advParams["bindPort"].ParameterValue));
      }

      if (advParams["nds"].Checked())
      {
        args.nds = new("nds", Convert.ToString(advParams["nds"].ParameterValue));
      }

      if (advParams["nwkResolution"].Checked())
      {
        args.nwkResolution = new("nwkResolution", Convert.ToString(advParams["nwkResolution"].ParameterValue));
      }

      if (advParams["staggeringBudget"].Checked())
      {
        args.staggeringBudget = new("staggeringBudget", Convert.ToString(advParams["staggeringBudget"].ParameterValue));
      }

      if (advParams["streamingBudget"].Checked())
      {
        args.streamingBudget = new("streamingBudget", Convert.ToString(advParams["streamingBudget"].ParameterValue));
      }

      if (advParams["streamsDelta"].Checked())
      {
        args.streamsDelta = new("streamsDelta", Convert.ToString(advParams["streamsDelta"].ParameterValue));
      }

      if (advParams["autoreload"].Checked())
      {
        args.autoReload = new("autoreload", Convert.ToString(advParams["autoreload"].ParameterValue));
      }

      if (advParams["rpl-timeout-ms"].Checked())
      {
        args.rplTimeoutMs = new("rpl-timeout-ms", Convert.ToString(advParams["rpl-timeout-ms"].ParameterValue));
      }

      if (advParams["freezeCheck"].Checked())
      {
        args.freezeCheck = new("freezeCheck", Convert.ToString(advParams["freezeCheck"].ParameterValue));
      }

      if (advParams["freezeCheckMode"].Checked())
      {
        args.freezeCheckMode = new("freezeCheckMode", ((AdvancedServerParameterEnumerated) advParams["freezeCheckMode"]).SelectedItem);
      }

      if (advParams["addonsRepair"].Checked())
      {
        args.addonsRepair = new("addonsRepair");
      }

      if (advParams["autoShutdown"].Checked())
      {
        args.autoShutdown = new("autoShutdown");
      }

      if (advParams["logVoting"].Checked())
      {
        args.logVoting = new("logVoting");
      }

      if (advParams["aiPartialSim"].Checked())
      {
        args.aiPartialSim = new("aiPartialSim");
      }

      if (advParams["createDB"].Checked())
      {
        args.createDB = new("createDB");
      }

      if (advParams["debugger"].Checked())
      {
        args.debugger = new("debugger", Convert.ToString(advParams["debugger"].ParameterValue));
      }

      if (advParams["debuggerPort"].Checked())
      {
        args.debuggerPort = new("debuggerPort", Convert.ToString(advParams["debuggerPort"].ParameterValue));
      }

      if (advParams["disableShadersBuild"].Checked())
      {
        args.disableShadersBuild = new("disableShadersBuild");
      }

      if (advParams["generateShaders"].Checked())
      {
        args.generateShaders = new("generateShaders");
      }

      if (advParams["rplEncodeAsLongJobs"].Checked())
      {
        args.rplEncodeAsLongJobs = new("rplEncodeAsLongJobs");
      }

      if (advParams["jobsysShortWorkerCount"].Checked())
      {
        args.jobSysShortWorkerCount = new("jobsysShortWorkerCount", Convert.ToString(advParams["jobsysShortWorkerCount"].ParameterValue));
      }

      if (advParams["jobsysLongWorkerCount"].Checked())
      {
        args.jobSysLongWorkerCount = new("jobsysLongWorkerCount", Convert.ToString(advParams["jobsysLongWorkerCount"].ParameterValue));
      }

      if (advParams["forceDisableNightGrain"].Checked())
      {
        args.forceDisableNightGrain = new("forceDisableNightGrain");
      }

      if (advParams["playerLimits"].Checked())
      {
        args.playerLimits = new("playerLimits", Convert.ToString(advParams["playerLimits"].ParameterValue));
      }

      if (advParams["keepSessionSave"].Checked())
      {
        args.keepSessionSave = new("keepSessionSave");
      }

      ProcessManager.GetInstance().SetLaunchArgumentsModel(args);
    }

    /// <summary>
    /// Filter a Source, searching for a given string
    /// </summary>
    /// <param name="filter">to use when searching for matching items</param>
    /// <param name="modList">source list to search</param>
    /// <returns>filtered list</returns>
    private static List<Mod> FilterModList(string filter, BindingList<Mod> modList)
    {
      return modList
          .Where(mod => mod.name.ToLower().Contains(filter))
          .ToList();
    }

    /// <summary>
    /// Convenience method for clearing filters and removing text 
    /// from the filter text fields
    /// </summary>
    private void ResetModFilters()
    {
      availableMods.DataSource = m_availableModsBindingSource;
      enabledMods.DataSource = m_enabledModsBindingSource;
      modsSearchTB.Text = string.Empty;
      availableMods.SelectedItem = enabledMods.SelectedItem = null;
    }

    /// <summary>
    /// Event Handler for when text is entered or removed in the 
    /// Search Available Mods text box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">arguments</param>
    private void OnSearchModsTextChanged(object sender, EventArgs e)
    {
      string filter = modsSearchTB.Text.ToLower();
      if (string.IsNullOrEmpty(filter))
      {
        m_availableModsBindingSource.DataSource = ConfigurationManager.GetInstance().GetAvailableMods();
        m_enabledModsBindingSource.DataSource = ConfigurationManager.GetInstance().GetEnabledMods();
      }
      else
      {
        m_availableModsBindingSource.DataSource = FilterModList(filter, ConfigurationManager.GetInstance().GetAvailableMods());
        m_enabledModsBindingSource.DataSource = FilterModList(filter, ConfigurationManager.GetInstance().GetEnabledMods());
      }
    }

    private void UseExperimentalServerCheckboxChanged(object sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().useExperimentalServer = useExperimentalCheckBox.Checked;
    }

    private void OnUseUPnPCheckChanged(object sender, EventArgs e)
    {
      NetworkManager.GetInstance().useUPnP = useUpnp.Checked;
    }

    private void ExportModsListBtnPressed(object sender, EventArgs e)
    {
      FileIOManager.SaveModsListToFile();
    }

    private void ImportModsListBtnPressed(object sender, EventArgs e)
    {
      FileIOManager.LoadModsListFromFile();
    }

    /// <summary>
    /// Utility method to update state of each advanced server parameter with the SavedStateManager
    /// </summary>
    private void UpdateStateForAdvancedSettings()
    {
      foreach (AdvancedServerParameter param in advancedParametersPanel.Controls)
      {
        SavedState ss = SavedStateManager.GetInstance().GetSavedState();
        if (ss.advancedSettings.ContainsKey(param.ParameterName))
        {
          ss.advancedSettings[param.ParameterName].Enabled = param.CheckBox.Checked;
          if (param is AdvancedServerParameterEnumerated)
          {
            AdvancedServerParameterEnumerated enumParam = (AdvancedServerParameterEnumerated) param;
            ss.advancedSettings[param.ParameterName].Value = enumParam.SelectedItem;
          }
          else
          {
            if (param is AdvancedServerParameterNumeric)
            {
              ss.advancedSettings[param.ParameterName].Value = Convert.ToInt32(param.ParameterValue);
            }
            else if (param is AdvancedServerParameterString)
            {
              ss.advancedSettings[param.ParameterName].Value = param.ParameterValue;
            }
          }
        }
      }
    }

    /// <summary>
    /// Event Handler for when the application is closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
      if (ProcessManager.GetInstance().IsServerStarted())
      {
        ProcessManager.GetInstance().KillServer();
      }

      UpdateStateForAdvancedSettings();

      // Update state of the checkboxes
      SavedStateManager.GetInstance().GetSavedState().advancedSettings["useUpnp"].Enabled = useUpnp.Checked;
      SavedStateManager.GetInstance().GetSavedState().advancedSettings["useExperimental"].Enabled = useExperimentalCheckBox.Checked;
      SavedStateManager.GetInstance().GetSavedState().advancedSettings["keepServerUpdated"].Enabled = keepServerUpdated.Checked;

      FileIOManager.GetInstance().WriteStateFile();
      FileIOManager.GetInstance().WriteModsDatabase();
    }

    private void LoadSaveGameBtnPressed(object sender, EventArgs e)
    {
      SpawnSaveSelect();
    }

    private void KeepServerUpdatedCheckedChanged(object sender, EventArgs e)
    {
      ProcessManager.GetInstance().KeepServerUpdated = keepServerUpdated.Checked;
    }

    private void OnServerAddressToClipboard(object sender, EventArgs e)
    {
      Clipboard.SetText(serverAddressStatusLabel.Text);
    }

    private void OnRconAddressToClipboard(object sender, EventArgs e)
    {
      Clipboard.SetText(rconAddressStatusLabel.Text);
    }

    private void OnJoinCodeToClipboard(object sender, EventArgs e)
    {
      Clipboard.SetText(joinCodeStatusLabel.Text);
    }
  }
}
