# Troubleshooting

Dated log of real issues found or fixed in this codebase. Never delete a resolved entry — mark
it `Resolved` and leave it for history. See [README.md](./README.md) for the status-label
convention (Open / Monitoring / Resolved) and the entry template at the bottom of this file.

---

### 2026-07-03 — `build_output.txt` disagrees with current source (SteamWorkshopMetadataProvider)

**Status:** Monitoring

**Symptom:** The committed `build_output.txt` at the repo root shows a real `dotnet build`
failure — 4x `CS1061` errors in
`ArmaReforgerServerTool/Utils/SteamWorkshopMetadataProvider.cs` (lines 102-104, 110), saying the
type `(string Name, ModDependency[] Dependencies, string Version)?` "does not contain a
definition for 'Name'" / `'Dependencies'` / `'Version'`.

**Root cause:** Read against the current source, this doesn't reproduce by inspection: the
method that produces this tuple (`QuerySteamWorkshopAsync`, lines 192-230) declares the return
type with those exact element names, and every access site (`metadata.Value.Name`, `.Dependencies`,
`.Version`) uses the matching names — which is valid, ordinary C# (named element access on a
`Nullable<(...)>` via `.Value` works fine in .NET 8 / C# 12). `git log` shows this file has only
ever been touched by the single initial "Phase 1A MVP" commit (`c236838`) — it was never edited
to "fix" this error. That, plus the fact that this repo shares Phase 1A source with the private
Sentinel-Desktop repo (see `3f79bad build: reconcile source trees`), makes it likely that
`build_output.txt` was captured from a slightly different snapshot of the file (or a transient
Roslyn/IDE state) and committed as-is rather than regenerated against what actually shipped.

**Remedy:** Could not run `dotnet build` in the environment this audit was performed in (no .NET
SDK available) to get a fresh, authoritative answer — this is a code-reading judgment call, not a
verified compile. Whoever next has a working `dotnet` toolchain for this repo should run a clean
`dotnet build -c Release` and:
- If it succeeds: delete or regenerate `build_output.txt` so it isn't misleading future readers,
  and flip this entry to `Resolved`.
- If it still fails with the same CS1061s: this write-up's reasoning above is wrong somewhere —
  re-open as `Open` and dig into why the named tuple isn't resolving (check for a second
  `ModDependency` type shadowing the one in `Models/`, or a TFM/LangVersion mismatch).

---

### 2026-07-03 — Duplicate mod IDs crash validation with an unhelpful error

**Status:** Open

**Symptom:** If two `Mod` entries in the enabled-mods list end up with the same (case-insensitive)
`modId`, clicking "Check Mods" (or the initial validation on form load) shows a generic error
message box: `Error checking mods: An item with the same key has already been added.` No mod
validation results are produced.

**Root cause:** `ModValidationService.ValidateMods` (`Managers/ModValidationService.cs:64`) builds
its lookup with:
```csharp
var modDict = enabledMods.ToDictionary(m => m.modId.ToLower(), m => m);
```
`Dictionary.ToDictionary` throws `ArgumentException` on a duplicate key, and this isn't guarded
anywhere upstream — there is no duplicate-ID check in `AddModDialog` (adding a mod manually),
`FileIOManager.LoadModsListFromFile` (importing a mods JSON file via `ConfigurationManager.
ImportModsList`), or anywhere else mods enter `ConfigurationManager`'s enabled list. The
exception bubbles out of `Task.Run(() => ModValidationService.GetInstance().ValidateMods(...))`
in `Main.cs` and is only caught by the generic `catch (Exception ex)` around the whole button
handler, which just surfaces `ex.Message` verbatim — a .NET framework message that means nothing
to an end user. Confirmed there's no existing test for this case
(`Longbow.Tests/ModValidationServiceTests.cs` has no duplicate-ID test).

**Remedy:** Not fixed as part of this documentation pass (docs-only change). Suggested fix for
whoever picks this up: either (a) de-duplicate the enabled-mods list before validation (e.g.
`GroupBy(m => m.modId.ToLower()).Select(g => g.First())` with a `WARNING`-level
`ValidationError` noting the dropped duplicate), or (b) use `ToDictionary` inside a try/catch
that turns the failure into a proper `ValidationError` naming the conflicting mod ID, so the user
gets an actionable message instead of a raw framework exception string.

---

### 2026-07-03 — Update check swallows non-`HttpRequestException` failures

**Status:** Open

**Symptom:** If the update-check request to GitHub succeeds at the HTTP layer but returns
unexpected content for `version.txt` (e.g. a GitHub outage page, a redirect-following HTML
response, an empty file, or a request that times out), the startup update check
(`FileIOManager.CheckForUpdates`, fired-and-forgotten from `Main.cs` via `_ = FileIOManager.
CheckForUpdates();`) fails silently — no error dialog, nothing actionable in the log beyond
whatever Serilog captures from an unobserved task fault.

**Root cause:** `CheckForUpdates` (`Managers/FileIOManager.cs:341-389`) wraps the whole body in
`catch (HttpRequestException e)` only. Two realistic failure modes aren't `HttpRequestException`:
- `new Version(latestVersionString.Trim())` (line 357) throws `FormatException` or
  `ArgumentException` if the fetched `version.txt` content isn't a clean `System.Version`-parsable
  string (e.g. `"1.2"` — needs 2-4 components with correct grouping in some cases — an HTML error
  page, or trailing whitespace/BOM edge cases).
- `HttpClient.GetStringAsync` (line 355) on a timeout throws `TaskCanceledException` (wrapping a
  `TimeoutException`), not `HttpRequestException`, under .NET 5+ semantics.
Since the call site is `_ = FileIOManager.CheckForUpdates();` (deliberately not awaited, so it
doesn't block form startup), any exception that escapes the narrow catch becomes an unobserved
task exception — it won't crash the app, but the user gets no indication the update check failed,
which defeats the purpose of `checkForUpdatesOnStartup` in `properties.json`.

**Remedy:** Not fixed as part of this documentation pass. Suggested fix: broaden the catch to
`catch (Exception e)` (this method is already at the top of a fire-and-forget call chain, so
there's no cleaner exception to let propagate), and keep using the existing
`Utilities.DisplayErrorMessage` fallback so failures are visible instead of silent.

---

### 2026-07-03 — Duplicate, unused `SitrepConfigService` implementations

**Status:** Open

**Symptom:** Two different classes named `SitrepConfigService` exist in the codebase:
`ReforgerServerApp.Managers.SitrepConfigService` (`Managers/SitrepConfigService.cs`, 399 lines,
local-JSON-file-backed with `// TODO: Implement when Sitrep API is available` placeholders at
every API call) and `ReforgerServerApp.Services.SitrepConfigService`
(`Services/SitrepConfigService.cs`, 242 lines, a working `HttpClient`-based REST client pointed
at `http://localhost:3000` by default). They compile fine side-by-side because they're in
different namespaces, but this is confusing: a search for `SitrepConfigService` returns two
unrelated implementations with no obvious signal for which (if either) is "real."

**Root cause:** Neither class is referenced anywhere else in the codebase — verified with
`grep -rn "SitrepConfigService"` across all `.cs` files, matches are confined to each file's own
declaration. Both are dead code. This is almost certainly a byproduct of the source-tree
reconciliation commit (`3f79bad build: reconcile source trees - add RconManager and proven
managers from Documents`) pulling in files from more than one working copy without deduping —
the same commit that added `RconManager` (which, unlike these, *is* wired up and used).

**Remedy:** Not fixed as part of this documentation pass (docs-only change; out of scope to
delete application code here). Whoever next touches mod-config persistence should pick one
implementation (or neither, if the local JSON/save-file flow already covers Phase 1A's needs —
it does; see `ConfigurationManager`/`FileIOManager`) and delete the other to avoid future
confusion.

---

## How to add an entry

Copy this template, fill it in, and add it to the top of the dated log above (newest first).
Use today's date. Never delete an entry once added — only change its `Status`.

```markdown
### YYYY-MM-DD — Short description

**Status:** Open | Monitoring | Resolved

**Symptom:** What a user or developer actually observes (error message, crash, wrong behavior).

**Root cause:** What in the code actually causes it — file/line references where possible.

**Remedy:** What was done (or should be done) to fix or mitigate it. If not yet fixed, say so
explicitly and describe the suggested fix so the next person doesn't have to re-diagnose it.
```
