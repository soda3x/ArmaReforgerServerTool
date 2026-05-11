#[tauri::command]
pub fn start_server_instance(id: String) -> Result<String, String> {
  Ok(format!("Orchestrator started instance: {}", id))
}

#[tauri::command]
pub fn get_server_stats(id: String) -> Result<String, String> {
  Ok(format!("Stats for {}: CPU n%, RAM nGB", id))
}