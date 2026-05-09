namespace ReforgerServerApp
{
    partial class ServerParameterList
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      parameterName = new Label();
      parameterValue = new FontAwesome.Sharp.IconButton();
      SuspendLayout();
      // 
      // parameterName
      // 
      parameterName.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      parameterName.AutoSize = true;
      parameterName.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
      parameterName.Location = new Point(15, 4);
      parameterName.Name = "parameterName";
      parameterName.Size = new Size(0, 21);
      parameterName.TabIndex = 0;
      parameterName.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // parameterValue
      // 
      parameterValue.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      parameterValue.IconChar = FontAwesome.Sharp.IconChar.List;
      parameterValue.IconColor = Color.Black;
      parameterValue.IconFont = FontAwesome.Sharp.IconFont.Auto;
      parameterValue.IconSize = 16;
      parameterValue.Location = new Point(354, 4);
      parameterValue.Name = "parameterValue";
      parameterValue.Size = new Size(24, 24);
      parameterValue.TabIndex = 1;
      parameterValue.Click += OnButtonPressed;
      // 
      // ServerParameterList
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(parameterValue);
      Controls.Add(parameterName);
      Name = "ServerParameterList";
      Controls.SetChildIndex(parameterName, 0);
      Controls.SetChildIndex(parameterValue, 0);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label parameterName;
        private FontAwesome.Sharp.IconButton parameterValue;
    }
}
