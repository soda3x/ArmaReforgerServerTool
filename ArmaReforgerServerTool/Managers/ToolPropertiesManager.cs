/******************************************************************************
 * File Name:    ToolPropertiesManager.cs
 * Project:      Longbow
 * Description:  This file contains the singleton ToolPropertiesManager class
 *               responsible for the retrieval and management of the server 
 *               tool properties.
 * 
 * Authors:      Kye Seyhun
 ******************************************************************************/

using System.Text.Json;
using Serilog;
using ReforgerServerApp.Models;
using ReforgerServerApp.Utils;
using Longbow.Models;
using Longbow.Utils;

namespace ReforgerServerApp.Managers;

/// <summary>
/// This class manages the application's <c>ToolProperties</c>
/// </summary>
internal class ToolPropertiesManager
{
  private static ToolPropertiesManager? m_instance;
  private readonly ToolProperties m_toolProperties;
  private readonly string m_toolPropertiesFile = "./properties.json";

  /// <summary>
  /// Constructs a ToolPropertiesManager object. Intended to only be used within the singleton <c>GetInstance()</c> method.
  /// This will check for the properties files existence and if its found, loads it, otherwise creates a new default one.
  /// Usually the FileIOManager would handle this kind of task but it's a bit of a chicken-egg situation so the ToolPropertiesManager
  /// can handle the loading of it's own file.
  /// </summary>
  private ToolPropertiesManager()
  {
    if (File.Exists(m_toolPropertiesFile))
    {
      try
      {
        using StreamReader sr = File.OpenText(m_toolPropertiesFile);
        var toolProperties = sr.ReadToEnd();
        m_toolProperties = JsonSerializer.Deserialize<ToolProperties>(toolProperties)!;
        Log.Information("ToolPropertiesManager - successfully loaded properties file.");
      }
      catch (Exception)
      {
        string path = Path.GetFullPath(m_toolPropertiesFile);

        Utilities.DisplayErrorMessage(
                Strings.Get(Constants.TOOL_PROPERTIES_MANAGER_MALFORMED_STR),
                Strings.Get(Constants.TOOL_PROPERTIES_MANAGER_MALFORMED_DETAIL_STR, ("path", path))
        );
        m_toolProperties = ToolProperties.Default;
      }
    }
    else
    {
      Log.Information("ToolPropertiesManager - Properties file was not found, a default one was created.");
      m_toolProperties = ToolProperties.Default;
      File.WriteAllText(m_toolPropertiesFile, m_toolProperties.AsJsonString());
    }
  }

  public static ToolPropertiesManager GetInstance()
  {
    m_instance ??= new ToolPropertiesManager();
    return m_instance;
  }

  public List<Scenario> GetDefaultScenarios() { return m_toolProperties.defaultScenarios; }
  public ToolProperties GetToolProperties() { return m_toolProperties; }

  /// <summary>
  /// Writes the current <c>ToolProperties</c> (as returned by <c>GetToolProperties()</c>, which
  /// callers mutate directly) back to <c>properties.json</c>. There was previously no write path
  /// for this file outside of first-run default creation - this is the first caller of it.
  /// </summary>
  public void SaveToolProperties()
  {
    try
    {
      File.WriteAllText(m_toolPropertiesFile, m_toolProperties.AsJsonString());
      Log.Information("ToolPropertiesManager - successfully saved properties file.");
    }
    catch (Exception ex)
    {
      Utilities.DisplayErrorMessage(Strings.Get(Constants.TOOL_PROPERTIES_MANAGER_SAVE_FAILED_STR), ex.Message);
    }
  }
}
