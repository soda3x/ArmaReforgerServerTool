using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ReforgerServerApp
{
    public partial class ScenarioSelector : Form
    {
        private string launchPath;
        private string scenariosOutput;
        // Matches string like "SCRIPT       : {ECC61978EDCC2B5A}Missions/23_Campaign.conf"
        private string regexScenarioPattern = @"SCRIPT\W*:\W\{.*.conf";
        private Process serverProcess;
        private ServerConfiguration serverConfig;
        private ReforgerServerApp parentForm;

        public ScenarioSelector(ReforgerServerApp parent, ServerConfiguration sc, string path, bool serverInstalled)
        {
            InitializeComponent();
            launchPath = path ?? string.Empty;
            serverConfig = sc;
            PrintSelectedScenario();
            scenariosOutput = string.Empty;
            parentForm = parent;

            serverProcess = new();
            ProcessStartInfo serverStartInfo = new();
            serverStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            serverStartInfo.UseShellExecute = false;
            serverStartInfo.WorkingDirectory = $"{launchPath}\\arma_reforger";
            serverStartInfo.FileName = $"{launchPath}\\arma_reforger\\ArmaReforgerServer.exe";
            serverStartInfo.Arguments = "-listScenarios";
            serverStartInfo.RedirectStandardOutput = true;
            serverStartInfo.RedirectStandardError = true;
            serverStartInfo.CreateNoWindow = true;
            serverProcess.EnableRaisingEvents = true;
            serverProcess.StartInfo = serverStartInfo;

            if (!serverInstalled)
            {
                reloadScenariosBtn.Enabled = false;
                selectScenarioBtn.Enabled = false;
                currentlySelectedLbl.Text = Constants.SERVER_FILES_NOT_FOUND_SCENARIO_SELECT_STR;
            }
            else
            {
                GetScenarios();
            }
        }

        /// <summary>
        /// Main logic here, will call 'Stop' to make sure theres no process already running and then 
        /// it will start the -listScenarios process. It will then kill the process after 3 seconds 
        /// (plenty of time for the script to run)
        /// </summary>
        private void GetScenarios()
        {
            StopServerProcess();
            StartServerProcess();

            // Close the Server after 3 seconds as by this time we should already have our results
            const int THREE_SECONDS_IN_MS = 3000;
            System.Timers.Timer t = new(THREE_SECONDS_IN_MS);
            t.Elapsed += new ElapsedEventHandler(TimerCompleted);
            t.Enabled = true;
        }

        /// <summary>
        /// Logic to start the List Scenarios Process
        /// </summary>
        private void StartServerProcess()
        {
            try
            {
                serverProcess.OutputDataReceived += SteamCmdDataReceived;
                serverProcess.ErrorDataReceived += SteamCmdDataReceived;
                serverProcess.Start();
                serverProcess.BeginOutputReadLine();
                serverProcess.BeginErrorReadLine();
                reloadScenariosBtn.Enabled = false;
                selectScenarioBtn.Enabled = false;
                currentlySelectedLbl.Text = "Retrieving Scenarios...";
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to kill process, probably because its already running.");
            }
        }

        /// <summary>
        /// Logic to stop the List Scenarios Process
        /// </summary>
        private void StopServerProcess()
        {
            try
            {
                serverProcess.OutputDataReceived -= SteamCmdDataReceived;
                serverProcess.ErrorDataReceived -= SteamCmdDataReceived;
                serverProcess.CancelOutputRead();
                serverProcess.CancelErrorRead();
                serverProcess.Kill();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to kill process, probably because its not running.");
            }
            try
            {
                reloadScenariosBtn.Invoke((MethodInvoker)(() => reloadScenariosBtn.Enabled = true));
                selectScenarioBtn.Invoke((MethodInvoker)(() => selectScenarioBtn.Enabled = true));
                currentlySelectedLbl.Invoke((MethodInvoker)(() => PrintSelectedScenario()));
            }
            catch (Exception)
            {
                Console.WriteLine("Unsure why but this needs to be caught...");
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
                lock (scenariosOutput)
                {
                    scenariosOutput = e.Data;

                    foreach (Match match in Regex.Matches(scenariosOutput, regexScenarioPattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
                    {
                        // Remove the extraneous SCRIPT stuff from the start of the scenario and only display the scenario name itself
                        string scenarioRaw = match.Value;
                        string[] scenarioSplit = scenarioRaw.Split(" :");
                        scenarioList.Invoke((MethodInvoker)(() => scenarioList.Items.Add(scenarioSplit[1])));
                    }
                }
            }
        }

        /// <summary>
        /// Handler for when the Reload Scenarios Button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadScenariosButtonClicked(object sender, EventArgs e)
        {
            GetScenarios();
        }

        /// <summary>
        /// Handler for when the Timer for the List Scenarios Server Process elapses
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void TimerCompleted(Object source, ElapsedEventArgs e)
        {
            StopServerProcess();
        }

        /// <summary>
        /// Handler for when Select Scenario Button is pressed. 
        /// It will set the Scenario ID in the Server Config then close the modal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectScenarioButtonClicked(object sender, EventArgs e)
        {
            if (scenarioList.SelectedItem != null)
            {
                serverConfig.ScenarioId = scenarioList.SelectedItem.ToString();
                parentForm.RefreshLoadedScenario();
                this.Close();
            }
        }

        /// <summary>
        /// Handler for when the Scenario List selected item changes to prevent the 
        /// ability to change to a null selection (nothing selected)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioListSelectionChanged(object sender, EventArgs e)
        {
            if (scenarioList.SelectedItem == null)
            {
                selectScenarioBtn.Enabled = false;
            }
            else
            {
                selectScenarioBtn.Enabled = true;
            }
        }

        /// <summary>
        /// If the scenario ID is valid (not empty and not null), print the selected scenario, 
        /// otherwise prompt user to select a scenario from the list
        /// </summary>
        private void PrintSelectedScenario()
        {
            if (serverConfig.ScenarioId != null && serverConfig.ScenarioId != string.Empty)
            {
                currentlySelectedLbl.Text = $"{Constants.CURRENTLY_SELECTED_STR} {serverConfig.ScenarioId}";
            }
            else
            {
                currentlySelectedLbl.Text = Constants.SELECT_SCENARIO_STR;
            }
        }
    }
}
