# FAQ

Common questions about the Longbow (ArmaReforgerServerTool) Phase 1A public repository. Answers
here are grounded in the actual code and config in this repo. For deeper detail see
[OVERVIEW.md](./OVERVIEW.md) and [ARCHITECTURE.md](./ARCHITECTURE.md); for known bugs see
[TROUBLESHOOTING.md](./TROUBLESHOOTING.md).

---

## What is Longbow, and what does the validator actually do?

Longbow is a WinForms (.NET 8) desktop tool for managing an **Arma Reforger** dedicated server.
The Phase 1A feature that defines this public repo is the **mod validator**. Given your
enabled-mod list, it:

- **Detects problems** — missing required dependencies, version conflicts (installed version
  outside a dependency's declared min/max window), and circular dependencies.
- **Auto-fixes** — adds missing dependency mods and reorders the list into a valid load order.
- **Gates the Start button** — "Start Server" stays disabled (red) until validation passes
  (green), so you can't launch a server with a broken mod set.

It also does the surrounding server-management work: start/stop/monitor the server process,
SteamCMD-driven binary updates, RCON broadcast messages, UPnP port mapping, and a large
server-parameters UI. The validator is the headline feature; the rest is supporting
infrastructure. See [ARCHITECTURE.md](./ARCHITECTURE.md) for the module-by-module breakdown.

## What platforms does it run on?

**Windows only.** The app targets `net8.0-windows` and uses WinForms
(`UseWindowsForms=true`, `OutputType=winexe` in `ArmaReforgerServerTool/Longbow.csproj`). It will
not build or run on Linux or macOS.

## What do I need to build it?

- The **.NET 8 SDK** (to build). To only *run* a published build, the .NET 8 Runtime is enough.
- **Windows** — see above.
- Optionally Visual Studio 2022, which can open the solution and build it for you.

## How do I build it?

From the repo root:

```bash
# Build (Release)
dotnet build -c Release

# Or build the whole solution explicitly (includes the test project)
dotnet build Longbow.sln -c Release
```

## How do I run it?

After a Release build, run the produced executable (Windows):

```bash
bin/Release/net8.0-windows/Longbow.exe
```

Equivalently, `dotnet run --project ArmaReforgerServerTool -c Release`.

## How do I run the tests?

There's an xUnit test project, `Longbow.Tests`, covering the mod-validation service plus a
performance test:

```bash
dotnet test Longbow.Tests
```

The `[Fact]` tests live in `Longbow.Tests/ModValidationServiceTests.cs` and
`Longbow.Tests/PerformanceTest.cs`.

## There are two `.sln` files — which one do I use?

Prefer the top-level **`Longbow.sln`**. There's also a narrower
`ArmaReforgerServerTool/ArmaReforgerServerTool.sln` that references only the app project. The
top-level solution references both the app and `Longbow.Tests`, so use it if you want the test
project included.

## Where does configuration live?

Tool-level settings are plain JSON files at the repo root:

- **`properties.json`** — the main tool config: log file path (`logFile`), minimum log level,
  update-check URLs (`updateRepositoryUrl`, `checkForUpdatesOnStartup`), the SteamCMD download
  URL, the Arma workshop base URL (`armaWorkshopUrl`), and the default scenario list. Loaded by
  `ToolPropertiesManager`.
- **`state.json`** — persisted UI / advanced-settings state, managed by `SavedStateManager`.
- **`mod_database.json`** — local mod-metadata cache. It's empty (`[]`) by default in the repo.

## Where do logs go?

Logging is via Serilog, configured in `ArmaReforgerServerTool/Program.cs`. It writes to the
console and to a rolling daily file. The default path is **`logs/longbow.log`** (see
`Models/ToolProperties.cs`), and it's configurable through the `logFile` / `minimumLogLevel`
keys in `properties.json`.

## Where does the Steam Workshop metadata cache live?

Separately from `mod_database.json`, `SteamWorkshopMetadataProvider` caches scraped mod metadata
under your Windows AppData folder: **`%AppData%/Longbow/ModMetadata.json`**.

## How does the "Steam Workshop integration" / real mod metadata work?

There is no official, versioned API behind it — it's web scraping, via two independent paths:

- `ModDependencyManager` fetches each mod's page at `{armaWorkshopUrl}/{modId}` (the
  `reforger.armaplatform.com/workshop` base from `properties.json`) with `HtmlAgilityPack` and
  parses the `__NEXT_DATA__` JSON blob the Bohemia Next.js site embeds in the page.
- `SteamWorkshopMetadataProvider` queries `steamcommunity.com/sharedfiles/filedetails/?id=...`
  pages and regex-matches the HTML, as a fallback in `ModMetadataSource` when a mod isn't in the
  hardcoded metadata table.

Because both depend on third-party page markup/JSON shape, either can silently break if Bohemia
or Valve change their pages. See "Fragile points" in [ARCHITECTURE.md](./ARCHITECTURE.md).

## Validation crashes with "An item with the same key has already been added" — what's wrong?

You have two mods in the enabled list with the same (case-insensitive) mod ID.
`ModValidationService.ValidateMods` builds its lookup with `ToDictionary(m => m.modId.ToLower())`,
which throws on a duplicate key, and nothing upstream guards against duplicates. The generic
error box you see is that raw framework exception. This is a known, unfixed issue — see the
"Duplicate mod IDs crash validation" entry in [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) for the
root cause and a suggested fix. Workaround: remove the duplicate mod entry before validating.

## The startup update check doesn't seem to tell me anything when it fails — is that expected?

Partly, yes — it's a known limitation. The startup update check
(`FileIOManager.CheckForUpdates`) only catches `HttpRequestException`. Other realistic failures
(a request timeout, or `version.txt` coming back as something `System.Version` can't parse, e.g.
an HTML error page) escape that catch, and because the check is fire-and-forget the failure is
swallowed silently. See "Update check swallows non-`HttpRequestException` failures" in
[TROUBLESHOOTING.md](./TROUBLESHOOTING.md).

## Why are there two `SitrepConfigService` classes? Which one is real?

Neither — they're both dead code. One lives in `Managers/SitrepConfigService.cs` (399 lines,
local-JSON-backed with `// TODO` placeholders) and one in `Services/SitrepConfigService.cs`
(242 lines, an `HttpClient` REST client). They compile side-by-side because they're in different
namespaces, but nothing else in the codebase references either. If you pick up Sitrep work, pick
one and delete the other. Details: "Duplicate, unused `SitrepConfigService` implementations" in
[TROUBLESHOOTING.md](./TROUBLESHOOTING.md).

## `build_output.txt` shows build errors — is the build broken?

Not necessarily. `build_output.txt` at the repo root is a **captured, point-in-time build log**,
not something CI regenerates on every change — treat it as a snapshot, not a live signal. It
records `CS1061` errors in `SteamWorkshopMetadataProvider.cs`, but reading the current source,
those don't reproduce by inspection. Don't trust that file over an actual `dotnet build` you run
yourself. See "`build_output.txt` disagrees with current source" in
[TROUBLESHOOTING.md](./TROUBLESHOOTING.md), which asks whoever next has a working toolchain to run
a clean build and update or delete the file.

## How do I report a bug?

Open a GitHub issue on this repo and label it `bug`. Per [`../CLAUDE.md`](../CLAUDE.md), this repo
is monitored by an automated triage session that picks up open `bug`-labeled issues, investigates,
and opens a fix PR. If you spot an issue while working in the code, also add a dated entry to
[TROUBLESHOOTING.md](./TROUBLESHOOTING.md) following the template at the bottom of that file.

## Is Phase 1B / the roadmap here?

No. This repository is the **Phase 1A MVP only**. Further development happens in a separate
private repo and is out of scope for these docs — everything documented here is limited to what's
actually implemented in this codebase.
