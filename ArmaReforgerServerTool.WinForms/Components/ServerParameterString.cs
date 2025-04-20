/******************************************************************************
 * File Name:    ServerParameterString.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The ServerParameterString component represents a graphical
 *               means to manage string server configuration parameters
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.WinForms
{
  public partial class ServerParameterString : ServerParameter
  {
    public ServerParameterString()
    {
      InitializeComponent();
      underlyingControl = parameterValue;
    }

    public override object ParameterValue
    {
      get => parameterValue.Text;
      set => parameterValue.Text = (string)value;
    }
  }
}
