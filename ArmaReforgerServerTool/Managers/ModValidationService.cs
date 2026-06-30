/******************************************************************************
 * File Name:    ModValidationService.cs
 * Project:      Longbow
 * Description:  This file contains the ModValidationService class, a singleton
 *               that validates mod configurations for dependency issues, version
 *               mismatches, circular dependencies, and conflicts.
 *
 * Author:       Bradley Newman
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ReforgerServerApp.Utils;
using Serilog;

namespace ReforgerServerApp.Managers
{
  /// <summary>
  /// Singleton service for validating mod configurations.
  /// Detects missing dependencies, version mismatches, circular dependencies, and conflicts.
  /// </summary>
  public class ModValidationService
  {
    private static ModValidationService? _instance;

    /// <summary>
    /// Gets or creates the singleton instance
    /// </summary>
    public static ModValidationService GetInstance()
    {
      _instance ??= new ModValidationService();
      return _instance;
    }

    private ModValidationService() { }

    /// <summary>
    /// Validates a list of enabled mods for dependency issues and returns a sorted load order.
    /// Three-pass validation:
    ///   Pass 1: Check that all required dependencies are present
    ///   Pass 2: Check version constraints
    ///   Pass 3: Detect circular dependencies
    ///   Pass 4: Topological sort for load order
    /// </summary>
    /// <param name="enabledMods">List of mods to validate</param>
    /// <param name="gameVersion">The game version (e.g., "1.6.0.119"), defaults to "1.6.0.119"</param>
    /// <returns>A ValidationResult containing errors, warnings, and sorted mod list</returns>
    public ValidationResult ValidateMods(List<Mod> enabledMods, string gameVersion = "1.6.0.119")
    {
      var result = new ValidationResult();

      if (enabledMods == null || enabledMods.Count == 0)
      {
        Log.Debug("ModValidationService - No mods to validate");
        result.IsValid = true;
        result.SortedMods = new List<Mod>();
        return result;
      }

      Log.Debug("ModValidationService - Starting validation of {ModCount} mods", enabledMods.Count);

      // Build a lookup dictionary for quick access to mods by ID
      var modDict = enabledMods.ToDictionary(m => m.modId.ToLower(), m => m);

      // Pass 1: Check for missing required dependencies
      Pass1_CheckDependencyPresence(enabledMods, modDict, result);

      // Pass 2: Check version constraints
      Pass2_CheckVersionConstraints(enabledMods, modDict, result);

      // Pass 3: Check for circular dependencies
      Pass3_DetectCircularDependencies(enabledMods, modDict, result);

      // Pass 4: Topological sort (only if no fatal errors)
      var fatalErrors = result.GetFatalErrors();
      if (!fatalErrors.Any())
      {
        var sortedMods = TopologicalSort(enabledMods, modDict);
        result.SortedMods = sortedMods;
        result.IsValid = true;

        Log.Debug("ModValidationService - Validation passed. Load order: {LoadOrder}",
          string.Join(" -> ", sortedMods.Select(m => m.modId)));
      }
      else
      {
        result.IsValid = false;
        result.SortedMods = new List<Mod>();

        Log.Warning("ModValidationService - Validation failed with {ErrorCount} fatal errors",
          fatalErrors.Count);
      }

      // Ensure Warnings list is populated from the Errors with WARNING/INFO level
      result.Warnings = result.Errors
        .Where(e => e.Level == ValidationError.Severity.WARNING || e.Level == ValidationError.Severity.INFO)
        .ToList();

      result.ValidatedAt = DateTime.UtcNow;
      return result;
    }

    /// <summary>
    /// Pass 1: Check that all required dependencies are present in enabledMods
    /// </summary>
    private void Pass1_CheckDependencyPresence(List<Mod> enabledMods, Dictionary<string, Mod> modDict, ValidationResult result)
    {
      foreach (var mod in enabledMods)
      {
        var dependencies = ModMetadataSource.GetDependencies(mod.modId);
        if (dependencies == null || dependencies.Length == 0)
          continue;

        foreach (var dep in dependencies)
        {
          var depFound = modDict.ContainsKey(dep.ModId.ToLower());

          if (!depFound)
          {
            if (!dep.IsOptional)
            {
              // Required dependency is missing - FATAL
              var error = new ValidationError(
                ValidationError.ErrorType.MissingDependency,
                mod.modId,
                mod.name,
                $"Requires dependency '{dep.ModName}' ({dep.ModId}), but it is not enabled",
                ValidationError.Severity.FATAL,
                dep.ModId
              );
              result.Errors.Add(error);
              Log.Warning("ModValidationService - Missing required dependency: {Mod} requires {Dep}",
                mod.modId, dep.ModId);
            }
            else
            {
              // Optional dependency is missing - INFO
              var error = new ValidationError(
                ValidationError.ErrorType.MissingDependency,
                mod.modId,
                mod.name,
                $"Optionally uses '{dep.ModName}' ({dep.ModId}), but it is not enabled",
                ValidationError.Severity.INFO,
                dep.ModId
              );
              result.Errors.Add(error);
              Log.Debug("ModValidationService - Missing optional dependency: {Mod} optionally uses {Dep}",
                mod.modId, dep.ModId);
            }
          }
        }
      }
    }

    /// <summary>
    /// Pass 2: Check version constraints for dependencies that are present
    /// </summary>
    private void Pass2_CheckVersionConstraints(List<Mod> enabledMods, Dictionary<string, Mod> modDict, ValidationResult result)
    {
      foreach (var mod in enabledMods)
      {
        var dependencies = ModMetadataSource.GetDependencies(mod.modId);
        if (dependencies == null || dependencies.Length == 0)
          continue;

        foreach (var dep in dependencies)
        {
          if (!modDict.TryGetValue(dep.ModId.ToLower(), out var depMod))
            continue; // Already handled in Pass 1

          // Get the dependency mod's actual version
          var depVersion = depMod.ModVersion ?? ModMetadataSource.GetModVersion(dep.ModId);
          if (string.IsNullOrWhiteSpace(depVersion))
          {
            // Can't check version if we don't know the installed version
            Log.Debug("ModValidationService - Cannot check version for {Dep}, unknown installed version",
              dep.ModId);
            continue;
          }

          // Check if installed version satisfies constraints
          if (!CheckVersionConstraint(depVersion, dep.MinVersion, dep.MaxVersion))
          {
            var error = new ValidationError(
              ValidationError.ErrorType.VersionMismatch,
              mod.modId,
              mod.name,
              $"Requires '{dep.ModName}' version {FormatVersionConstraint(dep.MinVersion, dep.MaxVersion)}, " +
              $"but found v{depVersion}",
              ValidationError.Severity.WARNING,
              dep.ModId
            );
            result.Errors.Add(error);
            Log.Warning("ModValidationService - Version mismatch: {Mod} needs {Dep} {MinVer}+, found v{DepVer}",
              mod.modId, dep.ModId, dep.MinVersion, depVersion);
          }
        }
      }
    }

    /// <summary>
    /// Pass 3: Detect circular dependencies using DFS
    /// </summary>
    private void Pass3_DetectCircularDependencies(List<Mod> enabledMods, Dictionary<string, Mod> modDict, ValidationResult result)
    {
      var visited = new HashSet<string>();

      foreach (var mod in enabledMods)
      {
        if (!visited.Contains(mod.modId.ToLower()))
        {
          var path = new HashSet<string>();
          if (HasCircularDependency(mod.modId.ToLower(), visited, path, modDict))
          {
            var error = new ValidationError(
              ValidationError.ErrorType.CircularDependency,
              mod.modId,
              mod.name,
              $"Circular dependency detected in mod chain",
              ValidationError.Severity.FATAL
            );
            result.Errors.Add(error);
            Log.Error("ModValidationService - Circular dependency detected: {Mod}", mod.modId);
          }
        }
      }
    }

    /// <summary>
    /// DFS-based cycle detection using memoization
    /// </summary>
    private bool HasCircularDependency(string modId, HashSet<string> visited, HashSet<string> path, Dictionary<string, Mod> modDict)
    {
      if (path.Contains(modId))
        return true; // Found a cycle

      if (visited.Contains(modId))
        return false; // Already checked, no cycle

      // Get the dependencies for this mod
      var dependencies = ModMetadataSource.GetDependencies(modId);
      if (dependencies == null || dependencies.Length == 0)
      {
        visited.Add(modId);
        return false; // No dependencies, no cycle possible
      }

      path.Add(modId);

      foreach (var dep in dependencies)
      {
        var depId = dep.ModId.ToLower();
        if (!modDict.ContainsKey(depId))
          continue; // Dependency not in enabled mods, skip

        if (HasCircularDependency(depId, visited, path, modDict))
        {
          path.Remove(modId);
          return true;
        }
      }

      path.Remove(modId);
      visited.Add(modId);
      return false;
    }

    /// <summary>
    /// Topological sort using Kahn's algorithm (dependency-first order)
    /// </summary>
    private List<Mod> TopologicalSort(List<Mod> enabledMods, Dictionary<string, Mod> modDict)
    {
      // Build in-degree map and adjacency list
      var inDegree = new Dictionary<string, int>();
      var adjacency = new Dictionary<string, List<string>>();

      // Initialize all mods
      foreach (var mod in enabledMods)
      {
        var modIdLower = mod.modId.ToLower();
        if (!inDegree.ContainsKey(modIdLower))
        {
          inDegree[modIdLower] = 0;
          adjacency[modIdLower] = new List<string>();
        }
      }

      // Build the graph (edges go from dependency to dependent)
      foreach (var mod in enabledMods)
      {
        var modIdLower = mod.modId.ToLower();
        var dependencies = ModMetadataSource.GetDependencies(mod.modId);

        if (dependencies != null)
        {
          foreach (var dep in dependencies)
          {
            var depIdLower = dep.ModId.ToLower();
            if (modDict.ContainsKey(depIdLower))
            {
              // Edge from dependency to mod (dependency loads first)
              adjacency[depIdLower].Add(modIdLower);
              inDegree[modIdLower]++;
            }
          }
        }
      }

      // Kahn's algorithm
      var queue = new Queue<string>();
      foreach (var mod in enabledMods)
      {
        var modIdLower = mod.modId.ToLower();
        if (inDegree[modIdLower] == 0)
        {
          queue.Enqueue(modIdLower);
        }
      }

      var sorted = new List<Mod>();
      while (queue.Count > 0)
      {
        var current = queue.Dequeue();
        var mod = modDict[current];
        sorted.Add(mod);

        // Process neighbors
        foreach (var neighbor in adjacency[current])
        {
          inDegree[neighbor]--;
          if (inDegree[neighbor] == 0)
          {
            queue.Enqueue(neighbor);
          }
        }
      }

      return sorted;
    }

    /// <summary>
    /// Check if installedVersion satisfies the version constraint (minVersion, maxVersion)
    /// Uses semantic versioning comparison
    /// </summary>
    private bool CheckVersionConstraint(string installedVersion, string minVersion, string maxVersion)
    {
      if (string.IsNullOrWhiteSpace(installedVersion))
        return false;

      // Check minimum version
      if (!string.IsNullOrWhiteSpace(minVersion) && minVersion != "any")
      {
        if (CompareVersions(installedVersion, minVersion) < 0)
          return false; // Installed < minimum
      }

      // Check maximum version
      if (!string.IsNullOrWhiteSpace(maxVersion) && maxVersion != "any")
      {
        if (CompareVersions(installedVersion, maxVersion) > 0)
          return false; // Installed > maximum
      }

      return true;
    }

    /// <summary>
    /// Compare two semantic versions
    /// Returns: -1 if v1 < v2, 0 if v1 == v2, 1 if v1 > v2
    /// </summary>
    private int CompareVersions(string v1, string v2)
    {
      if (v1 == v2)
        return 0;

      var parts1 = v1.Split('.');
      var parts2 = v2.Split('.');

      int maxLength = Math.Max(parts1.Length, parts2.Length);

      for (int i = 0; i < maxLength; i++)
      {
        int num1 = i < parts1.Length && int.TryParse(parts1[i], out int n1) ? n1 : 0;
        int num2 = i < parts2.Length && int.TryParse(parts2[i], out int n2) ? n2 : 0;

        if (num1 < num2)
          return -1;
        if (num1 > num2)
          return 1;
      }

      return 0;
    }

    /// <summary>
    /// Format version constraints for display (e.g., "3.15.0+" or "1.0.0 - 2.0.0")
    /// </summary>
    private string FormatVersionConstraint(string minVersion, string maxVersion)
    {
      if (string.IsNullOrWhiteSpace(minVersion) && string.IsNullOrWhiteSpace(maxVersion))
        return "any";

      if (!string.IsNullOrWhiteSpace(minVersion) && string.IsNullOrWhiteSpace(maxVersion))
        return $"{minVersion}+";

      if (string.IsNullOrWhiteSpace(minVersion) && !string.IsNullOrWhiteSpace(maxVersion))
        return $"up to {maxVersion}";

      return $"{minVersion} to {maxVersion}";
    }
  }
}
