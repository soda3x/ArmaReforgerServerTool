use chrono::{DateTime, Local};
use serde::{Deserialize, Serialize};

/// A ban entry as reported by the RCON `#ban list` command.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct BanEntry {
    pub identity_id: String,
    pub reason: String,
    /// `None` means a permanent ban (duration `0`).
    pub expires_at: Option<DateTime<Local>>,
}
