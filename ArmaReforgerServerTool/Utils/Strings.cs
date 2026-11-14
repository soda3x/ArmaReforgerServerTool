/******************************************************************************
 * File Name:    Strings.cs
 * Project:      Longbow
 * Description:  Static facade over LocalisationManager. Call sites use
 *               Strings.Get(...) instead of reaching into the manager
 *               directly, in the same spirit as Utilities/JsonUtils/FlagUtils.
 ******************************************************************************/

using Longbow.Managers;

namespace Longbow.Utils
{
  /// <summary>
  /// Static entry point for resolving translation keys to text.
  /// </summary>
  internal static class Strings
  {
    /// <summary>
    /// Resolves a translation key to text in the current language.
    /// </summary>
    /// <param name="key">Translation key, e.g. "serverParam.name.tooltip" (see Constants.cs).</param>
    /// <param name="args">Named placeholder values, e.g. ("count", playerCount).</param>
    public static string Get(string key, params (string Name, object Value)[] args)
    {
      return LocalisationManager.GetInstance().Get(key, args);
    }
  }
}
