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
      serverRunningLabel = new Label();
      groupBox2 = new GroupBox();
      tableLayoutPanel2 = new TableLayoutPanel();
      moveModPosDownBtn = new FontAwesome.Sharp.IconButton();
      exportModsBtn = new FontAwesome.Sharp.IconButton();
      moveModPosUpBtn = new FontAwesome.Sharp.IconButton();
      importModsBtn = new FontAwesome.Sharp.IconButton();
      tableLayoutPanel1 = new TableLayoutPanel();
      addModBtn = new FontAwesome.Sharp.IconButton();
      editModBtn = new FontAwesome.Sharp.IconButton();
      removeModBtn = new FontAwesome.Sharp.IconButton();
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
      editMissionHeaderBtn = new FontAwesome.Sharp.IconButton();
      loadedScenarioLabel = new Label();
      scenarioSelectBtn = new FontAwesome.Sharp.IconButton();
      pictureBox1 = new PictureBox();
      saveSettingsBtn = new FontAwesome.Sharp.IconButton();
      loadSettingsBtn = new FontAwesome.Sharp.IconButton();
      tabPage2 = new TabPage();
      useUpnp = new CheckBox();
      useExperimentalCheckBox = new CheckBox();
      label30 = new Label();
      logLevelComboBox = new ComboBox();
      locateServerFilesBtn = new FontAwesome.Sharp.IconButton();
      clearLogBtn = new FontAwesome.Sharp.IconButton();
      deleteServerFilesBtn = new FontAwesome.Sharp.IconButton();
      aboutBtn = new FontAwesome.Sharp.IconButton();
      groupBox4 = new GroupBox();
      advancedParametersPanel = new FlowLayoutPanel();
      startServerBtn = new FontAwesome.Sharp.IconButton();
      groupBox3 = new GroupBox();
      steamCmdLog = new TextBox();
      steamCmdAlert = new Label();
      downloadSteamCmdBtn = new FontAwesome.Sharp.IconButton();
      tabControl1.SuspendLayout();
      tabPage1.SuspendLayout();
      groupBox2.SuspendLayout();
      tableLayoutPanel2.SuspendLayout();
      tableLayoutPanel1.SuspendLayout();
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
      tabPage1.Controls.Add(serverRunningLabel);
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
      // serverRunningLabel
      // 
      serverRunningLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      serverRunningLabel.AutoSize = true;
      serverRunningLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
      serverRunningLabel.Location = new Point(648, 723);
      serverRunningLabel.Name = "serverRunningLabel";
      serverRunningLabel.Size = new Size(128, 17);
      serverRunningLabel.TabIndex = 50;
      serverRunningLabel.Text = "serverRunningLabel";
      // 
      // groupBox2
      // 
      groupBox2.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      groupBox2.Controls.Add(tableLayoutPanel2);
      groupBox2.Controls.Add(tableLayoutPanel1);
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
      groupBox2.Size = new Size(636, 738);
      groupBox2.TabIndex = 47;
      groupBox2.TabStop = false;
      groupBox2.Text = "Mods";
      // 
      // tableLayoutPanel2
      // 
      tableLayoutPanel2.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      tableLayoutPanel2.ColumnCount = 4;
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72F));
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 74F));
      tableLayoutPanel2.Controls.Add(moveModPosDownBtn, 0, 0);
      tableLayoutPanel2.Controls.Add(exportModsBtn, 3, 0);
      tableLayoutPanel2.Controls.Add(moveModPosUpBtn, 1, 0);
      tableLayoutPanel2.Controls.Add(importModsBtn, 2, 0);
      tableLayoutPanel2.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
      tableLayoutPanel2.Location = new Point(335, 701);
      tableLayoutPanel2.Margin = new Padding(0);
      tableLayoutPanel2.Name = "tableLayoutPanel2";
      tableLayoutPanel2.RowCount = 1;
      tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
      tableLayoutPanel2.Size = new Size(291, 33);
      tableLayoutPanel2.TabIndex = 54;
      // 
      // moveModPosDownBtn
      // 
      moveModPosDownBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      moveModPosDownBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowDown;
      moveModPosDownBtn.IconColor = Color.Black;
      moveModPosDownBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      moveModPosDownBtn.IconSize = 16;
      moveModPosDownBtn.Location = new Point(3, 2);
      moveModPosDownBtn.Margin = new Padding(3, 2, 3, 2);
      moveModPosDownBtn.Name = "moveModPosDownBtn";
      moveModPosDownBtn.Size = new Size(66, 29);
      moveModPosDownBtn.TabIndex = 10;
      moveModPosDownBtn.UseVisualStyleBackColor = true;
      moveModPosDownBtn.Click += MoveEnabledModPositionDownBtnPressed;
      // 
      // exportModsBtn
      // 
      exportModsBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      exportModsBtn.IconChar = FontAwesome.Sharp.IconChar.FileExport;
      exportModsBtn.IconColor = Color.Black;
      exportModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      exportModsBtn.IconSize = 16;
      exportModsBtn.Location = new Point(219, 2);
      exportModsBtn.Margin = new Padding(3, 2, 3, 2);
      exportModsBtn.Name = "exportModsBtn";
      exportModsBtn.Size = new Size(69, 29);
      exportModsBtn.TabIndex = 11;
      exportModsBtn.UseVisualStyleBackColor = true;
      exportModsBtn.Click += ExportModsListBtnPressed;
      // 
      // moveModPosUpBtn
      // 
      moveModPosUpBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      moveModPosUpBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowUp;
      moveModPosUpBtn.IconColor = Color.Black;
      moveModPosUpBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      moveModPosUpBtn.IconSize = 16;
      moveModPosUpBtn.Location = new Point(75, 2);
      moveModPosUpBtn.Margin = new Padding(3, 2, 3, 2);
      moveModPosUpBtn.Name = "moveModPosUpBtn";
      moveModPosUpBtn.Size = new Size(66, 29);
      moveModPosUpBtn.TabIndex = 9;
      moveModPosUpBtn.UseVisualStyleBackColor = true;
      moveModPosUpBtn.Click += MoveEnabledModPositionUpBtnPressed;
      // 
      // importModsBtn
      // 
      importModsBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      importModsBtn.IconChar = FontAwesome.Sharp.IconChar.FileImport;
      importModsBtn.IconColor = Color.Black;
      importModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      importModsBtn.IconSize = 16;
      importModsBtn.Location = new Point(147, 2);
      importModsBtn.Margin = new Padding(3, 2, 3, 2);
      importModsBtn.Name = "importModsBtn";
      importModsBtn.Size = new Size(66, 29);
      importModsBtn.TabIndex = 12;
      importModsBtn.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel1
      // 
      tableLayoutPanel1.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      tableLayoutPanel1.ColumnCount = 3;
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 97F));
      tableLayoutPanel1.Controls.Add(addModBtn, 0, 0);
      tableLayoutPanel1.Controls.Add(editModBtn, 1, 0);
      tableLayoutPanel1.Controls.Add(removeModBtn, 2, 0);
      tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
      tableLayoutPanel1.Location = new Point(6, 701);
      tableLayoutPanel1.Margin = new Padding(0);
      tableLayoutPanel1.Name = "tableLayoutPanel1";
      tableLayoutPanel1.RowCount = 1;
      tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 52.63158F));
      tableLayoutPanel1.Size = new Size(294, 33);
      tableLayoutPanel1.TabIndex = 53;
      // 
      // addModBtn
      // 
      addModBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      addModBtn.IconChar = FontAwesome.Sharp.IconChar.Plus;
      addModBtn.IconColor = Color.Black;
      addModBtn.IconFont = FontAwesome.Sharp.IconFont.Solid;
      addModBtn.IconSize = 16;
      addModBtn.Location = new Point(3, 3);
      addModBtn.Name = "addModBtn";
      addModBtn.Size = new Size(92, 27);
      addModBtn.TabIndex = 49;
      addModBtn.UseVisualStyleBackColor = true;
      addModBtn.Click += AddModBtnPressed;
      // 
      // editModBtn
      // 
      editModBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      editModBtn.Enabled = false;
      editModBtn.IconChar = FontAwesome.Sharp.IconChar.Pen;
      editModBtn.IconColor = Color.Black;
      editModBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      editModBtn.IconSize = 16;
      editModBtn.Location = new Point(101, 3);
      editModBtn.Name = "editModBtn";
      editModBtn.Size = new Size(92, 27);
      editModBtn.TabIndex = 51;
      editModBtn.UseVisualStyleBackColor = true;
      editModBtn.Click += EditModBtnPressed;
      // 
      // removeModBtn
      // 
      removeModBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      removeModBtn.Enabled = false;
      removeModBtn.IconChar = FontAwesome.Sharp.IconChar.Minus;
      removeModBtn.IconColor = Color.Black;
      removeModBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      removeModBtn.IconSize = 16;
      removeModBtn.Location = new Point(199, 3);
      removeModBtn.Name = "removeModBtn";
      removeModBtn.Size = new Size(92, 27);
      removeModBtn.TabIndex = 48;
      removeModBtn.UseVisualStyleBackColor = true;
      removeModBtn.Click += RemoveSelectedModBtnPressed;
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
      enabledMods.Size = new Size(291, 619);
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
      editMissionHeaderBtn.IconChar = FontAwesome.Sharp.IconChar.Header;
      editMissionHeaderBtn.IconColor = Color.Black;
      editMissionHeaderBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      editMissionHeaderBtn.IconSize = 16;
      editMissionHeaderBtn.ImageAlign = ContentAlignment.MiddleLeft;
      editMissionHeaderBtn.Location = new Point(134, 650);
      editMissionHeaderBtn.Name = "editMissionHeaderBtn";
      editMissionHeaderBtn.Size = new Size(122, 23);
      editMissionHeaderBtn.TabIndex = 85;
      editMissionHeaderBtn.Text = "Mission Header";
      editMissionHeaderBtn.TextAlign = ContentAlignment.MiddleRight;
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
      scenarioSelectBtn.IconChar = FontAwesome.Sharp.IconChar.Map;
      scenarioSelectBtn.IconColor = Color.Black;
      scenarioSelectBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      scenarioSelectBtn.IconSize = 24;
      scenarioSelectBtn.ImageAlign = ContentAlignment.MiddleLeft;
      scenarioSelectBtn.Location = new Point(6, 650);
      scenarioSelectBtn.Name = "scenarioSelectBtn";
      scenarioSelectBtn.Size = new Size(122, 23);
      scenarioSelectBtn.TabIndex = 82;
      scenarioSelectBtn.Text = "Select Scenario";
      scenarioSelectBtn.TextAlign = ContentAlignment.MiddleRight;
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
      saveSettingsBtn.IconChar = FontAwesome.Sharp.IconChar.Download;
      saveSettingsBtn.IconColor = Color.Black;
      saveSettingsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      saveSettingsBtn.IconSize = 16;
      saveSettingsBtn.ImageAlign = ContentAlignment.MiddleLeft;
      saveSettingsBtn.Location = new Point(1409, 721);
      saveSettingsBtn.Name = "saveSettingsBtn";
      saveSettingsBtn.Size = new Size(96, 23);
      saveSettingsBtn.TabIndex = 2;
      saveSettingsBtn.Text = "Save Config";
      saveSettingsBtn.TextAlign = ContentAlignment.MiddleRight;
      saveSettingsBtn.UseVisualStyleBackColor = true;
      saveSettingsBtn.Click += SaveSettingsToFileBtnPressed;
      // 
      // loadSettingsBtn
      // 
      loadSettingsBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      loadSettingsBtn.IconChar = FontAwesome.Sharp.IconChar.Upload;
      loadSettingsBtn.IconColor = Color.Black;
      loadSettingsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      loadSettingsBtn.IconSize = 16;
      loadSettingsBtn.ImageAlign = ContentAlignment.MiddleLeft;
      loadSettingsBtn.Location = new Point(1307, 721);
      loadSettingsBtn.Name = "loadSettingsBtn";
      loadSettingsBtn.Size = new Size(96, 23);
      loadSettingsBtn.TabIndex = 1;
      loadSettingsBtn.Text = "Load Config";
      loadSettingsBtn.TextAlign = ContentAlignment.MiddleRight;
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
      useExperimentalCheckBox.Size = new Size(151, 19);
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
      locateServerFilesBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      locateServerFilesBtn.IconColor = Color.Black;
      locateServerFilesBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
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
      clearLogBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      clearLogBtn.IconColor = Color.Black;
      clearLogBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
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
      deleteServerFilesBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      deleteServerFilesBtn.IconColor = Color.Black;
      deleteServerFilesBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
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
      startServerBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      startServerBtn.IconColor = Color.Black;
      startServerBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
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
      downloadSteamCmdBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      downloadSteamCmdBtn.IconColor = Color.Black;
      downloadSteamCmdBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
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
      tableLayoutPanel2.ResumeLayout(false);
      tableLayoutPanel1.ResumeLayout(false);
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
        private FontAwesome.Sharp.IconButton saveSettingsBtn;
        private FontAwesome.Sharp.IconButton loadSettingsBtn;
        private Label regionLabel;
        private ComboBox region;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private FontAwesome.Sharp.IconButton addModBtn;
        private FontAwesome.Sharp.IconButton removeModBtn;
        private FontAwesome.Sharp.IconButton removeFromEnabledBtn;
        private FontAwesome.Sharp.IconButton addToEnabledBtn;
        private Label label16;
        private Label label15;
        private Label steamCmdAlert;
        private TextBox steamCmdLog;
        private FontAwesome.Sharp.IconButton downloadSteamCmdBtn;
        private GroupBox groupBox3;
        private FontAwesome.Sharp.IconButton startServerBtn;
        private GroupBox groupBox4;
        private FontAwesome.Sharp.IconButton disableAllModsBtn;
        private FontAwesome.Sharp.IconButton enableAllModsBtn;
        private FontAwesome.Sharp.IconButton aboutBtn;
        private FontAwesome.Sharp.IconButton deleteServerFilesBtn;
        private Label serverRunningLabel;
        private FontAwesome.Sharp.IconButton clearLogBtn;
        private PictureBox pictureBox1;
        private FontAwesome.Sharp.IconButton locateServerFilesBtn;
        private Label label30;
        private ComboBox logLevelComboBox;
        private FontAwesome.Sharp.IconButton scenarioSelectBtn;
        private Label loadedScenarioLabel;
        private FontAwesome.Sharp.IconButton editMissionHeaderBtn;
        private FlowLayoutPanel serverParameters;
        private FontAwesome.Sharp.IconButton editModBtn;
        private BoundListBox enabledMods;
        private BoundListBox availableMods;
        private TextBox modsSearchTB;
        private CheckBox useExperimentalCheckBox;
        private FlowLayoutPanel advancedParametersPanel;
        private CheckBox useUpnp;
        private FontAwesome.Sharp.IconButton moveModPosDownBtn;
        private FontAwesome.Sharp.IconButton moveModPosUpBtn;
        private FontAwesome.Sharp.IconButton exportModsBtn;
        private FontAwesome.Sharp.IconButton importModsBtn;
    private TableLayoutPanel tableLayoutPanel1;
    private TableLayoutPanel tableLayoutPanel2;
  }
}
