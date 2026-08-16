//! Storage for named mod loadouts ("templates"), persisted as JSON alongside the app's other
//! config files.
//!
//! Follows the same shape as `SavedStateService`: constructed explicitly with its path, owned by
//! `AppState` behind a `Mutex`, no singleton. Unlike `SavedStateService` a corrupt file is *not*
//! fatal here — templates are a convenience layer, and refusing to start the app over an
//! unreadable convenience file would be a worse outcome than starting with none.

use std::path::PathBuf;

use crate::models::{Mod, ModTemplate, ModTemplateStore};

use super::error::ServiceError;

pub struct TemplateService {
    path: PathBuf,
    store: ModTemplateStore,
}

impl TemplateService {
    pub fn load(path: impl Into<PathBuf>) -> Self {
        let path = path.into();
        let store = std::fs::read_to_string(&path)
            .ok()
            .and_then(|contents| match serde_json::from_str::<ModTemplateStore>(&contents) {
                Ok(store) => Some(store),
                Err(e) => {
                    tracing::warn!(
                        "Ignoring unreadable mod template file {}: {e}",
                        path.display()
                    );
                    None
                }
            })
            .unwrap_or_default();

        Self { path, store }
    }

    pub fn templates(&self) -> &[ModTemplate] {
        &self.store.templates
    }

    pub fn get(&self, name: &str) -> Option<&ModTemplate> {
        self.store
            .templates
            .iter()
            .find(|t| t.name.eq_ignore_ascii_case(name))
    }

    /// Creates or replaces the template called `name`. Matching is case-insensitive so "RHS" and
    /// "rhs" don't become two entries that look identical in the list.
    pub fn save_template(
        &mut self,
        name: &str,
        description: &str,
        mods: Vec<Mod>,
    ) -> Result<(), ServiceError> {
        let name = name.trim();
        if name.is_empty() {
            return Err(ServiceError::Other("A template name is required.".to_string()));
        }

        let template = ModTemplate::new(name, description.trim(), mods);
        match self
            .store
            .templates
            .iter_mut()
            .find(|t| t.name.eq_ignore_ascii_case(name))
        {
            Some(existing) => *existing = template,
            None => self.store.templates.push(template),
        }
        self.store.templates.sort_by_key(|t| t.name.to_lowercase());
        self.persist()
    }

    /// Removes the template called `name`. Removing something that isn't there is not an error —
    /// the caller's intent (that it be gone) is satisfied either way.
    pub fn delete_template(&mut self, name: &str) -> Result<(), ServiceError> {
        self.store
            .templates
            .retain(|t| !t.name.eq_ignore_ascii_case(name));
        self.persist()
    }

    fn persist(&self) -> Result<(), ServiceError> {
        if let Some(parent) = self.path.parent() {
            std::fs::create_dir_all(parent)?;
        }
        let json = serde_json::to_string_pretty(&self.store)?;
        std::fs::write(&self.path, json)?;
        Ok(())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn mods(names: &[(&str, &str)]) -> Vec<Mod> {
        names
            .iter()
            .map(|(id, name)| Mod::new_latest(*id, *name, false))
            .collect()
    }

    #[test]
    fn missing_file_starts_with_no_templates() {
        let dir = tempfile::tempdir().unwrap();
        let service = TemplateService::load(dir.path().join("mod_templates.json"));
        assert!(service.templates().is_empty());
    }

    #[test]
    fn saves_and_reloads_a_template_preserving_load_order() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("mod_templates.json");

        let ordered = mods(&[("C", "Third"), ("A", "First"), ("B", "Second")]);
        let mut service = TemplateService::load(&path);
        service.save_template("Ops Night", "our usual set", ordered.clone()).unwrap();

        // Reload from disk to prove it round-trips rather than just living in memory.
        let reloaded = TemplateService::load(&path);
        let template = reloaded.get("Ops Night").unwrap();
        assert_eq!(template.description, "our usual set");
        assert_eq!(template.mods, ordered, "load order must survive the round trip");
        assert!(!template.updated_at.is_empty());
    }

    #[test]
    fn saving_the_same_name_replaces_rather_than_duplicates() {
        let dir = tempfile::tempdir().unwrap();
        let mut service = TemplateService::load(dir.path().join("t.json"));

        service.save_template("Vanilla+", "", mods(&[("A", "One")])).unwrap();
        // Different case, same template as far as the user is concerned.
        service.save_template("vanilla+", "", mods(&[("B", "Two"), ("C", "Three")])).unwrap();

        assert_eq!(service.templates().len(), 1);
        assert_eq!(service.get("VANILLA+").unwrap().mods.len(), 2);
    }

    #[test]
    fn rejects_a_blank_template_name() {
        let dir = tempfile::tempdir().unwrap();
        let mut service = TemplateService::load(dir.path().join("t.json"));
        assert!(service.save_template("   ", "", mods(&[("A", "One")])).is_err());
        assert!(service.templates().is_empty());
    }

    #[test]
    fn delete_removes_the_template_and_is_idempotent() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("t.json");
        let mut service = TemplateService::load(&path);
        service.save_template("Gone", "", mods(&[("A", "One")])).unwrap();

        service.delete_template("gone").unwrap();
        assert!(service.templates().is_empty());
        // Deleting again is a no-op, not an error.
        service.delete_template("gone").unwrap();

        assert!(TemplateService::load(&path).templates().is_empty());
    }

    #[test]
    fn a_corrupt_file_yields_no_templates_rather_than_failing_to_load() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("mod_templates.json");
        std::fs::write(&path, "{ this is not json").unwrap();

        let service = TemplateService::load(&path);
        assert!(service.templates().is_empty());
    }

    #[test]
    fn templates_are_listed_in_case_insensitive_name_order() {
        let dir = tempfile::tempdir().unwrap();
        let mut service = TemplateService::load(dir.path().join("t.json"));
        for name in ["zulu", "Alpha", "mike"] {
            service.save_template(name, "", mods(&[("A", "One")])).unwrap();
        }
        let names: Vec<&str> = service.templates().iter().map(|t| t.name.as_str()).collect();
        assert_eq!(names, vec!["Alpha", "mike", "zulu"]);
    }
}
