<script lang="ts">
  import { onMount } from "svelte";
  import ConfirmModal from "../modals/ConfirmModal.svelte";
  import {
    rconConnect,
    rconDisconnect,
    rconSendRawCommand,
    rconKick,
    rconBanCreate,
    rconBanRemove,
    rconBanList,
    listPlayerSessions,
    clearPlayerHistory,
    type BanEntry,
    type PlayerSession,
  } from "../../api";
  import {
    serverConfiguration,
    rconConnected,
    rconPlayers,
    rconConsoleLines,
    clearRconConsole,
  } from "../../stores";

  let errorMsg = $state("");
  let connectBusy = $state(false);

  const rcon = $derived($serverConfiguration.root.rcon);

  async function connect() {
    if (!rcon) return;
    connectBusy = true;
    errorMsg = "";
    try {
      await rconConnect(rcon.address || "127.0.0.1", rcon.port, rcon.password);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      connectBusy = false;
    }
  }

  async function disconnect() {
    connectBusy = true;
    try {
      await rconDisconnect();
    } finally {
      connectBusy = false;
    }
  }

  // --- Console -------------------------------------------------------------------------------

  let commandInput = $state("");
  let consoleContainer: HTMLDivElement | undefined = $state();

  $effect(() => {
    $rconConsoleLines;
    if (consoleContainer) consoleContainer.scrollTop = consoleContainer.scrollHeight;
  });

  async function sendCommand() {
    const command = commandInput.trim();
    if (!command) return;
    commandInput = "";
    try {
      await rconSendRawCommand(command);
    } catch {
      // The backend already emits an "Error: ..." console line for this; nothing more to do.
    }
  }

  // --- Players -------------------------------------------------------------------------------

  let kickTarget = $state<{ id: string; name: string } | null>(null);

  async function confirmKick() {
    if (!kickTarget) return;
    try {
      await rconKick(kickTarget.id);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  // --- Bans ----------------------------------------------------------------------------------

  let bans = $state<BanEntry[]>([]);
  let bansLoading = $state(false);
  let banIdentityId = $state("");
  let banDurationSecs = $state(0);
  let banReason = $state("");
  let removeBanTarget = $state<string | null>(null);

  async function refreshBans() {
    bansLoading = true;
    try {
      bans = await rconBanList(null);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      bansLoading = false;
    }
  }

  async function createBan() {
    if (!banIdentityId.trim()) return;
    try {
      await rconBanCreate(banIdentityId.trim(), banDurationSecs, banReason.trim());
      banIdentityId = "";
      banDurationSecs = 0;
      banReason = "";
      await refreshBans();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  async function confirmRemoveBan() {
    if (!removeBanTarget) return;
    try {
      await rconBanRemove(removeBanTarget);
      await refreshBans();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  // --- Session history -------------------------------------------------------------------------

  let sessions = $state<PlayerSession[]>([]);
  let sessionsLoading = $state(false);
  let showClearHistoryConfirm = $state(false);

  async function refreshSessions() {
    sessionsLoading = true;
    try {
      sessions = await listPlayerSessions();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      sessionsLoading = false;
    }
  }

  async function confirmClearHistory() {
    try {
      await clearPlayerHistory();
      await refreshSessions();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }

  function formatDuration(joinedAt: string, leftAt: string | null): string {
    const end = leftAt ? new Date(leftAt).getTime() : Date.now();
    const start = new Date(joinedAt).getTime();
    const totalSeconds = Math.max(0, Math.round((end - start) / 1000));
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;
    return hours > 0 ? `${hours}h ${minutes}m` : minutes > 0 ? `${minutes}m ${seconds}s` : `${seconds}s`;
  }

  onMount(() => {
    refreshBans();
    refreshSessions();
  });
</script>

<div style="display:flex; flex-direction:column; gap:1rem;">
  <div class="card" style="display:flex; align-items:center; justify-content:space-between; flex-wrap:wrap; gap:0.5rem;">
    <div style="display:flex; align-items:center; gap:0.75rem;">
      <h2 style="margin:0;">RCON</h2>
      <span class={"badge " + ($rconConnected ? "online" : "offline")}>
        {$rconConnected ? "Connected" : "Disconnected"}
      </span>
    </div>
    {#if !rcon}
      <span class="field-hint">Configure RCON on the Configuration tab first.</span>
    {:else}
      <div style="display:flex; align-items:center; gap:0.75rem; flex-wrap:wrap;">
        <span class="field-hint" style="font-family:var(--font-mono);">{rcon.address || "0.0.0.0"}:{rcon.port}</span>
        {#if $rconConnected}
          <button class="small" disabled={connectBusy} onclick={disconnect}>Disconnect</button>
        {:else}
          <button class="small primary" disabled={connectBusy} onclick={connect}>Connect</button>
        {/if}
      </div>
    {/if}
  </div>

  {#if errorMsg}
    <div class="card" style="border-color:var(--danger);">
      <p style="margin:0; color:var(--danger);">{errorMsg}</p>
    </div>
  {/if}

  <div class="card">
    <div class="section-title">Console</div>
    <div
      bind:this={consoleContainer}
      class="scrollbar-thin"
      style="height:220px; overflow-y:auto; background:var(--bg-input); border:1px solid var(--border); border-radius:var(--radius-sm); padding:0.6rem; font-family:var(--font-mono); font-size:0.8em; white-space:pre-wrap; word-break:break-all;"
    >
      {#each $rconConsoleLines as line, i (i)}
        <div>{line}</div>
      {:else}
        <span class="field-hint">No console output yet.</span>
      {/each}
    </div>
    <div style="display:flex; gap:0.5rem; margin-top:0.6rem;">
      <input
        type="text"
        placeholder="#players, #kick <id>, #ban list…"
        style="flex:1; font-family:var(--font-mono);"
        bind:value={commandInput}
        disabled={!$rconConnected}
        onkeydown={(e) => e.key === "Enter" && sendCommand()}
      />
      <button class="small primary" disabled={!$rconConnected || !commandInput.trim()} onclick={sendCommand}>
        Send
      </button>
      <button class="small" onclick={clearRconConsole}>Clear</button>
    </div>
  </div>

  <div class="grid-2" style="gap:1rem;">
    <div class="card">
      <div class="section-title">Players ({$rconPlayers.length})</div>
      {#if $rconPlayers.length === 0}
        <p class="field-hint">
          {$rconConnected ? "No players online." : "Connect to RCON to see the player list."}
        </p>
      {:else}
        <table style="width:100%; border-collapse:collapse;">
          <tbody>
            {#each $rconPlayers as player (player.id)}
              <tr style="border-bottom:1px solid var(--border);">
                <td style="padding:0.4rem 0;">{player.name}</td>
                <td style="padding:0.4rem 0; font-family:var(--font-mono); font-size:0.85em; color:var(--text-dim);">
                  {player.id}
                </td>
                <td style="padding:0.4rem 0; text-align:right;">
                  <button class="small danger" onclick={() => (kickTarget = { id: player.id, name: player.name })}>
                    Kick
                  </button>
                </td>
              </tr>
            {/each}
          </tbody>
        </table>
      {/if}
    </div>

    <div class="card">
      <div style="display:flex; align-items:center; justify-content:space-between;">
        <div class="section-title" style="margin:0; border:none; padding:0;">Bans</div>
        <button class="small" onclick={refreshBans} disabled={bansLoading}>Refresh</button>
      </div>
      <div style="display:flex; gap:0.4rem; margin:0.6rem 0; flex-wrap:wrap;">
        <input type="text" placeholder="Identity id" style="flex:1; min-width:140px;" bind:value={banIdentityId} />
        <input
          type="number"
          placeholder="Seconds (0 = permanent)"
          style="width:170px;"
          bind:value={banDurationSecs}
        />
        <input type="text" placeholder="Reason (optional)" style="flex:1; min-width:120px;" bind:value={banReason} />
        <button class="small primary" onclick={createBan}>Ban</button>
      </div>
      {#if bans.length === 0}
        <p class="field-hint">No bans found.</p>
      {:else}
        <table style="width:100%; border-collapse:collapse;">
          <tbody>
            {#each bans as ban (ban.identityId)}
              <tr style="border-bottom:1px solid var(--border);">
                <td style="padding:0.4rem 0; font-family:var(--font-mono); font-size:0.85em;">{ban.identityId}</td>
                <td style="padding:0.4rem 0; color:var(--text-dim);">{ban.reason}</td>
                <td style="padding:0.4rem 0; text-align:right;">
                  <button class="small danger" onclick={() => (removeBanTarget = ban.identityId)}>Remove</button>
                </td>
              </tr>
            {/each}
          </tbody>
        </table>
      {/if}
    </div>
  </div>

  <div class="card">
    <div style="display:flex; align-items:center; justify-content:space-between;">
      <div class="section-title" style="margin:0; border:none; padding:0;">Player Session History</div>
      <div style="display:flex; gap:0.4rem;">
        <button class="small" onclick={refreshSessions} disabled={sessionsLoading}>Refresh</button>
        <button class="small danger" onclick={() => (showClearHistoryConfirm = true)}>Clear</button>
      </div>
    </div>
    {#if sessions.length === 0}
      <p class="field-hint" style="margin-top:0.6rem;">No recorded sessions yet.</p>
    {:else}
      <table style="width:100%; border-collapse:collapse; margin-top:0.6rem;">
        <thead>
          <tr style="text-align:left; border-bottom:1px solid var(--border);">
            <th style="padding:0.3rem 0; font-weight:500; color:var(--text-dim);">Player</th>
            <th style="padding:0.3rem 0; font-weight:500; color:var(--text-dim);">Joined</th>
            <th style="padding:0.3rem 0; font-weight:500; color:var(--text-dim);">Left</th>
            <th style="padding:0.3rem 0; font-weight:500; color:var(--text-dim);">Duration</th>
          </tr>
        </thead>
        <tbody>
          {#each sessions as session, i (session.playerId + session.joinedAt + i)}
            <tr style="border-bottom:1px solid var(--border);">
              <td style="padding:0.4rem 0;">{session.playerName}</td>
              <td style="padding:0.4rem 0; font-family:var(--font-mono); font-size:0.85em;">
                {new Date(session.joinedAt).toLocaleString()}
              </td>
              <td style="padding:0.4rem 0; font-family:var(--font-mono); font-size:0.85em;">
                {session.leftAt ? new Date(session.leftAt).toLocaleString() : "Online"}
              </td>
              <td style="padding:0.4rem 0;">{formatDuration(session.joinedAt, session.leftAt)}</td>
            </tr>
          {/each}
        </tbody>
      </table>
    {/if}
  </div>
</div>

{#if kickTarget}
  <ConfirmModal
    title="Kick player"
    body={`Kick ${kickTarget.name} from the server? They'll be able to rejoin.`}
    confirmLabel="Kick"
    danger
    onConfirm={confirmKick}
    onClose={() => (kickTarget = null)}
  />
{/if}

{#if removeBanTarget}
  <ConfirmModal
    title="Remove ban"
    body={`Remove the ban for identity ${removeBanTarget}?`}
    confirmLabel="Remove"
    danger
    onConfirm={confirmRemoveBan}
    onClose={() => (removeBanTarget = null)}
  />
{/if}

{#if showClearHistoryConfirm}
  <ConfirmModal
    title="Clear session history"
    body="Permanently delete all recorded player session history? This can't be undone."
    confirmLabel="Clear"
    danger
    onConfirm={confirmClearHistory}
    onClose={() => (showClearHistoryConfirm = false)}
  />
{/if}
