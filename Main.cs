using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using static ReforgerServerApp.ServerConfiguration;

namespace ReforgerServerApp
{
    public partial class Main : Form
    {
        private readonly string MOD_DATABASE_FILE = "./mod_database.txt";
        private readonly string STEAM_CMD_FILE = "./steamcmd/steamcmd.exe";
        private readonly string REFORGER_SERVER_EXE = "./arma_reforger/ArmaReforgerServer.exe";
        private bool serverStarted = false;
        private List<string> scenarioIds;
        public Main()
        {
            InitializeComponent();
            region.SelectedIndex = 0;
            if (File.Exists(MOD_DATABASE_FILE))
            {
                ReadModsDatabase();
            }
            //if (File.Exists(REFORGER_SERVER_EXE))
            //{
            //    PopulateScenarioIdField();
            //}
            UpdateSteamCmdInstallStatus();
            fpsLimitUpDown.Enabled = false;

        }

        private void UpdateSteamCmdInstallStatus()
        {
            if (File.Exists(STEAM_CMD_FILE))
            {
                steamCmdAlert.Text = "";
                downloadSteamCmdBtn.Enabled = false;
                downloadSteamCmdBtn.Text = "SteamCMD detected";
            }
        }

        public ListBox GetEnabledModsList()
        {
            return enabledMods;
        }

        public ListBox GetAvailableModsList()
        {
            return availableMods;
        }

        private void AddModBtnPressed(object sender, EventArgs e)
        {
            AddModDialog addModDialog = new(this);
            addModDialog.ShowDialog();
        }

        private void RemoveSelectedModBtnPressed(object sender, EventArgs e)
        {
            GetAvailableModsList().Items.Remove(GetAvailableModsList().SelectedItem);
        }

        private void AddToEnabledModsBtnPressed(object sender, EventArgs e)
        {
            if ((Mod)GetAvailableModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetAvailableModsList().SelectedItem;
                GetAvailableModsList().Items.Remove(m);
                GetEnabledModsList().Items.Add(m);
            }
        }

        private void RemovedFromEnabledModsBtnPressed(object sender, EventArgs e)
        {
            if ((Mod)GetEnabledModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetEnabledModsList().SelectedItem;
                GetEnabledModsList().Items.Remove(m);
                GetAvailableModsList().Items.Add(m);
            }
        }

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

        private void LoadSettingsFromFileBtnPressed(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            ofd.Filter = "Text files (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                using StreamReader sr = File.OpenText(filePath);
                PopulateServerConfiguration(sr.ReadToEnd());
            }
        }

        public void WriteModsDatabase()
        {
            using StreamWriter sw = File.AppendText(MOD_DATABASE_FILE);

            foreach (Mod mod in availableMods.Items)
            {
                sw.WriteLine(mod.GetModID() + "," + mod.GetModName());
            }
            sw.Close();
        }

        public void ReadModsDatabase()
        {
            using StreamReader sr = File.OpenText(MOD_DATABASE_FILE);
            string[] lines = sr.ReadToEnd().Trim().Split(Environment.NewLine);
            foreach (string line in lines)
            {
                if (line != null)
                {
                    string[] split = line.Split(",");
                    Mod m = new Mod(split[0], split[1]);
                    if (!GetAvailableModsList().Items.Contains(m))
                    {
                        GetAvailableModsList().Items.Add(m);
                    }
                }
            }
        }

        private void PopulateServerConfiguration(string input)
        {
            const int MINIMUM_CONFIG_FILE_LENGTH = 41;
            string[] configLines = input.Trim().Split(Environment.NewLine);
            List<string> configParams = new List<string>();

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
                    .WithDisableThirdPerson(Convert.ToBoolean(configParams[33]))
                    .WithFastValidation(Convert.ToBoolean(configParams[35]))
                    .WithBattlEye(Convert.ToBoolean(configParams[37]))
                    .WithA2SQueryEnabled(Convert.ToBoolean(configParams[39]))
                    .WithSteamQueryPort(Convert.ToInt32(configParams[41]));

                for (int i = 0; i < configParams.Count; ++i)
                {
                    if (configParams[i].Equals("modId"))
                    {
                        builder.AddModToConfiguration(new Mod(configParams[i + 1], configParams[i + 3]));
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
            }
            else
            {
                MessageBox.Show("Server Config file is malformed and cannot be opened.");
            }
        }

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

        private void DownloadSteamCmdBtnPressed(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += (s, e) =>
                {
                    if (File.Exists(@".\steamcmd.zip"))
                    {
                        ZipFile.ExtractToDirectory(@".\steamcmd.zip", @".\steamcmd");
                    }
                    UpdateSteamCmdInstallStatus();
                };
                client.DownloadFileAsync(new Uri("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip"), @".\steamcmd.zip");
            }


        }

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

        private void StartServerBtnPressed(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SteamCmdUpdateWorkerDoWork;

            if (serverStarted)
            {
                if (File.Exists("./server.json"))
                {
                    File.Delete("./server.json");
                }
                serverStarted = false;
                startServerBtn.Text = "Start Server";
                worker.CancelAsync();
            }
            else
            {
                string jsonConfig = CreateConfiguration().AsJsonString();
                File.WriteAllText(@"./server.json", jsonConfig);
                serverStarted = true;
                startServerBtn.Text = "Stop Server";
                worker.RunWorkerAsync();
            }
        }

        private void SteamCmdDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                steamCmdLog.AppendText(e.Data + Environment.NewLine);
            }
        }

        private void SteamCmdUpdateWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Process steamCmdUpdateProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = ".\\steamcmd\\steamcmd.exe +force_install_dir ..\\Arma_Reforger +login anonymous anonymous +app_update 1874900 +quit";
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
                Process serverProcess = new Process();
                ProcessStartInfo serverStartInfo = new ProcessStartInfo();
                serverStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                serverStartInfo.FileName = "powershell.exe";
                string limitFPSArg = string.Empty;
                if (limitFPS.Checked)
                {
                    limitFPSArg = "-maxFPS " + fpsLimitUpDown.Value.ToString();
                }
                string args = ".\\arma_reforger\\ArmaReforgerServer.exe -config \""
                    + ".\\server.json\" -profile \"" 
                    + Environment.CurrentDirectory 
                    + "\\saves\" -logStats 5000" + limitFPSArg;
                serverStartInfo.Arguments = args;
                serverStartInfo.RedirectStandardOutput = true;
                serverStartInfo.RedirectStandardError = true;
                serverProcess.EnableRaisingEvents = true;
                serverStartInfo.CreateNoWindow = true;
                serverProcess.StartInfo = serverStartInfo;
                serverProcess.OutputDataReceived += SteamCmdDataReceived;
                serverProcess.ErrorDataReceived += SteamCmdDataReceived;
                serverProcess.SynchronizingObject = steamCmdLog;
                serverProcess.Start();
                serverProcess.WaitForExit();
            }
        }

        private void GetScenarioIdWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Process serverProcess = new Process();
            ProcessStartInfo serverStartInfo = new ProcessStartInfo();
            serverStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            serverStartInfo.FileName = "cmd.exe";
            serverStartInfo.Arguments = "/c arma_reforger\\ArmaReforgerServer.exe -listScenarios";
            serverStartInfo.RedirectStandardOutput = true;
            serverStartInfo.RedirectStandardError = true;
            serverProcess.EnableRaisingEvents = true;
            serverStartInfo.CreateNoWindow = true;
            serverProcess.StartInfo = serverStartInfo;
            serverProcess.OutputDataReceived += ListScenariosDataReceived;
            serverProcess.Start();
            serverProcess.WaitForExit();
        }

        private void ListScenariosDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                string[] splitLines = e.Data.Trim().Split(Environment.NewLine);

                foreach (string s in splitLines)
                {
                    if (s.Contains('{'))
                    {
                        scenarioIds.Add(s[s.IndexOf('{')..]);
                    }
                }
            }
        }

        private void PopulateScenarioIdField()
        {
            if (scenarioIds == null)
            {
                scenarioIds = new List<string>();
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += GetScenarioIdWorkerDoWork;
            worker.RunWorkerAsync();
            foreach (string s in scenarioIds)
            {
                scenarioId.Items.Add(s);
            }
        }
    }
}