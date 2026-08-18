use serde::{Deserialize, Serialize};

use super::scenario::Scenario;

/// Represents the structure of the properties file containing the application settings.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct ToolProperties {
    #[serde(default)]
    pub default_scenarios: Vec<Scenario>,
    #[serde(default)]
    pub mod_database_file: String,
    #[serde(default)]
    pub release_repository_url: String,
    #[serde(default)]
    pub bug_report_url: String,
    #[serde(default)]
    pub check_for_updates_on_startup: bool,
    #[serde(default)]
    pub steam_cmd_download_url: String,
    #[serde(default)]
    pub arma_workshop_url: String,
    #[serde(default)]
    pub log_file: String,
    #[serde(default)]
    pub minimum_log_level: String,
    #[serde(default)]
    pub auto_restart_time_ms: u64,
}

impl ToolProperties {
    /// The full hardcoded list of built-in Arma Reforger scenarios, ported verbatim from
    /// the C# `ToolProperties.DEFAULT_SCENARIOS`.
    pub fn default_scenarios_list() -> Vec<Scenario> {
        vec![
            Scenario::new("Conflict - Everon", "{ECC61978EDCC2B5A}Missions/23_Campaign.conf"),
            Scenario::new("Training", "{002AF7323E0129AF}Missions/Tutorial.conf"),
            Scenario::new("Game Master - Everon", "{59AD59368755F41A}Missions/21_GM_Eden.conf"),
            Scenario::new("Game Master - Arland", "{2BBBE828037C6F4B}Missions/22_GM_Arland.conf"),
            Scenario::new("Game Master - Kolguyev", "{F45C6C15D31252E6}Missions/27_GM_Cain.conf"),
            Scenario::new(
                "Conflict - Northern Everon",
                "{C700DB41F0C546E1}Missions/23_Campaign_NorthCentral.conf",
            ),
            Scenario::new(
                "Conflict - Southern Everon",
                "{28802845ADA64D52}Missions/23_Campaign_SWCoast.conf",
            ),
            Scenario::new(
                "Conflict - Western Everon",
                "{94992A3D7CE4FF8A}Missions/23_Campaign_Western.conf",
            ),
            Scenario::new(
                "Conflict - Montignac",
                "{FDE33AFE2ED7875B}Missions/23_Campaign_Montignac.conf",
            ),
            Scenario::new("Combat Ops - Arland", "{DAA03C6E6099D50F}Missions/24_CombatOps.conf"),
            Scenario::new(
                "Conflict - Arland",
                "{C41618FD18E9D714}Missions/23_Campaign_Arland.conf",
            ),
            Scenario::new(
                "Combat Ops - Everon",
                "{DFAC5FABD11F2390}Missions/26_CombatOpsEveron.conf",
            ),
            Scenario::new(
                "Capture & Hold - Briars",
                "{3F2E005F43DBD2F8}Missions/CAH_Briars_Coast.conf",
            ),
            Scenario::new(
                "Capture & Hold - Montfort Castle",
                "{F1A1BEA67132113E}Missions/CAH_Castle.conf",
            ),
            Scenario::new(
                "Capture & Hold - Concrete Plant",
                "{589945FB9FA7B97D}Missions/CAH_Concrete_Plant.conf",
            ),
            Scenario::new(
                "Capture & Hold - Almara Factory",
                "{9405201CBD22A30C}Missions/CAH_Factory.conf",
            ),
            Scenario::new(
                "Capture & Hold - Simon's Wood",
                "{1CD06B409C6FAE56}Missions/CAH_Forest.conf",
            ),
            Scenario::new(
                "Capture & Hold - Le Moule",
                "{7C491B1FCC0FF0E1}Missions/CAH_LeMoule.conf",
            ),
            Scenario::new(
                "Capture & Hold - Camp Blake",
                "{6EA2E454519E5869}Missions/CAH_Military_Base.conf",
            ),
            Scenario::new("Capture & Hold - Morton", "{2B4183DF23E88249}Missions/CAH_Morton.conf"),
            Scenario::new("Elimination", "{C47A1A6245A13B26}Missions/SP01_ReginaV2.conf"),
            Scenario::new("Air Support", "{0648CDB32D6B02B3}Missions/SP02_AirSupport.conf"),
            Scenario::new(
                "Conflict: HQ Commander - Everon",
                "{0220741028718E7F}Missions/23_Campaign_HQC_Everon.conf",
            ),
            Scenario::new(
                "Conflict: HQ Commander - Arland",
                "{68D1240A11492545}Missions/23_Campaign_HQC_Arland.conf",
            ),
            Scenario::new(
                "Conflict: HQ Commander - Kolguyev",
                "{BB5345C22DD2B655}Missions/23_Campaign_HQC_Cain.conf",
            ),
            Scenario::new(
                "Operation Omega 01: Over The Hills And Far Away",
                "{10B8582BAD9F7040}Missions/Scenario01_Intro.conf",
            ),
            Scenario::new(
                "Operation Omega 02: Radio Check",
                "{1D76AF6DC4DF0577}Missions/Scenario02_Steal.conf",
            ),
            Scenario::new(
                "Operation Omega 03: Light In The Dark",
                "{D1647575BCEA5A05}Missions/Scenario03_Villa.conf",
            ),
            Scenario::new(
                "Operation Omega 04: Red Silence",
                "{6D224A109B973DD8}Missions/Scenario04_Sabotage.conf",
            ),
            Scenario::new(
                "Operation Omega 05: Cliffhanger",
                "{FA2AB0181129CB16}Missions/Scenario05_Hill.conf",
            ),
            Scenario::new(
                "Combat Ops - Kolguyev",
                "{CB347F2F10065C9C}Missions/CombatOpsCain.conf",
            ),
        ]
    }
}

impl Default for ToolProperties {
    fn default() -> Self {
        Self {
            default_scenarios: Self::default_scenarios_list(),
            mod_database_file: "./mod_database.json".to_string(),
            release_repository_url: "https://github.com/cullenwerks/longbow/releases".to_string(),
            bug_report_url: "https://github.com/cullenwerks/longbow/issues".to_string(),
            check_for_updates_on_startup: true,
            steam_cmd_download_url: "https://steamcdn-a.akamaihd.net/client/installer".to_string(),
            arma_workshop_url: "https://reforger.armaplatform.com/workshop".to_string(),
            log_file: "logs/longbow.log".to_string(),
            minimum_log_level: "Debug".to_string(),
            auto_restart_time_ms: 2000,
        }
    }
}
