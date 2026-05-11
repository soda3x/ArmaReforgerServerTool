use serde::{Deserialize, Serialize};
use std::path::PathBuf;

#[derive(Debug, Serialize, Deserialize, Clone)]
pub enum GameType {
  Reforger,
  Arma3,
  Arma4
}

#[derive(Debug, Serialize, Deserialize, Clone)]
pub enum ServerStatus {
  Running,
  Stopped,
  Unknown
}

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct ServerInstance {
  id: String,
  name: String,
  game: GameType,
  path: PathBuf,
  status: ServerStatus,
  port: u16,
}