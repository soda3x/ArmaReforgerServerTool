/******************************************************************************
 * File Name:    AdvancedServerParameterTime.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The AdvancedServerParameterTime component represents a 
 *               graphical means to manage numeric server launch 
 *               arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using static System.Windows.Forms.DataFormats;

namespace ReforgerServerApp.WinForms.Components
{
    public partial class AdvancedServerParameterTime : AdvancedServerParameter
    {
        public AdvancedServerParameterTime()
        {
            InitializeComponent();
            m_underlyingControl = parameterValue;
            SetFieldEnabled(Checked());
        }

        public override object ParameterValue
        {
            get => parameterValue.Value;
            set => parameterValue.Value = Convert.ToDateTime(value);
        }

        public DateTime ParameterMin
        {
            get => parameterValue.MinDate;
            set => parameterValue.MinDate = value;
        }

        public DateTime ParameterMax
        {
            get => parameterValue.MaxDate;
            set => parameterValue.MaxDate = value;
        }

        public override void OnCheckChanged(object sender, EventArgs e)
        {
            if (CheckBox.Enabled)
            {
                SetFieldEnabled(Checked());
            }
            else
            {
                SetFieldEnabled(false);
            }
        }
    }
}
