//! Tauri commands for the Workshop browser modal: search and mod detail lookups. Adding a
//! selected mod to the server doesn't need a dedicated command — the frontend just calls the
//! existing `add_mod` then `enable_mod` commands with the data these return.

use crate::models::{WorkshopAssetDetail, WorkshopSearchResult};
use crate::state::AppState;

/// A scenario contributed by a currently-enabled mod, with the owning mod's name attached so the
/// Scenario picker can group/label entries by which mod they came from.
#[derive(Debug, Clone, serde::Serialize)]
#[serde(rename_all = "camelCase")]
pub struct ModScenario {
    pub mod_name: String,
    pub name: String,
    pub path: String,
    pub game_mode: String,
    pub player_count: u32,
}

#[tauri::command]
pub async fn search_workshop_mods(
    state: tauri::State<'_, AppState>,
    query: Option<String>,
    page: u32,
    sort: Option<String>,
) -> Result<WorkshopSearchResult, String> {
    let workshop = state.workshop.lock().await;
    workshop
        .search(query.as_deref(), page, sort.as_deref())
        .await
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn get_workshop_mod_details(
    state: tauri::State<'_, AppState>,
    mod_id: String,
) -> Result<WorkshopAssetDetail, String> {
    let workshop = state.workshop.lock().await;
    workshop.get_details(&mod_id).await.map_err(|e| e.to_string())
}

/// Scenarios bundled by whichever mods are currently enabled — e.g. the Conflict/AAS layouts a
/// terrain mod ships with — so the Scenario picker can offer them without the user having to know
/// (or type) a scenario ID by hand.
///
/// A mod that isn't Workshop-hosted, or whose lookup fails for any reason, is skipped rather than
/// failing the whole request: most enabled mods (weapons, vehicles, faction packs) legitimately
/// have zero scenarios, and one bad ID shouldn't blank the picker for the mods that are fine.
#[tauri::command]
pub async fn get_scenarios_for_enabled_mods(
    state: tauri::State<'_, AppState>,
) -> Result<Vec<ModScenario>, String> {
    let mods: Vec<(String, String)> = {
        let config = state.config.lock().await;
        config
            .enabled_mods()
            .iter()
            .map(|m| (m.mod_id.clone(), m.name.clone()))
            .collect()
    };

    let workshop = state.workshop.lock().await;
    let mut seen_paths = std::collections::HashSet::new();
    let mut scenarios = Vec::new();
    for (mod_id, mod_name) in mods {
        let Ok(detail) = workshop.get_details(&mod_id).await else {
            continue;
        };
        for s in detail.scenarios {
            // The same scenario can legitimately show up under more than one enabled mod (a
            // terrain mod and a compatibility patch for it, say) — keep the first one seen
            // rather than listing it twice.
            if seen_paths.insert(s.path.clone()) {
                scenarios.push(ModScenario {
                    mod_name: mod_name.clone(),
                    name: s.name,
                    path: s.path,
                    game_mode: s.game_mode,
                    player_count: s.player_count,
                });
            }
        }
    }
    Ok(scenarios)
}
