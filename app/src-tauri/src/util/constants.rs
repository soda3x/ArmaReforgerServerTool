/******************************************************************************
 * File Name:    constants.rs
 * Project:      Longbow / ARST-RUST
 * Description:  Logic constants ported from Utils/Constants.cs. UI tooltip /
 *               message-box strings are intentionally omitted here — those
 *               move to the frontend as UI copy.
 ******************************************************************************/

/// Filename for the scenario's server config. Note: the Rust version drops
/// the leading "\\" from the C# `SERVER_JSON_STR` ("\\server.json") since
/// paths should be built with `PathBuf::join`, not string concatenation.
pub const SERVER_JSON_FILENAME: &str = "server.json";

pub const NO_BACKEND_SCENARIO_LOADER_MOD_ID: &str = "6324F7124A9768FB";

pub const SUPPORTED_PLATFORM_PC: &str = "PLATFORM_PC";
pub const SUPPORTED_PLATFORM_XBOX: &str = "PLATFORM_XBL";
pub const SUPPORTED_PLATFORM_PSN: &str = "PLATFORM_PSN";
