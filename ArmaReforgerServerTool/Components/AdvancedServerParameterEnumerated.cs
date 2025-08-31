/******************************************************************************
 * File Name:    AdvancedServerParameterEnumerated.cs
 * Project:      Longbow
 * Description:  The AdvancedServerParameterEnumerated component represents a 
 *               graphical means to manage enumerated server launch 
 *               arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.Components
{
  public partial class AdvancedServerParameterEnumerated : AdvancedServerParameter
  {
    public AdvancedServerParameterEnumerated()
    {
      InitializeComponent();
      m_underlyingControl = parameterValue;
      SetFieldEnabled(Checked());
    }

    public override object ParameterValue
    {
      get => parameterValue;
      set => parameterValue = (ComboBox) value;
    }

    public List<string> ParameterAvailableValues
    {
      get => parameterValue.Items.Cast<string>().ToList();
      set
      {
        // Clear any previous values
        parameterValue.Items.Clear();
        foreach (string s in value)
        {
          parameterValue.Items.Add(s);
        }
        // Set default to the first item in the collection
        parameterValue.SelectedIndex = 0;
      }

    }

    public string SelectedItem
    {
      get => (string) parameterValue.SelectedItem; // Safe to cast to string as we only allow string values
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
