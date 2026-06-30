/******************************************************************************
 * File Name:    SteamWorkshopMetadataProvider.cs
 * Project:      Longbow
 * Description:  This file contains the SteamWorkshopMetadataProvider class,
 *               which fetches mod metadata from the Steam Workshop API.
 *               Implements caching for performance and reliability.
 *
 * Author:       Claude Code
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace ReforgerServerApp.Utils
{
  /// <summary>
  /// Provides metadata for mods by querying the Steam Workshop API and caching results.
  /// Supports parsing mod.cpp files to extract Workshop IDs and querying Steam Community.
  /// </summary>
  public static class SteamWorkshopMetadataProvider
  {
    private static readonly string CacheDirectory = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
      "Longbow"
    );
    private static readonly string CachePath = Path.Combine(CacheDirectory, "ModMetadata.json");
    private static readonly int CacheTTLDays = 7;
    private static readonly HttpClient _httpClient = new HttpClient();
    private static Dictionary<string, CachedModMetadata>? _cache;

    static SteamWorkshopMetadataProvider()
    {
      _httpClient.DefaultRequestHeaders.Add("User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    /// <summary>
    /// Container for cached metadata with timestamp
    /// </summary>
    private class CachedModMetadata
    {
      public string? ModName { get; set; }
      public ModDependency[]? Dependencies { get; set; }
      public string? Version { get; set; }
      public DateTime CachedAt { get; set; }

      public bool IsExpired => DateTime.UtcNow - CachedAt > TimeSpan.FromDays(CacheTTLDays);
    }

    /// <summary>
    /// Gets dependencies for a mod by querying Steam Workshop.
    /// Attempts to find the mod's Workshop ID from mod.cpp, then queries Steam.
    /// </summary>
    /// <param name="modId">The mod ID (e.g., "ace_core")</param>
    /// <param name="modFolder">Optional path to the mod folder to parse mod.cpp</param>
    /// <returns>An array of ModDependency objects, or empty array if lookup fails</returns>
    public static async Task<ModDependency[]> GetDependenciesAsync(string modId, string? modFolder = null)
    {
      try
      {
        // Load cache
        await LoadCacheAsync();

        // Check cache first
        if (_cache?.TryGetValue(modId.ToLower(), out var cached) ?? false)
        {
          if (!cached.IsExpired)
          {
            Log.Information($"[SteamWorkshopMetadataProvider] Found {modId} in cache (expires in {CacheTTLDays - (int)(DateTime.UtcNow - cached.CachedAt).TotalDays} days)");
            return cached.Dependencies ?? Array.Empty<ModDependency>();
          }
        }

        // Try to find Workshop ID
        string workshopId = null;
        if (!string.IsNullOrEmpty(modFolder) && Directory.Exists(modFolder))
        {
          workshopId = ExtractWorkshopIdFromModCpp(modFolder);
        }

        if (string.IsNullOrEmpty(workshopId))
        {
          Log.Warning($"[SteamWorkshopMetadataProvider] Could not find Workshop ID for mod {modId}");
          return Array.Empty<ModDependency>();
        }

        // Query Steam Workshop
        var metadata = await QuerySteamWorkshopAsync(workshopId, modId);
        if (metadata.HasValue)
        {
          // Update cache
          if (_cache == null) _cache = new Dictionary<string, CachedModMetadata>();
          _cache[modId.ToLower()] = new CachedModMetadata
          {
            ModName = metadata.Value.Name,
            Dependencies = metadata.Value.Dependencies,
            Version = metadata.Value.Version,
            CachedAt = DateTime.UtcNow
          };

          // Save cache to disk
          await SaveCacheAsync();
          return metadata.Value.Dependencies ?? Array.Empty<ModDependency>();
        }

        return Array.Empty<ModDependency>();
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"[SteamWorkshopMetadataProvider] Error fetching dependencies for mod {modId}");
        return Array.Empty<ModDependency>();
      }
    }

    /// <summary>
    /// Gets the version of a mod from Steam Workshop
    /// </summary>
    public static async Task<string?> GetModVersionAsync(string modId, string? modFolder = null)
    {
      try
      {
        await LoadCacheAsync();

        if (_cache?.TryGetValue(modId.ToLower(), out var cached) ?? false)
        {
          if (!cached.IsExpired)
          {
            return cached.Version;
          }
        }

        string workshopId = null;
        if (!string.IsNullOrEmpty(modFolder) && Directory.Exists(modFolder))
        {
          workshopId = ExtractWorkshopIdFromModCpp(modFolder);
        }

        if (string.IsNullOrEmpty(workshopId))
          return null;

        var metadata = await QuerySteamWorkshopAsync(workshopId, modId);
        return metadata.HasValue ? metadata.Value.Version : null;
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"[SteamWorkshopMetadataProvider] Error fetching version for mod {modId}");
        return null;
      }
    }

    /// <summary>
    /// Extracts the Workshop ID from a mod's mod.cpp file
    /// </summary>
    private static string? ExtractWorkshopIdFromModCpp(string modFolder)
    {
      try
      {
        string modCppPath = Path.Combine(modFolder, "mod.cpp");
        if (!File.Exists(modCppPath))
        {
          return null;
        }

        string content = File.ReadAllText(modCppPath);

        // Match patterns like: workshopID="1234567890"; or workshopID = "1234567890"
        Match match = Regex.Match(content, @"workshopID\s*=\s*[""'](\d+)[""']", RegexOptions.IgnoreCase);
        if (match.Success)
        {
          return match.Groups[1].Value;
        }

        return null;
      }
      catch (Exception ex)
      {
        Log.Warning(ex, $"[SteamWorkshopMetadataProvider] Error parsing mod.cpp from {modFolder}");
        return null;
      }
    }

    /// <summary>
    /// Queries Steam Workshop for mod metadata
    /// </summary>
    private static async Task<(string Name, ModDependency[] Dependencies, string Version)?> QuerySteamWorkshopAsync(
      string workshopId, string modId)
    {
      try
      {
        string url = $"https://steamcommunity.com/sharedfiles/filedetails/?id={workshopId}";
        Log.Information($"[SteamWorkshopMetadataProvider] Querying Steam Workshop: {url}");

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
          Log.Warning($"[SteamWorkshopMetadataProvider] Steam query failed with status {response.StatusCode}");
          return null;
        }

        string html = await response.Content.ReadAsStringAsync();

        // Extract title
        Match titleMatch = Regex.Match(html, @"<div\s+class=""headline""[^>]*>\s*([^<]+)\s*</div>");
        if (!titleMatch.Success)
        {
          titleMatch = Regex.Match(html, @"<h1\s+class=""workshopItemTitle""[^>]*>([^<]+)</h1>");
        }
        string modName = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : modId;

        // Extract update date (use as version proxy, or look for version info in description)
        string version = ExtractVersionFromMetaTags(html) ?? DateTime.Now.ToString("yyyy.MM.dd");

        // Parse dependencies from description (if documented)
        var dependencies = ParseDependenciesFromDescription(html, modId);

        return (modName, dependencies, version);
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"[SteamWorkshopMetadataProvider] Error querying Steam Workshop for {workshopId}");
        return null;
      }
    }

    /// <summary>
    /// Extracts version from Steam meta tags or page content
    /// </summary>
    private static string? ExtractVersionFromMetaTags(string html)
    {
      try
      {
        // Try to find version in meta tags or description
        Match match = Regex.Match(html, @"<meta\s+name=""description""\s+content=""[^""]*v(?:ersion)?[:\s]+([0-9.]+)[^""]*""", RegexOptions.IgnoreCase);
        if (match.Success)
        {
          return match.Groups[1].Value;
        }

        // Extract from update date if available
        match = Regex.Match(html, @"<div\s+class=""detailsStatRight""[^>]*>\s*([^<]+)\s*</div>");
        if (match.Success)
        {
          string dateStr = match.Groups[1].Value.Trim();
          // Try to parse date like "Jun 15, 2024" and convert to version format
          if (DateTime.TryParse(dateStr, out var date))
          {
            return date.ToString("yyyy.MM.dd");
          }
        }

        return null;
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Parses dependencies from the Steam Workshop description
    /// This is a basic implementation - real mods often document dependencies in description
    /// </summary>
    private static ModDependency[] ParseDependenciesFromDescription(string html, string modId)
    {
      try
      {
        // Extract description content
        Match descMatch = Regex.Match(html, @"<div\s+class=""workshopItemDescription""[^>]*>(.*?)</div>", RegexOptions.Singleline);
        if (!descMatch.Success)
        {
          return Array.Empty<ModDependency>();
        }

        string description = descMatch.Groups[1].Value;

        // Common dependency patterns in mod descriptions
        var dependencies = new List<ModDependency>();

        // Pattern: "Requires: CBA" or "Requires CBA" or similar
        MatchCollection matches = Regex.Matches(description,
          @"[Rr]equires?:?\s+(?:[Tt]he\s+)?([A-Za-z0-9\s\-&]+?)(?:\s*(?:v|version|v\.|@)[0-9.]+)?(?=[,\n<])",
          RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
          string depName = match.Groups[1].Value.Trim();
          if (!string.IsNullOrEmpty(depName))
          {
            // Convert dependency name to ID (e.g., "CBA" -> "cba")
            string depId = ConvertNameToModId(depName);
            dependencies.Add(new ModDependency(depId, depName));
          }
        }

        return dependencies.ToArray();
      }
      catch
      {
        return Array.Empty<ModDependency>();
      }
    }

    /// <summary>
    /// Converts a mod name to a likely mod ID
    /// </summary>
    private static string ConvertNameToModId(string name)
    {
      // Simple conversion: lowercase, replace spaces with underscores, remove special chars
      return Regex.Replace(name.ToLower(), @"[^\w]", "_").Replace("__", "_").TrimEnd('_');
    }

    /// <summary>
    /// Loads the cache from disk
    /// </summary>
    private static async Task LoadCacheAsync()
    {
      if (_cache != null)
        return;

      _cache = new Dictionary<string, CachedModMetadata>();

      try
      {
        if (!File.Exists(CachePath))
          return;

        string json = await File.ReadAllTextAsync(CachePath);
        var dict = JsonSerializer.Deserialize<Dictionary<string, CachedModMetadata>>(json);
        if (dict != null)
        {
          _cache = dict;
        }
      }
      catch (Exception ex)
      {
        Log.Warning(ex, "[SteamWorkshopMetadataProvider] Error loading cache from disk");
        _cache = new Dictionary<string, CachedModMetadata>();
      }
    }

    /// <summary>
    /// Saves the cache to disk
    /// </summary>
    private static async Task SaveCacheAsync()
    {
      try
      {
        if (_cache == null || _cache.Count == 0)
          return;

        // Ensure directory exists
        if (!Directory.Exists(CacheDirectory))
        {
          Directory.CreateDirectory(CacheDirectory);
        }

        // Remove expired entries before saving
        var validEntries = _cache
          .Where(kvp => !kvp.Value.IsExpired)
          .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        string json = JsonSerializer.Serialize(validEntries, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(CachePath, json);
      }
      catch (Exception ex)
      {
        Log.Warning(ex, "[SteamWorkshopMetadataProvider] Error saving cache to disk");
      }
    }

    /// <summary>
    /// Clears the in-memory cache (useful for testing)
    /// </summary>
    public static void ClearCache()
    {
      _cache = null;
    }

    /// <summary>
    /// Gets the cache directory path
    /// </summary>
    public static string GetCacheDirectory()
    {
      return CacheDirectory;
    }

    /// <summary>
    /// Gets the cache file path
    /// </summary>
    public static string GetCachePath()
    {
      return CachePath;
    }
  }
}
