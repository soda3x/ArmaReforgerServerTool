/******************************************************************************
 * File Name:    ModDependencyManager.cs
 * Project:      Longbow
 * Description:  Resolves Arma Reforger mod dependency order by reading the
 *               __NEXT_DATA__ JSON embedded in each workshop page.
 *               Performs a topological sort so required mods load first.
 *               Auto-adds any missing dependency mods to the enabled list.
 *
 * Author:       Longbow contributors
 ******************************************************************************/

using HtmlAgilityPack;
using ReforgerServerApp.Managers;
using Serilog;
using System.Text.Json;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ReforgerServerApp
{
  internal static class ModDependencyManager
  {
    // Data extracted from one workshop page load.
    private record ModPageData(
        string Name,
        List<string> DepIds,
        Dictionary<string, string> DepIdToName,   // depId → dep display name (from same JSON)
        long SizeBytes);                           // currentVersionSize in bytes (0 if unavailable)

    /// <summary>
    /// Loads the workshop page for <paramref name="modId"/> and parses the
    /// <c>__NEXT_DATA__</c> JSON blob embedded by the Next.js SSR layer.
    /// Returns the mod's display name and its declared dependency list.
    /// </summary>
    private static ModPageData LoadAndParsePage(string modId)
    {
      string url = $"{ToolPropertiesManager.GetInstance().GetToolProperties().armaWorkshopUrl}/{modId}";
      HtmlWeb web = new();
      HtmlDocument doc = web.Load(url);

      HtmlNode? scriptNode = doc.GetElementbyId("__NEXT_DATA__");
      if (scriptNode == null)
      {
        Log.Warning("ModDependencyManager - No __NEXT_DATA__ found on page for {id}", modId);
        return new ModPageData(modId, new List<string>(), new Dictionary<string, string>(), 0);
      }

      string name = modId;
      List<string> depIds = new();
      Dictionary<string, string> depNames = new(StringComparer.OrdinalIgnoreCase);
      long sizeBytes = 0;

      try
      {
        using JsonDocument jdoc = JsonDocument.Parse(scriptNode.InnerText);
        JsonElement asset = jdoc.RootElement
            .GetProperty("props")
            .GetProperty("pageProps")
            .GetProperty("asset");

        if (asset.TryGetProperty("name", out JsonElement nameEl))
          name = nameEl.GetString() ?? modId;

        if (asset.TryGetProperty("currentVersionSize", out JsonElement sizeEl))
          sizeBytes = sizeEl.GetInt64();

        if (asset.TryGetProperty("dependencies", out JsonElement depsEl))
        {
          foreach (JsonElement dep in depsEl.EnumerateArray())
          {
            // Each entry is { asset: { id, name }, version, ... }
            if (!dep.TryGetProperty("asset", out JsonElement depAsset)) continue;
            if (!depAsset.TryGetProperty("id", out JsonElement idEl)) continue;
            string depId = (idEl.GetString() ?? string.Empty).ToUpperInvariant();
            if (string.IsNullOrEmpty(depId) ||
                depId.Equals(modId, StringComparison.OrdinalIgnoreCase)) continue;

            depIds.Add(depId);
            if (depAsset.TryGetProperty("name", out JsonElement depNameEl))
              depNames[depId] = depNameEl.GetString() ?? depId;
          }
        }
      }
      catch (Exception ex)
      {
        Log.Warning("ModDependencyManager - Failed to parse __NEXT_DATA__ for {id}: {msg}",
            modId, ex.Message);
      }

      return new ModPageData(name, depIds, depNames, sizeBytes);
    }

    /// <summary>
    /// Resolves mod load order for <paramref name="enabled"/>.
    /// <para>
    /// Phase 1 — BFS: loads each mod's workshop page and reads the embedded
    /// Next.js JSON to discover direct and transitive dependencies.
    /// Any dependency not already in the list is added automatically.
    /// </para>
    /// <para>
    /// Phase 2 — Topological sort (DFS): orders the full set so every
    /// dependency appears before the mod that requires it.
    /// </para>
    /// </summary>
    /// <param name="enabled">Current enabled mod list.</param>
    /// <param name="progress">
    /// Optional callback invoked as <c>(completed, total)</c> after each mod
    /// page is fetched so the caller can update a progress bar.
    /// </param>
    /// <returns>
    /// A tuple of (sorted list, auto-added mods, warning strings, total size in bytes).
    /// On fatal network failure the original order is returned unchanged with a warning.
    /// </returns>
    public static (List<Mod> sorted, List<Mod> added, List<string> warnings, long totalSizeBytes) ResolveDependencies(
        IList<Mod> enabled,
        Action<int, int>? progress = null)
    {
      List<Mod> added = new();
      List<string> warnings = new();
      long totalSize = 0;

      Log.Information("ModDependencyManager - Starting resolution for {count} enabled mods", enabled.Count);

      // Working set: modId (upper) → Mod
      Dictionary<string, Mod> workingSet = new(StringComparer.OrdinalIgnoreCase);
      foreach (Mod mod in enabled)
        workingSet[mod.modId.ToUpperInvariant()] = mod;

      // Dependency graph: modId → list of dep modIds
      Dictionary<string, List<string>> graph = new(StringComparer.OrdinalIgnoreCase);

      Queue<string> queue = new(workingSet.Keys);
      int processed = 0;

      try
      {
        while (queue.Count > 0)
        {
          string modId = queue.Dequeue();
          if (graph.ContainsKey(modId))
          {
            processed++;
            progress?.Invoke(processed, workingSet.Count);
            continue;
          }

          ModPageData pageData;
          try
          {
            pageData = LoadAndParsePage(modId);
          }
          catch (Exception ex)
          {
            Log.Warning("ModDependencyManager - Could not load page for {id}: {msg}", modId, ex.Message);
            warnings.Add($"Could not fetch workshop page for mod {modId}: {ex.Message}");
            graph[modId] = new List<string>();
            processed++;
            progress?.Invoke(processed, workingSet.Count);
            continue;
          }

          graph[modId] = pageData.DepIds;
          totalSize += pageData.SizeBytes;

          Log.Information("ModDependencyManager - {name} ({id}): {count} dep(s) declared: [{deps}]",
              pageData.Name, modId, pageData.DepIds.Count,
              string.Join(", ", pageData.DepIds));

          foreach (string depId in pageData.DepIds)
          {
            if (!workingSet.ContainsKey(depId))
            {
              // Name comes from the parent's JSON — no extra HTTP request needed
              string depName = pageData.DepIdToName.TryGetValue(depId, out string? n) ? n : depId;
              Mod newMod = new(depId, depName);
              workingSet[depId] = newMod;
              added.Add(newMod);
              queue.Enqueue(depId);
              Log.Information("ModDependencyManager - Auto-added missing dependency: {name} ({id})",
                  depName, depId);
            }
          }

          processed++;
          progress?.Invoke(processed, workingSet.Count);
        }
      }
      catch (Exception ex)
      {
        Log.Error("ModDependencyManager - Unexpected error during resolution: {msg}", ex.Message);
        warnings.Add($"Dependency resolution failed: {ex.Message}. Original mod order preserved.");
        progress?.Invoke(workingSet.Count, workingSet.Count);
        return (new List<Mod>(enabled), added, warnings, 0);
      }

      // Topological sort (recursive DFS)
      List<Mod> sorted = new();
      HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
      HashSet<string> inStack = new(StringComparer.OrdinalIgnoreCase);

      void Visit(string id)
      {
        if (visited.Contains(id)) return;
        if (inStack.Contains(id))
        {
          warnings.Add($"Circular dependency detected involving mod {id}. Load order may be incorrect.");
          return;
        }
        inStack.Add(id);
        if (graph.TryGetValue(id, out List<string>? deps))
          foreach (string dep in deps)
            Visit(dep);
        inStack.Remove(id);
        visited.Add(id);
        if (workingSet.TryGetValue(id, out Mod? mod))
          sorted.Add(mod);
      }

      foreach (string id in workingSet.Keys)
        Visit(id);

      Log.Information("ModDependencyManager - Resolution complete. {added} dep(s) auto-added, {warn} warning(s). Total size: {size} bytes.",
          added.Count, warnings.Count, totalSize);
      progress?.Invoke(workingSet.Count, workingSet.Count);
      return (sorted, added, warnings, totalSize);
    }
  }
}
