use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct ServerStats {
  id: String,
  name: String,
  cpu: f32,
  ram: f32
}