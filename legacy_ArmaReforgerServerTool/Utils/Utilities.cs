/******************************************************************************
 * File Name:    Utilities.cs
 * Project:      Longbow
 * Description:  Static class containing utility methods for 
 *               performing various simple tasks
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Serilog;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReforgerServerApp.Utils
{
  /// <summary>
  /// Static class containing utility methods for performing various simple tasks
  /// </summary>
  internal class Utilities
  {
    private const double KB_IN_MB = 1024.0;
    private const double KB_IN_GB = KB_IN_MB * 1024.0;

    /// <summary>
    /// Convenience method to sort a list of Mods in order of Mod Name
    /// </summary>
    /// <param name="list"> takes a list of Mods as a reference</param>
    public static List<Mod> AlphabetiseModList(BindingList<Mod> list)
    {
      List<Mod> temp = new(list);
      temp.Sort((x, y) => string.Compare(x.GetModName(), y.GetModName()));
      return temp;
    }

    /// <summary>
    /// Utility method for checking a List of mods is lexicographically sorted
    /// </summary>
    /// <param name="list"></param>
    /// <returns>True if sorted, False if not</returns>
    public static bool IsSorted(BindingList<Mod> list)
    {
      for (int i = 0; i < list.Count - 1; i++)
      {
        if (string.Compare(list[i].name, list[i + 1].name, StringComparison.Ordinal) > 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Moves an item in a list forward or backward.
    /// </summary>
    /// <param name="list">The list of items.</param>
    /// <param name="item">The item to move.</param>
    /// <param name="moveBackward">Optional, moves forward by default, set to true to move it backward.</param>
    public static void MoveItem<T>(BindingList<T> list, T item, bool moveBackward = false)
    {
      if (list == null)
      {
        // List is null, don't do anything
        return;
      }

      int index = list.IndexOf(item);

      if (index == -1)
      {
        // Item doesn't exist in the list, don't do anything
        return;
      }

      int newIndex = moveBackward ? index - 1 : index + 1;

      if (newIndex < 0 || newIndex >= list.Count)
      {
        // Can't move outside list bounds, do nothing
        return;
      }

      // Swap the items
      (list[newIndex], list[index]) = (list[index], list[newIndex]);
    }

    /// <summary>
    /// Return string representation of DateTime.Now
    /// </summary>
    /// <returns></returns>
    public static string GetTimestamp()
    {
      return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// Convenience method to return a formatted JSON string
    /// </summary>
    /// <param name="input">to serialize</param>
    /// <param name="converters">optional any custom converters to use</param>
    /// <returns>Formatted serialized JSON string</returns>
    public static string GetFormattedJsonString(object input, params JsonConverter[] converters)
    {
      var options = new JsonSerializerOptions
      {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };
      foreach (JsonConverter jc in converters)
      {
        options.Converters.Add(jc);
      }

      string serializedJson = JsonSerializer.Serialize(input, options);
      return serializedJson;
    }

    /// <summary>
    /// Convenience method for retrieving a deserialized Server Config object from
    /// JSON
    /// </summary>
    /// <param name="json">to deserialise</param>
    /// <param name="converters">optional any custom converters to use</param>
    /// <returns>'Root' object representing a server configuration</returns>
    public static Root GetServerConfigFromJson(string json, params JsonConverter[] converters)
    {
      var options = new JsonSerializerOptions
      {
        WriteIndented = true
      };
      foreach (JsonConverter jc in converters)
      {
        options.Converters.Add(jc);
      }
      return JsonSerializer.Deserialize<Root>(json, options);
    }

    /// <summary>
    /// Set supported platforms.
    /// </summary>
    /// <param name="crossplayEnabled"></param>
    /// <returns>Supported Platforms based on whether crossplay is enabled</returns>
    public static string[] GetSupportedPlatforms(bool crossplayEnabled)
    {
      List<string> supportedPlatforms = new() { Constants.SUPPORTED_PLATFORM_PC };
      if (crossplayEnabled)
      {
        Log.Debug("Crossplay is enabled");
        Log.Debug("Adding Xbox to supported platforms");
        supportedPlatforms.Add(Constants.SUPPORTED_PLATFORM_XBOX);
        Log.Debug("Adding PlayStation to supported platforms");
        supportedPlatforms.Add(Constants.SUPPORTED_PLATFORM_PSN);
      }
      return supportedPlatforms.ToArray();
    }

    /// <summary>
    /// Convenience method for Displaying an Error Messagebox
    /// </summary>
    /// <param name="genMsg">General info about the error</param>
    /// <param name="errMsg">detailed message from the exception, if applicable</param>
    public static void DisplayErrorMessage(string genMsg, string errMsg)
    {
      Log.Error("An error prompt was displayed: {genMsg} - {errMsg}", genMsg, errMsg);
      MessageBox.Show(
          $"{genMsg}\r\n\r\n" +
          $"Detail: {errMsg}\r\n\r\n" +
          $"Include the detail above in your bug reports.",
          Constants.ERROR_MESSAGEBOX_TITLE_STR);
    }

    /// <summary>
    /// Convenience method for Displaying a Confirmation Messagebox 
    /// (message box with OK and Cancel buttons, or Yes and No if useYesOrNo = true)
    /// </summary>
    /// <param name="msg">Warning message to display</param>
    /// <param name="useYesOrNo">Use Yes or No buttons instead of OK and Cancel</param>
    /// <returns>True if the following logic should continue, False otherwise</returns>
    public static bool DisplayConfirmationMessage(string msg, bool useYesOrNo = false)
    {
      DialogResult result =
          MessageBox.Show($"{msg}",
          Constants.WARN_MESSAGEBOX_TITLE_STR,
          useYesOrNo ? MessageBoxButtons.YesNo : MessageBoxButtons.OKCancel);

      return result == DialogResult.OK || result == DialogResult.Yes;
    }

    /// <summary>
    /// Convert a given string to an Enum T
    /// </summary>
    /// <typeparam name="T">Enum type to convert to</typeparam>
    /// <param name="enumValue">String representation of enum value (case doesn't matter)</param>
    /// <returns>Enum value if successful</returns>
    /// <exception cref="ArgumentException">thrown if unsuccessful</exception>
    public static T StringToEnum<T>(string parameterValue)
    {
      if (!Enum.IsDefined(typeof(T), parameterValue.ToUpper()))
      {
        throw new ArgumentException($"'{parameterValue.ToUpper()}' is not a valid value for enum '{typeof(T).Name}'");
      }
      return (T) Enum.Parse(typeof(T), parameterValue.ToUpper());
    }

    /// <summary>
    /// Utility method for converting a RconPermission value to a string
    /// </summary>
    /// <param name="permission">to convert</param>
    /// <returns>string representation (in lowercase)</returns>
    public static string RconPermissionToString(RconPermission permission)
    {
      return permission switch
      {
        RconPermission.ADMIN => "admin",
        RconPermission.MONITOR => "monitor",
        _ => "monitor",
      };
    }

    /// <summary>
    /// Utility method to help create a list of ipAddr:port mappings for UPnP
    /// </summary>
    /// <returns>List of pairs of ipAddr:port</returns>
    public static List<(string ipAddr, int port)> GetPortMappingsFromServerConfig()
    {
      List<(string ipAddr, int port)> mappings = new List<(string ipAddr, int port)>();

      // Bind Port
      if (ConfigurationManager.GetInstance().GetServerConfiguration().root.bindAddress != null)
      {
        mappings.Add((ConfigurationManager.GetInstance().GetServerConfiguration().root.bindAddress,
                      ConfigurationManager.GetInstance().GetServerConfiguration().root.bindPort));
      }

      // Public Port
      if (ConfigurationManager.GetInstance().GetServerConfiguration().root.publicAddress != null)
      {
        mappings.Add((ConfigurationManager.GetInstance().GetServerConfiguration().root.publicAddress,
                      ConfigurationManager.GetInstance().GetServerConfiguration().root.publicPort));
      }

      // A2S Port
      if (ConfigurationManager.GetInstance().GetServerConfiguration().root.a2s.address != null)
      {
        mappings.Add((ConfigurationManager.GetInstance().GetServerConfiguration().root.a2s.address,
                      ConfigurationManager.GetInstance().GetServerConfiguration().root.a2s.port));
      }

      // Rcon Port
      // Rcon is treated a bit differently due to the fact that it can be omitted
      if (ConfigurationManager.GetInstance().GetServerConfiguration().root.rcon != null)
      {
        if (ConfigurationManager.GetInstance().GetServerConfiguration().root.rcon.address != null)
        {
          mappings.Add((ConfigurationManager.GetInstance().GetServerConfiguration().root.rcon.address,
                        ConfigurationManager.GetInstance().GetServerConfiguration().root.rcon.port));
        }
      }

      return mappings;
    }

    /// <returns>
    /// Return the lowest of either Number of CPUs or 16
    /// </returns>
    public static int GetNumberAvailableThreads()
    {
      const int maxNumThreads = 16;
      return Environment.ProcessorCount > maxNumThreads ? maxNumThreads : Environment.ProcessorCount;
    }

    /// <summary>
    /// Utility for formatting a kilobyte value into its best represented human-readable format
    /// GB --> MB --> kB
    /// </summary>
    /// <param name="kilobytes">to represent</param>
    /// <returns>Human readable formatted kilobytes</returns>
    public static string FormatMemoryValue(long kilobytes)
    {
      if (kilobytes >= KB_IN_GB)
      {
        double gigabytes = kilobytes / KB_IN_GB;
        return $"{gigabytes:F2} GB";
      }

      if (kilobytes >= KB_IN_MB)
      {
        double megabytes = kilobytes / KB_IN_MB;
        return $"{megabytes:F2} MB";
      }

      return $"{kilobytes} kB";
    }
  }
}
