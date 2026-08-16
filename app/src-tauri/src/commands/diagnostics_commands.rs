//! Exposes the troubleshooting reference to the frontend.
//!
//! These are static catalogs with no application state behind them, so the commands take no
//! `AppState` — the same data the log stream uses to annotate errors, offered up front so a user
//! can read it before something breaks rather than only after.

use crate::services::diagnostics::{self, CommonIssue, Diagnostic};

#[tauri::command]
pub async fn list_diagnostics() -> Result<Vec<Diagnostic>, String> {
    Ok(diagnostics::catalog().to_vec())
}

#[tauri::command]
pub async fn list_common_issues() -> Result<Vec<CommonIssue>, String> {
    Ok(diagnostics::common_issues().to_vec())
}
