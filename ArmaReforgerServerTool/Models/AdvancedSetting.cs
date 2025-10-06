/******************************************************************************
 * File Name:    AdvancedSetting.cs
 * Project:      Longbow
 * Description:  This file contains the AdvancedSetting class which is a model
 *               representing the saved / loaded state of an advanced setting
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using System.Text.Json.Serialization;
using static ReforgerServerApp.Utils.JsonUtils;

namespace Longbow.Models
{
  [JsonConverter(typeof(AdvancedSettingConverter))]
  internal class AdvancedSetting
  {
    public string Name { get; set; }
    
    public object Value { get; set; }
    public bool Enabled { get; set; }

    public AdvancedSetting()
    {
      // Used for JSON deserialising
    }

    /// <summary>
    /// Constructs and advanced setting
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="enabled"></param>
    public AdvancedSetting(string name, object value, bool enabled)
    {
      Name = name;
      Value = value;
      Enabled = enabled;
    }

    /// <summary>
    /// Constructs a <c>switch</c> style (i.e. no value) advanced setting
    /// </summary>
    /// <param name="name"></param>
    /// <param name="enabled"></param>
    public AdvancedSetting(string name, bool enabled)
    {
      Name = name;
      Value = "switch";
      Enabled = enabled;
    }

    /// <summary>
    /// The string literal <c>switch</c> is a reserved word for being
    /// a switch-type advanced setting
    /// </summary>
    /// <returns>True if advanced setting is to be treated as a switch,
    /// False if advanced setting has a real value</returns>
    public bool IsSwitch()
    {
      return Value.Equals("switch");
    }
  }
}
