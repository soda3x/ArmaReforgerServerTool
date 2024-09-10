/******************************************************************************
 * File Name:    AdvancedServerParameterString.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The AdvancedServerParameterString component represents a 
 *               graphical means to manage string server launch 
 *               arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.Components
{
    public partial class AdvancedServerParameterString : AdvancedServerParameter
    {
        public AdvancedServerParameterString()
        {
            InitializeComponent();
            m_underlyingControl = parameterValue;
            SetFieldEnabled(Checked());
        }

        public override object ParameterValue
        {
            get => parameterValue.Text;
            set => parameterValue.Text = (string) value;
        }

        public string ParameterPlaceholder
        {
            get => parameterValue.PlaceholderText;
            set => parameterValue.PlaceholderText = value;
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
    }
}
