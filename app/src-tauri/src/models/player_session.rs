use chrono::{DateTime, Local};
use serde::{Deserialize, Serialize};

/// One player's connection session, derived by diffing successive RCON `#players` snapshots.
/// `left_at: None` means the session is still open (the player was present in the most recent
/// poll).
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct PlayerSession {
    pub player_id: String,
    pub player_name: String,
    pub joined_at: DateTime<Local>,
    pub left_at: Option<DateTime<Local>>,
}
