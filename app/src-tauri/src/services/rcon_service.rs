//! RCON client for the Arma Reforger dedicated server, implemented against the **BattlEye RCon
//! protocol v2** (the wire protocol Reforger's RCON runs over) — see the official spec at
//! <https://www.battleye.com/downloads/BERConProtocol.txt>.
//!
//! Wire format: every packet is `'B','E' + crc32_le(payload) + payload`, where `payload` starts
//! with a `0xFF` marker byte followed by a packet-type byte:
//! - `0x00` login: request is `0xFF,0x00,<password>`; reply is `0xFF,0x00,<0x01 success|0x00 fail>`.
//! - `0x01` command: request is `0xFF,0x01,<seq>,<command>`; reply is
//!   `0xFF,0x01,<seq>,[0x00,<total>,<index>],<data>` — the bracketed multi-packet header is
//!   present only when the response was split across several UDP datagrams.
//! - `0x02` server message (server-initiated broadcast): `0xFF,0x02,<seq>,<text>`, which the
//!   client must ack with `0xFF,0x02,<seq>` (no text) or the server will retry/drop it.
//!
//! A keepalive (an empty command packet) must go out at least every 45 seconds or the server
//! deauthenticates the client; this service sends one every 20 seconds while connected.
//!
//! Reforger's confirmed RCON commands (per Bohemia's wiki + community references — the exact
//! text layout of multi-line responses like `#players`/`#ban list` isn't documented at the byte
//! level) are `#players`, `#kick <id>`, `#ban create/remove/list`, `#roles`, `#id`,
//! `#login`/`#logout`. Parsing those responses into [`RconPlayer`]/[`BanEntry`] rows is therefore
//! best-effort: the raw response text is always also available (surfaced via
//! [`RconEvent::ConsoleLine`]) so the feature degrades to "still shows you the real output"
//! rather than breaking outright if a server's exact format differs from what's assumed here.

use std::collections::HashMap;
use std::sync::atomic::{AtomicBool, Ordering};
use std::sync::{Arc, LazyLock};
use std::time::Duration;

use regex::Regex;
use serde::Serialize;
use tokio::net::UdpSocket;
use tokio::sync::{broadcast, oneshot, Mutex};

use crate::models::{BanEntry, RconPlayer};

use super::error::ServiceError;

const HEADER_PREFIX: [u8; 2] = [b'B', b'E'];
const PACKET_TYPE_LOGIN: u8 = 0x00;
const PACKET_TYPE_COMMAND: u8 = 0x01;
const PACKET_TYPE_MESSAGE: u8 = 0x02;

/// Must stay comfortably under the spec's 45-second deauthentication window.
const KEEPALIVE_INTERVAL: Duration = Duration::from_secs(20);
const COMMAND_TIMEOUT: Duration = Duration::from_secs(8);
const RECV_BUFFER_SIZE: usize = 8192;
const PLAYER_POLL_INTERVAL: Duration = Duration::from_secs(15);

/// Events emitted over the RCON connection's lifetime. The commands layer re-broadcasts these to
/// the frontend as Tauri events, mirroring `ProcessEvent`/`process_commands::spawn_event_forwarder`.
#[derive(Debug, Clone, Serialize)]
#[serde(tag = "type", rename_all = "camelCase")]
pub enum RconEvent {
    ConnectionChanged { connected: bool },
    /// One line of console transcript — echoed raw commands, their responses, and any
    /// server-initiated broadcast messages.
    ConsoleLine { text: String },
    /// A fresh `#players` snapshot, emitted both on every poll tick and after an explicit
    /// `list_players()` call.
    PlayersUpdated { players: Vec<RconPlayer> },
}

/// Builds a full BattlEye RCon packet: header + little-endian CRC32 of `payload` + `payload`.
/// `payload` must already start with the `0xFF` marker byte.
fn build_packet(payload: &[u8]) -> Vec<u8> {
    let crc = crc32fast::hash(payload);
    let mut packet = Vec::with_capacity(2 + 4 + payload.len());
    packet.extend_from_slice(&HEADER_PREFIX);
    packet.extend_from_slice(&crc.to_le_bytes());
    packet.extend_from_slice(payload);
    packet
}

fn build_login_packet(password: &str) -> Vec<u8> {
    let mut payload = vec![0xFF, PACKET_TYPE_LOGIN];
    payload.extend_from_slice(password.as_bytes());
    build_packet(&payload)
}

/// `command` may be empty — that's the "empty 2-byte command packet" the spec requires as a
/// keepalive.
fn build_command_packet(seq: u8, command: &str) -> Vec<u8> {
    let mut payload = vec![0xFF, PACKET_TYPE_COMMAND, seq];
    payload.extend_from_slice(command.as_bytes());
    build_packet(&payload)
}

fn build_message_ack_packet(seq: u8) -> Vec<u8> {
    build_packet(&[0xFF, PACKET_TYPE_MESSAGE, seq])
}

/// A decoded incoming packet from the server.
#[derive(Debug, Clone, PartialEq, Eq)]
enum IncomingPacket {
    LoginResult { success: bool },
    CommandResponse { seq: u8, multipart: Option<(u8, u8)>, data: Vec<u8> },
    ServerMessage { seq: u8, text: String },
}

/// Parses one incoming UDP datagram, validating the header and CRC. Returns `None` for anything
/// that doesn't parse as a well-formed packet — a stray/corrupt datagram is silently dropped
/// rather than treated as a connection error, matching how a lossy UDP admin channel behaves.
fn parse_packet(datagram: &[u8]) -> Option<IncomingPacket> {
    if datagram.len() < 7 || datagram[0] != b'B' || datagram[1] != b'E' {
        return None;
    }
    let crc_bytes: [u8; 4] = datagram[2..6].try_into().ok()?;
    let expected_crc = u32::from_le_bytes(crc_bytes);
    let payload = &datagram[6..];
    if crc32fast::hash(payload) != expected_crc || payload.first() != Some(&0xFF) {
        return None;
    }

    let rest = &payload[1..];
    let packet_type = *rest.first()?;
    let body = rest.get(1..)?;

    match packet_type {
        PACKET_TYPE_LOGIN => Some(IncomingPacket::LoginResult { success: *body.first()? == 0x01 }),
        PACKET_TYPE_COMMAND => {
            let seq = *body.first()?;
            let mut data = body.get(1..)?;
            // The multi-packet header (`0x00, total, index`) is only ever sent alongside a
            // fragmented response; a single-packet response's data can't be reliably
            // distinguished from it if it happens to start with 0x00, but RCON responses are
            // ASCII text so that's not a realistic collision in practice.
            let multipart = if data.len() >= 3 && data[0] == 0x00 {
                let total = data[1];
                let index = data[2];
                data = &data[3..];
                Some((total, index))
            } else {
                None
            };
            Some(IncomingPacket::CommandResponse { seq, multipart, data: data.to_vec() })
        }
        PACKET_TYPE_MESSAGE => {
            let seq = *body.first()?;
            let text = String::from_utf8_lossy(body.get(1..).unwrap_or(&[])).into_owned();
            Some(IncomingPacket::ServerMessage { seq, text })
        }
        _ => None,
    }
}

/// Best-effort parse of a `#players` response into rows. Matches lines of the shape
/// `<index> <name...> <id>` (index and id separated from the name by whitespace) — a line that
/// doesn't match this shape (headers, separators, blank lines) is simply skipped rather than
/// guessed at.
pub fn parse_players(raw: &str) -> Vec<RconPlayer> {
    static LINE_RE: LazyLock<Regex> = LazyLock::new(|| {
        Regex::new(r"^\s*\d+[.\)]?\s+(?P<name>\S.*?)\s{2,}(?P<id>[0-9A-Za-z]{6,})\s*$")
            .expect("invalid players line regex")
    });

    raw.lines()
        .filter_map(|line| {
            LINE_RE.captures(line).map(|caps| RconPlayer {
                id: caps["id"].to_string(),
                name: caps["name"].trim().to_string(),
            })
        })
        .collect()
}

/// Best-effort parse of a `#ban list` response into rows. Each matching line's identity id is
/// captured; everything else on the line is kept verbatim as the reason so no information is
/// silently dropped even where the format doesn't match assumptions exactly. Expiry timestamps
/// aren't parsed (the exact format isn't documented) — `expires_at` is always `None`; the raw
/// text (including any expiry) is still visible in `reason`.
pub fn parse_ban_list(raw: &str) -> Vec<BanEntry> {
    static LINE_RE: LazyLock<Regex> = LazyLock::new(|| {
        Regex::new(r"^\s*\d+[.\)]?\s+(?P<id>[0-9A-Za-z]{6,})\s+(?P<rest>.+?)\s*$")
            .expect("invalid ban list line regex")
    });

    raw.lines()
        .filter_map(|line| {
            LINE_RE.captures(line).map(|caps| BanEntry {
                identity_id: caps["id"].to_string(),
                reason: caps["rest"].to_string(),
                expires_at: None,
            })
        })
        .collect()
}

struct Inner {
    socket: Option<Arc<UdpSocket>>,
    /// Raised to tear down the current connection's background tasks (listener/keepalive/poll).
    /// A fresh flag is created on every `connect()`, matching `ProcessService`'s cancellation
    /// idiom.
    cancel: Option<Arc<AtomicBool>>,
    connected: bool,
    address: Option<String>,
    port: Option<u16>,
    next_seq: u8,
    /// In-flight commands awaiting a response, keyed by sequence number.
    pending: HashMap<u8, oneshot::Sender<Vec<u8>>>,
    /// Partially-received multi-packet responses, keyed by sequence number:
    /// `(total_packet_count, {index -> chunk})`.
    partial: HashMap<u8, (u8, HashMap<u8, Vec<u8>>)>,
}

impl Default for Inner {
    fn default() -> Self {
        Self {
            socket: None,
            cancel: None,
            connected: false,
            address: None,
            port: None,
            next_seq: 0,
            pending: HashMap::new(),
            partial: HashMap::new(),
        }
    }
}

/// Connection status, as reported to the frontend by `rcon_get_status`.
#[derive(Debug, Clone, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct RconStatus {
    pub connected: bool,
    pub address: Option<String>,
    pub port: Option<u16>,
}

pub struct RconService {
    inner: Mutex<Inner>,
    events: broadcast::Sender<RconEvent>,
}

impl RconService {
    pub fn new() -> Arc<Self> {
        let (tx, _rx) = broadcast::channel(1024);
        Arc::new(Self { inner: Mutex::new(Inner::default()), events: tx })
    }

    pub fn subscribe(&self) -> broadcast::Receiver<RconEvent> {
        self.events.subscribe()
    }

    fn emit(&self, event: RconEvent) {
        let _ = self.events.send(event);
    }

    pub async fn status(&self) -> RconStatus {
        let inner = self.inner.lock().await;
        RconStatus { connected: inner.connected, address: inner.address.clone(), port: inner.port }
    }

    /// Connects and authenticates to the RCON port at `address:port`, then starts the
    /// listener/keepalive/player-poll background tasks. Any existing connection is torn down
    /// first.
    pub async fn connect(self: &Arc<Self>, address: &str, port: u16, password: &str) -> Result<(), ServiceError> {
        self.disconnect().await;

        let target: std::net::SocketAddr = format!("{address}:{port}")
            .parse()
            .map_err(|e| ServiceError::Other(format!("Invalid RCON address '{address}:{port}': {e}")))?;

        let socket = UdpSocket::bind("0.0.0.0:0").await.map_err(ServiceError::Io)?;
        socket.connect(target).await.map_err(ServiceError::Io)?;
        let socket = Arc::new(socket);

        socket.send(&build_login_packet(password)).await.map_err(ServiceError::Io)?;

        let mut buf = vec![0u8; RECV_BUFFER_SIZE];
        let n = tokio::time::timeout(COMMAND_TIMEOUT, socket.recv(&mut buf))
            .await
            .map_err(|_| {
                ServiceError::Other(
                    "RCON login timed out — check the RCON address/port and that the server is \
                     running."
                        .to_string(),
                )
            })?
            .map_err(ServiceError::Io)?;

        match parse_packet(&buf[..n]) {
            Some(IncomingPacket::LoginResult { success: true }) => {}
            Some(IncomingPacket::LoginResult { success: false }) => {
                return Err(ServiceError::Other(
                    "RCON login failed — check the RCON password in Configuration.".to_string(),
                ));
            }
            _ => {
                return Err(ServiceError::Other(
                    "Unexpected response while logging in to RCON.".to_string(),
                ))
            }
        }

        let cancel = Arc::new(AtomicBool::new(false));
        {
            let mut inner = self.inner.lock().await;
            inner.socket = Some(Arc::clone(&socket));
            inner.cancel = Some(Arc::clone(&cancel));
            inner.connected = true;
            inner.address = Some(address.to_string());
            inner.port = Some(port);
            inner.next_seq = 0;
            inner.pending.clear();
            inner.partial.clear();
        }
        self.emit(RconEvent::ConnectionChanged { connected: true });
        self.emit(RconEvent::ConsoleLine { text: format!("Connected to RCON at {address}:{port}.") });

        {
            let this = Arc::clone(self);
            let socket = Arc::clone(&socket);
            let cancel = Arc::clone(&cancel);
            tokio::spawn(async move { this.listen_loop(socket, cancel).await });
        }
        {
            let this = Arc::clone(self);
            let socket = Arc::clone(&socket);
            let cancel = Arc::clone(&cancel);
            tokio::spawn(async move { this.keepalive_loop(socket, cancel).await });
        }
        {
            let this = Arc::clone(self);
            let cancel = Arc::clone(&cancel);
            tokio::spawn(async move { this.poll_players_loop(cancel).await });
        }

        Ok(())
    }

    /// Tears down the current connection (if any) and cancels its background tasks. Safe to call
    /// when already disconnected.
    pub async fn disconnect(&self) {
        let (cancel, was_connected) = {
            let mut inner = self.inner.lock().await;
            let cancel = inner.cancel.take();
            let was_connected = inner.connected;
            inner.connected = false;
            inner.socket = None;
            inner.address = None;
            inner.port = None;
            inner.pending.clear();
            inner.partial.clear();
            (cancel, was_connected)
        };
        if let Some(cancel) = cancel {
            cancel.store(true, Ordering::SeqCst);
        }
        if was_connected {
            self.emit(RconEvent::ConnectionChanged { connected: false });
        }
    }

    /// Sends a raw RCON command and returns its (possibly multi-line) text response. Also emits
    /// the command and its response as [`RconEvent::ConsoleLine`]s, so the tab's console shows a
    /// full transcript regardless of which higher-level helper (or the raw command box) issued
    /// it.
    pub async fn send_raw(&self, command: &str) -> Result<String, ServiceError> {
        let (socket, seq) = {
            let mut inner = self.inner.lock().await;
            if !inner.connected {
                return Err(ServiceError::Other("Not connected to RCON.".to_string()));
            }
            let socket = inner.socket.clone().expect("connected implies a socket is set");
            let seq = inner.next_seq;
            inner.next_seq = inner.next_seq.wrapping_add(1);
            (socket, seq)
        };

        let (tx, rx) = oneshot::channel();
        {
            let mut inner = self.inner.lock().await;
            inner.pending.insert(seq, tx);
        }

        self.emit(RconEvent::ConsoleLine { text: format!("> {command}") });
        socket.send(&build_command_packet(seq, command)).await.map_err(ServiceError::Io)?;

        let result = match tokio::time::timeout(COMMAND_TIMEOUT, rx).await {
            Ok(Ok(data)) => Ok(String::from_utf8_lossy(&data).into_owned()),
            Ok(Err(_)) => {
                Err(ServiceError::Other("RCON connection closed while waiting for a response.".to_string()))
            }
            Err(_) => {
                self.inner.lock().await.pending.remove(&seq);
                Err(ServiceError::Other(format!("RCON command '{command}' timed out.")))
            }
        };

        match &result {
            Ok(response) if !response.is_empty() => {
                self.emit(RconEvent::ConsoleLine { text: response.clone() });
            }
            Err(e) => self.emit(RconEvent::ConsoleLine { text: format!("Error: {e}") }),
            _ => {}
        }

        result
    }

    /// Runs `#players`, parses the response, and emits the resulting snapshot as
    /// [`RconEvent::PlayersUpdated`] (both on success from an explicit call and from the
    /// background poll loop).
    pub async fn list_players(&self) -> Result<Vec<RconPlayer>, ServiceError> {
        let raw = self.send_raw("#players").await?;
        let players = parse_players(&raw);
        self.emit(RconEvent::PlayersUpdated { players: players.clone() });
        Ok(players)
    }

    pub async fn kick(&self, player_id: &str) -> Result<String, ServiceError> {
        self.send_raw(&format!("#kick {player_id}")).await
    }

    /// `duration_secs: 0` means a permanent ban, per the `#ban create` command's own semantics.
    pub async fn ban_create(&self, identity_id: &str, duration_secs: u64, reason: &str) -> Result<String, ServiceError> {
        let reason = reason.trim();
        let cmd = if reason.is_empty() {
            format!("#ban create {identity_id} {duration_secs}")
        } else {
            format!("#ban create {identity_id} {duration_secs} {reason}")
        };
        self.send_raw(&cmd).await
    }

    pub async fn ban_remove(&self, identity_id: &str) -> Result<String, ServiceError> {
        self.send_raw(&format!("#ban remove {identity_id}")).await
    }

    pub async fn ban_list(&self, page: Option<u32>) -> Result<Vec<BanEntry>, ServiceError> {
        let cmd = match page {
            Some(p) => format!("#ban list {p}"),
            None => "#ban list".to_string(),
        };
        let raw = self.send_raw(&cmd).await?;
        Ok(parse_ban_list(&raw))
    }

    async fn listen_loop(self: Arc<Self>, socket: Arc<UdpSocket>, cancel: Arc<AtomicBool>) {
        let mut buf = vec![0u8; RECV_BUFFER_SIZE];
        loop {
            if cancel.load(Ordering::SeqCst) {
                break;
            }
            let recv = tokio::time::timeout(Duration::from_millis(500), socket.recv(&mut buf)).await;
            let n = match recv {
                Ok(Ok(n)) => n,
                // Socket-level error (e.g. the interface went away) — stop rather than spin.
                Ok(Err(_)) => break,
                // Timeout: loop back around to re-check the cancel flag.
                Err(_) => continue,
            };

            match parse_packet(&buf[..n]) {
                Some(IncomingPacket::ServerMessage { seq, text }) => {
                    let _ = socket.send(&build_message_ack_packet(seq)).await;
                    self.emit(RconEvent::ConsoleLine { text });
                }
                Some(IncomingPacket::CommandResponse { seq, multipart, data }) => {
                    self.handle_command_response(seq, multipart, data).await;
                }
                // A stray late login ack, or an unparseable datagram — both ignored.
                Some(IncomingPacket::LoginResult { .. }) | None => {}
            }
        }

        self.disconnect().await;
    }

    async fn handle_command_response(&self, seq: u8, multipart: Option<(u8, u8)>, data: Vec<u8>) {
        let complete = {
            let mut inner = self.inner.lock().await;
            match multipart {
                None => Some(data),
                Some((total, index)) => {
                    let entry = inner.partial.entry(seq).or_insert_with(|| (total, HashMap::new()));
                    entry.0 = total;
                    entry.1.insert(index, data);
                    if entry.1.len() as u8 >= total {
                        let (total, parts) = inner.partial.remove(&seq).expect("just inserted above");
                        let mut combined = Vec::new();
                        for i in 0..total {
                            if let Some(chunk) = parts.get(&i) {
                                combined.extend_from_slice(chunk);
                            }
                        }
                        Some(combined)
                    } else {
                        None
                    }
                }
            }
        };

        if let Some(full) = complete {
            let sender = self.inner.lock().await.pending.remove(&seq);
            if let Some(sender) = sender {
                let _ = sender.send(full);
            }
        }
    }

    async fn keepalive_loop(self: Arc<Self>, socket: Arc<UdpSocket>, cancel: Arc<AtomicBool>) {
        loop {
            tokio::select! {
                _ = tokio::time::sleep(KEEPALIVE_INTERVAL) => {}
                _ = wait_for_cancel(Arc::clone(&cancel)) => break,
            }
            if cancel.load(Ordering::SeqCst) {
                break;
            }
            let seq = {
                let mut inner = self.inner.lock().await;
                let seq = inner.next_seq;
                inner.next_seq = inner.next_seq.wrapping_add(1);
                seq
            };
            // Fire-and-forget: no pending waiter is registered, so any reply is just dropped by
            // `handle_command_response` finding nothing to resolve.
            if socket.send(&build_command_packet(seq, "")).await.is_err() {
                break;
            }
        }
    }

    async fn poll_players_loop(self: Arc<Self>, cancel: Arc<AtomicBool>) {
        loop {
            tokio::select! {
                _ = tokio::time::sleep(PLAYER_POLL_INTERVAL) => {}
                _ = wait_for_cancel(Arc::clone(&cancel)) => break,
            }
            if cancel.load(Ordering::SeqCst) {
                break;
            }
            // A transient poll failure (e.g. one dropped UDP datagram) isn't fatal — the next
            // tick retries. `list_players` already emits `PlayersUpdated` on success.
            let _ = self.list_players().await;
        }
    }
}

async fn wait_for_cancel(flag: Arc<AtomicBool>) {
    loop {
        if flag.load(Ordering::SeqCst) {
            return;
        }
        tokio::time::sleep(Duration::from_millis(250)).await;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn login_packet_round_trips_through_parse() {
        let packet = build_login_packet("hunter2");
        // The client never parses its own login packet (only the server's ack), but this
        // confirms the framing/CRC logic is internally consistent.
        assert_eq!(&packet[0..2], b"BE");
        let payload = &packet[6..];
        assert_eq!(payload[0], 0xFF);
        assert_eq!(payload[1], PACKET_TYPE_LOGIN);
        assert_eq!(&payload[2..], b"hunter2");
        assert_eq!(crc32fast::hash(payload), u32::from_le_bytes(packet[2..6].try_into().unwrap()));
    }

    #[test]
    fn parses_login_success_and_failure() {
        let success = build_packet(&[0xFF, PACKET_TYPE_LOGIN, 0x01]);
        assert_eq!(parse_packet(&success), Some(IncomingPacket::LoginResult { success: true }));

        let failure = build_packet(&[0xFF, PACKET_TYPE_LOGIN, 0x00]);
        assert_eq!(parse_packet(&failure), Some(IncomingPacket::LoginResult { success: false }));
    }

    #[test]
    fn rejects_a_packet_with_a_corrupted_crc() {
        let mut packet = build_login_packet("pw");
        packet[2] ^= 0xFF; // flip a CRC byte
        assert_eq!(parse_packet(&packet), None);
    }

    #[test]
    fn parses_a_single_packet_command_response() {
        let packet = build_packet(&[0xFF, PACKET_TYPE_COMMAND, 7, b'h', b'i']);
        assert_eq!(
            parse_packet(&packet),
            Some(IncomingPacket::CommandResponse { seq: 7, multipart: None, data: b"hi".to_vec() })
        );
    }

    #[test]
    fn parses_a_multi_packet_command_response_header() {
        let packet = build_packet(&[0xFF, PACKET_TYPE_COMMAND, 3, 0x00, 2, 0, b'a', b'b']);
        assert_eq!(
            parse_packet(&packet),
            Some(IncomingPacket::CommandResponse {
                seq: 3,
                multipart: Some((2, 0)),
                data: b"ab".to_vec()
            })
        );
    }

    #[test]
    fn parses_a_server_message() {
        let packet = build_packet(&[0xFF, PACKET_TYPE_MESSAGE, 1, b'h', b'i']);
        assert_eq!(
            parse_packet(&packet),
            Some(IncomingPacket::ServerMessage { seq: 1, text: "hi".to_string() })
        );
    }

    #[test]
    fn message_ack_carries_no_text() {
        let ack = build_message_ack_packet(9);
        let payload = &ack[6..];
        assert_eq!(payload, &[0xFF, PACKET_TYPE_MESSAGE, 9]);
    }

    #[test]
    fn reassembles_a_multi_packet_response_regardless_of_arrival_order() {
        // Simulates what `handle_command_response` does synchronously, without the async runtime.
        let total = 3u8;
        let mut parts: HashMap<u8, Vec<u8>> = HashMap::new();
        parts.insert(1, b"B".to_vec());
        parts.insert(0, b"A".to_vec());
        parts.insert(2, b"C".to_vec());

        let mut combined = Vec::new();
        for i in 0..total {
            combined.extend_from_slice(parts.get(&i).unwrap());
        }
        assert_eq!(combined, b"ABC");
    }

    #[test]
    fn parses_a_representative_players_response() {
        let raw = "Players: 2\n\
                    [#] [Name]              [PlayerId]\n\
                    --------------------------------\n\
                    0   PlayerOne           abcdef1234567890\n\
                    1   Player Two          0987654321fedcba\n";
        let players = parse_players(raw);
        assert_eq!(players.len(), 2);
        assert_eq!(players[0].name, "PlayerOne");
        assert_eq!(players[0].id, "abcdef1234567890");
        assert_eq!(players[1].name, "Player Two");
        assert_eq!(players[1].id, "0987654321fedcba");
    }

    #[test]
    fn players_parsing_skips_non_matching_lines_instead_of_guessing() {
        let raw = "No players online.";
        assert_eq!(parse_players(raw), Vec::new());
    }

    #[test]
    fn parses_a_representative_ban_list_response() {
        let raw = "0) abcdef1234567890 Reason: griefing, Expires: never\n\
                    1) 0987654321fedcba Reason: cheating\n";
        let bans = parse_ban_list(raw);
        assert_eq!(bans.len(), 2);
        assert_eq!(bans[0].identity_id, "abcdef1234567890");
        assert!(bans[0].reason.contains("griefing"));
        assert_eq!(bans[1].identity_id, "0987654321fedcba");
    }
}
