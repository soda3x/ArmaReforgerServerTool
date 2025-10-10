/******************************************************************************
 * File Name:    AdvancedServerParameterBool.cs
 * Project:      Longbow
 * Description:  The AdvancedServerParameterBool component represents a 
 *               graphical means to manage switch style server launch 
 *               arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.Components
{
  public partial class AdvancedServerParameterBool : AdvancedServerParameter
  {
    public AdvancedServerParameterBool()
    {
      InitializeComponent();
    }

    public override object ParameterValue
    {
      get => CheckBox.Checked;
      set => CheckBox.Checked = (bool)value;
    }

    override
    public void OnCheckChanged(object sender, EventArgs e)
    {
      // Do nothing, this is a boolean parameter so we only care if it's enabled or not
    }
  }
}
