/******************************************************************************
 * File Name:    ServerParameterSelect.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The ServerParameterSelect component represents a graphical
 *               means to manage choices
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.WinForms
{
  public partial class ServerParameterSelect : ServerParameter
  {
    public ServerParameterSelect()
    {
      InitializeComponent();
      underlyingControl = parameterValue;
    }

    public override object ParameterValue
    {
      get => parameterValue.Text;
      set => parameterValue.DataSource = value;
    }

    public void ParameterValueSelection(string selectionString)
    {
      if (parameterValue.Items.Contains(selectionString))
      {
        parameterValue.SelectedIndex = parameterValue.Items.IndexOf(selectionString);
      }
    }
  }
}
