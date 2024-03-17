using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ReforgerServerApp
{
    public class Mod
    {
        readonly string ModID;
        readonly string ModName;

        public Mod(string modId, string modName)
        {
            ModID = modId;
            ModName = modName;
        }

        public Mod(Mod m)
        {
            ModID = m.ModID;
            ModName = m.ModName;
        }

        public string GetModID()
        {
            return ModID;
        }

        public string GetModName()
        {
            return ModName;
        }

        public override string ToString()
        {
            return ModName;
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
                return GetModName().Equals(((Mod)obj).GetModName()) && GetModID().Equals(((Mod)obj).GetModID());
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
