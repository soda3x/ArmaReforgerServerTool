use std::collections::HashMap;

use serde::{Deserialize, Serialize};

use super::advanced_setting::{AdvancedSetting, AdvancedSettingValue};

/// Number of available hardware threads, capped at 16. Mirrors
/// `Utilities.GetNumberAvailableThreads()` from the C# original.
///
/// NOTE: this is intentionally a local, private helper (not `crate::util`) to avoid a
/// cross-module dependency while `util` is being ported in parallel elsewhere.
fn available_threads() -> usize {
    std::cmp::min(
        std::thread::available_parallelism()
            .map(|n| n.get())
            .unwrap_or(4),
        16,
    )
}

/// Represents any parameters that can save state and should survive exits and restarts.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
#[serde(rename_all = "camelCase")]
pub struct SavedState {
    #[serde(default)]
    pub advanced_settings: HashMap<String, AdvancedSetting>,
    #[serde(default)]
    pub server_location: String,
}

impl SavedState {
    /// Builds the default `SavedState`: the ~30 default advanced settings (mirroring the
    /// C# `SavedState.Default`) and an empty server location.
    pub fn default_state() -> SavedState {
        let mut advanced_settings = HashMap::new();

        let threads = available_threads();

        let defaults = [
            AdvancedSetting::new("maxFPS", AdvancedSettingValue::Int(60), true),
            AdvancedSetting::new_switch("addonsRepair", true),
            AdvancedSetting::new_switch("useUpnp", true),
            AdvancedSetting::new_switch("keepServerUpdated", true),
            AdvancedSetting::new_switch("useExperimental", false),
            AdvancedSetting::new("loadSessionSave", AdvancedSettingValue::Str(String::new()), false),
            AdvancedSetting::new_switch("autoRestartOnCrash", false),
            AdvancedSetting::new("autoreload", AdvancedSettingValue::Int(10), false),
            AdvancedSetting::new_switch("noBackend", false),
            AdvancedSetting::new_switch("autoShutdown", false),
            AdvancedSetting::new_switch("logVoting", false),
            AdvancedSetting::new("bindPort", AdvancedSettingValue::Int(2001), false),
            AdvancedSetting::new("nds", AdvancedSettingValue::Int(2), false),
            AdvancedSetting::new("nwkResolution", AdvancedSettingValue::Int(500), false),
            AdvancedSetting::new("staggeringBudget", AdvancedSettingValue::Int(5000), false),
            AdvancedSetting::new("streamingBudget", AdvancedSettingValue::Int(500), false),
            AdvancedSetting::new("streamsDelta", AdvancedSettingValue::Int(100), false),
            AdvancedSetting::new("rpl-timeout-ms", AdvancedSettingValue::Int(10000), false),
            AdvancedSetting::new_switch("aiPartialSim", false),
            AdvancedSetting::new_switch("createDB", false),
            AdvancedSetting::new(
                "debugger",
                AdvancedSettingValue::Str("127.0.0.1".to_string()),
                false,
            ),
            AdvancedSetting::new("debuggerPort", AdvancedSettingValue::Int(1000), false),
            AdvancedSetting::new_switch("disableShadersBuild", false),
            AdvancedSetting::new_switch("generateShaders", false),
            AdvancedSetting::new_switch("rplEncodeAsLongJobs", false),
            AdvancedSetting::new(
                "jobsysShortWorkerCount",
                AdvancedSettingValue::Int(threads as i64),
                false,
            ),
            AdvancedSetting::new(
                "jobsysLongWorkerCount",
                AdvancedSettingValue::Int((threads / 2) as i64),
                false,
            ),
            AdvancedSetting::new("freezeCheck", AdvancedSettingValue::Int(300), false),
            AdvancedSetting::new(
                "freezeCheckMode",
                AdvancedSettingValue::Str("minidump".to_string()),
                false,
            ),
            AdvancedSetting::new_switch("forceDisableNightGrain", false),
        ];

        for setting in defaults {
            advanced_settings.insert(setting.name.clone(), setting);
        }

        SavedState {
            advanced_settings,
            server_location: String::new(),
        }
    }
}

impl Default for SavedState {
    fn default() -> Self {
        Self::default_state()
    }
}
