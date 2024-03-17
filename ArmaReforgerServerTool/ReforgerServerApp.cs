using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using static ReforgerServerApp.ServerConfiguration;

namespace ReforgerServerApp
{
    public partial class ReforgerServerApp : Form
    {
        private readonly string MOD_DATABASE_FILE = "./mod_database.txt";
        private readonly string INSTALL_DIR_FILE = "./install_directory.txt";
        private string steamCmdFile;
        private string installDirectory;
        private bool serverStarted;
        private bool serverStartedWithTimer;
        private Process steamCmdUpdateProcess;
        private Process serverProcess;
        private CancellationTokenSource timerCancellationTokenSource;
        private ServerConfiguration serverConfig;
        private Dictionary<string, ServerParameter> serverParamDict;

        public ReforgerServerApp()
        {
            InitializeComponent();

            serverParamDict = CreateServerParameterControls();

            serverRunningLabel.Text = string.Empty;

            serverConfig = new ServerConfiguration();

            // Create tooltips
            CreateTooltips();

            if (File.Exists(MOD_DATABASE_FILE))
            {
                ReadModsDatabase();
            }

            if (File.Exists(INSTALL_DIR_FILE))
            {
                using StreamReader sr = File.OpenText(INSTALL_DIR_FILE);
                installDirectory = sr.ReadToEnd();
                steamCmdFile = $"{installDirectory}\\steamcmd\\steamcmd.exe";
            }
            else
            {
                installDirectory = string.Empty;
                steamCmdFile = string.Empty;
            }

            loadedScenarioLabel.Text = "No scenario ID chosen.";

            UpdateSteamCmdInstallStatus();
            fpsLimitUpDown.Enabled = false;
            restartIntervalUpDown.Enabled = false;
            restartUnitsComboBox.Enabled = false;
            overridePortNumericUpDown.Enabled = false;
            ndsUpDown.Enabled = false;
            nwkResolutionUpDown.Enabled = false;
            staggeringBudgetUpDown.Enabled = false;
            streamingBudgetUpDown.Enabled = false;
            streamsDeltaUpDown.Enabled = false;
            serverStarted = false;
            serverStartedWithTimer = false;
            serverProcess = new();
            timerCancellationTokenSource = new();
            sessionSave.Enabled = false;
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());

            CheckForUpdates();

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
            if (File.Exists(steamCmdFile))
            {
                steamCmdAlert.Text = $"Using Arma Reforger Server files found at: {installDirectory}";
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
        /// Remove the selected mod from the Available Mods ListBox when the "Remove Mod" button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelectedModBtnPressed(object sender, EventArgs e)
        {
            GetAvailableModsList().Items.Remove(GetAvailableModsList().SelectedItem);
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
                GetAvailableModsList().Items.Remove(m);
                if (!GetEnabledModsList().Items.Contains(m))
                {
                    GetEnabledModsList().Items.Add(m);
                }
            }
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
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
            if ((Mod)GetEnabledModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetEnabledModsList().SelectedItem;
                GetEnabledModsList().Items.Remove(m);
                if (!GetAvailableModsList().Items.Contains(m))
                {
                    GetAvailableModsList().Items.Add(m);
                }
            }
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
        }

        /// <summary>
        /// Save the server settings to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSettingsToFileBtnPressed(object sender, EventArgs e)
        {
            using SaveFileDialog sfd = new();
            sfd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            sfd.Filter = "Properties files (*.prop)|*.prop";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string modsFilename = $"{sfd.FileName}_mods.txt";
                File.WriteAllText(sfd.FileName, CreateConfiguration().AsKeyValue(modsFilename));
                File.WriteAllText(modsFilename, CreateConfiguration().ModsAsCommaSeparatedString());
            }
        }

        /// <summary>
        /// Load the server settings from a txt file in comma separated format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSettingsFromFileBtnPressed(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new();
            ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            ofd.Filter = "Properties files (*.prop)|*.prop";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                using StreamReader sr = File.OpenText(filePath);
                PopulateServerConfiguration(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Write the available and enabled mods from the ListBoxes to the mod_database.txt file so that on next open, 
        /// they can be imported without the need for re-adding.
        /// The mods are stored in a comma separated format of "MOD_ID,MOD_NAME" on each line.
        /// </summary>
        public void WriteModsDatabase()
        {
            StringBuilder sb = new();
            foreach (Mod mod in availableMods.Items)
            {
                sb.AppendLine($"{mod.GetModID()},{mod.GetModName()}");
            }
            foreach (Mod mod in enabledMods.Items)
            {
                sb.AppendLine($"{mod.GetModID()},{mod.GetModName()}");
            }
            File.WriteAllText(MOD_DATABASE_FILE, sb.ToString().Trim());
        }

        /// <summary>
        /// Read the mods_database.txt file.
        /// This method also calls the AlphabetiseModListBox method so the ListBoxes are always 
        /// displaying the mods in alphabetical order.
        /// </summary>
        public void ReadModsDatabase()
        {
            using StreamReader sr = File.OpenText(MOD_DATABASE_FILE);
            string[] lines = sr.ReadToEnd().Trim().Split(Environment.NewLine);
            foreach (string line in lines)
            {
                if (line != null)
                {
                    string[] split = line.Split(",");
                    // Only attempt to add mods if the file isn't empty
                    if (split.Length > 1)
                    {
                        Mod m = new(split[0], split[1]);
                        if (!GetAvailableModsList().Items.Contains(m))
                        {
                            GetAvailableModsList().Items.Add(m);
                        }
                    }
                }
            }
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
        }

        /// <summary>
        /// Get Config Parameter for Server Configuration file (dictionary), if it's not present, use default value for the parameter
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="paramKey"></param>
        /// <returns>Value to set config parameter to (likely either an int, boolean or string)</returns>
        private object? GetConfigParameterOrDefault(Dictionary<string, string> dict, string paramKey)
        {
            try
            {
                return dict[paramKey];
            }
            catch (Exception)
            {
                foreach (ServerParameter sp in serverParameters.Controls)
                {
                    if (sp.ParameterName == paramKey)
                    {
                        return sp.ParameterValue;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// This method populates the GUI controls from an imported comma-separated settings file.
        /// It is important that the order of the settings file does not change as this method will break if indexes are wrong.
        /// This method also calls the alphabetise list box methods and will write the imported mods to the mod_database.txt file.`
        /// </summary>
        /// <param name="input"></param>
        private void PopulateServerConfiguration(string input)
        {
            string[] configLines = input.Trim().Split(Environment.NewLine);
            Dictionary<string, string> configParams = new();

            foreach (string line in configLines)
            {
                string[] splitLine = line.Split("=");
                configParams.Add(splitLine[0], splitLine[1]);
            }

            try
            {
                ServerConfigurationBuilder builder = new();
                builder
                    .WithBindAddress(Convert.ToString(GetConfigParameterOrDefault(configParams, "bindAddress")))
                    .WithBindPort(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "bindPort")))
                    .WithPublicAddress(Convert.ToString(GetConfigParameterOrDefault(configParams, "publicAddress")))
                    .WithPublicPort(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "publicPort")))
                    .WithAdminPassword(Convert.ToString(GetConfigParameterOrDefault(configParams, "passwordAdmin")))
                    .WithServerName(Convert.ToString(GetConfigParameterOrDefault(configParams, "name")))
                    .WithServerPassword(Convert.ToString(GetConfigParameterOrDefault(configParams, "password")))
                    .WithScenarioId(Convert.ToString(GetConfigParameterOrDefault(configParams, "scenarioId")))
                    .WithMaxPlayers(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "maxPlayers")))
                    .WithVisible(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "visible")))
                    .WithCrossPlatform(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "crossPlatform")))
                    .WithServerMaxViewDistance(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "serverMaxViewDistance")))
                    .WithServerMinGrassDistance(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "serverMinGrassDistance")))
                    .WithNetworkViewDistance(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "networkViewDistance")))
                    .WithDisableThirdPerson(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "disableThirdPerson")))
                    .WithFastValidation(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "fastValidation")))
                    .WithBattlEye(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "battlEye")))
                    .WithSteamQueryPort(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "steamQueryPort")))
                    .WithVONDisableUI(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "vonDisableUI")))
                    .WithVONDisableDirectSpeechUI(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "vonDisableDirectSpeechUI")))
                    .WithLobbyPlayerSynchronise(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "lobbyPlayerSynchronise")))
                    .WithPlayerSaveTime(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "playerSaveTime")))
                    .WithAILimit(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "aiLimit")))
                    .WithVONCanTransmitCrossFaction(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "vonCanTransmitCrossFaction")))
                    .WithSlotReservationTimeout(Convert.ToInt32(GetConfigParameterOrDefault(configParams, "slotReservationTimeout")))
                    .WithDisableNavmeshStreaming(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "disableNavmeshStreaming")))
                    .WithDisableServerShutdown(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "disableServerShutdown")))
                    .WithDisableCrashReporter(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "disableCrashReporter")))
                    .WithDisableAI(Convert.ToBoolean(GetConfigParameterOrDefault(configParams, "disableAI")))
                    .WithMissionHeader(Convert.ToString(GetConfigParameterOrDefault(configParams, "missionHeader")))
                    .WithAdmins(Convert.ToString(GetConfigParameterOrDefault(configParams, "admins")));

                try
                {
                    string[] modEntries = File.ReadAllLines(configParams["modCollection"]);
                    foreach (string mod in modEntries)
                    {
                        string[] modEntry = mod.Split(',');
                        builder.AddModToConfiguration(new(modEntry[1], modEntry[3]));
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show($"Unable to load file mod collection file '{configParams["modCollection"]}'.\r\n" +
                        $"It may be malformed, has been moved or does not exist.\r\n" +
                        $"Skipping loading mods...", Constants.ERROR_MESSAGEBOX_TITLE_STR);
                }

                serverConfig = builder.Build();

                serverParamDict["bindAddress"].ParameterValue = serverConfig.BindAddress;
                serverParamDict["bindPort"].ParameterValue = serverConfig.BindPort;
                serverParamDict["publicAddress"].ParameterValue = serverConfig.PublicAddress;
                serverParamDict["publicPort"].ParameterValue = serverConfig.PublicPort;
                serverParamDict["passwordAdmin"].ParameterValue = serverConfig.PasswordAdmin;
                serverParamDict["name"].ParameterValue = serverConfig.ServerName;
                serverParamDict["password"].ParameterValue = serverConfig.Password;
                loadedScenarioLabel.Text = serverConfig.ScenarioId;
                serverParamDict["maxPlayers"].ParameterValue = serverConfig.MaxPlayers;
                serverParamDict["visible"].ParameterValue = serverConfig.Visible;
                serverParamDict["crossPlatform"].ParameterValue = serverConfig.CrossPlatform;
                serverParamDict["serverMaxViewDistance"].ParameterValue = serverConfig.ServerMaxViewDistance;
                serverParamDict["serverMinGrassDistance"].ParameterValue = serverConfig.ServerMinGrassDistance;
                serverParamDict["networkViewDistance"].ParameterValue = serverConfig.NetworkViewDistance;
                serverParamDict["disableThirdPerson"].ParameterValue = serverConfig.DisableThirdPerson;
                serverParamDict["fastValidation"].ParameterValue = serverConfig.FastValidation;
                serverParamDict["battlEye"].ParameterValue = serverConfig.BattlEye;
                serverParamDict["steamQueryPort"].ParameterValue = serverConfig.SteamQueryPort;
                serverParamDict["lobbyPlayerSynchronise"].ParameterValue = serverConfig.LobbyPlayerSynchronise;
                serverParamDict["VONDisableUI"].ParameterValue = serverConfig.VONDisableUI;
                serverParamDict["VONDisableDirectSpeechUI"].ParameterValue = serverConfig.VONDisableDirectSpeechUI;
                serverParamDict["playerSaveTime"].ParameterValue = serverConfig.PlayerSaveTime;
                serverParamDict["aiLimit"].ParameterValue = serverConfig.AiLimit;
                serverParamDict["disableCrashReporter"].ParameterValue = serverConfig.DisableCrashReporter;
                serverParamDict["disableNavmeshStreaming"].ParameterValue = serverConfig.DisableNavmeshStreaming;
                serverParamDict["disableServerShutdown"].ParameterValue = serverConfig.DisableServerShutdown;
                serverParamDict["slotReservationTimeout"].ParameterValue = serverConfig.SlotReservationTimeout;
                serverParamDict["VONCanTransmitCrossFaction"].ParameterValue = serverConfig.VONCanTransmitCrossFaction;
                serverParamDict["disableAI"].ParameterValue = serverConfig.DisableAI;

                enabledMods.Items.Clear();

                foreach (Mod m in serverConfig.Mods)
                {
                    enabledMods.Items.Add(m);
                    if (availableMods.Items.Contains(m))
                    {
                        availableMods.Items.Remove(m);
                    }
                }
                AlphabetiseModListBox(GetAvailableModsList());
                AlphabetiseModListBox(GetEnabledModsList());
                WriteModsDatabase();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred while attempting to load the configuration file.\r\n" +
                    $"It may have been created for an earlier version.\r\n" +
                    $"The configuration has not been loaded.\r\n\r\n" +
                    $"Detail: {e.Message}\r\n\r\n" +
                    $"Include the detail above in your bug reports.",
                    Constants.ERROR_MESSAGEBOX_TITLE_STR);
            }
        }

        /// <summary>
        /// This method is used to create a server configuration object, 
        /// using the ServerConfigurationBuilder, from the GUI controls.
        /// </summary>
        /// <returns></returns>
        private ServerConfiguration CreateConfiguration()
        {
            ServerConfigurationBuilder builder = new();
            builder
                .WithBindAddress((string)serverParamDict["bindAddress"].ParameterValue)
                .WithBindPort(Convert.ToInt32(serverParamDict["bindPort"].ParameterValue))
                .WithPublicAddress((string)serverParamDict["publicAddress"].ParameterValue)
                .WithPublicPort(Convert.ToInt32(serverParamDict["publicPort"].ParameterValue))
                .WithAdminPassword((string)serverParamDict["passwordAdmin"].ParameterValue)
                .WithServerName((string)serverParamDict["name"].ParameterValue)
                .WithServerPassword((string)serverParamDict["password"].ParameterValue)
                .WithScenarioId(loadedScenarioLabel.Text)
                .WithMaxPlayers(Convert.ToInt32(serverParamDict["maxPlayers"].ParameterValue))
                .WithVisible((bool)serverParamDict["visible"].ParameterValue)
                .WithCrossPlatform((bool)serverParamDict["crossPlatform"].ParameterValue)
                .WithServerMaxViewDistance(Convert.ToInt32(serverParamDict["serverMaxViewDistance"].ParameterValue))
                .WithServerMinGrassDistance(Convert.ToInt32(serverParamDict["serverMinGrassDistance"].ParameterValue))
                .WithNetworkViewDistance(Convert.ToInt32(serverParamDict["networkViewDistance"].ParameterValue))
                .WithDisableThirdPerson((bool)serverParamDict["disableThirdPerson"].ParameterValue)
                .WithFastValidation((bool)serverParamDict["fastValidation"].ParameterValue)
                .WithBattlEye((bool)serverParamDict["battlEye"].ParameterValue)
                .WithSteamQueryPort(Convert.ToInt32(serverParamDict["steamQueryPort"].ParameterValue))
                .WithLobbyPlayerSynchronise((bool)serverParamDict["lobbyPlayerSynchronise"].ParameterValue)
                .WithVONDisableUI((bool)serverParamDict["VONDisableUI"].ParameterValue)
                .WithVONDisableDirectSpeechUI((bool)serverParamDict["VONDisableDirectSpeechUI"].ParameterValue)
                .WithPlayerSaveTime(Convert.ToInt32(serverParamDict["playerSaveTime"].ParameterValue))
                .WithAILimit(Convert.ToInt32(serverParamDict["aiLimit"].ParameterValue))
                .WithVONCanTransmitCrossFaction((bool)serverParamDict["VONCanTransmitCrossFaction"].ParameterValue)
                .WithSlotReservationTimeout(Convert.ToInt32(serverParamDict["slotReservationTimeout"].ParameterValue))
                .WithDisableNavmeshStreaming((bool)serverParamDict["disableNavmeshStreaming"].ParameterValue)
                .WithDisableServerShutdown((bool)serverParamDict["disableServerShutdown"].ParameterValue)
                .WithDisableCrashReporter((bool)serverParamDict["disableCrashReporter"].ParameterValue)
                .WithMissionHeader(serverConfig.MissionHeader)
                .WithAdmins(serverConfig.Admins)
                .WithDisableAI(serverConfig.DisableAI);

            foreach (Mod m in enabledMods.Items)
            {
                builder.AddModToConfiguration(m);
            }
            return builder.Build();
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
            string path = string.Empty;
            using FolderBrowserDialog fbd = new();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                installDirectory = fbd.SelectedPath;
                steamCmdFile = $"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe";
                File.WriteAllText(INSTALL_DIR_FILE, installDirectory);
            }

            using WebClient client = new();
            client.DownloadFileCompleted += (s, e) =>
            {
                if (File.Exists($"{installDirectory}\\steamcmd.zip"))
                {
                    ZipFile.ExtractToDirectory($"{installDirectory}\\steamcmd.zip", $"{installDirectory}\\steamcmd");
                }
                UpdateSteamCmdInstallStatus();
            };
            client.DownloadFileAsync(new Uri("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip"), $"{installDirectory}\\steamcmd.zip");
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
            const int INTERVAL_MINS = 0;
            const int INTERVAL_HRS = 1;
            const int INTERVAL_DAYS = 2;

            // If we are starting the server for the first time and using the automatic restart functionality, configure the timer
            if (automaticallyRestart.Checked && !serverStartedWithTimer)
            {
                // Create a timespan based on which units the user has selected
                // Use default value of 1 hour restarts so VS stops yelling at us
                TimeSpan interval = TimeSpan.FromHours(1);
                switch (restartUnitsComboBox.SelectedIndex)
                {
                    case INTERVAL_MINS:
                        interval = TimeSpan.FromMinutes((int)restartIntervalUpDown.Value);
                        break;
                    case INTERVAL_HRS:
                        interval = TimeSpan.FromHours((int)restartIntervalUpDown.Value);
                        break;
                    case INTERVAL_DAYS:
                        interval = TimeSpan.FromDays((int)restartIntervalUpDown.Value);
                        break;
                }

                Task automaticRestartTask = PeriodicAsync(DoStartStopServerLogicTimer, interval, timerCancellationTokenSource.Token);
                serverStartedWithTimer = true;
            }
            // The user is turning the server off manually
            else if (automaticallyRestart.Checked && serverStartedWithTimer)
            {
                timerCancellationTokenSource.Cancel();
                // Call this method to shut the server down finally
                DoStartStopServerLogic();
                serverStartedWithTimer = false;
            }
            // User just normally pressed the button
            else if (!automaticallyRestart.Checked && !serverStartedWithTimer)
            {
                DoStartStopServerLogic();
            }
        }

        /// <summary>
        /// This method controls the logic for Starting and Stopping the Server.
        /// When Starting the server, this will spawn the Worker Thread that runs the SteamCMD and server processes.
        /// When Stopping the server, this will kill the server process and remove the Output / Error redirects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoStartStopServerLogic()
        {
            BackgroundWorker worker = new();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SteamCmdUpdateWorkerDoWork;

            if (serverStarted)
            {
                if (File.Exists(installDirectory + Constants.SERVER_JSON_STR))
                {
                    File.Delete(installDirectory + Constants.SERVER_JSON_STR);
                }
                worker.CancelAsync();
                try
                {
                    serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                    serverProcess.CancelOutputRead();
                    serverProcess.CancelErrorRead();
                    steamCmdLog.AppendText($"{GetTimestamp()}: User stopped server.{Environment.NewLine}");
                    serverProcess.Kill();

                    serverStarted = false;
                    startServerBtn.Text = Constants.START_SERVER_STR;
                    EnableServerFields(true);
                    serverRunningLabel.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", Constants.ERROR_MESSAGEBOX_TITLE_STR);
                }
            }
            else
            {
                string jsonConfig = CreateConfiguration().AsJsonString();
                File.WriteAllText(installDirectory + Constants.SERVER_JSON_STR, jsonConfig);
                serverStarted = true;
                startServerBtn.Text = Constants.STOP_SERVER_STR;
                startServerBtn.Enabled = false;
                EnableServerFields(false);
                serverRunningLabel.Text = Constants.SERVER_CURRENTLY_RUNNING_STR;
                steamCmdLog.AppendText($"{GetTimestamp()}: User started server.{Environment.NewLine}");
                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// This is almost identical to the DoStartStopServerLogic method with the difference being it
        /// automatically restarts the server after stopping it as it's not a toggle.
        /// </summary>
        private void DoStartStopServerLogicTimer()
        {
            BackgroundWorker worker = new();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SteamCmdUpdateWorkerDoWork;

            if (serverStarted)
            {
                if (File.Exists($"{installDirectory}{Constants.SERVER_JSON_STR}"))
                {
                    File.Delete($"{installDirectory}{Constants.SERVER_JSON_STR}");
                }
                serverStarted = false;
                startServerBtn.Text = Constants.START_SERVER_STR;
                worker.CancelAsync();
                EnableServerFields(true);
                serverRunningLabel.Text = string.Empty;
                try
                {
                    serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                    serverProcess.CancelOutputRead();
                    serverProcess.CancelErrorRead();
                    steamCmdLog.AppendText($"{GetTimestamp()}: Automatically stopped server.{Environment.NewLine}");
                    serverProcess.Kill();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", Constants.ERROR_MESSAGEBOX_TITLE_STR);
                }
            }
            string jsonConfig = CreateConfiguration().AsJsonString();
            File.WriteAllText($"{installDirectory}{Constants.SERVER_JSON_STR}", jsonConfig);
            serverStarted = true;
            startServerBtn.Text = Constants.STOP_SERVER_STR;
            startServerBtn.Enabled = false;
            EnableServerFields(false);
            serverRunningLabel.Text = Constants.SERVER_CURRENTLY_RUNNING_STR;
            steamCmdLog.AppendText($"{GetTimestamp()}: Automatically started server.{Environment.NewLine}");
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Handler for when data is received from the Std Output or Error from SteamCMD or the Arma Server processes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SteamCmdDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                lock (steamCmdLog)
                {
                    steamCmdLog.Invoke((MethodInvoker)(() => steamCmdLog.AppendText($"{GetTimestamp()}: {e.Data}{Environment.NewLine}")));
                }
                // Kill the server if it fails to start correctly.
                if (e.Data.Contains("Unable to Initialize"))
                {
                    steamCmdLog.Invoke((MethodInvoker)(() => steamCmdLog.AppendText($"{GetTimestamp()}: System stopped server due to an error.{Environment.NewLine}")));
                    serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                    serverProcess.CancelOutputRead();
                    serverProcess.CancelErrorRead();
                    serverProcess.Kill();

                    serverStarted = false;
                    startServerBtn.Invoke((MethodInvoker)(() => startServerBtn.Text = Constants.START_SERVER_STR));
                    startServerBtn.Text = Constants.START_SERVER_STR;
                    // Invoke this method using a UI element, doesn't matter what it is
                    // so just use the startServerBtn
                    startServerBtn.Invoke((MethodInvoker)(() => EnableServerFields(true)));
                    serverRunningLabel.Invoke((MethodInvoker)(() => serverRunningLabel.Text = string.Empty));
                }
            }
        }

        /// <summary>
        /// Worker Thread task, this task spawns SteamCMD which will install / update the Arma Server files and then close, 
        /// once it has closed, the Arma Server is launched with the generated server configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SteamCmdUpdateWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            steamCmdUpdateProcess = new();
            ProcessStartInfo startInfo = new();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = steamCmdFile;
            startInfo.Arguments = "+force_install_dir ..\\Arma_Reforger +login anonymous anonymous +app_update 1874900 +quit";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            steamCmdUpdateProcess.EnableRaisingEvents = true;
            startInfo.CreateNoWindow = true;
            steamCmdUpdateProcess.StartInfo = startInfo;
            steamCmdUpdateProcess.OutputDataReceived += SteamCmdDataReceived;
            steamCmdUpdateProcess.ErrorDataReceived += SteamCmdDataReceived;
            steamCmdUpdateProcess.Start();
            steamCmdUpdateProcess.BeginOutputReadLine();
            steamCmdUpdateProcess.BeginErrorReadLine();
            steamCmdUpdateProcess.WaitForExit();

            if (steamCmdUpdateProcess.HasExited)
            {
                startServerBtn.Invoke((MethodInvoker)(() => startServerBtn.Enabled = true));
                serverProcess = new();
                ProcessStartInfo serverStartInfo = new();
                serverStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                serverStartInfo.UseShellExecute = false;
                serverStartInfo.WorkingDirectory = $"{installDirectory}\\arma_reforger";
                serverStartInfo.FileName = $"{installDirectory}\\arma_reforger\\ArmaReforgerServer.exe";
                serverStartInfo.Arguments = CreateLaunchArguments();
                serverStartInfo.RedirectStandardOutput = true;
                serverStartInfo.RedirectStandardError = true;
                serverStartInfo.CreateNoWindow = true;
                serverProcess.EnableRaisingEvents = true;
                serverProcess.StartInfo = serverStartInfo;
                serverProcess.OutputDataReceived += SteamCmdDataReceived;
                serverProcess.ErrorDataReceived += SteamCmdDataReceived;
                serverProcess.Start();
                serverProcess.BeginOutputReadLine();
                serverProcess.BeginErrorReadLine();
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
            List<Mod> availableMods = GetAvailableModsList().Items.OfType<Mod>().ToList();
            foreach (Mod m in availableMods)
            {
                if (!GetEnabledModsList().Items.Contains(m))
                {
                    GetEnabledModsList().Items.Add(m);
                    GetAvailableModsList().Items.Remove(m);
                }
            }
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
        }

        /// <summary>
        /// Handler for the Disable All Mods Button (displayed as '<<' on the GUI).
        /// Adds all mods from the Enabled Mods list to the Available Mods list and then alphabetises the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisableAllModsBtnPressed(object sender, EventArgs e)
        {
            List<Mod> enabledMods = GetEnabledModsList().Items.OfType<Mod>().ToList();
            foreach (Mod m in enabledMods)
            {
                if (!GetAvailableModsList().Items.Contains(m))
                {
                    GetAvailableModsList().Items.Add(m);
                    GetEnabledModsList().Items.Remove(m);
                }
            }
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
        }

        /// <summary>
        /// Convenience method to sort the Mod ListBoxes in order of Mod Name
        /// </summary>
        /// <param name="listBox"></param>
        public static void AlphabetiseModListBox(ListBox listBox)
        {
            List<Mod> list = listBox.Items.OfType<Mod>().ToList();
            list.Sort((x, y) => string.Compare(x.GetModName(), y.GetModName()));
            listBox.Items.Clear();

            foreach (Mod m in list)
            {
                listBox.Items.Add(m);
            }
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
            DialogResult result = MessageBox.Show("You are about to delete SteamCMD and all Arma Reforger server files," +
                " are you sure you would like to do this?", "Warning", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Directory.Delete(installDirectory, true);
                installDirectory = string.Empty;
                File.Delete(INSTALL_DIR_FILE);
                UpdateSteamCmdInstallStatus();
                MessageBox.Show("Server files deleted.", "Warning", MessageBoxButtons.OK);
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
            string path = string.Empty;
            using FolderBrowserDialog fbd = new();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                if (File.Exists($"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe") &&
                    File.Exists($"{fbd.SelectedPath}\\arma_reforger\\ArmaReforgerServer.exe"))
                {
                    installDirectory = fbd.SelectedPath;
                    steamCmdFile = $"{fbd.SelectedPath}\\steamcmd\\steamcmd.exe";
                    File.WriteAllText(INSTALL_DIR_FILE, installDirectory);
                    UpdateSteamCmdInstallStatus();
                }
                else
                {
                    MessageBox.Show("Arma Reforger Server Files could not be located." +
                        "\r\nPlease confirm the chosen path or download the files to start.", "Warning", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Enable / Disable Server Configuration Fields
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableServerFields(bool enabled)
        {
            foreach (KeyValuePair<string, ServerParameter> param in serverParamDict)
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
        /// Method for starting asynchronous periodic timer based functionality
        /// </summary>
        /// <param name="action">Method to run</param>
        /// <param name="interval">Interval at which to run</param>
        /// <param name="cancellationToken">Cancellation token to cancel with</param>
        /// <returns></returns>
        public static async Task PeriodicAsync(Action action, TimeSpan interval, CancellationToken cancellationToken = default)
        {
            using PeriodicTimer timer = new(interval);
            while (true)
            {
                action();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Return string representation of DateTime.Now
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
        /// Create a string with all requried launch arguments
        /// </summary>
        /// <returns>String containing launch arguments for the Reforger Server</returns>
        private string CreateLaunchArguments()
        {
            List<string> argsList = new List<string>();

            // Config will be placed in <server-files-dir>/server.json
            string configArg = $"-config \"{installDirectory}\\server.json\"";
            argsList.Add(configArg);

            // Saves etc. will be placed in <server-files-dir>/saves/
            string profileArg = $"-profile \"{installDirectory}\\saves\"";
            argsList.Add(profileArg);

            // Log performance stats every 5 seconds (represented in ms)
            string logStatsArg = "-logStats 5000";
            argsList.Add(logStatsArg);

            string maxFPSArg = string.Empty;
            if (limitFPS.Checked)
            {
                maxFPSArg = $"-maxFPS {Convert.ToString(fpsLimitUpDown.Value)}";
                argsList.Add(maxFPSArg);
            }

            string overridePortArg = string.Empty;
            if (forcePortCheckBox.Checked)
            {
                overridePortArg = $"-bindPort {Convert.ToString(overridePortNumericUpDown.Value)}";
                argsList.Add(overridePortArg);
            }

            string ndsArg = string.Empty;
            if (nds.Checked)
            {
                ndsArg = $"-nds {Convert.ToString(ndsUpDown.Value)}";
                argsList.Add(ndsArg);
            }

            string nwkResolutionArg = string.Empty;
            if (nwkResolution.Checked)
            {
                nwkResolutionArg = $"-nwkResolution {Convert.ToString(nwkResolutionUpDown.Value)}";
                argsList.Add(nwkResolutionArg);
            }

            string staggeringBudgetArg = string.Empty;
            if (staggeringBudget.Checked)
            {
                staggeringBudgetArg = $"-staggeringBudget {Convert.ToString(staggeringBudgetUpDown.Value)}";
                argsList.Add(staggeringBudgetArg);
            }

            string streamingBudgetArg = string.Empty;
            if (streamingBudget.Checked)
            {
                streamingBudgetArg = $"-streamingBudget {Convert.ToString(streamingBudgetUpDown.Value)}";
                argsList.Add(streamingBudgetArg);
            }

            string streamsDeltaArg = string.Empty;
            if (streamsDelta.Checked)
            {
                streamsDeltaArg = $"-streamsDelta {Convert.ToString(streamsDeltaUpDown.Value)}";
                argsList.Add(streamsDeltaArg);
            }

            string loadSessionSaveArg = string.Empty;
            if (loadSessionSave.Checked)
            {
                loadSessionSaveArg = $"-loadSessionSave {(sessionSave.Text.Length > 0 ? sessionSave.Text.Trim() : string.Empty)}";
                argsList.Add(loadSessionSaveArg);
            }

            string logLevelArg = string.Empty;
            // Use method invoker to set the Log Level to avoid cross-threaded operation
            logLevelComboBox.Invoke((MethodInvoker)(() => logLevelArg = $"-logLevel {logLevelComboBox.Text}"));
            argsList.Add(logLevelArg);

            return string.Join(" ", argsList);
        }

        /// <summary>
        /// Check our version against the version.txt file in the GitHub repository.
        /// Show a dialog prompting the user to update if we are out of date.
        /// If there is no internet connection, or this simply fails, 
        /// warn the user that we couldn't successfully check for updates.
        /// </summary>
        private void CheckForUpdates()
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
            loadedScenarioLabel.Text = serverConfig.ScenarioId;
        }

        /// <summary>
        /// Logic for starting the Scenario Select form
        /// </summary>
        private void SpawnScenarioSelect()
        {
            ScenarioSelector scenarioSelector = new(this, serverConfig);
            scenarioSelector.ShowDialog();
        }

        private void EditMissionHeaderBtnClicked(object sender, EventArgs e)
        {
            TextInputForm tif = new(serverConfig, "Edit Mission Header");
            tif.ShowDialog();
        }

        private void EditAdminsListBtnClicked(object sender, EventArgs e)
        {
            ListForm lf = new(serverConfig, "Edit Admins");
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
        /// Create a dictionary of Server Parameter UI controls to easily retrieve values and send them to the model
        /// </summary>
        private Dictionary<string, ServerParameter> CreateServerParameterControls()
        {
            Dictionary<string, ServerParameter> serverParameterDictionary = new();
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
                ParameterValue = "0.0.0.0",
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
            ServerParameterNumeric steamQueryPort = new()
            {
                ParameterName = "steamQueryPort",
                ParameterFriendlyName = "Steam Query Port",
                ParameterIncrement = 1,
                ParameterMin = 1,
                ParameterMax = 65535,
                ParameterValue = 17777,
                ParameterTooltip = Constants.SERVER_PARAM_STEAM_QUERY_PORT_TOOLTIP_STR
            };
            serverParameters.Controls.Add(steamQueryPort);
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
                serverParameterDictionary[param.ParameterName] = param;
            }
            return serverParameterDictionary;
        }
    }
}