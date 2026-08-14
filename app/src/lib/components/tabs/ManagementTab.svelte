<script lang="ts">
  import { onMount } from "svelte";
  import { open } from "@tauri-apps/plugin-dialog";
  import {
    type AdvancedSetting,
    type AdvancedSettingValue,
    type LaunchArguments,
    type ServerTarget,
    SETTING_NAME_TO_LAUNCH_ARG_FIELD,
    emptyLaunchArguments,
    getAdvancedSettings,
    setAdvancedSetting,
    getConfigFlags,
    setConfigFlags,
    getUseUpnp,
    setUseUpnp,
    getInstallDir,
    isSteamCmdInstalled,
    locateServerFiles,
    setInstallDir,
    downloadSteamCmd,
    deleteServerFiles,
    isServerStarted,
    startStopServer,
    killServer,
    buildLaunchArgumentsPreview,
    checkForUpdates,
    setServerConfiguration,
    isWslAvailable,
    listWslDistros,
    getSavedGames,
    renameSave,
    LOG_LEVELS,
    type LogLevel,
  } from "../../api";
  import {
    serverConfiguration,
    advancedSettings,
    configFlags,
    useUpnp,
    installDir,
    steamCmdInstalled,
    serverTargetKind,
    wslDistro,
    serverRunning,
    serverBusy,
    buttonIcon,
    serverRunningLabel,
    startServerBtnEnabled,
    logLines,
    clearLogLines,
  } from "../../stores";

  let loading = $state(true);
  let errorMsg = $state("");
  let installBusy = $state(false);
  let deleteConfirm = $state(false);
  let updateCheckMsg = $state("");
  let launchPreview = $state("");
  let logContainer: HTMLDivElement | undefined = $state();
  let autoScroll = $state(true);

  // WSL detection is lazy (only probed the first time the WSL target is selected) rather than
  // on every mount, since it shells out to `wsl.exe` and most users will never touch it.
  let wslChecked = $state(false);
  let wslAvailable = $state(false);
  let wslDistros = $state<string[]>([]);

  async function ensureWslChecked() {
    if (wslChecked) return;
    wslChecked = true;
    try {
      wslAvailable = await isWslAvailable();
      wslDistros = wslAvailable ? await listWslDistros() : [];
    } catch {
      // wsl.exe missing or erroring is a normal, expected outcome on a machine without WSL —
      // fall back to manual distro entry rather than surfacing this as an app error.
      wslAvailable = false;
      wslDistros = [];
    }
  }

  // Saved-game names -> full path, as returned by the backend (a `.LatestSave` sentinel entry
  // is filtered out server-side, since "use the latest save" is its own dropdown option below).
  let savedGames = $state<Record<string, string>>({});
  let renamingSave = $state<string | null>(null);
  let renameDraft = $state("");

  async function refreshSavedGames() {
    try {
      savedGames = await getSavedGames();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  function onSessionSaveChange(value: string) {
    if (value === "__none__") {
      configFlags.update((f) => ({ ...f, usingSave: false, save: null }));
    } else if (value === "__latest__") {
      configFlags.update((f) => ({ ...f, usingSave: true, save: null }));
    } else {
      configFlags.update((f) => ({ ...f, usingSave: true, save: value }));
    }
    persistConfigFlags();
  }

  function startRename(name: string) {
    renamingSave = name;
    renameDraft = name;
  }

  async function confirmRename() {
    if (!renamingSave) return;
    const oldName = renamingSave;
    const newName = renameDraft.trim();
    if (newName.length === 0 || newName === oldName) {
      renamingSave = null;
      return;
    }
    try {
      const finalName = await renameSave(oldName, newName);
      // Keep the current selection pointed at the renamed save if it was the active one.
      configFlags.update((f) => (f.save === oldName ? { ...f, save: finalName } : f));
      if ($configFlags.save === finalName) {
        await persistConfigFlags();
      }
      await refreshSavedGames();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      renamingSave = null;
    }
  }

  async function loadAll() {
    loading = true;
    errorMsg = "";
    try {
      const [settings, flags, upnp, dir, steamcmd, running] = await Promise.all([
        getAdvancedSettings(),
        getConfigFlags(),
        getUseUpnp(),
        getInstallDir(),
        isSteamCmdInstalled(),
        isServerStarted(),
      ]);
      await refreshSavedGames();
      advancedSettings.set(settings);
      configFlags.set(flags);
      useUpnp.set(upnp);
      installDir.set(dir);
      steamCmdInstalled.set(steamcmd);
      serverRunning.set(running);
      buttonIcon.set(running ? "stop" : "play");
      serverRunningLabel.set(running ? "Server running" : "Server stopped");
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }
  }

  onMount(() => {
    loadAll();
  });

  $effect(() => {
    if (autoScroll && logContainer) {
      logContainer.scrollTop = logContainer.scrollHeight;
    }
  });

  // --- Install directory / SteamCMD -------------------------------------------------------

  async function pickAndLocate() {
    const dir = await open({ title: "Locate existing server install", directory: true, multiple: false });
    if (!dir || Array.isArray(dir)) return;
    installBusy = true;
    errorMsg = "";
    try {
      await locateServerFiles(dir);
      installDir.set(dir);
      steamCmdInstalled.set(await isSteamCmdInstalled());
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      installBusy = false;
    }
  }

  async function pickAndDownload() {
    const dir = await open({ title: "Choose an empty install directory", directory: true, multiple: false });
    if (!dir || Array.isArray(dir)) return;
    installBusy = true;
    errorMsg = "";
    try {
      await setInstallDir(dir);
      installDir.set(dir);
      await downloadSteamCmd();
      steamCmdInstalled.set(await isSteamCmdInstalled());
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      installBusy = false;
    }
  }

  async function onDeleteServerFiles() {
    if (!deleteConfirm) {
      deleteConfirm = true;
      return;
    }
    deleteConfirm = false;
    installBusy = true;
    errorMsg = "";
    try {
      await deleteServerFiles();
      installDir.set(null);
      steamCmdInstalled.set(false);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      installBusy = false;
    }
  }

  async function onCheckForUpdates() {
    updateCheckMsg = "Checking…";
    try {
      const v = await checkForUpdates();
      updateCheckMsg = v ? `Update available: ${v}` : "You're up to date.";
    } catch (e) {
      updateCheckMsg = e instanceof Error ? e.message : String(e);
    }
  }

  // --- Toggles -----------------------------------------------------------------------------

  async function onToggleUpnp(enabled: boolean) {
    useUpnp.set(enabled);
    try {
      await setUseUpnp(enabled);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  async function persistConfigFlags() {
    try {
      await setConfigFlags($configFlags);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  function onToggleExperimental(enabled: boolean) {
    configFlags.update((f) => ({ ...f, useExperimentalServer: enabled }));
    persistConfigFlags();
  }
  function onToggleAutoRestart(enabled: boolean) {
    configFlags.update((f) => ({ ...f, autoRestartOnCrash: enabled }));
    persistConfigFlags();
  }
  function onToggleNoBackend(enabled: boolean) {
    configFlags.update((f) => ({ ...f, noBackend: enabled }));
    persistConfigFlags();
  }
  function onLogLevelChange(level: LogLevel) {
    configFlags.update((f) => ({ ...f, logLevel: level }));
    persistConfigFlags();
  }

  function onServerTargetChange(kind: "windows" | "wsl") {
    serverTargetKind.set(kind);
    if (kind === "wsl") {
      ensureWslChecked();
    }
  }

  // --- Advanced settings ---------------------------------------------------------------------

  function settingEntries(): [string, AdvancedSetting][] {
    return Object.entries($advancedSettings).sort(([a], [b]) => a.localeCompare(b));
  }

  function valueKind(v: AdvancedSettingValue): "switch" | "bool" | "number" | "string" {
    if (v === null || v === undefined) return "switch";
    if (typeof v === "boolean") return "bool";
    if (typeof v === "number") return "number";
    return "string";
  }

  async function persistSetting(setting: AdvancedSetting) {
    advancedSettings.update((s) => ({ ...s, [setting.name]: setting }));
    try {
      await setAdvancedSetting(setting);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  function onToggleSettingEnabled(name: string, enabled: boolean) {
    const s = $advancedSettings[name];
    if (!s) return;
    persistSetting({ ...s, enabled });
  }

  function onChangeSettingValue(name: string, value: AdvancedSettingValue) {
    const s = $advancedSettings[name];
    if (!s) return;
    persistSetting({ ...s, value });
  }

  // --- Launch arguments / start-stop ----------------------------------------------------------

  // Only the optional, user-toggled advanced settings are built here. The mandatory arguments
  // (config / profile / addonsDir / logStats / logLevel) are injected by the backend, which is
  // the only side that knows the resolved install directory they're derived from.
  function buildLaunchArguments(): LaunchArguments {
    const args = emptyLaunchArguments();
    for (const [name, setting] of Object.entries($advancedSettings)) {
      if (!setting.enabled) continue;
      const field = SETTING_NAME_TO_LAUNCH_ARG_FIELD[name];
      if (!field) continue;
      const value = setting.value === null || setting.value === undefined ? "" : String(setting.value);
      args[field] = { key: name, value };
    }
    return args;
  }

  function currentServerTarget(): ServerTarget {
    if ($serverTargetKind === "wsl") {
      return { kind: "wsl", distro: $wslDistro && $wslDistro.trim().length > 0 ? $wslDistro : null };
    }
    return { kind: "windows" };
  }

  async function onToggleStartStop() {
    serverBusy.set(true);
    errorMsg = "";
    try {
      // Push any unsaved Configuration-tab edits first: the backend serializes its own copy of
      // the config into server.json on start, so without this the server would launch with
      // whatever was last explicitly saved rather than what's on screen.
      if (!$serverRunning) {
        await setServerConfiguration($serverConfiguration);
      }
      await startStopServer(buildLaunchArguments(), currentServerTarget());
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      serverBusy.set(false);
    }
  }

  async function onKill() {
    errorMsg = "";
    try {
      await killServer();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  async function onPreview() {
    try {
      launchPreview = await buildLaunchArgumentsPreview(buildLaunchArguments(), currentServerTarget());
    } catch (e) {
      launchPreview = e instanceof Error ? e.message : String(e);
    }
  }
</script>

<div style="display:flex; flex-direction:column; gap:1rem;">
  <div class="card" style="display:flex; align-items:center; justify-content:space-between; flex-wrap:wrap; gap:0.75rem;">
    <div>
      <h2 style="margin:0;">Management</h2>
      <p class="field-hint" style="margin:0;">{$serverRunningLabel}</p>
    </div>
    <div style="display:flex; align-items:center; gap:0.6rem;">
      <button onclick={onPreview} disabled={$serverBusy}>Preview Launch Args</button>
      <button class="danger" onclick={onKill} disabled={!$serverRunning}>Kill</button>
      <button
        class={$buttonIcon === "play" ? "primary" : "danger"}
        onclick={onToggleStartStop}
        disabled={$serverBusy || !$startServerBtnEnabled}
      >
        {#if $serverBusy}
          Working…
        {:else if $buttonIcon === "play"}
          ▶ Start Server
        {:else}
          ■ Stop Server
        {/if}
      </button>
    </div>
  </div>

  {#if launchPreview}
    <div class="card">
      <div class="section-title">Launch Arguments Preview</div>
      <code style="display:block; white-space:pre-wrap; word-break:break-all; font-family:var(--font-mono); font-size:0.85em;">{launchPreview}</code>
    </div>
  {/if}

  {#if errorMsg}
    <div class="card" style="border-color:var(--danger);"><p style="color:var(--danger); margin:0;">{errorMsg}</p></div>
  {/if}

  <div class="card">
    <div class="section-title">Install Directory</div>
    <div style="display:flex; align-items:center; gap:0.6rem; margin-bottom:0.6rem;">
      <span style="font-family:var(--font-mono); flex:1; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;">
        {$installDir ?? "No install directory set"}
      </span>
      <span class={"badge " + ($steamCmdInstalled ? "online" : "offline")}>{$steamCmdInstalled ? "SteamCMD installed" : "SteamCMD missing"}</span>
    </div>
    <div style="display:flex; gap:0.6rem; flex-wrap:wrap;">
      <button onclick={pickAndLocate} disabled={installBusy}>Locate Existing Install…</button>
      <button onclick={pickAndDownload} disabled={installBusy}>New Install (Download SteamCMD)…</button>
      <button onclick={onCheckForUpdates} disabled={installBusy}>Check for Tool Updates</button>
      {#if !deleteConfirm}
        <button class="danger" onclick={onDeleteServerFiles} disabled={installBusy || !$installDir}>Delete Server Files</button>
      {:else}
        <button class="danger" onclick={onDeleteServerFiles} disabled={installBusy}>Confirm Delete?</button>
        <button onclick={() => (deleteConfirm = false)} disabled={installBusy}>Cancel</button>
      {/if}
    </div>
    {#if installBusy}
      <p class="field-hint" style="margin-top:0.6rem;">Working, this can take a while for SteamCMD downloads…</p>
    {/if}
    {#if updateCheckMsg}
      <p class="field-hint" style="margin-top:0.6rem;">{updateCheckMsg}</p>
    {/if}
  </div>

  <div class="card">
    <div class="section-title">Runtime Options</div>
    <div class="grid-3">
      <label class="toggle">
        <input type="checkbox" checked={$useUpnp} onchange={(e) => onToggleUpnp((e.target as HTMLInputElement).checked)} />
        <span class="switch"></span>
        <span>Use UPnP</span>
      </label>
      <label class="toggle">
        <input type="checkbox" checked={$configFlags.useExperimentalServer} onchange={(e) => onToggleExperimental((e.target as HTMLInputElement).checked)} />
        <span class="switch"></span>
        <span>Use Experimental Server Branch</span>
      </label>
      <label class="toggle">
        <input type="checkbox" checked={$configFlags.autoRestartOnCrash} onchange={(e) => onToggleAutoRestart((e.target as HTMLInputElement).checked)} />
        <span class="switch"></span>
        <span>Auto-restart on Crash</span>
      </label>
      <label class="toggle">
        <input type="checkbox" checked={$configFlags.noBackend} onchange={(e) => onToggleNoBackend((e.target as HTMLInputElement).checked)} />
        <span class="switch"></span>
        <span>No Backend Mode</span>
      </label>
    </div>
    <div class="grid-2" style="margin-top:0.6rem;">
      <div class="field-row">
        <span class="field-label">Server Target</span>
        <div style="display:flex; gap:1rem;">
          <label style="display:flex; align-items:center; gap:0.35rem;">
            <input type="radio" name="target" checked={$serverTargetKind === "windows"} onchange={() => onServerTargetChange("windows")} /> Windows
          </label>
          <label style="display:flex; align-items:center; gap:0.35rem;">
            <input type="radio" name="target" checked={$serverTargetKind === "wsl"} onchange={() => onServerTargetChange("wsl")} /> WSL
          </label>
        </div>
      </div>
      {#if $serverTargetKind === "wsl"}
        <div class="field-row">
          <label for="wsl-distro">WSL Distro (blank = default)</label>
          {#if wslChecked && !wslAvailable}
            <span class="field-hint" style="color:var(--danger);">
              WSL was not detected on this machine — the server will fail to start.
            </span>
          {/if}
          {#if wslDistros.length > 0}
            <select
              id="wsl-distro"
              value={$wslDistro ?? ""}
              onchange={(e) => wslDistro.set((e.target as HTMLSelectElement).value)}
            >
              <option value="">Default</option>
              {#each wslDistros as distro (distro)}
                <option value={distro}>{distro}</option>
              {/each}
            </select>
          {:else}
            <input
              id="wsl-distro"
              type="text"
              value={$wslDistro ?? ""}
              oninput={(e) => wslDistro.set((e.target as HTMLInputElement).value)}
              placeholder="Ubuntu"
            />
          {/if}
        </div>
      {/if}
      <div class="field-row">
        <label for="log-level">Server Log Level</label>
        <select
          id="log-level"
          value={$configFlags.logLevel}
          onchange={(e) => onLogLevelChange((e.target as HTMLSelectElement).value as LogLevel)}
        >
          {#each LOG_LEVELS as level (level)}
            <option value={level}>{level}</option>
          {/each}
        </select>
      </div>
    </div>
  </div>

  <div class="card">
    <div class="section-title">Session Save</div>
    <div class="grid-2">
      <div class="field-row">
        <label for="session-save">Load on start</label>
        <select
          id="session-save"
          value={!$configFlags.usingSave ? "__none__" : ($configFlags.save ?? "__latest__")}
          onchange={(e) => onSessionSaveChange((e.target as HTMLSelectElement).value)}
        >
          <option value="__none__">Don't load a save</option>
          <option value="__latest__">Latest save</option>
          {#each Object.keys(savedGames).sort() as name (name)}
            <option value={name}>{name}</option>
          {/each}
        </select>
        {#if Object.keys(savedGames).length === 0}
          <span class="field-hint">
            No saves found{$installDir ? "" : " — set an install directory first"}.
          </span>
        {/if}
      </div>

      <div class="field-row">
        <span class="field-label">Manage saves</span>
        {#if renamingSave}
          <div style="display:flex; gap:0.4rem; align-items:center;">
            <input
              type="text"
              value={renameDraft}
              oninput={(e) => (renameDraft = (e.target as HTMLInputElement).value)}
              aria-label="New save name"
            />
            <button class="small primary" onclick={confirmRename}>Save</button>
            <button class="small" onclick={() => (renamingSave = null)}>Cancel</button>
          </div>
        {:else}
          <div style="display:flex; gap:0.4rem; flex-wrap:wrap; align-items:center;">
            <button class="small" onclick={refreshSavedGames}>Refresh</button>
            <button
              class="small"
              disabled={!$configFlags.save}
              onclick={() => $configFlags.save && startRename($configFlags.save)}
            >
              Rename selected…
            </button>
          </div>
        {/if}
      </div>
    </div>
  </div>

  <div class="card">
    <div class="section-title">Advanced Settings</div>
    {#if loading}
      <p class="field-hint">Loading…</p>
    {:else}
      <div style="display:flex; flex-direction:column; gap:0.5rem;">
        {#each settingEntries() as [name, setting] (name)}
          <div style="display:flex; align-items:center; gap:0.75rem; padding:0.4rem 0; border-bottom:1px solid var(--border);">
            <label class="toggle" style="min-width:220px;">
              <input type="checkbox" checked={setting.enabled} onchange={(e) => onToggleSettingEnabled(name, (e.target as HTMLInputElement).checked)} />
              <span class="switch"></span>
              <span style="font-family:var(--font-mono); font-size:0.9em;">{name}</span>
            </label>
            <div style="flex:1; max-width:320px;">
              {#if valueKind(setting.value) === "switch"}
                <span class="field-hint">Boolean CLI switch (no value)</span>
              {:else if valueKind(setting.value) === "bool"}
                <label class="toggle">
                  <input
                    type="checkbox"
                    checked={setting.value === true}
                    onchange={(e) => onChangeSettingValue(name, (e.target as HTMLInputElement).checked)}
                  />
                  <span class="switch"></span>
                </label>
              {:else if valueKind(setting.value) === "number"}
                <input
                  type="number"
                  value={setting.value as number}
                  oninput={(e) => onChangeSettingValue(name, Number((e.target as HTMLInputElement).value))}
                />
              {:else}
                <input
                  type="text"
                  value={setting.value as string}
                  oninput={(e) => onChangeSettingValue(name, (e.target as HTMLInputElement).value)}
                />
              {/if}
            </div>
          </div>
        {/each}
      </div>
    {/if}
  </div>

  <div class="card">
    <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:0.5rem;">
      <div class="section-title" style="margin:0; border:none; padding:0;">Server Log</div>
      <div style="display:flex; align-items:center; gap:0.6rem;">
        <label class="toggle">
          <input type="checkbox" bind:checked={autoScroll} />
          <span class="switch"></span>
          <span>Auto-scroll</span>
        </label>
        <button class="small" onclick={clearLogLines}>Clear</button>
      </div>
    </div>
    <div
      bind:this={logContainer}
      class="scrollbar-thin"
      style="height:280px; overflow-y:auto; background:var(--bg-input); border:1px solid var(--border); border-radius:var(--radius-sm); padding:0.6rem; font-family:var(--font-mono); font-size:0.8em; white-space:pre-wrap; word-break:break-all;"
    >
      {#each $logLines as line, i (i)}
        <div>{line}</div>
      {:else}
        <span class="field-hint">No log output yet.</span>
      {/each}
    </div>
  </div>
</div>
