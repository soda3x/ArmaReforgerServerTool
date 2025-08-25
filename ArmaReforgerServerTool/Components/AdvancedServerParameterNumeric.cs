/******************************************************************************
 * File Name:    AdvancedServerParameterNumeric.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  The AdvancedServerParameterNumeric component represents a 
 *               graphical means to manage numeric server launch 
 *               arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using static System.Windows.Forms.DataFormats;

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
      set => parameterValue.Value = (Int32)value;
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

    private int m_parameterPadding;
    public int ParameterPadding
    {
      get => m_parameterPadding;
      set
      {
        m_parameterPadding = value;
        string format = $"D{ParameterPadding}";
        parameterValue.Text = Convert.ToInt32(parameterValue.Value).ToString(format);
      }
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

    private void OnValueChanged(object sender, EventArgs e)
    {
      string format = $"D{ParameterPadding}";
      parameterValue.Text = Convert.ToInt32(parameterValue.Value).ToString(format);
    }
  }
}
