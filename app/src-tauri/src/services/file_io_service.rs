use std::collections::HashMap;
use std::path::{Path, PathBuf};

use crate::models::{Mod, ServerConfiguration};

use super::error::ServiceError;

/// The name of the sentinel save file representing "use latest save", which is excluded from
/// the map returned by [`FileIoService::get_saved_games`].
const LATEST_SAVE_SENTINEL: &str = ".LatestSave";

/// All file/IO operations for the server tool, ported from `Managers/FileIOManager.cs`.
///
/// No singleton pattern, and no GUI: file/folder picking is a frontend concern now — every
/// method here takes already-resolved paths and returns `Result<T, ServiceError>`.
pub struct FileIoService {
    install_dir: Option<PathBuf>,
    mod_database_file: PathBuf,
}

impl FileIoService {
    pub fn new(install_dir: Option<PathBuf>, mod_database_file: impl Into<PathBuf>) -> Self {
        Self {
            install_dir,
            mod_database_file: mod_database_file.into(),
        }
    }

    pub fn set_install_dir(&mut self, dir: Option<PathBuf>) {
        self.install_dir = dir;
    }

    pub fn install_dir(&self) -> Option<&Path> {
        self.install_dir.as_deref()
    }

    /// `{install_dir}/steamcmd/steamcmd.exe`
    pub fn steamcmd_exe_path(&self) -> Option<PathBuf> {
        self.install_dir
            .as_ref()
            .map(|dir| dir.join("steamcmd").join("steamcmd.exe"))
    }

    /// `{install_dir}/saves/profile/.save/sessions`
    pub fn saves_path(&self) -> Option<PathBuf> {
        self.install_dir.as_ref().map(|dir| {
            dir.join("saves")
                .join("profile")
                .join(".save")
                .join("sessions")
        })
    }

    pub fn is_steamcmd_installed(&self) -> bool {
        self.steamcmd_exe_path().is_some_and(|p| p.exists())
    }

    /// Read the combined mod database (all mods, available+enabled) as a JSON array of `Mod`.
    /// Returns an empty `Vec` if the file doesn't exist.
    pub fn read_mods_database(&self) -> Result<Vec<Mod>, ServiceError> {
        if !self.mod_database_file.exists() {
            return Ok(Vec::new());
        }
        let contents = std::fs::read_to_string(&self.mod_database_file)?;
        let mods: Vec<Mod> = serde_json::from_str(contents.trim())?;
        Ok(mods)
    }

    /// Write the combined mod database as pretty JSON.
    pub fn write_mods_database(&self, mods: &[Mod]) -> Result<(), ServiceError> {
        let json = serde_json::to_string_pretty(mods)?;
        std::fs::write(&self.mod_database_file, json)?;
        Ok(())
    }

    /// Save the enabled mods list to an arbitrary export path (no dialog here — caller supplies
    /// the path).
    pub fn save_mods_list_to_file(&self, path: &Path, mods: &[Mod]) -> Result<(), ServiceError> {
        let json = serde_json::to_string_pretty(mods)?;
        std::fs::write(path, json)?;
        Ok(())
    }

    /// Load a mods list from an arbitrary path.
    pub fn load_mods_list_from_file(&self, path: &Path) -> Result<Vec<Mod>, ServiceError> {
        let contents = std::fs::read_to_string(path)?;
        let mods: Vec<Mod> = serde_json::from_str(&contents)?;
        Ok(mods)
    }

    /// Save the server configuration to an arbitrary path.
    pub fn save_configuration_to_file(
        &self,
        path: &Path,
        config: &ServerConfiguration,
    ) -> Result<(), ServiceError> {
        let json = config.to_json_string()?;
        std::fs::write(path, json)?;
        Ok(())
    }

    /// Load the server configuration from an arbitrary path.
    pub fn load_configuration_from_file(&self, path: &Path) -> Result<ServerConfiguration, ServiceError> {
        let contents = std::fs::read_to_string(path)?;
        let config = ServerConfiguration::from_json_str(&contents)?;
        Ok(config)
    }

    /// Deletes a file if it exists; no-op (`Ok`) if it doesn't.
    pub fn delete_file_if_exists(path: &Path) -> Result<(), ServiceError> {
        if path.exists() {
            std::fs::remove_file(path)?;
        }
        Ok(())
    }

    /// Deletes the entire install directory recursively.
    ///
    /// FIX vs the C# original: the C# version also tried to delete `SavedState.serverLocation`
    /// as if it were a file path (a bug, since it's the directory itself being deleted). In this
    /// port, deleting the directory recursively is the only step needed; the caller is
    /// responsible for clearing `SavedState.server_location` afterwards.
    pub fn delete_server_files(&self) -> Result<(), ServiceError> {
        match &self.install_dir {
            Some(dir) => {
                if dir.exists() {
                    std::fs::remove_dir_all(dir)?;
                }
                Ok(())
            }
            None => Err(ServiceError::Other(
                "No install directory is set".to_string(),
            )),
        }
    }

    /// Validates that `path` contains both `{path}/steamcmd/steamcmd.exe` AND
    /// `{path}/arma_reforger/ArmaReforgerServer.exe`.
    pub fn validate_server_install_dir(path: &Path) -> Result<(), ServiceError> {
        let steamcmd = path.join("steamcmd").join("steamcmd.exe");
        let server_exe = path.join("arma_reforger").join("ArmaReforgerServer.exe");

        if steamcmd.exists() && server_exe.exists() {
            Ok(())
        } else {
            Err(ServiceError::Other(format!(
                "Arma Reforger Server files could not be located at '{}'. Please confirm the chosen path or download the files to start.",
                path.display()
            )))
        }
    }

    /// Downloads `steamcmd.zip` from `download_url` and extracts it into
    /// `{install_dir}/steamcmd/`, then deletes the temp zip.
    pub async fn download_steam_cmd(&self, download_url: &str) -> Result<(), ServiceError> {
        let install_dir = self.install_dir.as_ref().ok_or_else(|| {
            ServiceError::Other("No install directory is set".to_string())
        })?;

        let zip_file_path = install_dir.join("steamcmd.zip");
        let extract_path = install_dir.join("steamcmd");

        std::fs::create_dir_all(install_dir)?;

        let response = reqwest::get(download_url).await?;
        let bytes = response.error_for_status()?.bytes().await?;
        std::fs::write(&zip_file_path, &bytes)?;

        Self::extract_zip(&zip_file_path, &extract_path, true)?;

        Self::delete_file_if_exists(&zip_file_path)?;

        Ok(())
    }

    /// Extracts the bundled `NoBackendScenarioLoader_6324F7124A9768FB.zip` resource into
    /// `{install_dir}/addons`, overwriting existing files.
    ///
    /// For now, this looks for the zip file at `resource_zip_path` rather than embedding it via
    /// `include_bytes!`.
    // TODO: embed via include_bytes! once resource path is finalized
    pub fn install_no_backend_scenario_loader(
        &self,
        resource_zip_path: &Path,
    ) -> Result<(), ServiceError> {
        let install_dir = self.install_dir.as_ref().ok_or_else(|| {
            ServiceError::Other("No install directory is set".to_string())
        })?;

        let addons_path = install_dir.join("addons");
        Self::extract_zip(resource_zip_path, &addons_path, true)
    }

    /// Extracts a zip archive at `zip_path` into `dest_dir`. If `overwrite` is false, existing
    /// files are left untouched.
    fn extract_zip(zip_path: &Path, dest_dir: &Path, overwrite: bool) -> Result<(), ServiceError> {
        std::fs::create_dir_all(dest_dir)?;
        let file = std::fs::File::open(zip_path)?;
        let mut archive = zip::ZipArchive::new(file)?;

        for i in 0..archive.len() {
            let mut entry = archive.by_index(i)?;
            let out_path = match entry.enclosed_name() {
                Some(path) => dest_dir.join(path),
                None => continue,
            };

            if entry.is_dir() {
                std::fs::create_dir_all(&out_path)?;
            } else {
                if let Some(parent) = out_path.parent() {
                    std::fs::create_dir_all(parent)?;
                }
                if !overwrite && out_path.exists() {
                    continue;
                }
                let mut out_file = std::fs::File::create(&out_path)?;
                std::io::copy(&mut entry, &mut out_file)?;
            }
        }

        Ok(())
    }

    /// Migrates the legacy pre-0.8.3 mod database format (plain text, lines of
    /// `modId,name[,version]`, `version` field `"latest"` treated as absent) into `Vec<Mod>`.
    /// Fails if any non-empty line has fewer than 2 comma-separated fields. This is a pure
    /// function: it does not touch the filesystem.
    pub fn parse_legacy_mod_database(contents: &str) -> Result<Vec<Mod>, ServiceError> {
        let mut mods = Vec::new();

        for line in contents.lines() {
            let line = line.trim();
            if line.is_empty() {
                continue;
            }

            let split: Vec<&str> = line.split(',').collect();
            if split.len() < 2 {
                return Err(ServiceError::Other(
                    "At least one legacy mod was in an invalid format.".to_string(),
                ));
            }

            let mod_id = split[0].trim().to_string();
            let name = split[1].trim().to_string();

            if split.len() > 2 {
                let version = split[2].trim();
                if !version.is_empty() && version != "latest" {
                    mods.push(Mod::new(mod_id, name, version.to_string(), false));
                    continue;
                }
            }

            mods.push(Mod::new_latest(mod_id, name, false));
        }

        Ok(mods)
    }

    /// Lists `*.json` save files under `saves_path()`, returning a map of display-name -> full
    /// path, excluding the `.LatestSave` sentinel file. Creates the saves directory if it
    /// doesn't exist. Returns an empty map if `install_dir` is `None`.
    pub fn get_saved_games(&self) -> Result<HashMap<String, PathBuf>, ServiceError> {
        let saves_path = match self.saves_path() {
            Some(path) => path,
            None => return Ok(HashMap::new()),
        };

        if !saves_path.exists() {
            std::fs::create_dir_all(&saves_path)?;
        }

        let mut saved_games = HashMap::new();

        for entry in std::fs::read_dir(&saves_path)? {
            let entry = entry?;
            let path = entry.path();
            if !path.is_file() {
                continue;
            }
            if path.extension().and_then(|e| e.to_str()) != Some("json") {
                continue;
            }

            let file_stem = match path.file_stem().and_then(|s| s.to_str()) {
                Some(stem) => stem,
                None => continue,
            };

            if file_stem == LATEST_SAVE_SENTINEL {
                continue;
            }

            saved_games.insert(file_stem.to_string(), path);
        }

        Ok(saved_games)
    }

    /// Renames a file within `saves_path()`. `old_name`/`new_name` are file *names*, not full
    /// paths. Returns the resulting name on success.
    pub fn rename_save(&self, old_name: &str, new_name: &str) -> Result<String, ServiceError> {
        let saves_path = self.saves_path().ok_or_else(|| {
            ServiceError::Other("No install directory is set".to_string())
        })?;

        let old_path = saves_path.join(old_name);
        let new_path = saves_path.join(new_name);

        if !old_path.exists() {
            return Err(ServiceError::Other(format!(
                "Save file '{}' does not exist",
                old_name
            )));
        }

        std::fs::rename(&old_path, &new_path)?;
        Ok(new_name.to_string())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    // --- parse_legacy_mod_database ---

    #[test]
    fn parse_legacy_mod_database_valid_multiline() {
        let contents = "12345,MyMod,1.2.3\n67890,OtherMod";
        let mods = FileIoService::parse_legacy_mod_database(contents).unwrap();
        assert_eq!(mods.len(), 2);
        assert_eq!(mods[0].mod_id, "12345");
        assert_eq!(mods[0].name, "MyMod");
        assert_eq!(mods[0].version, "1.2.3");
        assert_eq!(mods[1].mod_id, "67890");
        assert_eq!(mods[1].name, "OtherMod");
        assert_eq!(mods[1].version, "latest");
    }

    #[test]
    fn parse_legacy_mod_database_latest_version_treated_as_absent() {
        let contents = "12345,MyMod,latest";
        let mods = FileIoService::parse_legacy_mod_database(contents).unwrap();
        assert_eq!(mods.len(), 1);
        assert_eq!(mods[0].version, "latest");
    }

    #[test]
    fn parse_legacy_mod_database_invalid_line_returns_err() {
        let contents = "12345,MyMod\njustoneentrynocomma";
        let result = FileIoService::parse_legacy_mod_database(contents);
        assert!(result.is_err());
    }

    // --- validate_server_install_dir ---

    #[test]
    fn validate_server_install_dir_valid() {
        let dir = tempfile::tempdir().unwrap();
        let steamcmd_dir = dir.path().join("steamcmd");
        let arma_dir = dir.path().join("arma_reforger");
        std::fs::create_dir_all(&steamcmd_dir).unwrap();
        std::fs::create_dir_all(&arma_dir).unwrap();
        std::fs::write(steamcmd_dir.join("steamcmd.exe"), b"").unwrap();
        std::fs::write(arma_dir.join("ArmaReforgerServer.exe"), b"").unwrap();

        assert!(FileIoService::validate_server_install_dir(dir.path()).is_ok());
    }

    #[test]
    fn validate_server_install_dir_invalid_missing_files() {
        let dir = tempfile::tempdir().unwrap();
        assert!(FileIoService::validate_server_install_dir(dir.path()).is_err());
    }

    // --- mods database round-trip ---

    #[test]
    fn mods_database_round_trip() {
        let dir = tempfile::tempdir().unwrap();
        let db_path = dir.path().join("mod_database.json");
        let service = FileIoService::new(None, &db_path);

        let mods = vec![
            Mod::new("1", "First", "1.0.0", true),
            Mod::new_latest("2", "Second", false),
        ];

        service.write_mods_database(&mods).unwrap();
        let read_back = service.read_mods_database().unwrap();

        assert_eq!(read_back.len(), mods.len());
        assert_eq!(read_back[0].mod_id, mods[0].mod_id);
        assert_eq!(read_back[0].name, mods[0].name);
        assert_eq!(read_back[0].version, mods[0].version);
        assert_eq!(read_back[1].mod_id, mods[1].mod_id);
        assert_eq!(read_back[1].version, "latest");
    }

    #[test]
    fn read_mods_database_missing_file_returns_empty() {
        let dir = tempfile::tempdir().unwrap();
        let db_path = dir.path().join("does_not_exist.json");
        let service = FileIoService::new(None, &db_path);

        let mods = service.read_mods_database().unwrap();
        assert!(mods.is_empty());
    }

    #[test]
    fn get_saved_games_returns_empty_when_no_install_dir() {
        let service = FileIoService::new(None, "mod_database.json");
        let games = service.get_saved_games().unwrap();
        assert!(games.is_empty());
    }

    #[test]
    fn delete_file_if_exists_noop_when_missing() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("nope.txt");
        assert!(FileIoService::delete_file_if_exists(&path).is_ok());
    }
}
