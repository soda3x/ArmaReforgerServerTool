//! Tauri commands for persisted advanced settings (Management screen's "Advanced" panel).

use std::collections::HashMap;

use crate::models::AdvancedSetting;
use crate::state::AppState;

#[tauri::command]
pub async fn get_advanced_settings(
    state: tauri::State<'_, AppState>,
) -> Result<HashMap<String, AdvancedSetting>, String> {
    Ok(state
        .saved_state
        .lock()
        .await
        .state()
        .advanced_settings
        .clone())
}

#[tauri::command]
pub async fn set_advanced_setting(
    state: tauri::State<'_, AppState>,
    setting: AdvancedSetting,
) -> Result<(), String> {
    let mut saved_state = state.saved_state.lock().await;
    saved_state
        .state_mut()
        .advanced_settings
        .insert(setting.name.clone(), setting);
    saved_state.save().map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn get_config_flags(
    state: tauri::State<'_, AppState>,
) -> Result<ConfigFlags, String> {
    let config = state.config.lock().await;
    Ok(ConfigFlags {
        auto_restart_on_crash: config.auto_restart_on_crash,
        use_experimental_server: config.use_experimental_server,
        no_backend: config.no_backend,
        using_save: config.using_save,
        save: config.save.clone(),
        log_level: config.log_level.clone(),
    })
}

#[tauri::command]
pub async fn set_config_flags(
    state: tauri::State<'_, AppState>,
    flags: ConfigFlags,
) -> Result<(), String> {
    let mut config = state.config.lock().await;
    config.auto_restart_on_crash = flags.auto_restart_on_crash;
    config.use_experimental_server = flags.use_experimental_server;
    config.no_backend = flags.no_backend;
    config.using_save = flags.using_save;
    config.save = flags.save;
    config.log_level = flags.log_level;
    Ok(())
}

#[derive(Debug, Clone, serde::Serialize, serde::Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ConfigFlags {
    pub auto_restart_on_crash: bool,
    pub use_experimental_server: bool,
    pub no_backend: bool,
    pub using_save: bool,
    pub save: Option<String>,
    #[serde(default = "default_log_level")]
    pub log_level: String,
}

fn default_log_level() -> String {
    crate::services::config_service::DEFAULT_LOG_LEVEL.to_string()
}
