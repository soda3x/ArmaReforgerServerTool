//! Tauri commands for the Configuration screen: server config get/set, save/load to disk.

use crate::models::ServerConfiguration;
use crate::state::AppState;

#[tauri::command]
pub async fn get_server_configuration(
    state: tauri::State<'_, AppState>,
) -> Result<ServerConfiguration, String> {
    let config = state.config.lock().await;
    Ok(config.build_configuration())
}

#[tauri::command]
pub async fn set_server_configuration(
    state: tauri::State<'_, AppState>,
    configuration: ServerConfiguration,
) -> Result<(), String> {
    let mut config = state.config.lock().await;
    *config.config_mut() = configuration;
    Ok(())
}

/// Saves the current configuration (mods folded in) to `{install_dir}/server.json`. Fails if no
/// install directory is set.
#[tauri::command]
pub async fn save_server_configuration(state: tauri::State<'_, AppState>) -> Result<(), String> {
    let config = state.config.lock().await.build_configuration();
    let file_io = state.file_io.lock().await;
    let install_dir = file_io
        .install_dir()
        .ok_or_else(|| "No install directory is set".to_string())?;
    let path = install_dir.join(crate::util::SERVER_JSON_FILENAME);
    file_io
        .save_configuration_to_file(&path, &config)
        .map_err(|e| e.to_string())
}

/// Loads a `server.json` from an arbitrary path (the frontend resolves the path via the Tauri
/// dialog plugin) and populates the current configuration + mod lists from it.
#[tauri::command]
pub async fn load_server_configuration_from_path(
    state: tauri::State<'_, AppState>,
    path: String,
) -> Result<ServerConfiguration, String> {
    let file_io = state.file_io.lock().await;
    let loaded = file_io
        .load_configuration_from_file(std::path::Path::new(&path))
        .map_err(|e| e.to_string())?;
    drop(file_io);

    let json = loaded.to_json_string().map_err(|e| e.to_string())?;
    let mut config = state.config.lock().await;
    config.populate_from_json(&json).map_err(|e| e.to_string())?;
    Ok(config.build_configuration())
}

#[tauri::command]
pub async fn save_server_configuration_to_path(
    state: tauri::State<'_, AppState>,
    path: String,
) -> Result<(), String> {
    let config = state.config.lock().await.build_configuration();
    let file_io = state.file_io.lock().await;
    file_io
        .save_configuration_to_file(std::path::Path::new(&path), &config)
        .map_err(|e| e.to_string())
}
