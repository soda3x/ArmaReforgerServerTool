//! Support for running the Arma Reforger dedicated server under WSL (Windows Subsystem for
//! Linux) as an alternative to a native Windows install. New in the Rust port — the C#
//! original was Windows-native only.

use std::path::{Path, PathBuf};
use std::process::Stdio;

use tokio::process::Command;

use super::error::ServiceError;

/// Which platform the managed dedicated server binary targets.
#[derive(Debug, Clone, PartialEq, Eq, serde::Serialize, serde::Deserialize)]
#[serde(tag = "kind", rename_all = "camelCase")]
pub enum ServerTarget {
    /// Native Windows server, launched directly.
    Windows,
    /// Linux server, launched inside WSL. `distro` selects a specific WSL distribution
    /// (`wsl -d <distro> -- ...`); `None` uses the default distro.
    Wsl { distro: Option<String> },
}

impl Default for ServerTarget {
    fn default() -> Self {
        ServerTarget::Windows
    }
}

/// Checks whether `wsl.exe` is available on PATH and at least one distribution is installed.
pub async fn is_wsl_available() -> bool {
    match Command::new("wsl")
        .arg("--status")
        .stdout(Stdio::null())
        .stderr(Stdio::null())
        .status()
        .await
    {
        Ok(status) => status.success(),
        Err(_) => false,
    }
}

/// Lists installed WSL distribution names (via `wsl -l -q`, which prints one name per line
/// with no extra decoration).
pub async fn list_distros() -> Result<Vec<String>, ServiceError> {
    let output = Command::new("wsl")
        .args(["-l", "-q"])
        .output()
        .await
        .map_err(ServiceError::Io)?;

    if !output.status.success() {
        return Err(ServiceError::Other(
            "Failed to list WSL distributions".to_string(),
        ));
    }

    // `wsl -l -q` emits UTF-16LE on some Windows builds; try UTF-16 first, fall back to UTF-8.
    let text = decode_wsl_output(&output.stdout);

    Ok(text
        .lines()
        .map(|l| l.trim().trim_matches('\0').to_string())
        .filter(|l| !l.is_empty())
        .collect())
}

fn decode_wsl_output(bytes: &[u8]) -> String {
    if bytes.len() >= 2 && bytes.iter().skip(1).step_by(2).take(8).all(|b| *b == 0) {
        // Looks like UTF-16LE (lots of zero high bytes) — decode as such.
        let utf16: Vec<u16> = bytes
            .chunks_exact(2)
            .map(|c| u16::from_le_bytes([c[0], c[1]]))
            .collect();
        String::from_utf16_lossy(&utf16)
    } else {
        String::from_utf8_lossy(bytes).to_string()
    }
}

/// Translates a Windows path (e.g. `C:\Games\ArmaServer`) into its WSL equivalent
/// (`/mnt/c/Games/ArmaServer`). This is a plain string transform (no `wslpath` invocation
/// needed for the common case of a drive-letter path).
pub fn windows_path_to_wsl(path: &Path) -> String {
    let s = path.to_string_lossy().replace('\\', "/");
    let mut chars = s.chars();
    match (chars.next(), chars.next()) {
        (Some(drive), Some(':')) if drive.is_ascii_alphabetic() => {
            let rest = &s[2..];
            format!("/mnt/{}{}", drive.to_ascii_lowercase(), rest)
        }
        _ => s,
    }
}

/// Builds a `tokio::process::Command` that runs `program args...` inside WSL, with the
/// working directory (a Windows path) translated automatically. `program` and `args` should
/// already be Linux-side values (e.g. a Linux binary path, not a Windows one).
pub fn wsl_command(
    distro: Option<&str>,
    working_dir_windows: &Path,
    program: &str,
    args: &[String],
) -> Command {
    let mut cmd = Command::new("wsl");
    if let Some(distro) = distro {
        cmd.args(["-d", distro]);
    }
    let wsl_cwd = windows_path_to_wsl(working_dir_windows);
    cmd.arg("--cd").arg(&wsl_cwd);
    cmd.arg("--");
    cmd.arg(program);
    cmd.args(args);
    cmd
}

/// Convenience: the WSL path to a dedicated server binary given its Windows install dir.
pub fn wsl_server_binary_path(install_dir_windows: &Path, relative: &str) -> String {
    let base = windows_path_to_wsl(install_dir_windows);
    format!("{}/{}", base.trim_end_matches('/'), relative.trim_start_matches('/'))
}

#[allow(dead_code)]
fn _unused_pathbuf_import_anchor(_p: PathBuf) {}

#[cfg(test)]
mod tests {
    use super::*;
    use std::path::PathBuf;

    #[test]
    fn translates_drive_letter_path() {
        let p = PathBuf::from(r"C:\Games\ArmaServer\steamcmd");
        assert_eq!(windows_path_to_wsl(&p), "/mnt/c/Games/ArmaServer/steamcmd");
    }

    #[test]
    fn lowercases_drive_letter() {
        let p = PathBuf::from(r"D:\stuff");
        assert_eq!(windows_path_to_wsl(&p), "/mnt/d/stuff");
    }

    #[test]
    fn server_target_serializes_as_tagged_enum() {
        let t = ServerTarget::Wsl {
            distro: Some("Ubuntu".to_string()),
        };
        let json = serde_json::to_string(&t).unwrap();
        assert!(json.contains("\"kind\":\"wsl\""));
        assert!(json.contains("Ubuntu"));
    }
}
