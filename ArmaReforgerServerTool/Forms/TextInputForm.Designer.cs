using Longbow.Components.ui;

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
      textInputField = new FluentTextBox();
      okBtn = new Button();
      SuspendLayout();
      // 
      // textInputField
      // 
      textInputField.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      textInputField.BackColor = Color.Transparent;
      textInputField.Location = new Point(12, 12);
      textInputField.Name = "textInputField";
      textInputField.Padding = new Padding(10, 7, 10, 7);
      textInputField.Size = new Size(600, 378);
      textInputField.TabIndex = 0;
      // 
      // okBtn
      // 
      okBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      okBtn.Location = new Point(537, 397);
      okBtn.Name = "okBtn";
      okBtn.Size = new Size(75, 32);
      okBtn.TabIndex = 1;
      okBtn.Text = "OK";
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
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - Text Input Form";
      ResumeLayout(false);
    }

    #endregion

    private FluentTextBox textInputField;
        private Button okBtn;
    }
}
