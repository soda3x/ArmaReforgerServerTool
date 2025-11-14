/******************************************************************************
 * File Name:    Main.cs
 * Project:      Longbow
 * Description:  This is the Main Form
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp.Managers;
using ReforgerServerApp.Models;
using System.ComponentModel;
using Serilog;
using ReforgerServerApp.Components;
using ReforgerServerApp.Utils;
using Longbow.Models;
using Longbow.Managers;
using Longbow.Forms;

namespace ReforgerServerApp
{
  public partial class Main : Form
  {
    private BindingSource m_availableModsBindingSource;
    private BindingSource m_enabledModsBindingSource;
    public Main()
    {
      InitializeComponent();

      CreateServerParameterControls();
      CreateAdvancedServerParameterControls();

      serverRunningLabel.Text = string.Empty;

      ProcessManager.GetInstance().UpdateGuiControlsEvent += HandleUpdateGuiControlsEvent;
      ProcessManager.GetInstance().UpdateSteamCmdLogEvent += HandleUpdateSteamCmdLogEvent;
      ConfigurationManager.GetInstance().UpdateScenarioIdFromLoadedConfigEvent += HandleUpdateScenarioIdFromLoadedConfigEvent;

      useUpnp.Checked = SavedStateManager.GetInstance().GetLoadedAdvancedSettings()["useUpnp"].Enabled;
      useExperimentalCheckBox.Checked = SavedStateManager.GetInstance().GetLoadedAdvancedSettings()["useExperimental"].Enabled;
      NetworkManager.GetInstance().useUPnP = useUpnp.Checked;

      // Create tooltips
      CreateTooltips();

      loadedScenarioLabel.Text = "No scenario chosen.";

      UpdateSteamCmdInstallStatus();


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
    }

    /// <summary>
    /// Initialise tool tips for certain UI elements.
    /// </summary>
    private void CreateTooltips()
    {
      ToolTip enableAllModsToolTip = new();
      enableAllModsToolTip.SetToolTip(enableAllModsBtn, Constants.ENABLE_ALL_MODS_STR);
      ToolTip disableAllModsToolTip = new();
      disableAllModsToolTip.SetToolTip(disableAllModsBtn, Constants.DISABLE_ALL_MODS_STR);
      ToolTip enableModToolTip = new();
      enableModToolTip.SetToolTip(addToEnabledBtn, Constants.ENABLE_MOD_STR);
      ToolTip disableModToolTip = new();
      disableModToolTip.SetToolTip(removeFromEnabledBtn, Constants.DISABLE_MOD_STR);
      ToolTip useUpnpToolTip = new();
      useUpnpToolTip.SetToolTip(useUpnp, Constants.USE_UPNP_STR);
      ToolTip exportModsToolTip = new();
      exportModsToolTip.SetToolTip(exportModsBtn, Constants.EXPORT_MODS_STR);
      ToolTip importModsToolTip = new();
      exportModsToolTip.SetToolTip(importModsBtn, Constants.IMPORT_MODS_STR);
      ToolTip startServerToolTip = new();
      startServerToolTip.SetToolTip(startServerBtn, Constants.START_SERVER_STR);
      ToolTip downloadToolTip = new();
      downloadToolTip.SetToolTip(downloadSteamCmdBtn, Constants.DOWNLOAD_SERVER_FILES_STR);
      ToolTip locateServerToolTip = new();
      locateServerToolTip.SetToolTip(locateServerFilesBtn, Constants.LOCATE_SERVER_FILES_STR);
      ToolTip deleteServerToolTip = new();
      deleteServerToolTip.SetToolTip(deleteServerFilesBtn, Constants.DELETE_SERVER_FILES_STR);
      ToolTip useExperimentalToolTip = new();
      useExperimentalToolTip.SetToolTip(useExperimentalCheckBox, Constants.USE_EXPERIMENTAL_STR);
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
          steamCmdAlert.Text = $"Using Arma Reforger Server found at: \"{FileIOManager.GetInstance().GetInstallDirectory()}\"";
          downloadSteamCmdBtn.Enabled = false;
          startServerBtn.Enabled = true;
          deleteServerFilesBtn.Enabled = true;
          loadSaveGameBtn.Enabled = true;
        }
        else
        {
          steamCmdAlert.Text = "SteamCMD and the server files were not detected, please Download before continuing.";
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
    public ListBox GetEnabledModsList()
    {
      return enabledMods;
    }

    /// <summary>
    /// Get the Available Mods ListBox
    /// </summary>
    /// <returns>availableMods ListBox</returns>
    public ListBox GetAvailableModsList()
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
    /// Remove the selected mod from the Available Mods ListBox when the "Remove Mod" button is pressed. (multiple selection supported)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveSelectedModBtnPressed(object sender, EventArgs e)
    {
      Mod[] modsToDelete = new Mod[GetAvailableModsList().SelectedItems.Count];
      GetAvailableModsList().SelectedItems.CopyTo(modsToDelete, 0);

      var availableMods = ConfigurationManager.GetInstance().GetAvailableMods();
      bool hasDeletedAtLeastOne = false;

      foreach (Mod mod in modsToDelete)
      {
        if (availableMods.Remove(mod))
          hasDeletedAtLeastOne = true;
      }

      if (hasDeletedAtLeastOne)
      {
        FileIOManager.GetInstance().WriteModsDatabase();
      }
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
      GetAvailableModsList().SelectedItems.CopyTo(modsToMove, 0);
      foreach (Mod mod in modsToMove)
      {
        // Move mod from Available Mods -> Enabled Mods
        ConfigurationManager.MoveMod(mod, ConfigurationManager.GetInstance().GetAvailableMods(),
                                        ConfigurationManager.GetInstance().GetEnabledMods());
      }
      ConfigurationManager.GetInstance().AlphabetiseModLists();
      ResetModFilters();
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
      GetEnabledModsList().SelectedItems.CopyTo(modsToMove, 0);
      foreach (Mod mod in modsToMove)
      {
        // Move mod from Enabled Mods -> Available Mods
        ConfigurationManager.MoveMod(mod, ConfigurationManager.GetInstance().GetEnabledMods(),
                                        ConfigurationManager.GetInstance().GetAvailableMods());
      }
      ConfigurationManager.GetInstance().AlphabetiseModLists();
      ResetModFilters();
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
        Mod m = (Mod)GetEnabledModsList().SelectedItem;

        // Set move backward to true as moving position 'up' actually means moving the mod earlier in the list
        Utilities.MoveItem(ConfigurationManager.GetInstance().GetEnabledMods(), m, true);

        // Re-select the mod so we can do multiple moves in a row if we like
        GetEnabledModsList().SelectedItems.Clear();
        GetEnabledModsList().SelectedItem = m;
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
        Mod m = (Mod)GetEnabledModsList().SelectedItem;

        // Move forward is the default, this will mean moving the mod later in the list
        Utilities.MoveItem(ConfigurationManager.GetInstance().GetEnabledMods(), m);

        // Re-select the mod so we can do multiple moves in a row if we like
        GetEnabledModsList().SelectedItems.Clear();
        GetEnabledModsList().SelectedItem = m;
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
      TextInputForm tif = new("Edit Mission Header");
      tif.ShowDialog();
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
        ParameterFriendlyName = "Server Name",
        ParameterTooltip = Constants.SERVER_PARAM_NAME_TOOLTIP_STR
      };
      serverParameters.Controls.Add(serverName);
      ServerParameterString serverPassword = new()
      {
        ParameterName = "password",
        ParameterFriendlyName = "Server Password",
        ParameterTooltip = Constants.SERVER_PARAM_PASSWORD_TOOLTIP_STR
      };
      serverParameters.Controls.Add(serverPassword);
      ServerParameterString adminPassword = new()
      {
        ParameterName = "passwordAdmin",
        ParameterFriendlyName = "Admin Password",
        ParameterTooltip = Constants.SERVER_PARAM_ADMIN_PASSWORD_TOOLTIP_STR
      };
      serverParameters.Controls.Add(adminPassword);
      ServerParameterList admins = new()
      {
        ParameterName = "admins",
        ParameterFriendlyName = "Admins",
        ParameterTooltip = Constants.SERVER_PARAM_ADMINS_TOOLTIP_STR,
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.game.admins
      };
      serverParameters.Controls.Add(admins);
      ServerParameterBool modsRequiredByDefault = new()
      {
        ParameterName = "modsRequiredByDefault",
        ParameterFriendlyName = "Mods Required by Default",
        ParameterTooltip = Constants.SERVER_PARAM_MODS_REQUIRED_BY_DEFAULT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(modsRequiredByDefault);
      ServerParameterNumeric maxPlayers = new()
      {
        ParameterName = "maxPlayers",
        ParameterFriendlyName = "Max Players",
        ParameterIncrement = 1,
        ParameterMin = Game.MIN_PLAYERS,
        ParameterMax = Game.MAX_PLAYERS,
        ParameterValue = Game.DEFAULT_PLAYERS,
        ParameterTooltip = Constants.SERVER_PARAM_MAX_PLAYERS_TOOLTIP_STR
      };
      serverParameters.Controls.Add(maxPlayers);
      ServerParameterBool visible = new()
      {
        ParameterName = "visible",
        ParameterFriendlyName = "Server Visible",
        ParameterValue = Game.DEFAULT_VISIBLE,
        ParameterTooltip = Constants.SERVER_PARAM_VISIBLE_TOOLTIP_STR
      };
      serverParameters.Controls.Add(visible);
      ServerParameterString bindAddress = new()
      {
        ParameterName = "bindAddress",
        ParameterFriendlyName = "Bind Address",
        ParameterValue = Root.DEFAULT_BIND_ADDRESS,
        ParameterTooltip = Constants.SERVER_PARAM_BIND_ADDRESS_TOOLTIP_STR
      };
      serverParameters.Controls.Add(bindAddress);
      ServerParameterNumeric bindPort = new()
      {
        ParameterName = "bindPort",
        ParameterFriendlyName = "Bind Port",
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = Root.DEFAULT_PORT,
        ParameterTooltip = Constants.SERVER_PARAM_BIND_PORT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(bindPort);
      ServerParameterString publicAddress = new()
      {
        ParameterName = "publicAddress",
        ParameterFriendlyName = "Public Address",
        ParameterTooltip = Constants.SERVER_PARAM_PUBLIC_ADDRESS_TOOLTIP_STR
      };
      serverParameters.Controls.Add(publicAddress);
      ServerParameterNumeric publicPort = new()
      {
        ParameterName = "publicPort",
        ParameterFriendlyName = "Public Port",
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = Root.DEFAULT_PORT,
        ParameterTooltip = Constants.SERVER_PARAM_PUBLIC_PORT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(publicPort);
      ServerParameterString a2sAddress = new()
      {
        ParameterName = "address",
        ParameterFriendlyName = "A2S Address",
        ParameterValue = A2S.DEFAULT_ADDRESS,
        ParameterTooltip = Constants.SERVER_PARAM_A2S_ADDRESS_TOOLTIP_STR
      };
      serverParameters.Controls.Add(a2sAddress);
      ServerParameterNumeric a2sPort = new()
      {
        ParameterName = "port",
        ParameterFriendlyName = "A2S Port",
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = A2S.DEFAULT_PORT,
        ParameterTooltip = Constants.SERVER_PARAM_A2S_PORT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(a2sPort);
      ServerParameterBool rconEnabled = new()
      {
        ParameterName = "rconEnabled",
        ParameterFriendlyName = "Enable Rcon",
        ParameterTooltip = Constants.SERVER_PARAM_ENABLE_RCON_TOOLTIP_STR
      };
      serverParameters.Controls.Add(rconEnabled);
      ServerParameterString rconAddress = new()
      {
        ParameterName = "rconAddress",
        ParameterFriendlyName = "Rcon Address",
        ParameterTooltip = Constants.SERVER_PARAM_RCON_ADDRESS_TOOLTIP_STR
      };
      serverParameters.Controls.Add(rconAddress);
      ServerParameterNumeric rconPort = new()
      {
        ParameterName = "rconPort",
        ParameterFriendlyName = "Rcon Port",
        ParameterIncrement = 1,
        ParameterMin = Constants.SERVER_PARAM_MIN_PORT,
        ParameterMax = Constants.SERVER_PARAM_MAX_PORT,
        ParameterValue = Rcon.DEFAULT_PORT,
        ParameterTooltip = Constants.SERVER_PARAM_RCON_PORT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(rconPort);
      ServerParameterString rconPassword = new()
      {
        ParameterName = "rconPassword",
        ParameterFriendlyName = "Rcon Password",
        ParameterTooltip = Constants.SERVER_PARAM_RCON_PASSWORD_TOOLTIP_STR
      };
      serverParameters.Controls.Add(rconPassword);
      ServerParameterNumeric rconMaxClients = new()
      {
        ParameterName = "rconMaxClients",
        ParameterFriendlyName = "Rcon Max Clients",
        ParameterTooltip = Constants.SERVER_PARAM_RCON_MAX_CLIENTS_TOOLTIP_STR,
        ParameterIncrement = 1,
        ParameterMin = Rcon.MIN_CLIENTS,
        ParameterMax = Rcon.MAX_CLIENTS,
        ParameterValue = Rcon.DEFAULT_CLIENTS
      };
      serverParameters.Controls.Add(rconMaxClients);
      ServerParameterSelect rconPermission = new()
      {
        ParameterName = "rconPermission",
        ParameterFriendlyName = "Rcon Permission",
        ParameterTooltip = Constants.SERVER_PARAM_RCON_PERMISSION_TOOLTIP_STR,
        ParameterValue = Rcon.PERMISSIONS,
      };
      rconPermission.ParameterValueSelection(Rcon.DEFAULT_PERMISSION);
      serverParameters.Controls.Add(rconPermission);
      ServerParameterList rconWhitelist = new()
      {
        ParameterName = "rconWhitelist",
        ParameterFriendlyName = "Rcon Whitelist",
        ParameterTooltip = Constants.SERVER_PARAM_RCON_WHITELIST_TOOLTIP_STR,
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.rcon.whitelist
      };
      serverParameters.Controls.Add(rconWhitelist);
      ServerParameterList rconBlacklist = new()
      {
        ParameterName = "rconBlacklist",
        ParameterFriendlyName = "Rcon Blacklist",
        ParameterTooltip = Constants.SERVER_PARAM_RCON_BLACKLIST_TOOLTIP_STR,
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.rcon.blacklist
      };
      serverParameters.Controls.Add(rconBlacklist);
      ServerParameterNumeric playerSaveTime = new()
      {
        ParameterName = "playerSaveTime",
        ParameterFriendlyName = "Player Save Time (secs)",
        ParameterIncrement = 1,
        ParameterMin = Operating.MIN_PLAYER_SAVE_TIME,
        ParameterMax = Operating.MAX_PLAYER_SAVE_TIME,
        ParameterValue = Operating.DEFAULT_PLAYER_SAVE_TIME,
        ParameterTooltip = Constants.SERVER_PARAM_PLAYER_SAVE_TIME_TOOLTIP_STR
      };
      serverParameters.Controls.Add(playerSaveTime);
      ServerParameterNumeric serverMaxViewDistance = new()
      {
        ParameterName = "serverMaxViewDistance",
        ParameterFriendlyName = "Server Max View Distance",
        ParameterIncrement = 1,
        ParameterMin = GameProperties.MIN_SERVER_VIEW_DISTANCE,
        ParameterMax = GameProperties.MAX_SERVER_VIEW_DISTANCE,
        ParameterValue = GameProperties.DEFAULT_SERVER_VIEW_DISTANCE,
        ParameterTooltip = Constants.SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_TOOLTIP_STR
      };
      serverParameters.Controls.Add(serverMaxViewDistance);
      ServerParameterNumeric serverMinGrassDistance = new()
      {
        ParameterName = "serverMinGrassDistance",
        ParameterFriendlyName = "Server Min Grass Distance",
        ParameterIncrement = 1,
        ParameterMin = GameProperties.MIN_SERVER_GRASS_DISTANCE,
        ParameterMax = GameProperties.MAX_SERVER_GRASS_DISTANCE,
        ParameterValue = GameProperties.DEFAULT_SERVER_GRASS_DISTANCE,
        ParameterTooltip = Constants.SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_TOOLTIP_STR
      };
      serverParameters.Controls.Add(serverMinGrassDistance);
      ServerParameterNumeric networkViewDistance = new()
      {
        ParameterName = "networkViewDistance",
        ParameterFriendlyName = "Network View Distance",
        ParameterIncrement = 1,
        ParameterMin = GameProperties.MIN_NETWORK_VIEW_DISTANCE,
        ParameterMax = GameProperties.MAX_NETWORK_VIEW_DISTANCE,
        ParameterValue = GameProperties.DEFAULT_NETWORK_VIEW_DISTANCE,
        ParameterTooltip = Constants.SERVER_PARAM_NETWORK_VIEW_DISTANCE_TOOLTIP_STR
      };
      serverParameters.Controls.Add(networkViewDistance);
      ServerParameterBool disableThirdPerson = new()
      {
        ParameterName = "disableThirdPerson",
        ParameterFriendlyName = "Disable Third Person",
        ParameterValue = GameProperties.DEFAULT_DISABLE_THIRD_PERSON,
        ParameterTooltip = Constants.SERVER_PARAM_DISABLE_THIRD_PERSON_TOOLTIP_STR
      };
      serverParameters.Controls.Add(disableThirdPerson);
      ServerParameterBool fastValidation = new()
      {
        ParameterName = "fastValidation",
        ParameterFriendlyName = "Fast Validation",
        ParameterValue = GameProperties.DEFAULT_FAST_VALIDATION,
        ParameterTooltip = Constants.SERVER_PARAM_FAST_VALIDATION_TOOLTIP_STR
      };
      serverParameters.Controls.Add(fastValidation);
      ServerParameterBool battlEye = new()
      {
        ParameterName = "battlEye",
        ParameterFriendlyName = "BattlEye",
        ParameterValue = GameProperties.DEFAULT_BATTLE_EYE,
        ParameterTooltip = Constants.SERVER_PARAM_BATTLEYE_TOOLTIP_STR
      };
      serverParameters.Controls.Add(battlEye);
      ServerParameterBool lobbyPlayerSynchronise = new()
      {
        ParameterName = "lobbyPlayerSynchronise",
        ParameterFriendlyName = "Lobby Player Synchronise",
        ParameterValue = Operating.DEFAULT_LOBBY_PLAYER_SYNCHRONISE,
        ParameterTooltip = Constants.SERVER_PARAM_LOBBY_PLAYER_SYNC_TOOLTIP_STR
      };
      serverParameters.Controls.Add(lobbyPlayerSynchronise);
      ServerParameterBool vonDisableUI = new()
      {
        ParameterName = "VONDisableUI",
        ParameterFriendlyName = "VON Disable UI",
        ParameterValue = GameProperties.DEFAULT_VON_DISABLE_UI,
        ParameterTooltip = Constants.SERVER_PARAM_VON_DISABLE_UI_TOOLTIP_STR
      };
      serverParameters.Controls.Add(vonDisableUI);
      ServerParameterBool vonDisableDirectSpeechUI = new()
      {
        ParameterName = "VONDisableDirectSpeechUI",
        ParameterFriendlyName = "VON Disable Direct Speech UI",
        ParameterValue = GameProperties.DEFAULT_VON_DISABLE_DIRECT_SPEECH_UI,
        ParameterTooltip = Constants.SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_TOOLTIP_STR
      };
      serverParameters.Controls.Add(vonDisableDirectSpeechUI);
      ServerParameterBool vonCanTransmitCrossFaction = new()
      {
        ParameterName = "VONCanTransmitCrossFaction",
        ParameterFriendlyName = "VON Can Transmit Cross Faction",
        ParameterValue = GameProperties.DEFAULT_VON_CAN_TRANSMIT_CROSS_FACTION,
        ParameterTooltip = Constants.SERVER_PARAM_VON_CAN_TRANSMIT_ACROSS_FACTION_TOOLTIP_STR
      };
      serverParameters.Controls.Add(vonCanTransmitCrossFaction);
      ServerParameterBool crossPlatform = new()
      {
        ParameterName = "crossPlatform",
        ParameterFriendlyName = "Cross Platform",
        ParameterValue = Game.DEFAULT_CROSS_PLATFORM,
        ParameterTooltip = Constants.SERVER_PARAM_CROSS_PLATFORM_TOOLTIP_STR
      };
      serverParameters.Controls.Add(crossPlatform);
      ServerParameterNumeric aiLimit = new()
      {
        ParameterName = "aiLimit",
        ParameterFriendlyName = "AI Limit",
        ParameterIncrement = 1,
        ParameterMin = Operating.MIN_AI_LIMIT,
        ParameterMax = Operating.MAX_AI_LIMIT,
        ParameterValue = Operating.DEFAULT_AI_LIMIT,
        ParameterTooltip = Constants.SERVER_PARAM_AI_LIMIT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(aiLimit);
      ServerParameterNumeric slotReservationTimeout = new()
      {
        ParameterName = "slotReservationTimeout",
        ParameterFriendlyName = "Slot Reservation Timeout (secs)",
        ParameterIncrement = 1,
        ParameterMin = Operating.MIN_SLOT_RESERVATION_TIMEOUT,
        ParameterMax = Operating.MAX_SLOT_RESERVATION_TIMEOUT,
        ParameterValue = Operating.DEFAULT_SLOT_RESERVATION_TIMEOUT,
        ParameterTooltip = Constants.SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(slotReservationTimeout);
      ServerParameterBool toggleDisableNavmeshStreaming = new()
      {
        ParameterName = "toggleDisableNavmeshStreaming",
        ParameterFriendlyName = "Disable Navmesh Streaming",
        ParameterTooltip = Constants.SERVER_PARAM_DISABLE_NAVMESH_STREAMING_TOOLTIP_STR
      };
      serverParameters.Controls.Add(toggleDisableNavmeshStreaming);
      ServerParameterList disableNavmeshStreaming = new()
      {
        ParameterName = "disableNavmeshStreaming",
        ParameterFriendlyName = "Disable Specific Navmesh Streaming",
        ParameterTooltip = Constants.SERVER_PARAM_DISABLE_SPECIFIC_NAVMESH_STREAMING_TOOLTIP_STR,
        ParameterList = ConfigurationManager.GetInstance()
                                              .GetServerConfiguration()
                                              .root.operating.disableNavmeshStreaming
      };
      serverParameters.Controls.Add(disableNavmeshStreaming);
      ServerParameterBool disableServerShutdown = new()
      {
        ParameterName = "disableServerShutdown",
        ParameterFriendlyName = "Disable Server Shutdown",
        ParameterValue = Operating.DEFAULT_DISABLE_SERVER_SHUTDOWN,
        ParameterTooltip = Constants.SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_TOOLTIP_STR
      };
      serverParameters.Controls.Add(disableServerShutdown);
      ServerParameterBool disableCrashReporter = new()
      {
        ParameterName = "disableCrashReporter",
        ParameterFriendlyName = "Disable Crash Reporter",
        ParameterValue = Operating.DEFAULT_DISABLE_CRASH_REPORTER,
        ParameterTooltip = Constants.SERVER_PARAM_DISABLE_CRASH_REPORT_TOOLTIP_STR
      };
      serverParameters.Controls.Add(disableCrashReporter);
      ServerParameterBool disableAI = new()
      {
        ParameterName = "disableAI",
        ParameterFriendlyName = "Disable AI",
        ParameterValue = Operating.DEFAULT_DISABLE_AI,
        ParameterTooltip = Constants.SERVER_PARAM_DISABLE_AI_TOOLTIP_STR
      };
      serverParameters.Controls.Add(disableAI);
      ServerParameterNumeric joinQueueMaxSize = new()
      {
        ParameterName = "maxSize",
        ParameterFriendlyName = "Join Queue Max Size",
        ParameterValue = JoinQueue.DEFAULT_MAX_SIZE,
        ParameterTooltip = Constants.SERVER_PARAM_JOIN_QUEUE_MAX_SIZE_TOOLTIP_STR
      };
      serverParameters.Controls.Add(joinQueueMaxSize);

      foreach (ServerParameter param in serverParameters.Controls)
      {
        ConfigurationManager.GetInstance().GetServerParametersDictionary()[param.ParameterName] = param;
      }
    }

    void CreateAdvancedServerParameterControls()
    {
      Dictionary<string, AdvancedSetting> loadedSettings = SavedStateManager.GetInstance().GetSavedState().advancedSettings;

      AdvancedServerParameterNumeric limitServerMaxFPS = new()
      {
        ParameterName = "maxFPS",
        ParameterFriendlyName = "Limit Server Max FPS",
        ParameterMin = 1,
        ParameterMax = 1000,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["maxFPS"].Value,
        Description = "Limits your server to the specified target FPS. Recommended."
      };
      limitServerMaxFPS.CheckBox.Checked = loadedSettings["maxFPS"].Enabled;
      advancedParametersPanel.Controls.Add(limitServerMaxFPS);

      AdvancedServerParameterTime autoRestartTime = new()
      {
        ParameterName = "autoRestartDaily",
        ParameterFriendlyName = "Auto Restart",
        Description = "Specify, in 24 hour time, at what time the server will restart every day.",
        ParameterMin = DateTime.Today,
        ParameterMax = DateTime.Today.AddDays(1).AddMinutes(-1),
        ParameterValue = DateTime.Today
      };
      advancedParametersPanel.Controls.Add(autoRestartTime);

      AdvancedServerParameterBool addonsRepair = new()
      {
        ParameterName = "addonsRepair",
        ParameterFriendlyName = "Verify and Repair Addons",
        Description ="Verifies the integrity of all installed addons. If any corrupted addons are found, they will be repaired automatically."
      };
      addonsRepair.CheckBox.Checked = loadedSettings["addonsRepair"].Enabled;
      advancedParametersPanel.Controls.Add(addonsRepair);

      AdvancedServerParameterBool autoRestartOnCrash = new()
      {
        ParameterName = "autoRestartOnCrash",
        ParameterFriendlyName = "Restart on Game Destroyed",
        Description = "The tool will monitor the server for crashes and attempt to restart it automatically."
      };
      autoRestartOnCrash.CheckBox.CheckedChanged += AutoRestartOnCrashCheckChanged;
      autoRestartOnCrash.CheckBox.Checked = loadedSettings.ContainsKey("autoRestartOnCrash") ? loadedSettings["autoRestartOnCrash"].Enabled : false;
      advancedParametersPanel.Controls.Add(autoRestartOnCrash);

      AdvancedServerParameterNumeric autoReload = new()
      {
        ParameterName = "autoreload",
        ParameterFriendlyName = "Auto Reload Scenario",
        Description = "Automatically reload the scenario when finished after the specified time (in seconds) has elapsed.",
        ParameterMin = 1,
        ParameterMax = int.MaxValue,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["autoreload"].Value
      };
      autoReload.CheckBox.Checked = loadedSettings["autoreload"].Enabled;
      advancedParametersPanel.Controls.Add(autoReload);

      AdvancedServerParameterBool noBackend = new()
      {
        ParameterName = "noBackend",
        ParameterFriendlyName = "No Backend",
        Description = "Enable this to host the server without using the Arma Reforger backend."
      };
      noBackend.CheckBox.CheckedChanged += NoBackendCheckChanged;
      noBackend.CheckBox.Checked = loadedSettings["noBackend"].Enabled;
      advancedParametersPanel.Controls.Add(noBackend);

      AdvancedServerParameterBool autoShutdown = new()
      {
        ParameterName = "autoShutdown",
        ParameterFriendlyName = "Auto Shutdown",
        Description = "Ensures the correct server shutdown process, use with \"Auto Reload\"."
      };
      autoShutdown.CheckBox.Checked = loadedSettings["autoShutdown"].Enabled;
      advancedParametersPanel.Controls.Add(autoShutdown);

      AdvancedServerParameterBool logVoting = new()
      {
        ParameterName = "logVoting",
        ParameterFriendlyName = "Log Voting",
        Description = "Adds logging info to the voting system with information about who created, voted, and against whom the vote was created."
      };
      logVoting.CheckBox.Checked = loadedSettings["logVoting"].Enabled;
      advancedParametersPanel.Controls.Add(logVoting);

      AdvancedServerParameterNumeric overridePort = new()
      {
        ParameterName = "bindPort",
        ParameterFriendlyName = "Override Port",
        ParameterMin = 1,
        ParameterMax = 65535,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["bindPort"].Value,
        Description = "Override the ports specified in server configuration."
      };
      overridePort.CheckBox.Checked = loadedSettings["bindPort"].Enabled;
      advancedParametersPanel.Controls.Add(overridePort);

      AdvancedServerParameterNumeric networkDynamicSim = new()
      {
        ParameterName = "nds",
        ParameterFriendlyName = "Network Dynamic Simulation",
        ParameterMin = 0,
        ParameterMax = 2,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["nds"].Value,
        Description = "This is set to '2' by default if unchecked."
      };
      networkDynamicSim.CheckBox.Checked = loadedSettings["nds"].Enabled;
      advancedParametersPanel.Controls.Add(networkDynamicSim);

      AdvancedServerParameterNumeric spatialMapRes = new()
      {
        ParameterName = "nwkResolution",
        ParameterFriendlyName = "Spatial Map Resolution",
        ParameterMin = 100,
        ParameterMax = 1000,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["nwkResolution"].Value,
        Description = "Defines what resolution Spatial Map cells should be set at in a 100 - 1000m range."
      };
      spatialMapRes.CheckBox.Checked = loadedSettings["nwkResolution"].Enabled;
      advancedParametersPanel.Controls.Add(spatialMapRes);

      AdvancedServerParameterNumeric staggeringBudget = new()
      {
        ParameterName = "staggeringBudget",
        ParameterFriendlyName = "Staggering Budget",
        ParameterMin = 1,
        ParameterMax = 10201,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["staggeringBudget"].Value,
        Description = "Defines how many stationary spatial map cells are allowed to be processed in one tick. If not set it uses \"-nds\" diameter."
      };
      staggeringBudget.CheckBox.Checked = loadedSettings["staggeringBudget"].Enabled;
      advancedParametersPanel.Controls.Add(staggeringBudget);

      AdvancedServerParameterNumeric streamingBudget = new()
      {
        ParameterName = "streamingBudget",
        ParameterFriendlyName = "Streaming Budget",
        ParameterMin = 100,
        ParameterMax = 10201,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["streamingBudget"].Value,
        Description = "Streaming budget is the global streaming budget that is equally distributed between all connections."
      };
      streamingBudget.CheckBox.Checked = loadedSettings["streamingBudget"].Enabled;
      advancedParametersPanel.Controls.Add(streamingBudget);

      AdvancedServerParameterNumeric streamsDelta = new()
      {
        ParameterName = "streamsDelta",
        ParameterFriendlyName = "Streams Delta",
        ParameterMin = 1,
        ParameterMax = 1000,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["streamsDelta"].Value,
        Description = "Streams delta is a tool to limit the amount of streams being opened for a client."
      };
      streamsDelta.CheckBox.Checked = loadedSettings["streamsDelta"].Enabled;
      advancedParametersPanel.Controls.Add(streamsDelta);

      AdvancedServerParameterNumeric rplTimeoutMs = new()
      {
        ParameterName = "rpl-timeout-ms",
        ParameterFriendlyName = "RPL Timeout",
        ParameterMin = 1,
        ParameterMax = int.MaxValue,
        ParameterIncrement = 1,
        ParameterValue = loadedSettings["rpl-timeout-ms"].Value,
        Description = "Sets the server's timeout value, in milliseconds."
      };
      rplTimeoutMs.CheckBox.Checked = loadedSettings["rpl-timeout-ms"].Enabled;
      advancedParametersPanel.Controls.Add(rplTimeoutMs);

      AdvancedServerParameterBool aiPartialSim = new()
      {
        ParameterName = "aiPartialSim",
        ParameterFriendlyName = "AI Partial Sim",
        Description = "Sets in how many batches all simulable AIs will divided and processed."
      };
      aiPartialSim.CheckBox.Checked = loadedSettings["aiPartialSim"].Enabled;
      advancedParametersPanel.Controls.Add(aiPartialSim);

      AdvancedServerParameterBool createDB = new()
      {
        ParameterName = "createDB",
        ParameterFriendlyName = "Force Recreate Database",
        Description = "Forces database file's regeneration. Useful after file directories changes, when some resources were moved elsewhere."
      };
      createDB.CheckBox.Checked = loadedSettings["createDB"].Enabled;
      advancedParametersPanel.Controls.Add(createDB);

      AdvancedServerParameterString debugger = new()
      {
        ParameterName = "debugger",
        ParameterFriendlyName = "Debugger Address",
        ParameterPlaceholder = "127.0.0.1",
        Description = "Sets the script debugger to a specific address."
      };
      bool debuggerEnabled = loadedSettings["debugger"].Enabled;
      debugger.CheckBox.Checked = debuggerEnabled;
      if (debuggerEnabled)
      {
        debugger.ParameterValue = loadedSettings["debugger"].Value;
      }
      advancedParametersPanel.Controls.Add(debugger);

      AdvancedServerParameterNumeric debuggerPort = new()
      {
        ParameterName = "debuggerPort",
        ParameterFriendlyName = "Debugger Port",
        ParameterIncrement = 1,
        ParameterMin = 1,
        ParameterMax = 65535,
        ParameterValue = loadedSettings["debuggerPort"].Value,
        Description = "Sets the script debugger to a specific port. "
      };
      debuggerPort.CheckBox.Checked = loadedSettings["debuggerPort"].Enabled;
      advancedParametersPanel.Controls.Add(debuggerPort);

      AdvancedServerParameterBool disableShadersBuild = new()
      {
        ParameterName = "disableShadersBuild",
        ParameterFriendlyName = "Disable Shaders Generation",
        Description = "Disables shaders generation."
      };
      disableShadersBuild.CheckBox.Checked = loadedSettings["disableShadersBuild"].Enabled;
      advancedParametersPanel.Controls.Add(disableShadersBuild);

      AdvancedServerParameterBool generateShaders = new()
      {
        ParameterName = "generateShaders",
        ParameterFriendlyName = "Force Generate Shaders",
        Description = "Forces shaders generation."
      };
      generateShaders.CheckBox.Checked = loadedSettings["generateShaders"].Enabled;
      advancedParametersPanel.Controls.Add(generateShaders);

      AdvancedServerParameterBool rplEncodeAsLongJobs = new()
      {
        ParameterName = "rplEncodeAsLongJobs",
        ParameterFriendlyName = "RPL Encode as Long Jobs",
        Description = "Makes replication use long encoding jobs instead of short ones."
      };
      rplEncodeAsLongJobs.CheckBox.Checked = loadedSettings["rplEncodeAsLongJobs"].Enabled;
      advancedParametersPanel.Controls.Add(rplEncodeAsLongJobs);

      AdvancedServerParameterNumeric jobsysShortWorkerCount = new()
      {
        ParameterName = "jobsysShortWorkerCount",
        ParameterFriendlyName = "Short Worker Count",
        Description = "Sets the number of threads working on short jobs (jobs that must finish in one update loop).",
        ParameterMin = 1,
        ParameterMax = Utilities.GetNumberAvailableThreads(),
        ParameterValue = loadedSettings["jobsysShortWorkerCount"].Value
      };
      jobsysShortWorkerCount.CheckBox.Checked = loadedSettings["jobsysShortWorkerCount"].Enabled;
      advancedParametersPanel.Controls.Add(jobsysShortWorkerCount);

      AdvancedServerParameterNumeric jobsysLongWorkerCount = new()
      {
        ParameterName = "jobsysLongWorkerCount",
        ParameterFriendlyName = "Long Worker Count",
        Description = "Sets the number of threads working on long jobs (jobs that can span multiple iterations of update loop).",
        ParameterMin = 1,
        ParameterMax = Utilities.GetNumberAvailableThreads(),
        ParameterValue = loadedSettings["jobsysLongWorkerCount"].Value
      };
      jobsysLongWorkerCount.CheckBox.Checked = loadedSettings["jobsysLongWorkerCount"].Enabled;
      advancedParametersPanel.Controls.Add(jobsysLongWorkerCount);

      AdvancedServerParameterNumeric freezeCheck = new()
      {
        ParameterName = "freezeCheck",
        ParameterFriendlyName = "Freeze Check",
        Description = "Overrides time in seconds to forcefully crash on application freeze or completely disable detection.",
        ParameterIncrement = 1,
        ParameterMin = 0,
        ParameterMax = 600,
        ParameterValue = loadedSettings["freezeCheck"].Value
      };
      freezeCheck.CheckBox.Checked = loadedSettings["freezeCheck"].Enabled;
      advancedParametersPanel.Controls.Add(freezeCheck);

      AdvancedServerParameterEnumerated freezeCheckMode = new()
      {
        ParameterName = "freezeCheckMode",
        ParameterFriendlyName = "Freeze Check Mode",
        Description = "Overrides behavior which should happen when freeze is detected.",
        ParameterAvailableValues = new List<string>() {"crash", "minidump", "kill"}
      };
      bool freezeCheckModeEnabled = loadedSettings["freezeCheckMode"].Enabled;
      freezeCheckMode.CheckBox.Checked = freezeCheckModeEnabled;
      if (freezeCheckModeEnabled)
      {
        freezeCheckMode.ParameterValue = loadedSettings["freezeCheckMode"].Value;
      }
      advancedParametersPanel.Controls.Add(freezeCheckMode);

      AdvancedServerParameterBool forceDisableNightGrain = new()
      {
        ParameterName = "forceDisableNightGrain",
        ParameterFriendlyName = "Force Disable Night Grain",
        Description = "Disables night grain in multiplayer.",
      };
      forceDisableNightGrain.CheckBox.Checked = loadedSettings["forceDisableNightGrain"].Enabled;
      advancedParametersPanel.Controls.Add(forceDisableNightGrain);

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
        bool enableNoBackend = Utilities.DisplayConfirmationMessage("Setting your server to use No Backend means it will not be visible in the server browser.\r\n" +
            "Mods not already downloaded will not work as they will not be fetched from the Workshop.\r\n" +
            "You must provide a valid Public Address in the Server Configuration section. It cannot be empty.\r\n" +
            "Clients will only be able to connect via the '-client' launch argument, and it is their responsibility to acquire required mods.\r\n\r\n" +
            "Continue?", true);

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
      }
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
          loadedScenarioLabel.Text = "No scenario ID chosen.";
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
      UpdateStateForAdvancedSettings();

      // Update state of UPnp and Experimental
      SavedStateManager.GetInstance().GetSavedState().advancedSettings["useUpnp"].Enabled = useUpnp.Checked;
      SavedStateManager.GetInstance().GetSavedState().advancedSettings["useExperimental"].Enabled = useExperimentalCheckBox.Checked;

      FileIOManager.GetInstance().WriteStateFile();
      FileIOManager.GetInstance().WriteModsDatabase();
    }

    private void LoadSaveGameBtnPressed(object sender, EventArgs e)
    {
      SpawnSaveSelect();
    }
  }
}
