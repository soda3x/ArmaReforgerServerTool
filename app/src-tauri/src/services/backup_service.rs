//! Backup/restore for a server's save data (`{install_dir}/saves/`) and its `server.json`,
//! bundled together into one timestamped zip per backup — reuses the `zip` crate already used
//! elsewhere in this app for Workshop mod extraction (`file_io_service::extract_zip`), just in
//! the write direction.

use std::io::Write;
use std::path::{Path, PathBuf};

use chrono::Local;

use crate::models::{BackupInfo, ServerConfiguration};

use super::error::ServiceError;

/// Filename, inside the archive, that the live `server.json` is stored under.
const CONFIG_ENTRY_NAME: &str = "server.json";
/// Directory prefix, inside the archive, that the `saves/` tree is stored under.
const SAVES_ENTRY_PREFIX: &str = "saves/";
const ZIP_EXTENSION: &str = "zip";

pub struct BackupService {
    backups_dir: PathBuf,
}

impl BackupService {
    pub fn new(backups_dir: impl Into<PathBuf>) -> Self {
        Self { backups_dir: backups_dir.into() }
    }

    /// Zips `{install_dir}/saves/` (if present) plus `config` (serialized fresh, capturing
    /// whatever's currently loaded — not necessarily what's on disk) into a new timestamped
    /// archive.
    pub fn create_backup(
        &self,
        install_dir: &Path,
        config: &ServerConfiguration,
        label: &str,
    ) -> Result<BackupInfo, ServiceError> {
        std::fs::create_dir_all(&self.backups_dir)?;

        let created_at = Local::now();
        let safe_label = sanitize_label(label);
        let filename = format!("{}_{safe_label}.{ZIP_EXTENSION}", created_at.format("%Y%m%d-%H%M%S"));
        let archive_path = self.backups_dir.join(&filename);

        let file = std::fs::File::create(&archive_path)?;
        let mut zip = zip::ZipWriter::new(file);
        let options: zip::write::FileOptions<'_, ()> =
            zip::write::FileOptions::default().compression_method(zip::CompressionMethod::Deflated);

        zip.start_file(CONFIG_ENTRY_NAME, options)?;
        zip.write_all(config.to_json_string()?.as_bytes())?;

        let saves_dir = install_dir.join("saves");
        if saves_dir.exists() {
            add_dir_to_zip(&mut zip, &saves_dir, SAVES_ENTRY_PREFIX, options)?;
        }

        zip.finish()?;

        Ok(BackupInfo {
            id: filename,
            created_at,
            label: label.to_string(),
            size_bytes: std::fs::metadata(&archive_path)?.len(),
        })
    }

    /// Lists backups newest-first. Reads size/mtime straight from each archive on disk rather
    /// than a separate index file, so a manually-added or -removed zip is picked up for free and
    /// there's nothing else that can drift out of sync with reality.
    pub fn list_backups(&self) -> Result<Vec<BackupInfo>, ServiceError> {
        if !self.backups_dir.exists() {
            return Ok(Vec::new());
        }

        let mut backups = Vec::new();
        for entry in std::fs::read_dir(&self.backups_dir)? {
            let entry = entry?;
            let path = entry.path();
            if path.extension().and_then(|e| e.to_str()) != Some(ZIP_EXTENSION) {
                continue;
            }
            let metadata = entry.metadata()?;
            let id = path.file_name().and_then(|n| n.to_str()).unwrap_or_default().to_string();
            let created_at = metadata
                .created()
                .or_else(|_| metadata.modified())
                .map(chrono::DateTime::<Local>::from)
                .unwrap_or_else(|_| Local::now());
            let label = label_from_filename(&id);

            backups.push(BackupInfo { id, created_at, label, size_bytes: metadata.len() });
        }

        backups.sort_by_key(|b| std::cmp::Reverse(b.created_at));
        Ok(backups)
    }

    /// Restores `backup_id` over `install_dir`'s `saves/` directory and returns the archived
    /// `server.json`, deserialized, for the caller to apply via the normal
    /// `set_server_configuration`/`save_server_configuration` flow.
    ///
    /// Takes an automatic safety backup (labeled `pre-restore`) of the *current* state before
    /// touching anything, so a restore that turns out to be a mistake is itself recoverable.
    /// Callers must ensure the server isn't running before calling this — restoring save files
    /// out from under a live server risks corrupting them.
    pub fn restore_backup(
        &self,
        backup_id: &str,
        install_dir: &Path,
        current_config: &ServerConfiguration,
    ) -> Result<ServerConfiguration, ServiceError> {
        let backup_path = self.resolve_id(backup_id)?;

        let _safety = self.create_backup(install_dir, current_config, "pre-restore")?;

        let file = std::fs::File::open(&backup_path)?;
        let mut archive = zip::ZipArchive::new(file)?;

        let restored_config = {
            let mut entry = archive.by_name(CONFIG_ENTRY_NAME)?;
            let mut contents = String::new();
            std::io::Read::read_to_string(&mut entry, &mut contents)?;
            ServerConfiguration::from_json_str(&contents)?
        };

        let saves_dir = install_dir.join("saves");
        if saves_dir.exists() {
            std::fs::remove_dir_all(&saves_dir)?;
        }

        for i in 0..archive.len() {
            let mut entry = archive.by_index(i)?;
            let Some(name) = entry.enclosed_name() else { continue };
            let Ok(relative) = name.strip_prefix("saves") else { continue };

            let out_path = saves_dir.join(relative);
            if entry.is_dir() {
                std::fs::create_dir_all(&out_path)?;
            } else {
                if let Some(parent) = out_path.parent() {
                    std::fs::create_dir_all(parent)?;
                }
                let mut out_file = std::fs::File::create(&out_path)?;
                std::io::copy(&mut entry, &mut out_file)?;
            }
        }

        Ok(restored_config)
    }

    pub fn delete_backup(&self, backup_id: &str) -> Result<(), ServiceError> {
        let path = self.resolve_id(backup_id)?;
        std::fs::remove_file(path)?;
        Ok(())
    }

    /// Resolves a backup id (a bare filename) to its full path, rejecting anything that would
    /// escape `backups_dir` (e.g. a path-traversal id smuggled in from the frontend).
    fn resolve_id(&self, backup_id: &str) -> Result<PathBuf, ServiceError> {
        let candidate = Path::new(backup_id);
        if candidate.file_name() != Some(candidate.as_os_str()) {
            return Err(ServiceError::Other(format!("Invalid backup id: '{backup_id}'")));
        }
        let path = self.backups_dir.join(candidate);
        if !path.exists() {
            return Err(ServiceError::Other(format!("Backup '{backup_id}' does not exist.")));
        }
        Ok(path)
    }
}

/// Recursively adds every file under `src_dir` to `zip`, storing entries under
/// `{entry_prefix}{relative_path}`.
fn add_dir_to_zip(
    zip: &mut zip::ZipWriter<std::fs::File>,
    src_dir: &Path,
    entry_prefix: &str,
    options: zip::write::FileOptions<'_, ()>,
) -> Result<(), ServiceError> {
    for entry in walk_files(src_dir)? {
        let relative = entry.strip_prefix(src_dir).expect("walked path is under src_dir");
        let entry_name = format!("{entry_prefix}{}", relative.to_string_lossy().replace('\\', "/"));
        zip.start_file(entry_name, options)?;
        let mut file = std::fs::File::open(&entry)?;
        std::io::copy(&mut file, zip)?;
    }
    Ok(())
}

/// Recursively lists every regular file under `dir`.
fn walk_files(dir: &Path) -> Result<Vec<PathBuf>, ServiceError> {
    let mut files = Vec::new();
    let mut stack = vec![dir.to_path_buf()];
    while let Some(current) = stack.pop() {
        for entry in std::fs::read_dir(&current)? {
            let entry = entry?;
            let path = entry.path();
            if path.is_dir() {
                stack.push(path);
            } else {
                files.push(path);
            }
        }
    }
    Ok(files)
}

/// Keeps a user-supplied backup label filesystem-safe: only alphanumerics, dash, underscore
/// survive; anything else becomes `_`, and an empty result falls back to `"backup"`.
fn sanitize_label(label: &str) -> String {
    let sanitized: String = label
        .trim()
        .chars()
        .map(|c| if c.is_alphanumeric() || c == '-' || c == '_' { c } else { '_' })
        .collect();
    if sanitized.is_empty() {
        "backup".to_string()
    } else {
        sanitized
    }
}

/// Recovers the human label from a generated filename (`{timestamp}_{label}.zip`), falling back
/// to the whole stem for a file that doesn't match that shape (e.g. one a user renamed/added by
/// hand).
fn label_from_filename(filename: &str) -> String {
    let stem = filename.strip_suffix(&format!(".{ZIP_EXTENSION}")).unwrap_or(filename);
    match stem.split_once('_') {
        Some((prefix, label)) if prefix.len() == 15 && prefix.chars().all(|c| c.is_ascii_digit() || c == '-') => {
            label.to_string()
        }
        _ => stem.to_string(),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn sample_config() -> ServerConfiguration {
        let mut config = ServerConfiguration::default();
        config.root.game.name = "Test Server".to_string();
        config
    }

    #[test]
    fn create_backup_bundles_config_and_saves() {
        let dir = tempfile::tempdir().unwrap();
        let install_dir = dir.path().join("install");
        std::fs::create_dir_all(install_dir.join("saves").join("profile")).unwrap();
        std::fs::write(install_dir.join("saves").join("profile").join("save1.json"), "{}").unwrap();

        let backups_dir = dir.path().join("backups");
        let svc = BackupService::new(&backups_dir);

        let info = svc.create_backup(&install_dir, &sample_config(), "nightly").unwrap();
        assert!(info.id.ends_with("_nightly.zip"));
        assert!(backups_dir.join(&info.id).exists());

        let listed = svc.list_backups().unwrap();
        assert_eq!(listed.len(), 1);
        assert_eq!(listed[0].id, info.id);
        assert_eq!(listed[0].label, "nightly");
    }

    #[test]
    fn create_backup_without_a_saves_dir_still_captures_the_config() {
        let dir = tempfile::tempdir().unwrap();
        let install_dir = dir.path().join("install"); // no saves/ subfolder created
        std::fs::create_dir_all(&install_dir).unwrap();

        let svc = BackupService::new(dir.path().join("backups"));
        let info = svc.create_backup(&install_dir, &sample_config(), "empty").unwrap();
        assert!(info.size_bytes > 0);
    }

    #[test]
    fn restore_round_trips_saves_and_config() {
        let dir = tempfile::tempdir().unwrap();
        let install_dir = dir.path().join("install");
        std::fs::create_dir_all(install_dir.join("saves")).unwrap();
        std::fs::write(install_dir.join("saves").join("a.json"), "original").unwrap();

        let svc = BackupService::new(dir.path().join("backups"));
        let mut config = sample_config();
        config.root.game.name = "Backed Up Name".to_string();
        let info = svc.create_backup(&install_dir, &config, "snap").unwrap();

        // Mutate the live state after the backup, then restore over it.
        std::fs::write(install_dir.join("saves").join("a.json"), "mutated after backup").unwrap();
        let mut different_current = sample_config();
        different_current.root.game.name = "Different Current".to_string();

        let restored = svc.restore_backup(&info.id, &install_dir, &different_current).unwrap();
        assert_eq!(restored.root.game.name, "Backed Up Name");
        assert_eq!(
            std::fs::read_to_string(install_dir.join("saves").join("a.json")).unwrap(),
            "original"
        );

        // The pre-restore safety backup should exist alongside the original.
        assert_eq!(svc.list_backups().unwrap().len(), 2);
    }

    #[test]
    fn delete_backup_removes_the_archive() {
        let dir = tempfile::tempdir().unwrap();
        let svc = BackupService::new(dir.path().join("backups"));
        let install_dir = dir.path().join("install");
        std::fs::create_dir_all(&install_dir).unwrap();

        let info = svc.create_backup(&install_dir, &sample_config(), "to-delete").unwrap();
        svc.delete_backup(&info.id).unwrap();
        assert!(svc.list_backups().unwrap().is_empty());
    }

    #[test]
    fn resolve_id_rejects_path_traversal() {
        let dir = tempfile::tempdir().unwrap();
        let svc = BackupService::new(dir.path().join("backups"));
        assert!(svc.delete_backup("../../etc/passwd").is_err());
        assert!(svc.delete_backup("nested/escape.zip").is_err());
    }

    #[test]
    fn sanitize_label_strips_unsafe_characters() {
        assert_eq!(sanitize_label("my backup!/2"), "my_backup__2");
        assert_eq!(sanitize_label(""), "backup");
        assert_eq!(sanitize_label("nightly-01_test"), "nightly-01_test");
    }
}
