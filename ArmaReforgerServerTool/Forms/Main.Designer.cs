using ReforgerServerApp.Components;

namespace ReforgerServerApp
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            editModBtn = new Button();
            serverRunningLabel = new Label();
            addModBtn = new Button();
            removeModBtn = new Button();
            groupBox2 = new GroupBox();
            modsSearchTB = new TextBox();
            disableAllModsBtn = new Button();
            enableAllModsBtn = new Button();
            label16 = new Label();
            label15 = new Label();
            removeFromEnabledBtn = new Button();
            addToEnabledBtn = new Button();
            enabledMods = new BoundListBox();
            availableMods = new BoundListBox();
            groupBox1 = new GroupBox();
            serverParameters = new FlowLayoutPanel();
            editMissionHeaderBtn = new Button();
            loadedScenarioLabel = new Label();
            scenarioSelectBtn = new Button();
            pictureBox1 = new PictureBox();
            saveSettingsBtn = new Button();
            loadSettingsBtn = new Button();
            tabPage2 = new TabPage();
            label30 = new Label();
            logLevelComboBox = new ComboBox();
            locateServerFilesBtn = new Button();
            clearLogBtn = new Button();
            deleteServerFilesBtn = new Button();
            aboutBtn = new Button();
            groupBox4 = new GroupBox();
            panel1 = new Panel();
            sessionSave = new TextBox();
            loadSessionSaveLabel = new Label();
            loadSessionSave = new CheckBox();
            limitFPS = new CheckBox();
            streamsDeltaUpDown = new NumericUpDown();
            nwkResolutionUpDown = new NumericUpDown();
            label20 = new Label();
            nwkResolutionLabel = new Label();
            restartUnitsComboBox = new ComboBox();
            staggeringBudget = new CheckBox();
            streamsDeltaLabel = new Label();
            nwkResolution = new CheckBox();
            forcePortCheckBox = new CheckBox();
            label28 = new Label();
            restartIntervalUpDown = new NumericUpDown();
            staggeringBudgetLabel = new Label();
            streamsDelta = new CheckBox();
            ndsUpDown = new NumericUpDown();
            label23 = new Label();
            staggeringBudgetUpDown = new NumericUpDown();
            label10 = new Label();
            fpsLimitUpDown = new NumericUpDown();
            streamingBudgetUpDown = new NumericUpDown();
            ndsLabel = new Label();
            overridePortNumericUpDown = new NumericUpDown();
            label27 = new Label();
            label6 = new Label();
            label21 = new Label();
            streamingBudgetLabel = new Label();
            nds = new CheckBox();
            label22 = new Label();
            streamingBudget = new CheckBox();
            automaticallyRestart = new CheckBox();
            startServerBtn = new Button();
            groupBox3 = new GroupBox();
            steamCmdLog = new TextBox();
            steamCmdAlert = new Label();
            downloadSteamCmdBtn = new Button();
            useExperimentalCheckBox = new CheckBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage2.SuspendLayout();
            groupBox4.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)streamsDeltaUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nwkResolutionUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)restartIntervalUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ndsUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)staggeringBudgetUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)fpsLimitUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)streamingBudgetUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)overridePortNumericUpDown).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1526, 778);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(editModBtn);
            tabPage1.Controls.Add(serverRunningLabel);
            tabPage1.Controls.Add(addModBtn);
            tabPage1.Controls.Add(removeModBtn);
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(saveSettingsBtn);
            tabPage1.Controls.Add(loadSettingsBtn);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Margin = new Padding(3, 4, 3, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3, 4, 3, 4);
            tabPage1.Size = new Size(1736, 1004);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Server Configuration";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // editModBtn
            // 
            editModBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            editModBtn.Enabled = false;
            editModBtn.Location = new Point(97, 961);
            editModBtn.Name = "editModBtn";
            editModBtn.Size = new Size(132, 23);
            editModBtn.TabIndex = 51;
            editModBtn.Text = "Edit Selected Mod";
            editModBtn.UseVisualStyleBackColor = true;
            editModBtn.Click += EditModBtnPressed;
            // 
            // serverRunningLabel
            // 
            serverRunningLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            serverRunningLabel.AutoSize = true;
            serverRunningLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            serverRunningLabel.Location = new Point(576, 964);
            serverRunningLabel.Name = "serverRunningLabel";
            serverRunningLabel.Size = new Size(162, 23);
            serverRunningLabel.TabIndex = 50;
            serverRunningLabel.Text = "serverRunningLabel";
            // 
            // addModBtn
            // 
            addModBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addModBtn.Location = new Point(7, 961);
            addModBtn.Margin = new Padding(3, 4, 3, 4);
            addModBtn.Name = "addModBtn";
            addModBtn.Size = new Size(83, 31);
            addModBtn.TabIndex = 49;
            addModBtn.Text = "Add Mod";
            addModBtn.UseVisualStyleBackColor = true;
            addModBtn.Click += AddModBtnPressed;
            // 
            // removeModBtn
            // 
            removeModBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            removeModBtn.Enabled = false;
            removeModBtn.Location = new Point(255, 961);
            removeModBtn.Margin = new Padding(3, 4, 3, 4);
            removeModBtn.Name = "removeModBtn";
            removeModBtn.Size = new Size(167, 31);
            removeModBtn.TabIndex = 48;
            removeModBtn.Text = "Remove Selected Mod";
            removeModBtn.UseVisualStyleBackColor = true;
            removeModBtn.Click += RemoveSelectedModBtnPressed;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox2.Controls.Add(modsSearchTB);
            groupBox2.Controls.Add(disableAllModsBtn);
            groupBox2.Controls.Add(enableAllModsBtn);
            groupBox2.Controls.Add(label16);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(removeFromEnabledBtn);
            groupBox2.Controls.Add(addToEnabledBtn);
            groupBox2.Controls.Add(enabledMods);
            groupBox2.Controls.Add(availableMods);
            groupBox2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox2.Location = new Point(7, 8);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(727, 945);
            groupBox2.TabIndex = 47;
            groupBox2.TabStop = false;
            groupBox2.Text = "Mods";
            // 
            // modsSearchTB
            // 
            modsSearchTB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            modsSearchTB.Location = new Point(7, 53);
            modsSearchTB.Margin = new Padding(3, 4, 3, 4);
            modsSearchTB.Name = "modsSearchTB";
            modsSearchTB.PlaceholderText = "Search Mods...";
            modsSearchTB.Size = new Size(708, 27);
            modsSearchTB.TabIndex = 8;
            modsSearchTB.TextChanged += OnSearchModsTextChanged;
            // 
            // disableAllModsBtn
            // 
            disableAllModsBtn.Location = new Point(350, 564);
            disableAllModsBtn.Margin = new Padding(3, 4, 3, 4);
            disableAllModsBtn.Name = "disableAllModsBtn";
            disableAllModsBtn.Size = new Size(26, 69);
            disableAllModsBtn.TabIndex = 7;
            disableAllModsBtn.Text = "<<";
            disableAllModsBtn.UseVisualStyleBackColor = true;
            disableAllModsBtn.Click += DisableAllModsBtnPressed;
            // 
            // enableAllModsBtn
            // 
            enableAllModsBtn.Location = new Point(350, 276);
            enableAllModsBtn.Margin = new Padding(3, 4, 3, 4);
            enableAllModsBtn.Name = "enableAllModsBtn";
            enableAllModsBtn.Size = new Size(26, 69);
            enableAllModsBtn.TabIndex = 6;
            enableAllModsBtn.Text = ">>";
            enableAllModsBtn.UseVisualStyleBackColor = true;
            enableAllModsBtn.Click += EnableAllModsBtnPressed;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(383, 25);
            label16.Name = "label16";
            label16.Size = new Size(106, 20);
            label16.TabIndex = 5;
            label16.Text = "Enabled Mods";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(7, 25);
            label15.Name = "label15";
            label15.Size = new Size(114, 20);
            label15.TabIndex = 4;
            label15.Text = "Available Mods";
            // 
            // removeFromEnabledBtn
            // 
            removeFromEnabledBtn.Location = new Point(350, 468);
            removeFromEnabledBtn.Margin = new Padding(3, 4, 3, 4);
            removeFromEnabledBtn.Name = "removeFromEnabledBtn";
            removeFromEnabledBtn.Size = new Size(26, 69);
            removeFromEnabledBtn.TabIndex = 3;
            removeFromEnabledBtn.Text = "<";
            removeFromEnabledBtn.UseVisualStyleBackColor = true;
            removeFromEnabledBtn.Click += RemovedFromEnabledModsBtnPressed;
            // 
            // addToEnabledBtn
            // 
            addToEnabledBtn.Location = new Point(350, 369);
            addToEnabledBtn.Margin = new Padding(3, 4, 3, 4);
            addToEnabledBtn.Name = "addToEnabledBtn";
            addToEnabledBtn.Size = new Size(26, 69);
            addToEnabledBtn.TabIndex = 2;
            addToEnabledBtn.Text = ">";
            addToEnabledBtn.UseVisualStyleBackColor = true;
            addToEnabledBtn.Click += AddToEnabledModsBtnPressed;
            // 
            // enabledMods
            // 
            enabledMods.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            enabledMods.FormattingEnabled = true;
            enabledMods.ItemHeight = 20;
            enabledMods.Location = new Point(383, 99);
            enabledMods.Margin = new Padding(3, 4, 3, 4);
            enabledMods.Name = "enabledMods";
            enabledMods.SelectionMode = SelectionMode.MultiExtended;
            enabledMods.Size = new Size(332, 824);
            enabledMods.TabIndex = 1;
            // 
            // availableMods
            // 
            availableMods.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            availableMods.FormattingEnabled = true;
            availableMods.ItemHeight = 20;
            availableMods.Location = new Point(7, 99);
            availableMods.Margin = new Padding(3, 4, 3, 4);
            availableMods.Name = "availableMods";
            availableMods.SelectionMode = SelectionMode.MultiExtended;
            availableMods.Size = new Size(335, 824);
            availableMods.TabIndex = 0;
            availableMods.SelectedIndexChanged += AvailableModsSelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(serverParameters);
            groupBox1.Controls.Add(editMissionHeaderBtn);
            groupBox1.Controls.Add(loadedScenarioLabel);
            groupBox1.Controls.Add(scenarioSelectBtn);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox1.Location = new Point(741, 8);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(979, 945);
            groupBox1.TabIndex = 46;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server Settings";
            // 
            // serverParameters
            // 
            serverParameters.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            serverParameters.AutoScroll = true;
            serverParameters.FlowDirection = FlowDirection.TopDown;
            serverParameters.Location = new Point(7, 295);
            serverParameters.Margin = new Padding(17, 0, 17, 0);
            serverParameters.Name = "serverParameters";
            serverParameters.Size = new Size(966, 564);
            serverParameters.TabIndex = 90;
            // 
            // editMissionHeaderBtn
            // 
            editMissionHeaderBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            editMissionHeaderBtn.Location = new Point(216, 867);
            editMissionHeaderBtn.Margin = new Padding(3, 4, 3, 4);
            editMissionHeaderBtn.Name = "editMissionHeaderBtn";
            editMissionHeaderBtn.Size = new Size(202, 31);
            editMissionHeaderBtn.TabIndex = 85;
            editMissionHeaderBtn.Text = "Edit Mission Header";
            editMissionHeaderBtn.UseVisualStyleBackColor = true;
            editMissionHeaderBtn.Click += EditMissionHeaderBtnClicked;
            // 
            // loadedScenarioLabel
            // 
            loadedScenarioLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            loadedScenarioLabel.AutoEllipsis = true;
            loadedScenarioLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            loadedScenarioLabel.Location = new Point(7, 901);
            loadedScenarioLabel.Name = "loadedScenarioLabel";
            loadedScenarioLabel.Size = new Size(962, 28);
            loadedScenarioLabel.TabIndex = 83;
            loadedScenarioLabel.Text = "Scenario ID";
            loadedScenarioLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // scenarioSelectBtn
            // 
            scenarioSelectBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            scenarioSelectBtn.Location = new Point(7, 867);
            scenarioSelectBtn.Margin = new Padding(3, 4, 3, 4);
            scenarioSelectBtn.Name = "scenarioSelectBtn";
            scenarioSelectBtn.Size = new Size(202, 31);
            scenarioSelectBtn.TabIndex = 82;
            scenarioSelectBtn.Text = "Select a Scenario";
            scenarioSelectBtn.UseVisualStyleBackColor = true;
            scenarioSelectBtn.Click += ScenarioSelectBtnClicked;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(122, 25);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(721, 265);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 51;
            pictureBox1.TabStop = false;
            // 
            // saveSettingsBtn
            // 
            saveSettingsBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveSettingsBtn.Location = new Point(1545, 961);
            saveSettingsBtn.Margin = new Padding(3, 4, 3, 4);
            saveSettingsBtn.Name = "saveSettingsBtn";
            saveSettingsBtn.Size = new Size(175, 31);
            saveSettingsBtn.TabIndex = 2;
            saveSettingsBtn.Text = "Save Settings to File";
            saveSettingsBtn.UseVisualStyleBackColor = true;
            saveSettingsBtn.Click += SaveSettingsToFileBtnPressed;
            // 
            // loadSettingsBtn
            // 
            loadSettingsBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            loadSettingsBtn.Location = new Point(1363, 961);
            loadSettingsBtn.Margin = new Padding(3, 4, 3, 4);
            loadSettingsBtn.Name = "loadSettingsBtn";
            loadSettingsBtn.Size = new Size(175, 31);
            loadSettingsBtn.TabIndex = 1;
            loadSettingsBtn.Text = "Load Settings from File";
            loadSettingsBtn.UseVisualStyleBackColor = true;
            loadSettingsBtn.Click += LoadSettingsFromFileBtnPressed;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(useExperimentalCheckBox);
            tabPage2.Controls.Add(label30);
            tabPage2.Controls.Add(logLevelComboBox);
            tabPage2.Controls.Add(locateServerFilesBtn);
            tabPage2.Controls.Add(clearLogBtn);
            tabPage2.Controls.Add(deleteServerFilesBtn);
            tabPage2.Controls.Add(aboutBtn);
            tabPage2.Controls.Add(groupBox4);
            tabPage2.Controls.Add(startServerBtn);
            tabPage2.Controls.Add(groupBox3);
            tabPage2.Controls.Add(steamCmdAlert);
            tabPage2.Controls.Add(downloadSteamCmdBtn);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Margin = new Padding(3, 4, 3, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3, 4, 3, 4);
            tabPage2.Size = new Size(1736, 1004);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Server Management";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // useExperimentalCheckBox
            // 
            useExperimentalCheckBox.AutoSize = true;
            useExperimentalCheckBox.Location = new Point(216, 41);
            useExperimentalCheckBox.Margin = new Padding(3, 4, 3, 4);
            useExperimentalCheckBox.Name = "useExperimentalCheckBox";
            useExperimentalCheckBox.Size = new Size(191, 24);
            useExperimentalCheckBox.TabIndex = 31;
            useExperimentalCheckBox.Text = "Use Experimental Server";
            useExperimentalCheckBox.UseVisualStyleBackColor = true;
            useExperimentalCheckBox.CheckedChanged += UseExperimentalServerCheckboxChanged;
            // 
            // label30
            // 
            label30.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label30.AutoSize = true;
            label30.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label30.Location = new Point(267, 967);
            label30.Name = "label30";
            label30.Size = new Size(73, 20);
            label30.TabIndex = 30;
            label30.Text = "Log Level";
            // 
            // logLevelComboBox
            // 
            logLevelComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logLevelComboBox.FormattingEnabled = true;
            logLevelComboBox.Items.AddRange(new object[] { "normal", "warning", "error", "fatal" });
            logLevelComboBox.Location = new Point(339, 961);
            logLevelComboBox.Margin = new Padding(3, 4, 3, 4);
            logLevelComboBox.Name = "logLevelComboBox";
            logLevelComboBox.Size = new Size(109, 28);
            logLevelComboBox.TabIndex = 30;
            logLevelComboBox.Text = "normal";
            // 
            // locateServerFilesBtn
            // 
            locateServerFilesBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            locateServerFilesBtn.Location = new Point(1397, 37);
            locateServerFilesBtn.Margin = new Padding(3, 4, 3, 4);
            locateServerFilesBtn.Name = "locateServerFilesBtn";
            locateServerFilesBtn.Size = new Size(127, 31);
            locateServerFilesBtn.TabIndex = 7;
            locateServerFilesBtn.Text = "Locate Server Files";
            locateServerFilesBtn.UseVisualStyleBackColor = true;
            locateServerFilesBtn.Click += LocateServerFilesBtnPressed;
            // 
            // clearLogBtn
            // 
            clearLogBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            clearLogBtn.Location = new Point(1653, 963);
            clearLogBtn.Margin = new Padding(3, 4, 3, 4);
            clearLogBtn.Name = "clearLogBtn";
            clearLogBtn.Size = new Size(75, 31);
            clearLogBtn.TabIndex = 6;
            clearLogBtn.Text = "Clear Log";
            clearLogBtn.UseVisualStyleBackColor = true;
            clearLogBtn.Click += ClearLogBtnPressed;
            // 
            // deleteServerFilesBtn
            // 
            deleteServerFilesBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            deleteServerFilesBtn.Location = new Point(1530, 37);
            deleteServerFilesBtn.Margin = new Padding(3, 4, 3, 4);
            deleteServerFilesBtn.Name = "deleteServerFilesBtn";
            deleteServerFilesBtn.Size = new Size(127, 31);
            deleteServerFilesBtn.TabIndex = 5;
            deleteServerFilesBtn.Text = "Delete Server Files";
            deleteServerFilesBtn.UseVisualStyleBackColor = true;
            deleteServerFilesBtn.Click += DeleteServerFilesBtnPressed;
            // 
            // aboutBtn
            // 
            aboutBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            aboutBtn.Location = new Point(1664, 37);
            aboutBtn.Margin = new Padding(3, 4, 3, 4);
            aboutBtn.Name = "aboutBtn";
            aboutBtn.Size = new Size(64, 31);
            aboutBtn.TabIndex = 4;
            aboutBtn.Text = "About";
            aboutBtn.UseVisualStyleBackColor = true;
            aboutBtn.Click += AboutBtnPressed;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox4.Controls.Add(panel1);
            groupBox4.Location = new Point(7, 76);
            groupBox4.Margin = new Padding(3, 4, 3, 4);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(3, 4, 3, 4);
            groupBox4.Size = new Size(254, 883);
            groupBox4.TabIndex = 4;
            groupBox4.TabStop = false;
            groupBox4.Text = "Advanced";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel1.AutoScroll = true;
            panel1.Controls.Add(sessionSave);
            panel1.Controls.Add(loadSessionSaveLabel);
            panel1.Controls.Add(loadSessionSave);
            panel1.Controls.Add(limitFPS);
            panel1.Controls.Add(streamsDeltaUpDown);
            panel1.Controls.Add(nwkResolutionUpDown);
            panel1.Controls.Add(label20);
            panel1.Controls.Add(nwkResolutionLabel);
            panel1.Controls.Add(restartUnitsComboBox);
            panel1.Controls.Add(staggeringBudget);
            panel1.Controls.Add(streamsDeltaLabel);
            panel1.Controls.Add(nwkResolution);
            panel1.Controls.Add(forcePortCheckBox);
            panel1.Controls.Add(label28);
            panel1.Controls.Add(restartIntervalUpDown);
            panel1.Controls.Add(staggeringBudgetLabel);
            panel1.Controls.Add(streamsDelta);
            panel1.Controls.Add(ndsUpDown);
            panel1.Controls.Add(label23);
            panel1.Controls.Add(staggeringBudgetUpDown);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(fpsLimitUpDown);
            panel1.Controls.Add(streamingBudgetUpDown);
            panel1.Controls.Add(ndsLabel);
            panel1.Controls.Add(overridePortNumericUpDown);
            panel1.Controls.Add(label27);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label21);
            panel1.Controls.Add(streamingBudgetLabel);
            panel1.Controls.Add(nds);
            panel1.Controls.Add(label22);
            panel1.Controls.Add(streamingBudget);
            panel1.Controls.Add(automaticallyRestart);
            panel1.Location = new Point(3, 25);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(247, 853);
            panel1.TabIndex = 30;
            // 
            // sessionSave
            // 
            sessionSave.Location = new Point(3, 767);
            sessionSave.Margin = new Padding(3, 4, 3, 4);
            sessionSave.Name = "sessionSave";
            sessionSave.Size = new Size(227, 27);
            sessionSave.TabIndex = 33;
            // 
            // loadSessionSaveLabel
            // 
            loadSessionSaveLabel.AutoSize = true;
            loadSessionSaveLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            loadSessionSaveLabel.Location = new Point(27, 739);
            loadSessionSaveLabel.Name = "loadSessionSaveLabel";
            loadSessionSaveLabel.Size = new Size(132, 20);
            loadSessionSaveLabel.TabIndex = 31;
            loadSessionSaveLabel.Text = "Load Session Save";
            // 
            // loadSessionSave
            // 
            loadSessionSave.AutoSize = true;
            loadSessionSave.Location = new Point(3, 740);
            loadSessionSave.Margin = new Padding(3, 4, 3, 4);
            loadSessionSave.Name = "loadSessionSave";
            loadSessionSave.Size = new Size(18, 17);
            loadSessionSave.TabIndex = 30;
            loadSessionSave.UseVisualStyleBackColor = true;
            loadSessionSave.CheckedChanged += LoadSessionSaveCheckChanged;
            // 
            // limitFPS
            // 
            limitFPS.AutoSize = true;
            limitFPS.Location = new Point(3, 5);
            limitFPS.Margin = new Padding(3, 4, 3, 4);
            limitFPS.Name = "limitFPS";
            limitFPS.Size = new Size(18, 17);
            limitFPS.TabIndex = 0;
            limitFPS.UseVisualStyleBackColor = true;
            limitFPS.CheckedChanged += LimitFPSCheckedChanged;
            // 
            // streamsDeltaUpDown
            // 
            streamsDeltaUpDown.Location = new Point(3, 696);
            streamsDeltaUpDown.Margin = new Padding(3, 4, 3, 4);
            streamsDeltaUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            streamsDeltaUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            streamsDeltaUpDown.Name = "streamsDeltaUpDown";
            streamsDeltaUpDown.Size = new Size(227, 27);
            streamsDeltaUpDown.TabIndex = 29;
            streamsDeltaUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nwkResolutionUpDown
            // 
            nwkResolutionUpDown.Location = new Point(3, 425);
            nwkResolutionUpDown.Margin = new Padding(3, 4, 3, 4);
            nwkResolutionUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nwkResolutionUpDown.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nwkResolutionUpDown.Name = "nwkResolutionUpDown";
            nwkResolutionUpDown.Size = new Size(227, 27);
            nwkResolutionUpDown.TabIndex = 19;
            nwkResolutionUpDown.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label20.Location = new Point(27, 4);
            label20.Name = "label20";
            label20.Size = new Size(154, 20);
            label20.TabIndex = 1;
            label20.Text = "Limit Server Max FPS";
            // 
            // nwkResolutionLabel
            // 
            nwkResolutionLabel.AutoSize = true;
            nwkResolutionLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            nwkResolutionLabel.Location = new Point(27, 397);
            nwkResolutionLabel.Name = "nwkResolutionLabel";
            nwkResolutionLabel.Size = new Size(166, 20);
            nwkResolutionLabel.TabIndex = 18;
            nwkResolutionLabel.Text = "Spatial Map Resolution";
            // 
            // restartUnitsComboBox
            // 
            restartUnitsComboBox.FormattingEnabled = true;
            restartUnitsComboBox.Items.AddRange(new object[] { "Mins", "Hours", "Days" });
            restartUnitsComboBox.Location = new Point(162, 128);
            restartUnitsComboBox.Margin = new Padding(3, 4, 3, 4);
            restartUnitsComboBox.Name = "restartUnitsComboBox";
            restartUnitsComboBox.Size = new Size(68, 28);
            restartUnitsComboBox.TabIndex = 8;
            restartUnitsComboBox.Text = "Mins";
            // 
            // staggeringBudget
            // 
            staggeringBudget.AutoSize = true;
            staggeringBudget.Location = new Point(3, 473);
            staggeringBudget.Margin = new Padding(3, 4, 3, 4);
            staggeringBudget.Name = "staggeringBudget";
            staggeringBudget.Size = new Size(18, 17);
            staggeringBudget.TabIndex = 20;
            staggeringBudget.UseVisualStyleBackColor = true;
            staggeringBudget.CheckedChanged += StaggeringBudgetCheckChanged;
            // 
            // streamsDeltaLabel
            // 
            streamsDeltaLabel.AutoSize = true;
            streamsDeltaLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            streamsDeltaLabel.Location = new Point(27, 668);
            streamsDeltaLabel.Name = "streamsDeltaLabel";
            streamsDeltaLabel.Size = new Size(103, 20);
            streamsDeltaLabel.TabIndex = 28;
            streamsDeltaLabel.Text = "Streams Delta";
            // 
            // nwkResolution
            // 
            nwkResolution.AutoSize = true;
            nwkResolution.Location = new Point(3, 399);
            nwkResolution.Margin = new Padding(3, 4, 3, 4);
            nwkResolution.Name = "nwkResolution";
            nwkResolution.Size = new Size(18, 17);
            nwkResolution.TabIndex = 17;
            nwkResolution.UseVisualStyleBackColor = true;
            nwkResolution.CheckedChanged += NWKCheckChanged;
            // 
            // forcePortCheckBox
            // 
            forcePortCheckBox.AutoSize = true;
            forcePortCheckBox.Location = new Point(3, 183);
            forcePortCheckBox.Margin = new Padding(3, 4, 3, 4);
            forcePortCheckBox.Name = "forcePortCheckBox";
            forcePortCheckBox.Size = new Size(18, 17);
            forcePortCheckBox.TabIndex = 9;
            forcePortCheckBox.UseVisualStyleBackColor = true;
            forcePortCheckBox.CheckedChanged += OverridePortCheckChanged;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold, GraphicsUnit.Point);
            label28.Location = new Point(3, 353);
            label28.Name = "label28";
            label28.Size = new Size(144, 30);
            label28.TabIndex = 16;
            label28.Text = "This is set to '2' by default\r\nif unchecked.\r\n";
            // 
            // restartIntervalUpDown
            // 
            restartIntervalUpDown.Location = new Point(48, 129);
            restartIntervalUpDown.Margin = new Padding(3, 4, 3, 4);
            restartIntervalUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            restartIntervalUpDown.Name = "restartIntervalUpDown";
            restartIntervalUpDown.Size = new Size(107, 27);
            restartIntervalUpDown.TabIndex = 7;
            restartIntervalUpDown.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // staggeringBudgetLabel
            // 
            staggeringBudgetLabel.AutoSize = true;
            staggeringBudgetLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            staggeringBudgetLabel.Location = new Point(27, 472);
            staggeringBudgetLabel.Name = "staggeringBudgetLabel";
            staggeringBudgetLabel.Size = new Size(137, 20);
            staggeringBudgetLabel.TabIndex = 21;
            staggeringBudgetLabel.Text = "Staggering Budget";
            // 
            // streamsDelta
            // 
            streamsDelta.AutoSize = true;
            streamsDelta.Location = new Point(3, 669);
            streamsDelta.Margin = new Padding(3, 4, 3, 4);
            streamsDelta.Name = "streamsDelta";
            streamsDelta.Size = new Size(18, 17);
            streamsDelta.TabIndex = 27;
            streamsDelta.UseVisualStyleBackColor = true;
            streamsDelta.CheckedChanged += StreamsDeltaCheckChanged;
            // 
            // ndsUpDown
            // 
            ndsUpDown.Location = new Point(3, 319);
            ndsUpDown.Margin = new Padding(3, 4, 3, 4);
            ndsUpDown.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            ndsUpDown.Name = "ndsUpDown";
            ndsUpDown.Size = new Size(227, 27);
            ndsUpDown.TabIndex = 15;
            ndsUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label23.Location = new Point(27, 181);
            label23.Name = "label23";
            label23.Size = new Size(101, 20);
            label23.TabIndex = 10;
            label23.Text = "Override Port";
            // 
            // staggeringBudgetUpDown
            // 
            staggeringBudgetUpDown.Location = new Point(3, 500);
            staggeringBudgetUpDown.Margin = new Padding(3, 4, 3, 4);
            staggeringBudgetUpDown.Maximum = new decimal(new int[] { 10201, 0, 0, 0 });
            staggeringBudgetUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            staggeringBudgetUpDown.Name = "staggeringBudgetUpDown";
            staggeringBudgetUpDown.Size = new Size(227, 27);
            staggeringBudgetUpDown.TabIndex = 22;
            staggeringBudgetUpDown.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label10.Location = new Point(1, 133);
            label10.Name = "label10";
            label10.Size = new Size(48, 20);
            label10.TabIndex = 6;
            label10.Text = "Every";
            // 
            // fpsLimitUpDown
            // 
            fpsLimitUpDown.Location = new Point(3, 32);
            fpsLimitUpDown.Margin = new Padding(3, 4, 3, 4);
            fpsLimitUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            fpsLimitUpDown.Name = "fpsLimitUpDown";
            fpsLimitUpDown.Size = new Size(227, 27);
            fpsLimitUpDown.TabIndex = 2;
            fpsLimitUpDown.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // streamingBudgetUpDown
            // 
            streamingBudgetUpDown.Location = new Point(3, 627);
            streamingBudgetUpDown.Margin = new Padding(3, 4, 3, 4);
            streamingBudgetUpDown.Maximum = new decimal(new int[] { 10201, 0, 0, 0 });
            streamingBudgetUpDown.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            streamingBudgetUpDown.Name = "streamingBudgetUpDown";
            streamingBudgetUpDown.Size = new Size(227, 27);
            streamingBudgetUpDown.TabIndex = 26;
            streamingBudgetUpDown.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // ndsLabel
            // 
            ndsLabel.AutoSize = true;
            ndsLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            ndsLabel.Location = new Point(27, 291);
            ndsLabel.Name = "ndsLabel";
            ndsLabel.Size = new Size(209, 20);
            ndsLabel.TabIndex = 14;
            ndsLabel.Text = "Network Dynamic Simulation";
            // 
            // overridePortNumericUpDown
            // 
            overridePortNumericUpDown.Location = new Point(3, 209);
            overridePortNumericUpDown.Margin = new Padding(3, 4, 3, 4);
            overridePortNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            overridePortNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            overridePortNumericUpDown.Name = "overridePortNumericUpDown";
            overridePortNumericUpDown.Size = new Size(227, 27);
            overridePortNumericUpDown.TabIndex = 11;
            overridePortNumericUpDown.Value = new decimal(new int[] { 2001, 0, 0, 0 });
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold, GraphicsUnit.Point);
            label27.Location = new Point(3, 535);
            label27.Name = "label27";
            label27.Size = new Size(164, 60);
            label27.TabIndex = 23;
            label27.Text = "If not set, uses the \r\nNetwork Dynamic Simulation\r\ndiameter.\r\n\r\n";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(27, 101);
            label6.Name = "label6";
            label6.Size = new Size(154, 20);
            label6.TabIndex = 5;
            label6.Text = "Automatically Restart";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold, GraphicsUnit.Point);
            label21.Location = new Point(3, 67);
            label21.Name = "label21";
            label21.Size = new Size(170, 15);
            label21.TabIndex = 3;
            label21.Text = "Recommended at the moment";
            // 
            // streamingBudgetLabel
            // 
            streamingBudgetLabel.AutoSize = true;
            streamingBudgetLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            streamingBudgetLabel.Location = new Point(27, 599);
            streamingBudgetLabel.Name = "streamingBudgetLabel";
            streamingBudgetLabel.Size = new Size(132, 20);
            streamingBudgetLabel.TabIndex = 25;
            streamingBudgetLabel.Text = "Streaming Budget";
            // 
            // nds
            // 
            nds.AutoSize = true;
            nds.Location = new Point(3, 292);
            nds.Margin = new Padding(3, 4, 3, 4);
            nds.Name = "nds";
            nds.Size = new Size(18, 17);
            nds.TabIndex = 13;
            nds.UseVisualStyleBackColor = true;
            nds.CheckedChanged += NDSCheckChanged;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold, GraphicsUnit.Point);
            label22.Location = new Point(3, 244);
            label22.Name = "label22";
            label22.Size = new Size(152, 30);
            label22.TabIndex = 12;
            label22.Text = "Override the ports specified\r\nin Server Configuration";
            // 
            // streamingBudget
            // 
            streamingBudget.AutoSize = true;
            streamingBudget.Location = new Point(3, 600);
            streamingBudget.Margin = new Padding(3, 4, 3, 4);
            streamingBudget.Name = "streamingBudget";
            streamingBudget.Size = new Size(18, 17);
            streamingBudget.TabIndex = 24;
            streamingBudget.UseVisualStyleBackColor = true;
            streamingBudget.CheckedChanged += StreamingBudgetCheckChanged;
            // 
            // automaticallyRestart
            // 
            automaticallyRestart.AutoSize = true;
            automaticallyRestart.Location = new Point(3, 103);
            automaticallyRestart.Margin = new Padding(3, 4, 3, 4);
            automaticallyRestart.Name = "automaticallyRestart";
            automaticallyRestart.Size = new Size(18, 17);
            automaticallyRestart.TabIndex = 4;
            automaticallyRestart.UseVisualStyleBackColor = true;
            automaticallyRestart.CheckedChanged += AutoRestartCheckedChanged;
            // 
            // startServerBtn
            // 
            startServerBtn.Location = new Point(106, 37);
            startServerBtn.Margin = new Padding(3, 4, 3, 4);
            startServerBtn.Name = "startServerBtn";
            startServerBtn.Size = new Size(99, 31);
            startServerBtn.TabIndex = 4;
            startServerBtn.Text = "Start Server";
            startServerBtn.UseVisualStyleBackColor = true;
            startServerBtn.Click += StartServerBtnPressed;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(steamCmdLog);
            groupBox3.Location = new Point(267, 76);
            groupBox3.Margin = new Padding(3, 4, 3, 4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 4, 3, 4);
            groupBox3.Size = new Size(1461, 883);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Log";
            // 
            // steamCmdLog
            // 
            steamCmdLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            steamCmdLog.Location = new Point(7, 29);
            steamCmdLog.Margin = new Padding(3, 4, 3, 4);
            steamCmdLog.Multiline = true;
            steamCmdLog.Name = "steamCmdLog";
            steamCmdLog.ReadOnly = true;
            steamCmdLog.ScrollBars = ScrollBars.Vertical;
            steamCmdLog.Size = new Size(1446, 844);
            steamCmdLog.TabIndex = 1;
            // 
            // steamCmdAlert
            // 
            steamCmdAlert.AutoSize = true;
            steamCmdAlert.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            steamCmdAlert.Location = new Point(7, 8);
            steamCmdAlert.Name = "steamCmdAlert";
            steamCmdAlert.Size = new Size(727, 23);
            steamCmdAlert.TabIndex = 2;
            steamCmdAlert.Text = "SteamCMD and the Arma Server files were not detected, please Download before continuing.";
            steamCmdAlert.TextAlign = ContentAlignment.MiddleRight;
            // 
            // downloadSteamCmdBtn
            // 
            downloadSteamCmdBtn.Location = new Point(7, 37);
            downloadSteamCmdBtn.Margin = new Padding(3, 4, 3, 4);
            downloadSteamCmdBtn.Name = "downloadSteamCmdBtn";
            downloadSteamCmdBtn.Size = new Size(93, 31);
            downloadSteamCmdBtn.TabIndex = 0;
            downloadSteamCmdBtn.Text = "Download";
            downloadSteamCmdBtn.UseVisualStyleBackColor = true;
            downloadSteamCmdBtn.Click += DownloadSteamCmdBtnPressed;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1771, 1081);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1643, 1070);
            Name = "Main";
            Text = "Arma Reforger Dedicated Server Tool";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            groupBox4.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)streamsDeltaUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)nwkResolutionUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)restartIntervalUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)ndsUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)staggeringBudgetUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)fpsLimitUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)streamingBudgetUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)overridePortNumericUpDown).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button saveSettingsBtn;
        private Button loadSettingsBtn;
        private Label regionLabel;
        private ComboBox region;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button addModBtn;
        private Button removeModBtn;
        private Button removeFromEnabledBtn;
        private Button addToEnabledBtn;
        private Label label16;
        private Label label15;
        private Label steamCmdAlert;
        private TextBox steamCmdLog;
        private Button downloadSteamCmdBtn;
        private GroupBox groupBox3;
        private Button startServerBtn;
        private GroupBox groupBox4;
        private NumericUpDown fpsLimitUpDown;
        private Label label20;
        private CheckBox limitFPS;
        private Label label21;
        private Button disableAllModsBtn;
        private Button enableAllModsBtn;
        private Button aboutBtn;
        private Button deleteServerFilesBtn;
        private Label serverRunningLabel;
        private ComboBox restartUnitsComboBox;
        private NumericUpDown restartIntervalUpDown;
        private Label label10;
        private Label label6;
        private CheckBox automaticallyRestart;
        private Button clearLogBtn;
        private Label label22;
        private NumericUpDown overridePortNumericUpDown;
        private Label label23;
        private CheckBox forcePortCheckBox;
        private PictureBox pictureBox1;
        private Button locateServerFilesBtn;
        private NumericUpDown ndsUpDown;
        private Label ndsLabel;
        private CheckBox nds;
        private Label label28;
        private NumericUpDown nwkResolutionUpDown;
        private Label nwkResolutionLabel;
        private CheckBox nwkResolution;
        private NumericUpDown staggeringBudgetUpDown;
        private Label staggeringBudgetLabel;
        private CheckBox staggeringBudget;
        private Label label27;
        private NumericUpDown streamingBudgetUpDown;
        private Label streamingBudgetLabel;
        private CheckBox streamingBudget;
        private NumericUpDown streamsDeltaUpDown;
        private Label streamsDeltaLabel;
        private CheckBox streamsDelta;
        private Label label30;
        private ComboBox logLevelComboBox;
        private Panel panel1;
        private Button scenarioSelectBtn;
        private Label loadedScenarioLabel;
        private Button editMissionHeaderBtn;
        private TextBox sessionSave;
        private Label loadSessionSaveLabel;
        private CheckBox loadSessionSave;
        private FlowLayoutPanel serverParameters;
        private Button editModBtn;
        private BoundListBox enabledMods;
        private BoundListBox availableMods;
        private TextBox modsSearchTB;
        private CheckBox useExperimentalCheckBox;
    }
}