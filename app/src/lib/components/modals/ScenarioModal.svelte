<script lang="ts">
  import { onMount } from "svelte";
  import { getDefaultScenarios, type Scenario } from "../../api";

  interface Props {
    currentScenarioId: string;
    onSelect: (scenarioId: string) => void;
    onClose: () => void;
  }

  let { currentScenarioId, onSelect, onClose }: Props = $props();

  let scenarios = $state<Scenario[]>([]);
  let loading = $state(true);
  let loadError = $state("");
  let manualId = $state(currentScenarioId);

  onMount(async () => {
    try {
      scenarios = await getDefaultScenarios();
    } catch (e) {
      loadError = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }
  });

  function pick(path: string) {
    onSelect(path);
    onClose();
  }

  function useManual() {
    onSelect(manualId.trim());
    onClose();
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal" style="width:min(640px, 92vw);">
    <div class="modal-header">
      <h3>Select Scenario</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>
    <div class="modal-body">
      {#if loading}
        <p class="field-hint">Loading built-in scenarios…</p>
      {:else if loadError}
        <p style="color:var(--danger);">{loadError}</p>
      {:else if scenarios.length === 0}
        <p class="field-hint">No built-in scenarios available.</p>
      {:else}
        <ul style="list-style:none; margin:0 0 1rem 0; padding:0; display:flex; flex-direction:column; gap:0.4rem; max-height:280px; overflow-y:auto;" class="scrollbar-thin">
          {#each scenarios as s (s.path)}
            <li>
              <button
                style="width:100%; text-align:left; display:flex; flex-direction:column; gap:0.15rem;"
                class={s.path === currentScenarioId ? "primary" : ""}
                onclick={() => pick(s.path)}
              >
                <span>{s.name}</span>
                <span class="field-hint" style="font-family:var(--font-mono);">{s.path}</span>
              </button>
            </li>
          {/each}
        </ul>
      {/if}

      <div class="section-title">Manual scenario ID</div>
      <p class="field-hint">
        Workshop-hosted scenarios aren't scraped automatically yet — paste a scenario ID/path directly.
      </p>
      <div style="display:flex; gap:0.5rem;">
        <input type="text" bind:value={manualId} placeholder="{'{'}ADDON_ID{'}'}Missions/Scenario.conf" />
        <button class="primary" onclick={useManual}>Use</button>
      </div>
    </div>
    <div class="modal-footer">
      <button onclick={onClose}>Close</button>
    </div>
  </div>
</div>
