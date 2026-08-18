//! Tauri commands for the RCON tab's player/session history panel.

use crate::models::PlayerSession;
use crate::state::AppState;

const DEFAULT_HISTORY_LIMIT: usize = 500;

#[tauri::command]
pub async fn list_player_sessions(state: tauri::State<'_, AppState>) -> Result<Vec<PlayerSession>, String> {
    state.history.lock().await.list_sessions(DEFAULT_HISTORY_LIMIT).map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn clear_player_history(state: tauri::State<'_, AppState>) -> Result<(), String> {
    state.history.lock().await.clear().map_err(|e| e.to_string())
}
