//! Plain-language explanations for dedicated-server failures.
//!
//! Two separate things live here, deliberately kept apart:
//!
//! - [`diagnose`] matches a **server log line** against known failure signatures. Every pattern
//!   here has been observed verbatim — either in a real server log, or as a literal string inside
//!   `ArmaReforgerServer` itself. Nothing is added on the strength of "this error probably looks
//!   like X": a pattern that never matches is dead weight, and one that matches the wrong thing
//!   actively misleads.
//! - [`common_issues`] is a symptom-indexed reference for problems that produce **no server-side
//!   error at all** — the server runs perfectly and the operator's actual complaint is "nobody can
//!   see it". Those can't be log-matched by definition, so they're offered as guidance instead of
//!   pretending to be detections.

use serde::Serialize;

/// A failure signature that can be recognised in a server log line.
#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct Diagnostic {
    pub id: &'static str,
    /// Substring identifying the condition. Matched case-sensitively, since the engine's own
    /// casing is stable and loosening it would invite false positives on user-supplied text
    /// (mod names, save names) that happens to echo a keyword.
    #[serde(skip)]
    pub pattern: &'static str,
    pub title: &'static str,
    /// What the message actually means.
    pub meaning: &'static str,
    /// What to do about it.
    pub fix: &'static str,
}

/// A problem that has no distinctive server-side error line — indexed by what the operator
/// observes rather than by anything in the log.
#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct CommonIssue {
    pub id: &'static str,
    pub symptom: &'static str,
    pub causes: &'static [&'static str],
}

/// Log-line signatures, most specific first: [`diagnose`] returns the first match, so a narrow
/// pattern must precede any broader one it would otherwise be swallowed by.
static DIAGNOSTICS: &[Diagnostic] = &[
    Diagnostic {
        id: "config-unreadable",
        pattern: "Unable to open server config file",
        title: "The server couldn't open its config file",
        meaning:
            "The server was given a path to server.json that it can't read. The file is missing, \
             the path is wrong, or the server process lacks permission to read it. Under WSL this \
             also happens when the path is a Windows path (C:\\...) instead of a Linux one \
             (/mnt/c/...), since the server is a Linux process there.",
        fix:
            "Confirm the install directory on the Management tab is correct and that server.json \
             exists in it. Longbow writes that file and passes its path automatically, so if this \
             appears with an unmodified setup, check that the install directory isn't on a drive \
             or share the server can't reach.",
    },
    Diagnostic {
        id: "config-schema-min-length",
        pattern: "bellow the minimum limit",
        title: "A config value is shorter than the server allows",
        meaning:
            "The server validates server.json against its own JSON schema, and a field is below \
             its minimum length. The most common case is an empty Server Name. (The misspelling \
             is the server's own.)",
        fix:
            "The preceding log line names the offending field as a JSON pointer, e.g. \
             \"#/game/name\". Fill that field in on the Configuration tab. Longbow refuses to \
             start with an empty Server Name or scenario, so this points at a field it doesn't \
             pre-check yet.",
    },
    Diagnostic {
        id: "config-schema-pattern",
        pattern: "does not match the required pattern",
        title: "A config value is in the wrong format",
        meaning:
            "A field in server.json doesn't match the format the server requires. The usual cause \
             is a scenario ID that isn't a 16-character resource GUID in braces followed by a \
             path, e.g. {ECC61978EDCC2B5A}Missions/23_Campaign.conf.",
        fix:
            "The surrounding log lines give the field and the expected pattern. For a scenario, \
             use the Select… button on the Configuration tab rather than typing the ID by hand.",
    },
    Diagnostic {
        id: "config-invalid",
        pattern: "There are errors in server config",
        title: "The config file was rejected",
        meaning:
            "server.json was read but failed validation, so the server stopped before starting. \
             The BACKEND lines just above list every field that failed and why.",
        fix:
            "Fix the fields named above and start again. If you hand-edited server.json, check it \
             is still valid JSON — a stray or missing comma will fail it before any field is even \
             looked at.",
    },
    Diagnostic {
        id: "script-compile-failed",
        pattern: r#"Can't compile "Game" script module!"#,
        title: "A mod's scripts failed to compile",
        meaning:
            "One mod's script code has an error, and that takes the entire server down — the game \
             compiles every mod's scripts into a single module, so one bad mod stops all of them. \
             This is a fault in the mod (or an incompatibility with the current game version), not \
             in your server setup. A common form is a mod referencing an editor/Workbench-only \
             class that doesn't exist in the dedicated-server build.",
        fix:
            "The SCRIPT (E) lines just above name the script files that failed. Disable the mod \
             that owns them and start again; if it isn't obvious which one that is, disable half \
             your mods and retry to narrow it down. Then check the mod's Workshop page for an \
             update, and report it to the author if there isn't one.",
    },
    Diagnostic {
        id: "addon-loading-failed",
        pattern: "Addon loading failed",
        title: "The server gave up loading mods",
        meaning:
            "Mod loading failed and the server stopped. The braced list is every mod that was \
             loaded when the failure happened — not the one at fault — so treat it as the set to \
             search, not a list of broken mods.",
        fix:
            "Look for a more specific error above this line (a script compile error, or a missing \
             dependency). If nothing stands out, disable half the mods and retry to bisect. Also \
             confirm every mod's dependencies are enabled — Browse Workshop shows what each mod \
             requires and can add them for you.",
    },
    Diagnostic {
        id: "resource-guid-mismatch",
        pattern: "Wrong GUID/name for resource",
        title: "A mod referenced a resource that doesn't match",
        meaning:
            "A mod points at a resource by a GUID that doesn't resolve to what it expects — \
             usually a localisation file. This is normally harmless: it's logged as an error, but \
             servers routinely start and run fine with these present.",
        fix:
            "Safe to ignore unless the server actually fails to start. If it does, the failure \
             will be reported separately below this line — diagnose that instead.",
    },
    Diagnostic {
        id: "linux-binary-not-found",
        pattern: "ArmaReforgerServer: command not found",
        title: "WSL couldn't find the Linux server binary",
        meaning:
            "The server was launched under WSL but the Linux binary wasn't there to run. Either \
             SteamCMD installed the Windows build instead of the Linux one, or the binary isn't \
             where the launch expected it.",
        fix:
            "Make sure Server Target is set to WSL before starting, so SteamCMD is told to fetch \
             the Linux build. If you previously installed with Windows selected, start once more \
             with WSL selected to download the Linux files.",
    },
    Diagnostic {
        id: "game-init-failed",
        pattern: "Unable to initialize the game",
        title: "The game couldn't start",
        meaning:
            "A catch-all the engine prints when startup fails. On its own it says nothing about \
             the cause — the real error is always logged above it.",
        fix:
            "Scroll up to the first line marked (E) and work from that. It's the actual failure; \
             this line is only the consequence.",
    },
];

/// Problems that leave no distinctive trace in the server log.
static COMMON_ISSUES: &[CommonIssue] = &[
    CommonIssue {
        id: "not-in-browser",
        symptom: "The server runs fine but doesn't appear in the in-game browser",
        causes: &[
            "\"Visible in server list\" is off on the Configuration tab — the server runs \
             normally and simply isn't advertised.",
            "The A2S query port (default UDP 17777) isn't forwarded or is blocked. Without it \
             the server runs but is never listed, which is the single most common cause of this.",
            "The game port (default UDP 2001) isn't forwarded, so it's listed but unjoinable.",
            "Public Address is wrong or empty when hosting behind NAT — it must be the public IP \
             players reach you on, not the LAN address.",
            "The server has only just started; registration isn't instant.",
        ],
    },
    CommonIssue {
        id: "version-mismatch",
        symptom: "Players get a version mismatch and can't join",
        causes: &[
            "The server is on a different game build than the clients. Keep \"Keep server \
             up-to-date\" enabled so SteamCMD updates it on every start.",
            "The server is on the experimental branch while players are on the release branch \
             (or the reverse) — check \"Use Experimental Server Branch\" on the Management tab.",
            "A mod updated on the Workshop but the server still has the old version cached; \
             restarting with updates enabled pulls the new one.",
        ],
    },
    CommonIssue {
        id: "console-players-cannot-join",
        symptom: "Xbox or PlayStation players can't join, but PC players can",
        causes: &[
            "Cross-platform is off, or the supported platforms list doesn't include the \
             console — both are set by the Cross-platform toggle on the Configuration tab.",
            "BattlEye is disabled. Console players can only join servers with BattlEye enabled.",
            "The server runs mods. Consoles can't join modded servers.",
        ],
    },
    CommonIssue {
        id: "rcon-not-connecting",
        symptom: "RCON won't connect",
        causes: &[
            "The RCON password is under 3 characters or contains a space — the server requires \
             at least 3 characters and no spaces, and rejects the config otherwise.",
            "The RCON port isn't forwarded, or collides with the game or A2S port.",
        ],
    },
    CommonIssue {
        id: "crashes-under-load",
        symptom: "The server starts fine but crashes or stutters once players join",
        causes: &[
            "Max Players is set beyond what the hardware can carry — the ceiling is 128, but a \
             realistic figure depends on the CPU, the scenario's AI count, and the mod load.",
            "A mod is misbehaving under load; disable mods in halves to find it.",
            "The machine ran out of memory. Watch memory use on the Status tab while players \
             connect.",
            "Enable \"Auto-restart on Crash\" on the Management tab so a crash doesn't leave the \
             server down until someone notices.",
        ],
    },
    CommonIssue {
        id: "install-problems",
        symptom: "SteamCMD downloads repeatedly, or the install never completes",
        causes: &[
            "The install directory is inside OneDrive, Dropbox, or similar. Cloud sync \
             virtualises files and corrupts SteamCMD installs — move it somewhere local.",
            "SteamCMD updates itself and exits without installing the game on a first run. \
             Longbow retries automatically; repeated failures usually mean the point above.",
            "Not enough free disk space — the server plus a large mod set runs to tens of GB.",
        ],
    },
];

/// Returns the diagnostic matching `line`, if any.
pub fn diagnose(line: &str) -> Option<&'static Diagnostic> {
    DIAGNOSTICS.iter().find(|d| line.contains(d.pattern))
}

pub fn catalog() -> &'static [Diagnostic] {
    DIAGNOSTICS
}

pub fn common_issues() -> &'static [CommonIssue] {
    COMMON_ISSUES
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn recognizes_lines_captured_from_real_server_logs() {
        // Every line below is copied verbatim from an actual failing run.
        let cases = [
            (
                " BACKEND   (E): Unable to open server config file: C:\\Users\\x\\reforger\\server.json",
                "config-unreadable",
            ),
            (
                "   BACKEND   (E): Param \"#/game/name\" is bellow the minimum limit of value/length. The minimum allowed is 1.",
                "config-schema-min-length",
            ),
            (
                "   BACKEND   (E): Value of \"#/game/scenarioId\" does not match the required pattern. Value: \"\"",
                "config-schema-pattern",
            ),
            (
                " BACKEND   (E): There are errors in server config!",
                "config-invalid",
            ),
            (
                " SCRIPT    (E): Can't compile \"Game\" script module!",
                "script-compile-failed",
            ),
            (
                "ENGINE    (E): Addon loading failed {5B383D4CB27E0D54,5E389BB9F58B79A6}",
                "addon-loading-failed",
            ),
            (
                "   RESOURCES (E): Wrong GUID/name for resource @\"{0AFDD005CD669892}Language/rhs_localization.st\" in property \"StringTableSource\"",
                "resource-guid-mismatch",
            ),
            (
                "/bin/bash: line 1: ArmaReforgerServer: command not found",
                "linux-binary-not-found",
            ),
            (
                "ENGINE    (E): Unable to initialize the game",
                "game-init-failed",
            ),
        ];

        for (line, expected_id) in cases {
            let got = diagnose(line).unwrap_or_else(|| panic!("no diagnostic matched: {line}"));
            assert_eq!(got.id, expected_id, "wrong diagnostic for: {line}");
        }
    }

    #[test]
    fn ordinary_log_lines_match_nothing() {
        // Healthy startup lines, and a mod name that echoes diagnostic vocabulary, must not
        // produce a scary explanation.
        for line in [
            "BACKEND      : Server config loaded.",
            "   BACKEND      : JSON is Valid",
            "ENGINE       : Game successfully created.",
            "PROFILING    : Compiling Game scripts took: 1321.824000 ms",
            " ENGINE       : gproj: './addons/data/ArmaReforger.gproj' guid: '58D0FB3206B6F859' (packed)",
        ] {
            assert!(diagnose(line).is_none(), "false positive on: {line}");
        }
    }

    #[test]
    fn a_successful_compile_is_not_read_as_a_failure() {
        // "Compiling Game scripts" contains most of the compile-failure pattern's words; only
        // the real failure line must match.
        assert!(diagnose(" SCRIPT       : SCRIPT       : Compiling Game scripts").is_none());
        assert!(diagnose(" SCRIPT    (E): Can't compile \"Game\" script module!").is_some());
    }

    #[test]
    fn every_entry_is_populated_and_uniquely_identified() {
        let mut ids: Vec<&str> = Vec::new();
        for d in catalog() {
            assert!(!d.pattern.is_empty(), "{} has no pattern", d.id);
            assert!(!d.meaning.is_empty(), "{} has no meaning", d.id);
            assert!(!d.fix.is_empty(), "{} has no fix", d.id);
            ids.push(d.id);
        }
        for i in common_issues() {
            assert!(!i.causes.is_empty(), "{} has no causes", i.id);
            ids.push(i.id);
        }
        let unique: std::collections::HashSet<_> = ids.iter().collect();
        assert_eq!(unique.len(), ids.len(), "diagnostic ids must be unique");
    }
}
