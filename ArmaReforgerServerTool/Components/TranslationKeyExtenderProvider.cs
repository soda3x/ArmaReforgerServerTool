/******************************************************************************
 * File Name:    TranslationKeyExtenderProvider.cs
 * Project:      Longbow
 * Description:  IExtenderProvider that lets any control carry an extra
 *               "TranslationKey" property, the same way the built-in
 *               ToolTip component adds a "ToolTip on X" property to every
 *               control. Set at design time (Visual Studio's Properties
 *               panel) or in code; resolved to display text at startup by
 *               LocalisationManager.ApplyStrings().
 ******************************************************************************/

using System.ComponentModel;

namespace Longbow.Components
{
  /// <summary>
  /// Design-time/runtime extender that attaches a translation key to any Control.
  /// One instance is added per Form (drag from the Toolbox in Visual Studio, or a
  /// single line in InitializeComponent() otherwise - both produce identical code).
  /// </summary>
  [ProvideProperty("TranslationKey", typeof(Control))]
  public class TranslationKeyExtenderProvider : Component, IExtenderProvider
  {
    private readonly Dictionary<Control, string> m_keys = [];

    public TranslationKeyExtenderProvider()
    {
    }

    public TranslationKeyExtenderProvider(IContainer container)
    {
      container?.Add(this);
    }

    /// <summary>
    /// Required by IExtenderProvider. Every Control is eligible to carry a
    /// TranslationKey - there's no narrower type to restrict this to.
    /// </summary>
    public bool CanExtend(object extendee)
    {
      return extendee is Control;
    }

    /// <summary>
    /// Getter for the extended "TranslationKey" property. Named GetTranslationKey
    /// by WinForms convention so it shows up as "TranslationKey" in the Properties panel.
    /// </summary>
    [DefaultValue("")]
    public string GetTranslationKey(Control control)
    {
      return m_keys.TryGetValue(control, out string? key) ? key : string.Empty;
    }

    /// <summary>
    /// Setter for the extended "TranslationKey" property.
    /// </summary>
    public void SetTranslationKey(Control control, string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        m_keys.Remove(control);
      }
      else
      {
        m_keys[control] = key;
      }
    }

    /// <summary>
    /// All (control, key) pairs currently registered. Used by
    /// LocalisationManager.ApplyStrings() to resolve and assign .Text without
    /// this class needing to know anything about translation resolution itself.
    /// </summary>
    public IReadOnlyDictionary<Control, string> Keys => m_keys;
  }
}
