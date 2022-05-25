using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReforgerServerApp
{
    internal class Mod
    {
        readonly string ModID;
        readonly string ModName;
        readonly string ModVersion;

        public Mod(string modId, string modName, string modVersion)
        {
            ModID = modId;
            ModName = modName;
            ModVersion = modVersion;
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
            return ModName + " Version: " + ModVersion;
        }
    }
}
