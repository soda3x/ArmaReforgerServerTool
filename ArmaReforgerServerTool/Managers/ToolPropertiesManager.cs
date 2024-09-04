/******************************************************************************
 * File Name:    ToolPropertiesManager.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
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

namespace ReforgerServerApp.Managers;

/// <summary>
/// This class manages the application's <c>ToolProperties</c>
/// </summary>
internal class ToolPropertiesManager
{
    private static ToolPropertiesManager?   m_instance;
    private readonly ToolProperties         m_toolProperties;
    private readonly string                 m_toolPropertiesFile = "./properties.json";

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
                        "Properties is malformed. Please check your formatting is valid JSON and try again.",
                        "Unable to parse properties. Temporarily loading default settings. " +
                        $"Delete the properties file at {path} and restart the application to permanently revert to default settings."
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
    
    public List<string> GetDefaultScenarios() { return m_toolProperties.defaultScenarios; }
    public ToolProperties GetToolProperties() { return m_toolProperties; }
}