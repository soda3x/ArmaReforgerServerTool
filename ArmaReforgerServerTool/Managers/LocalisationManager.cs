/******************************************************************************
 * File Name:    LocalisationManager.cs
 * Project:      Longbow
 * Description:  This file contains the singleton LocalisationManager class,
 *               responsible for loading translation packs and resolving
 *               translation keys to text for the current language.
 *
 *               Phase 1 of the i18n system (see i18n-system.md): the engine
 *               only. Nothing in the UI calls this yet.
 ******************************************************************************/

using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Longbow.Components;
using Longbow.Models;
using Serilog;

namespace Longbow.Managers
{
  /// <summary>
  /// Loads and resolves translation packs (<c>Resources/Strings/strings_&lt;code&gt;.json</c>).
  /// Mirrors the loading/fallback style of <c>ToolPropertiesManager</c>: a malformed or
  /// missing file never crashes the app, it just falls back and logs.
  /// </summary>
  internal class LocalisationManager
  {
    private static LocalisationManager? m_instance;

    private const string DEFAULT_LANGUAGE_CODE = "en";
    private static readonly string s_stringsDirectory = Path.Combine(".", "Resources", "Strings");
    private static readonly string s_embeddedResourcePrefix = $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.Strings.strings_";

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
    };

    /// <summary>
    /// English, loaded from the embedded resource. This is the fallback of last resort -
    /// if it's somehow missing from the build, <c>Get()</c> degrades to returning raw keys
    /// rather than throwing.
    /// </summary>
    private readonly LanguagePack m_fallbackPack;

    private LanguagePack m_currentPack;
    private string m_currentLanguageCode;

    /// <summary>
    /// Constructs a LocalisationManager object. Intended to only be used within the
    /// singleton <c>GetInstance()</c> method. Loads the embedded English pack as the
    /// permanent fallback, then loads it again as the initial "current" pack.
    /// </summary>
    private LocalisationManager()
    {
      m_fallbackPack = LoadEmbeddedPack(DEFAULT_LANGUAGE_CODE) ?? new LanguagePack
      {
        Meta = new LanguagePackMeta { Code = DEFAULT_LANGUAGE_CODE, Name = "English", NativeName = "English" }
      };

      if (m_fallbackPack.Strings.Count == 0)
      {
        Log.Error("LocalisationManager - embedded fallback pack '{code}' failed to load or is empty. " +
            "Translated text will fall back to raw keys until this is fixed in the build.", DEFAULT_LANGUAGE_CODE);
      }

      m_currentPack = m_fallbackPack;
      m_currentLanguageCode = DEFAULT_LANGUAGE_CODE;
    }

    public static LocalisationManager GetInstance()
    {
      m_instance ??= new LocalisationManager();
      return m_instance;
    }

    /// <summary>The BCP-47 code of the language currently loaded.</summary>
    public string CurrentLanguage => m_currentLanguageCode;

    /// <summary>
    /// Loads the given language as the current pack. On-disk packs (editable by
    /// translators without a rebuild) take precedence over embedded ones.
    /// If the language cannot be found or fails to parse, the current pack is left
    /// unchanged and a warning is logged - this never throws.
    /// </summary>
    /// <param name="code">BCP-47 language code, e.g. "en", "pt-BR".</param>
    public void LoadLanguage(string code)
    {
      LanguagePack? pack = LoadDiskPack(code) ?? LoadEmbeddedPack(code);

      if (pack is null)
      {
        Log.Warning("LocalisationManager - could not find a language pack for '{code}'. " +
            "Keeping '{current}' loaded.", code, m_currentLanguageCode);
        return;
      }

      m_currentPack = pack;
      m_currentLanguageCode = code;
      Log.Information("LocalisationManager - loaded language '{code}' ({name}).", code, pack.Meta.Name);
    }

    /// <summary>
    /// Scans both the on-disk Strings folder and the embedded resources for available
    /// language packs, returning their metadata. If the same code exists in both places,
    /// the on-disk one wins (it's what will actually be loaded).
    /// </summary>
    public List<LanguagePackMeta> AvailableLanguages()
    {
      Dictionary<string, LanguagePackMeta> found = [];

      foreach (string code in EnumerateEmbeddedCodes())
      {
        LanguagePack? pack = LoadEmbeddedPack(code);
        if (pack is not null)
        {
          found[code] = pack.Meta;
        }
      }

      foreach (string code in EnumerateDiskCodes())
      {
        LanguagePack? pack = LoadDiskPack(code);
        if (pack is not null)
        {
          found[code] = pack.Meta;
        }
      }

      return [.. found.Values.OrderBy(meta => meta.Code, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Walks a Form's control tree (mirroring ThemeManager.SyncThemeColours) and, for every
    /// control that has a TranslationKey set via <paramref name="provider"/>, resolves it
    /// and assigns .Text. Controls without a key are left exactly as the Designer set them -
    /// this is how placeholders like "0" or "12345678" stay untranslated for free.
    /// </summary>
    /// <param name="parent">The Form (or any Control) to walk.</param>
    /// <param name="provider">That Form's TranslationKeyExtenderProvider instance.</param>
    public void ApplyStrings(Control parent, TranslationKeyExtenderProvider provider)
    {
      foreach (Control control in parent.Controls)
      {
        string key = provider.GetTranslationKey(control);
        if (!string.IsNullOrEmpty(key))
        {
          control.Text = Get(key);
        }

        if (control.HasChildren)
        {
          ApplyStrings(control, provider);
        }
      }
    }

    /// <summary>
    /// Resolves a translation key to text for the currently loaded language, interpolating
    /// any named placeholders. Falls back to English, then to the raw key, if the current
    /// language doesn't have it. Never throws on a missing key - see the i18n plan for why.
    /// </summary>
    /// <param name="key">Translation key, e.g. "serverParam.name.tooltip".</param>
    /// <param name="args">Named placeholder values, e.g. ("count", 3).</param>
    public string Get(string key, params (string Name, object Value)[] args)
    {
      (string? raw, LanguagePack? sourcePack) = Resolve(key, args);

      if (raw is null || sourcePack is null)
      {
        Log.Warning("LocalisationManager - missing translation key '{key}' in '{current}' and fallback '{fallback}'.",
            key, m_currentLanguageCode, DEFAULT_LANGUAGE_CODE);
        return key;
      }

      return Interpolate(raw, args, sourcePack.Meta.Code);
    }

    /// <summary>
    /// Looks a key up in the current pack, then the fallback pack, resolving plurals
    /// along the way. Returns the pack the value was actually found in, since plural
    /// category selection depends on that pack's own plural rule.
    /// </summary>
    private (string? raw, LanguagePack? sourcePack) Resolve(string key, (string Name, object Value)[] args)
    {
      foreach (LanguagePack pack in DistinctPacks(m_currentPack, m_fallbackPack))
      {
        if (TryResolveFromPack(pack, key, args, out string value))
        {
          return (value, pack);
        }
      }
      return (null, null);
    }

    private static IEnumerable<LanguagePack> DistinctPacks(params LanguagePack[] packs)
    {
      HashSet<string> seen = [];
      foreach (LanguagePack pack in packs)
      {
        if (seen.Add(pack.Meta.Code))
        {
          yield return pack;
        }
      }
    }

    private static bool TryResolveFromPack(LanguagePack pack, string key, (string Name, object Value)[] args, out string value)
    {
      object? count = args.FirstOrDefault(a => a.Name == "count").Value;
      if (count is not null)
      {
        string category = PluralRules.SelectCategory(pack.Meta.PluralRule, Convert.ToDouble(count, CultureInfo.InvariantCulture));
        if (pack.TryGetPluralForm(key, category, out value))
        {
          return true;
        }
      }

      return pack.TryGetString(key, out value);
    }

    /// <summary>
    /// Replaces every "{name}" placeholder in <paramref name="raw"/> with its value from
    /// <paramref name="args"/>, formatted using the given language's culture where possible.
    /// Values are looked up by name, not position, so a translation is free to reorder them.
    /// </summary>
    private static string Interpolate(string raw, (string Name, object Value)[] args, string languageCode)
    {
      if (args.Length == 0)
      {
        return raw;
      }

      CultureInfo culture = GetCultureOrInvariant(languageCode);
      StringBuilder result = new(raw);

      foreach ((string name, object value) in args)
      {
        string formatted = value is IFormattable formattable
            ? formattable.ToString(null, culture)
            : value.ToString() ?? string.Empty;
        result.Replace($"{{{name}}}", formatted);
      }

      return result.ToString();
    }

    private static CultureInfo GetCultureOrInvariant(string languageCode)
    {
      try
      {
        return CultureInfo.GetCultureInfo(languageCode);
      }
      catch (CultureNotFoundException)
      {
        return CultureInfo.InvariantCulture;
      }
    }

    private static LanguagePack? LoadDiskPack(string code)
    {
      string path = Path.Combine(s_stringsDirectory, $"strings_{code}.json");
      if (!File.Exists(path))
      {
        return null;
      }

      try
      {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<LanguagePack>(json, s_jsonOptions);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "LocalisationManager - failed to parse on-disk language pack '{path}'.", path);
        return null;
      }
    }

    private static LanguagePack? LoadEmbeddedPack(string code)
    {
      string resourceName = $"{s_embeddedResourcePrefix}{code}.json";
      try
      {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
        {
          return null;
        }
        using StreamReader reader = new(stream);
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<LanguagePack>(json, s_jsonOptions);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "LocalisationManager - failed to parse embedded language pack '{resource}'.", resourceName);
        return null;
      }
    }

    private static IEnumerable<string> EnumerateDiskCodes()
    {
      if (!Directory.Exists(s_stringsDirectory))
      {
        yield break;
      }

      foreach (string path in Directory.EnumerateFiles(s_stringsDirectory, "strings_*.json"))
      {
        yield return CodeFromFileName(Path.GetFileNameWithoutExtension(path));
      }
    }

    private static IEnumerable<string> EnumerateEmbeddedCodes()
    {
      foreach (string resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
      {
        if (resourceName.StartsWith(s_embeddedResourcePrefix, StringComparison.OrdinalIgnoreCase) &&
            resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
          string code = resourceName[s_embeddedResourcePrefix.Length..^".json".Length];
          yield return code;
        }
      }
    }

    private static string CodeFromFileName(string fileName)
    {
      // "strings_pt-BR" -> "pt-BR"
      const string prefix = "strings_";
      return fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? fileName[prefix.Length..] : fileName;
    }
  }

  /// <summary>
  /// Plural category selection per language family. A translation's plural-form entries
  /// (e.g. {"one": "...", "other": "..."}) only need to cover the categories its own
  /// rule actually uses - this is what decides which category applies for a given count.
  /// See i18n-system.md section 4.2 for why this can't live in C# as a single if/else.
  /// </summary>
  internal static class PluralRules
  {
    /// <summary>English, German, Portuguese, and similar: singular only for exactly 1.</summary>
    private static string English(double n) => n == 1 ? "one" : "other";

    /// <summary>French and similar Romance-language rules: singular covers 0 and 1.</summary>
    private static string Romance(double n) => n == 0 || n == 1 ? "one" : "other";

    /// <summary>Japanese, Korean, Chinese, and similar: no grammatical plural at all.</summary>
    private static string None(double n) => "other";

    public static string SelectCategory(string pluralRule, double n)
    {
      return pluralRule switch
      {
        "romance" => Romance(n),
        "none" => None(n),
        "english" or _ => English(n),
      };
    }
  }
}
