use std::path::PathBuf;

use crate::models::ToolProperties;

use super::error::ServiceError;

/// Loads and persists `ToolProperties` to/from a JSON file on disk (default: `./properties.json`).
///
/// No singleton pattern — construct explicitly via [`ToolPropertiesService::load`].
pub struct ToolPropertiesService {
    path: PathBuf,
    properties: ToolProperties,
}

impl ToolPropertiesService {
    /// Loads properties from `path` if it exists. If the file doesn't exist, builds
    /// `ToolProperties::default()` and writes it to disk immediately. If the file exists but
    /// fails to deserialize, returns an `Err` (no silent fallback to defaults).
    pub fn load(path: impl Into<PathBuf>) -> Result<Self, ServiceError> {
        let path = path.into();

        if path.exists() {
            let contents = std::fs::read_to_string(&path)?;
            let properties: ToolProperties = serde_json::from_str(&contents)?;
            Ok(Self { path, properties })
        } else {
            let properties = ToolProperties::default();
            let service = Self { path, properties };
            service.save()?;
            Ok(service)
        }
    }

    pub fn properties(&self) -> &ToolProperties {
        &self.properties
    }

    pub fn save(&self) -> Result<(), ServiceError> {
        let json = serde_json::to_string_pretty(&self.properties)?;
        std::fs::write(&self.path, json)?;
        Ok(())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn load_creates_default_file_when_missing() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("properties.json");
        assert!(!path.exists());

        let service = ToolPropertiesService::load(&path).unwrap();
        assert!(path.exists());
        assert_eq!(service.properties(), &ToolProperties::default());
    }

    #[test]
    fn load_fails_on_corrupt_file() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("properties.json");
        std::fs::write(&path, "not json at all").unwrap();

        let result = ToolPropertiesService::load(&path);
        assert!(result.is_err());
    }
}
