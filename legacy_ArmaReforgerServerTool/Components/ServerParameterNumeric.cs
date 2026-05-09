/******************************************************************************
 * File Name:    ServerParameterNumeric.cs
 * Project:      Longbow
 * Description:  The ServerParameterNumeric component represents a graphical
 *               means to manage numeric server configuration parameters
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  public partial class ServerParameterNumeric : ServerParameter
  {

    public ServerParameterNumeric()
    {
      InitializeComponent();
      underlyingControl = parameterValue;
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
  }
}
