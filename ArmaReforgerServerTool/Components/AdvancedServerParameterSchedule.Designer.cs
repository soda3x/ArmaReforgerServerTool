namespace ReforgerServerApp.Components
{
    partial class AdvancedServerParameterSchedule
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
            label1 = new Label();
            parameterValue = new NumericUpDown();
            timeUnit = new ComboBox();
            ((System.ComponentModel.ISupportInitialize) parameterValue).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(4, 35);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 2;
            label1.Text = "Every";
            // 
            // parameterValue
            // 
            parameterValue.Location = new Point(48, 33);
            parameterValue.Name = "parameterValue";
            parameterValue.Size = new Size(72, 23);
            parameterValue.TabIndex = 3;
            // 
            // timeUnit
            // 
            timeUnit.FormattingEnabled = true;
            timeUnit.Location = new Point(126, 33);
            timeUnit.Name = "timeUnit";
            timeUnit.Size = new Size(65, 23);
            timeUnit.TabIndex = 4;
            // 
            // AdvancedServerParameterSchedule
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(timeUnit);
            Controls.Add(parameterValue);
            Controls.Add(label1);
            Name = "AdvancedServerParameterSchedule";
            Controls.SetChildIndex(label1, 0);
            Controls.SetChildIndex(parameterValue, 0);
            Controls.SetChildIndex(timeUnit, 0);
            ((System.ComponentModel.ISupportInitialize) parameterValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown parameterValue;
        private ComboBox timeUnit;
    }
}
