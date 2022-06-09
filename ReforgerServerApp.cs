using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        private Process steamCmdUpdateProcess;
        private Process serverProcess;
        public ReforgerServerApp()
        {
            InitializeComponent();

            serverRunningLabel.Text = string.Empty;

            // Create tooltips
            ToolTip enableAllModsToolTip = new();
            enableAllModsToolTip.SetToolTip(enableAllModsBtn, "Enable All Mods");
            ToolTip disableAllModsToolTip = new();
            disableAllModsToolTip.SetToolTip(disableAllModsBtn, "Disable All Mods");
            ToolTip enableModToolTip = new();
            enableModToolTip.SetToolTip(addToEnabledBtn, "Enable Mod");
            ToolTip disableModToolTip = new();
            disableModToolTip.SetToolTip(removeFromEnabledBtn, "Disable Mod");
            ToolTip scenarioIdLabelToolTip = new();
            scenarioIdLabelToolTip.SetToolTip(scenarioIdLabel, "Enter the Scenario ID found in the Scenario's serverData.json file, or select one of the default ones");
            ToolTip regionLabelToolTip = new();
            regionLabelToolTip.SetToolTip(regionLabel, "Enter an ISO 3166-1 alpha-2 region code, or select one of the default ones");

            // Initialise region combo box to select the first option
            region.SelectedIndex = 0;

            if (File.Exists(MOD_DATABASE_FILE))
            {
                ReadModsDatabase();
            }

            if (File.Exists(INSTALL_DIR_FILE))
            {
                using StreamReader sr = File.OpenText(INSTALL_DIR_FILE);
                installDirectory = sr.ReadToEnd();
                steamCmdFile = installDirectory + "\\steamcmd\\steamcmd.exe";
            }
            else
            {
                installDirectory = string.Empty;
                steamCmdFile = string.Empty;
            }

            UpdateSteamCmdInstallStatus();
            fpsLimitUpDown.Enabled = false;
            serverStarted = false;
            serverProcess = new();
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
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
                steamCmdAlert.Text = "Using Arma Reforger Server files found at: " + installDirectory;
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
        /// Save the server settings to a txt file in comma separated format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSettingsToFileBtnPressed(object sender, EventArgs e)
        {
            using SaveFileDialog sfd = new();
            sfd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            sfd.Filter = "Text files (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, CreateConfiguration().AsCommaSeparatedString());
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
            ofd.Filter = "Text files (*.txt)|*.txt";
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
                sb.AppendLine(mod.GetModID() + "," + mod.GetModName());
            }
            foreach (Mod mod in enabledMods.Items)
            {
                sb.AppendLine(mod.GetModID() + "," + mod.GetModName());
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
                    Mod m = new(split[0], split[1]);
                    if (!GetAvailableModsList().Items.Contains(m))
                    {
                        GetAvailableModsList().Items.Add(m);
                    }
                }
            }
            AlphabetiseModListBox(GetAvailableModsList());
            AlphabetiseModListBox(GetEnabledModsList());
        }

        /// <summary>
        /// This method populates the GUI controls from an imported comma-separated settings file.
        /// It is important that the order of the settings file does not change as this method will break if indexes are wrong.
        /// This method also calls the alphabetise list box methods and will write the imported mods to the mod_database.txt file.`
        /// </summary>
        /// <param name="input"></param>
        private void PopulateServerConfiguration(string input)
        {
            const int MINIMUM_CONFIG_FILE_LENGTH = 43;
            string[] configLines = input.Trim().Split(Environment.NewLine);
            List<string> configParams = new();

            foreach (string line in configLines)
            {
                string[] splitLine = line.Split(",");
                foreach (string s in splitLine)
                {
                    configParams.Add(s);
                }
            }

            if (configParams.Count >= MINIMUM_CONFIG_FILE_LENGTH)
            {
                ServerConfigurationBuilder builder = new();
                builder
                    .WithDedicatedServerId(configParams[1])
                    .WithRegion(configParams[3])
                    .WithGameHostBindAddress(configParams[5])
                    .WithGameHostBindPort(Convert.ToInt32(configParams[7]))
                    .WithGameHostRegisterBindAddress(configParams[9])
                    .WithGameHostRegisterBindPort(Convert.ToInt32(configParams[11]))
                    .WithAdminPassword(configParams[13])
                    .WithServerName(configParams[15])
                    .WithServerPassword(configParams[17])
                    .WithScenarioId(configParams[19])
                    .WithPlayerCountLimit(Convert.ToInt32(configParams[21]))
                    .WithAutoJoinable(Convert.ToBoolean(configParams[23]))
                    .WithVisible(Convert.ToBoolean(configParams[25]))
                    .WithServerMaxViewDistance(Convert.ToInt32(configParams[27]))
                    .WithServerMinGrassDistance(Convert.ToInt32(configParams[29]))
                    .WithNetworkViewDistance(Convert.ToInt32(configParams[31]))
                    .WithGameNumber(Convert.ToInt32(configParams[33]))
                    .WithDisableThirdPerson(Convert.ToBoolean(configParams[35]))
                    .WithFastValidation(Convert.ToBoolean(configParams[37]))
                    .WithBattlEye(Convert.ToBoolean(configParams[39]))
                    .WithA2SQueryEnabled(Convert.ToBoolean(configParams[41]))
                    .WithSteamQueryPort(Convert.ToInt32(configParams[43]));

                for (int i = 0; i < configParams.Count; ++i)
                {
                    if (configParams[i].Equals("modId"))
                    {
                        builder.AddModToConfiguration(new(configParams[i + 1], configParams[i + 3]));
                    }
                }

                ServerConfiguration sc = builder.Build();

                dedicatedServerId.Text = sc.DedicatedServerId;
                region.Text = sc.Region;
                gameHostBindAddress.Text = sc.GameHostBindAddress;
                gameHostBindPort.Value = sc.GameHostBindPort;
                gameHostRegisterBindAddress.Text = sc.GameHostRegisterBindAddress;
                gameHostRegisterBindPort.Value = sc.GameHostRegisterBindPort;
                adminPassword.Text = sc.AdminPassword;
                serverName.Text = sc.ServerName;
                serverPassword.Text = sc.ServerPassword;
                scenarioId.Text = sc.ScenarioId;
                playerCountLimit.Value = sc.PlayerCountLimit;
                autoJoinable.Checked = sc.AutoJoinable;
                visible.Checked = sc.Visible;
                serverMaxViewDistance.Value = sc.ServerMaxViewDistance;
                serverMinGrassDistance.Value = sc.ServerMinGrassDistance;
                networkViewDistance.Value = sc.NetworkViewDistance;
                gameNumber.Value = sc.GameNumber;
                disableThirdPerson.Checked = sc.DisableThirdPerson;
                fastValidation.Checked = sc.FastValidation;
                battlEye.Checked = sc.BattlEye;
                a2sQueryEnabled.Checked = sc.A2sQueryEnabled;
                steamQueryPort.Value = sc.SteamQueryPort;

                enabledMods.Items.Clear();

                foreach (Mod m in sc.Mods)
                {
                    enabledMods.Items.Add(m);
                    if (!availableMods.Items.Contains(m))
                    {
                        availableMods.Items.Add(m);
                    }
                }
                AlphabetiseModListBox(GetAvailableModsList());
                AlphabetiseModListBox(GetEnabledModsList());
                WriteModsDatabase();
            }
            else
            {
                MessageBox.Show("Server Config file is malformed and cannot be opened.");
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
                .WithDedicatedServerId(dedicatedServerId.Text)
                .WithRegion(region.GetItemText(region.SelectedItem))
                .WithGameHostBindAddress(gameHostBindAddress.Text)
                .WithGameHostBindPort((int)gameHostBindPort.Value)
                .WithGameHostRegisterBindAddress(gameHostRegisterBindAddress.Text)
                .WithGameHostRegisterBindPort((int)gameHostRegisterBindPort.Value)
                .WithAdminPassword(adminPassword.Text)
                .WithServerName(serverName.Text)
                .WithServerPassword(serverPassword.Text)
                .WithScenarioId(scenarioId.Text)
                .WithGameNumber((int)gameNumber.Value)
                .WithPlayerCountLimit((int)playerCountLimit.Value)
                .WithAutoJoinable(autoJoinable.Checked)
                .WithVisible(visible.Checked)
                .WithServerMaxViewDistance((int)serverMaxViewDistance.Value)
                .WithServerMinGrassDistance((int)serverMinGrassDistance.Value)
                .WithNetworkViewDistance((int)networkViewDistance.Value)
                .WithDisableThirdPerson(disableThirdPerson.Checked)
                .WithFastValidation(fastValidation.Checked)
                .WithBattlEye(battlEye.Checked)
                .WithA2SQueryEnabled(a2sQueryEnabled.Checked)
                .WithSteamQueryPort((int)steamQueryPort.Value);

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
                steamCmdFile = fbd.SelectedPath + "\\steamcmd\\steamcmd.exe";
                File.WriteAllText(INSTALL_DIR_FILE, installDirectory);
            }

            using WebClient client = new();
            client.DownloadFileCompleted += (s, e) =>
            {
                if (File.Exists(installDirectory + "\\steamcmd.zip"))
                {
                    ZipFile.ExtractToDirectory(installDirectory + "\\steamcmd.zip", installDirectory + "\\steamcmd");
                }
                UpdateSteamCmdInstallStatus();
            };
            client.DownloadFileAsync(new Uri("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip"), installDirectory + "\\steamcmd.zip");
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
        /// This method controls the logic for Starting and Stopping the Server.
        /// When Starting the server, this will spawn the Worker Thread that runs the SteamCMD and server processes.
        /// When Stopping the server, this will kill the server process and remove the Output / Error redirects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartServerBtnPressed(object sender, EventArgs e)
        {
            BackgroundWorker worker = new();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SteamCmdUpdateWorkerDoWork;

            if (serverStarted)
            {
                if (File.Exists(installDirectory + "\\server.json"))
                {
                    File.Delete(installDirectory + "\\server.json");
                }
                serverStarted = false;
                startServerBtn.Text = "Start Server";
                deleteServerFilesBtn.Enabled = true;
                worker.CancelAsync();
                EnableServerFields(true);
                serverRunningLabel.Text = string.Empty;
                try
                {
                    serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                    serverProcess.CancelOutputRead();
                    serverProcess.CancelErrorRead();
                    serverProcess.Kill();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                string jsonConfig = CreateConfiguration().AsJsonString();
                File.WriteAllText(installDirectory + "\\server.json", jsonConfig);
                serverStarted = true;
                startServerBtn.Text = "Stop Server";
                deleteServerFilesBtn.Enabled = false;
                EnableServerFields(false);
                serverRunningLabel.Text = "Server is currently running. To modify the configuration, you will need to stop it first.";
                worker.RunWorkerAsync();
            }
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
                steamCmdLog.AppendText(e.Data + Environment.NewLine);
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
            steamCmdUpdateProcess.SynchronizingObject = steamCmdLog;
            steamCmdUpdateProcess.Start();
            steamCmdUpdateProcess.BeginOutputReadLine();
            steamCmdUpdateProcess.BeginErrorReadLine();
            steamCmdUpdateProcess.WaitForExit();

            if (steamCmdUpdateProcess.HasExited)
            {
                serverProcess = new();
                ProcessStartInfo serverStartInfo = new();
                serverStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                serverStartInfo.UseShellExecute = false;
                serverStartInfo.WorkingDirectory = installDirectory + "\\arma_reforger";
                serverStartInfo.FileName = installDirectory + "\\arma_reforger\\ArmaReforgerServer.exe";
                string limitFPSArg = string.Empty;
                if (limitFPS.Checked)
                {
                    limitFPSArg = "-maxFPS " + Convert.ToString(fpsLimitUpDown.Value);
                }
                string args = "-config \"" + installDirectory + ".\\server.json\" -profile \""
                     + installDirectory + "\\saves\" -logStats 5000 " + limitFPSArg;
                serverStartInfo.Arguments = args;
                serverStartInfo.RedirectStandardOutput = true;
                serverStartInfo.RedirectStandardError = true;
                serverProcess.EnableRaisingEvents = true;
                serverStartInfo.CreateNoWindow = true;
                serverProcess.StartInfo = serverStartInfo;
                serverProcess.SynchronizingObject = steamCmdLog;
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
            StringBuilder sb = new();
            sb.AppendLine("Arma Reforger Dedicated Server Tool by soda3x");
            sb.Append("Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            MessageBox.Show(sb.ToString(), "About");
        }

        /// <summary>
        /// Handler for the "Delete Server Files" button.
        /// Deletes all server files and references to them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteServerFilesBtnPressed(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("You are about to delete SteamCMD and all Arma Reforger server files, are you sure you would like to do this?", "Warning", MessageBoxButtons.YesNo);

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
        /// Enable / Disable Server Configuration Fields
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableServerFields(bool enabled)
        {
            dedicatedServerId.Enabled = enabled;
            region.Enabled = enabled;
            gameHostBindAddress.Enabled = enabled;
            gameHostBindPort.Enabled = enabled;
            gameHostRegisterBindAddress.Enabled = enabled;
            gameHostRegisterBindPort.Enabled = enabled;
            adminPassword.Enabled = enabled;
            serverName.Enabled = enabled;
            serverPassword.Enabled = enabled;
            scenarioId.Enabled = enabled;
            playerCountLimit.Enabled = enabled;
            autoJoinable.Enabled = enabled;
            visible.Enabled = enabled;
            serverMaxViewDistance.Enabled = enabled;
            serverMinGrassDistance.Enabled = enabled;
            networkViewDistance.Enabled = enabled;
            gameNumber.Enabled = enabled;
            disableThirdPerson.Enabled = enabled;
            fastValidation.Enabled = enabled;
            battlEye.Enabled = enabled;
            a2sQueryEnabled.Enabled = enabled;
            steamQueryPort.Enabled = enabled;
            enableAllModsBtn.Enabled = enabled;
            addToEnabledBtn.Enabled = enabled;
            disableAllModsBtn.Enabled = enabled;
            removeFromEnabledBtn.Enabled = enabled;
            loadSettingsBtn.Enabled = enabled;
            saveSettingsBtn.Enabled = enabled;
            addModBtn.Enabled = enabled;
            removeModBtn.Enabled = enabled;
        }
    }
}