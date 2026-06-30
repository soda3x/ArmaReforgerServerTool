/******************************************************************************
 * File Name:    SitrepConfigService.cs
 * Project:      Longbow
 * Description:  Service for communicating with Sitrep's config/mod APIs
 *               Handles loading/saving configs and retrieving mod list
 *
 * Author:       Claude Code
 ******************************************************************************/

using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ReforgerServerApp.Models;

namespace ReforgerServerApp.Services
{
  /// <summary>
  /// Service for interacting with Sitrep's REST APIs
  /// Manages config loading/saving and mod list retrieval
  /// </summary>
  public class SitrepConfigService
  {
    private readonly HttpClient _httpClient;
    private readonly string _sitrepBaseUrl;
    private static SitrepConfigService? _instance;

    public static SitrepConfigService GetInstance()
    {
      _instance ??= new SitrepConfigService();
      return _instance;
    }

    /// <summary>
    /// Initialize with Sitrep base URL (default localhost:3000)
    /// </summary>
    public SitrepConfigService(string sitrepBaseUrl = "http://localhost:3000")
    {
      _sitrepBaseUrl = sitrepBaseUrl.TrimEnd('/');
      _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    /// <summary>
    /// Fetch list of scenarios (saved configs) from Sitrep
    /// GET /api/scenarios
    /// </summary>
    public async Task<List<ScenarioInfo>> GetScenarios()
    {
      try
      {
        Log.Debug("SitrepConfigService - Fetching scenarios from {Url}",
          $"{_sitrepBaseUrl}/api/scenarios");

        var response = await _httpClient.GetAsync($"{_sitrepBaseUrl}/api/scenarios");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var scenarios = JsonSerializer.Deserialize<List<ScenarioInfo>>(json,
          new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Log.Information("SitrepConfigService - Retrieved {Count} scenarios",
          scenarios?.Count ?? 0);

        return scenarios ?? new List<ScenarioInfo>();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "SitrepConfigService - Failed to fetch scenarios");
        return new List<ScenarioInfo>();
      }
    }

    /// <summary>
    /// Load a specific scenario config by folder and name
    /// GET /api/scenarios/:folder/:name
    /// </summary>
    public async Task<ScenarioData?> GetScenario(string folder, string name)
    {
      try
      {
        Log.Debug("SitrepConfigService - Fetching scenario {Folder}/{Name}",
          folder, name);

        var response = await _httpClient.GetAsync(
          $"{_sitrepBaseUrl}/api/scenarios/{Uri.EscapeDataString(folder)}/{Uri.EscapeDataString(name)}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var scenario = JsonSerializer.Deserialize<ScenarioData>(json,
          new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Log.Information("SitrepConfigService - Retrieved scenario {Folder}/{Name}",
          folder, name);

        return scenario;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "SitrepConfigService - Failed to fetch scenario {Folder}/{Name}",
          folder, name);
        return null;
      }
    }

    /// <summary>
    /// Save/update a scenario config
    /// PUT /api/scenarios/:folder/:name
    /// </summary>
    public async Task<bool> SaveScenario(string folder, string name, ScenarioData data)
    {
      try
      {
        Log.Debug("SitrepConfigService - Saving scenario {Folder}/{Name}",
          folder, name);

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(
          $"{_sitrepBaseUrl}/api/scenarios/{Uri.EscapeDataString(folder)}/{Uri.EscapeDataString(name)}",
          content);
        response.EnsureSuccessStatusCode();

        Log.Information("SitrepConfigService - Saved scenario {Folder}/{Name}",
          folder, name);

        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "SitrepConfigService - Failed to save scenario {Folder}/{Name}",
          folder, name);
        return false;
      }
    }

    /// <summary>
    /// Delete a scenario config
    /// DELETE /api/scenarios/:folder/:name
    /// </summary>
    public async Task<bool> DeleteScenario(string folder, string name)
    {
      try
      {
        Log.Debug("SitrepConfigService - Deleting scenario {Folder}/{Name}",
          folder, name);

        var response = await _httpClient.DeleteAsync(
          $"{_sitrepBaseUrl}/api/scenarios/{Uri.EscapeDataString(folder)}/{Uri.EscapeDataString(name)}");
        response.EnsureSuccessStatusCode();

        Log.Information("SitrepConfigService - Deleted scenario {Folder}/{Name}",
          folder, name);

        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "SitrepConfigService - Failed to delete scenario {Folder}/{Name}",
          folder, name);
        return false;
      }
    }

    /// <summary>
    /// Fetch list of installed mods from Sentinel
    /// GET /api/mods
    /// </summary>
    public async Task<List<Mod>> GetAvailableMods()
    {
      try
      {
        Log.Debug("SitrepConfigService - Fetching available mods from {Url}",
          $"{_sitrepBaseUrl}/api/mods");

        var response = await _httpClient.GetAsync($"{_sitrepBaseUrl}/api/mods");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var mods = JsonSerializer.Deserialize<List<Mod>>(json,
          new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Log.Information("SitrepConfigService - Retrieved {Count} available mods",
          mods?.Count ?? 0);

        return mods ?? new List<Mod>();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "SitrepConfigService - Failed to fetch available mods");
        return new List<Mod>();
      }
    }

    /// <summary>
    /// Test connection to Sitrep API
    /// </summary>
    public async Task<bool> TestConnection()
    {
      try
      {
        Log.Debug("SitrepConfigService - Testing connection to {Url}", _sitrepBaseUrl);
        var response = await _httpClient.GetAsync($"{_sitrepBaseUrl}/api/scenarios");
        bool connected = response.IsSuccessStatusCode;

        Log.Information("SitrepConfigService - Connection test {Result}",
          connected ? "successful" : "failed");

        return connected;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "SitrepConfigService - Connection test failed");
        return false;
      }
    }
  }

  /// <summary>
  /// Represents scenario metadata from Sitrep
  /// </summary>
  public class ScenarioInfo
  {
    public string? Folder { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }

  /// <summary>
  /// Represents complete scenario data (config + mods)
  /// </summary>
  public class ScenarioData
  {
    public string? Name { get; set; }
    public string? Folder { get; set; }
    public List<Mod>? Mods { get; set; }
    public Dictionary<string, object>? Config { get; set; }
  }
}
