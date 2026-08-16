//! Tauri commands for mod list management (Configuration screen's mod transfer list).

use crate::models::Mod;
use crate::state::AppState;

#[derive(Debug, Clone, serde::Serialize)]
pub struct ModLists {
    pub available: Vec<Mod>,
    pub enabled: Vec<Mod>,
}

pub(crate) async fn mod_lists(state: &tauri::State<'_, AppState>) -> ModLists {
    let config = state.config.lock().await;
    ModLists {
        available: config.available_mods().to_vec(),
        enabled: config.enabled_mods().to_vec(),
    }
}

#[tauri::command]
pub async fn get_mod_lists(state: tauri::State<'_, AppState>) -> Result<ModLists, String> {
    Ok(mod_lists(&state).await)
}

pub(crate) async fn persist_mods_database(state: &tauri::State<'_, AppState>) -> Result<(), String> {
    let config = state.config.lock().await;
    let mut all = config.available_mods().to_vec();
    all.extend(config.enabled_mods().iter().cloned());
    drop(config);
    let file_io = state.file_io.lock().await;
    file_io.write_mods_database(&all).map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn add_mod(state: tauri::State<'_, AppState>, m: Mod) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        config.move_mod_to_disabled(m);
    }
    persist_mods_database(&state).await?;
    Ok(mod_lists(&state).await)
}

#[tauri::command]
pub async fn remove_mod(state: tauri::State<'_, AppState>, m: Mod) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        config.remove_mod(&m);
    }
    persist_mods_database(&state).await?;
    Ok(mod_lists(&state).await)
}

#[tauri::command]
pub async fn enable_mod(state: tauri::State<'_, AppState>, m: Mod) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        config.move_mod_to_enabled(m);
    }
    Ok(mod_lists(&state).await)
}

#[tauri::command]
pub async fn disable_mod(state: tauri::State<'_, AppState>, m: Mod) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        config.move_mod_to_disabled(m);
    }
    Ok(mod_lists(&state).await)
}

#[tauri::command]
pub async fn enable_all_mods(state: tauri::State<'_, AppState>) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        let available: Vec<Mod> = config.available_mods().to_vec();
        for m in available {
            config.move_mod_to_enabled(m);
        }
        config.alphabetise_mod_lists();
    }
    Ok(mod_lists(&state).await)
}

#[tauri::command]
pub async fn disable_all_mods(state: tauri::State<'_, AppState>) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        let enabled: Vec<Mod> = config.enabled_mods().to_vec();
        for m in enabled {
            config.move_mod_to_disabled(m);
        }
        config.alphabetise_mod_lists();
    }
    Ok(mod_lists(&state).await)
}

/// Reorders `enabled_mods` by moving the mod matching `m` up (`delta = -1`) or down
/// (`delta = 1`) one position. Load order is meaningful for enabled mods, so this must not
/// re-sort the list afterwards.
#[tauri::command]
pub async fn move_enabled_mod(
    state: tauri::State<'_, AppState>,
    m: Mod,
    delta: i32,
) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        config.move_enabled_mod(&m, delta);
    }
    Ok(mod_lists(&state).await)
}

/// Applies an edit to an existing mod, keeping it in whichever list it's currently in (and at
/// its current load-order position if enabled).
#[tauri::command]
pub async fn update_mod(
    state: tauri::State<'_, AppState>,
    original: Mod,
    updated: Mod,
) -> Result<ModLists, String> {
    {
        let mut config = state.config.lock().await;
        config.update_mod(&original, updated);
    }
    persist_mods_database(&state).await?;
    Ok(mod_lists(&state).await)
}

#[tauri::command]
pub async fn export_mods_list_to_path(
    state: tauri::State<'_, AppState>,
    path: String,
) -> Result<(), String> {
    let config = state.config.lock().await;
    let mods = config.enabled_mods().to_vec();
    drop(config);
    let file_io = state.file_io.lock().await;
    file_io
        .save_mods_list_to_file(std::path::Path::new(&path), &mods)
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn import_mods_list_from_path(
    state: tauri::State<'_, AppState>,
    path: String,
) -> Result<ModLists, String> {
    let file_io = state.file_io.lock().await;
    let mods = file_io
        .load_mods_list_from_file(std::path::Path::new(&path))
        .map_err(|e| e.to_string())?;
    drop(file_io);
    {
        let mut config = state.config.lock().await;
        config.import_mods_list(mods);
    }
    Ok(mod_lists(&state).await)
}
