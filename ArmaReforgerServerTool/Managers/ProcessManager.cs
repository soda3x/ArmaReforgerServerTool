using ReforgerServerApp.Models;
using ReforgerServerApp.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace ReforgerServerApp.Managers
{
    public enum ServerRestartIntervalUnit {  MINUTES, HOURS, DAYS }

    internal class SteamCmdLogEventArgs : EventArgs
    {
        public string line;
        public SteamCmdLogEventArgs(string line) { this.line = line; }
    }

    internal class GuiModelEventArgs : EventArgs
    {
        public string startServerText;
        public bool   enableServerFields;
        public string serverRunningLabelText;
        public bool   startServerBtnEnabled;
        public GuiModelEventArgs() { }
    }

    internal class ProcessManager
    {
        private static ProcessManager?           m_instance;
        private bool                             m_isServerStarted;
        private bool                             m_isServerUsingTimer;
        private Process                          m_steamCmdUpdateProcess;
        private Process                          m_serverProcess;
        private readonly CancellationTokenSource m_timerCancellationTokenSource;
        private LaunchArguments                  m_launchArgumentsModel;

        public delegate void UpdateSteamCmdLogEventHandler(object sender, SteamCmdLogEventArgs e);
        public event UpdateSteamCmdLogEventHandler UpdateSteamCmdLogEvent;

        public delegate void UpdateGuiControlsEventHandler(object sender, GuiModelEventArgs e);
        public event UpdateGuiControlsEventHandler UpdateGuiControlsEvent;

        private ProcessManager()
        {
            m_steamCmdUpdateProcess        = new();
            m_serverProcess                = new();
            m_timerCancellationTokenSource = new();
        }

        public static ProcessManager GetInstance()
        {
            m_instance ??= new ProcessManager();
            return m_instance;
        }

        public bool IsServerStarted() { return m_isServerStarted; }
        public bool IsServerUsingTimer() { return m_isServerUsingTimer; }
        public LaunchArguments GetLaunchArgumentsModel() { return m_launchArgumentsModel; }
        public void SetLaunchArgumentsModel(LaunchArguments la) {  m_launchArgumentsModel = la; }

        /// <summary>
        /// This method controls the logic for Starting and Stopping the Server.
        /// When Starting the server, this will spawn the Worker Thread that runs the SteamCMD and server processes.
        /// When Stopping the server, this will kill the server process and remove the Output / Error redirects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StartStopServer()
        {
            BackgroundWorker worker = new();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SteamCmdUpdateWorkerDoWork;

            if (m_isServerStarted)
            {
                if (File.Exists(FileIOManager.GetInstance().GetInstallDirectory() + Constants.SERVER_JSON_STR))
                {
                    File.Delete(FileIOManager.GetInstance().GetInstallDirectory() + Constants.SERVER_JSON_STR);
                }
                worker.CancelAsync();
                try
                {
                    m_serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    m_serverProcess.ErrorDataReceived  -= SteamCmdDataReceived;
                    m_serverProcess.CancelOutputRead();
                    m_serverProcess.CancelErrorRead();

                    SteamCmdLogEventArgs steamCmd = new($"{Utilities.GetTimestamp()}: User stopped server.{Environment.NewLine}");
                    OnUpdateSteamCmdLogEvent(steamCmd);

                    m_serverProcess.Kill();
                    m_isServerStarted = false;

                    GuiModelEventArgs guiModel = new()
                    {
                        startServerText = Constants.START_SERVER_STR,
                        enableServerFields = true,
                        serverRunningLabelText = string.Empty,
                        startServerBtnEnabled = true
                    };
                    OnUpdateGuiControlsEvent(guiModel);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", Constants.ERROR_MESSAGEBOX_TITLE_STR);
                }
            }
            else
            {
                string jsonConfig = ConfigurationManager.GetInstance().GetServerConfiguration().AsJsonString();
                File.WriteAllText(FileIOManager.GetInstance().GetInstallDirectory() + Constants.SERVER_JSON_STR, jsonConfig);
                m_isServerStarted = true;

                GuiModelEventArgs guiModel = new()
                {
                    startServerText = Constants.STOP_SERVER_STR,
                    enableServerFields = false,
                    serverRunningLabelText = Constants.SERVER_CURRENTLY_RUNNING_STR,
                    startServerBtnEnabled = false
                };
                OnUpdateGuiControlsEvent(guiModel);

                SteamCmdLogEventArgs steamCmd = new($"{Utilities.GetTimestamp()}: User started server.{Environment.NewLine}");
                OnUpdateSteamCmdLogEvent(steamCmd);

                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// This is almost identical to the DoStartStopServerLogic method with the difference being it
        /// automatically restarts the server after stopping it as it's not a toggle.
        /// </summary>
        private void StartStopServerUsingTimer()
        {
            BackgroundWorker worker = new();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SteamCmdUpdateWorkerDoWork;
            GuiModelEventArgs guiModel;
            SteamCmdLogEventArgs steamCmd;

            if (m_isServerStarted)
            {
                if (File.Exists($"{FileIOManager.GetInstance().GetInstallDirectory()}{Constants.SERVER_JSON_STR}"))
                {
                    File.Delete($"{FileIOManager.GetInstance().GetInstallDirectory()}{Constants.SERVER_JSON_STR}");
                }
                m_isServerStarted = false;
                worker.CancelAsync();

                guiModel = new()
                {
                    startServerText = Constants.START_SERVER_STR,
                    enableServerFields = true,
                    serverRunningLabelText = string.Empty,
                    startServerBtnEnabled = true
                };
                OnUpdateGuiControlsEvent(guiModel);

                try
                {
                    m_serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    m_serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                    m_serverProcess.CancelOutputRead();
                    m_serverProcess.CancelErrorRead();

                    steamCmd = new($"{Utilities.GetTimestamp()}: Automatically stopped server.{Environment.NewLine}");
                    OnUpdateSteamCmdLogEvent(steamCmd);

                    m_serverProcess.Kill();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", Constants.ERROR_MESSAGEBOX_TITLE_STR);
                }
            }
            string jsonConfig = ConfigurationManager.GetInstance().GetServerConfiguration().AsJsonString();
            File.WriteAllText($"{FileIOManager.GetInstance().GetInstallDirectory()}{Constants.SERVER_JSON_STR}", jsonConfig);
            m_isServerStarted = true;

            guiModel = new()
            {
                startServerText = Constants.STOP_SERVER_STR,
                enableServerFields = false,
                serverRunningLabelText = Constants.SERVER_CURRENTLY_RUNNING_STR,
                startServerBtnEnabled = false
            };
            OnUpdateGuiControlsEvent(guiModel);

            steamCmd = new($"{Utilities.GetTimestamp()}: Automatically started server.{Environment.NewLine}");
            OnUpdateSteamCmdLogEvent(steamCmd);

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
                SteamCmdLogEventArgs steamCmd = new($"{Utilities.GetTimestamp()}: {e.Data}{Environment.NewLine}");
                OnUpdateSteamCmdLogEvent(steamCmd);
                // Kill the server if it fails to start correctly.
                if (e.Data.Contains("Unable to Initialize"))
                {
                    steamCmd = new($"{Utilities.GetTimestamp()}: System stopped server due to an error.{Environment.NewLine}");
                    OnUpdateSteamCmdLogEvent(steamCmd);
                    m_serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                    m_serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                    m_serverProcess.CancelOutputRead();
                    m_serverProcess.CancelErrorRead();
                    m_serverProcess.Kill();

                    m_isServerStarted = false;

                    GuiModelEventArgs guiModel = new()
                    {
                        startServerText = Constants.START_SERVER_STR,
                        enableServerFields = true,
                        serverRunningLabelText = string.Empty,
                        startServerBtnEnabled = true
                    };
                    OnUpdateGuiControlsEvent(guiModel);
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
            m_steamCmdUpdateProcess = new();
            ProcessStartInfo startInfo = new();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = FileIOManager.GetInstance().GetSteamCmdFile();
            startInfo.Arguments = "+force_install_dir ..\\Arma_Reforger +login anonymous anonymous +app_update 1874900 +quit";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            m_steamCmdUpdateProcess.EnableRaisingEvents = true;
            startInfo.CreateNoWindow = true;
            m_steamCmdUpdateProcess.StartInfo = startInfo;
            m_steamCmdUpdateProcess.OutputDataReceived += SteamCmdDataReceived;
            m_steamCmdUpdateProcess.ErrorDataReceived += SteamCmdDataReceived;
            m_steamCmdUpdateProcess.Start();
            m_steamCmdUpdateProcess.BeginOutputReadLine();
            m_steamCmdUpdateProcess.BeginErrorReadLine();
            m_steamCmdUpdateProcess.WaitForExit();

            if (m_steamCmdUpdateProcess.HasExited)
            {
                GuiModelEventArgs guiModel = new()
                {
                    startServerText = Constants.START_SERVER_STR,
                    enableServerFields = true,
                    serverRunningLabelText = string.Empty,
                    startServerBtnEnabled = true
                };
                OnUpdateGuiControlsEvent(guiModel);

                m_serverProcess = new();
                ProcessStartInfo serverStartInfo = new();
                serverStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                serverStartInfo.UseShellExecute = false;
                serverStartInfo.WorkingDirectory = $"{FileIOManager.GetInstance().GetInstallDirectory()}\\arma_reforger";
                serverStartInfo.FileName = $"{FileIOManager.GetInstance().GetInstallDirectory()}\\arma_reforger\\ArmaReforgerServer.exe";
                serverStartInfo.Arguments = GetLaunchArguments();
                serverStartInfo.RedirectStandardOutput = true;
                serverStartInfo.RedirectStandardError = true;
                serverStartInfo.CreateNoWindow = true;
                m_serverProcess.EnableRaisingEvents = true;
                m_serverProcess.StartInfo = serverStartInfo;
                m_serverProcess.OutputDataReceived += SteamCmdDataReceived;
                m_serverProcess.ErrorDataReceived += SteamCmdDataReceived;
                m_serverProcess.Start();
                m_serverProcess.BeginOutputReadLine();
                m_serverProcess.BeginErrorReadLine();
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
        /// Configure the timer to start the server
        /// </summary>
        /// <param name="interval">to set the timer to use</param>
        public void ConfigureAutomaticRestartTask(TimeSpan interval)
        {
            Task automaticRestartTask = PeriodicAsync(StartStopServerUsingTimer, interval, m_timerCancellationTokenSource.Token);
            m_isServerUsingTimer = true;
        }

        /// <summary>
        /// Cancel the Automatic Server Restart Task and timer
        /// This will also shut down the server
        /// </summary>
        public void CancelAutomaticRestartTask()
        {
            m_timerCancellationTokenSource.Cancel();
            StartStopServer();
            m_isServerUsingTimer = false;
        }

        /// <summary>
        /// Sender for the 'OnUpdateSteamCmdLog' Event
        /// </summary>
        /// <param name="e">Arguments to pass to the GUI to inform it that it needs to update the Steam CMD Log</param>
        protected virtual void OnUpdateSteamCmdLogEvent(SteamCmdLogEventArgs e)
        {
            UpdateSteamCmdLogEvent?.Invoke(this, e);
        }

        /// <summary>
        /// Sender for the 'OnUpdateGuiControls' Event
        /// </summary>
        /// <param name="e">Arguments to pass to the GUI to inform it that it needs to update various controls</param>
        protected virtual void OnUpdateGuiControlsEvent(GuiModelEventArgs e)
        {
            UpdateGuiControlsEvent?.Invoke(this, e);
        }

        /// <summary>
        /// Get Launch Arguments as a String
        /// </summary>
        /// <returns>String representation of Launch Arguments</returns>
        public string GetLaunchArguments()
        {
            return string.Join(",", m_launchArgumentsModel.config,
                                    m_launchArgumentsModel.profile,
                                    m_launchArgumentsModel.logStats,
                                    m_launchArgumentsModel.maxFPS,
                                    m_launchArgumentsModel.bindPort,
                                    m_launchArgumentsModel.nds,
                                    m_launchArgumentsModel.nwkResolution,
                                    m_launchArgumentsModel.staggeringBudget,
                                    m_launchArgumentsModel.streamingBudget,
                                    m_launchArgumentsModel.streamsDelta,
                                    m_launchArgumentsModel.loadSessionSave,
                                    m_launchArgumentsModel.logLevel);
        }
    }
}
