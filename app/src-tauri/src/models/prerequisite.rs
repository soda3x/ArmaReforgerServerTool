//! Status of a runtime prerequisite the dedicated server needs in order to start at all —
//! reported to the frontend by `commands::prereq_commands`.

use serde::Serialize;

#[derive(Debug, Clone, PartialEq, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct Prerequisite {
    /// Stable identifier the frontend keys actions off (e.g. `"vcredist"`).
    pub id: String,
    pub name: String,
    pub satisfied: bool,
    /// The specific files/libraries that were looked for and not found. Empty when satisfied.
    pub missing: Vec<String>,
    /// Human-readable explanation of what this is and, when unsatisfied, how to resolve it.
    pub detail: String,
    /// Whether Longbow can install this itself (see `install_prerequisite`). When false the
    /// `detail` text carries manual instructions instead.
    pub auto_installable: bool,
}

impl Prerequisite {
    pub fn satisfied(id: &str, name: &str, detail: &str) -> Self {
        Self {
            id: id.to_string(),
            name: name.to_string(),
            satisfied: true,
            missing: Vec::new(),
            detail: detail.to_string(),
            auto_installable: false,
        }
    }

    pub fn missing(id: &str, name: &str, missing: Vec<String>, detail: &str, auto_installable: bool) -> Self {
        Self {
            id: id.to_string(),
            name: name.to_string(),
            satisfied: false,
            missing,
            detail: detail.to_string(),
            auto_installable,
        }
    }
}
