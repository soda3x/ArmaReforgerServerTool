/******************************************************************************
 * File Name:    config_service.rs
 * Project:      Longbow / ARST-RUST
 * Description:  Port of Managers/ConfigurationManager.cs. Manages the server
 *               configuration model and the available/enabled mod lists.
 *
 *               No singleton pattern (per the Rust port's architecture
 *               decision) — construct explicitly via `ConfigService::new`
 *               and let the caller own/share the instance (e.g. wrapped in
 *               `tauri::State<Mutex<ConfigService>>` at the application
 *               layer).
 *
 *               GUI-control-dictionary concerns from the C# original
 *               (`m_serverParamsDictionary` / `m_advServerParamsDictionary`)
 *               are intentionally omitted — the frontend now holds the
 *               equivalent form state directly and sends plain data via
 *               Tauri commands.
 ******************************************************************************/

use crate::models::{Mod, ServerConfiguration};

use super::error::ServiceError;

/// Sentinel value representing "use whatever the latest save game is" for
/// [`ConfigService::save`]. Mirrors the C# `.LatestSave` constant.
pub const LATEST_SAVE_SENTINEL: &str = ".LatestSave";

/// Default `-logLevel` value, matching the C# log-level combo box's default.
pub const DEFAULT_LOG_LEVEL: &str = "normal";

/// How often the dedicated server should print performance stats, in milliseconds. These are
/// the `FPS: … Mem: … Player: …` lines that [`crate::util::ServerStatusParser`] scrapes to
/// drive the Status screen, so this argument is mandatory, not optional.
pub const LOG_STATS_INTERVAL_MS: u32 = 5000;

/// Manages the manipulation of the server configuration and its mod lists.
pub struct ConfigService {
    config: ServerConfiguration,
    available_mods: Vec<Mod>,
    enabled_mods: Vec<Mod>,
    pub auto_restart_on_crash: bool,
    pub use_experimental_server: bool,
    pub no_backend: bool,
    pub using_save: bool,
    /// Selected save-game name. `None` or `Some(LATEST_SAVE_SENTINEL)` both
    /// represent "use latest" per the C# original — represented explicitly
    /// here rather than relying on `None` alone.
    pub save: Option<String>,
    /// Server log verbosity, passed through as `-logLevel`. Matches the C#
    /// original's combo box values: normal / warning / error / fatal.
    pub log_level: String,
}

impl ConfigService {
    pub fn new() -> Self {
        Self {
            config: ServerConfiguration::default(),
            available_mods: Vec::new(),
            enabled_mods: Vec::new(),
            auto_restart_on_crash: false,
            use_experimental_server: false,
            no_backend: false,
            using_save: false,
            save: None,
            log_level: DEFAULT_LOG_LEVEL.to_string(),
        }
    }

    pub fn config(&self) -> &ServerConfiguration {
        &self.config
    }

    pub fn config_mut(&mut self) -> &mut ServerConfiguration {
        &mut self.config
    }

    pub fn available_mods(&self) -> &[Mod] {
        &self.available_mods
    }

    pub fn enabled_mods(&self) -> &[Mod] {
        &self.enabled_mods
    }

    /// Loads a `server.json` string into the model.
    ///
    /// Before loading, moves all currently-enabled mods back into
    /// `available_mods` (nothing lost). After a successful parse, merges the
    /// loaded config's `game.mods` into `enabled_mods`: for each loaded mod,
    /// if an existing enabled mod has the same name but a different version,
    /// the existing one is removed first (the loaded mod takes precedence),
    /// then the loaded mod is added to `enabled_mods` (and removed from
    /// `available_mods` if present there). Finally both lists are
    /// alphabetised.
    ///
    /// This is validate-then-commit: the incoming JSON is parsed into a
    /// local `ServerConfiguration` first, and `self` is only mutated after a
    /// successful parse. This fixes the C# original's non-atomic behavior,
    /// where a partially-failed load could leave the model inconsistent.
    pub fn populate_from_json(&mut self, json: &str) -> Result<(), ServiceError> {
        let parsed = ServerConfiguration::from_json_str(json)?;

        // Stage the mod-list merge on clones so `self` is untouched until we
        // know the whole operation succeeds.
        let mut staged_available = self.available_mods.clone();
        let mut staged_enabled = self.enabled_mods.clone();

        // First move all currently-enabled mods back to available so we
        // don't lose them.
        for m in std::mem::take(&mut staged_enabled) {
            if !staged_available.contains(&m) {
                staged_available.push(m);
            }
        }

        // Merge the loaded config's mods into the enabled list.
        for m in parsed.root.game.mods.iter().cloned() {
            if !staged_enabled.contains(&m) {
                // Same name, different version already present -> the
                // loaded mod takes precedence.
                staged_enabled.retain(|em| !(em.name == m.name && em.version != m.version));
                staged_enabled.push(m.clone());
            }

            if let Some(pos) = staged_available.iter().position(|am| *am == m) {
                staged_available.remove(pos);
            }
        }

        // Commit.
        self.config = parsed;
        self.available_mods = staged_available;
        self.enabled_mods = staged_enabled;
        self.alphabetise_mod_lists();

        Ok(())
    }

    /// Builds a fresh `ServerConfiguration` from the current model by
    /// cloning `self.config` and overwriting `root.game.mods` with the
    /// current `enabled_mods`, and `root.game.supported_platforms` from the
    /// current cross-platform setting. Does NOT touch `scenario_id` or
    /// `mission_header` — those are set directly elsewhere, matching the C#
    /// original's comment that those two fields bypass this create/populate
    /// roundtrip.
    pub fn build_configuration(&self) -> ServerConfiguration {
        let mut cfg = self.config.clone();
        cfg.root.game.mods = self.enabled_mods.clone();
        cfg.root.game.supported_platforms =
            crate::models::supported_platforms(cfg.root.game.cross_platform);
        cfg
    }

    /// Replaces `enabled_mods` with `imported`. Mirrors the C# `ImportModsList`:
    /// first moves any currently-enabled mods back into `available_mods` (so
    /// nothing is lost), then for each imported mod, moves a matching
    /// existing `available_mods` entry across (by `Mod` equality — name +
    /// mod_id) if present, otherwise adds the imported mod directly. Both
    /// lists are re-alphabetised afterward.
    pub fn import_mods_list(&mut self, imported: Vec<Mod>) {
        let mut staged_available = self.available_mods.clone();

        for m in self.enabled_mods.drain(..) {
            if !staged_available.contains(&m) {
                staged_available.push(m);
            }
        }

        let mut staged_enabled = Vec::with_capacity(imported.len());
        for m in imported {
            if let Some(pos) = staged_available.iter().position(|am| *am == m) {
                staged_enabled.push(staged_available.remove(pos));
            } else {
                staged_enabled.push(m);
            }
        }

        self.available_mods = staged_available;
        self.enabled_mods = staged_enabled;
        self.alphabetise_mod_lists();
    }

    /// Moves `m` from `available_mods` into `enabled_mods`, deduping via
    /// `Mod` equality (name + mod_id).
    pub fn move_mod_to_enabled(&mut self, m: Mod) {
        if !self.enabled_mods.contains(&m) {
            self.enabled_mods.push(m.clone());
        }
        self.available_mods.retain(|am| *am != m);
    }

    /// Moves `m` from `enabled_mods` into `available_mods`, deduping via
    /// `Mod` equality (name + mod_id).
    pub fn move_mod_to_disabled(&mut self, m: Mod) {
        if !self.available_mods.contains(&m) {
            self.available_mods.push(m.clone());
        }
        self.enabled_mods.retain(|em| *em != m);
    }

    /// Removes `m` entirely from both `available_mods` and `enabled_mods` (e.g. the user
    /// deleted a mod entry outright, as opposed to just disabling it).
    pub fn remove_mod(&mut self, m: &Mod) {
        self.available_mods.retain(|am| am != m);
        self.enabled_mods.retain(|em| em != m);
    }

    /// Replaces the entry matching `original` with `updated`, in whichever list currently holds
    /// it and at the same position.
    ///
    /// This exists because `Mod`'s identity (`PartialEq`) is name + mod_id only: an edit that
    /// changes just `version` or `required` looks like the *same* mod to every dedup check, so
    /// naive add/remove paths silently discard it. Editing an enabled mod must also keep it
    /// enabled (and keep its load-order position), which a remove-then-add cannot do.
    pub fn update_mod(&mut self, original: &Mod, updated: Mod) {
        if let Some(pos) = self.enabled_mods.iter().position(|em| em == original) {
            self.enabled_mods[pos] = updated;
            return;
        }
        if let Some(pos) = self.available_mods.iter().position(|am| am == original) {
            self.available_mods[pos] = updated;
            self.alphabetise_mod_lists();
            return;
        }
        // Not currently tracked — treat as a plain add.
        self.available_mods.push(updated);
        self.alphabetise_mod_lists();
    }

    /// Moves the enabled mod matching `m` by `delta` positions (-1 up, +1 down).
    ///
    /// Deliberately does NOT re-sort: `enabled_mods` is a load-order list, so alphabetising it
    /// here would immediately undo the move the user just made.
    pub fn move_enabled_mod(&mut self, m: &Mod, delta: i32) {
        let Some(pos) = self.enabled_mods.iter().position(|em| em == m) else {
            return;
        };
        let new_pos = pos as i32 + delta;
        if new_pos < 0 || new_pos as usize >= self.enabled_mods.len() {
            return;
        }
        self.enabled_mods.swap(pos, new_pos as usize);
    }

    /// Sorts the *available* mods list by name, case-insensitively.
    ///
    /// `enabled_mods` is deliberately left alone: unlike the available catalogue, the enabled
    /// list is a load-order list that the user controls (and that arrives in a meaningful order
    /// from a loaded `server.json` or an imported mods file). The C# original sorted both,
    /// which silently fought with its own reorder buttons.
    pub fn alphabetise_mod_lists(&mut self) {
        self.available_mods.sort_by_key(|m| m.name.to_lowercase());
    }

    /// Builds the raw CLI arg string for "No Backend" mode.
    pub fn create_no_backend_launch_arguments(&self) -> String {
        let mut mod_ids: Vec<String> =
            vec![crate::util::NO_BACKEND_SCENARIO_LOADER_MOD_ID.to_string()];
        for m in &self.enabled_mods {
            mod_ids.push(m.mod_id.clone());
        }
        let mods = mod_ids.join(",");

        format!(
            "-adminPassword \"{}\" -addons {} -server worlds/NoBackendScenarioLoader.ent -scenarioId {} -bindIP {} -publicAddress {}",
            self.config.root.game.password_admin,
            mods,
            self.config.root.game.scenario_id,
            self.config.root.bind_address,
            self.config.root.public_address,
        )
    }
}

impl Default for ConfigService {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn minimal_server_json() -> &'static str {
        r#"{
            "bindAddress": "0.0.0.0",
            "bindPort": 2001,
            "publicAddress": "1.2.3.4",
            "publicPort": 2001,
            "a2s": { "address": "0.0.0.0", "port": 17777 },
            "game": {
                "name": "Test Server",
                "password": "",
                "passwordAdmin": "admin",
                "admins": [],
                "scenarioId": "{ECC61978EDCC2B5A}Missions/23_Campaign.conf",
                "maxPlayers": 32,
                "visible": true,
                "mods": [
                    { "modId": "AAAA1111", "name": "Zeta Mod" },
                    { "modId": "BBBB2222", "name": "Alpha Mod" }
                ]
            },
            "operating": {}
        }"#
    }

    #[test]
    fn populate_from_json_round_trips_and_enables_mods() {
        let mut svc = ConfigService::new();
        svc.populate_from_json(minimal_server_json()).unwrap();

        assert_eq!(svc.config().root.game.name, "Test Server");
        assert_eq!(svc.config().root.public_address, "1.2.3.4");
        assert_eq!(svc.enabled_mods().len(), 2);
        assert!(svc.available_mods().is_empty());

        // Load order from the config file is preserved verbatim — NOT alphabetised, because
        // mod load order is significant to the game.
        assert_eq!(svc.enabled_mods()[0].name, "Zeta Mod");
        assert_eq!(svc.enabled_mods()[1].name, "Alpha Mod");
    }

    #[test]
    fn move_enabled_mod_reorders_and_survives_sorting() {
        let mut svc = ConfigService::new();
        svc.enabled_mods.push(Mod::new_latest("1", "Alpha", false));
        svc.enabled_mods.push(Mod::new_latest("2", "Beta", false));
        svc.enabled_mods.push(Mod::new_latest("3", "Gamma", false));

        // Move Gamma up one slot.
        let gamma = Mod::new_latest("3", "Gamma", false);
        svc.move_enabled_mod(&gamma, -1);
        let names: Vec<&str> = svc.enabled_mods().iter().map(|m| m.name.as_str()).collect();
        assert_eq!(names, vec!["Alpha", "Gamma", "Beta"]);

        // Sorting the catalogue must not disturb the enabled load order.
        svc.alphabetise_mod_lists();
        let names: Vec<&str> = svc.enabled_mods().iter().map(|m| m.name.as_str()).collect();
        assert_eq!(names, vec!["Alpha", "Gamma", "Beta"]);
    }

    #[test]
    fn move_enabled_mod_clamps_at_list_edges() {
        let mut svc = ConfigService::new();
        svc.enabled_mods.push(Mod::new_latest("1", "Alpha", false));
        svc.enabled_mods.push(Mod::new_latest("2", "Beta", false));

        svc.move_enabled_mod(&Mod::new_latest("1", "Alpha", false), -1);
        svc.move_enabled_mod(&Mod::new_latest("2", "Beta", false), 1);

        let names: Vec<&str> = svc.enabled_mods().iter().map(|m| m.name.as_str()).collect();
        assert_eq!(names, vec!["Alpha", "Beta"]);
    }

    #[test]
    fn update_mod_edits_version_in_place_and_keeps_it_enabled() {
        let mut svc = ConfigService::new();
        svc.enabled_mods.push(Mod::new_latest("1", "Alpha", false));
        svc.enabled_mods.push(Mod::new_latest("2", "Beta", false));

        let original = Mod::new_latest("2", "Beta", false);
        let updated = Mod::new("2", "Beta", "1.4.2", true);
        svc.update_mod(&original, updated);

        // Still enabled, still in position 1, and the edit actually took effect — the case the
        // old remove-then-add path silently dropped, since Mod equality ignores version.
        assert_eq!(svc.enabled_mods().len(), 2);
        assert_eq!(svc.enabled_mods()[1].name, "Beta");
        assert_eq!(svc.enabled_mods()[1].version, "1.4.2");
        assert!(svc.enabled_mods()[1].required);
        assert!(svc.available_mods().is_empty());
    }

    #[test]
    fn populate_from_json_malformed_leaves_state_unchanged() {
        let mut svc = ConfigService::new();
        svc.populate_from_json(minimal_server_json()).unwrap();

        let name_before = svc.config().root.game.name.clone();
        let enabled_before = svc.enabled_mods().len();

        let result = svc.populate_from_json("{ not valid json ][");
        assert!(result.is_err());

        assert_eq!(svc.config().root.game.name, name_before);
        assert_eq!(svc.enabled_mods().len(), enabled_before);
    }

    #[test]
    fn import_mods_list_moves_available_mod_into_enabled() {
        let mut svc = ConfigService::new();
        let mod_a = Mod::new_latest("AAAA1111", "Available Mod", false);
        svc.available_mods.push(mod_a.clone());

        svc.import_mods_list(vec![mod_a.clone()]);

        assert_eq!(svc.enabled_mods().len(), 1);
        assert_eq!(svc.enabled_mods()[0], mod_a);
        assert!(svc.available_mods().is_empty());
    }

    #[test]
    fn alphabetise_mod_lists_orders_case_insensitively() {
        let mut svc = ConfigService::new();
        svc.available_mods.push(Mod::new_latest("1", "zebra", false));
        svc.available_mods.push(Mod::new_latest("2", "Apple", false));
        svc.available_mods.push(Mod::new_latest("3", "banana", false));

        svc.alphabetise_mod_lists();

        let names: Vec<&str> = svc.available_mods().iter().map(|m| m.name.as_str()).collect();
        assert_eq!(names, vec!["Apple", "banana", "zebra"]);
    }

    #[test]
    fn create_no_backend_launch_arguments_has_expected_shape() {
        let mut svc = ConfigService::new();
        svc.config_mut().root.game.password_admin = "hunter2".to_string();
        svc.config_mut().root.game.scenario_id = "{ID}Missions/Foo.conf".to_string();
        svc.config_mut().root.bind_address = "0.0.0.0".to_string();
        svc.config_mut().root.public_address = "5.6.7.8".to_string();
        svc.enabled_mods.push(Mod::new_latest("DEADBEEF", "Some Mod", false));

        let args = svc.create_no_backend_launch_arguments();

        let expected = format!(
            "-adminPassword \"hunter2\" -addons {},DEADBEEF -server worlds/NoBackendScenarioLoader.ent -scenarioId {{ID}}Missions/Foo.conf -bindIP 0.0.0.0 -publicAddress 5.6.7.8",
            crate::util::NO_BACKEND_SCENARIO_LOADER_MOD_ID
        );
        assert_eq!(args, expected);
    }
}
