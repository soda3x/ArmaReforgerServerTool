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
            scenarioList = new ListBox();
            selectScenarioBtn = new Button();
            reloadScenariosBtn = new Button();
            currentlySelectedLbl = new Label();
            manualScenarioIdTextBox = new TextBox();
            SuspendLayout();
            // 
            // scenarioList
            // 
            scenarioList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            scenarioList.FormattingEnabled = true;
            scenarioList.ItemHeight = 15;
            scenarioList.Location = new Point(12, 12);
            scenarioList.Name = "scenarioList";
            scenarioList.Size = new Size(682, 394);
            scenarioList.TabIndex = 0;
            scenarioList.SelectedIndexChanged += ScenarioListSelectionChanged;
            // 
            // selectScenarioBtn
            // 
            selectScenarioBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            selectScenarioBtn.Location = new Point(579, 443);
            selectScenarioBtn.Name = "selectScenarioBtn";
            selectScenarioBtn.Size = new Size(115, 23);
            selectScenarioBtn.TabIndex = 1;
            selectScenarioBtn.Text = "Select Scenario";
            selectScenarioBtn.UseVisualStyleBackColor = true;
            selectScenarioBtn.Click += SelectScenarioButtonClicked;
            // 
            // reloadScenariosBtn
            // 
            reloadScenariosBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            reloadScenariosBtn.Location = new Point(12, 443);
            reloadScenariosBtn.Name = "reloadScenariosBtn";
            reloadScenariosBtn.Size = new Size(115, 23);
            reloadScenariosBtn.TabIndex = 2;
            reloadScenariosBtn.Text = "Reload Scenarios";
            reloadScenariosBtn.UseVisualStyleBackColor = true;
            reloadScenariosBtn.Click += ReloadScenariosButtonClicked;
            // 
            // currentlySelectedLbl
            // 
            currentlySelectedLbl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            currentlySelectedLbl.AutoEllipsis = true;
            currentlySelectedLbl.Location = new Point(133, 447);
            currentlySelectedLbl.Name = "currentlySelectedLbl";
            currentlySelectedLbl.Size = new Size(440, 15);
            currentlySelectedLbl.TabIndex = 3;
            currentlySelectedLbl.Text = "Currently selected Scenario is:";
            currentlySelectedLbl.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // manualScenarioIdTextBox
            // 
            manualScenarioIdTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            manualScenarioIdTextBox.Location = new Point(12, 414);
            manualScenarioIdTextBox.Name = "manualScenarioIdTextBox";
            manualScenarioIdTextBox.PlaceholderText = "Enter a Scenario ID here to manually set one...";
            manualScenarioIdTextBox.Size = new Size(682, 23);
            manualScenarioIdTextBox.TabIndex = 4;
            manualScenarioIdTextBox.TextChanged += ManualScenarioIDTextChanged;
            // 
            // ScenarioSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(706, 478);
            Controls.Add(manualScenarioIdTextBox);
            Controls.Add(currentlySelectedLbl);
            Controls.Add(reloadScenariosBtn);
            Controls.Add(selectScenarioBtn);
            Controls.Add(scenarioList);
            MinimumSize = new Size(640, 480);
            Name = "ScenarioSelector";
            ShowIcon = false;
            Text = "Arma Reforger Dedicated Server Tool - Select Scenario";
            FormClosing += OnFormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox scenarioList;
        private Button selectScenarioBtn;
        private Button reloadScenariosBtn;
        private Label currentlySelectedLbl;
        private TextBox manualScenarioIdTextBox;
    }
}