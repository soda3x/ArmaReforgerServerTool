//! Support for running the Arma Reforger dedicated server under WSL (Windows Subsystem for
//! Linux) as an alternative to a native Windows install. New in the Rust port — the C#
//! original was Windows-native only.

use std::path::Path;
use std::process::Stdio;

use tokio::process::Command;

use super::error::ServiceError;

/// Which platform the managed dedicated server binary targets.
#[derive(Debug, Clone, PartialEq, Eq, Default, serde::Serialize, serde::Deserialize)]
#[serde(tag = "kind", rename_all = "camelCase")]
pub enum ServerTarget {
    /// Native Windows server, launched directly.
    #[default]
    Windows,
    /// Linux server, launched inside WSL. `distro` selects a specific WSL distribution
    /// (`wsl -d <distro> -- ...`); `None` uses the default distro.
    Wsl { distro: Option<String> },
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
            .as_chunks::<2>().0.iter()
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

/// Builds a `tokio::process::Command` that runs `program args...` inside WSL, with the working
/// directory (a Windows path) translated automatically. `program` is a binary name relative to
/// that working directory (e.g. one SteamCMD just installed there) — it is invoked as `./program`
/// rather than a bare name, since POSIX shells only search `$PATH` for unqualified names and
/// never implicitly include the current directory. It's also `chmod +x`'d immediately before
/// being exec'd: Windows drives mounted into WSL (`/mnt/c/...`) generally don't preserve Linux
/// executable bits, so a binary that was just written onto an NTFS-backed install directory can't
/// be assumed to already be marked executable.
///
/// Uses `--exec` rather than plain `--`: `wsl.exe --` runs the trailing command line through the
/// distribution's *default login shell*, which re-joins the argv `tokio::process::Command` built
/// back into a single string and re-tokenizes it — silently dropping our intended positional
/// parameters (confirmed live: with `--`, `/bin/sh -c '...$1...' sh foo "has space" bar` receives
/// `$1`/`$2`/`$3` as empty). `--exec` executes the given program directly with the given argv,
/// with no shell rewriting it in between, so `program`/`args` arrive on the Linux side exactly as
/// given here — including any embedded spaces.
///
/// The chmod+exec logic itself runs inside `/bin/sh -c '<script>' sh <binary> <args...>` rather
/// than interpolating `program`/`args` into the script text — they're passed as positional
/// parameters instead, so nothing in a mod name, save name, or other launch argument can be
/// interpreted as shell syntax.
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
    cmd.arg("--exec");
    cmd.arg("/bin/sh");
    cmd.arg("-c");
    cmd.arg(r#"bin="$1"; shift; chmod +x -- "$bin"; exec "$bin" "$@""#);
    cmd.arg("sh"); // $0 placeholder, never read by the script above
    cmd.arg(format!("./{program}"));
    cmd.args(args);
    cmd
}

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
    fn wsl_command_runs_binary_relative_to_cwd_via_chmod_exec_wrapper() {
        let cmd = wsl_command(
            Some("Ubuntu"),
            &PathBuf::from(r"C:\Arma Server\arma_reforger"),
            "ArmaReforgerServer",
            &["-config".to_string(), "server.json".to_string()],
        );
        let std_cmd = cmd.as_std();
        let args: Vec<String> = std_cmd.get_args().map(|a| a.to_string_lossy().to_string()).collect();

        assert_eq!(std_cmd.get_program(), "wsl");
        assert_eq!(
            args,
            vec![
                "-d",
                "Ubuntu",
                "--cd",
                "/mnt/c/Arma Server/arma_reforger",
                "--exec",
                "/bin/sh",
                "-c",
                r#"bin="$1"; shift; chmod +x -- "$bin"; exec "$bin" "$@""#,
                "sh",
                "./ArmaReforgerServer",
                "-config",
                "server.json",
            ]
        );
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
