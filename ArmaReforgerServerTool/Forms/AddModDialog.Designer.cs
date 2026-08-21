using Longbow.Components.ui;

namespace ReforgerServerApp
{
    partial class AddModDialog
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
      modName = new FluentTextBox();
      modId = new FluentTextBox();
      addBtn = new Button();
      cancelBtn = new Button();
      modVers = new FluentTextBox();
      requiredLabel = new Label();
      required = new FluentToggleSwitch();
      SuspendLayout();
      // 
      // modName
      // 
      modName.BackColor = Color.Transparent;
      modName.FieldBackColor = SystemColors.Window;
      modName.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
      modName.Location = new Point(12, 12);
      modName.Multiline = false;
      modName.Name = "modName";
      modName.Padding = new Padding(10, 7, 10, 7);
      modName.PlaceholderText = "Enter Mod Name...";
      modName.ReadOnly = false;
      modName.ScrollBars = ScrollBars.None;
      modName.Size = new Size(460, 32);
      modName.TabIndex = 0;
      modName.UseSystemPasswordChar = false;
      // 
      // modId
      // 
      modId.BackColor = Color.Transparent;
      modId.FieldBackColor = SystemColors.Window;
      modId.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
      modId.Location = new Point(12, 57);
      modId.Multiline = false;
      modId.Name = "modId";
      modId.Padding = new Padding(10, 7, 10, 7);
      modId.PlaceholderText = "Enter Mod ID from Arma Workshop...";
      modId.ReadOnly = false;
      modId.ScrollBars = ScrollBars.None;
      modId.Size = new Size(460, 32);
      modId.TabIndex = 1;
      modId.UseSystemPasswordChar = false;
      // 
      // addBtn
      // 
      addBtn.Location = new Point(397, 157);
      addBtn.Name = "addBtn";
      addBtn.Size = new Size(75, 32);
      addBtn.TabIndex = 4;
      addBtn.Text = "Add";
      addBtn.UseVisualStyleBackColor = true;
      addBtn.Click += AddBtnPressed;
      // 
      // cancelBtn
      // 
      cancelBtn.Location = new Point(12, 157);
      cancelBtn.Name = "cancelBtn";
      cancelBtn.Size = new Size(75, 32);
      cancelBtn.TabIndex = 3;
      cancelBtn.Text = "Cancel";
      cancelBtn.UseVisualStyleBackColor = true;
      cancelBtn.Click += CancelBtnPressed;
      // 
      // modVers
      // 
      modVers.BackColor = Color.Transparent;
      modVers.FieldBackColor = SystemColors.Window;
      modVers.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
      modVers.Location = new Point(12, 104);
      modVers.Multiline = false;
      modVers.Name = "modVers";
      modVers.Padding = new Padding(10, 7, 10, 7);
      modVers.PlaceholderText = "Enter Mod Version, or leave empty to get the latest...";
      modVers.ReadOnly = false;
      modVers.ScrollBars = ScrollBars.None;
      modVers.Size = new Size(300, 32);
      modVers.TabIndex = 2;
      modVers.UseSystemPasswordChar = false;
      // 
      // requiredLabel
      // 
      requiredLabel.AutoSize = true;
      requiredLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
      requiredLabel.Location = new Point(394, 110);
      requiredLabel.Name = "requiredLabel";
      requiredLabel.Size = new Size(77, 21);
      requiredLabel.TabIndex = 11;
      requiredLabel.Text = "Required";
      // 
      // required
      // 
      required.BackColor = Color.Transparent;
      required.Checked = false;
      required.Location = new Point(338, 108);
      required.Name = "required";
      required.Size = new Size(50, 24);
      required.TabIndex = 12;
      // 
      // AddModDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(484, 201);
      ControlBox = false;
      Controls.Add(required);
      Controls.Add(requiredLabel);
      Controls.Add(modVers);
      Controls.Add(cancelBtn);
      Controls.Add(addBtn);
      Controls.Add(modId);
      Controls.Add(modName);
      MaximumSize = new Size(500, 240);
      MinimumSize = new Size(500, 240);
      Name = "AddModDialog";
      SizeGripStyle = SizeGripStyle.Hide;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - Add Mod";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private FluentTextBox modName;
        private FluentTextBox modId;
        private Button addBtn;
        private Button cancelBtn;
        private FluentTextBox modVers;
        private Label requiredLabel;
        private FluentToggleSwitch required;
    }
}
