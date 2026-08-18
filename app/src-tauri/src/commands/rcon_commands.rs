//! Tauri commands for the RCON tab: connect/disconnect and the player/ban/console operations
//! built on top of `RconService`, plus the two background forwarders that keep it wired into the
//! rest of the app (frontend event relay + session history, and auto-connect/disconnect off the
//! server's own lifecycle).

use tauri::{Emitter, Manager};

use crate::models::{BanEntry, RconPlayer};
use crate::services::rcon_service::{RconEvent, RconStatus};
use crate::state::AppState;

/// The Tauri event name used to forward every [`RconEvent`] to the frontend, mirroring
/// `process_commands::SERVER_EVENT`.
pub const RCON_EVENT: &str = "rcon-event";

/// Spawns a background task that forwards every `RconEvent` to the frontend and, on
/// `PlayersUpdated`/disconnect, feeds `SessionHistoryService` so join/leave history stays up to
/// date without every call site having to remember to update it. Call once during app setup,
/// alongside `process_commands::spawn_event_forwarder`.
pub fn spawn_rcon_event_forwarder(app_handle: tauri::AppHandle) {
    let mut rx = app_handle.state::<AppState>().rcon.subscribe();
    tauri::async_runtime::spawn(async move {
        loop {
            match rx.recv().await {
                Ok(event) => {
                    let _ = app_handle.emit(RCON_EVENT, &event);

                    let state = app_handle.state::<AppState>();
                    match &event {
                        RconEvent::PlayersUpdated { players } => {
                            let _ = state.history.lock().await.record_snapshot(players);
                        }
                        RconEvent::ConnectionChanged { connected: false } => {
                            let _ = state.history.lock().await.close_all_open_sessions();
                        }
                        _ => {}
                    }
                }
                Err(tokio::sync::broadcast::error::RecvError::Lagged(_)) => continue,
                Err(tokio::sync::broadcast::error::RecvError::Closed) => break,
            }
        }
    });
}

/// Spawns a background task that auto-connects RCON when the server comes online (if `root.rcon`
/// has a non-empty address/port/password configured) and disconnects it when the server stops.
/// This is on top of the RCON tab's manual Connect/Disconnect buttons, which still work for cases
/// auto-connect doesn't cover (e.g. RCON enabled in the config after the server was already
/// running).
pub fn spawn_rcon_autoconnect(app_handle: tauri::AppHandle) {
    let mut rx = app_handle.state::<AppState>().process.subscribe();
    tauri::async_runtime::spawn(async move {
        let mut was_online = false;
        loop {
            match rx.recv().await {
                Ok(crate::services::process_service::ProcessEvent::Status(status)) => {
                    let state = app_handle.state::<AppState>();
                    if status.server_online && !was_online {
                        was_online = true;
                        let rcon_config = state.config.lock().await.build_configuration().root.rcon.clone();
                        if let Some(rcon) = rcon_config {
                            if !rcon.address.is_empty() && rcon.port != 0 && !rcon.password.is_empty() {
                                let _ = state.rcon.connect(&rcon.address, rcon.port, &rcon.password).await;
                            }
                        }
                    } else if !status.server_online && was_online {
                        was_online = false;
                        state.rcon.disconnect().await;
                    }
                }
                Ok(_) => {}
                Err(tokio::sync::broadcast::error::RecvError::Lagged(_)) => continue,
                Err(tokio::sync::broadcast::error::RecvError::Closed) => break,
            }
        }
    });
}

#[tauri::command]
pub async fn rcon_connect(
    state: tauri::State<'_, AppState>,
    address: String,
    port: u16,
    password: String,
) -> Result<(), String> {
    state.rcon.connect(&address, port, &password).await.map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn rcon_disconnect(state: tauri::State<'_, AppState>) -> Result<(), String> {
    state.rcon.disconnect().await;
    Ok(())
}

#[tauri::command]
pub async fn rcon_get_status(state: tauri::State<'_, AppState>) -> Result<RconStatus, String> {
    Ok(state.rcon.status().await)
}

#[tauri::command]
pub async fn rcon_send_raw_command(state: tauri::State<'_, AppState>, command: String) -> Result<String, String> {
    state.rcon.send_raw(&command).await.map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn rcon_list_players(state: tauri::State<'_, AppState>) -> Result<Vec<RconPlayer>, String> {
    state.rcon.list_players().await.map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn rcon_kick(state: tauri::State<'_, AppState>, player_id: String) -> Result<String, String> {
    state.rcon.kick(&player_id).await.map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn rcon_ban_create(
    state: tauri::State<'_, AppState>,
    identity_id: String,
    duration_secs: u64,
    reason: String,
) -> Result<String, String> {
    state.rcon.ban_create(&identity_id, duration_secs, &reason).await.map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn rcon_ban_remove(state: tauri::State<'_, AppState>, identity_id: String) -> Result<String, String> {
    state.rcon.ban_remove(&identity_id).await.map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn rcon_ban_list(state: tauri::State<'_, AppState>, page: Option<u32>) -> Result<Vec<BanEntry>, String> {
    state.rcon.ban_list(page).await.map_err(|e| e.to_string())
}
