/******************************************************************************
 * File Name:    AdvancedServerParameterNumeric.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The AdvancedServerParameterNumeric component represents a 
 *               graphical means to manage numeric server launch 
 *               arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.Components
{
    public partial class AdvancedServerParameterNumeric : AdvancedServerParameter
    {
        public AdvancedServerParameterNumeric()
        {
            InitializeComponent();
            m_underlyingControl = parameterValue;
            SetFieldEnabled(Checked());
        }

        public override object ParameterValue
        {
            get => parameterValue.Value;
            set => parameterValue.Value = (Int32) value;
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
