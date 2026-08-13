//! Tauri commands for the Management/Status screens: server start/stop, launch-argument
//! building, and forwarding `ProcessService` events to the frontend.

use tauri::{Emitter, Manager};

use crate::models::LaunchArguments;
use crate::services::process_service::{ProcessEvent, StartServerContext};
use crate::services::wsl_service::ServerTarget;
use crate::state::AppState;

/// The Tauri event name used to forward every [`ProcessEvent`] to the frontend. The frontend
/// listens once via `listen('server-event', ...)` and dispatches on the event's `type` tag.
pub const SERVER_EVENT: &str = "server-event";

/// Spawns a background task that subscribes to `ProcessService`'s broadcast channel and
/// forwards every event to the frontend. Call this once during app setup.
pub fn spawn_event_forwarder(app_handle: tauri::AppHandle) {
    let state = app_handle.state::<AppState>();
    let mut rx = state.process.subscribe();
    // Use Tauri's async-runtime wrapper rather than `tokio::spawn` directly: this function is
    // called from the synchronous `setup()` hook, which runs before the app has fully entered
    // Tokio's runtime context in this Tauri version — `tokio::spawn` panics there with "there is
    // no reactor running", while `tauri::async_runtime::spawn` handles it correctly.
    tauri::async_runtime::spawn(async move {
        loop {
            match rx.recv().await {
                Ok(event) => {
                    let _ = app_handle.emit(SERVER_EVENT, &event);
                }
                Err(tokio::sync::broadcast::error::RecvError::Lagged(_)) => continue,
                Err(tokio::sync::broadcast::error::RecvError::Closed) => break,
            }
        }
    });
}

#[tauri::command]
pub async fn is_server_started(state: tauri::State<'_, AppState>) -> Result<bool, String> {
    Ok(state.process.is_server_started().await)
}

/// Builds a [`StartServerContext`] from current app state. `launch_arguments` is supplied by
/// the frontend (built from the Advanced settings screen); everything else is read from the
/// services already held in `AppState`.
async fn build_start_context(
    state: &tauri::State<'_, AppState>,
    launch_arguments: LaunchArguments,
    server_target: ServerTarget,
) -> Result<StartServerContext, String> {
    let mut launch_arguments = launch_arguments;

    let file_io = state.file_io.lock().await;
    let steamcmd_exe = file_io
        .steamcmd_exe_path()
        .ok_or_else(|| "No install directory is set".to_string())?;
    let install_dir = file_io
        .install_dir()
        .ok_or_else(|| "No install directory is set".to_string())?
        .to_path_buf();
    drop(file_io);

    let config = state.config.lock().await;
    let no_backend_args = config.create_no_backend_launch_arguments();
    let no_backend = config.no_backend;
    let use_experimental = config.use_experimental_server;
    let auto_restart_on_crash = config.auto_restart_on_crash;
    let using_save = config.using_save;
    let save_name = config
        .save
        .clone()
        .filter(|s| s != crate::services::config_service::LATEST_SAVE_SENTINEL);
    let config_bind_port = config.config().root.bind_port;
    let log_level = config.log_level.clone();
    drop(config);

    // The mandatory arguments the C# original always set in `Main.CreateLaunchArguments()`.
    // These are injected here rather than in the frontend because only the backend knows the
    // resolved install directory — and getting them wrong breaks things silently:
    // `-addonsDir` is what makes mods load, `-profile` is where saves go, and `-logStats` is
    // what makes the server emit the stat lines the Status screen's charts are built from.
    // Paths are quoted so `shell_split` keeps them intact when they contain spaces.
    let quoted = |p: std::path::PathBuf| format!("\"{}\"", p.display());
    launch_arguments.config = Some(crate::models::LaunchArgument::new(
        "config",
        quoted(install_dir.join("server.json")),
    ));
    launch_arguments.profile = Some(crate::models::LaunchArgument::new(
        "profile",
        quoted(install_dir.join("saves")),
    ));
    launch_arguments.addons_dir = Some(crate::models::LaunchArgument::new(
        "addonsDir",
        quoted(install_dir.join("addons")),
    ));
    launch_arguments.log_stats = Some(crate::models::LaunchArgument::new(
        "logStats",
        crate::services::config_service::LOG_STATS_INTERVAL_MS.to_string(),
    ));
    launch_arguments.log_level = Some(crate::models::LaunchArgument::new("logLevel", log_level));

    let saved_state = state.saved_state.lock().await;
    let keep_server_updated = saved_state
        .state()
        .advanced_settings
        .get("keepServerUpdated")
        .map(|s| s.enabled)
        .unwrap_or(true);
    drop(saved_state);

    let tool_properties = state.tool_properties.lock().await;
    let auto_restart_delay_ms = tool_properties.properties().auto_restart_time_ms;
    drop(tool_properties);

    Ok(StartServerContext {
        steamcmd_exe,
        install_dir,
        use_experimental,
        keep_server_updated,
        no_backend,
        no_backend_args,
        config_bind_port,
        using_save,
        save_name,
        launch_arguments,
        auto_restart_on_crash,
        auto_restart_delay_ms,
        server_target,
    })
}

/// Starts (or, if already running, stops) the server. Handles UPnP mapping/unmapping and
/// `server.json` persistence around the process lifecycle call, matching the C# original's
/// sequencing but as an explicit orchestration here rather than inside `ProcessService`.
#[tauri::command]
pub async fn start_stop_server(
    state: tauri::State<'_, AppState>,
    launch_arguments: LaunchArguments,
    server_target: ServerTarget,
) -> Result<(), String> {
    let already_started = state.process.is_server_started().await;

    if already_started {
        let file_io = state.file_io.lock().await;
        let _ = file_io.reset_server_file();
        drop(file_io);

        let config = state.config.lock().await.build_configuration();
        let network = state.network.lock().await;
        let mappings = crate::services::network_service::port_mappings_from_config(&config);
        let _ = network.remove_port_mappings(&mappings).await;
        drop(network);

        state
            .process
            .stop_server(false)
            .await
            .map_err(|e| e.to_string())
    } else {
        let config = state.config.lock().await.build_configuration();
        let file_io = state.file_io.lock().await;
        let install_dir = file_io
            .install_dir()
            .ok_or_else(|| "No install directory is set".to_string())?;
        let server_json_path = install_dir.join("server.json");
        file_io
            .save_configuration_to_file(&server_json_path, &config)
            .map_err(|e| e.to_string())?;
        drop(file_io);

        let network = state.network.lock().await;
        let mappings = crate::services::network_service::port_mappings_from_config(&config);
        let _ = network.configure_port_mappings(&mappings).await;
        drop(network);

        let ctx = build_start_context(&state, launch_arguments, server_target).await?;
        state
            .process
            .start_server(ctx, false)
            .await
            .map_err(|e| e.to_string())
    }
}

#[tauri::command]
pub async fn kill_server(state: tauri::State<'_, AppState>) -> Result<(), String> {
    state.process.kill_server().await;
    Ok(())
}

#[tauri::command]
pub async fn build_launch_arguments_preview(
    state: tauri::State<'_, AppState>,
    launch_arguments: LaunchArguments,
    server_target: ServerTarget,
) -> Result<String, String> {
    let ctx = build_start_context(&state, launch_arguments, server_target).await?;
    Ok(crate::services::process_service::ProcessService::build_launch_arguments(&ctx))
}

// Re-emit the event enum's variant shape to the frontend type layer implicitly via serde tags —
// no extra command needed, `ProcessEvent` is `Serialize` and forwarded verbatim by
// `spawn_event_forwarder`. This marker keeps the import used even if no command references it
// directly yet.
#[allow(dead_code)]
fn _process_event_type_anchor(_e: ProcessEvent) {}
