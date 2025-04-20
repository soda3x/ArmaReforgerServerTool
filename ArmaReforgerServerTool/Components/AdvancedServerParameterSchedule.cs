/******************************************************************************
 * File Name:    AdvancedServerParameterSchedule.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The AdvancedServerParameterSchedule component represents a 
 *               graphical means to manage time intervals
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Serilog;

namespace ReforgerServerApp.WinForms.Components
{
    public partial class AdvancedServerParameterSchedule : AdvancedServerParameter
    {
        public AdvancedServerParameterSchedule()
        {
            InitializeComponent();
            m_underlyingControl = parameterValue;
            SetFieldEnabled(Checked());
        }

        public override object ParameterValue 
        { 
            get => parameterValue.Value;
            set => parameterValue.Value = Convert.ToInt32(value);
        }

        public decimal ParameterMin
        {
            get => parameterValue.Minimum;
            set => parameterValue.Minimum = value;
        }

        public decimal ParameterMax
        {
            get => parameterValue.Maximum;
            set => parameterValue.Maximum = value;
        }

        public decimal ParameterIncrement
        {
            get => parameterValue.Increment;
            set => parameterValue.Increment = value;
        }

        public string CurrentItem
        {
            get => timeUnit.Text;
        }

        public int CurrentIndex
        {
            get => timeUnit.SelectedIndex;
        }

        public string[] Items
        {
            get
            {
                string[] items = new string[] { };
                foreach (object o in timeUnit.Items)
                {
                    _ = items.Append(o as string);
                }
                return items;
            }
            set
            {
                timeUnit.Items.Clear();
                timeUnit.Items.AddRange(value);
                timeUnit.SelectedIndex = 0;
            } 
        }

        public override void OnCheckChanged(object sender, EventArgs e)
        {
            if (CheckBox.Enabled)
            {
                SetFieldEnabled(Checked());
            } else
            {
                SetFieldEnabled(false);
            }
        }

        /// <summary>
        /// Because the Schedule advanced server parameter basically
        /// has two underlying controls, it has to be treated differently.
        /// 
        /// This method will set both the value and units fields to be either
        /// enabled or disabled
        /// </summary>
        /// <param name="enabled"></param>
        public new void SetFieldEnabled(bool enabled)
        {
            m_underlyingControl.Enabled = enabled;
            timeUnit.Enabled = enabled;
        }
    }
}
