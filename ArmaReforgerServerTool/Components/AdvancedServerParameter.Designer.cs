namespace ReforgerServerApp.Components
{
    partial class AdvancedServerParameter
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
      parameterEnabled = new CheckBox();
      description = new Label();
      SuspendLayout();
      // 
      // parameterEnabled
      // 
      parameterEnabled.AutoSize = true;
      parameterEnabled.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      parameterEnabled.Location = new Point(3, 3);
      parameterEnabled.Name = "parameterEnabled";
      parameterEnabled.Size = new Size(168, 19);
      parameterEnabled.TabIndex = 0;
      parameterEnabled.Text = "Parameter Friendly Name";
      parameterEnabled.UseVisualStyleBackColor = true;
      parameterEnabled.CheckedChanged += OnCheckChanged;
      // 
      // description
      // 
      description.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      description.Font = new Font("Segoe UI", 6.75F, FontStyle.Bold);
      description.Location = new Point(4, 61);
      description.Name = "description";
      description.Size = new Size(193, 37);
      description.TabIndex = 1;
      description.Text = "descriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescription";
      // 
      // AdvancedServerParameter
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      AutoSize = true;
      Controls.Add(description);
      Controls.Add(parameterEnabled);
      MaximumSize = new Size(200, 100);
      MinimumSize = new Size(200, 100);
      Name = "AdvancedServerParameter";
      Size = new Size(200, 100);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private CheckBox parameterEnabled;
        protected Label description;
    }
}
