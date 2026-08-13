<script lang="ts">
  import LineChart from "../LineChart.svelte";
  import { latestStatus, fpsHistory, memHistory } from "../../stores";
  import { pingSiteCountryCode, countryCodeToFlagEmoji } from "../../api";

  async function copy(text: string) {
    try {
      await navigator.clipboard.writeText(text);
    } catch {
      // clipboard access can fail silently in some contexts; no-op.
    }
  }

  // Resolve the ping site's flag only when the site actually changes, rather than on every
  // status tick (they arrive every few seconds).
  let flag = $state("");
  let resolvedSite = "";

  $effect(() => {
    const site = $latestStatus?.lastPingSite ?? "";
    if (site === resolvedSite) return;
    resolvedSite = site;
    flag = "";
    if (!site || site === "Unknown") return;
    pingSiteCountryCode(site).then((code) => {
      if (resolvedSite === site && code) flag = countryCodeToFlagEmoji(code);
    });
  });
</script>

<div style="display:flex; flex-direction:column; gap:1rem;">
  <div class="card" style="display:flex; align-items:center; justify-content:space-between;">
    <h2 style="margin:0;">Status</h2>
    <span class={"badge " + ($latestStatus?.serverOnline ? "online" : "offline")}>
      {$latestStatus?.serverOnline ? "Online" : "Offline"}
    </span>
  </div>

  <div class="grid-3" style="gap:1rem;">
    <div class="card">
      <div class="section-title">Server Address</div>
      <div style="display:flex; align-items:center; justify-content:space-between; gap:0.5rem;">
        <span style="font-family:var(--font-mono);">{$latestStatus ? `${$latestStatus.lastIp}:${$latestStatus.lastPort}` : "—"}</span>
        {#if $latestStatus}
          <button class="small" onclick={() => copy(`${$latestStatus?.lastIp}:${$latestStatus?.lastPort}`)}>Copy</button>
        {/if}
      </div>
    </div>

    <div class="card">
      <div class="section-title">RCON Address</div>
      <div style="display:flex; align-items:center; justify-content:space-between; gap:0.5rem;">
        <span style="font-family:var(--font-mono);">{$latestStatus ? `${$latestStatus.lastRconIp}:${$latestStatus.lastRconPort}` : "—"}</span>
        {#if $latestStatus}
          <button class="small" onclick={() => copy(`${$latestStatus?.lastRconIp}:${$latestStatus?.lastRconPort}`)}>Copy</button>
        {/if}
      </div>
    </div>

    <div class="card">
      <div class="section-title">Join Code</div>
      <div style="display:flex; align-items:center; justify-content:space-between; gap:0.5rem;">
        <span style="font-family:var(--font-mono);">{$latestStatus?.lastJoinCode || "—"}</span>
        {#if $latestStatus?.lastJoinCode}
          <button class="small" onclick={() => copy($latestStatus?.lastJoinCode ?? "")}>Copy</button>
        {/if}
      </div>
    </div>

    <div class="card">
      <div class="section-title">Player Count</div>
      <div style="font-size:1.6em; font-weight:600;">{$latestStatus?.lastPlayerCount ?? "—"}</div>
    </div>

    <div class="card">
      <div class="section-title">Ping Site</div>
      <div style="font-size:1.1em;">
        {#if flag}<span style="margin-right:0.4rem;">{flag}</span>{/if}{$latestStatus?.lastPingSite || "Unknown"}
      </div>
    </div>

    <div class="card">
      <div class="section-title">Last Update</div>
      <div style="font-family:var(--font-mono); font-size:0.9em;">{$latestStatus?.lastUpdate || "—"}</div>
    </div>
  </div>

  <div class="grid-2" style="gap:1rem;">
    <div class="card">
      <div class="section-title">FPS (last {$fpsHistory.length})</div>
      <LineChart points={$fpsHistory} color="#4f8cff" decimals={0} />
    </div>
    <div class="card">
      <div class="section-title">Memory (GB)</div>
      <LineChart points={$memHistory} color="#3ecf8e" unit=" GB" decimals={2} />
    </div>
  </div>
</div>
