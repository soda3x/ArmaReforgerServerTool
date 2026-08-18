<script lang="ts">
  import { onMount } from "svelte";
  import { getDefaultScenarios, getScenariosForEnabledMods, type Scenario, type ModScenario } from "../../api";

  interface Props {
    currentScenarioId: string;
    onSelect: (scenarioId: string) => void;
    onClose: () => void;
  }

  let { currentScenarioId, onSelect, onClose }: Props = $props();

  let scenarios = $state<Scenario[]>([]);
  let modScenarios = $state<ModScenario[]>([]);
  let loading = $state(true);
  let loadError = $state("");
  // Mod-contributed scenarios failing to load (a mod lookup timing out, say) shouldn't block the
  // built-in list from showing — tracked separately so one failure doesn't blank the whole modal.
  let modScenariosError = $state("");

  // Grouped by owning mod, in first-seen order, so scenarios from the same mod stay together
  // under one heading rather than interleaved.
  const modScenarioGroups = $derived.by(() => {
    const groups = new Map<string, ModScenario[]>();
    for (const s of modScenarios) {
      const list = groups.get(s.modName);
      if (list) list.push(s);
      else groups.set(s.modName, [s]);
    }
    return groups;
  });
  // `currentScenarioId` seeds the manual-entry field once when the modal opens; each open
  // creates a fresh instance of this component (see ConfigurationTab's `{#if
  // showScenarioModal}`), so it never changes mid-life.
  // svelte-ignore state_referenced_locally
  let manualId = $state(currentScenarioId);

  onMount(async () => {
    try {
      scenarios = await getDefaultScenarios();
    } catch (e) {
      loadError = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }

    // Separate try/catch: mod scenarios are a best-effort addition on top of the built-in list,
    // and shouldn't be reported as though the whole picker failed to load.
    try {
      modScenarios = await getScenariosForEnabledMods();
    } catch (e) {
      modScenariosError = e instanceof Error ? e.message : String(e);
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

      {#if modScenariosError}
        <p style="color:var(--danger);">{modScenariosError}</p>
      {:else if modScenarios.length > 0}
        <div class="section-title">From your enabled mods</div>
        <div style="max-height:280px; overflow-y:auto; margin-bottom:1rem;" class="scrollbar-thin">
          {#each modScenarioGroups.entries() as [modName, group] (modName)}
            <div class="field-hint" style="margin:0.5rem 0 0.2rem 0;">{modName}</div>
            <ul style="list-style:none; margin:0; padding:0; display:flex; flex-direction:column; gap:0.4rem;">
              {#each group as s (s.path)}
                <li>
                  <button
                    style="width:100%; text-align:left; display:flex; flex-direction:column; gap:0.15rem;"
                    class={s.path === currentScenarioId ? "primary" : ""}
                    onclick={() => pick(s.path)}
                  >
                    <span>{s.name} {#if s.playerCount > 0}<span class="field-hint">· {s.playerCount} players</span>{/if}</span>
                    <span class="field-hint" style="font-family:var(--font-mono);">{s.path}</span>
                  </button>
                </li>
              {/each}
            </ul>
          {/each}
        </div>
      {/if}

      <div class="section-title">Manual scenario ID</div>
      <p class="field-hint">
        Scenarios from other Workshop mods aren't detected unless the mod is enabled here first —
        paste a scenario ID/path directly if you need one that isn't listed above.
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
