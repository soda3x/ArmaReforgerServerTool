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
      modName = new TextBox();
      label1 = new Label();
      label2 = new Label();
      modId = new TextBox();
      addBtn = new Button();
      cancelBtn = new Button();
      label3 = new Label();
      modVers = new TextBox();
      requiredLabel = new Label();
      required = new CheckBox();
      SuspendLayout();
      // 
      // modName
      // 
      modName.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
      modName.Location = new Point(117, 12);
      modName.Name = "modName";
      modName.Size = new Size(355, 23);
      modName.TabIndex = 0;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
      label1.Location = new Point(8, 14);
      label1.Name = "label1";
      label1.Size = new Size(92, 21);
      label1.TabIndex = 1;
      label1.Text = "Mod Name";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
      label2.Location = new Point(8, 43);
      label2.Name = "label2";
      label2.Size = new Size(65, 21);
      label2.TabIndex = 3;
      label2.Text = "Mod ID";
      // 
      // modId
      // 
      modId.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
      modId.Location = new Point(117, 41);
      modId.Name = "modId";
      modId.Size = new Size(355, 23);
      modId.TabIndex = 1;
      // 
      // addBtn
      // 
      addBtn.Location = new Point(400, 109);
      addBtn.Name = "addBtn";
      addBtn.Size = new Size(75, 23);
      addBtn.TabIndex = 4;
      addBtn.Text = "Add";
      addBtn.UseVisualStyleBackColor = true;
      addBtn.Click += AddBtnPressed;
      // 
      // cancelBtn
      // 
      cancelBtn.Location = new Point(319, 109);
      cancelBtn.Name = "cancelBtn";
      cancelBtn.Size = new Size(75, 23);
      cancelBtn.TabIndex = 3;
      cancelBtn.Text = "Cancel";
      cancelBtn.UseVisualStyleBackColor = true;
      cancelBtn.Click += CancelBtnPressed;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
      label3.Location = new Point(8, 72);
      label3.Name = "label3";
      label3.Size = new Size(103, 21);
      label3.TabIndex = 9;
      label3.Text = "Mod Version";
      // 
      // modVers
      // 
      modVers.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
      modVers.Location = new Point(117, 70);
      modVers.Name = "modVers";
      modVers.PlaceholderText = "Latest";
      modVers.Size = new Size(150, 23);
      modVers.TabIndex = 2;
      // 
      // requiredLabel
      // 
      requiredLabel.AutoSize = true;
      requiredLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
      requiredLabel.Location = new Point(273, 72);
      requiredLabel.Name = "requiredLabel";
      requiredLabel.Size = new Size(77, 21);
      requiredLabel.TabIndex = 11;
      requiredLabel.Text = "Required";
      // 
      // required
      // 
      required.AutoSize = true;
      required.Location = new Point(457, 78);
      required.Name = "required";
      required.Size = new Size(15, 14);
      required.TabIndex = 12;
      required.UseVisualStyleBackColor = true;
      // 
      // AddModDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(484, 141);
      ControlBox = false;
      Controls.Add(required);
      Controls.Add(requiredLabel);
      Controls.Add(label3);
      Controls.Add(modVers);
      Controls.Add(cancelBtn);
      Controls.Add(addBtn);
      Controls.Add(label2);
      Controls.Add(modId);
      Controls.Add(label1);
      Controls.Add(modName);
      MaximumSize = new Size(500, 180);
      MinimumSize = new Size(500, 180);
      Name = "AddModDialog";
      SizeGripStyle = SizeGripStyle.Hide;
      Text = "Longbow - Add Mod";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox modName;
        private Label label1;
        private Label label2;
        private TextBox modId;
        private Button addBtn;
        private Button cancelBtn;
        private Label label3;
        private TextBox modVers;
        private Label requiredLabel;
        private CheckBox required;
    }
}