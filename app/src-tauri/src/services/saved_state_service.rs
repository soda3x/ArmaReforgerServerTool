use std::path::{Path, PathBuf};

use crate::models::SavedState;

use super::error::ServiceError;

/// Loads and persists `SavedState` to/from a JSON file on disk (default: `./state.json`).
///
/// No singleton pattern (per the Rust port's architecture decision) — construct explicitly
/// via [`SavedStateService::load`] and let the caller own/share the instance (e.g. wrapped in
/// `tauri::State<Mutex<SavedStateService>>` at the application layer).
pub struct SavedStateService {
    path: PathBuf,
    state: SavedState,
}

impl SavedStateService {
    /// Loads the saved state from `path` if it exists (deserializing via serde — the model's
    /// `#[serde(default)]` fields handle forward/backward compatibility). If the file doesn't
    /// exist, builds `SavedState::default_state()` and writes it to disk immediately.
    ///
    /// If the file exists but fails to deserialize (corrupt/malformed), this returns an `Err`
    /// rather than silently falling back to defaults, so the caller/frontend can surface a
    /// clear error to the user.
    pub fn load(path: impl Into<PathBuf>) -> Result<Self, ServiceError> {
        let path = path.into();

        if path.exists() {
            let contents = std::fs::read_to_string(&path)?;
            let state: SavedState = serde_json::from_str(&contents)?;
            Ok(Self { path, state })
        } else {
            let state = SavedState::default_state();
            let service = Self { path, state };
            service.save()?;
            Ok(service)
        }
    }

    pub fn state(&self) -> &SavedState {
        &self.state
    }

    pub fn state_mut(&mut self) -> &mut SavedState {
        &mut self.state
    }

    /// Writes `self.state` to `self.path` as pretty JSON.
    pub fn save(&self) -> Result<(), ServiceError> {
        let json = serde_json::to_string_pretty(&self.state)?;
        std::fs::write(&self.path, json)?;
        Ok(())
    }

    pub fn path(&self) -> &Path {
        &self.path
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn load_creates_default_file_when_missing() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("state.json");
        assert!(!path.exists());

        let service = SavedStateService::load(&path).unwrap();
        assert!(path.exists());
        assert_eq!(service.state(), &SavedState::default_state());
    }

    #[test]
    fn load_reads_existing_file() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("state.json");

        let mut state = SavedState::default_state();
        state.server_location = "C:/servers/arma".to_string();
        std::fs::write(&path, serde_json::to_string_pretty(&state).unwrap()).unwrap();

        let service = SavedStateService::load(&path).unwrap();
        assert_eq!(service.state().server_location, "C:/servers/arma");
    }

    #[test]
    fn load_fails_on_corrupt_file() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("state.json");
        std::fs::write(&path, "{ not valid json").unwrap();

        let result = SavedStateService::load(&path);
        assert!(result.is_err());
    }

    #[test]
    fn save_persists_mutations() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("state.json");

        let mut service = SavedStateService::load(&path).unwrap();
        service.state_mut().server_location = "D:/foo".to_string();
        service.save().unwrap();

        let reloaded = SavedStateService::load(&path).unwrap();
        assert_eq!(reloaded.state().server_location, "D:/foo");
    }
}
