//! Tauri commands for saving, applying, and managing named mod loadouts.

use crate::models::{Mod, ModTemplate};
use crate::state::AppState;

use super::mods_commands::ModLists;

#[tauri::command]
pub async fn list_mod_templates(
    state: tauri::State<'_, AppState>,
) -> Result<Vec<ModTemplate>, String> {
    Ok(state.templates.lock().await.templates().to_vec())
}

/// Saves the currently enabled mods (in their current load order) as a named template,
/// replacing any existing template with the same name.
#[tauri::command]
pub async fn save_mod_template(
    state: tauri::State<'_, AppState>,
    name: String,
    description: String,
) -> Result<Vec<ModTemplate>, String> {
    let mods = state.config.lock().await.enabled_mods().to_vec();
    if mods.is_empty() {
        return Err("There are no enabled mods to save as a template.".to_string());
    }

    let mut templates = state.templates.lock().await;
    templates
        .save_template(&name, &description, mods)
        .map_err(|e| e.to_string())?;
    Ok(templates.templates().to_vec())
}

#[tauri::command]
pub async fn delete_mod_template(
    state: tauri::State<'_, AppState>,
    name: String,
) -> Result<Vec<ModTemplate>, String> {
    let mut templates = state.templates.lock().await;
    templates.delete_template(&name).map_err(|e| e.to_string())?;
    Ok(templates.templates().to_vec())
}

/// Applies a template to the current server.
///
/// `replace` chooses between the two things "apply a template" can reasonably mean: replacing the
/// enabled list outright (the template *is* the loadout), or adding its mods on top of what's
/// already enabled (the template is a component of the loadout). Either way the mods land in the
/// known-mods database too, so they behave exactly like mods added by hand.
#[tauri::command]
pub async fn apply_mod_template(
    state: tauri::State<'_, AppState>,
    name: String,
    replace: bool,
) -> Result<ModLists, String> {
    let template_mods: Vec<Mod> = {
        let templates = state.templates.lock().await;
        templates
            .get(&name)
            .ok_or_else(|| format!("No mod template named '{name}'."))?
            .mods
            .clone()
    };

    {
        let mut config = state.config.lock().await;
        if replace {
            // `import_mods_list` already does exactly this: everything currently enabled goes
            // back to available, then the given list becomes the enabled list in order.
            config.import_mods_list(template_mods);
        } else {
            for m in template_mods {
                config.move_mod_to_enabled(m);
            }
        }
    }

    super::mods_commands::persist_mods_database(&state).await?;
    Ok(super::mods_commands::mod_lists(&state).await)
}
