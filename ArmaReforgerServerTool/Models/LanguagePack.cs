/******************************************************************************
 * File Name:    LanguagePack.cs
 * Project:      Longbow
 * Description:  This file contains the models representing a single loaded
 *               translation file (Resources/Strings/strings_<code>.json).
 ******************************************************************************/

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Longbow.Models
{
  /// <summary>
  /// Metadata describing a language pack: who it's for, who wrote it, and
  /// which plural rule its "strings" entries follow.
  /// </summary>
  internal class LanguagePackMeta
  {
    /// <summary>BCP-47 language code, e.g. "en", "pt-BR", "de".</summary>
    public string Code { get; set; } = "en";

    /// <summary>English name of the language, e.g. "Portuguese (Brazil)".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Name of the language written in itself, e.g. "Português (Brasil)".</summary>
    public string NativeName { get; set; } = string.Empty;

    /// <summary>
    /// Which plural rule this pack's plural-form entries follow.
    /// See <c>PluralRules</c> in <c>LocalisationManager</c> for the supported values.
    /// </summary>
    public string PluralRule { get; set; } = "english";

    /// <summary>Names/handles of people who contributed to this translation.</summary>
    public List<string> Contributors { get; set; } = [];

    /// <summary>Longbow version this pack was last translated against.</summary>
    public string BasedOnVersion { get; set; } = string.Empty;
  }

  /// <summary>
  /// Represents a single loaded translation file. A pack's "strings" values are
  /// either a plain string, or an object of plural categories (e.g. "one"/"other")
  /// keyed by the categories that pack's <see cref="LanguagePackMeta.PluralRule"/> uses.
  /// Held as <see cref="JsonElement"/> since the shape is resolved by the caller.
  /// </summary>
  internal class LanguagePack
  {
    public LanguagePackMeta Meta { get; set; } = new();

    [JsonPropertyName("strings")]
    public Dictionary<string, JsonElement> Strings { get; set; } = [];

    /// <summary>
    /// Resolves a plain-string entry.
    /// </summary>
    /// <returns>True if the key exists and is a plain string.</returns>
    public bool TryGetString(string key, out string value)
    {
      value = string.Empty;
      if (!Strings.TryGetValue(key, out JsonElement element))
      {
        return false;
      }
      if (element.ValueKind != JsonValueKind.String)
      {
        return false;
      }
      value = element.GetString() ?? string.Empty;
      return true;
    }

    /// <summary>
    /// Resolves a plural-form entry for a specific category (e.g. "one", "other").
    /// Falls back to the "other" category if the requested category isn't present,
    /// since "other" is required by every plural rule this project supports.
    /// </summary>
    /// <returns>True if the key exists and is a plural-form object.</returns>
    public bool TryGetPluralForm(string key, string category, out string value)
    {
      value = string.Empty;
      if (!Strings.TryGetValue(key, out JsonElement element))
      {
        return false;
      }
      if (element.ValueKind != JsonValueKind.Object)
      {
        return false;
      }
      if (element.TryGetProperty(category, out JsonElement categoryElement) ||
          element.TryGetProperty("other", out categoryElement))
      {
        value = categoryElement.GetString() ?? string.Empty;
        return true;
      }
      return false;
    }

    /// <summary>True if this pack's "strings" has an entry (of any shape) for the given key.</summary>
    public bool HasKey(string key)
    {
      return Strings.ContainsKey(key);
    }
  }
}
