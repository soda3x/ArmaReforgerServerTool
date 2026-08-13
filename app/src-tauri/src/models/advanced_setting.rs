use serde::{Deserialize, Serialize};

/// The value held by an [`AdvancedSetting`]. The C# original modeled this as `object`
/// (holding an int/double/string/bool) with the string literal `"switch"` as a sentinel
/// meaning "boolean CLI switch, no value". Here the "switch" case is represented instead
/// by `AdvancedSetting::value` being `None` (see below), which is cleaner Rust and equally
/// faithful to the original semantics.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(untagged)]
pub enum AdvancedSettingValue {
    Int(i64),
    Float(f64),
    Bool(bool),
    Str(String),
}

/// Represents the saved/loaded state of an advanced launch setting.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct AdvancedSetting {
    #[serde(default)]
    pub name: String,
    /// `None` means this is a switch-style setting (boolean flag, no value) — this
    /// mirrors the C# `IsSwitch()` / literal `"switch"` sentinel behavior, but modeled
    /// as an `Option` instead of a magic string.
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub value: Option<AdvancedSettingValue>,
    #[serde(default)]
    pub enabled: bool,
}

impl AdvancedSetting {
    pub fn new(name: impl Into<String>, value: AdvancedSettingValue, enabled: bool) -> Self {
        Self {
            name: name.into(),
            value: Some(value),
            enabled,
        }
    }

    /// Constructs a `switch` style (i.e. no value) advanced setting.
    pub fn new_switch(name: impl Into<String>, enabled: bool) -> Self {
        Self {
            name: name.into(),
            value: None,
            enabled,
        }
    }

    pub fn is_switch(&self) -> bool {
        self.value.is_none()
    }
}

impl Default for AdvancedSetting {
    fn default() -> Self {
        Self {
            name: String::new(),
            value: None,
            enabled: false,
        }
    }
}
