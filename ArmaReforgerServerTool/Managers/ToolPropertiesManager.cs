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
/// This class manages the server tool properties
/// </summary>
internal class ToolPropertiesManager
{
    private static ToolPropertiesManager?   m_instance;
    private readonly ToolProperties         m_toolProperties;
    public ToolProperties ToolProperties => m_toolProperties;
    public List<string> DefaultScenarios => m_toolProperties.defaultScenarios;

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
    /// This method initialises the ToolPropertiesManager with the default tool properties.
    /// </summary>
    /// <returns>The default tool properties as Json</returns>
    public static string GenerateToolProperties()
    {
        m_instance = new ToolPropertiesManager(ToolProperties.Default);
        return m_instance.ToolProperties.AsJsonString();
    }

    /// <summary>
    /// This method deserializes the tool properties from file
    /// </summary>
    private static ToolProperties LoadToolProperties()
    {
        try
        {
            string serializedProperties = FileIOManager.GetInstance().GetToolProperties();
            return JsonSerializer.Deserialize<ToolProperties>(serializedProperties)!;
        }
        catch (Exception e)
        {
            string path = Path.GetFullPath(FileIOManager.ServerToolPropertiesFile);
            Utilities.DisplayErrorMessage(
                    "ARMA Reforger Server Tool properties is malformed. Please check your formatting is valid JSON and try again.",
                    "Unable to parse properties. Temporarily loading default settings. " +
                    $"Delete the properties file at {path} and restart the application to permanently revert to default settings."
                    // TODO: add exception message to logger: #69
            );

            return ToolProperties.Default;
        }
    }
}