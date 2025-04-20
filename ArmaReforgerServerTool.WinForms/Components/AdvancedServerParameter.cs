/******************************************************************************
 * File Name:    AdvancedServerParameter.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  Represents the base class for the AdvancedServerParameter 
 *               components, providing the generalisation for concrete 
 *               implementations
 * 
 * Author:       Bradley Newman
 ******************************************************************************/


namespace ReforgerServerApp.WinForms.Components
{
  public partial class AdvancedServerParameter : UserControl
  {
    private string m_parameterProperName;

    protected Control m_underlyingControl;

    public AdvancedServerParameter()
    {
      InitializeComponent();
      m_parameterProperName = string.Empty;
    }

    public bool Checked()
    {
      return parameterEnabled.Checked;
    }

    public virtual object ParameterValue
    {
      get { throw new Exception("This method must be overridden"); }
      set { throw new Exception("This method must be overridden"); }
    }

    public string ParameterName
    {
      get => m_parameterProperName;
      set => m_parameterProperName = value;
    }

    public string ParameterFriendlyName
    {
      get => parameterEnabled.Text;
      set => parameterEnabled.Text = value;
    }

    public CheckBox CheckBox
    {
      get => parameterEnabled;
    }

    public Control UnderlyingType
    {
      get => m_underlyingControl;
    }

    public string Description
    {
      get => description.Text;
      set => description.Text = value;
    }

    /// <summary>
    /// Sets the checkbox component to be either enabled or disabled
    /// </summary>
    /// <param name="fieldEnabled"></param>
    public void SetEnabled(bool fieldEnabled)
    {
      parameterEnabled.Enabled = fieldEnabled;
      OnCheckChanged(new object(), new EventArgs());
    }

    /// <summary>
    /// Set only the field to be enabled or disabled, the intention 
    /// is that toggling the checkbox will either enable or 
    /// disable the field
    /// </summary>
    /// <param name="fieldEnabled"></param>
    public void SetFieldEnabled(bool fieldEnabled)
    {
      m_underlyingControl.Enabled = fieldEnabled;
    }

    public virtual void OnCheckChanged(object sender, EventArgs e)
    {
      throw new Exception("This method must be overridden");
    }
  }
}
