//! Tauri commands for the Workshop browser modal: search and mod detail lookups. Adding a
//! selected mod to the server doesn't need a dedicated command — the frontend just calls the
//! existing `add_mod` then `enable_mod` commands with the data these return.

use crate::models::{WorkshopAssetDetail, WorkshopSearchResult};
use crate::state::AppState;

#[tauri::command]
pub async fn search_workshop_mods(
    state: tauri::State<'_, AppState>,
    query: Option<String>,
    page: u32,
    sort: Option<String>,
) -> Result<WorkshopSearchResult, String> {
    let workshop = state.workshop.lock().await;
    workshop
        .search(query.as_deref(), page, sort.as_deref())
        .await
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn get_workshop_mod_details(
    state: tauri::State<'_, AppState>,
    mod_id: String,
) -> Result<WorkshopAssetDetail, String> {
    let workshop = state.workshop.lock().await;
    workshop.get_details(&mod_id).await.map_err(|e| e.to_string())
}
