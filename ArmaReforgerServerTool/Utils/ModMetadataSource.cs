/******************************************************************************
 * File Name:    ModMetadataSource.cs
 * Project:      Longbow
 * Description:  This file contains the ModMetadataSource class, a static utility
 *               for loading metadata (dependencies, versions, min game version)
 *               for well-known Arma Reforger mods.
 *
 * Author:       Bradley Newman
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace ReforgerServerApp.Utils
{
  /// <summary>
  /// Provides static metadata for well-known Arma Reforger mods.
  /// This is an MVP implementation with hardcoded mod data.
  /// Future iterations will load from a database, JSON file, or workshop API.
  /// </summary>
  public static class ModMetadataSource
  {
    /// <summary>
    /// A dictionary mapping mod IDs to their metadata (dependencies, version, min game version).
    /// This data is based on real Arma Reforger mods and their known dependency chains.
    /// </summary>
    private static readonly Dictionary<string, ModMetadata> ModMetadata = new()
    {
      {
        "cba",
        new ModMetadata(
          name: "CBA - Community Based Addons",
          version: "3.16.0",
          minGameVersion: "1.0.0",
          dependencies: new ModDependency[] { }
        )
      },
      {
        "ace_core",
        new ModMetadata(
          name: "ACE3 Core",
          version: "3.15.2",
          minGameVersion: "1.2.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("cba", "CBA - Community Based Addons", "3.15.0", null, false)
          }
        )
      },
      {
        "ace_medical",
        new ModMetadata(
          name: "ACE3 Medical",
          version: "3.15.2",
          minGameVersion: "1.2.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("ace_core", "ACE3 Core", "3.15.0", null, false),
            new ModDependency("cba", "CBA - Community Based Addons", "3.15.0", null, false)
          }
        )
      },
      {
        "rhs_usf_core",
        new ModMetadata(
          name: "RHS: USAF Core",
          version: "0.74.0",
          minGameVersion: "1.0.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("cba", "CBA - Community Based Addons", "3.15.0", null, false)
          }
        )
      },
      {
        "rhs_afrf_core",
        new ModMetadata(
          name: "RHS: AFRF Core",
          version: "0.74.0",
          minGameVersion: "1.0.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("cba", "CBA - Community Based Addons", "3.15.0", null, false)
          }
        )
      },
      {
        "enhanced_movement",
        new ModMetadata(
          name: "Enhanced Movement",
          version: "1.8.4",
          minGameVersion: "1.1.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("cba", "CBA - Community Based Addons", "3.13.0", null, false)
          }
        )
      },
      {
        "3den_enhanced",
        new ModMetadata(
          name: "3DEN Enhanced",
          version: "1.96.0",
          minGameVersion: "1.0.0",
          dependencies: new ModDependency[] { }
        )
      },
      {
        "cup_units_core",
        new ModMetadata(
          name: "CUP Units Core",
          version: "1.24.0",
          minGameVersion: "1.0.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("cba", "CBA - Community Based Addons", "3.15.0", null, false),
            new ModDependency("cup_weapons_core", "CUP Weapons Core", "1.24.0", null, true) // soft dependency
          }
        )
      },
      {
        "cup_weapons_core",
        new ModMetadata(
          name: "CUP Weapons Core",
          version: "1.24.0",
          minGameVersion: "1.0.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("cba", "CBA - Community Based Addons", "3.15.0", null, false)
          }
        )
      },
      {
        "ace_rhs_compat",
        new ModMetadata(
          name: "ACE3 + RHS Compatibility",
          version: "3.15.2",
          minGameVersion: "1.2.0",
          dependencies: new ModDependency[]
          {
            new ModDependency("ace_core", "ACE3 Core", "3.15.0", null, false),
            new ModDependency("rhs_usf_core", "RHS: USAF Core", "0.70.0", null, false)
          }
        )
      }
    };

    /// <summary>
    /// Gets the dependencies for a given mod ID.
    /// Synchronous wrapper that calls GetDependenciesAsync and waits for completion.
    /// </summary>
    /// <param name="modId">The mod ID to look up (e.g., "ace_core")</param>
    /// <param name="modFolder">Optional path to the mod folder for Steam Workshop lookup</param>
    /// <returns>An array of ModDependency objects. Empty array if mod not found or has no dependencies.</returns>
    public static ModDependency[] GetDependencies(string modId, string? modFolder = null)
    {
      try
      {
        return GetDependenciesAsync(modId, modFolder).Result;
      }
      catch (Exception ex)
      {
        Log.Warning(ex, $"[ModMetadataSource] Error getting dependencies for {modId}");
        return Array.Empty<ModDependency>();
      }
    }

    /// <summary>
    /// Gets the dependencies for a given mod ID asynchronously.
    /// First checks hardcoded metadata (fast path), then queries Steam Workshop if not found.
    /// </summary>
    /// <param name="modId">The mod ID to look up (e.g., "ace_core")</param>
    /// <param name="modFolder">Optional path to the mod folder for Steam Workshop lookup</param>
    /// <returns>An array of ModDependency objects. Empty array if mod not found or has no dependencies.</returns>
    public static async Task<ModDependency[]> GetDependenciesAsync(string modId, string? modFolder = null)
    {
      if (string.IsNullOrWhiteSpace(modId))
      {
        return Array.Empty<ModDependency>();
      }

      // Fast path: check hardcoded metadata first
      if (ModMetadata.TryGetValue(modId.ToLower(), out var metadata))
      {
        return metadata.Dependencies ?? Array.Empty<ModDependency>();
      }

      // Fallback: query Steam Workshop
      Log.Information($"[ModMetadataSource] Mod {modId} not in hardcoded metadata, querying Steam Workshop...");
      var steamDependencies = await SteamWorkshopMetadataProvider.GetDependenciesAsync(modId, modFolder);
      return steamDependencies;
    }

    /// <summary>
    /// Gets the version of a given mod.
    /// Synchronous wrapper that calls GetModVersionAsync and waits for completion.
    /// </summary>
    /// <param name="modId">The mod ID to look up</param>
    /// <param name="modFolder">Optional path to the mod folder for Steam Workshop lookup</param>
    /// <returns>The version string (e.g., "3.15.2"), or null if mod not found</returns>
    public static string? GetModVersion(string modId, string? modFolder = null)
    {
      try
      {
        return GetModVersionAsync(modId, modFolder).Result;
      }
      catch (Exception ex)
      {
        Log.Warning(ex, $"[ModMetadataSource] Error getting version for {modId}");
        return null;
      }
    }

    /// <summary>
    /// Gets the version of a given mod asynchronously.
    /// First checks hardcoded metadata, then queries Steam Workshop if not found.
    /// </summary>
    /// <param name="modId">The mod ID to look up</param>
    /// <param name="modFolder">Optional path to the mod folder for Steam Workshop lookup</param>
    /// <returns>The version string (e.g., "3.15.2"), or null if mod not found</returns>
    public static async Task<string?> GetModVersionAsync(string modId, string? modFolder = null)
    {
      if (string.IsNullOrWhiteSpace(modId))
      {
        return null;
      }

      // Fast path: check hardcoded metadata first
      if (ModMetadata.TryGetValue(modId.ToLower(), out var metadata))
      {
        return metadata.Version;
      }

      // Fallback: query Steam Workshop
      Log.Information($"[ModMetadataSource] Mod {modId} version not in hardcoded metadata, querying Steam Workshop...");
      return await SteamWorkshopMetadataProvider.GetModVersionAsync(modId, modFolder);
    }

    /// <summary>
    /// Gets the minimum game version required for a mod.
    /// </summary>
    /// <param name="modId">The mod ID to look up</param>
    /// <returns>The minimum game version (e.g., "1.2.0"), or null if mod not found</returns>
    public static string GetMinGameVersion(string modId)
    {
      if (string.IsNullOrWhiteSpace(modId))
      {
        return null;
      }

      if (ModMetadata.TryGetValue(modId.ToLower(), out var metadata))
      {
        return metadata.MinGameVersion;
      }

      return null;
    }

    /// <summary>
    /// Gets the human-readable name of a mod.
    /// </summary>
    /// <param name="modId">The mod ID to look up</param>
    /// <returns>The mod name, or null if mod not found</returns>
    public static string GetModName(string modId)
    {
      if (string.IsNullOrWhiteSpace(modId))
      {
        return null;
      }

      if (ModMetadata.TryGetValue(modId.ToLower(), out var metadata))
      {
        return metadata.Name;
      }

      return null;
    }

    /// <summary>
    /// Checks whether a mod ID exists in the metadata source.
    /// </summary>
    /// <param name="modId">The mod ID to check</param>
    /// <returns>True if the mod exists in metadata, false otherwise</returns>
    public static bool HasMetadata(string modId)
    {
      if (string.IsNullOrWhiteSpace(modId))
      {
        return false;
      }

      return ModMetadata.ContainsKey(modId.ToLower());
    }

    /// <summary>
    /// Gets all mod IDs that are currently in the metadata source.
    /// </summary>
    /// <returns>An enumerable of mod IDs</returns>
    public static IEnumerable<string> GetAllModIds()
    {
      return ModMetadata.Keys.AsEnumerable();
    }

  }

  /// <summary>
  /// Internal metadata container for a mod (name, version, min game version, dependencies).
  /// </summary>
  internal class ModMetadata
  {
    public string Name { get; set; }
    public string Version { get; set; }
    public string MinGameVersion { get; set; }
    public ModDependency[] Dependencies { get; set; }

    public ModMetadata(string name, string version, string minGameVersion, ModDependency[] dependencies)
    {
      this.Name = name;
      this.Version = version;
      this.MinGameVersion = minGameVersion;
      this.Dependencies = dependencies;
    }
  }
}
