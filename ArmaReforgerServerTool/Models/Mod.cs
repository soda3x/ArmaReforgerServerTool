using HtmlAgilityPack;
using ReforgerServerApp.Utils;
using System.Diagnostics;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ReforgerServerApp
{
    public class Mod
    {
        public string modId {get; set;}
        public string name { get; set; }
        public string version { get; set; }

        private const string LATEST_MOD_VER_STR = "latest";

        public Mod() { /* Exists only for JSON deserializing */ }

        public Mod(string modId, string modName, string modVersion)
        {
            this.modId   = modId;
            this.name    = modName;
            this.version = modVersion;
        }

        public Mod(string modId, string modName)
        {
            this.modId   = modId;
            this.name    = modName;
            this.version = LATEST_MOD_VER_STR;
        }

        public Mod(Mod m)
        {
            this.modId   = m.modId;
            this.name    = m.name;
            this.version = m.version;
        }

        public string GetModID()
        {
            return modId!;
        }

        public string GetModName()
        {
            return name!;
        }

        public string GetModVersion()
        {
            return version!;
        }

        public override string ToString()
        {
            return $"{name} | {version}";
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
                Mod other    = (Mod) obj;
                bool modSame = name!.Equals(other.name);
                modSame     &= modId!.Equals(other.modId);
                modSame     &= version!.Equals(other.version);
                return modSame;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return modId!.GetHashCode() + name!.GetHashCode();
        }

        public static List<string> GetScenariosForMod(string modId)
        {
            List<string> scenarios = new();
            try
            {
                string fetchUrl               = $"https://reforger.armaplatform.com/workshop/{modId}/scenarios";
                HtmlWeb web                   = new();
                HtmlDocument doc              = web.Load(fetchUrl);
                const string className        = "text-start";
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
            }
            catch (Exception ex)
            {
                Utilities.DisplayErrorMessage("Unable to fetch Scenario IDs from Arma Reforger Workshop, please check your internet connection.", ex.Message);
            }
            return scenarios;
        }
    }
}