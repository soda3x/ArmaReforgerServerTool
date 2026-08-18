//! Parses SteamCMD's progress lines into structured values.
//!
//! While installing or updating the dedicated server, SteamCMD emits lines like:
//!
//! ```text
//!  Update state (0x61) downloading, progress: 46.18 (4728063568 / 10237303511)
//!  Update state (0x81) verifying update, progress: 11.38 (1165171472 / 10237303511)
//!  Update state (0x0) unknown, progress: 0.00 (0 / 0)
//! ```
//!
//! Those numbers are the only indication a ~10 GB transfer is making progress at all, so they're
//! lifted out of the log and shown as a progress bar rather than left to scroll past.

use std::sync::LazyLock;

use regex::Regex;
use serde::Serialize;

static PROGRESS_REGEX: LazyLock<Regex> = LazyLock::new(|| {
    Regex::new(
        r"Update state \(0x[0-9a-fA-F]+\)\s*(?P<stage>[^,]+),\s*progress:\s*(?P<percent>[\d.]+)\s*\((?P<done>\d+)\s*/\s*(?P<total>\d+)\)",
    )
    .expect("invalid PROGRESS_REGEX")
});

#[derive(Debug, Clone, PartialEq, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct SteamCmdProgress {
    /// SteamCMD's own word for what it's doing — "downloading", "verifying update", and so on.
    pub stage: String,
    pub percent: f64,
    pub bytes_done: u64,
    pub bytes_total: u64,
}

/// Extracts progress from a SteamCMD line, or `None` if the line isn't one.
///
/// A line with a zero total is treated as no progress: SteamCMD emits
/// `Update state (0x0) unknown, progress: 0.00 (0 / 0)` as a terminator once a transfer
/// finishes, and showing that as "0%" would make a completed download look like a stalled one.
pub fn parse_progress(line: &str) -> Option<SteamCmdProgress> {
    let caps = PROGRESS_REGEX.captures(line)?;
    let bytes_total: u64 = caps.name("total")?.as_str().parse().ok()?;
    if bytes_total == 0 {
        return None;
    }

    Some(SteamCmdProgress {
        stage: caps.name("stage")?.as_str().trim().to_string(),
        percent: caps.name("percent")?.as_str().parse().ok()?,
        bytes_done: caps.name("done")?.as_str().parse().ok()?,
        bytes_total,
    })
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_a_download_progress_line() {
        let progress =
            parse_progress(" Update state (0x61) downloading, progress: 46.18 (4728063568 / 10237303511)")
                .unwrap();
        assert_eq!(progress.stage, "downloading");
        assert_eq!(progress.percent, 46.18);
        assert_eq!(progress.bytes_done, 4728063568);
        assert_eq!(progress.bytes_total, 10237303511);
    }

    #[test]
    fn parses_a_multi_word_stage() {
        let progress =
            parse_progress(" Update state (0x81) verifying update, progress: 11.38 (1165171472 / 10237303511)")
                .unwrap();
        assert_eq!(progress.stage, "verifying update");
        assert_eq!(progress.percent, 11.38);
    }

    #[test]
    fn treats_a_zero_total_as_no_progress() {
        // SteamCMD's end-of-transfer terminator — reporting it would show 0% right after a
        // download completed.
        assert_eq!(parse_progress(" Update state (0x0) unknown, progress: 0.00 (0 / 0)"), None);
    }

    #[test]
    fn ignores_unrelated_lines() {
        for line in [
            "Success! App '1874900' fully installed.",
            "Connecting anonymously to Steam Public...OK",
            " ENGINE       : Initializing engine, version 191843",
            "[  0%] Checking for available updates...",
        ] {
            assert_eq!(parse_progress(line), None, "false positive on: {line}");
        }
    }
}
