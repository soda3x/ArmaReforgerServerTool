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
      aboutText = new Label();
      okBtn = new Button();
      reportBtn = new Button();
      SuspendLayout();
      // 
      // aboutText
      // 
      aboutText.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      aboutText.AutoSize = true;
      aboutText.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold);
      aboutText.Location = new Point(12, 9);
      aboutText.Name = "aboutText";
      aboutText.Size = new Size(61, 13);
      aboutText.TabIndex = 0;
      aboutText.Text = "About text";
      // 
      // okBtn
      // 
      okBtn.Location = new Point(347, 106);
      okBtn.Name = "okBtn";
      okBtn.Size = new Size(75, 23);
      okBtn.TabIndex = 1;
      okBtn.Text = "OK";
      okBtn.UseVisualStyleBackColor = true;
      okBtn.Click += OKBtnPressed;
      // 
      // reportBtn
      // 
      reportBtn.Location = new Point(12, 106);
      reportBtn.Name = "reportBtn";
      reportBtn.Size = new Size(113, 23);
      reportBtn.TabIndex = 2;
      reportBtn.Text = "Report an Issue...";
      reportBtn.UseVisualStyleBackColor = true;
      reportBtn.Click += ReportBtnPressed;
      // 
      // AboutBox
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(434, 141);
      Controls.Add(reportBtn);
      Controls.Add(okBtn);
      Controls.Add(aboutText);
      MaximizeBox = false;
      MaximumSize = new Size(450, 180);
      MinimizeBox = false;
      MinimumSize = new Size(450, 180);
      Name = "AboutBox";
      ShowIcon = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - About";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label aboutText;
        private Button okBtn;
        private Button reportBtn;
    }
}