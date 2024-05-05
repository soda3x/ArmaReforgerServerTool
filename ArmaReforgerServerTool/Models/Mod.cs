using HtmlAgilityPack;
using ReforgerServerApp.Utils;
using System.Diagnostics;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ReforgerServerApp
{
    public class Mod
    {
        readonly string m_modId;
        readonly string m_modName;
        readonly string m_modVersion;

        private readonly static string LATEST_MOD_VER_STR = "latest";

        public Mod() 
        {
            // Exists purely for JSON (de)serializing
        }

        public Mod(string modId, string modName, string modVersion)
        {
            m_modId = modId;
            m_modName = modName;
            m_modVersion = modVersion;
        }

        public Mod(string modId, string modName)
        {
            m_modId = modId;
            m_modName = modName;
            m_modVersion = LATEST_MOD_VER_STR;
        }

        public Mod(Mod m)
        {
            m_modId = m.m_modId;
            m_modName = m.m_modName;
            m_modVersion = m.m_modVersion;
        }

        public string GetModID()
        {
            return m_modId;
        }

        public string GetModName()
        {
            return m_modName;
        }

        public string GetModVersion()
        {
            return m_modVersion;
        }

        public override string ToString()
        {
            return $"{m_modName} | {m_modVersion}";
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
                bool modNameSame = m_modName.Equals(other.m_modName);
                bool modIdSame = m_modId.Equals(other.m_modId);
                bool modVerSame = m_modVersion.Equals(other.m_modVersion);
                return modNameSame && modIdSame && modVerSame;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return m_modId.GetHashCode() + m_modName.GetHashCode();
        }

        public static List<string> GetScenariosForMod(string modId)
        {
            List<string> scenarios = new();
            try
            {
                
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
            }
            catch (Exception ex)
            {
                Utilities.DisplayErrorMessage("Unable to fetch Scenario IDs from Arma Reforger Workshop, please check your internet connection.", ex.Message);
                
            }
            return scenarios;
        }
    }
}
