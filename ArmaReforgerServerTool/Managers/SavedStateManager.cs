/******************************************************************************
 * File Name:    SavedStateManager.cs
 * Project:      Longbow
 * Description:  This file contains the singleton SavedStateManager class
 *               responsible for keeping track of changes made to the state
 *               of the application for survival across restarts
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Longbow.Models;
using ReforgerServerApp.Utils;
using Serilog;
using System.Text.Json;

namespace Longbow.Managers
{
  internal class SavedStateManager
  {
    private static SavedStateManager? m_instance;
    private readonly SavedState m_savedState;
    private readonly string m_savedStateFile = "./state.json";

    /// <summary>
    /// Constructs a SavedStateManager object. Intended to only be used within the singleton <c>GetInstance()</c> method.
    /// This will check for the state files existence and if its found, loads it, otherwise creates a new default one.
    /// Usually the FileIOManager would handle this kind of task but it's a bit of a chicken-egg situation so the SavedStateManager
    /// can handle the loading of it's own file.
    /// </summary>
    private SavedStateManager()
    {
      if (File.Exists(m_savedStateFile))
      {
        try
        {
          using StreamReader sr = File.OpenText(m_savedStateFile);
          var savedState = sr.ReadToEnd();
          m_savedState = JsonSerializer.Deserialize<SavedState>(savedState)!;
          Log.Information("SavedStateManager - successfully loaded state file.");
        }
        catch (Exception)
        {
          string path = Path.GetFullPath(m_savedStateFile);

          Utilities.DisplayErrorMessage(
                  "State file is malformed. Please check your formatting is valid JSON and try again.",
                  "Unable to parse state file. Temporarily using default state. " +
                  $"Delete the state file at {path} and restart the application to permanently revert to default settings."
          );
          m_savedState = SavedState.Default;
        }
      }
      else
      {
        Log.Information("SavedStateManager - State file was not found, a default one was created.");
        m_savedState = SavedState.Default;
        File.WriteAllText(m_savedStateFile, m_savedState.AsJsonString());
      }
    }

    public static SavedStateManager GetInstance()
    {
      m_instance ??= new SavedStateManager();
      return m_instance;
    }

    public Dictionary<string, AdvancedSetting> GetLoadedAdvancedSettings() { return m_savedState.advancedSettings; }
    public string GetSavedStateFile() { return m_savedStateFile; }
    public SavedState GetSavedState() { return m_savedState; }
  }
}
