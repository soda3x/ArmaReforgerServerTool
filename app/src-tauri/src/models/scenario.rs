use serde::{Deserialize, Serialize};

/// Represents a default Arma Reforger scenario with a friendly name and a ".conf" path.
#[derive(Debug, Clone, Default, PartialEq, Eq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Scenario {
    #[serde(default)]
    pub name: String,
    #[serde(default)]
    pub path: String,
}

impl Scenario {
    pub fn new(name: impl Into<String>, path: impl Into<String>) -> Self {
        Self {
            name: name.into(),
            path: path.into(),
        }
    }
}

impl std::fmt::Display for Scenario {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{} // {}", self.name, self.path)
    }
}
