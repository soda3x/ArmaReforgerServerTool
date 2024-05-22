using ReforgerServerApp.Managers;
using ReforgerServerApp.Models;
using System.Diagnostics;

namespace ReforgerServerApp
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            CreateServerParameterControls();

            serverRunningLabel.Text = string.Empty;

            ProcessManager.GetInstance().UpdateGuiControlsEvent += HandleUpdateGuiControlsEvent;
            ProcessManager.GetInstance().UpdateSteamCmdLogEvent += HandleUpdateSteamCmdLogEvent;
            ConfigurationManager.GetInstance().UpdateScenarioIdFromLoadedConfigEvent += HandleUpdateScenarioIdFromLoadedConfigEvent;
            
            // Create tooltips
            CreateTooltips();

            loadedScenarioLabel.Text = "No scenario ID chosen.";

            UpdateSteamCmdInstallStatus();

            fpsLimitUpDown.Enabled            = false;
            restartIntervalUpDown.Enabled     = false;
            restartUnitsComboBox.Enabled      = false;
            overridePortNumericUpDown.Enabled = false;
            ndsUpDown.Enabled                 = false;
            nwkResolutionUpDown.Enabled       = false;
            staggeringBudgetUpDown.Enabled    = false;
            streamingBudgetUpDown.Enabled     = false;
            streamsDeltaUpDown.Enabled        = false;
            sessionSave.Enabled               = false;

            availableMods.DataSource = ConfigurationManager.GetInstance().GetAvailableMods();
            enabledMods.DataSource   = ConfigurationManager.GetInstance().GetEnabledMods();

            ConfigurationManager.GetInstance().AlphabetiseModLists();

            FileIOManager.CheckForUpdates();

            Mod.GetScenariosForMod("591AF5BDA9F7CE8B");
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
            ToolTip ndsToolTip = new();
            ndsToolTip.SetToolTip(ndsLabel, Constants.NDS_TOOLTIP_STR);
            ToolTip nwkResolutionToolTip = new();
            nwkResolutionToolTip.SetToolTip(nwkResolutionLabel, Constants.NWK_RESOL_TOOLTIP_STR);
            ToolTip staggeringBudgetToolTip = new();
            staggeringBudgetToolTip.SetToolTip(staggeringBudgetLabel, Constants.STAGGER_BDGT_TOOLTIP_STR);
            ToolTip streamingBudgetToolTip = new();
            streamingBudgetToolTip.SetToolTip(streamingBudgetLabel, Constants.STREAMING_BDGT_TOOLTIP_STR);
            ToolTip streamsDeltaToolTip = new();
            streamsDeltaToolTip.SetToolTip(streamsDeltaLabel, Constants.STREAMS_DELTA_TOOLTIP_STR);
            ToolTip loadSessionSaveToolTip = new();
            loadSessionSaveToolTip.SetToolTip(loadSessionSaveLabel, Constants.LOAD_SESSION_SAVE_TOOLTIP_STR);
        }

        /// <summary>
        /// This method is used to control the state of the controls used to Download SteamCMD and start the server.
        /// If SteamCMD is detected, The message telling the user to Download SteamCMD is hidden, 
        /// the Download button is disabled and the Start Server button is enabled.
        /// </summary>
        private void UpdateSteamCmdInstallStatus()
        {
            if (FileIOManager.GetInstance().IsSteamCMDInstalled())
            {
                steamCmdAlert.Text = $"Using Arma Reforger Server files found at: { FileIOManager.GetInstance().GetInstallDirectory() }";
                downloadSteamCmdBtn.Enabled = false;
                startServerBtn.Enabled = true;
                deleteServerFilesBtn.Enabled = true;

            }
            else
            {
                steamCmdAlert.Text = "SteamCMD and the Arma Server files were not detected, please Download before continuing.";
                startServerBtn.Enabled = false;
                downloadSteamCmdBtn.Enabled = true;
                deleteServerFilesBtn.Enabled = false;
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
            editModBtn.Enabled   = availableMods.SelectedItem != null;
            removeModBtn.Enabled = availableMods.SelectedItem != null;
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
        /// Remove the selected mod from the Available Mods ListBox when the "Remove Mod" button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelectedModBtnPressed(object sender, EventArgs e)
        {
            ConfigurationManager.GetInstance().GetAvailableMods().Remove((Mod) GetAvailableModsList().SelectedItem);
            FileIOManager.GetInstance().WriteModsDatabase();
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
            if ((Mod)GetAvailableModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetAvailableModsList().SelectedItem;
          
                if (!ConfigurationManager.GetInstance().GetEnabledMods().Contains(m))
                {
                    ConfigurationManager.GetInstance().GetEnabledMods().Add(new(m));
                }
                ConfigurationManager.GetInstance().GetAvailableMods().Remove(m);
            }
            ConfigurationManager.GetInstance().AlphabetiseModLists();
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
            if ((Mod) GetEnabledModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetEnabledModsList().SelectedItem;

                if (!ConfigurationManager.GetInstance().GetAvailableMods().Contains(m))
                {
                    ConfigurationManager.GetInstance().GetAvailableMods().Add(new(m));
                }
                ConfigurationManager.GetInstance().GetEnabledMods().Remove(m);
            }
            ConfigurationManager.GetInstance().AlphabetiseModLists(); 
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
            FileIOManager.GetInstance().DownloadSteamCMD();
            UpdateSteamCmdInstallStatus();
        }

        /// <summary>
        /// Handler for the LimitFPS Checkbox, enables / disables the FPS Limit Numeric Up Down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimitFPSCheckedChanged(object sender, EventArgs e)
        {
            if (limitFPS.Checked)
            {
                fpsLimitUpDown.Enabled = true;
            }
            else
            {
                fpsLimitUpDown.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for the Auto Restart Checkbox, enables / disables the Interval and Units Numeric Up Down and ComboBoxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoRestartCheckedChanged(object sender, EventArgs e)
        {
            if (automaticallyRestart.Checked)
            {
                restartIntervalUpDown.Enabled = true;
                restartUnitsComboBox.Enabled = true;
            }
            else
            {
                restartIntervalUpDown.Enabled = false;
                restartUnitsComboBox.Enabled = false;
            }
        }

        /// <summary>
        /// This is the handler for the Start Server Button. This is also used for the automatic server restart functionality.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartServerBtnPressed(object sender, EventArgs e)
        {
            // If we are starting the server for the first time and using the automatic restart functionality, configure the timer
            if (automaticallyRestart.Checked && !ProcessManager.GetInstance().IsServerUsingTimer())
            {
                // Create a timespan based on which units the user has selected
                // Use default value of 1 hour restarts so VS stops yelling at us
                TimeSpan interval = TimeSpan.FromHours(1);
                switch (restartUnitsComboBox.SelectedIndex)
                {
                    case (int) ServerRestartIntervalUnit.MINUTES:
                        interval = TimeSpan.FromMinutes((int)restartIntervalUpDown.Value);
                        break;
                    case (int) ServerRestartIntervalUnit.HOURS:
                        interval = TimeSpan.FromHours((int)restartIntervalUpDown.Value);
                        break;
                    case (int) ServerRestartIntervalUnit.DAYS:
                        interval = TimeSpan.FromDays((int)restartIntervalUpDown.Value);
                        break;
                }
                ProcessManager.GetInstance().ConfigureAutomaticRestartTask(interval);
            }

            // The user is turning the server off manually
            else if (automaticallyRestart.Checked && ProcessManager.GetInstance().IsServerUsingTimer())
            {
                ProcessManager.GetInstance().CancelAutomaticRestartTask();
            }

            // User just normally pressed the button
            else if (!automaticallyRestart.Checked && !ProcessManager.GetInstance().IsServerUsingTimer())
            {
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
            limitFPS.Enabled = enabled;
            fpsLimitUpDown.Enabled = enabled;
            automaticallyRestart.Enabled = enabled;
            forcePortCheckBox.Enabled = enabled;
            overridePortNumericUpDown.Enabled = enabled;
            nds.Enabled = enabled;
            ndsUpDown.Enabled = enabled;
            nwkResolution.Enabled = enabled;
            nwkResolutionUpDown.Enabled = enabled;
            staggeringBudget.Enabled = enabled;
            staggeringBudgetUpDown.Enabled = enabled;
            streamingBudget.Enabled = enabled;
            streamingBudgetUpDown.Enabled = enabled;
            streamsDelta.Enabled = enabled;
            streamsDeltaUpDown.Enabled = enabled;
            logLevelComboBox.Enabled = enabled;
            scenarioSelectBtn.Enabled = enabled;
            editMissionHeaderBtn.Enabled = enabled;
            sessionSave.Enabled = enabled;
            loadSessionSave.Enabled = enabled;
            editAdminListBtn.Enabled = enabled;

            // Handle these differently as we don't want them enabled if 'Automatically Restart' isn't enabled
            if (automaticallyRestart.Enabled && automaticallyRestart.Checked)
            {
                restartIntervalUpDown.Enabled = true;
                restartUnitsComboBox.Enabled = true;
            }
            else
            {
                restartIntervalUpDown.Enabled = false;
                restartUnitsComboBox.Enabled = false;
            }
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
        /// Handler for the Override Port Checkbox, enables / disables the Override Port field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverridePortCheckChanged(object sender, EventArgs e)
        {
            if (forcePortCheckBox.Checked)
            {
                overridePortNumericUpDown.Enabled = true;
            }
            else
            {
                overridePortNumericUpDown.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for the NDS Checkbox, enables / disables the NDS field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NDSCheckChanged(object sender, EventArgs e)
        {
            if (nds.Checked)
            {
                ndsUpDown.Enabled = true;
            }
            else
            {
                ndsUpDown.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for the NWK Resolution Checkbox, enables / disables the NWK Resolution field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NWKCheckChanged(object sender, EventArgs e)
        {
            if (nwkResolution.Checked)
            {
                nwkResolutionUpDown.Enabled = true;
            }
            else
            {
                nwkResolutionUpDown.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for the Staggering Budget Checkbox, enables / disables the Staggering Budget field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StaggeringBudgetCheckChanged(object sender, EventArgs e)
        {
            if (staggeringBudget.Checked)
            {
                staggeringBudgetUpDown.Enabled = true;
            }
            else
            {
                staggeringBudgetUpDown.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for the Streaming Budget Checkbox, enables / disables the Streaming Budget field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StreamingBudgetCheckChanged(object sender, EventArgs e)
        {
            if (streamingBudget.Checked)
            {
                streamingBudgetUpDown.Enabled = true;
            }
            else
            {
                streamingBudgetUpDown.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for the Streams Delta Checkbox, enables / disables the Streams Delta field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StreamsDeltaCheckChanged(object sender, EventArgs e)
        {
            if (streamsDelta.Checked)
            {
                streamsDeltaUpDown.Enabled = true;
            }
            else
            {
                streamsDeltaUpDown.Enabled = false;
            }
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

        private void EditMissionHeaderBtnClicked(object sender, EventArgs e)
        {
            TextInputForm tif = new("Edit Mission Header");
            tif.ShowDialog();
        }

        private void EditAdminsListBtnClicked(object sender, EventArgs e)
        {
            ListForm lf = new("Edit Admins");
            lf.ShowDialog();
        }

        /// <summary>
        /// Handler for the Load Session Save Checkbox, enables / disables the Load Session Save field
        /// and enables / disables the Load Session Save functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSessionSaveCheckChanged(object sender, EventArgs e)
        {
            sessionSave.Enabled = loadSessionSave.Checked;
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
            ServerParameterNumeric maxPlayers = new()
            {
                ParameterName = "maxPlayers",
                ParameterFriendlyName = "Max Players",
                ParameterIncrement = 1,
                ParameterMin = 1,
                ParameterMax = 256,
                ParameterValue = 64,
                ParameterTooltip = Constants.SERVER_PARAM_MAX_PLAYERS_TOOLTIP_STR
            };
            serverParameters.Controls.Add(maxPlayers);
            ServerParameterBool visible = new()
            {
                ParameterName = "visible",
                ParameterFriendlyName = "Server Visible",
                ParameterValue = true,
                ParameterTooltip = Constants.SERVER_PARAM_VISIBLE_TOOLTIP_STR
            };
            serverParameters.Controls.Add(visible);
            ServerParameterString bindAddress = new()
            {
                ParameterName = "bindAddress",
                ParameterFriendlyName = "Bind Address",
                ParameterValue = "0.0.0.0",
                ParameterTooltip = Constants.SERVER_PARAM_BIND_ADDRESS_TOOLTIP_STR
            };
            serverParameters.Controls.Add(bindAddress);
            ServerParameterNumeric bindPort = new()
            {
                ParameterName = "bindPort",
                ParameterFriendlyName = "Bind Port",
                ParameterIncrement = 1,
                ParameterMin = 1,
                ParameterMax = 65535,
                ParameterValue = 2001,
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
                ParameterMin = 1,
                ParameterMax = 65535,
                ParameterValue = 2001,
                ParameterTooltip = Constants.SERVER_PARAM_PUBLIC_PORT_TOOLTIP_STR
            };
            serverParameters.Controls.Add(publicPort);
            ServerParameterString a2sAddress = new()
            {
                ParameterName = "address",
                ParameterFriendlyName = "A2S Address",
                ParameterValue = "0.0.0.0",
                ParameterTooltip = Constants.SERVER_PARAM_A2S_ADDRESS_TOOLTIP_STR
            };
            serverParameters.Controls.Add(a2sAddress);
            ServerParameterNumeric a2sPort = new()
            {
                ParameterName = "port",
                ParameterFriendlyName = "A2S Port",
                ParameterIncrement = 1,
                ParameterMin = 1,
                ParameterMax = 65535,
                ParameterValue = 17777,
                ParameterTooltip = Constants.SERVER_PARAM_A2S_PORT_TOOLTIP_STR
            };
            serverParameters.Controls.Add(a2sPort);
            ServerParameterNumeric playerSaveTime = new()
            {
                ParameterName = "playerSaveTime",
                ParameterFriendlyName = "Player Save Time (secs)",
                ParameterIncrement = 1,
                ParameterMin = 1,
                ParameterMax = 65535,
                ParameterValue = 120,
                ParameterTooltip = Constants.SERVER_PARAM_PLAYER_SAVE_TIME_TOOLTIP_STR
            };
            serverParameters.Controls.Add(playerSaveTime);
            ServerParameterNumeric serverMaxViewDistance = new()
            {
                ParameterName = "serverMaxViewDistance",
                ParameterFriendlyName = "Server Max View Distance",
                ParameterIncrement = 1,
                ParameterMin = 500,
                ParameterMax = 10000,
                ParameterValue = 1600,
                ParameterTooltip = Constants.SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_TOOLTIP_STR
            };
            serverParameters.Controls.Add(serverMaxViewDistance);
            ServerParameterNumeric serverMinGrassDistance = new()
            {
                ParameterName = "serverMinGrassDistance",
                ParameterFriendlyName = "Server Min Grass Distance",
                ParameterIncrement = 1,
                ParameterMin = 0,
                ParameterMax = 150,
                ParameterValue = 50,
                ParameterTooltip = Constants.SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_TOOLTIP_STR
            };
            serverParameters.Controls.Add(serverMinGrassDistance);
            ServerParameterNumeric networkViewDistance = new()
            {
                ParameterName = "networkViewDistance",
                ParameterFriendlyName = "Network View Distance",
                ParameterIncrement = 1,
                ParameterMin = 500,
                ParameterMax = 5000,
                ParameterValue = 1500,
                ParameterTooltip = Constants.SERVER_PARAM_NETWORK_VIEW_DISTANCE_TOOLTIP_STR
            };
            serverParameters.Controls.Add(networkViewDistance);
            ServerParameterBool disableThirdPerson = new()
            {
                ParameterName = "disableThirdPerson",
                ParameterFriendlyName = "Disable Third Person",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_DISABLE_THIRD_PERSON_TOOLTIP_STR
            };
            serverParameters.Controls.Add(disableThirdPerson);
            ServerParameterBool fastValidation = new()
            {
                ParameterName = "fastValidation",
                ParameterFriendlyName = "Fast Validation",
                ParameterValue = true,
                ParameterTooltip = Constants.SERVER_PARAM_FAST_VALIDATION_TOOLTIP_STR
            };
            serverParameters.Controls.Add(fastValidation);
            ServerParameterBool battlEye = new()
            {
                ParameterName = "battlEye",
                ParameterFriendlyName = "BattlEye",
                ParameterValue = true,
                ParameterTooltip = Constants.SERVER_PARAM_BATTLEYE_TOOLTIP_STR
            };
            serverParameters.Controls.Add(battlEye);
            ServerParameterBool lobbyPlayerSynchronise = new()
            {
                ParameterName = "lobbyPlayerSynchronise",
                ParameterFriendlyName = "Lobby Player Synchronise",
                ParameterValue = true,
                ParameterTooltip = Constants.SERVER_PARAM_LOBBY_PLAYER_SYNC_TOOLTIP_STR
            };
            serverParameters.Controls.Add(lobbyPlayerSynchronise);
            ServerParameterBool vonDisableUI = new()
            {
                ParameterName = "VONDisableUI",
                ParameterFriendlyName = "VON Disable UI",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_VON_DISABLE_UI_TOOLTIP_STR
            };
            serverParameters.Controls.Add(vonDisableUI);
            ServerParameterBool vonDisableDirectSpeechUI = new()
            {
                ParameterName = "VONDisableDirectSpeechUI",
                ParameterFriendlyName = "VON Disable Direct Speech UI",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_TOOLTIP_STR
            };
            serverParameters.Controls.Add(vonDisableDirectSpeechUI);
            ServerParameterBool vonCanTransmitCrossFaction = new()
            {
                ParameterName = "VONCanTransmitCrossFaction",
                ParameterFriendlyName = "VON Can Transmit Cross Faction",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_VON_CAN_TRANSMIT_ACROSS_FACTION_TOOLTIP_STR
            };
            serverParameters.Controls.Add(vonCanTransmitCrossFaction);
            ServerParameterBool crossPlatform = new()
            {
                ParameterName = "crossPlatform",
                ParameterFriendlyName = "Cross Platform",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_CROSS_PLATFORM_TOOLTIP_STR
            };
            serverParameters.Controls.Add(crossPlatform);
            ServerParameterNumeric aiLimit = new()
            {
                ParameterName = "aiLimit",
                ParameterFriendlyName = "AI Limit",
                ParameterIncrement = 1,
                ParameterMin = -1,
                ParameterMax = 1000,
                ParameterValue = -1,
                ParameterTooltip = Constants.SERVER_PARAM_AI_LIMIT_TOOLTIP_STR
            };
            serverParameters.Controls.Add(aiLimit);
            ServerParameterNumeric slotReservationTimeout = new()
            {
                ParameterName = "slotReservationTimeout",
                ParameterFriendlyName = "Slot Reservation Timeout (secs)",
                ParameterIncrement = 1,
                ParameterMin = 5,
                ParameterMax = 300,
                ParameterValue = 60,
                ParameterTooltip = Constants.SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_TOOLTIP_STR
            };
            serverParameters.Controls.Add(slotReservationTimeout);
            ServerParameterBool disableNavmeshStreaming = new()
            {
                ParameterName = "disableNavmeshStreaming",
                ParameterFriendlyName = "Disable Navmesh Streaming",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_DISABLE_NAVMESH_STREAMING_TOOLTIP_STR
            };
            serverParameters.Controls.Add(disableNavmeshStreaming);
            ServerParameterBool disableServerShutdown = new()
            {
                ParameterName = "disableServerShutdown",
                ParameterFriendlyName = "Disable Server Shutdown",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_TOOLTIP_STR
            };
            serverParameters.Controls.Add(disableServerShutdown);
            ServerParameterBool disableCrashReporter = new()
            {
                ParameterName = "disableCrashReporter",
                ParameterFriendlyName = "Disable Crash Reporter",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_DISABLE_CRASH_REPORT_TOOLTIP_STR
            };
            serverParameters.Controls.Add(disableCrashReporter);
            ServerParameterBool disableAI = new()
            {
                ParameterName = "disableAI",
                ParameterFriendlyName = "Disable AI",
                ParameterValue = false,
                ParameterTooltip = Constants.SERVER_PARAM_DISABLE_AI_TOOLTIP_STR
            };
            serverParameters.Controls.Add(disableAI);

            foreach (ServerParameter param in serverParameters.Controls)
            {
                ConfigurationManager.GetInstance().GetServerParametersDictionary()[param.ParameterName] = param;
            }
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
                startServerBtn.Text = e.startServerText;
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
                loadedScenarioLabel.Text = e.scenarioId;
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
                // Config will be placed in <server-files-dir>/server.json
                config   = new("config", $"\"{FileIOManager.GetInstance().GetInstallDirectory()}\\server.json\""),
                // Saves etc. will be placed in <server-files-dir>/saves/
                profile  = new("profile", $"\"{FileIOManager.GetInstance().GetInstallDirectory()}\\saves\""),
                // Log performance stats every 5 seconds (represented in ms)
                logStats = new("logStats", $"{Convert.ToString(5000)}"),
                logLevel = new("logLevel", $"{logLevelComboBox.Text}")
            };

            if (limitFPS.Checked)
            {
                args.maxFPS = new("maxFPS", Convert.ToString(fpsLimitUpDown.Value));
            }

            if (forcePortCheckBox.Checked)
            {
                args.bindPort = new("bindPort", Convert.ToString(overridePortNumericUpDown.Value));
            }

            if (nds.Checked)
            {
                args.nds = new("nds", Convert.ToString(ndsUpDown.Value));
            }

            if (nwkResolution.Checked)
            {
                args.nwkResolution = new("nwkResolution", Convert.ToString(nwkResolutionUpDown.Value));
            }

            if (staggeringBudget.Checked)
            {
                args.staggeringBudget = new("staggeringBudget", Convert.ToString(staggeringBudgetUpDown.Value));
            }

            if (streamingBudget.Checked)
            {
                args.streamingBudget = new("streamingBudget", Convert.ToString(streamingBudgetUpDown.Value));
            }

            if (streamsDelta.Checked)
            {
                args.streamsDelta = new("streamsDelta", Convert.ToString(streamsDeltaUpDown.Value));
            }

            if (loadSessionSave.Checked)
            {
                args.loadSessionSave = new("loadSessionSave", sessionSave.Text);
            }

            ProcessManager.GetInstance().SetLaunchArgumentsModel(args);
        }
    }
}
