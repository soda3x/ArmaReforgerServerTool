//! Tauri commands for install-directory management, SteamCMD, and saved games. File/folder
//! *picking* itself happens in the frontend via the Tauri dialog plugin — these commands take
//! already-resolved paths.

use std::collections::HashMap;
use std::path::PathBuf;

use tauri::Manager;

use crate::state::AppState;

#[tauri::command]
pub async fn get_install_dir(state: tauri::State<'_, AppState>) -> Result<Option<String>, String> {
    let file_io = state.file_io.lock().await;
    Ok(file_io.install_dir().map(|p| p.display().to_string()))
}

#[tauri::command]
pub async fn is_steamcmd_installed(state: tauri::State<'_, AppState>) -> Result<bool, String> {
    Ok(state.file_io.lock().await.is_steamcmd_installed())
}

/// Name of the bundled loader mod archive shipped as a Tauri resource.
const NO_BACKEND_LOADER_ZIP: &str = "resources/NoBackendScenarioLoader_6324F7124A9768FB.zip";

/// Extracts the bundled "No Backend" scenario-loader mod into the install directory's `addons`
/// folder. The C# original did this whenever server files were located or downloaded — without
/// it, No Backend mode can't work, since the launch arguments reference a mod that isn't there.
/// Failures are logged rather than propagated: a missing loader only breaks No Backend mode,
/// and shouldn't block an otherwise-valid install from being selected.
async fn try_install_no_backend_loader(app: &tauri::AppHandle, state: &tauri::State<'_, AppState>) {
    let resource = match app
        .path()
        .resolve(NO_BACKEND_LOADER_ZIP, tauri::path::BaseDirectory::Resource)
    {
        Ok(path) => path,
        Err(e) => {
            tracing::warn!("Could not resolve the No Backend loader resource: {e}");
            return;
        }
    };

    let file_io = state.file_io.lock().await;
    if let Err(e) = file_io.install_no_backend_scenario_loader(&resource) {
        tracing::warn!("Failed to install the No Backend scenario loader: {e}");
    }
}

/// Validates and sets the install directory (the frontend has already prompted the user via a
/// folder picker). Persists the chosen location to `state.json`.
#[tauri::command]
pub async fn locate_server_files(
    app: tauri::AppHandle,
    state: tauri::State<'_, AppState>,
    path: String,
) -> Result<(), String> {
    let path_buf = PathBuf::from(&path);
    crate::services::FileIoService::validate_server_install_dir(&path_buf)
        .map_err(|e| e.to_string())?;

    state.file_io.lock().await.set_install_dir(Some(path_buf));

    {
        let mut saved_state = state.saved_state.lock().await;
        saved_state.state_mut().server_location = path;
        saved_state.save().map_err(|e| e.to_string())?;
    }

    try_install_no_backend_loader(&app, &state).await;
    Ok(())
}

/// Sets the install directory without validation (used right before `download_steam_cmd`,
/// where the directory won't have server files yet).
#[tauri::command]
pub async fn set_install_dir(
    state: tauri::State<'_, AppState>,
    path: String,
) -> Result<(), String> {
    let path_buf = PathBuf::from(&path);
    state.file_io.lock().await.set_install_dir(Some(path_buf));

    let mut saved_state = state.saved_state.lock().await;
    saved_state.state_mut().server_location = path;
    saved_state.save().map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn download_steam_cmd(
    app: tauri::AppHandle,
    state: tauri::State<'_, AppState>,
) -> Result<(), String> {
    // Read the URL first and release that lock before taking `file_io`, so this path locks in
    // the same order as every other command (file_io is never held while acquiring another).
    let download_url = {
        let tool_properties = state.tool_properties.lock().await;
        format!(
            "{}/steamcmd.zip",
            tool_properties.properties().steam_cmd_download_url
        )
    };

    {
        let file_io = state.file_io.lock().await;
        file_io
            .download_steam_cmd(&download_url)
            .await
            .map_err(|e| e.to_string())?;
    }

    try_install_no_backend_loader(&app, &state).await;
    Ok(())
}

#[tauri::command]
pub async fn delete_server_files(state: tauri::State<'_, AppState>) -> Result<(), String> {
    {
        let file_io = state.file_io.lock().await;
        file_io.delete_server_files().map_err(|e| e.to_string())?;
    }
    state.file_io.lock().await.set_install_dir(None);

    let mut saved_state = state.saved_state.lock().await;
    saved_state.state_mut().server_location = String::new();
    saved_state.save().map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn get_saved_games(
    state: tauri::State<'_, AppState>,
) -> Result<HashMap<String, String>, String> {
    let file_io = state.file_io.lock().await;
    let games = file_io.get_saved_games().map_err(|e| e.to_string())?;
    Ok(games
        .into_iter()
        .map(|(name, path)| (name, path.display().to_string()))
        .collect())
}

#[tauri::command]
pub async fn rename_save(
    state: tauri::State<'_, AppState>,
    old_name: String,
    new_name: String,
) -> Result<String, String> {
    let file_io = state.file_io.lock().await;
    file_io
        .rename_save(&old_name, &new_name)
        .map_err(|e| e.to_string())
}

/// Returns the built-in scenario catalog (from `properties.json`). Workshop-mod scenario
/// scraping (the C# original's `Mod.GetScenariosForMod`, which scrapes the Arma workshop
/// website's HTML) is not yet ported — this only covers the default/base-game scenarios for
/// now. Manual scenario ID entry (already supported by the Configuration screen's scenario ID
/// field) remains the fallback for workshop scenarios in the meantime.
#[tauri::command]
pub async fn get_default_scenarios(
    state: tauri::State<'_, AppState>,
) -> Result<Vec<crate::models::Scenario>, String> {
    let tool_properties = state.tool_properties.lock().await;
    Ok(tool_properties.properties().default_scenarios.clone())
}

/// Maps an Arma "ping site" name (as scraped from the server log) to an ISO country code, so
/// the Status screen can show a flag next to it. Returns `None` for unrecognised sites.
#[tauri::command]
pub fn ping_site_country_code(ping_site: String) -> Option<String> {
    crate::util::ping_site_to_country_code(&ping_site).map(|c| c.to_string())
}
