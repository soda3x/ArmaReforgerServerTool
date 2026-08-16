//! Named, reusable mod loadouts.
//!
//! The existing export/import pair already round-trips a mod list through a file the user picks
//! each time; a template is the same list given a name and kept inside the app, so switching a
//! server between loadouts is a two-click operation instead of a file dialog. Templates are also
//! what an exported file *contains*, which is what makes them shareable between people.

use serde::{Deserialize, Serialize};

use super::mod_entry::Mod;

#[derive(Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ModTemplate {
    pub name: String,
    #[serde(default)]
    pub description: String,
    /// In load order — a template preserves the order its mods were enabled in, since load order
    /// is significant to the game.
    #[serde(default)]
    pub mods: Vec<Mod>,
    /// ISO-8601 timestamp, informational only (shown in the UI so a user can tell two similar
    /// templates apart).
    #[serde(default)]
    pub updated_at: String,
}

impl ModTemplate {
    pub fn new(name: impl Into<String>, description: impl Into<String>, mods: Vec<Mod>) -> Self {
        Self {
            name: name.into(),
            description: description.into(),
            mods,
            updated_at: chrono::Local::now().to_rfc3339(),
        }
    }
}

/// The on-disk shape of the template store. A struct rather than a bare `Vec` so more fields can
/// be added later without breaking existing files.
#[derive(Debug, Clone, Default, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ModTemplateStore {
    #[serde(default)]
    pub templates: Vec<ModTemplate>,
}
