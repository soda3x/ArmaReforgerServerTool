/******************************************************************************
 * File Name:    Utilities.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  Static class containing utility methods for 
 *               performing various simple tasks
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

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
        /// Set supported platforms, seeing as its invalid to only host Xbox servers,
        /// if crossplay is enabled, its safe to assume PC and XBL are supported platforms
        /// </summary>
        /// <param name="crossplayEnabled"></param>
        /// <returns>Supported Platforms based on whether crossplay is enabled</returns>
        public static string[] GetSupportedPlatforms(bool crossplayEnabled)
        {
            if (crossplayEnabled)
            {
                return new string[] { Constants.SUPPORTED_PLATFORM_PC, Constants.SUPPORTED_PLATFORM_XBOX };
            }
            else
            {
                return new string[] { Constants.SUPPORTED_PLATFORM_PC };
            }
        }

        /// <summary>
        /// Convenience method for Displaying an Error Messagebox
        /// </summary>
        /// <param name="genMsg">General info about the error</param>
        /// <param name="errMsg">detailed message from the exception, if applicable</param>
        public static void DisplayErrorMessage(string genMsg, string errMsg)
        {
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
            switch (permission)
            {
                case RconPermission.ADMIN:
                return "admin";
                case RconPermission.MONITOR:
                return "monitor";
                default:
                return "admin";
            }
        }
    }
}