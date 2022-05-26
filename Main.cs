using Newtonsoft.Json;
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
        private bool serverStarted = false;
        public Main()
        {
            InitializeComponent();
            region.SelectedIndex = 0;
            if (File.Exists(MOD_DATABASE_FILE))
            {
                ReadModsDatabase();
            }
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
            sfd.Filter = "JSON files (*.json)|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, CreateConfiguration().AsJsonString());
            }
        }

        private void LoadSettingsFromFileBtnPressed(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            ofd.Filter = "JSON files (*.json)|*.json";
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
                sw.WriteLine(mod.GetModID() + "," + mod.GetModName() + "," + mod.GetModVersion());
            }
            sw.Close();
        }

        public void ReadModsDatabase()
        {
            using StreamReader sr = File.OpenText(MOD_DATABASE_FILE);

            string line = sr.ReadLine();

            if (line != null)
            {
                string[] split = line.Split(",");
                GetAvailableModsList().Items.Add(new Mod(split[0], split[1], split[2]));
            }
        }

        private void PopulateServerConfiguration(string input)
        {
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);

            if (values != null)
            {
                ServerConfigurationBuilder builder = new();
                builder
                    .WithDedicatedServerId((string)values["dedicatedServerId"])
                    .WithRegion((string)values["region"])
                    .WithGameHostBindAddress((string)values["gameHostBindAddress"])
                    .WithGameHostBindPort((int)values["gameHostBindPort"])
                    .WithGameHostRegisterBindAddress((string)values["gameHostRegisterBindAddress"])
                    .WithGameHostRegisterBindPort((int)values["gameHostRegisterBindPort"])
                    .WithAdminPassword((string)values["adminPassword"])
                    .WithServerName((string)values["serverName"])
                    .WithServerPassword((string)values["password"])
                    .WithScenarioId((string)values["scenarioId"])
                    .WithPlayerCountLimit((int)values["playerCountLimit"])
                    .WithAutoJoinable((bool)values["autoJoinable"])
                    .WithVisible((bool)values["visible"])
                    .WithServerMaxViewDistance((int)values["serverMaxViewDistance"])
                    .WithServerMinGrassDistance((int)values["serverMinGrassDistance"])
                    .WithNetworkViewDistance((int)values["networkViewDistance"])
                    .WithDisableThirdPerson((bool)values["disableThirdPerson"])
                    .WithFastValidation((bool)values["fastValidation"])
                    .WithBattlEye((bool)values["battlEye"])
                    .WithA2SQueryEnabled((bool)values["a2sQueryEnabled"])
                    .WithSteamQueryPort((int)values["steamQueryPort"]);

                foreach (Mod m in enabledMods.Items)
                {
                    builder.AddModToConfiguration(m);
                }
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
                serverStarted = false;
                startServerBtn.Text = "Start Server";
                worker.CancelAsync();
            }
            else
            {
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
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c steamcmd\\steamcmd.exe +force_install_dir ..\\Arma_Reforger +login anonymous anonymous +app_update 1874900 +quit";
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
        }
    }
}