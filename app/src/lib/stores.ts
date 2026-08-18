// Central Svelte stores for app-wide state. Populated at startup by routes/+page.svelte and
// kept live by the `server-event` listener.

import { writable, derived } from "svelte/store";
import {
  defaultServerConfiguration,
  type ServerConfiguration,
  type Mod,
  type AdvancedSetting,
  type ConfigFlags,
  type ServerStatus,
  type ServerButtonIcon,
  type SteamCmdProgress,
} from "./api";

export const serverConfiguration = writable<ServerConfiguration>(defaultServerConfiguration());

export const availableMods = writable<Mod[]>([]);
export const enabledMods = writable<Mod[]>([]);

export const advancedSettings = writable<Record<string, AdvancedSetting>>({});

export const configFlags = writable<ConfigFlags>({
  autoRestartOnCrash: false,
  useExperimentalServer: false,
  noBackend: false,
  usingSave: false,
  save: null,
  logLevel: "normal",
});

export const useUpnp = writable<boolean>(true);

export const installDir = writable<string | null>(null);
export const steamCmdInstalled = writable<boolean>(false);

export type ServerTargetKind = "windows" | "wsl";
export const serverTargetKind = writable<ServerTargetKind>("windows");
export const wslDistro = writable<string | null>(null);

// --- Server running / process state -----------------------------------------------------

export const serverRunning = writable<boolean>(false);
export const serverBusy = writable<boolean>(false); // true while a start/stop call is in-flight
export const buttonIcon = writable<ServerButtonIcon>("play");
export const serverRunningLabel = writable<string>("Server stopped");
export const startServerBtnEnabled = writable<boolean>(true);
export const steamCmdProgress = writable<SteamCmdProgress | null>(null);

// --- Logs --------------------------------------------------------------------------------

const MAX_LOG_LINES = 2000;
export const logLines = writable<string[]>([]);

export function appendLogLine(line: string): void {
  logLines.update((lines) => {
        const next = lines.length >= MAX_LOG_LINES ? lines.slice(lines.length - MAX_LOG_LINES + 1) : lines.slice();
    next.push(line);
    return next;
  });
}

export function clearLogLines(): void {
  logLines.set([]);
}

// --- Status / charts -----------------------------------------------------------------------

export const latestStatus = writable<ServerStatus | null>(null);

const MAX_HISTORY_POINTS = 60;

export interface HistoryPoint {
  t: number;
  v: number;
}

export const fpsHistory = writable<HistoryPoint[]>([]);
export const memHistory = writable<HistoryPoint[]>([]);

export function pushStatus(status: ServerStatus): void {
  latestStatus.set(status);
  const t = Date.now();

  fpsHistory.update((points) => {
    const next = points.length >= MAX_HISTORY_POINTS ? points.slice(points.length - MAX_HISTORY_POINTS + 1) : points.slice();
    next.push({ t, v: status.lastFps });
    return next;
  });

  memHistory.update((points) => {
    const next = points.length >= MAX_HISTORY_POINTS ? points.slice(points.length - MAX_HISTORY_POINTS + 1) : points.slice();
    next.push({ t, v: status.lastMemKb / (1024 * 1024) }); // KB -> GB
    return next;
  });
}

export const memoryGb = derived(latestStatus, ($status) => ($status ? $status.lastMemKb / (1024 * 1024) : 0));
