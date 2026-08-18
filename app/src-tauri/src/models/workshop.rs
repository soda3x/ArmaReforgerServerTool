//! Clean, stable shapes for Arma Workshop catalog data, returned to the frontend by
//! `commands::workshop_commands`. The upstream JSON's raw shape (and its quirks) stays inside
//! `services::workshop_service` — these types are what the rest of the app actually works with.

use serde::Serialize;

#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct WorkshopAssetSummary {
    pub id: String,
    pub name: String,
    pub summary: String,
    pub average_rating: f64,
    pub rating_count: u64,
    pub subscriber_count: u64,
    pub current_version_number: String,
    pub current_version_size: u64,
    /// One representative thumbnail URL, picked from the asset's preview images. `None` if the
    /// asset has no preview images at all (rare, but not impossible for a new/minimal upload).
    pub thumbnail_url: Option<String>,
    pub author_username: String,
    pub tags: Vec<String>,
}

#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct WorkshopAssetDetail {
    pub id: String,
    pub name: String,
    pub summary: String,
    pub description: String,
    pub license: Option<String>,
    pub average_rating: f64,
    pub rating_count: u64,
    pub subscriber_count: u64,
    pub current_version_number: String,
    pub current_version_size: u64,
    /// Every preview image URL (largest/cover size), for a detail-view gallery.
    pub preview_urls: Vec<String>,
    pub author_username: String,
    pub tags: Vec<String>,
    /// The full transitive set of other workshop mods this one requires (flattened and
    /// deduplicated from upstream's nested dependency tree), so the UI can offer to add them
    /// alongside the mod the user actually picked.
    pub dependencies: Vec<WorkshopDependency>,
    /// Scenarios this mod bundles (e.g. the Conflict/AAS layouts a terrain mod ships with).
    /// Empty for the great majority of mods, which don't bundle any.
    pub scenarios: Vec<WorkshopScenario>,
}

#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct WorkshopScenario {
    pub name: String,
    /// The `{GUID}Missions/Name.conf` value the game's own `game.scenarioId` field expects —
    /// upstream calls this `gameId`, but "path" is what it means everywhere else in this app.
    pub path: String,
    /// Upstream's raw value is a localization key (e.g. `#AR-Scenario_GameMode_Campaign`), not a
    /// display string — the game resolves it via its own string table, which this app has no
    /// access to. Shown as-is rather than guessed at.
    pub game_mode: String,
    pub player_count: u32,
}

#[derive(Debug, Clone, PartialEq, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct WorkshopDependency {
    pub id: String,
    pub name: String,
    pub total_file_size: u64,
}

#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct WorkshopSearchResult {
    pub count: u64,
    pub rows: Vec<WorkshopAssetSummary>,
}
