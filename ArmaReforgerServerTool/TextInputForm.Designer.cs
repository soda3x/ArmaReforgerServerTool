namespace ReforgerServerApp
{
    partial class TextInputForm
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
            textInputField = new TextBox();
            okBtn = new Button();
            SuspendLayout();
            // 
            // textInputField
            // 
            textInputField.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textInputField.Location = new Point(12, 12);
            textInputField.Multiline = true;
            textInputField.Name = "textInputField";
            textInputField.Size = new Size(600, 390);
            textInputField.TabIndex = 0;
            // 
            // okBtn
            // 
            okBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okBtn.Location = new Point(537, 408);
            okBtn.Name = "okBtn";
            okBtn.Size = new Size(75, 23);
            okBtn.TabIndex = 1;
            okBtn.Text = "&OK";
            okBtn.UseVisualStyleBackColor = true;
            okBtn.Click += OkBtnClicked;
            // 
            // TextInputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 441);
            Controls.Add(okBtn);
            Controls.Add(textInputField);
            MinimumSize = new Size(640, 480);
            Name = "TextInputForm";
            ShowIcon = false;
            Text = "Arma Reforger Dedicated Server Tool - Text Input Form";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textInputField;
        private Button okBtn;
    }
}