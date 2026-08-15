// Typed API client wrapping every `#[tauri::command]` exposed by the Rust backend
// (app/src-tauri). Field names are camelCase to match the backend's
// `#[serde(rename_all = "camelCase")]` structs.

import { invoke } from "@tauri-apps/api/core";


// ---------------------------------------------------------------------------
// Server configuration (server.json)
// ---------------------------------------------------------------------------

export interface A2S {
  address: string;
  port: number;
}

export type RconPermission = "admin" | "monitor";

export interface Rcon {
  address: string;
  port: number;
  password: string;
  permission: RconPermission;
  blacklist: string[];
  whitelist: string[];
  maxClients: number;
}

export interface Mod {
  modId: string;
  name: string;
  version: string;
  required: boolean;
}

export interface Persistence {
  autoSaveInterval: number;
  hiveId: number;
  databases: unknown;
  storages: unknown;
}

export interface GameProperties {
  serverMaxViewDistance: number;
  serverMinGrassDistance: number;
  networkViewDistance: number;
  disableThirdPerson: boolean;
  fastValidation: boolean;
  battlEye: boolean;
  VONDisableUI: boolean;
  VONDisableDirectSpeechUI: boolean;
  VONCanTransmitCrossFaction: boolean;
  persistence: Persistence;
  missionHeader: unknown;
}

export interface Game {
  name: string;
  password: string;
  passwordAdmin: string;
  admins: string[];
  scenarioId: string;
  maxPlayers: number;
  visible: boolean;
  crossPlatform: boolean;
  supportedPlatforms: string[];
  gameProperties: GameProperties;
  mods: Mod[];
  modsRequiredByDefault: boolean;
}

export interface JoinQueue {
  maxSize: number;
}

export interface Operating {
  lobbyPlayerSynchronise: boolean;
  playerSaveTime: number;
  aiLimit: number;
  slotReservationTimeout: number;
  /** null = off, [] = disable ALL navmeshes, [...] = disable specific ones */
  disableNavmeshStreaming: string[] | null;
  disableServerShutdown: boolean;
  disableCrashReporter: boolean;
  disableAI: boolean;
  joinQueue: JoinQueue;
}

export interface Root {
  bindAddress: string;
  bindPort: number;
  publicAddress: string;
  publicPort: number;
  a2s: A2S;
  rcon: Rcon | null;
  game: Game;
  operating: Operating;
}

export interface ServerConfiguration {
  root: Root;
}

export function defaultServerConfiguration(): ServerConfiguration {
  return {
    root: {
      bindAddress: "0.0.0.0",
      bindPort: 2001,
      publicAddress: "",
      publicPort: 2001,
      a2s: { address: "0.0.0.0", port: 17777 },
      rcon: null,
      game: {
        name: "",
        password: "",
        passwordAdmin: "",
        admins: [],
        scenarioId: "",
        maxPlayers: 64,
        visible: true,
        crossPlatform: false,
        supportedPlatforms: [],
        gameProperties: {
          serverMaxViewDistance: 1600,
          serverMinGrassDistance: 50,
          networkViewDistance: 1500,
          disableThirdPerson: false,
          fastValidation: true,
          battlEye: true,
          VONDisableUI: false,
          VONDisableDirectSpeechUI: false,
          VONCanTransmitCrossFaction: false,
          persistence: {
            autoSaveInterval: 10,
            hiveId: 0,
            databases: {},
            storages: {},
          },
          missionHeader: {},
        },
        mods: [],
        modsRequiredByDefault: false,
      },
      operating: {
        lobbyPlayerSynchronise: true,
        playerSaveTime: 120,
        aiLimit: -1,
        slotReservationTimeout: 60,
        disableNavmeshStreaming: null,
        disableServerShutdown: false,
        disableCrashReporter: false,
        disableAI: false,
        joinQueue: { maxSize: 0 },
      },
    },
  };
}

export function defaultRcon(): Rcon {
  return {
    address: "",
    port: 19999,
    password: "",
    permission: "monitor",
    blacklist: [],
    whitelist: [],
    maxClients: 16,
  };
}

export async function getServerConfiguration(): Promise<ServerConfiguration> {
  return invoke<ServerConfiguration>("get_server_configuration");
}

export async function setServerConfiguration(configuration: ServerConfiguration): Promise<void> {
  return invoke<void>("set_server_configuration", { configuration });
}

export async function saveServerConfiguration(): Promise<void> {
  return invoke<void>("save_server_configuration");
}

export async function loadServerConfigurationFromPath(path: string): Promise<ServerConfiguration> {
  return invoke<ServerConfiguration>("load_server_configuration_from_path", { path });
}

export async function saveServerConfigurationToPath(path: string): Promise<void> {
  return invoke<void>("save_server_configuration_to_path", { path });
}

// ---------------------------------------------------------------------------
// Mods
// ---------------------------------------------------------------------------

export interface ModLists {
  available: Mod[];
  enabled: Mod[];
}

export async function getModLists(): Promise<ModLists> {
  return invoke<ModLists>("get_mod_lists");
}

export async function addMod(m: Mod): Promise<ModLists> {
  return invoke<ModLists>("add_mod", { m });
}

export async function removeMod(m: Mod): Promise<ModLists> {
  return invoke<ModLists>("remove_mod", { m });
}

export async function enableMod(m: Mod): Promise<ModLists> {
  return invoke<ModLists>("enable_mod", { m });
}

export async function disableMod(m: Mod): Promise<ModLists> {
  return invoke<ModLists>("disable_mod", { m });
}

export async function enableAllMods(): Promise<ModLists> {
  return invoke<ModLists>("enable_all_mods");
}

export async function disableAllMods(): Promise<ModLists> {
  return invoke<ModLists>("disable_all_mods");
}

export async function moveEnabledMod(m: Mod, delta: number): Promise<ModLists> {
  return invoke<ModLists>("move_enabled_mod", { m, delta });
}

/** Edits an existing mod in place, preserving whether it's enabled and its load-order slot. */
export async function updateMod(original: Mod, updated: Mod): Promise<ModLists> {
  return invoke<ModLists>("update_mod", { original, updated });
}

export async function exportModsListToPath(path: string): Promise<void> {
  return invoke<void>("export_mods_list_to_path", { path });
}

export async function importModsListFromPath(path: string): Promise<ModLists> {
  return invoke<ModLists>("import_mods_list_from_path", { path });
}

// ---------------------------------------------------------------------------
// Process / server lifecycle
// ---------------------------------------------------------------------------

export interface LaunchArgument {
  key: string;
  /** Empty string means "switch, no value". */
  value: string;
}

export interface LaunchArguments {
  profile: LaunchArgument | null;
  addonsDir: LaunchArgument | null;
  logStats: LaunchArgument | null;
  maxFps: LaunchArgument | null;
  bindPort: LaunchArgument | null;
  nds: LaunchArgument | null;
  nwkResolution: LaunchArgument | null;
  staggeringBudget: LaunchArgument | null;
  streamingBudget: LaunchArgument | null;
  streamsDelta: LaunchArgument | null;
  loadSessionSave: LaunchArgument | null;
  logLevel: LaunchArgument | null;
  autoReload: LaunchArgument | null;
  rplTimeoutMs: LaunchArgument | null;
  freezeCheck: LaunchArgument | null;
  freezeCheckMode: LaunchArgument | null;
  addonsRepair: LaunchArgument | null;
  autoShutdown: LaunchArgument | null;
  logVoting: LaunchArgument | null;
  aiPartialSim: LaunchArgument | null;
  createDb: LaunchArgument | null;
  debugger: LaunchArgument | null;
  debuggerPort: LaunchArgument | null;
  disableShadersBuild: LaunchArgument | null;
  generateShaders: LaunchArgument | null;
  rplEncodeAsLongJobs: LaunchArgument | null;
  jobSysShortWorkerCount: LaunchArgument | null;
  jobSysLongWorkerCount: LaunchArgument | null;
  forceDisableNightGrain: LaunchArgument | null;
  config: LaunchArgument | null;
}

export function emptyLaunchArguments(): LaunchArguments {
  return {
    profile: null,
    addonsDir: null,
    logStats: null,
    maxFps: null,
    bindPort: null,
    nds: null,
    nwkResolution: null,
    staggeringBudget: null,
    streamingBudget: null,
    streamsDelta: null,
    loadSessionSave: null,
    logLevel: null,
    autoReload: null,
    rplTimeoutMs: null,
    freezeCheck: null,
    freezeCheckMode: null,
    addonsRepair: null,
    autoShutdown: null,
    logVoting: null,
    aiPartialSim: null,
    createDb: null,
    debugger: null,
    debuggerPort: null,
    disableShadersBuild: null,
    generateShaders: null,
    rplEncodeAsLongJobs: null,
    jobSysShortWorkerCount: null,
    jobSysLongWorkerCount: null,
    forceDisableNightGrain: null,
    config: null,
  };
}

export type ServerTarget = { kind: "windows" } | { kind: "wsl"; distro: string | null };

export async function isServerStarted(): Promise<boolean> {
  return invoke<boolean>("is_server_started");
}

export async function startStopServer(
  launchArguments: LaunchArguments,
  serverTarget: ServerTarget,
): Promise<void> {
  return invoke<void>("start_stop_server", { launchArguments, serverTarget });
}

export async function killServer(): Promise<void> {
  return invoke<void>("kill_server");
}

export async function buildLaunchArgumentsPreview(
  launchArguments: LaunchArguments,
  serverTarget: ServerTarget,
): Promise<string> {
  return invoke<string>("build_launch_arguments_preview", { launchArguments, serverTarget });
}

/** Whether `wsl.exe` is available with at least one distribution installed. */
export async function isWslAvailable(): Promise<boolean> {
  return invoke<boolean>("is_wsl_available");
}

/** Installed WSL distribution names, for populating the WSL distro picker. */
export async function listWslDistros(): Promise<string[]> {
  return invoke<string[]>("list_wsl_distros");
}

// ---------------------------------------------------------------------------
// Network
// ---------------------------------------------------------------------------

export async function getUseUpnp(): Promise<boolean> {
  return invoke<boolean>("get_use_upnp");
}

export async function setUseUpnp(enabled: boolean): Promise<void> {
  return invoke<void>("set_use_upnp", { enabled });
}

// ---------------------------------------------------------------------------
// Files / install directory / SteamCMD / saves
// ---------------------------------------------------------------------------

export interface Scenario {
  name: string;
  path: string;
}

export async function getInstallDir(): Promise<string | null> {
  return invoke<string | null>("get_install_dir");
}

export async function isSteamCmdInstalled(): Promise<boolean> {
  return invoke<boolean>("is_steamcmd_installed");
}

export async function locateServerFiles(path: string): Promise<void> {
  return invoke<void>("locate_server_files", { path });
}

export async function setInstallDir(path: string): Promise<void> {
  return invoke<void>("set_install_dir", { path });
}

export async function downloadSteamCmd(): Promise<void> {
  return invoke<void>("download_steam_cmd");
}

export async function deleteServerFiles(): Promise<void> {
  return invoke<void>("delete_server_files");
}

export async function getSavedGames(): Promise<Record<string, string>> {
  return invoke<Record<string, string>>("get_saved_games");
}

export async function renameSave(oldName: string, newName: string): Promise<string> {
  return invoke<string>("rename_save", { oldName, newName });
}

export async function checkForUpdates(): Promise<string | null> {
  return invoke<string | null>("check_for_updates");
}

export async function getDefaultScenarios(): Promise<Scenario[]> {
  return invoke<Scenario[]>("get_default_scenarios");
}

/** ISO country code for an Arma ping-site name, or null if unrecognised. */
export async function pingSiteCountryCode(pingSite: string): Promise<string | null> {
  return invoke<string | null>("ping_site_country_code", { pingSite });
}

/** Converts an ISO 3166-1 alpha-2 code into its regional-indicator flag emoji. */
export function countryCodeToFlagEmoji(code: string): string {
  if (code.length !== 2) return "";
  return String.fromCodePoint(
    ...code
      .toUpperCase()
      .split("")
      .map((c) => 0x1f1e6 + (c.charCodeAt(0) - 65)),
  );
}

// ---------------------------------------------------------------------------
// Workshop catalog (browse/search the live Arma Workshop)
// ---------------------------------------------------------------------------

export interface WorkshopAssetSummary {
  id: string;
  name: string;
  summary: string;
  averageRating: number;
  ratingCount: number;
  subscriberCount: number;
  currentVersionNumber: string;
  currentVersionSize: number;
  thumbnailUrl: string | null;
  authorUsername: string;
  tags: string[];
}

export interface WorkshopDependency {
  id: string;
  name: string;
  totalFileSize: number;
}

export interface WorkshopAssetDetail {
  id: string;
  name: string;
  summary: string;
  description: string;
  license: string | null;
  averageRating: number;
  ratingCount: number;
  subscriberCount: number;
  currentVersionNumber: string;
  currentVersionSize: number;
  previewUrls: string[];
  authorUsername: string;
  tags: string[];
  dependencies: WorkshopDependency[];
}

export interface WorkshopSearchResult {
  count: number;
  rows: WorkshopAssetSummary[];
}

export async function searchWorkshopMods(
  query: string | null,
  page: number,
  sort: string | null,
): Promise<WorkshopSearchResult> {
  return invoke<WorkshopSearchResult>("search_workshop_mods", { query, page, sort });
}

export async function getWorkshopModDetails(modId: string): Promise<WorkshopAssetDetail> {
  return invoke<WorkshopAssetDetail>("get_workshop_mod_details", { modId });
}

// ---------------------------------------------------------------------------
// Advanced settings & config flags
// ---------------------------------------------------------------------------

export type AdvancedSettingValue = number | string | boolean | null | undefined;

export interface AdvancedSetting {
  name: string;
  value?: AdvancedSettingValue;
  enabled: boolean;
}

export type LogLevel = "normal" | "warning" | "error" | "fatal";

export const LOG_LEVELS: LogLevel[] = ["normal", "warning", "error", "fatal"];

export interface ConfigFlags {
  autoRestartOnCrash: boolean;
  useExperimentalServer: boolean;
  noBackend: boolean;
  usingSave: boolean;
  save: string | null;
  logLevel: LogLevel;
}

export async function getAdvancedSettings(): Promise<Record<string, AdvancedSetting>> {
  return invoke<Record<string, AdvancedSetting>>("get_advanced_settings");
}

export async function setAdvancedSetting(setting: AdvancedSetting): Promise<void> {
  return invoke<void>("set_advanced_setting", { setting });
}

export async function getConfigFlags(): Promise<ConfigFlags> {
  return invoke<ConfigFlags>("get_config_flags");
}

export async function setConfigFlags(flags: ConfigFlags): Promise<void> {
  return invoke<void>("set_config_flags", { flags });
}

// ---------------------------------------------------------------------------
// Events
// ---------------------------------------------------------------------------

export type ServerButtonIcon = "play" | "stop";

export interface ServerStatus {
  lastFps: number;
  lastMemKb: number;
  lastPlayerCount: number;
  lastPingSite: string;
  lastJoinCode: string;
  lastRconIp: string;
  lastRconPort: number;
  lastIp: string;
  lastPort: number;
  serverOnline: boolean;
  lastUpdate: string;
}

export type ProcessEvent =
  | { type: "log"; line: string }
  | {
      type: "guiUpdate";
      buttonIcon: ServerButtonIcon;
      enableServerFields: boolean;
      serverRunningLabelText: string;
      startServerBtnEnabled: boolean;
    }
  | ({ type: "status" } & ServerStatus);

export const SERVER_EVENT = "server-event";

/**
 * Maps an advanced-setting name (as stored in `SavedState.advancedSettings`, keyed exactly as
 * in the C# original) to the field name on `LaunchArguments`. The CLI flag text itself (the
 * `key` of the resulting `LaunchArgument`) is always the setting's own name unchanged.
 *
 * Only settings that aren't CLI flags at all are omitted: `useUpnp`, `keepServerUpdated`,
 * `useExperimental`, `autoRestartOnCrash` and `noBackend` are tool-level toggles handled via
 * ConfigFlags / NetworkService. Note `bindPort` IS a real flag (the "Override Port" setting) —
 * the backend also keys its no-backend port fallback off whether this one is set.
 *
 * The mandatory arguments (`config`, `profile`, `addonsDir`, `logStats`, `logLevel`) are NOT
 * built here: the backend injects them during `build_start_context`, because only it knows the
 * resolved install directory they're derived from.
 */
export const SETTING_NAME_TO_LAUNCH_ARG_FIELD: Record<string, keyof LaunchArguments> = {
  maxFPS: "maxFps",
  bindPort: "bindPort",
  addonsRepair: "addonsRepair",
  loadSessionSave: "loadSessionSave",
  autoreload: "autoReload",
  autoShutdown: "autoShutdown",
  logVoting: "logVoting",
  nds: "nds",
  nwkResolution: "nwkResolution",
  staggeringBudget: "staggeringBudget",
  streamingBudget: "streamingBudget",
  streamsDelta: "streamsDelta",
  "rpl-timeout-ms": "rplTimeoutMs",
  aiPartialSim: "aiPartialSim",
  createDB: "createDb",
  debugger: "debugger",
  debuggerPort: "debuggerPort",
  disableShadersBuild: "disableShadersBuild",
  generateShaders: "generateShaders",
  rplEncodeAsLongJobs: "rplEncodeAsLongJobs",
  jobsysShortWorkerCount: "jobSysShortWorkerCount",
  jobsysLongWorkerCount: "jobSysLongWorkerCount",
  freezeCheck: "freezeCheck",
  freezeCheckMode: "freezeCheckMode",
  forceDisableNightGrain: "forceDisableNightGrain",
};
