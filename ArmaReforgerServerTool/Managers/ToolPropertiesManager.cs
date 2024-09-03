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

    private ToolPropertiesManager()
    {
        m_toolProperties = LoadToolProperties();
    }

    private ToolPropertiesManager(ToolProperties toolProperties)
    {
        m_toolProperties = toolProperties;
    }

    public static ToolPropertiesManager GetInstance()
    {
        m_instance ??= new ToolPropertiesManager();
        return m_instance;
    }

    /// <summary>
    /// This method returns the default <c>ToolProperties</c> as Json string.
    /// The singleton instance is constructed with a default <c>ToolProperties</c>
    /// if it hasn't already.
    /// </summary>
    /// <returns>The default <c>ToolProperties</c> as Json</returns>
    public static string GenerateToolProperties()
    {
        m_instance ??= new ToolPropertiesManager(ToolProperties.Default);
        return m_instance.GetToolProperties().AsJsonString();
    }

    /// <summary>
    /// This method deserializes the loaded <c>ToolProperties</c>
    /// </summary>
    private static ToolProperties LoadToolProperties()
    {
        try
        {
            string serializedProperties = FileIOManager.GetInstance().GetToolProperties();
            return JsonSerializer.Deserialize<ToolProperties>(serializedProperties)!;
        }
        catch (Exception)
        {
            string path = Path.GetFullPath(FileIOManager.GetInstance().GetServerToolPropertiesFile());
            Utilities.DisplayErrorMessage(
                    "Properties is malformed. Please check your formatting is valid JSON and try again.",
                    "Unable to parse properties. Temporarily loading default settings. " +
                    $"Delete the properties file at {path} and restart the application to permanently revert to default settings."
            );

            return ToolProperties.Default;
        }
    }
    
    public List<string> GetDefaultScenarios() { return m_toolProperties.defaultScenarios; }
    public ToolProperties GetToolProperties() { return m_toolProperties; }
}