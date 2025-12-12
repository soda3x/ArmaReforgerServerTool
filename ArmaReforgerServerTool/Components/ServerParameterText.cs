/******************************************************************************
 * File Name:    ServerParameterText.cs
 * Project:      Longbow
 * Description:  The ServerParameterText component represents long text input
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  public partial class ServerParameterText : ServerParameter
  {
    public ServerParameterText()
    {
      InitializeComponent();
      underlyingControl = parameterValue;
      ParameterText = String.Empty;
    }

    public string ParameterText { get; set; }

    public override object ParameterValue
    {
      get => ParameterText;
      set => ParameterText = (string) value;
    }

    private void OnButtonPressed(object sender, EventArgs e)
    {
      TextInputForm tif = new($"Edit {ParameterFriendlyName}", ParameterText);
      tif.ShowDialog();
      ParameterText = tif.GetText();
    }


  }
}
