mod commands;
mod models;

use models::ServerInstance;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
  tauri::Builder::default()
    .invoke_handler(tauri::generate_handler![
      commands::start_server_instance,
      commands::get_server_stats
    ])
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
