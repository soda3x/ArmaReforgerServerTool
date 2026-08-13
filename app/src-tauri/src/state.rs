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
    ServiceError, ToolPropertiesService,
};

pub struct AppState {
    pub saved_state: Mutex<SavedStateService>,
    pub tool_properties: Mutex<ToolPropertiesService>,
    pub file_io: Mutex<FileIoService>,
    pub config: Mutex<ConfigService>,
    pub network: Mutex<NetworkService>,
    pub process: Arc<ProcessService>,
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
        let file_io = FileIoService::new(install_dir, mod_database_file);

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

        Ok(Self {
            saved_state: Mutex::new(saved_state),
            tool_properties: Mutex::new(tool_properties),
            file_io: Mutex::new(file_io),
            config: Mutex::new(config),
            network: Mutex::new(network),
            process: ProcessService::new(),
        })
    }
}
