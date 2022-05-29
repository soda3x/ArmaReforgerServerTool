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

        public Mod(string modId, string modName)
        {
            ModID = modId;
            ModName = modName;
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
    }
}
