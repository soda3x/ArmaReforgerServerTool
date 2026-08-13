/******************************************************************************
 * File Name:    network_service.rs
 * Project:      Longbow / ARST-RUST
 * Description:  Port of Managers/NetworkManager.cs. Manages UPnP port
 *               mapping for the server.
 *
 *               No singleton pattern (per the Rust port's architecture
 *               decision) — construct explicitly via `NetworkService::new`
 *               and let the caller own/share the instance (e.g. wrapped in
 *               `tauri::State<Mutex<NetworkService>>` at the application
 *               layer). No GUI/dialog calls here — errors are returned for
 *               the frontend/commands layer to handle.
 *
 *               Intentional bug fix vs the C# original: the original only
 *               ever mapped TCP even though the actual game traffic (bind
 *               port, public port, A2S port, RCON port) is UDP. This port
 *               maps UDP for all of these.
 ******************************************************************************/

use std::net::{IpAddr, SocketAddr, UdpSocket};

use igd_next::aio::tokio::search_gateway;
use igd_next::{PortMappingProtocol, SearchOptions};

use crate::models::ServerConfiguration;

use super::error::ServiceError;

/// Lease duration of `0` means "infinite" per the `igd_next` API, matching
/// the C# original's choice of an infinite/permanent lease.
const INFINITE_LEASE: u32 = 0;

/// A single port to map/unmap via UPnP.
pub struct PortMapping {
    /// Informational label only (used in the mapping description) — the
    /// actual mapping is keyed by the local port.
    pub address: String,
    pub port: u16,
}

/// Manages the application's UPnP networking logic.
pub struct NetworkService {
    pub use_upnp: bool,
}

impl NetworkService {
    pub fn new(use_upnp: bool) -> Self {
        Self { use_upnp }
    }

    /// Discovers a UPnP gateway and maps every port in `mappings` as UDP
    /// with an infinite lease. No-ops immediately if `!self.use_upnp`.
    pub async fn configure_port_mappings(&self, mappings: &[PortMapping]) -> Result<(), ServiceError> {
        if !self.use_upnp || mappings.is_empty() {
            return Ok(());
        }

        let gateway = search_gateway(SearchOptions::default())
            .await
            .map_err(|e| ServiceError::Other(format!("Failed to discover UPnP gateway: {e}")))?;

        let local_ip = local_ip_addr()
            .map_err(|e| ServiceError::Other(format!("Failed to determine local IP address: {e}")))?;

        for m in mappings {
            let local_addr = SocketAddr::new(local_ip, m.port);
            let description = format!("Longbow mapping for {}:{}", m.address, m.port);

            gateway
                .add_port(
                    PortMappingProtocol::UDP,
                    m.port,
                    local_addr,
                    INFINITE_LEASE,
                    &description,
                )
                .await
                .map_err(|e| {
                    ServiceError::Other(format!(
                        "Failed to map UPnP port {}:{} - {e}",
                        m.address, m.port
                    ))
                })?;
        }

        Ok(())
    }

    /// Removes the same port mappings previously configured via
    /// [`Self::configure_port_mappings`]. No-ops if `!self.use_upnp`.
    pub async fn remove_port_mappings(&self, mappings: &[PortMapping]) -> Result<(), ServiceError> {
        if !self.use_upnp || mappings.is_empty() {
            return Ok(());
        }

        let gateway = search_gateway(SearchOptions::default())
            .await
            .map_err(|e| ServiceError::Other(format!("Failed to discover UPnP gateway: {e}")))?;

        for m in mappings {
            gateway
                .remove_port(PortMappingProtocol::UDP, m.port)
                .await
                .map_err(|e| {
                    ServiceError::Other(format!(
                        "Failed to remove UPnP mapping for {}:{} - {e}",
                        m.address, m.port
                    ))
                })?;
        }

        Ok(())
    }
}

/// Determines this machine's local (LAN) IP address by opening a UDP socket
/// and "connecting" it to a well-known external address — no packets are
/// actually sent, this just forces the OS to pick a local route/address.
fn local_ip_addr() -> std::io::Result<IpAddr> {
    let socket = UdpSocket::bind("0.0.0.0:0")?;
    socket.connect("8.8.8.8:80")?;
    Ok(socket.local_addr()?.ip())
}

/// Port of `Utilities.GetPortMappingsFromServerConfig()`: builds the mapping
/// list from a `ServerConfiguration` — bind port, public port, A2S port, and
/// (only if `root.rcon` is present) the RCON port. Each entry is included
/// whenever its associated address string is non-empty (`bind_address`
/// defaults to `"0.0.0.0"` so it's virtually always included).
pub fn port_mappings_from_config(config: &ServerConfiguration) -> Vec<PortMapping> {
    let root = &config.root;
    let mut mappings = Vec::new();

    if !root.bind_address.is_empty() {
        mappings.push(PortMapping {
            address: root.bind_address.clone(),
            port: root.bind_port,
        });
    }

    if !root.public_address.is_empty() {
        mappings.push(PortMapping {
            address: root.public_address.clone(),
            port: root.public_port,
        });
    }

    if !root.a2s.address.is_empty() {
        mappings.push(PortMapping {
            address: root.a2s.address.clone(),
            port: root.a2s.port,
        });
    }

    if let Some(rcon) = &root.rcon {
        if !rcon.address.is_empty() {
            mappings.push(PortMapping {
                address: rcon.address.clone(),
                port: rcon.port,
            });
        }
    }

    mappings
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn port_mappings_from_config_includes_rcon_when_present() {
        let mut config = ServerConfiguration::default();
        config.root.bind_address = "0.0.0.0".to_string();
        config.root.bind_port = 2001;
        config.root.public_address = "1.2.3.4".to_string();
        config.root.public_port = 2001;
        config.root.a2s.address = "0.0.0.0".to_string();
        config.root.a2s.port = 17777;
        config.root.rcon = Some(crate::models::Rcon {
            address: "0.0.0.0".to_string(),
            port: 19999,
            ..crate::models::Rcon::default()
        });

        let mappings = port_mappings_from_config(&config);

        assert_eq!(mappings.len(), 4);
        assert!(mappings.iter().any(|m| m.port == 2001 && m.address == "0.0.0.0"));
        assert!(mappings.iter().any(|m| m.port == 2001 && m.address == "1.2.3.4"));
        assert!(mappings.iter().any(|m| m.port == 17777));
        assert!(mappings.iter().any(|m| m.port == 19999));
    }

    #[test]
    fn port_mappings_from_config_excludes_rcon_when_absent() {
        let mut config = ServerConfiguration::default();
        config.root.rcon = None;

        let mappings = port_mappings_from_config(&config);

        assert!(mappings.iter().all(|m| m.port != 19999));
        // bind/public/a2s still present (bind defaults non-empty, public
        // defaults empty so it is excluded, a2s defaults non-empty).
        assert!(mappings.iter().any(|m| m.address == "0.0.0.0"));
    }

    #[tokio::test]
    async fn configure_port_mappings_noops_when_upnp_disabled() {
        let service = NetworkService::new(false);
        let mappings = vec![PortMapping {
            address: "0.0.0.0".to_string(),
            port: 2001,
        }];

        let result = service.configure_port_mappings(&mappings).await;
        assert!(result.is_ok());
    }

    #[tokio::test]
    async fn remove_port_mappings_noops_when_upnp_disabled() {
        let service = NetworkService::new(false);
        let mappings = vec![PortMapping {
            address: "0.0.0.0".to_string(),
            port: 2001,
        }];

        let result = service.remove_port_mappings(&mappings).await;
        assert!(result.is_ok());
    }
}
