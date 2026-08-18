<script lang="ts">
  import { onDestroy, onMount } from "svelte";
  import { listen, type UnlistenFn } from "@tauri-apps/api/event";
  import ConfigurationTab from "$lib/components/tabs/ConfigurationTab.svelte";
  import ManagementTab from "$lib/components/tabs/ManagementTab.svelte";
  import StatusTab from "$lib/components/tabs/StatusTab.svelte";
  import RconTab from "$lib/components/tabs/RconTab.svelte";
  import { SERVER_EVENT, RCON_EVENT, type ProcessEvent, type RconEvent } from "$lib/api";
  import {
    appendLogLine,
    pushStatus,
    buttonIcon,
    serverRunningLabel,
    startServerBtnEnabled,
    serverRunning,
    steamCmdProgress,
    rconConnected,
    rconPlayers,
    appendRconLine,
  } from "$lib/stores";

  type TabId = "configuration" | "management" | "status" | "rcon";
  let activeTab = $state<TabId>("configuration");

  const tabs: { id: TabId; label: string }[] = [
    { id: "configuration", label: "Configuration" },
    { id: "management", label: "Management" },
    { id: "status", label: "Status" },
    { id: "rcon", label: "RCON" },
  ];

  let unlisten: UnlistenFn | undefined;
  let unlistenRcon: UnlistenFn | undefined;

  onMount(async () => {
    unlisten = await listen<ProcessEvent>(SERVER_EVENT, (event) => {
      const payload = event.payload;
      switch (payload.type) {
        case "log":
          appendLogLine(payload.line);
          break;
        case "guiUpdate":
          buttonIcon.set(payload.buttonIcon);
          serverRunningLabel.set(payload.serverRunningLabelText);
          startServerBtnEnabled.set(payload.startServerBtnEnabled);
          serverRunning.set(payload.buttonIcon === "stop");
          break;
        case "status":
          pushStatus({
            lastFps: payload.lastFps,
            lastMemKb: payload.lastMemKb,
            lastPlayerCount: payload.lastPlayerCount,
            lastPingSite: payload.lastPingSite,
            lastJoinCode: payload.lastJoinCode,
            lastRconIp: payload.lastRconIp,
            lastRconPort: payload.lastRconPort,
            lastIp: payload.lastIp,
            lastPort: payload.lastPort,
            serverOnline: payload.serverOnline,
            lastUpdate: payload.lastUpdate,
          });
          break;
        case "steamCmdProgress":
          steamCmdProgress.set(payload.progress);
          break;
      }
    });

    unlistenRcon = await listen<RconEvent>(RCON_EVENT, (event) => {
      const payload = event.payload;
      switch (payload.type) {
        case "connectionChanged":
          rconConnected.set(payload.connected);
          if (!payload.connected) rconPlayers.set([]);
          break;
        case "consoleLine":
          appendRconLine(payload.text);
          break;
        case "playersUpdated":
          rconPlayers.set(payload.players);
          break;
      }
    });
  });

  onDestroy(() => {
    unlisten?.();
    unlistenRcon?.();
  });
</script>

<div class="app-shell">
  <header class="app-header">
    <div class="brand">
      <img class="brand-logo" src="/longbow.png" alt="" />
      <span class="field-hint">Dedicated Server Tool for Arma Reforger</span>
    </div>
    <nav class="tabs">
      {#each tabs as tab (tab.id)}
        <button class={"tab-btn" + (activeTab === tab.id ? " active" : "")} onclick={() => (activeTab = tab.id)}>
          {tab.label}
        </button>
      {/each}
    </nav>
  </header>

  <main class="app-content scrollbar-thin">
    {#if activeTab === "configuration"}
      <ConfigurationTab />
    {:else if activeTab === "management"}
      <ManagementTab />
    {:else if activeTab === "rcon"}
      <RconTab />
    {:else}
      <StatusTab />
    {/if}
  </main>
</div>

<style>
  .app-shell {
    display: flex;
    flex-direction: column;
    height: 100vh;
  }

  .app-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.75rem 1.25rem;
    border-bottom: 1px solid var(--border);
    background: var(--bg-elevated);
    flex-shrink: 0;
    gap: 1.5rem;
    flex-wrap: wrap;
  }

  .brand {
    display: flex;
    align-items: center;
    gap: 0.75rem;
  }

  .brand-logo {
    height: 34px;
    width: auto;
  }

  .tabs {
    display: flex;
    gap: 0.4rem;
  }

  .tab-btn {
    background: transparent;
    border: 1px solid transparent;
    color: var(--text-dim);
    padding: 0.5em 1.1em;
    border-radius: var(--radius-sm);
  }

  .tab-btn:hover {
    color: var(--text);
    background: var(--bg-hover);
    border-color: transparent;
  }

  .tab-btn.active {
    color: #fff;
    background: var(--accent);
    border-color: var(--accent);
  }

  .app-content {
    flex: 1;
    overflow-y: auto;
    padding: 1.25rem;
    /* Wide enough for the Configuration tab's two-column (mods | settings) layout — the
       original tool ran at 1550px for the same reason. */
    max-width: 1700px;
    width: 100%;
    margin: 0 auto;
  }
</style>
