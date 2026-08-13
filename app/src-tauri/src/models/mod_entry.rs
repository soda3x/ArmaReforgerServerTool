use serde::{Deserialize, Serialize};

/// String literal used to mean "use whatever the latest version of the mod is".
pub const LATEST_MOD_VER_STR: &str = "latest";

fn default_version() -> String {
    LATEST_MOD_VER_STR.to_string()
}

fn is_latest_version(version: &str) -> bool {
    version == LATEST_MOD_VER_STR
}

/// Represents a single Mod entry within a server configuration's `mods` list.
#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Mod {
    #[serde(default)]
    pub mod_id: String,
    #[serde(default)]
    pub name: String,
    /// Defaults to "latest" when absent. Omitted entirely from JSON output when it
    /// equals "latest" (mirrors the C# `ModConverter` behavior).
    #[serde(default = "default_version", skip_serializing_if = "is_latest_version")]
    pub version: String,
    #[serde(default)]
    pub required: bool,
}

impl Mod {
    pub fn new(
        mod_id: impl Into<String>,
        name: impl Into<String>,
        version: impl Into<String>,
        required: bool,
    ) -> Self {
        Self {
            mod_id: mod_id.into(),
            name: name.into(),
            version: version.into(),
            required,
        }
    }

    pub fn new_latest(mod_id: impl Into<String>, name: impl Into<String>, required: bool) -> Self {
        Self {
            mod_id: mod_id.into(),
            name: name.into(),
            version: LATEST_MOD_VER_STR.to_string(),
            required,
        }
    }
}

impl Default for Mod {
    fn default() -> Self {
        Self {
            mod_id: String::new(),
            name: String::new(),
            version: default_version(),
            required: false,
        }
    }
}

// The C# `Mod.Equals` compares `name` and `modId` unconditionally, and additionally
// compares `version` only when the instance's `version` field is non-null. Since Rust's
// `version` is never null (it's a plain `String`, defaulted to "latest"), the closest
// faithful-enough behavior is to compare on `name` + `mod_id` only, which is what every
// call site in the C# code actually relies on in practice (de-duplicating mods by identity).
impl PartialEq for Mod {
    fn eq(&self, other: &Self) -> bool {
        self.name == other.name && self.mod_id == other.mod_id
    }
}
impl Eq for Mod {}
