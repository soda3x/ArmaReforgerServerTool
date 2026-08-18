//! Tauri commands for UPnP / networking toggles and reachability checks.

use crate::services::network_service::PortCheckResult;
use crate::state::AppState;

#[tauri::command]
pub async fn get_use_upnp(state: tauri::State<'_, AppState>) -> Result<bool, String> {
    Ok(state.network.lock().await.use_upnp)
}

#[tauri::command]
pub async fn set_use_upnp(state: tauri::State<'_, AppState>, enabled: bool) -> Result<(), String> {
    state.network.lock().await.use_upnp = enabled;
    let mut saved_state = state.saved_state.lock().await;
    if let Some(setting) = saved_state.state_mut().advanced_settings.get_mut("useUpnp") {
        setting.enabled = enabled;
    }
    saved_state.save().map_err(|e| e.to_string())
}

/// Checks whether the game/A2S/RCON ports from the current config have a live UPnP mapping on
/// the gateway. Read-only — safe to call whether or not the server is running.
#[tauri::command]
pub async fn check_port_reachability(
    state: tauri::State<'_, AppState>,
) -> Result<Vec<PortCheckResult>, String> {
    let config = state.config.lock().await.build_configuration();
    let mappings = crate::services::network_service::port_mappings_from_config(&config);
    let network = state.network.lock().await;
    network.check_port_mappings(&mappings).await.map_err(|e| e.to_string())
}
