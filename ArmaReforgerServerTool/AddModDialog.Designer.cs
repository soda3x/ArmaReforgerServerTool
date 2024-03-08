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
            SuspendLayout();
            // 
            // modName
            // 
            modName.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            modName.Location = new Point(106, 12);
            modName.Name = "modName";
            modName.Size = new Size(366, 23);
            modName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(8, 10);
            label1.Name = "label1";
            label1.Size = new Size(92, 21);
            label1.TabIndex = 1;
            label1.Text = "Mod Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(8, 39);
            label2.Name = "label2";
            label2.Size = new Size(65, 21);
            label2.TabIndex = 3;
            label2.Text = "Mod ID";
            // 
            // modId
            // 
            modId.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            modId.Location = new Point(106, 41);
            modId.Name = "modId";
            modId.Size = new Size(366, 23);
            modId.TabIndex = 2;
            // 
            // addBtn
            // 
            addBtn.Location = new Point(397, 70);
            addBtn.Name = "addBtn";
            addBtn.Size = new Size(75, 23);
            addBtn.TabIndex = 6;
            addBtn.Text = "Add";
            addBtn.UseVisualStyleBackColor = true;
            addBtn.Click += AddBtnPressed;
            // 
            // cancelBtn
            // 
            cancelBtn.Location = new Point(316, 70);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(75, 23);
            cancelBtn.TabIndex = 7;
            cancelBtn.Text = "Cancel";
            cancelBtn.UseVisualStyleBackColor = true;
            cancelBtn.Click += CancelBtnPressed;
            // 
            // AddModDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 101);
            ControlBox = false;
            Controls.Add(cancelBtn);
            Controls.Add(addBtn);
            Controls.Add(label2);
            Controls.Add(modId);
            Controls.Add(label1);
            Controls.Add(modName);
            MaximumSize = new Size(500, 140);
            MinimumSize = new Size(500, 140);
            Name = "AddModDialog";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Arma Refoger Dedicated Server Tool - Add Mod";
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
    }
}