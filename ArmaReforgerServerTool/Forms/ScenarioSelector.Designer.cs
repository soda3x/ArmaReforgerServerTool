using Longbow.Components.ui;

namespace ReforgerServerApp
{
    partial class ScenarioSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      scenarioList = new FluentListBox();
      selectScenarioBtn = new Button();
      reloadScenariosBtn = new Button();
      currentlySelectedLbl = new Label();
      manualScenarioIdTextBox = new FluentTextBox();
      loadingAnim = new AnimOfDots.Circular();
      SuspendLayout();
      // 
      // scenarioList
      // 
      scenarioList.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      scenarioList.BackColor = Color.Transparent;
      scenarioList.Location = new Point(12, 12);
      scenarioList.Name = "scenarioList";
      scenarioList.Padding = new Padding(3);
      scenarioList.Size = new Size(682, 374);
      scenarioList.TabIndex = 0;
      scenarioList.SelectedIndexChanged += ScenarioListSelectionChanged;
      // 
      // selectScenarioBtn
      // 
      selectScenarioBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      selectScenarioBtn.Location = new Point(579, 437);
      selectScenarioBtn.Name = "selectScenarioBtn";
      selectScenarioBtn.Size = new Size(115, 32);
      selectScenarioBtn.TabIndex = 1;
      selectScenarioBtn.Text = "Select Scenario";
      selectScenarioBtn.UseVisualStyleBackColor = true;
      selectScenarioBtn.Click += SelectScenarioButtonClicked;
      // 
      // reloadScenariosBtn
      // 
      reloadScenariosBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      reloadScenariosBtn.Location = new Point(12, 437);
      reloadScenariosBtn.Name = "reloadScenariosBtn";
      reloadScenariosBtn.Size = new Size(115, 32);
      reloadScenariosBtn.TabIndex = 2;
      reloadScenariosBtn.Text = "Reload Scenarios";
      reloadScenariosBtn.UseVisualStyleBackColor = true;
      reloadScenariosBtn.Click += ReloadScenariosButtonClicked;
      // 
      // currentlySelectedLbl
      // 
      currentlySelectedLbl.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      currentlySelectedLbl.AutoEllipsis = true;
      currentlySelectedLbl.Location = new Point(165, 446);
      currentlySelectedLbl.Name = "currentlySelectedLbl";
      currentlySelectedLbl.Size = new Size(408, 15);
      currentlySelectedLbl.TabIndex = 3;
      currentlySelectedLbl.Text = "Currently selected Scenario is:";
      currentlySelectedLbl.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // manualScenarioIdTextBox
      // 
      manualScenarioIdTextBox.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      manualScenarioIdTextBox.BackColor = Color.Transparent;
      manualScenarioIdTextBox.Location = new Point(12, 397);
      manualScenarioIdTextBox.Name = "manualScenarioIdTextBox";
      manualScenarioIdTextBox.PlaceholderText = "Enter a scenario here to use it instead of one from the above list...";
      manualScenarioIdTextBox.Padding = new Padding(10, 7, 10, 7);
      manualScenarioIdTextBox.Size = new Size(682, 32);
      manualScenarioIdTextBox.TabIndex = 4;
      manualScenarioIdTextBox.TextChanged += ManualScenarioIDTextChanged;
      // 
      // loadingAnim
      // 
      loadingAnim.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      loadingAnim.AnimationSpeed = 10;
      loadingAnim.BackColor = Color.Transparent;
      loadingAnim.ForeColor = Color.DodgerBlue;
      loadingAnim.Location = new Point(133, 441);
      loadingAnim.Name = "loadingAnim";
      loadingAnim.Running = true;
      loadingAnim.Size = new Size(26, 24);
      loadingAnim.TabIndex = 5;
      loadingAnim.Visible = false;
      // 
      // ScenarioSelector
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(706, 481);
      Controls.Add(loadingAnim);
      Controls.Add(manualScenarioIdTextBox);
      Controls.Add(currentlySelectedLbl);
      Controls.Add(reloadScenariosBtn);
      Controls.Add(selectScenarioBtn);
      Controls.Add(scenarioList);
      MinimumSize = new Size(640, 520);
      Name = "ScenarioSelector";
      ShowIcon = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - Select Scenario";
      FormClosing += OnFormClosing;
      ResumeLayout(false);
    }

    #endregion

    private FluentListBox scenarioList;
        private Button selectScenarioBtn;
        private Button reloadScenariosBtn;
        private Label currentlySelectedLbl;
        private FluentTextBox manualScenarioIdTextBox;
    private AnimOfDots.Circular loadingAnim;
  }
}
