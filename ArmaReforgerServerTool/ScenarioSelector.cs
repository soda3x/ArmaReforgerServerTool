

namespace ReforgerServerApp
{
    public partial class ScenarioSelector : Form
    {
        private ServerConfiguration serverConfig;
        private ReforgerServerApp parentForm;
        Thread getScenariosThread;
        private bool getScenariosThreadRunning = true;
        private bool getScenariosRequested = false;

        public ScenarioSelector(ReforgerServerApp parent, ServerConfiguration sc)
        {
            InitializeComponent();
            serverConfig = sc;
            PrintSelectedScenario();
            parentForm = parent;

            getScenariosRequested = true;
            getScenariosThread = new(new ThreadStart(DoGetScenarios));
            getScenariosThread.Start();
        }

        /// <summary>
        /// Get List of Stock Scenarios
        /// </summary>
        /// <returns>List of strings representing stock scenarios</returns>
        private static List<string> GetStockScenarios()
        {
            List<string> scenarios = new();
            const string campaignScen = "{ECC61978EDCC2B5A}Missions/23_Campaign.conf";
            const string gmEdenScen = "{59AD59368755F41A}Missions/21_GM_Eden.conf";
            const string arlandTutScen = "{94FDA7451242150B}Missions/103_Arland_Tutorial.conf";
            const string gmArlandScen = "{2BBBE828037C6F4B}Missions/22_GM_Arland.conf";
            const string campNthCentralScen = "{C700DB41F0C546E1}Missions/23_Campaign_NorthCentral.conf";
            const string campSWCoastScen = "{28802845ADA64D52}Missions/23_Campaign_SWCoast.conf";
            const string combatOpsScen = "{DAA03C6E6099D50F}Missions/24_CombatOps.conf";
            const string campArlandScen = "{C41618FD18E9D714}Missions/23_Campaign_Arland.conf";
            const string combatOpsEveronScen = "{DFAC5FABD11F2390}Missions/26_CombatOpsEveron.conf";
            scenarios.Add(campaignScen);
            scenarios.Add(gmEdenScen);
            scenarios.Add(arlandTutScen);
            scenarios.Add(gmArlandScen);
            scenarios.Add(campNthCentralScen);
            scenarios.Add(campSWCoastScen);
            scenarios.Add(combatOpsScen);
            scenarios.Add(campArlandScen);
            scenarios.Add(combatOpsEveronScen);
            return scenarios;
        }

        /// <summary>
        /// Retrieve Scenarios from Enabled mods
        /// </summary>
        private void GetScenarios()
        {
            List<string> scenarios = new();
            foreach (Mod m in parentForm.GetEnabledModsList().Items)
            {
                scenarios.AddRange(Mod.GetScenariosForMod(m.GetModID()));
            }

            foreach (string scenId in scenarios)
            {
                scenarioList.Invoke((MethodInvoker)(() => scenarioList.Items.Add(scenId)));
            }
        }

        private void DoGetScenarios()
        {
            while (getScenariosThreadRunning)
            {
                if (getScenariosRequested)
                {
                    if (reloadScenariosBtn.IsHandleCreated)
                    {
                        currentlySelectedLbl.Invoke((MethodInvoker)(() => currentlySelectedLbl.Text = "Fetching scenarios from the workshop..."));
                        getScenariosRequested = false;
                        reloadScenariosBtn.Invoke((MethodInvoker)(() => reloadScenariosBtn.Enabled = false));

                        foreach (String scen in GetStockScenarios())
                        {
                            scenarioList.Invoke((MethodInvoker)(() => scenarioList.Items.Add(scen)));
                        }

                        GetScenarios();
                        reloadScenariosBtn.Invoke((MethodInvoker)(() => reloadScenariosBtn.Enabled = true));
                        currentlySelectedLbl.Invoke((MethodInvoker)(() => currentlySelectedLbl.Text = Constants.SELECT_SCENARIO_STR));
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
            scenarioList.Items.Clear();
            getScenariosRequested = true;
        }

        /// <summary>
        /// Handler for when Select Scenario Button is pressed. 
        /// It will set the Scenario ID in the Server Config then close the modal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectScenarioButtonClicked(object sender, EventArgs e)
        {
            if (manualScenarioIdTextBox.Text != String.Empty)
            {
                serverConfig.ScenarioId = manualScenarioIdTextBox.Text;
            }
            else
            {
                if (scenarioList.SelectedItem != null)
                {
                    serverConfig.ScenarioId = scenarioList.SelectedItem.ToString();

                }
            }
            parentForm.RefreshLoadedScenario();
            this.Close();
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

        /// <summary>
        /// Handler for when text changes in the Manual Scenario ID field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualScenarioIDTextChanged(object sender, EventArgs e)
        {
            _ = manualScenarioIdTextBox.Text != string.Empty || scenarioList.Items.Count > 0 ?
                selectScenarioBtn.Enabled = true : selectScenarioBtn.Enabled = false;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            getScenariosThreadRunning = false;
        }
    }
}
