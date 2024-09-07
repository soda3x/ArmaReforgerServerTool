namespace ReforgerServerApp
{
    partial class AboutBox
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
            this.aboutText = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.reportBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // aboutText
            // 
            this.aboutText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutText.AutoSize = true;
            this.aboutText.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.aboutText.Location = new System.Drawing.Point(12, 9);
            this.aboutText.Name = "aboutText";
            this.aboutText.Size = new System.Drawing.Size(74, 17);
            this.aboutText.TabIndex = 0;
            this.aboutText.Text = "About text";
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(317, 67);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 1;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OKBtnPressed);
            // 
            // reportBtn
            // 
            this.reportBtn.Location = new System.Drawing.Point(12, 67);
            this.reportBtn.Name = "reportBtn";
            this.reportBtn.Size = new System.Drawing.Size(113, 23);
            this.reportBtn.TabIndex = 2;
            this.reportBtn.Text = "Report an Issue...";
            this.reportBtn.UseVisualStyleBackColor = true;
            this.reportBtn.Click += new System.EventHandler(this.ReportBtnPressed);
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 101);
            this.Controls.Add(this.reportBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.aboutText);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(420, 140);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(420, 140);
            this.Name = "AboutBox";
            this.ShowIcon = false;
            this.Text = "Arma Reforger Dedicated Server Tool - About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label aboutText;
        private Button okBtn;
        private Button reportBtn;
    }
}