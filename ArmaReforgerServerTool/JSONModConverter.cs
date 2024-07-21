using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReforgerServerApp
{
    internal class JSONModConverter
    {
        

        public List<InternalMod> AvailableMods { get; set; }
        public List<InternalMod> EnabledMods { get; set; }

        public JSONModConverter()
        {
            this.AvailableMods = new List<InternalMod>();
            this.EnabledMods = new List<InternalMod>();
        }

        public JSONModConverter(List<InternalMod> availableMods, List<InternalMod> enabledMods)
        {
            this.AvailableMods = availableMods;
            this.EnabledMods = enabledMods;
        }

        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }

        public void AddAvailableMod(Mod mod)
        {
            this.AvailableMods.Add((InternalMod) mod);
        }

        public void AddEnabledMod(Mod mod)
        {
            this.EnabledMods.Add((InternalMod)mod);
        }




        internal class InternalMod 
        {
            public string ModID { get; set; }
            public string ModName { get; set; }
            public string ModVersion { get; set; }

            public static explicit operator InternalMod(Mod mod)
            {
                InternalMod internalMod = new InternalMod();

                internalMod.ModID = mod.GetModID();
                internalMod.ModName = mod.GetModName();
                internalMod.ModVersion = mod.GetModVersion();

                return internalMod;
            }
        }
    }
}
