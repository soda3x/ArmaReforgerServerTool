mod commands;
mod logging;
mod models;
mod services;
mod state;
mod util;

use state::AppState;
use tauri::Manager;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_fs::init())
        .setup(|app| {
            let config_dir = app.path().app_config_dir()?;

            // Logging first, so failures in the rest of setup are actually recorded.
            let guard = logging::init(&config_dir.join("logs"), "debug");
            if let Some(guard) = guard {
                app.manage(guard);
            }
            tracing::info!("Longbow starting; config directory: {}", config_dir.display());

            // Without this, a panic inside a spawned async task (e.g. mid-command, after the
            // command's own logging already ran) unwinds silently: the task just stops, nothing
            // is logged, and — because the IPC response is only ever sent from the successful
            // completion path — the frontend's `invoke()` promise never settles. That failure
            // mode is otherwise invisible from both the Rust and JS sides simultaneously.
            std::panic::set_hook(Box::new(|info| {
                tracing::error!("PANIC: {info}");
            }));

            let state = AppState::init(&config_dir).map_err(|e| {
                tracing::error!("Failed to initialize application state: {e}");
                e
            })?;
            app.manage(state);
            commands::process_commands::spawn_event_forwarder(app.handle().clone());
            tracing::info!("Application state initialized");
            Ok(())
        })
        .on_window_event(|window, event| {
            // Don't leave an orphaned dedicated server running after the manager closes —
            // mirrors the C# original's `Main.OnFormClosing` behavior.
            if let tauri::WindowEvent::Destroyed = event {
                window.state::<AppState>().process.kill_server_blocking();
            }
        })
        .invoke_handler(tauri::generate_handler![
            commands::get_server_configuration,
            commands::set_server_configuration,
            commands::save_server_configuration,
            commands::load_server_configuration_from_path,
            commands::save_server_configuration_to_path,
            commands::get_mod_lists,
            commands::add_mod,
            commands::remove_mod,
            commands::enable_mod,
            commands::disable_mod,
            commands::enable_all_mods,
            commands::disable_all_mods,
            commands::move_enabled_mod,
            commands::update_mod,
            commands::export_mods_list_to_path,
            commands::import_mods_list_from_path,
            commands::is_server_started,
            commands::start_stop_server,
            commands::kill_server,
            commands::build_launch_arguments_preview,
            commands::is_wsl_available,
            commands::list_wsl_distros,
            commands::get_use_upnp,
            commands::set_use_upnp,
            commands::get_install_dir,
            commands::is_steamcmd_installed,
            commands::locate_server_files,
            commands::set_install_dir,
            commands::download_steam_cmd,
            commands::delete_server_files,
            commands::get_saved_games,
            commands::rename_save,
            commands::check_for_updates,
            commands::get_default_scenarios,
            commands::ping_site_country_code,
            commands::get_advanced_settings,
            commands::set_advanced_setting,
            commands::get_config_flags,
            commands::set_config_flags,
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
