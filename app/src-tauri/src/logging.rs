//! Application logging. Replaces the C# original's Serilog setup (console sink + rolling
//! daily file), which `Program.cs` configured before anything else ran.

use std::path::Path;

use tracing_subscriber::layer::SubscriberExt;
use tracing_subscriber::util::SubscriberInitExt;
use tracing_subscriber::EnvFilter;

/// Initializes tracing with a console layer plus a daily-rolling file in `{log_dir}/`.
///
/// Returns the file appender guard, which must be kept alive for the process's lifetime —
/// dropping it stops the background writer and silently loses buffered log lines.
pub fn init(log_dir: &Path, min_level: &str) -> Option<tracing_appender::non_blocking::WorkerGuard> {
    let filter = EnvFilter::try_from_default_env()
        .unwrap_or_else(|_| EnvFilter::new(normalise_level(min_level)));

    if std::fs::create_dir_all(log_dir).is_err() {
        // Fall back to console-only rather than losing logging entirely.
        tracing_subscriber::registry()
            .with(filter)
            .with(tracing_subscriber::fmt::layer())
            .init();
        return None;
    }

    let file_appender = tracing_appender::rolling::daily(log_dir, "longbow.log");
    let (file_writer, guard) = tracing_appender::non_blocking(file_appender);

    tracing_subscriber::registry()
        .with(filter)
        .with(tracing_subscriber::fmt::layer())
        .with(
            tracing_subscriber::fmt::layer()
                .with_writer(file_writer)
                .with_ansi(false),
        )
        .init();

    Some(guard)
}

/// Maps the C# `ToolProperties.minimumLogLevel` values onto `EnvFilter` directives.
fn normalise_level(level: &str) -> String {
    match level.to_ascii_lowercase().as_str() {
        "verbose" => "trace",
        "debug" => "debug",
        "information" | "info" => "info",
        "warning" | "warn" => "warn",
        "error" => "error",
        "fatal" => "error",
        _ => "debug",
    }
    .to_string()
}
