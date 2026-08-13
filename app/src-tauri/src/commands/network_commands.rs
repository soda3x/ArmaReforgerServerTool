//! Tauri commands for UPnP / networking toggles.

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
