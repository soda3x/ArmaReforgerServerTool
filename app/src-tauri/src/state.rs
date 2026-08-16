//! Shared application state, wrapped by Tauri and injected into every command.
//!
//! Mirrors the C# singletons (`ConfigurationManager`, `FileIOManager`, `NetworkManager`,
//! `SavedStateManager`, `ToolPropertiesManager`) but as plain fields on one struct managed by
//! `tauri::State`, instead of `GetInstance()` globals. `ProcessService` manages its own internal
//! locking (see its doc comment) so it's stored as a bare `Arc`, not behind an extra `Mutex`.

use std::sync::Arc;

use tokio::sync::Mutex;

use crate::services::{
    ConfigService, FileIoService, NetworkService, ProcessService, SavedStateService,
    ServiceError, TemplateService, ToolPropertiesService, WorkshopService,
};

pub struct AppState {
    pub saved_state: Mutex<SavedStateService>,
    pub tool_properties: Mutex<ToolPropertiesService>,
    pub file_io: Mutex<FileIoService>,
    pub config: Mutex<ConfigService>,
    pub network: Mutex<NetworkService>,
    pub process: Arc<ProcessService>,
    pub workshop: Mutex<WorkshopService>,
    pub templates: Mutex<TemplateService>,
}

impl AppState {
    /// Builds initial application state, loading (or creating with defaults) `state.json` and
    /// `properties.json` from the OS app-config directory. Mirrors the C# `Program.cs`
    /// bootstrap order: tool properties + saved state first (self-contained, no dependencies),
    /// then everything that depends on them (`FileIoService` needs `mod_database_file` from
    /// tool properties and `server_location` from saved state).
    ///
    /// The C# original wrote these next to the executable via relative `./state.json` paths.
    /// That's not safe for an installed app — the working directory is whatever the launcher
    /// happened to set, and Program Files isn't writable — so config lives in the per-user
    /// app-config directory instead.
    pub fn init(config_dir: &std::path::Path) -> Result<Self, ServiceError> {
        std::fs::create_dir_all(config_dir)?;

        let tool_properties = ToolPropertiesService::load(config_dir.join("properties.json"))?;
        let saved_state = SavedStateService::load(config_dir.join("state.json"))?;

        let install_dir = {
            let loc = &saved_state.state().server_location;
            if loc.is_empty() {
                None
            } else {
                Some(std::path::PathBuf::from(loc))
            }
        };
        // `mod_database_file` defaults to a relative "./mod_database.json"; resolve it against
        // the config directory so it lands somewhere writable and predictable. An absolute
        // path configured by the user is respected as-is.
        let configured_db = std::path::PathBuf::from(&tool_properties.properties().mod_database_file);
        let mod_database_file = if configured_db.is_absolute() {
            configured_db
        } else {
            config_dir.join(configured_db.file_name().unwrap_or_else(|| "mod_database.json".as_ref()))
        };
        let file_io = FileIoService::new(install_dir, mod_database_file.clone());

        // One-time migration from the pre-0.8.3 mod database format (a plain-text file of
        // `modId,name[,version]` lines) to the current JSON format, matching what the C#
        // original did on startup. Only runs if the new-format file doesn't exist yet, so it
        // can't clobber a database that's already been migrated.
        let legacy_path = mod_database_file.with_extension("txt");
        if legacy_path.exists() && !mod_database_file.exists() {
            match std::fs::read_to_string(&legacy_path)
                .map_err(ServiceError::from)
                .and_then(|contents| FileIoService::parse_legacy_mod_database(&contents))
            {
                Ok(mods) => match file_io.write_mods_database(&mods) {
                    Ok(()) => {
                        tracing::info!(
                            "Migrated {} mod(s) from the legacy mod database at {}",
                            mods.len(),
                            legacy_path.display()
                        );
                        if let Err(e) = std::fs::remove_file(&legacy_path) {
                            tracing::warn!(
                                "Migrated the legacy mod database but could not remove the old \
                                 file at {}: {e}",
                                legacy_path.display()
                            );
                        }
                    }
                    Err(e) => tracing::error!("Failed to write migrated mod database: {e}"),
                },
                Err(e) => tracing::error!(
                    "Failed to migrate legacy mod database at {}: {e}",
                    legacy_path.display()
                ),
            }
        }

        let mut config = ConfigService::new();
        if let Ok(mods) = file_io.read_mods_database() {
            for m in mods {
                config.move_mod_to_disabled(m);
            }
            config.alphabetise_mod_lists();
        }

        let use_upnp = saved_state
            .state()
            .advanced_settings
            .get("useUpnp")
            .map(|s| s.enabled)
            .unwrap_or(true);
        let network = NetworkService::new(use_upnp);

        let workshop = WorkshopService::new(
            &tool_properties.properties().arma_workshop_url,
            env!("CARGO_PKG_VERSION"),
        );

        let templates = TemplateService::load(config_dir.join("mod_templates.json"));

        Ok(Self {
            saved_state: Mutex::new(saved_state),
            tool_properties: Mutex::new(tool_properties),
            file_io: Mutex::new(file_io),
            config: Mutex::new(config),
            network: Mutex::new(network),
            process: ProcessService::new(),
            workshop: Mutex::new(workshop),
            templates: Mutex::new(templates),
        })
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn init_migrates_a_legacy_mod_database_and_removes_the_old_file() {
        let dir = tempfile::tempdir().unwrap();
        // `mod_database_file` defaults to "./mod_database.json" resolved against the config
        // dir, so the legacy sibling file is "mod_database.txt" in that same directory.
        std::fs::write(dir.path().join("mod_database.txt"), "12345,MyMod,1.2.3\n67890,OtherMod").unwrap();

        let state = AppState::init(dir.path()).unwrap();

        assert!(
            !dir.path().join("mod_database.txt").exists(),
            "legacy file should be removed after a successful migration"
        );
        assert!(dir.path().join("mod_database.json").exists());

        let config = tokio::runtime::Runtime::new()
            .unwrap()
            .block_on(async { state.config.lock().await.available_mods().to_vec() });
        assert_eq!(config.len(), 2);
        assert!(config.iter().any(|m| m.name == "MyMod" && m.version == "1.2.3"));
        assert!(config.iter().any(|m| m.name == "OtherMod" && m.version == "latest"));
    }

    #[test]
    fn init_does_not_migrate_when_the_new_format_file_already_exists() {
        let dir = tempfile::tempdir().unwrap();
        std::fs::write(dir.path().join("mod_database.txt"), "12345,LegacyMod").unwrap();
        std::fs::write(dir.path().join("mod_database.json"), "[]").unwrap();

        let _state = AppState::init(dir.path()).unwrap();

        // The legacy file is left alone rather than risk clobbering an already-migrated (and
        // possibly since-edited) database.
        assert!(dir.path().join("mod_database.txt").exists());
    }
}
