<script lang="ts">
  import { onMount } from "svelte";
  import { open, save } from "@tauri-apps/plugin-dialog";
  import TextField from "../fields/TextField.svelte";
  import NumberField from "../fields/NumberField.svelte";
  import BoolField from "../fields/BoolField.svelte";
  import SelectField from "../fields/SelectField.svelte";
  import ListField from "../fields/ListField.svelte";
  import TextBlockField from "../fields/TextBlockField.svelte";
  import ScenarioModal from "../modals/ScenarioModal.svelte";
  import ModTransferList from "../ModTransferList.svelte";
  import {
    defaultRcon,
    getServerConfiguration,
    setServerConfiguration,
    saveServerConfiguration,
    loadServerConfigurationFromPath,
    saveServerConfigurationToPath,
    type RconPermission,
  } from "../../api";
  import { serverConfiguration } from "../../stores";

  let loading = $state(true);
  let saving = $state(false);
  let statusMsg = $state("");
  let errorMsg = $state("");
  let showScenarioModal = $state(false);

  // 3-state navmesh streaming UI: off / all / specific.
  type NavmeshMode = "off" | "all" | "specific";
  let navmeshMode = $state<NavmeshMode>("off");
  let navmeshList = $state<string[]>([]);

  function syncNavmeshFromConfig() {
    const v = $serverConfiguration.root.operating.disableNavmeshStreaming;
    if (v === null || v === undefined) {
      navmeshMode = "off";
      navmeshList = [];
    } else if (v.length === 0) {
      navmeshMode = "all";
      navmeshList = [];
    } else {
      navmeshMode = "specific";
      navmeshList = v;
    }
  }

  function applyNavmeshToConfig() {
    if (navmeshMode === "off") {
      $serverConfiguration.root.operating.disableNavmeshStreaming = null;
    } else if (navmeshMode === "all") {
      $serverConfiguration.root.operating.disableNavmeshStreaming = [];
    } else {
      $serverConfiguration.root.operating.disableNavmeshStreaming = navmeshList;
    }
  }

  $effect(() => {
    // Keep the config object's navmesh field synced whenever the UI mode/list changes.
    navmeshMode;
    navmeshList;
    applyNavmeshToConfig();
  });

  let rconEnabled = $state(false);

  $effect(() => {
    rconEnabled = $serverConfiguration.root.rcon !== null;
  });

  function toggleRcon(enabled: boolean) {
    rconEnabled = enabled;
    if (enabled && $serverConfiguration.root.rcon === null) {
      $serverConfiguration.root.rcon = defaultRcon();
    } else if (!enabled) {
      $serverConfiguration.root.rcon = null;
    }
  }

  async function load() {
    loading = true;
    errorMsg = "";
    try {
      const cfg = await getServerConfiguration();
      serverConfiguration.set(cfg);
      syncNavmeshFromConfig();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }
  }

  onMount(() => {
    load();
  });

  async function persistToBackend() {
    await setServerConfiguration($serverConfiguration);
  }

  async function onSave() {
    saving = true;
    statusMsg = "";
    errorMsg = "";
    try {
      await persistToBackend();
      await saveServerConfiguration();
      statusMsg = "Saved to install directory's server.json.";
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      saving = false;
    }
  }

  async function onSaveAs() {
    const path = await save({
      title: "Save server configuration",
      defaultPath: "server.json",
      filters: [{ name: "JSON", extensions: ["json"] }],
    });
    if (!path) return;
    saving = true;
    statusMsg = "";
    errorMsg = "";
    try {
      await persistToBackend();
      await saveServerConfigurationToPath(path);
      statusMsg = `Saved to ${path}`;
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      saving = false;
    }
  }

  async function onLoad() {
    const path = await open({
      title: "Load server configuration",
      multiple: false,
      filters: [{ name: "JSON", extensions: ["json"] }],
    });
    if (!path || Array.isArray(path)) return;
    loading = true;
    statusMsg = "";
    errorMsg = "";
    try {
      const cfg = await loadServerConfigurationFromPath(path);
      serverConfiguration.set(cfg);
      syncNavmeshFromConfig();
      statusMsg = `Loaded from ${path}`;
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }
  }

  const permissionOptions: { value: RconPermission; label: string }[] = [
    { value: "admin", label: "Admin" },
    { value: "monitor", label: "Monitor" },
  ];
</script>

<div style="display:flex; flex-direction:column; gap:1rem;">
  <div class="card" style="display:flex; align-items:center; justify-content:space-between;">
    <div>
      <h2 style="margin:0;">Configuration</h2>
      <p class="field-hint" style="margin:0;">Edits server.json. Save writes to the install directory (or a custom path).</p>
    </div>
    <div style="display:flex; gap:0.6rem; align-items:center;">
      {#if statusMsg}<span class="field-hint">{statusMsg}</span>{/if}
      <button onclick={load} disabled={loading || saving}>Reload</button>
      <button onclick={onLoad} disabled={loading || saving}>Load From…</button>
      <button onclick={onSaveAs} disabled={loading || saving}>Save As…</button>
      <button class="primary" onclick={onSave} disabled={loading || saving}>Save</button>
    </div>
  </div>

  {#if errorMsg}
    <div class="card" style="border-color:var(--danger);"><p style="color:var(--danger); margin:0;">{errorMsg}</p></div>
  {/if}

  {#if !loading}
  <svelte:boundary>
    {#snippet failed(error)}
      <div class="card" style="border-color:var(--danger);">
        <p style="color:var(--danger);">Render error: {String(error)}</p>
      </div>
    {/snippet}
    <!-- Mods and Server Settings sit side by side (as in the original tool) so the settings
         aren't pushed below the fold by the tall mod lists. Collapses to one column when the
         window is too narrow to give both halves usable width. -->
    <div class="config-split">
      <div class="config-col">
        <ModTransferList />

        <div class="card">
          <div class="section-title">RCON</div>
          <div style="margin-bottom:0.6rem;">
            <label class="toggle">
              <input type="checkbox" checked={rconEnabled} onchange={(e) => toggleRcon((e.target as HTMLInputElement).checked)} />
              <span class="switch"></span>
              <span>{rconEnabled ? "Enabled" : "Disabled"}</span>
            </label>
          </div>
          {#if rconEnabled && $serverConfiguration.root.rcon}
            <div class="grid-2">
              <TextField label="Address" bind:value={$serverConfiguration.root.rcon.address} />
              <NumberField label="Port" bind:value={$serverConfiguration.root.rcon.port} min={1} max={65535} />
              <TextField label="Password" bind:value={$serverConfiguration.root.rcon.password} password />
              <SelectField label="Permission" bind:value={$serverConfiguration.root.rcon.permission} options={permissionOptions} />
              <NumberField label="Max Clients" bind:value={$serverConfiguration.root.rcon.maxClients} min={1} max={16} />
            </div>
            <div class="grid-2">
              <ListField label="Whitelist" bind:value={$serverConfiguration.root.rcon.whitelist} placeholder="IP address" />
              <ListField label="Blacklist" bind:value={$serverConfiguration.root.rcon.blacklist} placeholder="IP address" />
            </div>
          {/if}
        </div>

        <div class="card">
          <div class="section-title">VON (Voice over Network)</div>
          <div class="grid-3">
            <BoolField label="Disable VON UI" bind:value={$serverConfiguration.root.game.gameProperties.VONDisableUI} />
            <BoolField label="Disable Direct Speech UI" bind:value={$serverConfiguration.root.game.gameProperties.VONDisableDirectSpeechUI} />
            <BoolField label="Cross-faction Transmit" bind:value={$serverConfiguration.root.game.gameProperties.VONCanTransmitCrossFaction} />
          </div>
        </div>

        <div class="card">
          <div class="section-title">Persistence</div>
          <div class="grid-2">
            <NumberField label="Auto-save Interval (min)" bind:value={$serverConfiguration.root.game.gameProperties.persistence.autoSaveInterval} min={0} max={60} />
            <NumberField label="Hive ID" bind:value={$serverConfiguration.root.game.gameProperties.persistence.hiveId} min={0} max={16383} />
          </div>
          <div class="grid-2">
            <TextBlockField
              label="Databases (raw JSON)"
              value={$serverConfiguration.root.game.gameProperties.persistence.databases}
              onChange={(v) => ($serverConfiguration.root.game.gameProperties.persistence.databases = v)}
            />
            <TextBlockField
              label="Storages (raw JSON)"
              value={$serverConfiguration.root.game.gameProperties.persistence.storages}
              onChange={(v) => ($serverConfiguration.root.game.gameProperties.persistence.storages = v)}
            />
          </div>
        </div>
      </div>

      <div class="config-col">
    <div class="card">
      <div class="section-title">Basic</div>
      <div class="grid-2">
        <TextField label="Server Name" bind:value={$serverConfiguration.root.game.name} placeholder="Required" />
        <NumberField label="Max Players" bind:value={$serverConfiguration.root.game.maxPlayers} min={1} max={256} />
        <TextField label="Password" bind:value={$serverConfiguration.root.game.password} password />
        <TextField label="Admin Password" bind:value={$serverConfiguration.root.game.passwordAdmin} password />
      </div>
      <div class="grid-2">
        <div class="field-row">
          <label for="scenario-id">Scenario</label>
          <div style="display:flex; gap:0.5rem;">
            <input id="scenario-id" type="text" readonly value={$serverConfiguration.root.game.scenarioId} placeholder="No scenario selected" />
            <button class="small" onclick={() => (showScenarioModal = true)}>Select…</button>
          </div>
        </div>
        <ListField label="Admins" bind:value={$serverConfiguration.root.game.admins} placeholder="Steam64 ID" />
      </div>
      <div class="grid-3">
        <BoolField label="Visible in server list" bind:value={$serverConfiguration.root.game.visible} />
        <BoolField label="Cross-platform" bind:value={$serverConfiguration.root.game.crossPlatform} />
        <BoolField label="Mods required by default" bind:value={$serverConfiguration.root.game.modsRequiredByDefault} />
      </div>
    </div>

    <div class="card">
      <div class="section-title">Network / Ports</div>
      <div class="grid-2">
        <TextField label="Bind Address" bind:value={$serverConfiguration.root.bindAddress} />
        <NumberField label="Bind Port" bind:value={$serverConfiguration.root.bindPort} min={1} max={65535} />
        <TextField label="Public Address" bind:value={$serverConfiguration.root.publicAddress} />
        <NumberField label="Public Port" bind:value={$serverConfiguration.root.publicPort} min={1} max={65535} />
        <TextField label="A2S Address" bind:value={$serverConfiguration.root.a2s.address} />
        <NumberField label="A2S Port" bind:value={$serverConfiguration.root.a2s.port} min={1} max={65535} />
      </div>
    </div>

    <div class="card">
      <div class="section-title">Game Properties</div>
      <div class="grid-3">
        <NumberField label="Max View Distance" bind:value={$serverConfiguration.root.game.gameProperties.serverMaxViewDistance} min={500} max={12000} />
        <NumberField label="Min Grass Distance" bind:value={$serverConfiguration.root.game.gameProperties.serverMinGrassDistance} min={0} max={150} />
        <NumberField label="Network View Distance" bind:value={$serverConfiguration.root.game.gameProperties.networkViewDistance} min={500} max={5000} />
      </div>
      <div class="grid-3">
        <BoolField label="Disable 3rd Person" bind:value={$serverConfiguration.root.game.gameProperties.disableThirdPerson} />
        <BoolField label="Fast Validation" bind:value={$serverConfiguration.root.game.gameProperties.fastValidation} />
        <BoolField label="BattlEye" bind:value={$serverConfiguration.root.game.gameProperties.battlEye} />
      </div>
      <TextBlockField
        label="Mission Header (raw JSON)"
        value={$serverConfiguration.root.game.gameProperties.missionHeader}
        onChange={(v) => ($serverConfiguration.root.game.gameProperties.missionHeader = v)}
      />
    </div>

    <div class="card">
      <div class="section-title">Operating / AI</div>
      <div class="grid-3">
        <BoolField label="Lobby Player Sync" bind:value={$serverConfiguration.root.operating.lobbyPlayerSynchronise} />
        <NumberField label="Player Save Time (s)" bind:value={$serverConfiguration.root.operating.playerSaveTime} min={1} max={65535} />
        <NumberField label="AI Limit (-1 = unlimited)" bind:value={$serverConfiguration.root.operating.aiLimit} min={-1} max={1000} />
      </div>
      <div class="grid-3">
        <NumberField label="Slot Reservation Timeout (s)" bind:value={$serverConfiguration.root.operating.slotReservationTimeout} min={5} max={300} />
        <NumberField label="Join Queue Max Size" bind:value={$serverConfiguration.root.operating.joinQueue.maxSize} min={0} max={50} />
        <BoolField label="Disable AI" bind:value={$serverConfiguration.root.operating.disableAI} />
      </div>
      <div class="grid-3">
        <BoolField label="Disable Server Shutdown" bind:value={$serverConfiguration.root.operating.disableServerShutdown} />
        <BoolField label="Disable Crash Reporter" bind:value={$serverConfiguration.root.operating.disableCrashReporter} />
      </div>

      <div class="field-row">
        <span class="field-label">Disable Navmesh Streaming</span>
        <div style="display:flex; gap:1.2rem; align-items:center;">
          <label style="display:flex; align-items:center; gap:0.35rem;"><input type="radio" name="navmesh" value="off" bind:group={navmeshMode} /> Off</label>
          <label style="display:flex; align-items:center; gap:0.35rem;"><input type="radio" name="navmesh" value="all" bind:group={navmeshMode} /> Disable all</label>
          <label style="display:flex; align-items:center; gap:0.35rem;"><input type="radio" name="navmesh" value="specific" bind:group={navmeshMode} /> Specific</label>
        </div>
        {#if navmeshMode === "specific"}
          <ListField bind:value={navmeshList} placeholder="Navmesh name" />
        {/if}
      </div>
    </div>
      </div>
    </div>
  </svelte:boundary>
  {:else}
    <div class="card">Loading configuration…</div>
  {/if}
</div>

<style>
  .config-split {
    display: grid;
    grid-template-columns: minmax(380px, 5fr) minmax(460px, 7fr);
    gap: 1rem;
    align-items: start;
  }

  .config-col {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    min-width: 0;
  }

  /* Below this the two columns get too cramped to be useful, so stack them. */
  @media (max-width: 1100px) {
    .config-split {
      grid-template-columns: 1fr;
    }
  }
</style>

{#if showScenarioModal}
  <ScenarioModal
    currentScenarioId={$serverConfiguration.root.game.scenarioId}
    onSelect={(id) => ($serverConfiguration.root.game.scenarioId = id)}
    onClose={() => (showScenarioModal = false)}
  />
{/if}
