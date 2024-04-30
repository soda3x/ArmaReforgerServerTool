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
        public static void AlphabetiseModList(ref List<Mod> list)
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
        /// Convenience method to sort the Mod ListBoxes in order of Mod Name
        /// </summary>
        /// <param name="listBox"></param>
        public static void AlphabetiseModListBox(ListBox listBox)
        {
            List<Mod> list = listBox.Items.OfType<Mod>().ToList();
            list.Sort((x, y) => string.Compare(x.GetModName(), y.GetModName()));
            listBox.Items.Clear();

            foreach (Mod m in list)
            {
                listBox.Items.Add(m);
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
    }
}
