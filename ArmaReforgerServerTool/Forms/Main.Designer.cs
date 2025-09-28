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
      advancedParametersPanel = new FlowLayoutPanel();
      tableLayoutPanel5 = new TableLayoutPanel();
      scenarioSelectBtn = new FontAwesome.Sharp.IconButton();
      editMissionHeaderBtn = new FontAwesome.Sharp.IconButton();
      loadedScenarioLabel = new Label();
      saveSettingsBtn = new FontAwesome.Sharp.IconButton();
      loadSettingsBtn = new FontAwesome.Sharp.IconButton();
      serverRunningLabel = new Label();
      groupBox2 = new GroupBox();
      tableLayoutPanel4 = new TableLayoutPanel();
      enableAllModsBtn = new FontAwesome.Sharp.IconButton();
      addToEnabledBtn = new FontAwesome.Sharp.IconButton();
      removeFromEnabledBtn = new FontAwesome.Sharp.IconButton();
      disableAllModsBtn = new FontAwesome.Sharp.IconButton();
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
      label16 = new Label();
      label15 = new Label();
      enabledMods = new BoundListBox();
      availableMods = new BoundListBox();
      groupBox1 = new GroupBox();
      groupBox6 = new GroupBox();
      serverParameters = new FlowLayoutPanel();
      groupBox5 = new GroupBox();
      tableLayoutPanel6 = new TableLayoutPanel();
      aboutBtn = new FontAwesome.Sharp.IconButton();
      pictureBox1 = new PictureBox();
      useUpnp = new CheckBox();
      useExperimentalCheckBox = new CheckBox();
      label30 = new Label();
      logLevelComboBox = new ComboBox();
      locateServerFilesBtn = new FontAwesome.Sharp.IconButton();
      clearLogBtn = new FontAwesome.Sharp.IconButton();
      deleteServerFilesBtn = new FontAwesome.Sharp.IconButton();
      startServerBtn = new FontAwesome.Sharp.IconButton();
      groupBox3 = new GroupBox();
      steamCmdLog = new TextBox();
      steamCmdAlert = new Label();
      downloadSteamCmdBtn = new FontAwesome.Sharp.IconButton();
      splitContainer1 = new SplitContainer();
      groupBox4 = new GroupBox();
      tableLayoutPanel3 = new TableLayoutPanel();
      tableLayoutPanel5.SuspendLayout();
      groupBox2.SuspendLayout();
      tableLayoutPanel4.SuspendLayout();
      tableLayoutPanel2.SuspendLayout();
      tableLayoutPanel1.SuspendLayout();
      groupBox1.SuspendLayout();
      groupBox6.SuspendLayout();
      groupBox5.SuspendLayout();
      tableLayoutPanel6.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize) pictureBox1).BeginInit();
      groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      groupBox4.SuspendLayout();
      tableLayoutPanel3.SuspendLayout();
      SuspendLayout();
      // 
      // advancedParametersPanel
      // 
      advancedParametersPanel.AutoScroll = true;
      advancedParametersPanel.Dock = DockStyle.Fill;
      advancedParametersPanel.FlowDirection = FlowDirection.TopDown;
      advancedParametersPanel.Location = new Point(3, 19);
      advancedParametersPanel.Margin = new Padding(15, 0, 15, 0);
      advancedParametersPanel.Name = "advancedParametersPanel";
      advancedParametersPanel.Size = new Size(223, 262);
      advancedParametersPanel.TabIndex = 0;
      advancedParametersPanel.WrapContents = false;
      // 
      // tableLayoutPanel5
      // 
      tableLayoutPanel5.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      tableLayoutPanel5.ColumnCount = 3;
      tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.8796673F));
      tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.8796673F));
      tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62.2406769F));
      tableLayoutPanel5.Controls.Add(scenarioSelectBtn, 0, 0);
      tableLayoutPanel5.Controls.Add(editMissionHeaderBtn, 1, 0);
      tableLayoutPanel5.Controls.Add(loadedScenarioLabel, 2, 0);
      tableLayoutPanel5.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
      tableLayoutPanel5.Location = new Point(3, 511);
      tableLayoutPanel5.Margin = new Padding(0);
      tableLayoutPanel5.Name = "tableLayoutPanel5";
      tableLayoutPanel5.RowCount = 1;
      tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      tableLayoutPanel5.Size = new Size(676, 23);
      tableLayoutPanel5.TabIndex = 91;
      // 
      // scenarioSelectBtn
      // 
      scenarioSelectBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      scenarioSelectBtn.IconChar = FontAwesome.Sharp.IconChar.Map;
      scenarioSelectBtn.IconColor = Color.Black;
      scenarioSelectBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      scenarioSelectBtn.IconSize = 24;
      scenarioSelectBtn.ImageAlign = ContentAlignment.MiddleLeft;
      scenarioSelectBtn.Location = new Point(0, 0);
      scenarioSelectBtn.Margin = new Padding(0);
      scenarioSelectBtn.Name = "scenarioSelectBtn";
      scenarioSelectBtn.Size = new Size(127, 23);
      scenarioSelectBtn.TabIndex = 82;
      scenarioSelectBtn.Text = "Select Scenario";
      scenarioSelectBtn.TextAlign = ContentAlignment.MiddleRight;
      scenarioSelectBtn.UseVisualStyleBackColor = true;
      scenarioSelectBtn.Click += ScenarioSelectBtnClicked;
      // 
      // editMissionHeaderBtn
      // 
      editMissionHeaderBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      editMissionHeaderBtn.IconChar = FontAwesome.Sharp.IconChar.Header;
      editMissionHeaderBtn.IconColor = Color.Black;
      editMissionHeaderBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      editMissionHeaderBtn.IconSize = 16;
      editMissionHeaderBtn.ImageAlign = ContentAlignment.MiddleLeft;
      editMissionHeaderBtn.Location = new Point(127, 0);
      editMissionHeaderBtn.Margin = new Padding(0);
      editMissionHeaderBtn.Name = "editMissionHeaderBtn";
      editMissionHeaderBtn.Size = new Size(127, 23);
      editMissionHeaderBtn.TabIndex = 85;
      editMissionHeaderBtn.Text = "Mission Header";
      editMissionHeaderBtn.TextAlign = ContentAlignment.MiddleRight;
      editMissionHeaderBtn.UseVisualStyleBackColor = true;
      editMissionHeaderBtn.Click += EditMissionHeaderBtnClicked;
      // 
      // loadedScenarioLabel
      // 
      loadedScenarioLabel.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      loadedScenarioLabel.AutoEllipsis = true;
      loadedScenarioLabel.Font = new Font("Segoe UI", 9.75F);
      loadedScenarioLabel.Location = new Point(264, 0);
      loadedScenarioLabel.Margin = new Padding(10, 0, 0, 0);
      loadedScenarioLabel.Name = "loadedScenarioLabel";
      loadedScenarioLabel.Size = new Size(412, 23);
      loadedScenarioLabel.TabIndex = 83;
      loadedScenarioLabel.Text = "Scenario ID";
      loadedScenarioLabel.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // saveSettingsBtn
      // 
      saveSettingsBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      saveSettingsBtn.IconChar = FontAwesome.Sharp.IconChar.Download;
      saveSettingsBtn.IconColor = Color.Black;
      saveSettingsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      saveSettingsBtn.IconSize = 16;
      saveSettingsBtn.ImageAlign = ContentAlignment.MiddleLeft;
      saveSettingsBtn.Location = new Point(99, 0);
      saveSettingsBtn.Margin = new Padding(0);
      saveSettingsBtn.Name = "saveSettingsBtn";
      saveSettingsBtn.Size = new Size(100, 23);
      saveSettingsBtn.TabIndex = 2;
      saveSettingsBtn.Text = "Save Config";
      saveSettingsBtn.TextAlign = ContentAlignment.MiddleRight;
      saveSettingsBtn.UseVisualStyleBackColor = true;
      saveSettingsBtn.Click += SaveSettingsToFileBtnPressed;
      // 
      // loadSettingsBtn
      // 
      loadSettingsBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      loadSettingsBtn.IconChar = FontAwesome.Sharp.IconChar.Upload;
      loadSettingsBtn.IconColor = Color.Black;
      loadSettingsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      loadSettingsBtn.IconSize = 16;
      loadSettingsBtn.ImageAlign = ContentAlignment.MiddleLeft;
      loadSettingsBtn.Location = new Point(0, 0);
      loadSettingsBtn.Margin = new Padding(0);
      loadSettingsBtn.Name = "loadSettingsBtn";
      loadSettingsBtn.Size = new Size(99, 23);
      loadSettingsBtn.TabIndex = 1;
      loadSettingsBtn.Text = "Load Config";
      loadSettingsBtn.TextAlign = ContentAlignment.MiddleRight;
      loadSettingsBtn.UseVisualStyleBackColor = true;
      loadSettingsBtn.Click += LoadSettingsFromFileBtnPressed;
      // 
      // serverRunningLabel
      // 
      serverRunningLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      serverRunningLabel.AutoSize = true;
      serverRunningLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
      serverRunningLabel.Location = new Point(303, 193);
      serverRunningLabel.Name = "serverRunningLabel";
      serverRunningLabel.Size = new Size(128, 17);
      serverRunningLabel.TabIndex = 50;
      serverRunningLabel.Text = "serverRunningLabel";
      // 
      // groupBox2
      // 
      groupBox2.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      groupBox2.Controls.Add(tableLayoutPanel4);
      groupBox2.Controls.Add(tableLayoutPanel2);
      groupBox2.Controls.Add(tableLayoutPanel1);
      groupBox2.Controls.Add(modsSearchTB);
      groupBox2.Controls.Add(label16);
      groupBox2.Controls.Add(label15);
      groupBox2.Controls.Add(enabledMods);
      groupBox2.Controls.Add(availableMods);
      groupBox2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
      groupBox2.Location = new Point(11, 11);
      groupBox2.Name = "groupBox2";
      groupBox2.Size = new Size(636, 538);
      groupBox2.TabIndex = 47;
      groupBox2.TabStop = false;
      groupBox2.Text = "Mods";
      // 
      // tableLayoutPanel4
      // 
      tableLayoutPanel4.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tableLayoutPanel4.ColumnCount = 1;
      tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
      tableLayoutPanel4.Controls.Add(enableAllModsBtn, 0, 1);
      tableLayoutPanel4.Controls.Add(addToEnabledBtn, 0, 2);
      tableLayoutPanel4.Controls.Add(removeFromEnabledBtn, 0, 3);
      tableLayoutPanel4.Controls.Add(disableAllModsBtn, 0, 4);
      tableLayoutPanel4.Location = new Point(303, 74);
      tableLayoutPanel4.Name = "tableLayoutPanel4";
      tableLayoutPanel4.RowCount = 6;
      tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
      tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
      tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
      tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
      tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
      tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
      tableLayoutPanel4.Size = new Size(29, 460);
      tableLayoutPanel4.TabIndex = 55;
      // 
      // enableAllModsBtn
      // 
      enableAllModsBtn.Dock = DockStyle.Fill;
      enableAllModsBtn.IconChar = FontAwesome.Sharp.IconChar.AngleDoubleRight;
      enableAllModsBtn.IconColor = Color.Black;
      enableAllModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      enableAllModsBtn.IconSize = 16;
      enableAllModsBtn.Location = new Point(3, 79);
      enableAllModsBtn.Name = "enableAllModsBtn";
      enableAllModsBtn.Size = new Size(23, 70);
      enableAllModsBtn.TabIndex = 6;
      enableAllModsBtn.UseVisualStyleBackColor = true;
      enableAllModsBtn.Click += EnableAllModsBtnPressed;
      // 
      // addToEnabledBtn
      // 
      addToEnabledBtn.Dock = DockStyle.Fill;
      addToEnabledBtn.IconChar = FontAwesome.Sharp.IconChar.AngleRight;
      addToEnabledBtn.IconColor = Color.Black;
      addToEnabledBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      addToEnabledBtn.IconSize = 16;
      addToEnabledBtn.Location = new Point(3, 155);
      addToEnabledBtn.Name = "addToEnabledBtn";
      addToEnabledBtn.Size = new Size(23, 70);
      addToEnabledBtn.TabIndex = 2;
      addToEnabledBtn.UseVisualStyleBackColor = true;
      addToEnabledBtn.Click += AddToEnabledModsBtnPressed;
      // 
      // removeFromEnabledBtn
      // 
      removeFromEnabledBtn.Dock = DockStyle.Fill;
      removeFromEnabledBtn.IconChar = FontAwesome.Sharp.IconChar.AngleLeft;
      removeFromEnabledBtn.IconColor = Color.Black;
      removeFromEnabledBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      removeFromEnabledBtn.IconSize = 16;
      removeFromEnabledBtn.Location = new Point(3, 231);
      removeFromEnabledBtn.Name = "removeFromEnabledBtn";
      removeFromEnabledBtn.Size = new Size(23, 70);
      removeFromEnabledBtn.TabIndex = 3;
      removeFromEnabledBtn.UseVisualStyleBackColor = true;
      removeFromEnabledBtn.Click += RemovedFromEnabledModsBtnPressed;
      // 
      // disableAllModsBtn
      // 
      disableAllModsBtn.Dock = DockStyle.Fill;
      disableAllModsBtn.IconChar = FontAwesome.Sharp.IconChar.AngleDoubleLeft;
      disableAllModsBtn.IconColor = Color.Black;
      disableAllModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      disableAllModsBtn.IconSize = 16;
      disableAllModsBtn.Location = new Point(3, 307);
      disableAllModsBtn.Name = "disableAllModsBtn";
      disableAllModsBtn.Size = new Size(23, 70);
      disableAllModsBtn.TabIndex = 7;
      disableAllModsBtn.UseVisualStyleBackColor = true;
      disableAllModsBtn.Click += DisableAllModsBtnPressed;
      // 
      // tableLayoutPanel2
      // 
      tableLayoutPanel2.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      tableLayoutPanel2.ColumnCount = 4;
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
      tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
      tableLayoutPanel2.Controls.Add(moveModPosDownBtn, 0, 0);
      tableLayoutPanel2.Controls.Add(exportModsBtn, 3, 0);
      tableLayoutPanel2.Controls.Add(moveModPosUpBtn, 1, 0);
      tableLayoutPanel2.Controls.Add(importModsBtn, 2, 0);
      tableLayoutPanel2.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
      tableLayoutPanel2.Location = new Point(335, 511);
      tableLayoutPanel2.Margin = new Padding(0);
      tableLayoutPanel2.Name = "tableLayoutPanel2";
      tableLayoutPanel2.RowCount = 1;
      tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      tableLayoutPanel2.Size = new Size(291, 23);
      tableLayoutPanel2.TabIndex = 54;
      // 
      // moveModPosDownBtn
      // 
      moveModPosDownBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      moveModPosDownBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowDown;
      moveModPosDownBtn.IconColor = Color.Black;
      moveModPosDownBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      moveModPosDownBtn.IconSize = 16;
      moveModPosDownBtn.Location = new Point(0, 0);
      moveModPosDownBtn.Margin = new Padding(0);
      moveModPosDownBtn.Name = "moveModPosDownBtn";
      moveModPosDownBtn.Size = new Size(72, 23);
      moveModPosDownBtn.TabIndex = 10;
      moveModPosDownBtn.UseVisualStyleBackColor = true;
      moveModPosDownBtn.Click += MoveEnabledModPositionDownBtnPressed;
      // 
      // exportModsBtn
      // 
      exportModsBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      exportModsBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowRightFromFile;
      exportModsBtn.IconColor = Color.Black;
      exportModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      exportModsBtn.IconSize = 16;
      exportModsBtn.Location = new Point(216, 0);
      exportModsBtn.Margin = new Padding(0);
      exportModsBtn.Name = "exportModsBtn";
      exportModsBtn.Size = new Size(75, 23);
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
      moveModPosUpBtn.Location = new Point(72, 0);
      moveModPosUpBtn.Margin = new Padding(0);
      moveModPosUpBtn.Name = "moveModPosUpBtn";
      moveModPosUpBtn.Size = new Size(72, 23);
      moveModPosUpBtn.TabIndex = 9;
      moveModPosUpBtn.UseVisualStyleBackColor = true;
      moveModPosUpBtn.Click += MoveEnabledModPositionUpBtnPressed;
      // 
      // importModsBtn
      // 
      importModsBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      importModsBtn.IconChar = FontAwesome.Sharp.IconChar.ArrowRightToFile;
      importModsBtn.IconColor = Color.Black;
      importModsBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      importModsBtn.IconSize = 16;
      importModsBtn.Location = new Point(144, 0);
      importModsBtn.Margin = new Padding(0);
      importModsBtn.Name = "importModsBtn";
      importModsBtn.Size = new Size(72, 23);
      importModsBtn.TabIndex = 12;
      importModsBtn.UseVisualStyleBackColor = true;
      importModsBtn.Click += ImportModsListBtnPressed;
      // 
      // tableLayoutPanel1
      // 
      tableLayoutPanel1.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      tableLayoutPanel1.ColumnCount = 3;
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
      tableLayoutPanel1.Controls.Add(addModBtn, 0, 0);
      tableLayoutPanel1.Controls.Add(editModBtn, 1, 0);
      tableLayoutPanel1.Controls.Add(removeModBtn, 2, 0);
      tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
      tableLayoutPanel1.Location = new Point(6, 511);
      tableLayoutPanel1.Margin = new Padding(0);
      tableLayoutPanel1.Name = "tableLayoutPanel1";
      tableLayoutPanel1.RowCount = 1;
      tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      tableLayoutPanel1.Size = new Size(294, 23);
      tableLayoutPanel1.TabIndex = 53;
      // 
      // addModBtn
      // 
      addModBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      addModBtn.IconChar = FontAwesome.Sharp.IconChar.Add;
      addModBtn.IconColor = Color.Black;
      addModBtn.IconFont = FontAwesome.Sharp.IconFont.Solid;
      addModBtn.IconSize = 16;
      addModBtn.Location = new Point(0, 0);
      addModBtn.Margin = new Padding(0);
      addModBtn.Name = "addModBtn";
      addModBtn.Size = new Size(98, 23);
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
      editModBtn.Location = new Point(98, 0);
      editModBtn.Margin = new Padding(0);
      editModBtn.Name = "editModBtn";
      editModBtn.Size = new Size(98, 23);
      editModBtn.TabIndex = 51;
      editModBtn.UseVisualStyleBackColor = true;
      editModBtn.Click += EditModBtnPressed;
      // 
      // removeModBtn
      // 
      removeModBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      removeModBtn.Enabled = false;
      removeModBtn.IconChar = FontAwesome.Sharp.IconChar.Subtract;
      removeModBtn.IconColor = Color.Black;
      removeModBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      removeModBtn.IconSize = 16;
      removeModBtn.Location = new Point(196, 0);
      removeModBtn.Margin = new Padding(0);
      removeModBtn.Name = "removeModBtn";
      removeModBtn.Size = new Size(98, 23);
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
      // enabledMods
      // 
      enabledMods.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      enabledMods.FormattingEnabled = true;
      enabledMods.ItemHeight = 15;
      enabledMods.Location = new Point(335, 69);
      enabledMods.Name = "enabledMods";
      enabledMods.SelectionMode = SelectionMode.MultiExtended;
      enabledMods.Size = new Size(291, 439);
      enabledMods.TabIndex = 1;
      // 
      // availableMods
      // 
      availableMods.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      availableMods.FormattingEnabled = true;
      availableMods.ItemHeight = 15;
      availableMods.Location = new Point(6, 69);
      availableMods.Name = "availableMods";
      availableMods.SelectionMode = SelectionMode.MultiExtended;
      availableMods.Size = new Size(294, 439);
      availableMods.TabIndex = 0;
      availableMods.SelectedIndexChanged += AvailableModsSelectedIndexChanged;
      // 
      // groupBox1
      // 
      groupBox1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      groupBox1.Controls.Add(groupBox6);
      groupBox1.Controls.Add(groupBox5);
      groupBox1.Controls.Add(tableLayoutPanel6);
      groupBox1.Controls.Add(tableLayoutPanel5);
      groupBox1.Controls.Add(aboutBtn);
      groupBox1.Controls.Add(pictureBox1);
      groupBox1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
      groupBox1.Location = new Point(653, 11);
      groupBox1.Name = "groupBox1";
      groupBox1.Size = new Size(887, 538);
      groupBox1.TabIndex = 46;
      groupBox1.TabStop = false;
      // 
      // groupBox6
      // 
      groupBox6.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      groupBox6.Controls.Add(serverParameters);
      groupBox6.Location = new Point(3, 224);
      groupBox6.Name = "groupBox6";
      groupBox6.Size = new Size(646, 284);
      groupBox6.TabIndex = 94;
      groupBox6.TabStop = false;
      groupBox6.Text = "Server Configuration";
      // 
      // serverParameters
      // 
      serverParameters.AutoScroll = true;
      serverParameters.Dock = DockStyle.Fill;
      serverParameters.FlowDirection = FlowDirection.TopDown;
      serverParameters.Location = new Point(3, 19);
      serverParameters.Margin = new Padding(15, 0, 15, 0);
      serverParameters.Name = "serverParameters";
      serverParameters.Size = new Size(640, 262);
      serverParameters.TabIndex = 90;
      // 
      // groupBox5
      // 
      groupBox5.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      groupBox5.Controls.Add(advancedParametersPanel);
      groupBox5.Location = new Point(655, 224);
      groupBox5.Name = "groupBox5";
      groupBox5.Size = new Size(229, 284);
      groupBox5.TabIndex = 93;
      groupBox5.TabStop = false;
      groupBox5.Text = "Advanced Configuration";
      // 
      // tableLayoutPanel6
      // 
      tableLayoutPanel6.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      tableLayoutPanel6.ColumnCount = 2;
      tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel6.Controls.Add(saveSettingsBtn, 1, 0);
      tableLayoutPanel6.Controls.Add(loadSettingsBtn, 0, 0);
      tableLayoutPanel6.Location = new Point(682, 511);
      tableLayoutPanel6.Name = "tableLayoutPanel6";
      tableLayoutPanel6.RowCount = 1;
      tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
      tableLayoutPanel6.Size = new Size(199, 23);
      tableLayoutPanel6.TabIndex = 92;
      // 
      // aboutBtn
      // 
      aboutBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
      aboutBtn.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
      aboutBtn.IconColor = Color.Black;
      aboutBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      aboutBtn.IconSize = 18;
      aboutBtn.Location = new Point(826, 22);
      aboutBtn.Name = "aboutBtn";
      aboutBtn.Size = new Size(55, 23);
      aboutBtn.TabIndex = 4;
      aboutBtn.UseVisualStyleBackColor = true;
      aboutBtn.Click += AboutBtnPressed;
      // 
      // pictureBox1
      // 
      pictureBox1.BackgroundImageLayout = ImageLayout.Center;
      pictureBox1.Dock = DockStyle.Top;
      pictureBox1.Image = (Image) resources.GetObject("pictureBox1.Image");
      pictureBox1.Location = new Point(3, 19);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new Size(881, 199);
      pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      pictureBox1.TabIndex = 51;
      pictureBox1.TabStop = false;
      // 
      // useUpnp
      // 
      useUpnp.AutoSize = true;
      useUpnp.Checked = true;
      useUpnp.CheckState = CheckState.Checked;
      useUpnp.Dock = DockStyle.Fill;
      useUpnp.Location = new Point(415, 0);
      useUpnp.Margin = new Padding(5, 0, 0, 0);
      useUpnp.Name = "useUpnp";
      useUpnp.Size = new Size(422, 29);
      useUpnp.TabIndex = 32;
      useUpnp.Text = "Use UPnP";
      useUpnp.UseVisualStyleBackColor = true;
      useUpnp.CheckedChanged += OnUseUPnPCheckChanged;
      // 
      // useExperimentalCheckBox
      // 
      useExperimentalCheckBox.AutoSize = true;
      useExperimentalCheckBox.Dock = DockStyle.Fill;
      useExperimentalCheckBox.Location = new Point(257, 0);
      useExperimentalCheckBox.Margin = new Padding(5, 0, 0, 0);
      useExperimentalCheckBox.Name = "useExperimentalCheckBox";
      useExperimentalCheckBox.Size = new Size(153, 29);
      useExperimentalCheckBox.TabIndex = 31;
      useExperimentalCheckBox.Text = "Use Experimental Server";
      useExperimentalCheckBox.UseVisualStyleBackColor = true;
      useExperimentalCheckBox.CheckedChanged += UseExperimentalServerCheckboxChanged;
      // 
      // label30
      // 
      label30.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      label30.AutoSize = true;
      label30.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
      label30.Location = new Point(11, 190);
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
      logLevelComboBox.Location = new Point(74, 187);
      logLevelComboBox.Name = "logLevelComboBox";
      logLevelComboBox.Size = new Size(96, 23);
      logLevelComboBox.TabIndex = 30;
      logLevelComboBox.Text = "normal";
      // 
      // locateServerFilesBtn
      // 
      locateServerFilesBtn.Dock = DockStyle.Fill;
      locateServerFilesBtn.IconChar = FontAwesome.Sharp.IconChar.Search;
      locateServerFilesBtn.IconColor = Color.Black;
      locateServerFilesBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      locateServerFilesBtn.IconSize = 16;
      locateServerFilesBtn.Location = new Point(129, 3);
      locateServerFilesBtn.Name = "locateServerFilesBtn";
      locateServerFilesBtn.Size = new Size(57, 23);
      locateServerFilesBtn.TabIndex = 7;
      locateServerFilesBtn.UseVisualStyleBackColor = true;
      locateServerFilesBtn.Click += LocateServerFilesBtnPressed;
      // 
      // clearLogBtn
      // 
      clearLogBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      clearLogBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      clearLogBtn.IconColor = Color.Black;
      clearLogBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      clearLogBtn.Location = new Point(1458, 187);
      clearLogBtn.Name = "clearLogBtn";
      clearLogBtn.Size = new Size(66, 23);
      clearLogBtn.TabIndex = 6;
      clearLogBtn.Text = "Clear Log";
      clearLogBtn.UseVisualStyleBackColor = true;
      clearLogBtn.Click += ClearLogBtnPressed;
      // 
      // deleteServerFilesBtn
      // 
      deleteServerFilesBtn.Dock = DockStyle.Fill;
      deleteServerFilesBtn.IconChar = FontAwesome.Sharp.IconChar.Trash;
      deleteServerFilesBtn.IconColor = Color.Black;
      deleteServerFilesBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      deleteServerFilesBtn.IconSize = 16;
      deleteServerFilesBtn.Location = new Point(192, 3);
      deleteServerFilesBtn.Name = "deleteServerFilesBtn";
      deleteServerFilesBtn.Size = new Size(57, 23);
      deleteServerFilesBtn.TabIndex = 5;
      deleteServerFilesBtn.TextAlign = ContentAlignment.MiddleRight;
      deleteServerFilesBtn.UseVisualStyleBackColor = true;
      deleteServerFilesBtn.Click += DeleteServerFilesBtnPressed;
      // 
      // startServerBtn
      // 
      startServerBtn.Dock = DockStyle.Fill;
      startServerBtn.IconChar = FontAwesome.Sharp.IconChar.Play;
      startServerBtn.IconColor = Color.Black;
      startServerBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      startServerBtn.IconSize = 16;
      startServerBtn.Location = new Point(3, 3);
      startServerBtn.Name = "startServerBtn";
      startServerBtn.Size = new Size(57, 23);
      startServerBtn.TabIndex = 4;
      startServerBtn.TextAlign = ContentAlignment.MiddleRight;
      startServerBtn.UseVisualStyleBackColor = true;
      startServerBtn.Click += StartServerBtnPressed;
      // 
      // groupBox3
      // 
      groupBox3.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      groupBox3.Controls.Add(steamCmdLog);
      groupBox3.Location = new Point(8, 74);
      groupBox3.Name = "groupBox3";
      groupBox3.Size = new Size(1516, 110);
      groupBox3.TabIndex = 3;
      groupBox3.TabStop = false;
      groupBox3.Text = "Log";
      // 
      // steamCmdLog
      // 
      steamCmdLog.Dock = DockStyle.Fill;
      steamCmdLog.Location = new Point(3, 19);
      steamCmdLog.Multiline = true;
      steamCmdLog.Name = "steamCmdLog";
      steamCmdLog.ReadOnly = true;
      steamCmdLog.ScrollBars = ScrollBars.Vertical;
      steamCmdLog.Size = new Size(1510, 88);
      steamCmdLog.TabIndex = 1;
      // 
      // steamCmdAlert
      // 
      steamCmdAlert.AutoSize = true;
      steamCmdAlert.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
      steamCmdAlert.Location = new Point(8, 18);
      steamCmdAlert.Name = "steamCmdAlert";
      steamCmdAlert.Size = new Size(573, 17);
      steamCmdAlert.TabIndex = 2;
      steamCmdAlert.Text = "SteamCMD and the Arma Server files were not detected, please Download before continuing.";
      steamCmdAlert.TextAlign = ContentAlignment.MiddleRight;
      // 
      // downloadSteamCmdBtn
      // 
      downloadSteamCmdBtn.Dock = DockStyle.Fill;
      downloadSteamCmdBtn.IconChar = FontAwesome.Sharp.IconChar.Download;
      downloadSteamCmdBtn.IconColor = Color.Black;
      downloadSteamCmdBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      downloadSteamCmdBtn.IconSize = 16;
      downloadSteamCmdBtn.Location = new Point(66, 3);
      downloadSteamCmdBtn.Name = "downloadSteamCmdBtn";
      downloadSteamCmdBtn.Size = new Size(57, 23);
      downloadSteamCmdBtn.TabIndex = 0;
      downloadSteamCmdBtn.TextAlign = ContentAlignment.MiddleRight;
      downloadSteamCmdBtn.UseVisualStyleBackColor = true;
      downloadSteamCmdBtn.Click += DownloadSteamCmdBtnPressed;
      // 
      // splitContainer1
      // 
      splitContainer1.BorderStyle = BorderStyle.FixedSingle;
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.Location = new Point(0, 0);
      splitContainer1.Name = "splitContainer1";
      splitContainer1.Orientation = Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(groupBox2);
      splitContainer1.Panel1.Controls.Add(groupBox1);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(groupBox4);
      splitContainer1.Size = new Size(1550, 800);
      splitContainer1.SplitterDistance = 554;
      splitContainer1.TabIndex = 1;
      // 
      // groupBox4
      // 
      groupBox4.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      groupBox4.Controls.Add(tableLayoutPanel3);
      groupBox4.Controls.Add(steamCmdAlert);
      groupBox4.Controls.Add(serverRunningLabel);
      groupBox4.Controls.Add(logLevelComboBox);
      groupBox4.Controls.Add(label30);
      groupBox4.Controls.Add(clearLogBtn);
      groupBox4.Controls.Add(groupBox3);
      groupBox4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point,  0);
      groupBox4.Location = new Point(11, 11);
      groupBox4.Name = "groupBox4";
      groupBox4.Padding = new Padding(5);
      groupBox4.Size = new Size(1529, 218);
      groupBox4.TabIndex = 51;
      groupBox4.TabStop = false;
      groupBox4.Text = "Server Management";
      // 
      // tableLayoutPanel3
      // 
      tableLayoutPanel3.ColumnCount = 6;
      tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.383637F));
      tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.383637F));
      tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.383637F));
      tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.383637F));
      tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
      tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62.46545F));
      tableLayoutPanel3.Controls.Add(startServerBtn, 0, 0);
      tableLayoutPanel3.Controls.Add(downloadSteamCmdBtn, 1, 0);
      tableLayoutPanel3.Controls.Add(locateServerFilesBtn, 2, 0);
      tableLayoutPanel3.Controls.Add(deleteServerFilesBtn, 3, 0);
      tableLayoutPanel3.Controls.Add(useExperimentalCheckBox, 4, 0);
      tableLayoutPanel3.Controls.Add(useUpnp, 5, 0);
      tableLayoutPanel3.Location = new Point(6, 41);
      tableLayoutPanel3.Name = "tableLayoutPanel3";
      tableLayoutPanel3.RowCount = 1;
      tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      tableLayoutPanel3.Size = new Size(837, 29);
      tableLayoutPanel3.TabIndex = 51;
      // 
      // Main
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1550, 800);
      Controls.Add(splitContainer1);
      Icon = (Icon) resources.GetObject("$this.Icon");
      MinimumSize = new Size(1440, 782);
      Name = "Main";
      Text = "Longbow: Arma Reforger Dedicated Server Tool";
      tableLayoutPanel5.ResumeLayout(false);
      groupBox2.ResumeLayout(false);
      groupBox2.PerformLayout();
      tableLayoutPanel4.ResumeLayout(false);
      tableLayoutPanel2.ResumeLayout(false);
      tableLayoutPanel1.ResumeLayout(false);
      groupBox1.ResumeLayout(false);
      groupBox6.ResumeLayout(false);
      groupBox5.ResumeLayout(false);
      tableLayoutPanel6.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize) pictureBox1).EndInit();
      groupBox3.ResumeLayout(false);
      groupBox3.PerformLayout();
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      groupBox4.ResumeLayout(false);
      groupBox4.PerformLayout();
      tableLayoutPanel3.ResumeLayout(false);
      tableLayoutPanel3.PerformLayout();
      ResumeLayout(false);
    }

    #endregion
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
    private SplitContainer splitContainer1;
    private GroupBox groupBox4;
    private TableLayoutPanel tableLayoutPanel3;
    private TableLayoutPanel tableLayoutPanel4;
    private TableLayoutPanel tableLayoutPanel5;
    private TableLayoutPanel tableLayoutPanel6;
    private GroupBox groupBox6;
    private FlowLayoutPanel serverParameters;
    private GroupBox groupBox5;
  }
}
