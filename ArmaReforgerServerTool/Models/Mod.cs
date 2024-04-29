using HtmlAgilityPack;
using System.Diagnostics;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ReforgerServerApp
{
    public class Mod
    {
        readonly string ModID;
        readonly string ModName;
        readonly string ModVersion;

        private readonly static string LATEST_MOD_VER_STR = "latest";

        public Mod(string modId, string modName, string modVersion)
        {
            ModID = modId;
            ModName = modName;
            ModVersion = modVersion;
        }

        public Mod(string modId, string modName)
        {
            ModID = modId;
            ModName = modName;
            ModVersion = LATEST_MOD_VER_STR;
        }

        public Mod(Mod m)
        {
            ModID = m.ModID;
            ModName = m.ModName;
            ModVersion = m.ModVersion;
        }

        public string GetModID()
        {
            return ModID;
        }

        public string GetModName()
        {
            return ModName;
        }

        public string GetModVersion()
        {
            return ModVersion;
        }

        public override string ToString()
        {
            return $"{ModName} | {ModVersion}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(Mod))
            {
                Mod other = (Mod) obj;
                bool modNameSame = ModName.Equals(other.ModName);
                bool modIdSame = ModID.Equals(other.ModID);
                bool modVerSame = ModVersion.Equals(other.ModVersion);
                return modNameSame && modIdSame && modVerSame;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ModID.GetHashCode() + ModName.GetHashCode();
        }

        public static List<string> GetScenariosForMod(string modId)
        {
            List<string> scenarios = new();
            string fetchUrl = $"https://reforger.armaplatform.com/workshop/{modId}/scenarios";
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(fetchUrl);
            const string className = "text-start";
            HtmlNodeCollection rawScenIds = doc.DocumentNode.SelectNodes($"//*[contains(@class,'{className}')]");
            if (rawScenIds != null)
            {
                foreach (HtmlNode field in rawScenIds)
                {
                    scenarios.Add(field.InnerText);
                }
            }
            else
            {
                Debug.WriteLine("Failed to fetch any scenario ids for this mod. It may not have any.");
            }
            return scenarios;
        }
    }
}
