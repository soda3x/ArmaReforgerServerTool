use serde::{Deserialize, Serialize};
use std::path::PathBuf;

pub mod server;
pub mod stats;

pub use server::{ServerInstance, ServerStatus, GameType};
pub use stats::ServerStats;