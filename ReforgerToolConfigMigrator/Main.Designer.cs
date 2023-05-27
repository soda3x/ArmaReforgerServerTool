namespace ReforgerToolConfigMigrator
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
            convertBtn = new Button();
            loadBtn = new Button();
            helperLbl = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // convertBtn
            // 
            convertBtn.Location = new Point(626, 212);
            convertBtn.Name = "convertBtn";
            convertBtn.Size = new Size(94, 29);
            convertBtn.TabIndex = 0;
            convertBtn.Text = "Convert";
            convertBtn.UseVisualStyleBackColor = true;
            convertBtn.Click += convertBtn_Click;
            // 
            // loadBtn
            // 
            loadBtn.Location = new Point(12, 212);
            loadBtn.Name = "loadBtn";
            loadBtn.Size = new Size(167, 29);
            loadBtn.TabIndex = 1;
            loadBtn.Text = "Load Config File...";
            loadBtn.UseVisualStyleBackColor = true;
            loadBtn.Click += loadBtn_Click;
            // 
            // helperLbl
            // 
            helperLbl.AutoSize = true;
            helperLbl.Location = new Point(12, 65);
            helperLbl.Name = "helperLbl";
            helperLbl.Size = new Size(415, 20);
            helperLbl.TabIndex = 2;
            helperLbl.Text = "Load a configuration file from version 0.5 or earlier to begin...";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(252, 31);
            label2.TabIndex = 3;
            label2.Text = "Config Migration Tool";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(732, 253);
            Controls.Add(label2);
            Controls.Add(helperLbl);
            Controls.Add(loadBtn);
            Controls.Add(convertBtn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(750, 300);
            MinimumSize = new Size(750, 300);
            Name = "Main";
            Text = "Arma Reforger Dedicated Server Tool - Config Migration Tool";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button convertBtn;
        private Button loadBtn;
        private Label helperLbl;
        private Label label2;
    }
}