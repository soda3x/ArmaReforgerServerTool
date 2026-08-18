//! Tauri commands for the Management tab's Backup/Restore panel.

use crate::models::BackupInfo;
use crate::state::AppState;

#[tauri::command]
pub async fn create_backup(state: tauri::State<'_, AppState>, label: String) -> Result<BackupInfo, String> {
    let install_dir = {
        let file_io = state.file_io.lock().await;
        file_io.install_dir().ok_or_else(|| "No install directory is set".to_string())?.to_path_buf()
    };
    let config = state.config.lock().await.build_configuration();
    state.backups.lock().await.create_backup(&install_dir, &config, &label).map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn list_backups(state: tauri::State<'_, AppState>) -> Result<Vec<BackupInfo>, String> {
    state.backups.lock().await.list_backups().map_err(|e| e.to_string())
}

/// Restores `backup_id` over the current install directory's `saves/`, then applies the
/// archived `server.json` to the in-memory configuration (mirroring `set_server_configuration` —
/// the frontend is still responsible for calling `save_server_configuration` if it wants that
/// persisted to disk immediately). Refuses to run while the server is started, since overwriting
/// live save files out from under a running server risks corrupting them.
#[tauri::command]
pub async fn restore_backup(
    state: tauri::State<'_, AppState>,
    backup_id: String,
) -> Result<crate::models::ServerConfiguration, String> {
    if state.process.is_server_started().await {
        return Err("Stop the server before restoring a backup.".to_string());
    }

    let install_dir = {
        let file_io = state.file_io.lock().await;
        file_io.install_dir().ok_or_else(|| "No install directory is set".to_string())?.to_path_buf()
    };
    let current_config = state.config.lock().await.build_configuration();

    let restored = state
        .backups
        .lock()
        .await
        .restore_backup(&backup_id, &install_dir, &current_config)
        .map_err(|e| e.to_string())?;

    *state.config.lock().await.config_mut() = restored.clone();
    Ok(restored)
}

#[tauri::command]
pub async fn delete_backup(state: tauri::State<'_, AppState>, backup_id: String) -> Result<(), String> {
    state.backups.lock().await.delete_backup(&backup_id).map_err(|e| e.to_string())
}
