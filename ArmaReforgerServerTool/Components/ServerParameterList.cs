/******************************************************************************
 * File Name:    ServerParameterSelect.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The ServerParameterSelect component represents a graphical
 *               means to manage choices
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
    public partial class ServerParameterList : ServerParameter
    {
        public ServerParameterList()
        {
            InitializeComponent();
            underlyingControl = parameterValue;
            ParameterList = Array.Empty<string>();
        }

        public string[] ParameterList { get; set; }

        public override object ParameterValue
        {
            get => ParameterList;
            set => ParameterList = (string[]) value;
        }

        private void OnButtonPressed(object sender, EventArgs e)
        {
            ListForm lf = new(ParameterFriendlyName, ParameterList);
            lf.ShowDialog();
            ParameterList = lf.GetItems();
        }

        
    }
}
