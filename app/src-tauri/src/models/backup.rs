use chrono::{DateTime, Local};
use serde::{Deserialize, Serialize};

/// Metadata for one saves/config backup archive, as shown in the Management tab's backup list.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct BackupInfo {
    /// The backup zip's filename (without directory), used to identify it in later
    /// restore/delete calls.
    pub id: String,
    pub created_at: DateTime<Local>,
    pub label: String,
    pub size_bytes: u64,
}
