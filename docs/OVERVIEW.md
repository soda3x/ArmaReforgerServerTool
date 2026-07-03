# Overview

## What this is

Longbow is a WinForms (.NET 8) desktop tool for managing an **Arma Reforger** dedicated server
("Longbow", the internal/marketing name; the assembly and namespace are still
`ReforgerServerApp`/`Longbow`). This public repository contains **Phase 1A only**: the mod
validator and server-launch tool. Per [`../HANDOFF.md`](../HANDOFF.md), Phase 1A is complete and
shipped; all further (Phase 1B+) work happens in a separate private repository and is out of
scope here.

Core Phase 1A capability, in the project's own words (README.md):

- **Mod Validator** — detects missing dependencies, version conflicts, and circular dependencies
  in a user's enabled-mod list.
- **Auto-fix** — adds missing dependency mods and reorders the list into a valid load order.
- **Steam Workshop integration** — fetches mod metadata (name, version, dependencies) by scraping
  Steam Community and the Arma Reforger workshop web pages, with a hardcoded fallback table for
  well-known mods.
- **Save/load configurations** — persist and switch between server configs.
- **Start button gating** — the "Start Server" button is disabled (red) until validation passes
  (green).
- Server process management (start/stop/monitor), RCON broadcast support, SteamCMD-driven
  server binary updates, and a fairly large server-parameters UI (see
  [PARAMETERS.md](./PARAMETERS.md)).

## Tech stack

- **Language/runtime:** C# / .NET 8 (`net8.0-windows`), nullable reference types enabled,
  implicit usings enabled.
- **UI:** WinForms (`UseWindowsForms=true`), `OutputType=winexe`.
- **Logging:** Serilog, writing to console and a rolling daily file
  (`logs/longbow.log` by default, configurable via `properties.json`).
- **Key third-party packages** (`ArmaReforgerServerTool/Longbow.csproj`):
  - `HtmlAgilityPack` 1.12.4 — HTML parsing for Steam/workshop page scraping.
  - `Open.Nat` 2.1.0 — UPnP port mapping. Note: this package targets .NET Framework, not
    `net8.0-windows`, and is restored with a compatibility shim (`NU1701` warning at build time).
  - `WinForms.DataVisualization` 1.10.0 — the FPS/memory charts in the main form.
  - `FontAwesome.Sharp`, `FlagsISO`, `BatLine.AnimOfDots` — UI icon/decoration libraries.
- **Tests:** `Longbow.Tests` project, xUnit-style `[Fact]` tests (see
  `Longbow.Tests/ModValidationServiceTests.cs`, `Longbow.Tests/PerformanceTest.cs`).

## Build / run / test

From the repo root:

```bash
# Build (Release)
dotnet build -c Release

# Build the whole solution explicitly
dotnet build Longbow.sln -c Release

# Run the tool (Windows only — WinForms/net8.0-windows target)
bin/Release/net8.0-windows/Longbow.exe
# (equivalently: dotnet run --project ArmaReforgerServerTool -c Release)

# Run tests
dotnet test Longbow.Tests
```

Requirements: .NET 8 SDK (Runtime is enough to run a published build), Windows (the app targets
`net8.0-windows` and uses WinForms — it will not build/run on Linux/macOS).

There is a top-level `Longbow.sln` (referencing both projects) and a second,
narrower `ArmaReforgerServerTool/ArmaReforgerServerTool.sln`. Prefer the top-level
`Longbow.sln` so the test project is included.

## Directory map

```
ArmaReforgerServerTool/        Main WinForms application (namespace ReforgerServerApp)
  Forms/                       Windows Forms UI (Main, dialogs, selectors)
  Components/                  Custom WinForms controls (server parameter widgets)
  Managers/                    Singletons orchestrating app behavior (config, process, network,
                                mod validation/dependency resolution, RCON, saved state, etc.)
  Models/                      POCOs — Mod, ModDependency, ServerConfiguration, ValidationResult...
  Services/                    SitrepConfigService (duplicated — see ARCHITECTURE.md)
  Utils/                       Static helpers (JSON parsing, Steam Workshop scraping, constants)
  Design/                      App color/spacing/typography constants + custom controls
  Properties/                  Generated resources
  Resources/                   Icons, bundled scenario-loader zip
  Program.cs                   Entry point

Longbow.Tests/                 xUnit test project (ModValidationService + a performance test)

docs/                          This documentation set + PARAMETERS.md reference + screenshots
scripts/                       Helper scripts (see repo root)
steamcmd/                      Vendored SteamCMD tool + its own logs/cache (checked into git)
addons/                        Bundled NoBackendScenarioLoader addon package

properties.json                Tool-level settings (log level, update-check URLs, default
                                scenario list, SteamCMD/workshop URLs)
state.json                     Persisted UI/advanced-settings state
mod_database.json              Local mod metadata cache (empty by default)
version.txt                    Current shipped version string
build_output.txt               A captured `dotnet build` log — see TROUBLESHOOTING.md, it does
                                not necessarily reflect the current state of the source tree.
```

## Current status

Per `version.txt` and `ArmaReforgerServerTool/Longbow.csproj` (`AssemblyVersion`/`FileVersion`),
the shipped version is **1.2.0**, consistent with `../HANDOFF.md`'s "Phase 1A shipped:
2026-06-29" / "Status: COMPLETE." No Phase 1B/2 work belongs in this repository — see
`../HANDOFF.md` for the split with the private Sentinel-Desktop repo.
