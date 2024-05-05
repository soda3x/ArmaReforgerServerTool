using System.ComponentModel;
using System.Text.Json;

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
        public static void AlphabetiseModList(ref BindingList<Mod> list)
        {
            List<Mod> temp = new(list);
            temp.Sort((x, y) => string.Compare(x.GetModName(), y.GetModName()));
            list.Clear();

            foreach (Mod m in list)
            {
                list.Add(m);
            }
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
        /// <returns>Formatted serialized JSON string</returns>
        public static string GetFormattedJsonString(object input)
        {
            string serializedJson = JsonSerializer.Serialize(input, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return serializedJson;
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
                    Constants.ERROR_MESSAGEBOX_TITLE_STR
                    );
        }
    }
}
