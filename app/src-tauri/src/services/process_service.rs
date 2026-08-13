//! Server process lifecycle: SteamCMD download/update, spawning the dedicated server,
//! streaming its stdout/stderr, crash/fatal-error detection, and scheduled/auto restart.
//!
//! Ported from `Managers/ProcessManager.cs`. Architectural differences from the C# original:
//! - No singleton — this is a plain struct wrapped by the caller in `Arc<ProcessService>` (its
//!   own internal `Mutex` guards mutable state, so it's safe to call from multiple spawned
//!   tasks without an outer lock).
//! - No GUI coupling. Instead of raising WinForms events, this emits [`ProcessEvent`]s on a
//!   `tokio::sync::broadcast` channel; the Tauri commands layer subscribes and forwards them
//!   to the frontend via `app_handle.emit`.
//! - UPnP port mapping and `server.json` persistence are NOT done inside this service (unlike
//!   the C# `ProcessManager`, which reached into `NetworkManager`/`FileIOManager` directly).
//!   Those are orchestrated by the commands layer immediately before/after calling
//!   [`ProcessService::start_server`]/[`stop_server`], keeping this service focused purely on
//!   process lifecycle.
//! - Uses `tokio::io::AsyncBufReadExt::lines()` for line buffering instead of the C# original's
//!   manual char-by-char `\n`/`\r` scanning.

use std::path::PathBuf;
use std::sync::atomic::{AtomicBool, Ordering};
use std::sync::Arc;

use chrono::{Local, NaiveTime, Timelike};
use serde::Serialize;
use tokio::io::{AsyncBufReadExt, BufReader};
use tokio::process::Command;
use tokio::sync::{broadcast, Mutex};

use crate::models::LaunchArguments;
use crate::util::ServerStatusParser;

use super::error::ServiceError;
use super::wsl_service::{wsl_command, ServerTarget};

/// Icon the frontend's start/stop button should show — mirrors the C# `IconChar.Play`/`Stop`.
#[derive(Debug, Clone, Copy, PartialEq, Eq, Serialize)]
#[serde(rename_all = "camelCase")]
pub enum ServerButtonIcon {
    Play,
    Stop,
}

/// Replaces the C# `GuiModelEventArgs`.
#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct GuiModel {
    pub button_icon: ServerButtonIcon,
    pub enable_server_fields: bool,
    pub server_running_label_text: String,
    pub start_server_btn_enabled: bool,
}

/// Events emitted during the server lifecycle. The commands layer re-broadcasts these to the
/// frontend as Tauri events.
#[derive(Debug, Clone, Serialize)]
#[serde(tag = "type", rename_all = "camelCase")]
pub enum ProcessEvent {
    /// A single already-timestamped log line (SteamCMD or server stdout/stderr).
    Log { line: String },
    /// GUI state update (button icon/enabled state, running label).
    GuiUpdate(GuiModel),
    /// Parsed server telemetry (FPS/memory/player count/addresses/online state).
    Status(crate::util::ServerStatus),
}

/// Timestamp prefix matching the C# `Utilities.GetTimestamp()` (`yyyy-MM-dd HH:mm:ss`).
fn timestamp() -> String {
    Local::now().format("%Y-%m-%d %H:%M:%S").to_string()
}

fn log_line(msg: impl AsRef<str>) -> ProcessEvent {
    ProcessEvent::Log {
        line: format!("{}: {}", timestamp(), msg.as_ref()),
    }
}

/// Everything [`ProcessService::start_server`] needs to launch SteamCMD + the dedicated
/// server. Built by the commands layer from `ConfigService`/`FileIoService`/`SavedState`.
#[derive(Debug, Clone)]
pub struct StartServerContext {
    pub steamcmd_exe: PathBuf,
    pub install_dir: PathBuf,
    pub use_experimental: bool,
    pub keep_server_updated: bool,
    pub no_backend: bool,
    /// Pre-built via `ConfigService::create_no_backend_launch_arguments`, used only when
    /// `no_backend` is true.
    pub no_backend_args: String,
    pub config_bind_port: u16,
    pub using_save: bool,
    /// `None` = load latest save. `Some(name)` = load a specific named save.
    pub save_name: Option<String>,
    pub launch_arguments: LaunchArguments,
    pub auto_restart_on_crash: bool,
    pub auto_restart_delay_ms: u64,
    pub server_target: ServerTarget,
}

const SERVER_CURRENTLY_RUNNING_STR: &str = "Server is currently running";
const APP_ID_STANDARD: &str = "1874900";
const APP_ID_EXPERIMENTAL: &str = "1890870";

struct Inner {
    is_server_started: bool,
    is_server_using_timer: bool,
    /// Cancellation flag for the *current* run. A fresh flag is created on every start; setting
    /// it tells the in-flight run to kill SteamCMD (if still downloading), skip launching the
    /// server, and/or kill an already-running server. Replaces the previous design of storing
    /// the child handles for external killing, which couldn't cancel a pending launch.
    run_cancel: Option<Arc<AtomicBool>>,
    /// OS process id of the running server, kept so shutdown can kill it *synchronously*.
    /// The cancellation flag alone isn't enough at exit: it only asks the monitor task to do
    /// the killing, and during process teardown that task may never be scheduled again.
    server_pid: Option<u32>,
    last_start_ctx: Option<StartServerContext>,
    restart_timer_cancel: Option<Arc<AtomicBool>>,
}

impl Default for Inner {
    fn default() -> Self {
        Self {
            is_server_started: false,
            is_server_using_timer: false,
            run_cancel: None,
            server_pid: None,
            last_start_ctx: None,
            restart_timer_cancel: None,
        }
    }
}

pub struct ProcessService {
    inner: Mutex<Inner>,
    events: broadcast::Sender<ProcessEvent>,
}

impl ProcessService {
    pub fn new() -> Arc<Self> {
        let (tx, _rx) = broadcast::channel(1024);
        Arc::new(Self {
            inner: Mutex::new(Inner::default()),
            events: tx,
        })
    }

    /// Subscribe to lifecycle/log/status events (the commands layer forwards these to the
    /// frontend via Tauri's `emit`).
    pub fn subscribe(&self) -> broadcast::Receiver<ProcessEvent> {
        self.events.subscribe()
    }

    fn emit(&self, event: ProcessEvent) {
        // Ignore send errors — they just mean nobody is currently listening.
        let _ = self.events.send(event);
    }

    pub async fn is_server_started(&self) -> bool {
        self.inner.lock().await.is_server_started
    }

    pub async fn is_server_using_timer(&self) -> bool {
        self.inner.lock().await.is_server_using_timer
    }

    /// Builds the final CLI argument string for the dedicated server, mirroring
    /// `ProcessManager.GetLaunchArguments()`: a fixed, specific field order, `None` fields
    /// filtered out, and a `-config`/no-backend tail appended depending on mode.
    pub fn build_launch_arguments(ctx: &StartServerContext) -> String {
        let mut la = ctx.launch_arguments.clone();

        if ctx.using_save {
            la.load_session_save = Some(match &ctx.save_name {
                None => crate::models::LaunchArgument::switch("loadSessionSave"),
                Some(name) => {
                    crate::models::LaunchArgument::new("loadSessionSave", format!("\"{}\"", name))
                }
            });
        }

        let ordered = [
            &la.profile,
            &la.addons_dir,
            &la.log_stats,
            &la.max_fps,
            &la.bind_port,
            &la.auto_reload,
            &la.rpl_timeout_ms,
            &la.nds,
            &la.nwk_resolution,
            &la.staggering_budget,
            &la.streaming_budget,
            &la.streams_delta,
            &la.load_session_save,
            &la.freeze_check,
            &la.freeze_check_mode,
            &la.addons_repair,
            &la.auto_shutdown,
            &la.log_voting,
            &la.ai_partial_sim,
            &la.create_db,
            &la.debugger,
            &la.debugger_port,
            &la.disable_shaders_build,
            &la.generate_shaders,
            &la.rpl_encode_as_long_jobs,
            &la.job_sys_short_worker_count,
            &la.job_sys_long_worker_count,
            &la.force_disable_night_grain,
            &la.log_level,
        ];

        let mut args: Vec<String> = ordered
            .into_iter()
            .filter_map(|a| a.as_ref().map(|a| a.to_string()))
            .collect();

        if !ctx.no_backend {
            if let Some(config) = &la.config {
                args.push(config.to_string());
            }
        } else {
            args.push(ctx.no_backend_args.clone());
            if la.bind_port.is_none() {
                args.push(format!("-bindPort {}", ctx.config_bind_port));
            }
        }

        let joined = args.join(" ");
        tracing::info!("Launching server with the following launch arguments: \"{}\"", joined);
        joined
    }

    /// Starts (or, if already started, stops) the server — mirrors the C# `StartStopServer`
    /// toggle. `triggered_by_auto_restart` only affects log wording.
    pub async fn start_stop_server(
        self: &Arc<Self>,
        ctx: StartServerContext,
        triggered_by_auto_restart: bool,
    ) -> Result<(), ServiceError> {
        let started = self.inner.lock().await.is_server_started;
        if started {
            self.stop_server(triggered_by_auto_restart).await
        } else {
            self.start_server(ctx, triggered_by_auto_restart).await
        }
    }

    /// Spawns SteamCMD, waits for it to finish, then spawns the dedicated server and streams
    /// its output. Returns once SteamCMD *starts* (the rest runs in a background task) —
    /// callers are expected to have already saved `server.json` and configured UPnP mappings
    /// before calling this.
    ///
    /// Returns a boxed, dynamically-dispatched future rather than a plain `async fn`: this
    /// method is called recursively (indirectly, via a spawned task inside [`Self::stream_lines`]
    /// on crash-restart), and a plain `async fn` here creates a self-referential opaque-type
    /// cycle the compiler can't resolve. Boxing breaks the cycle.
    pub fn start_server<'a>(
        self: &'a Arc<Self>,
        ctx: StartServerContext,
        triggered_by_auto_restart: bool,
    ) -> std::pin::Pin<Box<dyn std::future::Future<Output = Result<(), ServiceError>> + Send + 'a>> {
        Box::pin(async move {
            let cancel = Arc::new(AtomicBool::new(false));
            {
                let mut inner = self.inner.lock().await;
                inner.is_server_started = true;
                inner.last_start_ctx = Some(ctx.clone());
                inner.run_cancel = Some(Arc::clone(&cancel));
            }

            tracing::info!(
                "ProcessService - {} server.",
                if triggered_by_auto_restart { "Automatically (re)started" } else { "User started" }
            );

            self.emit(ProcessEvent::GuiUpdate(GuiModel {
                button_icon: ServerButtonIcon::Stop,
                enable_server_fields: false,
                server_running_label_text: SERVER_CURRENTLY_RUNNING_STR.to_string(),
                start_server_btn_enabled: false,
            }));

            self.emit(log_line(if triggered_by_auto_restart {
                "Automatically restarted server."
            } else {
                "User started server."
            }));
            self.emit(log_line(
                "Downloading / updating Arma Reforger dedicated server files. Please be patient...",
            ));
            if ctx.use_experimental {
                self.emit(log_line(
                    "Server is using Experimental Branch. It is important to note that your server \
                     may not work as intended as the experimental branch frequently contains \
                     breaking changes.",
                ));
            }

            let this = Arc::clone(self);
            tokio::spawn(async move {
                if let Err(e) = this.run_steamcmd_then_server(ctx, cancel).await {
                    this.emit(log_line(format!("Server startup failed: {e}")));
                    this.mark_stopped().await;
                }
            });

            Ok(())
        })
    }

    /// Clears running state and tells the frontend the server is no longer up. Safe to call
    /// more than once (the events are idempotent).
    async fn mark_stopped(&self) {
        {
            let mut inner = self.inner.lock().await;
            inner.is_server_started = false;
        }
        self.emit(ProcessEvent::Status(crate::util::ServerStatus {
            server_online: false,
            ..Default::default()
        }));
        self.emit(ProcessEvent::GuiUpdate(GuiModel {
            button_icon: ServerButtonIcon::Play,
            enable_server_fields: true,
            server_running_label_text: String::new(),
            start_server_btn_enabled: true,
        }));
    }

    async fn run_steamcmd_then_server(
        self: &Arc<Self>,
        ctx: StartServerContext,
        cancel: Arc<AtomicBool>,
    ) -> Result<(), ServiceError> {
        let update_switch = if ctx.keep_server_updated { "+app_update" } else { "" };
        self.emit(log_line(if ctx.keep_server_updated {
            "Longbow will ensure the server is up-to-date."
        } else {
            "Longbow will not update the dedicated server."
        }));

        let (force_install_dir, app_id) = if ctx.use_experimental {
            ("..\\Arma_Reforger\\experimental", APP_ID_EXPERIMENTAL)
        } else {
            ("..\\Arma_Reforger", APP_ID_STANDARD)
        };

        let steam_args_str = format!(
            "+force_install_dir {force_install_dir} +login anonymous anonymous {update_switch} {app_id} +quit"
        );
        // Split on whitespace is safe here: none of these tokens contain spaces.
        let steam_args: Vec<String> = steam_args_str.split_whitespace().map(String::from).collect();

        let mut cmd = Command::new(&ctx.steamcmd_exe);
        cmd.args(&steam_args);
        cmd.stdout(std::process::Stdio::piped());
        cmd.stderr(std::process::Stdio::piped());
        #[cfg(target_os = "windows")]
        {
            const CREATE_NO_WINDOW: u32 = 0x08000000;
            cmd.creation_flags(CREATE_NO_WINDOW);
        }

        let mut child = cmd.spawn().map_err(ServiceError::Io)?;
        let stdout = child.stdout.take();
        let stderr = child.stderr.take();

        // SteamCMD output is pure log noise — it never carries server telemetry — so it gets a
        // throwaway parser that nothing reads.
        let steamcmd_parser = Arc::new(Mutex::new(ServerStatusParser::new()));
        let this_out = Arc::clone(self);
        let out_parser = Arc::clone(&steamcmd_parser);
        let out_task = tokio::spawn(async move {
            if let Some(stdout) = stdout {
                this_out.stream_lines(stdout, false, out_parser).await;
            }
        });
        let this_err = Arc::clone(self);
        let err_parser = Arc::clone(&steamcmd_parser);
        let err_task = tokio::spawn(async move {
            if let Some(stderr) = stderr {
                this_err.stream_lines(stderr, false, err_parser).await;
            }
        });

        // Wait for SteamCMD, but stay interruptible: if the user hits Stop mid-download we kill
        // SteamCMD and abandon the run rather than launching the server minutes later.
        let status = tokio::select! {
            res = child.wait() => Some(res.map_err(ServiceError::Io)?),
            _ = wait_for_cancel(Arc::clone(&cancel)) => {
                let _ = child.kill().await;
                None
            }
        };
        let _ = tokio::join!(out_task, err_task);

        let Some(status) = status else {
            self.emit(log_line("Update cancelled, server will not start."));
            return Ok(());
        };

        if !status.success() {
            tracing::warn!("SteamCMD exited with status {:?}", status);
        }

        if cancel.load(Ordering::SeqCst) {
            self.emit(log_line("Server start cancelled."));
            return Ok(());
        }

        self.emit(ProcessEvent::GuiUpdate(GuiModel {
            button_icon: ServerButtonIcon::Stop,
            enable_server_fields: false,
            server_running_label_text: SERVER_CURRENTLY_RUNNING_STR.to_string(),
            start_server_btn_enabled: true,
        }));

        let arma_subdir = if ctx.use_experimental {
            "arma_reforger\\experimental"
        } else {
            "arma_reforger"
        };
        let server_working_dir = ctx.install_dir.join(arma_subdir);
        let server_exe = server_working_dir.join("ArmaReforgerServer.exe");

        let launch_args_str = Self::build_launch_arguments(&ctx);
        let launch_args: Vec<String> = shell_split(&launch_args_str);

        self.emit(log_line("Download / update complete. Starting the dedicated server..."));

        let mut server_cmd = match &ctx.server_target {
            ServerTarget::Windows => {
                let mut c = Command::new(&server_exe);
                c.current_dir(&server_working_dir);
                c.args(&launch_args);
                c
            }
            ServerTarget::Wsl { distro } => {
                // Linux binary is conventionally named without the .exe suffix.
                let linux_binary = "ArmaReforgerServer";
                wsl_command(distro.as_deref(), &server_working_dir, linux_binary, &launch_args)
            }
        };
        server_cmd.stdout(std::process::Stdio::piped());
        server_cmd.stderr(std::process::Stdio::piped());
        #[cfg(target_os = "windows")]
        {
            const CREATE_NO_WINDOW: u32 = 0x08000000;
            server_cmd.creation_flags(CREATE_NO_WINDOW);
        }

        let mut server_child = server_cmd.spawn().map_err(ServiceError::Io)?;
        let server_stdout = server_child.stdout.take();
        let server_stderr = server_child.stderr.take();

        {
            let mut inner = self.inner.lock().await;
            inner.server_pid = server_child.id();
        }

        // One parser shared by both server streams: telemetry accumulates across lines, so
        // giving each stream its own would mean a line arriving on stderr emitted a status with
        // stale/default values for everything parsed from stdout.
        let server_parser = Arc::new(Mutex::new(ServerStatusParser::new()));
        let this_out = Arc::clone(self);
        let out_parser = Arc::clone(&server_parser);
        tokio::spawn(async move {
            if let Some(stdout) = server_stdout {
                this_out.stream_lines(stdout, true, out_parser).await;
            }
        });
        let this_err = Arc::clone(self);
        let err_parser = Arc::clone(&server_parser);
        tokio::spawn(async move {
            if let Some(stderr) = server_stderr {
                this_err.stream_lines(stderr, true, err_parser).await;
            }
        });

        // Monitor task owns the child: it either reaps a natural exit (so the UI doesn't stay
        // stuck on "running" after a crash) or kills the process when the run is cancelled.
        // Owning the child here — rather than parking it in `Inner` — is what makes both paths
        // possible without fighting over a `&mut` borrow.
        let this = Arc::clone(self);
        tokio::spawn(async move {
            let exited_on_its_own = tokio::select! {
                res = server_child.wait() => {
                    match res {
                        Ok(status) => {
                            this.emit(log_line(format!("Server process exited ({status}).")));
                        }
                        Err(e) => {
                            this.emit(log_line(format!("Lost track of the server process: {e}")));
                        }
                    }
                    true
                }
                _ = wait_for_cancel(Arc::clone(&cancel)) => {
                    let _ = server_child.kill().await;
                    let _ = server_child.wait().await;
                    false
                }
            };

            {
                let mut inner = this.inner.lock().await;
                inner.server_pid = None;
            }

            if exited_on_its_own {
                // `stop_server` already announces a user-initiated stop; only announce here when
                // the process went away on its own.
                this.mark_stopped().await;
            }
        });

        Ok(())
    }

    /// Reads lines from a child process pipe, timestamps + emits each one, and (when
    /// `is_server_output` is true) runs them through fatal-error / crash detection and the
    /// [`ServerStatusParser`].
    async fn stream_lines<R>(
        self: &Arc<Self>,
        reader: R,
        is_server_output: bool,
        status_parser: Arc<Mutex<ServerStatusParser>>,
    ) where
        R: tokio::io::AsyncRead + Unpin,
    {
        let mut lines = BufReader::new(reader).lines();
        loop {
            match lines.next_line().await {
                Ok(Some(line)) => {
                    self.emit(log_line(&line));

                    if !is_server_output {
                        continue;
                    }

                    {
                        let mut parser = status_parser.lock().await;
                        if parser.parse_line(&line) {
                            self.emit(ProcessEvent::Status(parser.status().clone()));
                        }
                    }

                    if line.contains("Error while initializing game")
                        || line.contains("Unable to initialize the game")
                    {
                        tracing::info!("ProcessService - System stopped server due to an error.");
                        self.emit(log_line("System stopped server due to an error."));
                        // Signalling cancellation lets the monitor task kill the process; it
                        // then reports the stop, so no state is updated directly here.
                        self.signal_cancel().await;
                    }

                    if line.contains("Game destroyed") {
                        let (auto_restart, ctx) = {
                            let inner = self.inner.lock().await;
                            let ctx = inner.last_start_ctx.clone();
                            (ctx.as_ref().map(|c| c.auto_restart_on_crash).unwrap_or(false), ctx)
                        };
                        if auto_restart {
                            if let Some(ctx) = ctx {
                                tracing::info!(
                                    "ProcessService - Game destroyed detected. Attempting to restart server..."
                                );
                                self.emit(log_line(
                                    "Game destroyed detected. System attempting to restart server...",
                                ));
                                let delay_ms = ctx.auto_restart_delay_ms;
                                let this = Arc::clone(self);
                                tokio::spawn(async move {
                                    // Stop (1st toggle), matching the C# original's two-toggle dance.
                                    let _ = this.stop_server(true).await;
                                    tokio::time::sleep(std::time::Duration::from_millis(delay_ms)).await;
                                    tracing::info!("ProcessService - Restarting server now...");
                                    let _ = this.start_server(ctx, true).await;
                                });
                            }
                        }
                    }
                }
                Ok(None) => break,
                Err(e) => {
                    tracing::warn!("Error reading process output: {e}");
                    break;
                }
            }
        }
    }

    /// Raises the current run's cancellation flag, which tells the in-flight run to kill
    /// SteamCMD and/or the server process and to skip any pending launch.
    async fn signal_cancel(&self) {
        let cancel = {
            let mut inner = self.inner.lock().await;
            inner.run_cancel.take()
        };
        if let Some(cancel) = cancel {
            cancel.store(true, Ordering::SeqCst);
        }
    }

    /// Stops the running server (kills the process). Mirrors the C# `StartStopServer`'s stop
    /// branch. Callers are expected to have already removed UPnP mappings and reset
    /// `server.json` before calling this, matching the new separation of concerns.
    pub async fn stop_server(&self, triggered_by_auto_restart: bool) -> Result<(), ServiceError> {
        tracing::info!(
            "ProcessService - {} server.",
            if triggered_by_auto_restart { "Automatically stopped" } else { "User stopped" }
        );
        self.emit(log_line(if triggered_by_auto_restart {
            "Automatically stopped server."
        } else {
            "User stopped server."
        }));

        self.signal_cancel().await;
        self.mark_stopped().await;

        Ok(())
    }

    /// Force-kills the server (or an in-progress SteamCMD update) and clears running state.
    /// Used by the Kill button.
    pub async fn kill_server(&self) {
        self.signal_cancel().await;
        self.mark_stopped().await;
    }

    /// Synchronously kills the server process tree by PID, without needing the async runtime to
    /// schedule anything. This is the shutdown path: at app exit the monitor task that would
    /// normally do the killing may never run again, so relying on the cancellation flag there
    /// would leave an orphaned dedicated server behind.
    pub fn kill_server_blocking(&self) {
        let pid = match self.inner.try_lock() {
            Ok(inner) => inner.server_pid,
            // If the lock is contended at shutdown, fall back to no-op rather than blocking the
            // UI thread; the OS will reap the child with the parent in the common case.
            Err(_) => None,
        };

        let Some(pid) = pid else { return };

        // `taskkill /T` takes the whole process tree, which also covers the `wsl.exe` wrapper
        // used by the Linux server target.
        let _ = std::process::Command::new("taskkill")
            .args(["/PID", &pid.to_string(), "/T", "/F"])
            .stdout(std::process::Stdio::null())
            .stderr(std::process::Stdio::null())
            .status();
    }

    /// Starts the daily scheduled auto-restart loop: restarts the server every day at
    /// `daily_time`. `restart` is called each time the schedule fires (the caller supplies a
    /// closure that rebuilds a fresh `StartServerContext`, since config may change between
    /// firings). Mirrors `ProcessManager.ConfigureAutomaticRestartTask` + `PeriodicAsync`.
    pub async fn configure_automatic_restart_task<F, Fut>(
        self: &Arc<Self>,
        daily_time: NaiveTime,
        rebuild_ctx: F,
    ) where
        F: Fn() -> Fut + Send + Sync + 'static,
        Fut: std::future::Future<Output = StartServerContext> + Send,
    {
        // Immediately (re)start once, matching the C# original calling
        // `StartStopServerUsingTimer()` synchronously before entering the periodic loop.
        let ctx = rebuild_ctx().await;
        let _ = self.start_stop_server(ctx, true).await;

        let cancel_flag = Arc::new(AtomicBool::new(false));
        {
            let mut inner = self.inner.lock().await;
            inner.is_server_using_timer = true;
            inner.restart_timer_cancel = Some(Arc::clone(&cancel_flag));
        }

        let this = Arc::clone(self);
        tokio::spawn(async move {
            loop {
                if cancel_flag.load(Ordering::SeqCst) {
                    break;
                }

                let now = Local::now();
                let today_scheduled = now
                    .date_naive()
                    .and_hms_opt(daily_time.hour(), daily_time.minute(), daily_time.second())
                    .unwrap();
                let now_naive = now.naive_local();
                let next_run = if today_scheduled > now_naive {
                    today_scheduled
                } else {
                    today_scheduled + chrono::Duration::days(1)
                };
                let delay = (next_run - now_naive)
                    .to_std()
                    .unwrap_or(std::time::Duration::from_secs(1));

                tokio::select! {
                    _ = tokio::time::sleep(delay) => {
                        if cancel_flag.load(Ordering::SeqCst) {
                            break;
                        }
                        let ctx = rebuild_ctx().await;
                        let _ = this.start_stop_server(ctx, true).await;
                    }
                    _ = wait_for_cancel(Arc::clone(&cancel_flag)) => {
                        break;
                    }
                }
            }
        });
    }

    /// Cancels the daily auto-restart loop and stops the server. Mirrors
    /// `ProcessManager.CancelAutomaticRestartTask`.
    pub async fn cancel_automatic_restart_task(&self) -> Result<(), ServiceError> {
        let cancel = {
            let mut inner = self.inner.lock().await;
            inner.is_server_using_timer = false;
            inner.restart_timer_cancel.take()
        };
        if let Some(cancel) = cancel {
            cancel.store(true, Ordering::SeqCst);
        }
        self.stop_server(false).await
    }
}

/// Polls a cancellation flag; used to make the daily-restart sleep interruptible.
async fn wait_for_cancel(flag: Arc<AtomicBool>) {
    loop {
        if flag.load(Ordering::SeqCst) {
            return;
        }
        tokio::time::sleep(std::time::Duration::from_millis(250)).await;
    }
}

/// Minimal shell-style argument splitter: splits on whitespace but keeps double-quoted
/// segments (including their spaces) together, stripping the quotes. Good enough for the
/// launch-argument strings this app builds itself (it doesn't need to handle arbitrary shell
/// syntax, escaping, or nested quotes).
fn shell_split(input: &str) -> Vec<String> {
    let mut args = Vec::new();
    let mut current = String::new();
    let mut in_quotes = false;
    // Tracks an explicitly-quoted token so a deliberate empty argument (e.g. an empty
    // `-adminPassword ""`) survives instead of being dropped as whitespace.
    let mut quoted_token = false;

    for c in input.chars() {
        match c {
            '"' => {
                in_quotes = !in_quotes;
                quoted_token = true;
            }
            c if c.is_whitespace() && !in_quotes => {
                if !current.is_empty() || quoted_token {
                    args.push(std::mem::take(&mut current));
                    quoted_token = false;
                }
            }
            c => current.push(c),
        }
    }
    if !current.is_empty() || quoted_token {
        args.push(current);
    }
    args
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::models::LaunchArgument;

    #[test]
    fn shell_split_handles_quoted_segments() {
        let parts = shell_split(r#"-bindPort 2001 -loadSessionSave "my save"#.to_string().as_str());
        // Unterminated quote: treat rest as part of the quoted run (best-effort).
        assert_eq!(parts[0], "-bindPort");
        assert_eq!(parts[1], "2001");
    }

    #[test]
    fn shell_split_basic() {
        let parts = shell_split("-a 1 -b 2");
        assert_eq!(parts, vec!["-a", "1", "-b", "2"]);
    }

    #[test]
    fn shell_split_quoted() {
        let parts = shell_split(r#"-loadSessionSave "my save""#);
        assert_eq!(parts, vec!["-loadSessionSave", "my save"]);
    }

    #[test]
    fn shell_split_preserves_deliberate_empty_quoted_argument() {
        // An empty admin password must still be passed as an empty argument rather than
        // vanishing, which would shift every following argument by one position.
        let parts = shell_split(r#"-adminPassword "" -addons 123"#);
        assert_eq!(parts, vec!["-adminPassword", "", "-addons", "123"]);
    }

    #[test]
    fn shell_split_keeps_quoted_paths_with_spaces_intact() {
        let parts = shell_split(r#"-config "C:\Arma Server\server.json" -logStats 5000"#);
        assert_eq!(
            parts,
            vec!["-config", r"C:\Arma Server\server.json", "-logStats", "5000"]
        );
    }

    fn base_ctx() -> StartServerContext {
        StartServerContext {
            steamcmd_exe: PathBuf::from("steamcmd.exe"),
            install_dir: PathBuf::from("C:/servers/arma"),
            use_experimental: false,
            keep_server_updated: true,
            no_backend: false,
            no_backend_args: String::new(),
            config_bind_port: 2001,
            using_save: false,
            save_name: None,
            launch_arguments: LaunchArguments::default(),
            auto_restart_on_crash: false,
            auto_restart_delay_ms: 2000,
            server_target: ServerTarget::Windows,
        }
    }

    #[test]
    fn build_launch_arguments_appends_config_when_not_no_backend() {
        let mut ctx = base_ctx();
        ctx.launch_arguments.config = Some(LaunchArgument::new("config", "server.json"));
        ctx.launch_arguments.max_fps = Some(LaunchArgument::new("maxFPS", "60"));
        let args = ProcessService::build_launch_arguments(&ctx);
        assert!(args.contains("-maxFPS 60"));
        assert!(args.ends_with("-config server.json"));
    }

    #[test]
    fn build_launch_arguments_uses_no_backend_args_and_bind_port_override() {
        let mut ctx = base_ctx();
        ctx.no_backend = true;
        ctx.no_backend_args = "-adminPassword \"x\" -addons 123".to_string();
        ctx.config_bind_port = 2001;
        let args = ProcessService::build_launch_arguments(&ctx);
        assert!(args.contains("-adminPassword"));
        assert!(args.contains("-bindPort 2001"));
    }

    #[test]
    fn build_launch_arguments_load_session_save_switch_when_latest() {
        let mut ctx = base_ctx();
        ctx.using_save = true;
        ctx.save_name = None;
        ctx.launch_arguments.config = Some(LaunchArgument::new("config", "server.json"));
        let args = ProcessService::build_launch_arguments(&ctx);
        assert!(args.contains("-loadSessionSave"));
        assert!(!args.contains("-loadSessionSave \""));
    }

    /// Populates the mandatory arguments exactly as `commands::process_commands` does, so the
    /// two stay in sync. The original port shipped without these entirely: mods silently never
    /// loaded (`-addonsDir`), saves went nowhere (`-profile`), and the Status screen's charts
    /// stayed permanently empty because the server was never told to emit stats (`-logStats`).
    fn ctx_with_mandatory_args(install_dir: &str) -> StartServerContext {
        let mut ctx = base_ctx();
        let dir = PathBuf::from(install_dir);
        let quoted = |p: PathBuf| format!("\"{}\"", p.display());
        ctx.install_dir = dir.clone();
        ctx.launch_arguments.config =
            Some(LaunchArgument::new("config", quoted(dir.join("server.json"))));
        ctx.launch_arguments.profile =
            Some(LaunchArgument::new("profile", quoted(dir.join("saves"))));
        ctx.launch_arguments.addons_dir =
            Some(LaunchArgument::new("addonsDir", quoted(dir.join("addons"))));
        ctx.launch_arguments.log_stats = Some(LaunchArgument::new("logStats", "5000"));
        ctx.launch_arguments.log_level = Some(LaunchArgument::new("logLevel", "normal"));
        ctx
    }

    #[test]
    fn mandatory_arguments_survive_as_a_correct_argv_with_spaces_in_paths() {
        let ctx = ctx_with_mandatory_args(r"C:\Arma Server");
        let argv = shell_split(&ProcessService::build_launch_arguments(&ctx));

        let pos = |flag: &str| argv.iter().position(|a| a == flag).expect(flag);

        // Each path stays a single argv entry despite the space in "Arma Server", and points at
        // the install directory — not at the server's own working directory.
        assert_eq!(argv[pos("-config") + 1], r"C:\Arma Server\server.json");
        assert_eq!(argv[pos("-profile") + 1], r"C:\Arma Server\saves");
        assert_eq!(argv[pos("-addonsDir") + 1], r"C:\Arma Server\addons");
        assert_eq!(argv[pos("-logStats") + 1], "5000");
        assert_eq!(argv[pos("-logLevel") + 1], "normal");
    }

    #[test]
    fn no_backend_keeps_mandatory_args_but_drops_config() {
        let mut ctx = ctx_with_mandatory_args(r"C:\Arma Server");
        ctx.no_backend = true;
        ctx.no_backend_args = "-adminPassword \"pw\" -addons ABC".to_string();
        let argv = shell_split(&ProcessService::build_launch_arguments(&ctx));

        // `-config` is meaningless without the backend, but the rest still apply.
        assert!(!argv.iter().any(|a| a == "-config"));
        assert!(argv.iter().any(|a| a == "-addonsDir"));
        assert!(argv.iter().any(|a| a == "-logStats"));
        assert!(argv.iter().any(|a| a == "-adminPassword"));
    }

    #[test]
    fn build_launch_arguments_load_session_save_named() {
        let mut ctx = base_ctx();
        ctx.using_save = true;
        ctx.save_name = Some("MySave".to_string());
        ctx.launch_arguments.config = Some(LaunchArgument::new("config", "server.json"));
        let args = ProcessService::build_launch_arguments(&ctx);
        assert!(args.contains("-loadSessionSave \"MySave\""));
    }
}
