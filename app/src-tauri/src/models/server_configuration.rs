use serde::{Deserialize, Serialize};

use super::mod_entry::Mod;

/// Returns the list of supported platforms for the given cross-platform setting. Port of
/// the C# `Utilities.GetSupportedPlatforms`.
pub fn supported_platforms(cross_platform: bool) -> Vec<String> {
    let mut platforms = vec![crate::util::SUPPORTED_PLATFORM_PC.to_string()];
    if cross_platform {
        platforms.push(crate::util::SUPPORTED_PLATFORM_XBOX.to_string());
        platforms.push(crate::util::SUPPORTED_PLATFORM_PSN.to_string());
    }
    platforms
}

/// Enum representing the permissions for RCon clients.
#[derive(Debug, Clone, Copy, Serialize, Deserialize, PartialEq, Eq, Default)]
#[serde(rename_all = "lowercase")]
pub enum RconPermission {
    Admin,
    #[default]
    Monitor,
}

/// Top-level wrapper matching the on-disk `server.json`.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Default)]
#[serde(rename_all = "camelCase")]
pub struct ServerConfiguration {
    #[serde(default)]
    pub root: Root,
}

impl ServerConfiguration {
    /// Display `ServerConfiguration` in the JSON format required by the Arma Server files.
    pub fn to_json_string(&self) -> serde_json::Result<String> {
        serde_json::to_string_pretty(&self.root)
    }

    /// Deserialize a JSON string into a `ServerConfiguration`.
    pub fn from_json_str(json: &str) -> serde_json::Result<ServerConfiguration> {
        let root: Root = serde_json::from_str(json)?;
        Ok(ServerConfiguration { root })
    }

    /// Checks the fields the dedicated server's own JSON schema rejects at startup, so a bad
    /// value is reported immediately instead of after SteamCMD and a full engine initialization
    /// — at which point it surfaces as a wall of `BACKEND (E)` schema output.
    ///
    /// Deliberately narrow: this mirrors specific, documented schema constraints rather than
    /// second-guessing the server about what a valid config looks like in general.
    pub fn validate_for_start(&self) -> Result<(), String> {
        if self.root.game.name.trim().is_empty() {
            return Err(
                "Server Name is required — set one on the Configuration tab before starting the \
                 server."
                    .to_string(),
            );
        }

        let scenario_id = self.root.game.scenario_id.trim();
        if scenario_id.is_empty() {
            return Err(
                "No scenario is selected — pick one with the Select… button on the Configuration \
                 tab before starting the server."
                    .to_string(),
            );
        }
        if !SCENARIO_ID_REGEX.is_match(scenario_id) {
            return Err(format!(
                "Scenario ID '{scenario_id}' isn't in the format the server accepts. It must be a \
                 16-character resource GUID in braces followed by a path, e.g. \
                 {{ECC61978EDCC2B5A}}Missions/23_Campaign.conf."
            ));
        }

        Ok(())
    }
}

/// The pattern the dedicated server's config schema enforces on `game.scenarioId`, copied from
/// the schema error the server itself emits when the value doesn't match.
static SCENARIO_ID_REGEX: std::sync::LazyLock<regex::Regex> = std::sync::LazyLock::new(|| {
    regex::Regex::new(r"^\{[0-9A-F]{16}\}[a-zA-Z0-9_./ -]+$").expect("invalid SCENARIO_ID_REGEX")
});


/// Structure representing the root of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct Root {
    #[serde(default)]
    pub bind_address: String,
    #[serde(default)]
    pub bind_port: u16,
    #[serde(default)]
    pub public_address: String,
    #[serde(default)]
    pub public_port: u16,
    #[serde(default)]
    pub a2s: A2S,
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub rcon: Option<Rcon>,
    #[serde(default)]
    pub game: Game,
    #[serde(default)]
    pub operating: Operating,
}

impl Default for Root {
    fn default() -> Self {
        Self {
            bind_address: "0.0.0.0".to_string(),
            bind_port: 2001,
            public_address: String::new(),
            public_port: 2001,
            a2s: A2S::default(),
            rcon: None,
            game: Game::default(),
            operating: Operating::default(),
        }
    }
}

/// Structure representing the a2s block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq)]
#[serde(rename_all = "camelCase")]
pub struct A2S {
    #[serde(default)]
    pub address: String,
    #[serde(default)]
    pub port: u16,
}

impl Default for A2S {
    fn default() -> Self {
        Self {
            address: "0.0.0.0".to_string(),
            port: 17777,
        }
    }
}

/// Structure representing the rcon block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct Rcon {
    #[serde(default)]
    pub address: String,
    #[serde(default)]
    pub port: u16,
    #[serde(default)]
    pub password: String,
    #[serde(default)]
    pub permission: RconPermission,
    #[serde(default)]
    pub blacklist: Vec<String>,
    #[serde(default)]
    pub whitelist: Vec<String>,
    #[serde(default)]
    pub max_clients: u8,
}

impl Default for Rcon {
    fn default() -> Self {
        Self {
            address: String::new(),
            port: 19999,
            password: String::new(),
            permission: RconPermission::Monitor,
            blacklist: Vec::new(),
            whitelist: Vec::new(),
            max_clients: 16,
        }
    }
}

/// Structure representing the game block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct Game {
    #[serde(default)]
    pub name: String,
    #[serde(default)]
    pub password: String,
    #[serde(default)]
    pub password_admin: String,
    #[serde(default)]
    pub admins: Vec<String>,
    #[serde(default)]
    pub scenario_id: String,
    #[serde(default)]
    pub max_players: u32,
    #[serde(default)]
    pub visible: bool,
    #[serde(default)]
    pub cross_platform: bool,
    #[serde(default)]
    pub supported_platforms: Vec<String>,
    #[serde(default)]
    pub game_properties: GameProperties,
    #[serde(default)]
    pub mods: Vec<Mod>,
    #[serde(default)]
    pub mods_required_by_default: bool,
}

impl Default for Game {
    fn default() -> Self {
        Self {
            name: String::new(),
            password: String::new(),
            password_admin: String::new(),
            admins: Vec::new(),
            scenario_id: String::new(),
            max_players: 64,
            visible: true,
            cross_platform: false,
            supported_platforms: Vec::new(),
            game_properties: GameProperties::default(),
            mods: Vec::new(),
            mods_required_by_default: false,
        }
    }
}

/// Structure representing the gameProperties block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct GameProperties {
    #[serde(default)]
    pub server_max_view_distance: u32,
    #[serde(default)]
    pub server_min_grass_distance: u32,
    #[serde(default)]
    pub network_view_distance: u32,
    #[serde(default)]
    pub disable_third_person: bool,
    #[serde(default)]
    pub fast_validation: bool,
    #[serde(default)]
    pub battl_eye: bool,
    #[serde(default, rename = "VONDisableUI")]
    pub von_disable_ui: bool,
    #[serde(default, rename = "VONDisableDirectSpeechUI")]
    pub von_disable_direct_speech_ui: bool,
    #[serde(default, rename = "VONCanTransmitCrossFaction")]
    pub von_can_transmit_cross_faction: bool,
    #[serde(default)]
    pub persistence: Persistence,
    #[serde(default = "default_mission_header")]
    pub mission_header: serde_json::Value,
}

fn default_mission_header() -> serde_json::Value {
    serde_json::json!({})
}

impl Default for GameProperties {
    fn default() -> Self {
        Self {
            server_max_view_distance: 1600,
            server_min_grass_distance: 50,
            network_view_distance: 1500,
            disable_third_person: false,
            fast_validation: true,
            battl_eye: true,
            von_disable_ui: false,
            von_disable_direct_speech_ui: false,
            von_can_transmit_cross_faction: false,
            persistence: Persistence::default(),
            mission_header: default_mission_header(),
        }
    }
}

/// Structure representing the Persistence block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct Persistence {
    #[serde(default)]
    pub auto_save_interval: u32,
    #[serde(default)]
    pub hive_id: u32,
    #[serde(default = "default_json_object")]
    pub databases: serde_json::Value,
    #[serde(default = "default_json_object")]
    pub storages: serde_json::Value,
}

fn default_json_object() -> serde_json::Value {
    serde_json::json!({})
}

impl Default for Persistence {
    fn default() -> Self {
        Self {
            auto_save_interval: 10,
            hive_id: 0,
            databases: default_json_object(),
            storages: default_json_object(),
        }
    }
}

/// Structure representing the operating block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct Operating {
    #[serde(default)]
    pub lobby_player_synchronise: bool,
    #[serde(default)]
    pub player_save_time: u32,
    #[serde(default)]
    pub ai_limit: i32,
    #[serde(default)]
    pub slot_reservation_timeout: u32,
    /// `None` = absent/off, `Some(vec![])` = disable ALL navmeshes, `Some([...])` = disable
    /// specific ones. This 3-state meaning must be preserved.
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub disable_navmesh_streaming: Option<Vec<String>>,
    #[serde(default)]
    pub disable_server_shutdown: bool,
    #[serde(default)]
    pub disable_crash_reporter: bool,
    // The game's schema (and the C# original) spell this `disableAI`, which camelCase
    // derivation would render as `disableAi` — an explicit rename is required, exactly like the
    // VON* fields above.
    #[serde(default, rename = "disableAI")]
    pub disable_ai: bool,
    #[serde(default)]
    pub join_queue: JoinQueue,
}

impl Default for Operating {
    fn default() -> Self {
        Self {
            lobby_player_synchronise: true,
            player_save_time: 120,
            ai_limit: -1,
            slot_reservation_timeout: 60,
            disable_navmesh_streaming: None,
            disable_server_shutdown: false,
            disable_crash_reporter: false,
            disable_ai: false,
            join_queue: JoinQueue::default(),
        }
    }
}

/// Structure representing the joinQueue block of the Server Config.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq, Default)]
#[serde(rename_all = "camelCase")]
pub struct JoinQueue {
    #[serde(default)]
    pub max_size: u32,
}


#[cfg(test)]
mod validation_tests {
    use super::*;

    fn valid_config() -> ServerConfiguration {
        let mut config = ServerConfiguration::default();
        config.root.game.name = "My Server".to_string();
        config.root.game.scenario_id = "{ECC61978EDCC2B5A}Missions/23_Campaign.conf".to_string();
        config
    }

    #[test]
    fn accepts_a_fully_populated_config() {
        assert_eq!(valid_config().validate_for_start(), Ok(()));
    }

    #[test]
    fn rejects_a_blank_or_whitespace_only_server_name() {
        for name in ["", "   "] {
            let mut config = valid_config();
            config.root.game.name = name.to_string();
            let err = config.validate_for_start().unwrap_err();
            assert!(err.contains("Server Name is required"), "unexpected error: {err}");
        }
    }

    #[test]
    fn rejects_a_missing_scenario() {
        let mut config = valid_config();
        config.root.game.scenario_id = String::new();
        let err = config.validate_for_start().unwrap_err();
        assert!(err.contains("No scenario is selected"), "unexpected error: {err}");
    }

    #[test]
    fn rejects_a_scenario_id_the_servers_schema_would_reject() {
        // Lowercase hex, a missing GUID, and a GUID with no path all fail the server's pattern.
        for bad in [
            "{ecc61978edcc2b5a}Missions/23_Campaign.conf",
            "Missions/23_Campaign.conf",
            "{ECC61978EDCC2B5A}",
        ] {
            let mut config = valid_config();
            config.root.game.scenario_id = bad.to_string();
            let err = config.validate_for_start().unwrap_err();
            assert!(err.contains("isn't in the format"), "expected a format error for {bad}, got: {err}");
        }
    }
}

#[cfg(test)]
mod schema_tests {
    use super::*;

    /// Every JSON key the Arma dedicated server expects in `server.json`, taken from the C#
    /// original's model (`ArmaReforgerServerTool/Models/ServerConfiguration.cs`).
    ///
    /// This guards a class of bug that is completely silent otherwise: serde's
    /// `rename_all = "camelCase"` derives `disableAI` as `disableAi`, which the game would not
    /// read and which broke the whole Configuration screen (the frontend bound `undefined` to a
    /// field and Svelte threw during render). Acronym-containing names need explicit renames.
    fn collect_keys(v: &serde_json::Value, out: &mut std::collections::BTreeSet<String>) {
        match v {
            serde_json::Value::Object(map) => {
                for (k, inner) in map {
                    out.insert(k.clone());
                    collect_keys(inner, out);
                }
            }
            serde_json::Value::Array(items) => {
                for item in items {
                    collect_keys(item, out);
                }
            }
            _ => {}
        }
    }

    #[test]
    fn serialized_keys_match_the_games_expected_schema() {
        let mut cfg = ServerConfiguration::default();
        // Populate the optional blocks so their keys are present in the comparison too.
        cfg.root.rcon = Some(Rcon::default());
        cfg.root.operating.disable_navmesh_streaming = Some(vec![]);

        let value = serde_json::to_value(&cfg).unwrap();
        let mut keys = std::collections::BTreeSet::new();
        collect_keys(&value, &mut keys);

        // Names that must appear verbatim, including the awkward-cased ones.
        for expected in [
            "disableAI",
            "VONDisableUI",
            "VONDisableDirectSpeechUI",
            "VONCanTransmitCrossFaction",
            "battlEye",
            "a2s",
            "disableNavmeshStreaming",
            "lobbyPlayerSynchronise",
            "modsRequiredByDefault",
            "passwordAdmin",
            "scenarioId",
            "maxClients",
        ] {
            assert!(
                keys.contains(expected),
                "server.json is missing the expected key `{expected}`; serialized keys were: {keys:?}"
            );
        }

        // And the mis-derived spellings must NOT appear.
        for forbidden in ["disableAi", "vonDisableUI", "battlEye2", "a2S"] {
            assert!(
                !keys.contains(forbidden),
                "server.json contains `{forbidden}`, which the game will not read"
            );
        }
    }
}
