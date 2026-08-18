use serde::{Deserialize, Serialize};

/// A player as reported by the RCON `#players` command.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq)]
#[serde(rename_all = "camelCase")]
pub struct RconPlayer {
    pub id: String,
    pub name: String,
}
