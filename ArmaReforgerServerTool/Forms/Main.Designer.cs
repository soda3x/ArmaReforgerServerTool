using ReforgerServerApp.WinForms.Components;

namespace ReforgerServerApp.WinForms
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
            exportModsBtn = new FontAwesome.Sharp.IconButton();
            moveModPosDownBtn = new FontAwesome.Sharp.IconButton();
            moveModPosUpBtn = new FontAwesome.Sharp.IconButton();
            modsSearchTB = new TextBox();
            disableAllModsBtn = new FontAwesome.Sharp.IconButton();
            enableAllModsBtn = new FontAwesome.Sharp.IconButton();
            label16 = new Label();
            label15 = new Label();
            removeFromEnabledBtn = new FontAwesome.Sharp.IconButton();
            addToEnabledBtn = new FontAwesome.Sharp.IconButton();
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
            useUpnp = new CheckBox();
            useExperimentalCheckBox = new CheckBox();
            label30 = new Label();
            logLevelComboBox = new ComboBox();
            locateServerFilesBtn = new Button();
            clearLogBtn = new Button();
            deleteServerFilesBtn = new Button();
            aboutBtn = new FontAwesome.Sharp.IconButton();
            groupBox4 = new GroupBox();
            advancedParametersPanel = new FlowLayoutPanel();
            startServerBtn = new Button();
            groupBox3 = new GroupBox();
            steamCmdLog = new TextBox();
            steamCmdAlert = new Label();
            downloadSteamCmdBtn = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) pictureBox1).BeginInit();
            tabPage2.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1518, 750);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Server Configuration";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // editModBtn
            // 
            editModBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            editModBtn.Enabled = false;
            editModBtn.Location = new Point(85, 721);
            editModBtn.Name = "editModBtn";
            editModBtn.Size = new Size(132, 23);
            editModBtn.TabIndex = 51;
            editModBtn.Text = "Edit Selected Mod";
            editModBtn.UseVisualStyleBackColor = true;
            editModBtn.Click += EditModBtnPressed;
            // 
            // serverRunningLabel
            // 
            serverRunningLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            serverRunningLabel.AutoSize = true;
            serverRunningLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            serverRunningLabel.Location = new Point(504, 723);
            serverRunningLabel.Name = "serverRunningLabel";
            serverRunningLabel.Size = new Size(128, 17);
            serverRunningLabel.TabIndex = 50;
            serverRunningLabel.Text = "serverRunningLabel";
            // 
            // addModBtn
            // 
            addModBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            addModBtn.Location = new Point(6, 721);
            addModBtn.Name = "addModBtn";
            addModBtn.Size = new Size(73, 23);
            addModBtn.TabIndex = 49;
            addModBtn.Text = "Add Mod";
            addModBtn.UseVisualStyleBackColor = true;
            addModBtn.Click += AddModBtnPressed;
            // 
            // removeModBtn
            // 
            removeModBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            removeModBtn.Enabled = false;
            removeModBtn.Location = new Point(223, 721);
            removeModBtn.Name = "removeModBtn";
            removeModBtn.Size = new Size(146, 23);
            removeModBtn.TabIndex = 48;
            removeModBtn.Text = "Remove Selected Mod";
            removeModBtn.UseVisualStyleBackColor = true;
            removeModBtn.Click += RemoveSelectedModBtnPressed;
            // 
            // groupBox2
            // 
            groupBox2.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox2.Controls.Add(exportModsBtn);
            groupBox2.Controls.Add(moveModPosDownBtn);
            groupBox2.Controls.Add(moveModPosUpBtn);
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
            groupBox2.Location = new Point(6, 6);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(636, 709);
            groupBox2.TabIndex = 47;
            groupBox2.TabStop = false;
            groupBox2.Text = "Mods";
            // 
            // exportModsBtn
            // 
            exportModsBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            exportModsBtn.IconChar = FontAwesome.Sharp.IconChar.FileExport;
            exportModsBtn.IconColor = Color.Black;
            exportModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            exportModsBtn.IconSize = 16;
            exportModsBtn.Location = new Point(557, 671);
            exportModsBtn.Margin = new Padding(3, 2, 3, 2);
            exportModsBtn.Name = "exportModsBtn";
            exportModsBtn.Size = new Size(69, 22);
            exportModsBtn.TabIndex = 11;
            exportModsBtn.UseVisualStyleBackColor = true;
            exportModsBtn.Click += ExportModsListBtnPressed;
            // 
            // moveModPosDownBtn
            // 
            moveModPosDownBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            moveModPosDownBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowDown;
            moveModPosDownBtn.IconColor = Color.Black;
            moveModPosDownBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            moveModPosDownBtn.IconSize = 16;
            moveModPosDownBtn.Location = new Point(335, 670);
            moveModPosDownBtn.Margin = new Padding(3, 2, 3, 2);
            moveModPosDownBtn.Name = "moveModPosDownBtn";
            moveModPosDownBtn.Size = new Size(105, 23);
            moveModPosDownBtn.TabIndex = 10;
            moveModPosDownBtn.UseVisualStyleBackColor = true;
            moveModPosDownBtn.Click += MoveEnabledModPositionDownBtnPressed;
            // 
            // moveModPosUpBtn
            // 
            moveModPosUpBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            moveModPosUpBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowUp;
            moveModPosUpBtn.IconColor = Color.Black;
            moveModPosUpBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            moveModPosUpBtn.IconSize = 16;
            moveModPosUpBtn.Location = new Point(446, 671);
            moveModPosUpBtn.Margin = new Padding(3, 2, 3, 2);
            moveModPosUpBtn.Name = "moveModPosUpBtn";
            moveModPosUpBtn.Size = new Size(105, 22);
            moveModPosUpBtn.TabIndex = 9;
            moveModPosUpBtn.UseVisualStyleBackColor = true;
            moveModPosUpBtn.Click += MoveEnabledModPositionUpBtnPressed;
            // 
            // modsSearchTB
            // 
            modsSearchTB.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            modsSearchTB.Location = new Point(6, 40);
            modsSearchTB.Name = "modsSearchTB";
            modsSearchTB.PlaceholderText = "Search Mods...";
            modsSearchTB.Size = new Size(620, 23);
            modsSearchTB.TabIndex = 8;
            modsSearchTB.TextChanged += OnSearchModsTextChanged;
            // 
            // disableAllModsBtn
            // 
            disableAllModsBtn.IconChar = FontAwesome.Sharp.IconChar.AngleDoubleLeft;
            disableAllModsBtn.IconColor = Color.Black;
            disableAllModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            disableAllModsBtn.IconSize = 16;
            disableAllModsBtn.Location = new Point(306, 451);
            disableAllModsBtn.Name = "disableAllModsBtn";
            disableAllModsBtn.Size = new Size(23, 52);
            disableAllModsBtn.TabIndex = 7;
            disableAllModsBtn.UseVisualStyleBackColor = true;
            disableAllModsBtn.Click += DisableAllModsBtnPressed;
            // 
            // enableAllModsBtn
            // 
            enableAllModsBtn.IconChar = FontAwesome.Sharp.IconChar.AngleDoubleRight;
            enableAllModsBtn.IconColor = Color.Black;
            enableAllModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            enableAllModsBtn.IconSize = 16;
            enableAllModsBtn.Location = new Point(306, 240);
            enableAllModsBtn.Name = "enableAllModsBtn";
            enableAllModsBtn.Size = new Size(23, 52);
            enableAllModsBtn.TabIndex = 6;
            enableAllModsBtn.UseVisualStyleBackColor = true;
            enableAllModsBtn.Click += EnableAllModsBtnPressed;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(335, 19);
            label16.Name = "label16";
            label16.Size = new Size(82, 15);
            label16.TabIndex = 5;
            label16.Text = "Enabled Mods";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(6, 19);
            label15.Name = "label15";
            label15.Size = new Size(88, 15);
            label15.TabIndex = 4;
            label15.Text = "Available Mods";
            // 
            // removeFromEnabledBtn
            // 
            removeFromEnabledBtn.IconChar = FontAwesome.Sharp.IconChar.AngleLeft;
            removeFromEnabledBtn.IconColor = Color.Black;
            removeFromEnabledBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            removeFromEnabledBtn.IconSize = 16;
            removeFromEnabledBtn.Location = new Point(306, 380);
            removeFromEnabledBtn.Name = "removeFromEnabledBtn";
            removeFromEnabledBtn.Size = new Size(23, 52);
            removeFromEnabledBtn.TabIndex = 3;
            removeFromEnabledBtn.UseVisualStyleBackColor = true;
            removeFromEnabledBtn.Click += RemovedFromEnabledModsBtnPressed;
            // 
            // addToEnabledBtn
            // 
            addToEnabledBtn.IconChar = FontAwesome.Sharp.IconChar.AngleRight;
            addToEnabledBtn.IconColor = Color.Black;
            addToEnabledBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            addToEnabledBtn.IconSize = 16;
            addToEnabledBtn.Location = new Point(306, 310);
            addToEnabledBtn.Name = "addToEnabledBtn";
            addToEnabledBtn.Size = new Size(23, 52);
            addToEnabledBtn.TabIndex = 2;
            addToEnabledBtn.UseVisualStyleBackColor = true;
            addToEnabledBtn.Click += AddToEnabledModsBtnPressed;
            // 
            // enabledMods
            // 
            enabledMods.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            enabledMods.FormattingEnabled = true;
            enabledMods.ItemHeight = 15;
            enabledMods.Location = new Point(335, 74);
            enabledMods.Name = "enabledMods";
            enabledMods.SelectionMode = SelectionMode.MultiExtended;
            enabledMods.Size = new Size(291, 589);
            enabledMods.TabIndex = 1;
            // 
            // availableMods
            // 
            availableMods.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            availableMods.FormattingEnabled = true;
            availableMods.ItemHeight = 15;
            availableMods.Location = new Point(6, 74);
            availableMods.Name = "availableMods";
            availableMods.SelectionMode = SelectionMode.MultiExtended;
            availableMods.Size = new Size(294, 619);
            availableMods.TabIndex = 0;
            availableMods.SelectedIndexChanged += AvailableModsSelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(serverParameters);
            groupBox1.Controls.Add(editMissionHeaderBtn);
            groupBox1.Controls.Add(loadedScenarioLabel);
            groupBox1.Controls.Add(scenarioSelectBtn);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox1.Location = new Point(648, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(857, 709);
            groupBox1.TabIndex = 46;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server Settings";
            // 
            // serverParameters
            // 
            serverParameters.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            serverParameters.AutoScroll = true;
            serverParameters.FlowDirection = FlowDirection.TopDown;
            serverParameters.Location = new Point(6, 221);
            serverParameters.Margin = new Padding(15, 0, 15, 0);
            serverParameters.Name = "serverParameters";
            serverParameters.Size = new Size(845, 423);
            serverParameters.TabIndex = 90;
            // 
            // editMissionHeaderBtn
            // 
            editMissionHeaderBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            editMissionHeaderBtn.Location = new Point(189, 650);
            editMissionHeaderBtn.Name = "editMissionHeaderBtn";
            editMissionHeaderBtn.Size = new Size(177, 23);
            editMissionHeaderBtn.TabIndex = 85;
            editMissionHeaderBtn.Text = "Edit Mission Header";
            editMissionHeaderBtn.UseVisualStyleBackColor = true;
            editMissionHeaderBtn.Click += EditMissionHeaderBtnClicked;
            // 
            // loadedScenarioLabel
            // 
            loadedScenarioLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            loadedScenarioLabel.AutoEllipsis = true;
            loadedScenarioLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            loadedScenarioLabel.Location = new Point(6, 676);
            loadedScenarioLabel.Name = "loadedScenarioLabel";
            loadedScenarioLabel.Size = new Size(842, 21);
            loadedScenarioLabel.TabIndex = 83;
            loadedScenarioLabel.Text = "Scenario ID";
            loadedScenarioLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // scenarioSelectBtn
            // 
            scenarioSelectBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            scenarioSelectBtn.Location = new Point(6, 650);
            scenarioSelectBtn.Name = "scenarioSelectBtn";
            scenarioSelectBtn.Size = new Size(177, 23);
            scenarioSelectBtn.TabIndex = 82;
            scenarioSelectBtn.Text = "Select a Scenario";
            scenarioSelectBtn.UseVisualStyleBackColor = true;
            scenarioSelectBtn.Click += ScenarioSelectBtnClicked;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Image = (Image) resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(107, 19);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(631, 199);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 51;
            pictureBox1.TabStop = false;
            // 
            // saveSettingsBtn
            // 
            saveSettingsBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
            saveSettingsBtn.Location = new Point(1352, 721);
            saveSettingsBtn.Name = "saveSettingsBtn";
            saveSettingsBtn.Size = new Size(153, 23);
            saveSettingsBtn.TabIndex = 2;
            saveSettingsBtn.Text = "Save Settings to File";
            saveSettingsBtn.UseVisualStyleBackColor = true;
            saveSettingsBtn.Click += SaveSettingsToFileBtnPressed;
            // 
            // loadSettingsBtn
            // 
            loadSettingsBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
            loadSettingsBtn.Location = new Point(1193, 721);
            loadSettingsBtn.Name = "loadSettingsBtn";
            loadSettingsBtn.Size = new Size(153, 23);
            loadSettingsBtn.TabIndex = 1;
            loadSettingsBtn.Text = "Load Settings from File";
            loadSettingsBtn.UseVisualStyleBackColor = true;
            loadSettingsBtn.Click += LoadSettingsFromFileBtnPressed;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(useUpnp);
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
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1518, 750);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Server Management";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // useUpnp
            // 
            useUpnp.AutoSize = true;
            useUpnp.Checked = true;
            useUpnp.CheckState = CheckState.Checked;
            useUpnp.Location = new Point(352, 31);
            useUpnp.Margin = new Padding(3, 2, 3, 2);
            useUpnp.Name = "useUpnp";
            useUpnp.Size = new Size(77, 19);
            useUpnp.TabIndex = 32;
            useUpnp.Text = "Use UPnP";
            useUpnp.UseVisualStyleBackColor = true;
            useUpnp.CheckedChanged += OnUseUPnPCheckChanged;
            // 
            // useExperimentalCheckBox
            // 
            useExperimentalCheckBox.AutoSize = true;
            useExperimentalCheckBox.Location = new Point(194, 31);
            useExperimentalCheckBox.Name = "useExperimentalCheckBox";
            useExperimentalCheckBox.Size = new Size(152, 19);
            useExperimentalCheckBox.TabIndex = 31;
            useExperimentalCheckBox.Text = "Use Experimental Server";
            useExperimentalCheckBox.UseVisualStyleBackColor = true;
            useExperimentalCheckBox.CheckedChanged += UseExperimentalServerCheckboxChanged;
            // 
            // label30
            // 
            label30.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            label30.AutoSize = true;
            label30.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label30.Location = new Point(252, 725);
            label30.Name = "label30";
            label30.Size = new Size(57, 15);
            label30.TabIndex = 30;
            label30.Text = "Log Level";
            // 
            // logLevelComboBox
            // 
            logLevelComboBox.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
            logLevelComboBox.FormattingEnabled = true;
            logLevelComboBox.Items.AddRange(new object[] { "normal", "warning", "error", "fatal" });
            logLevelComboBox.Location = new Point(310, 722);
            logLevelComboBox.Name = "logLevelComboBox";
            logLevelComboBox.Size = new Size(96, 23);
            logLevelComboBox.TabIndex = 30;
            logLevelComboBox.Text = "normal";
            // 
            // locateServerFilesBtn
            // 
            locateServerFilesBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
            locateServerFilesBtn.Location = new Point(1222, 23);
            locateServerFilesBtn.Name = "locateServerFilesBtn";
            locateServerFilesBtn.Size = new Size(111, 23);
            locateServerFilesBtn.TabIndex = 7;
            locateServerFilesBtn.Text = "Locate Server Files";
            locateServerFilesBtn.UseVisualStyleBackColor = true;
            locateServerFilesBtn.Click += LocateServerFilesBtnPressed;
            // 
            // clearLogBtn
            // 
            clearLogBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
            clearLogBtn.Location = new Point(1446, 722);
            clearLogBtn.Name = "clearLogBtn";
            clearLogBtn.Size = new Size(66, 23);
            clearLogBtn.TabIndex = 6;
            clearLogBtn.Text = "Clear Log";
            clearLogBtn.UseVisualStyleBackColor = true;
            clearLogBtn.Click += ClearLogBtnPressed;
            // 
            // deleteServerFilesBtn
            // 
            deleteServerFilesBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
            deleteServerFilesBtn.Location = new Point(1339, 23);
            deleteServerFilesBtn.Name = "deleteServerFilesBtn";
            deleteServerFilesBtn.Size = new Size(111, 23);
            deleteServerFilesBtn.TabIndex = 5;
            deleteServerFilesBtn.Text = "Delete Server Files";
            deleteServerFilesBtn.UseVisualStyleBackColor = true;
            deleteServerFilesBtn.Click += DeleteServerFilesBtnPressed;
            // 
            // aboutBtn
            // 
            aboutBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
            aboutBtn.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
            aboutBtn.IconColor = Color.Black;
            aboutBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            aboutBtn.IconSize = 18;
            aboutBtn.Location = new Point(1456, 23);
            aboutBtn.Name = "aboutBtn";
            aboutBtn.Size = new Size(56, 23);
            aboutBtn.TabIndex = 4;
            aboutBtn.UseVisualStyleBackColor = true;
            aboutBtn.Click += AboutBtnPressed;
            // 
            // groupBox4
            // 
            groupBox4.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox4.Controls.Add(advancedParametersPanel);
            groupBox4.Location = new Point(5, 57);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(242, 688);
            groupBox4.TabIndex = 4;
            groupBox4.TabStop = false;
            groupBox4.Text = "Advanced";
            // 
            // advancedParametersPanel
            // 
            advancedParametersPanel.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            advancedParametersPanel.AutoScroll = true;
            advancedParametersPanel.FlowDirection = FlowDirection.TopDown;
            advancedParametersPanel.Location = new Point(5, 16);
            advancedParametersPanel.Margin = new Padding(3, 2, 3, 2);
            advancedParametersPanel.Name = "advancedParametersPanel";
            advancedParametersPanel.Size = new Size(231, 668);
            advancedParametersPanel.TabIndex = 0;
            advancedParametersPanel.WrapContents = false;
            // 
            // startServerBtn
            // 
            startServerBtn.Location = new Point(93, 28);
            startServerBtn.Name = "startServerBtn";
            startServerBtn.Size = new Size(87, 23);
            startServerBtn.TabIndex = 4;
            startServerBtn.Text = "Start Server";
            startServerBtn.UseVisualStyleBackColor = true;
            startServerBtn.Click += StartServerBtnPressed;
            // 
            // groupBox3
            // 
            groupBox3.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(steamCmdLog);
            groupBox3.Location = new Point(253, 57);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1259, 659);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Log";
            // 
            // steamCmdLog
            // 
            steamCmdLog.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            steamCmdLog.Location = new Point(6, 22);
            steamCmdLog.Multiline = true;
            steamCmdLog.Name = "steamCmdLog";
            steamCmdLog.ReadOnly = true;
            steamCmdLog.ScrollBars = ScrollBars.Vertical;
            steamCmdLog.Size = new Size(1249, 631);
            steamCmdLog.TabIndex = 1;
            // 
            // steamCmdAlert
            // 
            steamCmdAlert.AutoSize = true;
            steamCmdAlert.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            steamCmdAlert.Location = new Point(6, 6);
            steamCmdAlert.Name = "steamCmdAlert";
            steamCmdAlert.Size = new Size(573, 17);
            steamCmdAlert.TabIndex = 2;
            steamCmdAlert.Text = "SteamCMD and the Arma Server files were not detected, please Download before continuing.";
            steamCmdAlert.TextAlign = ContentAlignment.MiddleRight;
            // 
            // downloadSteamCmdBtn
            // 
            downloadSteamCmdBtn.Location = new Point(6, 28);
            downloadSteamCmdBtn.Name = "downloadSteamCmdBtn";
            downloadSteamCmdBtn.Size = new Size(81, 23);
            downloadSteamCmdBtn.TabIndex = 0;
            downloadSteamCmdBtn.Text = "Download";
            downloadSteamCmdBtn.UseVisualStyleBackColor = true;
            downloadSteamCmdBtn.Click += DownloadSteamCmdBtnPressed;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1550, 796);
            Controls.Add(tabControl1);
            Icon = (Icon) resources.GetObject("$this.Icon");
            MinimumSize = new Size(1440, 782);
            Name = "Main";
            Text = "Arma Reforger Dedicated Server Tool";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) pictureBox1).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            groupBox4.ResumeLayout(false);
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
        private FontAwesome.Sharp.IconButton removeFromEnabledBtn;
        private FontAwesome.Sharp.IconButton addToEnabledBtn;
        private Label label16;
        private Label label15;
        private Label steamCmdAlert;
        private TextBox steamCmdLog;
        private Button downloadSteamCmdBtn;
        private GroupBox groupBox3;
        private Button startServerBtn;
        private GroupBox groupBox4;
        private FontAwesome.Sharp.IconButton disableAllModsBtn;
        private FontAwesome.Sharp.IconButton enableAllModsBtn;
        private FontAwesome.Sharp.IconButton aboutBtn;
        private Button deleteServerFilesBtn;
        private Label serverRunningLabel;
        private Button clearLogBtn;
        private PictureBox pictureBox1;
        private Button locateServerFilesBtn;
        private Label label30;
        private ComboBox logLevelComboBox;
        private Button scenarioSelectBtn;
        private Label loadedScenarioLabel;
        private Button editMissionHeaderBtn;
        private FlowLayoutPanel serverParameters;
        private Button editModBtn;
        private BoundListBox enabledMods;
        private BoundListBox availableMods;
        private TextBox modsSearchTB;
        private CheckBox useExperimentalCheckBox;
        private FlowLayoutPanel advancedParametersPanel;
        private CheckBox useUpnp;
        private FontAwesome.Sharp.IconButton moveModPosDownBtn;
        private FontAwesome.Sharp.IconButton moveModPosUpBtn;
        private FontAwesome.Sharp.IconButton exportModsBtn;
    }
}