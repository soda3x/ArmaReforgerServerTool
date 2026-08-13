/******************************************************************************
 * File Name:    server_status_parser.rs
 * Project:      Longbow / ARST-RUST
 * Description:  Utility for parsing server logs and keeping track of
 *               server statistics. Ported from Utils/ServerStatusParser.cs
 ******************************************************************************/

use chrono::{DateTime, Local};
use regex::Regex;
use serde::{Deserialize, Serialize};
use std::sync::LazyLock;

const UNKNOWN_STR: &str = "Unknown";

static STATS_REGEX: LazyLock<Regex> = LazyLock::new(|| {
    Regex::new(r"FPS:\s*(?P<fps>[\d\.]+).*?Mem:\s*(?P<mem>\d+)\s*kB.*?Player:\s*(?P<player>\d+)")
        .expect("invalid STATS_REGEX")
});

static PING_SITE_REGEX: LazyLock<Regex> =
    LazyLock::new(|| Regex::new(r"Ping Site:\s*(?P<site>.*)").expect("invalid PING_SITE_REGEX"));

static JOIN_CODE_REGEX: LazyLock<Regex> = LazyLock::new(|| {
    Regex::new(r"Direct Join Code:\s*(?P<code>\d+)").expect("invalid JOIN_CODE_REGEX")
});

static RCON_REGEX: LazyLock<Regex> = LazyLock::new(|| {
    Regex::new(r"Ip address=(?P<ip>[\d\.]+) and Port=(?P<port>\d+)").expect("invalid RCON_REGEX")
});

static ADDRESS_REGEX: LazyLock<Regex> = LazyLock::new(|| {
    Regex::new(r"Server registered with address:\s*(?P<ip>[\d\.]+):(?P<port>\d+)")
        .expect("invalid ADDRESS_REGEX")
});

/// Rolling "last known" server telemetry, updated by feeding it live
/// server-process stdout lines one at a time.
#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ServerStatus {
    pub last_fps: f64,
    pub last_mem_kb: u64,
    pub last_player_count: u32,
    pub last_ping_site: String,
    pub last_join_code: String,
    pub last_rcon_ip: String,
    pub last_rcon_port: u32,
    pub last_ip: String,
    pub last_port: u32,
    pub server_online: bool,
    pub last_update: DateTime<Local>,
}

impl Default for ServerStatus {
    fn default() -> Self {
        Self {
            last_fps: 0.0,
            last_mem_kb: 0,
            last_player_count: 0,
            last_ping_site: UNKNOWN_STR.to_string(),
            last_join_code: String::new(),
            last_rcon_ip: UNKNOWN_STR.to_string(),
            last_rcon_port: 0,
            last_ip: UNKNOWN_STR.to_string(),
            last_port: 0,
            server_online: false,
            last_update: Local::now(),
        }
    }
}

/// Parses live server log lines and maintains the last-known `ServerStatus`.
pub struct ServerStatusParser {
    status: ServerStatus,
}

impl ServerStatusParser {
    pub fn new() -> Self {
        Self {
            status: ServerStatus::default(),
        }
    }

    fn try_parse_stats(&mut self, line: &str) -> bool {
        if let Some(caps) = STATS_REGEX.captures(line) {
            if let Some(fps) = caps.name("fps").and_then(|m| m.as_str().parse::<f64>().ok()) {
                self.status.last_fps = fps;
            }
            if let Some(mem) = caps.name("mem").and_then(|m| m.as_str().parse::<u64>().ok()) {
                self.status.last_mem_kb = mem;
            }
            if let Some(player) = caps
                .name("player")
                .and_then(|m| m.as_str().parse::<u32>().ok())
            {
                self.status.last_player_count = player;
            }
            true
        } else {
            false
        }
    }

    fn try_parse_ping_site(&mut self, line: &str) -> bool {
        if let Some(caps) = PING_SITE_REGEX.captures(line) {
            if let Some(site) = caps.name("site") {
                self.status.last_ping_site = site.as_str().trim().to_string();
            }
            true
        } else {
            false
        }
    }

    fn try_parse_join_code(&mut self, line: &str) -> bool {
        if let Some(caps) = JOIN_CODE_REGEX.captures(line) {
            if let Some(code) = caps.name("code") {
                self.status.last_join_code = code.as_str().to_string();
            }
            true
        } else {
            false
        }
    }

    fn try_parse_rcon(&mut self, line: &str) -> bool {
        if let Some(caps) = RCON_REGEX.captures(line) {
            if let Some(ip) = caps.name("ip") {
                self.status.last_rcon_ip = ip.as_str().to_string();
            }
            if let Some(port) = caps
                .name("port")
                .and_then(|m| m.as_str().parse::<u32>().ok())
            {
                self.status.last_rcon_port = port;
            }
            true
        } else {
            false
        }
    }

    fn try_parse_address(&mut self, line: &str) -> bool {
        if let Some(caps) = ADDRESS_REGEX.captures(line) {
            if let Some(ip) = caps.name("ip") {
                self.status.last_ip = ip.as_str().to_string();
            }
            // Intentional bug fix vs the C# original, which used
            // `!x.Equals("") || !x.Equals("Unknown")` (always true due to OR).
            self.status.server_online =
                !self.status.last_ip.is_empty() && self.status.last_ip != "Unknown";
            if let Some(port) = caps
                .name("port")
                .and_then(|m| m.as_str().parse::<u32>().ok())
            {
                self.status.last_port = port;
            }
            true
        } else {
            false
        }
    }

    /// Feeds a single log line into the parser, updating internal status.
    /// Returns true if any of the five patterns matched.
    pub fn parse_line(&mut self, line: &str) -> bool {
        self.status.last_update = Local::now();

        let mut matched = false;
        matched |= self.try_parse_address(line);
        matched |= self.try_parse_rcon(line);
        matched |= self.try_parse_join_code(line);
        matched |= self.try_parse_ping_site(line);
        matched |= self.try_parse_stats(line);
        matched
    }

    pub fn status(&self) -> &ServerStatus {
        &self.status
    }
}

impl Default for ServerStatusParser {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_stats_line() {
        let mut parser = ServerStatusParser::new();
        let matched = parser.parse_line("FPS: 59.94 | Mem: 524288 kB | Player: 3/64");
        assert!(matched);
        assert_eq!(parser.status().last_fps, 59.94);
        assert_eq!(parser.status().last_mem_kb, 524288);
        assert_eq!(parser.status().last_player_count, 3);
    }

    #[test]
    fn parses_ping_site_line() {
        let mut parser = ServerStatusParser::new();
        let matched = parser.parse_line("Ping Site: frankfurt  ");
        assert!(matched);
        assert_eq!(parser.status().last_ping_site, "frankfurt");
    }

    #[test]
    fn parses_join_code_line() {
        let mut parser = ServerStatusParser::new();
        let matched = parser.parse_line("Direct Join Code: 123456");
        assert!(matched);
        assert_eq!(parser.status().last_join_code, "123456");
    }

    #[test]
    fn parses_rcon_line() {
        let mut parser = ServerStatusParser::new();
        let matched = parser.parse_line("Rcon started. Ip address=192.168.1.10 and Port=19999");
        assert!(matched);
        assert_eq!(parser.status().last_rcon_ip, "192.168.1.10");
        assert_eq!(parser.status().last_rcon_port, 19999);
    }

    #[test]
    fn parses_address_line() {
        let mut parser = ServerStatusParser::new();
        let matched = parser.parse_line("Server registered with address: 203.0.113.5:2001");
        assert!(matched);
        assert_eq!(parser.status().last_ip, "203.0.113.5");
        assert_eq!(parser.status().last_port, 2001);
    }

    #[test]
    fn server_online_only_after_valid_address() {
        let mut parser = ServerStatusParser::new();
        // Before any address match, server should not be online.
        assert!(!parser.status().server_online);
        parser.parse_line("FPS: 30.0 | Mem: 1024 kB | Player: 0");
        assert!(!parser.status().server_online);

        // After a valid, non-"Unknown" address match, it should become true.
        parser.parse_line("Server registered with address: 10.0.0.1:2001");
        assert!(parser.status().server_online);
    }
}
