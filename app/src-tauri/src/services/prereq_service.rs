//! Detection (and, where possible, installation) of the runtime prerequisites the Arma Reforger
//! dedicated server needs before it can start at all.
//!
//! Without this, a machine missing a runtime library fails in the least helpful way possible: the
//! Windows loader refuses to start the process and reports only `0xC0000135`
//! (`STATUS_DLL_NOT_FOUND`) with no indication of *which* library is missing — and only after
//! SteamCMD has already spent several minutes downloading ~10 GB.
//!
//! The required-DLL list below is not guesswork: it was read directly out of
//! `ArmaReforgerServer.exe`'s import table. Only the non-OS-guaranteed imports are checked here
//! (`KERNEL32`, `ntdll`, `WS2_32`, etc. ship with every Windows install and can't meaningfully go
//! missing); the ones that genuinely do go missing on real machines are:
//!
//! - `VCRUNTIME140.dll` / `VCRUNTIME140_1.dll` / `MSVCP140.dll` — the Visual C++ 2015-2022 x64
//!   redistributable. Note `VCRUNTIME140_1.dll` in particular: it was only introduced with VC++
//!   2019, so a machine with the 2015 or 2017 redistributable installed has the other two but not
//!   this one — which is why "the redistributable is already installed" and
//!   `STATUS_DLL_NOT_FOUND` are not contradictory.
//! - `ucrtbase.dll` (+ the `api-ms-win-crt-*` forwarders) — the Universal CRT. Part of the OS on
//!   Windows 10/Server 2016 and newer; delivered via a Windows Update package on older builds.
//! - `GDI32.dll` / `IMM32.dll` / `USER32.dll` — present on any normal install, but genuinely
//!   absent on Windows **Server Core**, which ships without the GUI subsystem. The dedicated
//!   server imports them even though it renders nothing, so Server Core needs the "Server with
//!   Desktop Experience" installation option (or the App Compatibility FOD).

use std::path::{Path, PathBuf};

use crate::models::Prerequisite;

use super::error::ServiceError;

/// Microsoft's official permalink for the current Visual C++ 2015-2022 x64 redistributable.
const VC_REDIST_X64_URL: &str = "https://aka.ms/vs/17/release/vc_redist.x64.exe";

/// The VC++ redistributable's own DLLs, which land in System32 when it's installed.
const VC_RUNTIME_DLLS: &[&str] = &["VCRUNTIME140.dll", "VCRUNTIME140_1.dll", "MSVCP140.dll"];

/// Universal CRT. `ucrtbase.dll` is the real library; the `api-ms-win-crt-*` imports are
/// forwarders that come with it, so checking the base is sufficient and avoids 11 redundant
/// filesystem probes.
const UCRT_DLLS: &[&str] = &["ucrtbase.dll"];

/// GUI-subsystem libraries, absent only on Windows Server Core.
const GUI_SUBSYSTEM_DLLS: &[&str] = &["GDI32.dll", "IMM32.dll", "USER32.dll"];

/// `%SystemRoot%\System32` — where all of the above live when present. A 64-bit process (which
/// this app is) sees the genuine 64-bit System32 here, with no WOW64 redirection to worry about.
fn system32_dir() -> PathBuf {
    let system_root = std::env::var("SystemRoot").unwrap_or_else(|_| r"C:\Windows".to_string());
    PathBuf::from(system_root).join("System32")
}

/// Returns the subset of `dlls` that aren't present in `dir`. Case doesn't matter on NTFS, so a
/// plain existence check is enough.
fn missing_from(dir: &Path, dlls: &[&str]) -> Vec<String> {
    dlls.iter()
        .filter(|dll| !dir.join(dll).exists())
        .map(|dll| dll.to_string())
        .collect()
}

/// Checks the runtime libraries a native Windows dedicated server needs.
pub fn check_windows_runtime() -> Vec<Prerequisite> {
    let system32 = system32_dir();

    let missing_vc = missing_from(&system32, VC_RUNTIME_DLLS);
    let vc = if missing_vc.is_empty() {
        Prerequisite::satisfied(
            "vcredist",
            "Visual C++ 2015-2022 Redistributable (x64)",
            "Installed.",
        )
    } else {
        Prerequisite::missing(
            "vcredist",
            "Visual C++ 2015-2022 Redistributable (x64)",
            missing_vc,
            "The dedicated server can't start without these. Note that VCRUNTIME140_1.dll only \
             ships with the 2019 release and later, so an older 2015/2017 redistributable is not \
             enough — the current x64 redistributable installs all of them. Longbow can download \
             and install it for you.",
            true,
        )
    };

    let missing_ucrt = missing_from(&system32, UCRT_DLLS);
    let ucrt = if missing_ucrt.is_empty() {
        Prerequisite::satisfied("ucrt", "Universal C Runtime", "Installed.")
    } else {
        Prerequisite::missing(
            "ucrt",
            "Universal C Runtime",
            missing_ucrt,
            "The Universal CRT is part of Windows 10 / Server 2016 and newer. On an older build, \
             install the latest Windows updates (specifically update KB2999226) to add it.",
            false,
        )
    };

    let missing_gui = missing_from(&system32, GUI_SUBSYSTEM_DLLS);
    let gui = if missing_gui.is_empty() {
        Prerequisite::satisfied("gui-subsystem", "Windows GUI subsystem", "Present.")
    } else {
        Prerequisite::missing(
            "gui-subsystem",
            "Windows GUI subsystem",
            missing_gui,
            "These are missing on Windows Server Core installations. Even though the dedicated \
             server renders nothing, it links against them and won't start without them. Use the \
             \"Server with Desktop Experience\" installation option, or add the Server Core App \
             Compatibility feature (FOD).",
            false,
        )
    };

    vec![vc, ucrt, gui]
}

/// Asks the dynamic linker *inside WSL* which shared libraries the Linux server binary needs and
/// can't resolve, via `ldd`. This is the Linux counterpart of the DLL checks above, and is more
/// precise than a hardcoded list could be: it reports exactly what this particular build of the
/// binary is missing on this particular distro.
///
/// Returns `Ok(None)` when the check couldn't be run at all (binary not installed yet, `ldd`
/// absent) — an inconclusive check must not be reported to the user as a failure.
pub async fn check_wsl_runtime(
    distro: Option<&str>,
    server_working_dir: &Path,
    server_binary: &str,
) -> Result<Option<Prerequisite>, ServiceError> {
    let output = super::wsl_service::wsl_raw_command(
        distro,
        server_working_dir,
        "ldd",
        &[format!("./{server_binary}")],
    )
    .output()
    .await
    .map_err(ServiceError::Io)?;

    if !output.status.success() {
        // `ldd` missing, binary not there yet, or not a dynamic executable — nothing conclusive.
        return Ok(None);
    }

    let stdout = String::from_utf8_lossy(&output.stdout);
    let missing = parse_ldd_missing(&stdout);

    if missing.is_empty() {
        Ok(Some(Prerequisite::satisfied(
            "wsl-libs",
            "Linux shared libraries",
            "All shared libraries the server needs are present in this distribution.",
        )))
    } else {
        let install_hint = format!(
            "Install them inside your WSL distribution, then try again. On Debian/Ubuntu, \
             `sudo apt-get update && sudo apt-get install -y {}` is usually enough (search for \
             the package providing each library if a name isn't recognised).",
            missing.join(" ")
        );
        Ok(Some(Prerequisite::missing(
            "wsl-libs",
            "Linux shared libraries",
            missing,
            &install_hint,
            false,
        )))
    }
}

/// Extracts the library names `ldd` reported as unresolvable. Its output lists one library per
/// line as `name => path (addr)`, with the path replaced by the literal `not found` when the
/// linker couldn't resolve it.
fn parse_ldd_missing(ldd_stdout: &str) -> Vec<String> {
    ldd_stdout
        .lines()
        .filter(|line| line.contains("not found"))
        .filter_map(|line| line.split("=>").next())
        .map(|name| name.trim().to_string())
        .filter(|name| !name.is_empty())
        .collect()
}

/// Downloads Microsoft's official Visual C++ x64 redistributable and runs it.
///
/// The installer carries a `requireAdministrator` manifest, so Windows shows a UAC consent prompt
/// when it starts — installing into System32 is inherently a machine-wide, elevated operation and
/// this deliberately does not try to work around that. `/passive` keeps the installer's own
/// progress UI visible rather than hiding a privileged install entirely from the person who asked
/// for it.
pub async fn install_vc_redist(app_version: &str, download_dir: &Path) -> Result<(), ServiceError> {
    let client = reqwest::Client::builder()
        .user_agent(format!("Longbow-ServerTool/{app_version}"))
        .build()?;

    let bytes = client
        .get(VC_REDIST_X64_URL)
        .send()
        .await?
        .error_for_status()?
        .bytes()
        .await?;

    std::fs::create_dir_all(download_dir)?;
    let installer_path = download_dir.join("vc_redist.x64.exe");
    std::fs::write(&installer_path, &bytes)?;

    let status = tokio::process::Command::new(&installer_path)
        .args(["/install", "/passive", "/norestart"])
        .status()
        .await
        .map_err(ServiceError::Io)?;

    let _ = std::fs::remove_file(&installer_path);

    // 3010 is the redistributable's documented "success, but a reboot is required" code, and 1638
    // means a newer version is already present — neither is a failure.
    match status.code() {
        Some(0) | Some(3010) | Some(1638) => Ok(()),
        Some(1602) => Err(ServiceError::Other(
            "The Visual C++ Redistributable installation was cancelled.".to_string(),
        )),
        Some(code) => Err(ServiceError::Other(format!(
            "The Visual C++ Redistributable installer failed with exit code {code}."
        ))),
        None => Err(ServiceError::Other(
            "The Visual C++ Redistributable installer was terminated before it finished."
                .to_string(),
        )),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_missing_libraries_from_ldd_output() {
        let output = "\tlinux-vdso.so.1 (0x00007ffd8d5f9000)\n\
                      \tlibssl.so.1.1 => not found\n\
                      \tlibc.so.6 => /lib/x86_64-linux-gnu/libc.so.6 (0x00007f8a2c000000)\n\
                      \tlibcrypto.so.1.1 => not found\n";
        assert_eq!(
            parse_ldd_missing(output),
            vec!["libssl.so.1.1".to_string(), "libcrypto.so.1.1".to_string()]
        );
    }

    #[test]
    fn reports_nothing_missing_when_every_library_resolves() {
        let output = "\tlinux-vdso.so.1 (0x00007ffd8d5f9000)\n\
                      \tlibc.so.6 => /lib/x86_64-linux-gnu/libc.so.6 (0x00007f8a2c000000)\n";
        assert!(parse_ldd_missing(output).is_empty());
    }

    #[test]
    fn missing_from_reports_only_absent_files() {
        let dir = tempfile::tempdir().unwrap();
        std::fs::write(dir.path().join("present.dll"), b"x").unwrap();

        let missing = missing_from(dir.path(), &["present.dll", "absent.dll"]);
        assert_eq!(missing, vec!["absent.dll".to_string()]);
    }

    #[test]
    fn windows_runtime_check_reports_every_component() {
        // Runs against the real machine, so satisfied/unsatisfied depends on the host — but the
        // set of components reported must be stable, and each must carry usable detail text.
        let results = check_windows_runtime();
        let ids: Vec<&str> = results.iter().map(|p| p.id.as_str()).collect();
        assert_eq!(ids, vec!["vcredist", "ucrt", "gui-subsystem"]);
        assert!(results.iter().all(|p| !p.detail.is_empty()));
        // Only the redistributable is something Longbow can install itself.
        assert!(results.iter().all(|p| !p.auto_installable || p.id == "vcredist"));
    }
}
