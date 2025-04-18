/******************************************************************************
 * File Name:    ServerParameterBool.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The ServerParameterBool component represents a graphical
 *               means to manage boolean server configuration parameters
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
    public partial class ServerParameterBool : ServerParameter
    {
        private ServerParameter associatedControl;

        public ServerParameterBool()
        {
            InitializeComponent();
            underlyingControl = parameterValue;
        }

        public override object ParameterValue
        {
            get => parameterValue.Checked;
            set => parameterValue.Checked = (bool)value;
        }

        public ServerParameter AssociatedControl
        {
            get => associatedControl;
            set => associatedControl = value;
        }

        public void OnCheckChanged(object sender, EventArgs e)
        {
            if (associatedControl != null)
            {
                associatedControl.SetFieldEnabled((bool)ParameterValue);
            }
        }
    }
}
