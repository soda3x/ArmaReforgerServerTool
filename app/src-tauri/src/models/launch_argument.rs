/// Represents one CLI flag for the Arma Reforger dedicated server, e.g. `-bindPort 2001`
/// (key + value) or `-nobackend` (bare switch, empty value).
#[derive(Debug, Clone, PartialEq, Eq, Default, serde::Serialize, serde::Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct LaunchArgument {
    pub key: String,
    /// Empty string means "switch, no value".
    pub value: String,
}

impl LaunchArgument {
    /// Constructs a launch argument with a key and value (e.g. `-bindPort 2001`).
    ///
    /// Returns an error if the trimmed key is empty.
    pub fn try_new(key: impl Into<String>, value: impl Into<String>) -> Result<Self, String> {
        let key = key.into();
        let trimmed_key = key.trim();
        if trimmed_key.is_empty() {
            return Err("Key length is invalid".to_string());
        }
        Ok(Self {
            key: trimmed_key.to_string(),
            value: value.into().trim().to_string(),
        })
    }

    /// Convenience constructor that panics if the key is empty. Prefer [`Self::try_new`]
    /// at call sites where the key isn't a compile-time constant.
    pub fn new(key: impl Into<String>, value: impl Into<String>) -> Self {
        Self::try_new(key, value).expect("LaunchArgument::new called with an invalid key")
    }

    /// Constructs a 'switch' style launch argument with only a key (e.g. `-nobackend`).
    pub fn switch(key: impl Into<String>) -> Self {
        Self::new(key, "")
    }
}

impl std::fmt::Display for LaunchArgument {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let rendered = format!("-{} {}", self.key, self.value);
        write!(f, "{}", rendered.trim())
    }
}

/// Holds all known launch arguments for the Arma Reforger Server. All fields are optional;
/// `None` means "not set / not included" on the command line.
#[derive(Debug, Clone, Default, serde::Serialize, serde::Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct LaunchArguments {
    pub config: Option<LaunchArgument>,
    pub profile: Option<LaunchArgument>,
    pub addons_dir: Option<LaunchArgument>,
    pub log_stats: Option<LaunchArgument>,
    pub max_fps: Option<LaunchArgument>,
    pub bind_port: Option<LaunchArgument>,
    pub nds: Option<LaunchArgument>,
    pub nwk_resolution: Option<LaunchArgument>,
    pub staggering_budget: Option<LaunchArgument>,
    pub streaming_budget: Option<LaunchArgument>,
    pub streams_delta: Option<LaunchArgument>,
    pub load_session_save: Option<LaunchArgument>,
    pub log_level: Option<LaunchArgument>,
    pub auto_reload: Option<LaunchArgument>,
    pub rpl_timeout_ms: Option<LaunchArgument>,
    pub freeze_check: Option<LaunchArgument>,
    pub freeze_check_mode: Option<LaunchArgument>,
    pub addons_repair: Option<LaunchArgument>,
    pub auto_shutdown: Option<LaunchArgument>,
    pub log_voting: Option<LaunchArgument>,
    pub ai_partial_sim: Option<LaunchArgument>,
    pub create_db: Option<LaunchArgument>,
    pub debugger: Option<LaunchArgument>,
    pub debugger_port: Option<LaunchArgument>,
    pub disable_shaders_build: Option<LaunchArgument>,
    pub generate_shaders: Option<LaunchArgument>,
    pub rpl_encode_as_long_jobs: Option<LaunchArgument>,
    pub job_sys_short_worker_count: Option<LaunchArgument>,
    pub job_sys_long_worker_count: Option<LaunchArgument>,
    pub force_disable_night_grain: Option<LaunchArgument>,
}
