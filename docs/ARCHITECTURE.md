# Architecture

Scope: this document covers only what is actually implemented in this repository — the Phase 1A
mod validator / server-launch tool (`ReforgerServerApp` namespace, `ArmaReforgerServerTool`
project). No Phase 1B/2/roadmap content belongs here.

## Module breakdown

### Forms (`ArmaReforgerServerTool/Forms/`)
WinForms UI. `Main.cs` is the primary window and the largest file in the project — it owns the
mod list UI, server parameter panels, FPS/memory charts, and wires up most of the button click
handlers that drive the flows below. Supporting dialogs: `AddModDialog`, `ModValidationResultDialog`,
`ScenarioSelector`, `SaveSelector`, `RenameForm`, `TextInputForm`, `AboutBox`, `ListForm`.

### Components (`ArmaReforgerServerTool/Components/`)
Custom WinForms controls used to render server parameters generically — `ServerParameter*` /
`AdvancedServerParameter*` families (Bool, Numeric, String, Enumerated, Schedule, Time, List).
These are databound to the `Models` below via `ConfigurationManager`.

### Managers (`ArmaReforgerServerTool/Managers/`)
Singletons (`GetInstance()` pattern) that hold application state and orchestrate behavior:

- `ConfigurationManager` — owns the current `ServerConfiguration`, the available/enabled mod
  lists (`BindingList<Mod>`), and the server-parameter dictionaries the UI binds to.
- `ModValidationService` — the validator (see Flow 1 below).
- `ModDependencyManager` — the Steam Workshop-backed dependency resolver/auto-fixer (Flow 2/3).
- `FileIOManager` — file dialogs, load/save/import of mod lists and configs, update checks,
  VC++ Redistributable check.
- `ProcessManager` — starts/stops/monitors the actual Arma Reforger server process, SteamCMD
  update runs.
- `NetworkManager` — UPnP port mapping (via `Open.Nat`), network-facing helpers.
- `RconManager` — sends BattlEye RCON broadcast messages to the running server over UDP.
- `SavedStateManager` — persists/restores UI/advanced-setting state (`state.json`).
- `SitrepConfigService` (in `Managers/`) — **unused/dead code**, see
  [TROUBLESHOOTING.md](./TROUBLESHOOTING.md).
- `ToolPropertiesManager` — loads `properties.json` (log level, update-check URL, default
  scenario list, etc.).

### Models (`ArmaReforgerServerTool/Models/`)
Plain data classes: `Mod`, `ModDependency`, `ServerConfiguration` (and its nested `Rcon`/`A2S`/etc
types), `ValidationResult`, `ValidationError`, `Scenario`, `SavedState`, `ToolProperties`,
`AdvancedSetting`, `LaunchArgument`.

### Services (`ArmaReforgerServerTool/Services/`)
Contains a second `SitrepConfigService` class (different namespace, different implementation
from the one in `Managers/`) — also unused/dead code. See TROUBLESHOOTING.md.

### Utils (`ArmaReforgerServerTool/Utils/`)
Static helpers: `JsonUtils` (custom `JsonConverter`s for the config/mod model types),
`Utilities` (misc formatting/enum/messagebox helpers), `Constants` (UI strings/tooltips),
`FlagUtils`, `ServerStatusParser` (parses server process stdout/log lines for status),
`ModMetadataSource` (hardcoded metadata table for well-known mods, with a Steam Workshop
fallback), `SteamWorkshopMetadataProvider` (scrapes `steamcommunity.com` pages + caches results
locally under `%AppData%/Longbow/ModMetadata.json`).

## Key flows

### 1. Mod validation (`ModValidationService.ValidateMods`)
Runs as a 4-pass algorithm over the enabled mod list:

1. **Presence check** — for each mod, look up its known dependencies
   (`ModMetadataSource.GetDependencies`) and flag any required dependency that isn't in the
   enabled list as a `FATAL` error (optional/missing deps become `INFO`).
2. **Version constraints** — for dependencies that *are* present, compare the installed version
   against the dependency's declared min/max version window; mismatches become `WARNING`s.
3. **Circular dependency detection** — DFS over the dependency graph (`HasCircularDependency`);
   any cycle is `FATAL`.
4. **Topological sort** (only runs if there were no fatal errors) — Kahn's algorithm produces the
   final load order (`result.SortedMods`).

This runs off the UI thread — call sites in `Main.cs` (`Main_Load`, `CheckModsBtnPressed`) wrap it
in `Task.Run(...)` specifically to avoid freezing the form, since the underlying dependency
lookups can block on `.Result` (see Fragile points below).

### 2. Auto-fix (`Main.cs: ApplyAutoFixes`, driven by `ModValidationService` results)
When validation fails, the UI walks the `FATAL` errors and applies automatic fixes — primarily
adding missing required-dependency mods to the enabled list — then re-runs validation to confirm
the fix worked before re-enabling the Start button.

### 3. Steam Workshop dependency resolution (`ModDependencyManager.ResolveDependencies`)
A second, independent dependency-resolution path (distinct from `ModValidationService`):

1. **BFS over live workshop pages** — for every enabled mod, `LoadAndParsePage` fetches
   `{armaWorkshopUrl}/{modId}` (see `properties.json`) via `HtmlAgilityPack`, and parses the
   `__NEXT_DATA__` JSON blob that the Bohemia Next.js-based workshop site embeds in the page.
   Declared dependencies not already in the enabled list are auto-added and queued for their own
   page fetch, so the crawl is transitive.
2. **Topological sort (DFS)** over the resulting graph, producing final load order, plus warnings
   for anything that couldn't be fetched or that forms a cycle.

This is the mechanism behind "real mod metadata" claimed in the README — it depends entirely on
scraping an internal, versionless JSON blob from a third-party website (see Fragile points).

`SteamWorkshopMetadataProvider` is a related but separate scraper that queries
`steamcommunity.com/sharedfiles/filedetails` pages directly (regex-matched HTML) as the fallback
path inside `ModMetadataSource.GetDependenciesAsync` when a mod isn't in the hardcoded table.

### 4. Server lifecycle (`ProcessManager`, `NetworkManager`, `RconManager`)
`ProcessManager` launches/monitors the dedicated server executable and drives SteamCMD update
runs on a background worker; `NetworkManager` optionally maps ports via UPnP; `RconManager`
sends BattlEye RCON `say` broadcasts over UDP when RCON is configured (silently no-ops otherwise).

## Fragile points

- **Two independent, unscoped scraping paths for Steam/Workshop metadata**
  (`ModDependencyManager` parsing `__NEXT_DATA__`, `SteamWorkshopMetadataProvider` regex-matching
  HTML class names on `steamcommunity.com`). Neither is backed by a public, versioned API —
  either can silently break the moment Bohemia or Valve change their page markup/JSON shape, with
  no contract or version check to detect the break other than "dependencies never resolve."
- **Sync-over-async wrappers** — `ModMetadataSource.GetDependencies`/`GetModVersion` block on
  `.Result` over their async counterparts. Currently every caller in `ModValidationService` is
  itself invoked from inside `Task.Run(...)` (no captured WinForms `SynchronizationContext`), so
  this doesn't currently deadlock, but it's a latent UI-thread-deadlock hazard if a future caller
  invokes these sync wrappers directly from an event handler without the `Task.Run` wrapper.
- **No duplicate-mod-ID guard** anywhere mods enter the enabled list (`AddModDialog`,
  `FileIOManager.LoadModsListFromFile` JSON import, save/load). `ModValidationService.ValidateMods`
  builds its working dictionary with `ToDictionary(m => m.modId...)`, which throws on a duplicate
  key — see TROUBLESHOOTING.md.
- **Dead/duplicate `SitrepConfigService`** — two differently-implemented classes with the same
  name in `Managers/` and `Services/`, neither referenced anywhere else in the codebase. Anyone
  picking this up to "finish the Sitrep integration" needs to pick one and delete the other first.
- **`build_output.txt` at the repo root** is a captured build log, not something regenerated by
  CI on every change — treat it as a point-in-time snapshot, not a live signal. See
  TROUBLESHOOTING.md for a case where it disagreed with the current source.
