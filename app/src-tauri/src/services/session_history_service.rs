//! Tracks player join/leave sessions by diffing successive RCON `#players` snapshots
//! ([`RconService`](super::rcon_service::RconService) has no notion of session history itself —
//! this is fed each snapshot by the event forwarder in `lib.rs`). Completed sessions are
//! appended one-per-line to a JSONL file, so a crash mid-write can lose at most the one
//! in-progress append rather than corrupting previously recorded history.

use std::collections::HashMap;
use std::io::{BufRead, Write};
use std::path::PathBuf;

use chrono::Local;

use crate::models::{PlayerSession, RconPlayer};

use super::error::ServiceError;

pub struct SessionHistoryService {
    path: PathBuf,
    /// Sessions currently open (the player was present in the most recent snapshot), keyed by
    /// player id.
    open: HashMap<String, PlayerSession>,
}

impl SessionHistoryService {
    pub fn new(path: impl Into<PathBuf>) -> Self {
        Self { path: path.into(), open: HashMap::new() }
    }

    /// Diffs `players` against the currently-open sessions: ids not seen before open a new
    /// session, ids previously open but now missing are closed and appended to the history file.
    /// Returns the sessions that were closed by this call.
    pub fn record_snapshot(&mut self, players: &[RconPlayer]) -> Result<Vec<PlayerSession>, ServiceError> {
        let now = Local::now();
        let present: std::collections::HashSet<&str> = players.iter().map(|p| p.id.as_str()).collect();

        for player in players {
            self.open.entry(player.id.clone()).or_insert_with(|| PlayerSession {
                player_id: player.id.clone(),
                player_name: player.name.clone(),
                joined_at: now,
                left_at: None,
            });
        }

        let left_ids: Vec<String> =
            self.open.keys().filter(|id| !present.contains(id.as_str())).cloned().collect();

        let mut closed = Vec::new();
        for id in left_ids {
            if let Some(mut session) = self.open.remove(&id) {
                session.left_at = Some(now);
                closed.push(session);
            }
        }

        if !closed.is_empty() {
            self.append(&closed)?;
        }

        Ok(closed)
    }

    /// Closes every currently-open session (e.g. on RCON disconnect / server stop) so none is
    /// left dangling with `left_at: None` forever.
    pub fn close_all_open_sessions(&mut self) -> Result<Vec<PlayerSession>, ServiceError> {
        let now = Local::now();
        let mut closed: Vec<PlayerSession> = self.open.drain().map(|(_, mut s)| {
            s.left_at = Some(now);
            s
        }).collect();
        closed.sort_by_key(|s| s.player_id.clone());

        if !closed.is_empty() {
            self.append(&closed)?;
        }

        Ok(closed)
    }

    fn append(&self, sessions: &[PlayerSession]) -> Result<(), ServiceError> {
        let mut file = std::fs::OpenOptions::new().create(true).append(true).open(&self.path)?;
        for session in sessions {
            let json = serde_json::to_string(session)?;
            writeln!(file, "{json}")?;
        }
        Ok(())
    }

    /// Reads every recorded session (completed sessions from disk, plus any still-open ones),
    /// newest-first.
    pub fn list_sessions(&self, limit: usize) -> Result<Vec<PlayerSession>, ServiceError> {
        let mut sessions: Vec<PlayerSession> = if self.path.exists() {
            let file = std::fs::File::open(&self.path)?;
            std::io::BufReader::new(file)
                .lines()
                .filter_map(|line| line.ok())
                .filter(|line| !line.trim().is_empty())
                .filter_map(|line| serde_json::from_str::<PlayerSession>(&line).ok())
                .collect()
        } else {
            Vec::new()
        };

        sessions.extend(self.open.values().cloned());
        sessions.sort_by_key(|s| std::cmp::Reverse(s.joined_at));
        sessions.truncate(limit);
        Ok(sessions)
    }

    /// Truncates the history file and clears any open sessions being tracked in memory.
    pub fn clear(&mut self) -> Result<(), ServiceError> {
        self.open.clear();
        if self.path.exists() {
            std::fs::write(&self.path, b"")?;
        }
        Ok(())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn player(id: &str, name: &str) -> RconPlayer {
        RconPlayer { id: id.to_string(), name: name.to_string() }
    }

    #[test]
    fn new_players_open_sessions_without_closing_anything() {
        let dir = tempfile::tempdir().unwrap();
        let mut svc = SessionHistoryService::new(dir.path().join("history.jsonl"));

        let closed = svc.record_snapshot(&[player("1", "Alice")]).unwrap();
        assert!(closed.is_empty());
        assert_eq!(svc.list_sessions(10).unwrap().len(), 1);
    }

    #[test]
    fn a_player_missing_from_the_next_snapshot_closes_their_session() {
        let dir = tempfile::tempdir().unwrap();
        let mut svc = SessionHistoryService::new(dir.path().join("history.jsonl"));

        svc.record_snapshot(&[player("1", "Alice"), player("2", "Bob")]).unwrap();
        let closed = svc.record_snapshot(&[player("1", "Alice")]).unwrap();

        assert_eq!(closed.len(), 1);
        assert_eq!(closed[0].player_id, "2");
        assert!(closed[0].left_at.is_some());
    }

    #[test]
    fn closed_sessions_persist_across_a_fresh_service_instance() {
        let dir = tempfile::tempdir().unwrap();
        let path = dir.path().join("history.jsonl");

        {
            let mut svc = SessionHistoryService::new(&path);
            svc.record_snapshot(&[player("1", "Alice")]).unwrap();
            svc.record_snapshot(&[]).unwrap(); // Alice leaves
        }

        let svc = SessionHistoryService::new(&path);
        let sessions = svc.list_sessions(10).unwrap();
        assert_eq!(sessions.len(), 1);
        assert_eq!(sessions[0].player_id, "1");
        assert!(sessions[0].left_at.is_some());
    }

    #[test]
    fn close_all_open_sessions_closes_everything_still_open() {
        let dir = tempfile::tempdir().unwrap();
        let mut svc = SessionHistoryService::new(dir.path().join("history.jsonl"));

        svc.record_snapshot(&[player("1", "Alice"), player("2", "Bob")]).unwrap();
        let closed = svc.close_all_open_sessions().unwrap();

        assert_eq!(closed.len(), 2);
        assert!(svc.list_sessions(10).unwrap().iter().all(|s| s.left_at.is_some()));
    }

    #[test]
    fn clear_truncates_history_and_open_sessions() {
        let dir = tempfile::tempdir().unwrap();
        let mut svc = SessionHistoryService::new(dir.path().join("history.jsonl"));

        svc.record_snapshot(&[player("1", "Alice")]).unwrap();
        svc.record_snapshot(&[]).unwrap();
        assert_eq!(svc.list_sessions(10).unwrap().len(), 1);

        svc.clear().unwrap();
        assert_eq!(svc.list_sessions(10).unwrap().len(), 0);
    }

    #[test]
    fn list_sessions_respects_the_limit() {
        let dir = tempfile::tempdir().unwrap();
        let mut svc = SessionHistoryService::new(dir.path().join("history.jsonl"));

        for i in 0..5 {
            svc.record_snapshot(&[player(&i.to_string(), "P")]).unwrap();
            svc.record_snapshot(&[]).unwrap();
        }

        assert_eq!(svc.list_sessions(3).unwrap().len(), 3);
    }
}
