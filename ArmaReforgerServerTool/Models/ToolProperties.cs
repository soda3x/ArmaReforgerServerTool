/******************************************************************************
 * File Name:    ToolProperties.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This file contains the ToolProperties class, a model
 *               representing the server tool properties file
 * 
 * Author:       Kye Seyhun
 ******************************************************************************/

using System.Text.Json;

namespace ReforgerServerApp.Models
{
    internal class ToolProperties
    {
        public static readonly List<string> DEFAULT_SCENARIOS = new List<string>
            {
                "{ECC61978EDCC2B5A}Missions/23_Campaign.conf",
                "{59AD59368755F41A}Missions/21_GM_Eden.conf",
                "{94FDA7451242150B}Missions/103_Arland_Tutorial.conf",
                "{2BBBE828037C6F4B}Missions/22_GM_Arland.conf",
                "{C700DB41F0C546E1}Missions/23_Campaign_NorthCentral.conf",
                "{28802845ADA64D52}Missions/23_Campaign_SWCoast.conf",
                "{DAA03C6E6099D50F}Missions/24_CombatOps.conf",
                "{C41618FD18E9D714}Missions/23_Campaign_Arland.conf",
                "{DFAC5FABD11F2390}Missions/26_CombatOpsEveron.conf"
            };        

        public List<string> defaultScenarios { get; set; }

        public ToolProperties(List<string> defaultScenarios)
        {
            this.defaultScenarios = defaultScenarios;
        }

        public static ToolProperties Default => new(DEFAULT_SCENARIOS);

        public string AsJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
