/******************************************************************************
 * File Name:    SitrepConfigService.cs
 * Project:      Longbow
 * Description:  Loads and saves mod configurations from Sitrep API or local storage.
 *               Provides fallback to local JSON file storage if API unavailable.
 *
 * Author:       Longbow contributors
 ******************************************************************************/

using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReforgerServerApp.Managers
{
  /// <summary>
  /// Represents a saved mod configuration scenario
  /// </summary>
  public class ModScenario
  {
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("mods")]
    public List<ModEntry> Mods { get; set; } = new();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
  }

  /// <summary>
  /// Represents a single mod entry in a scenario
  /// </summary>
  public class ModEntry
  {
    [JsonPropertyName("modId")]
    public string ModId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = "latest";

    [JsonPropertyName("required")]
    public bool Required { get; set; } = false;
  }

  /// <summary>
  /// Service for loading and saving mod configurations from Sitrep API or local storage
  /// </summary>
  internal class SitrepConfigService
  {
    private static SitrepConfigService? m_instance;
    private readonly string m_configDirectory;
    private readonly string m_sitrepApiUrl;
    private bool m_apiAvailable = false;

    private SitrepConfigService()
    {
      m_configDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Longbow",
        "ModConfigs"
      );

      // Create config directory if it doesn't exist
      Directory.CreateDirectory(m_configDirectory);

      // Try to load Sitrep API URL from properties (if configured)
      try
      {
        var props = ToolPropertiesManager.GetInstance().GetToolProperties();
        m_sitrepApiUrl = props.sitrepApiUrl ?? "https://api.sitrep.example.com";
        m_apiAvailable = true;
      }
      catch (Exception ex)
      {
        Log.Warning("SitrepConfigService - Could not load Sitrep API URL: {msg}. Falling back to local storage.", ex.Message);
        m_sitrepApiUrl = string.Empty;
        m_apiAvailable = false;
      }

      Log.Information("SitrepConfigService initialized. Config directory: {dir}, API available: {available}",
        m_configDirectory, m_apiAvailable);
    }

    public static SitrepConfigService GetInstance()
    {
      m_instance ??= new SitrepConfigService();
      return m_instance;
    }

    /// <summary>
    /// Lists all available mod configurations (from API or local storage)
    /// </summary>
    public async Task<List<ModScenario>> ListConfigurations()
    {
      Log.Information("SitrepConfigService - Listing configurations");
      var scenarios = new List<ModScenario>();

      try
      {
        // Try API first
        if (m_apiAvailable)
        {
          try
          {
            scenarios = await ListConfigurationsFromApi();
            if (scenarios.Count > 0)
            {
              Log.Information("SitrepConfigService - Loaded {count} configurations from Sitrep API", scenarios.Count);
              return scenarios;
            }
          }
          catch (Exception ex)
          {
            Log.Warning("SitrepConfigService - Failed to load from Sitrep API: {msg}. Falling back to local storage.", ex.Message);
          }
        }

        // Fall back to local storage
        scenarios = ListConfigurationsFromLocal();
        Log.Information("SitrepConfigService - Loaded {count} configurations from local storage", scenarios.Count);
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error listing configurations: {msg}", ex.Message);
      }

      return scenarios;
    }

    /// <summary>
    /// Loads a mod configuration by name
    /// </summary>
    public async Task<ModScenario?> LoadConfiguration(string configName)
    {
      Log.Information("SitrepConfigService - Loading configuration: {name}", configName);

      try
      {
        // Try API first
        if (m_apiAvailable)
        {
          try
          {
            var scenario = await LoadConfigurationFromApi(configName);
            if (scenario != null)
            {
              Log.Information("SitrepConfigService - Loaded configuration '{name}' from Sitrep API", configName);
              return scenario;
            }
          }
          catch (Exception ex)
          {
            Log.Warning("SitrepConfigService - Failed to load from Sitrep API: {msg}. Trying local storage.", ex.Message);
          }
        }

        // Fall back to local storage
        var localScenario = LoadConfigurationFromLocal(configName);
        if (localScenario != null)
        {
          Log.Information("SitrepConfigService - Loaded configuration '{name}' from local storage", configName);
          return localScenario;
        }

        Log.Warning("SitrepConfigService - Configuration '{name}' not found", configName);
        return null;
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error loading configuration: {msg}", ex.Message);
        return null;
      }
    }

    /// <summary>
    /// Saves a mod configuration
    /// </summary>
    public async Task<bool> SaveConfiguration(ModScenario scenario)
    {
      Log.Information("SitrepConfigService - Saving configuration: {name}", scenario.Name);

      try
      {
        // Always save to local storage first
        var localSuccess = SaveConfigurationToLocal(scenario);

        // Try API if available
        if (m_apiAvailable)
        {
          try
          {
            var apiSuccess = await SaveConfigurationToApi(scenario);
            if (apiSuccess)
            {
              Log.Information("SitrepConfigService - Saved configuration '{name}' to both local and Sitrep API", scenario.Name);
              return true;
            }
          }
          catch (Exception ex)
          {
            Log.Warning("SitrepConfigService - Failed to save to Sitrep API: {msg}. Configuration saved locally.", ex.Message);
          }
        }

        if (localSuccess)
        {
          Log.Information("SitrepConfigService - Saved configuration '{name}' to local storage", scenario.Name);
          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error saving configuration: {msg}", ex.Message);
        return false;
      }
    }

    /// <summary>
    /// Deletes a mod configuration
    /// </summary>
    public async Task<bool> DeleteConfiguration(string configName)
    {
      Log.Information("SitrepConfigService - Deleting configuration: {name}", configName);

      try
      {
        // Delete from local storage
        var localSuccess = DeleteConfigurationFromLocal(configName);

        // Try API if available
        if (m_apiAvailable)
        {
          try
          {
            var apiSuccess = await DeleteConfigurationFromApi(configName);
            if (!apiSuccess)
              Log.Warning("SitrepConfigService - Failed to delete from Sitrep API");
          }
          catch (Exception ex)
          {
            Log.Warning("SitrepConfigService - Failed to delete from Sitrep API: {msg}", ex.Message);
          }
        }

        if (localSuccess)
        {
          Log.Information("SitrepConfigService - Deleted configuration '{name}'", configName);
          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error deleting configuration: {msg}", ex.Message);
        return false;
      }
    }

    // ===== Local Storage Methods =====

    private List<ModScenario> ListConfigurationsFromLocal()
    {
      var scenarios = new List<ModScenario>();

      try
      {
        if (!Directory.Exists(m_configDirectory))
          return scenarios;

        var files = Directory.GetFiles(m_configDirectory, "*.json");
        foreach (var file in files)
        {
          try
          {
            var json = File.ReadAllText(file);
            var scenario = JsonSerializer.Deserialize<ModScenario>(json);
            if (scenario != null)
              scenarios.Add(scenario);
          }
          catch (Exception ex)
          {
            Log.Warning("SitrepConfigService - Error reading local config file {file}: {msg}", file, ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error listing local configurations: {msg}", ex.Message);
      }

      return scenarios;
    }

    private ModScenario? LoadConfigurationFromLocal(string configName)
    {
      try
      {
        var filePath = Path.Combine(m_configDirectory, $"{configName}.json");
        if (File.Exists(filePath))
        {
          var json = File.ReadAllText(filePath);
          return JsonSerializer.Deserialize<ModScenario>(json);
        }
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error loading local configuration: {msg}", ex.Message);
      }

      return null;
    }

    private bool SaveConfigurationToLocal(ModScenario scenario)
    {
      try
      {
        scenario.UpdatedAt = DateTime.Now;
        var filePath = Path.Combine(m_configDirectory, $"{scenario.Name}.json");
        var json = JsonSerializer.Serialize(scenario, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        return true;
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error saving local configuration: {msg}", ex.Message);
        return false;
      }
    }

    private bool DeleteConfigurationFromLocal(string configName)
    {
      try
      {
        var filePath = Path.Combine(m_configDirectory, $"{configName}.json");
        if (File.Exists(filePath))
        {
          File.Delete(filePath);
          return true;
        }
      }
      catch (Exception ex)
      {
        Log.Error("SitrepConfigService - Error deleting local configuration: {msg}", ex.Message);
      }

      return false;
    }

    // ===== API Methods (Placeholders for now) =====

    private async Task<List<ModScenario>> ListConfigurationsFromApi()
    {
      // TODO: Implement when Sitrep API is available
      // For now, return empty list to force fallback to local storage
      await Task.Delay(100); // Simulate async operation
      return new List<ModScenario>();
    }

    private async Task<ModScenario?> LoadConfigurationFromApi(string configName)
    {
      // TODO: Implement when Sitrep API is available
      // For now, return null to force fallback to local storage
      await Task.Delay(100); // Simulate async operation
      return null;
    }

    private async Task<bool> SaveConfigurationToApi(ModScenario scenario)
    {
      // TODO: Implement when Sitrep API is available
      // For now, return false to indicate API not implemented
      await Task.Delay(100); // Simulate async operation
      return false;
    }

    private async Task<bool> DeleteConfigurationFromApi(string configName)
    {
      // TODO: Implement when Sitrep API is available
      // For now, return false to indicate API not implemented
      await Task.Delay(100); // Simulate async operation
      return false;
    }
  }
}
