<script lang="ts">
  import { onMount } from "svelte";
  import { open, save } from "@tauri-apps/plugin-dialog";
  import ModModal from "./modals/ModModal.svelte";
  import {
    type Mod,
    type ModLists,
    getModLists,
    addMod,
    removeMod,
    enableMod,
    disableMod,
    enableAllMods,
    disableAllMods,
    moveEnabledMod,
    updateMod,
    exportModsListToPath,
    importModsListFromPath,
  } from "../api";
  import { availableMods, enabledMods } from "../stores";

  let availableFilter = $state("");
  let enabledFilter = $state("");
  let showAddModal = $state(false);
  let editingMod = $state<Mod | null>(null);
  let busy = $state(false);
  let errorMsg = $state("");

  function applyLists(lists: ModLists) {
    availableMods.set(lists.available);
    enabledMods.set(lists.enabled);
  }

  async function refresh() {
    applyLists(await getModLists());
  }

  onMount(() => {
    refresh();
  });

  async function run(fn: () => Promise<ModLists>) {
    busy = true;
    errorMsg = "";
    try {
      applyLists(await fn());
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      busy = false;
    }
  }

  function filtered(list: Mod[], filter: string): Mod[] {
    const f = filter.trim().toLowerCase();
    if (!f) return list;
    return list.filter((m) => m.name.toLowerCase().includes(f) || m.modId.toLowerCase().includes(f));
  }

  // Edge checks are against the real enabled list, not the filtered view — with a filter
  // active the first visible row usually isn't the first mod in load order.
  const sameMod = (a: Mod, b: Mod) => a.modId === b.modId && a.name === b.name;
  function isFirst(m: Mod): boolean {
    return $enabledMods.findIndex((e) => sameMod(e, m)) === 0;
  }
  function isLast(m: Mod): boolean {
    return $enabledMods.findIndex((e) => sameMod(e, m)) === $enabledMods.length - 1;
  }

  async function onEnable(m: Mod) {
    await run(() => enableMod(m));
  }
  async function onDisable(m: Mod) {
    await run(() => disableMod(m));
  }
  async function onEnableAll() {
    await run(() => enableAllMods());
  }
  async function onDisableAll() {
    await run(() => disableAllMods());
  }
  async function onMove(m: Mod, delta: number) {
    await run(() => moveEnabledMod(m, delta));
  }
  async function onRemove(m: Mod) {
    await run(() => removeMod(m));
  }

  function openAdd() {
    editingMod = null;
    showAddModal = true;
  }
  function openEdit(m: Mod) {
    editingMod = m;
    showAddModal = true;
  }

  async function onModSaved(m: Mod) {
    // Editing goes through updateMod so version/required-only edits actually stick (mod
    // identity is name+modId, so an add would be treated as a duplicate and dropped) and so an
    // enabled mod stays enabled at its current load-order position.
    const original = editingMod;
    if (original) {
      await run(() => updateMod(original, m));
    } else {
      await run(() => addMod(m));
    }
  }

  async function onExport() {
    const path = await save({
      title: "Export enabled mods list",
      defaultPath: "mods.json",
      filters: [{ name: "JSON", extensions: ["json"] }],
    });
    if (!path) return;
    busy = true;
    errorMsg = "";
    try {
      await exportModsListToPath(path);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      busy = false;
    }
  }

  async function onImport() {
    const path = await open({
      title: "Import mods list",
      multiple: false,
      filters: [{ name: "JSON", extensions: ["json"] }],
    });
    if (!path || Array.isArray(path)) return;
    await run(() => importModsListFromPath(path));
  }
</script>

<div class="card">
  <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:0.75rem;">
    <h3 style="margin:0;">Mods</h3>
    <div style="display:flex; gap:0.5rem;">
      <button class="small" onclick={openAdd} disabled={busy}>+ Add Mod</button>
      <button class="small" onclick={onImport} disabled={busy}>Import…</button>
      <button class="small" onclick={onExport} disabled={busy}>Export…</button>
    </div>
  </div>

  {#if errorMsg}
    <p style="color:var(--danger);">{errorMsg}</p>
  {/if}

  <div class="grid-2" style="gap:1rem;">
    <div>
      <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:0.4rem;">
        <strong>Available</strong>
        <button class="small" onclick={onEnableAll} disabled={busy}>Enable all →</button>
      </div>
      <input type="text" placeholder="Filter…" bind:value={availableFilter} style="margin-bottom:0.5rem;" />
      <ul class="scrollbar-thin" style="list-style:none; margin:0; padding:0; height:300px; overflow-y:auto; border:1px solid var(--border); border-radius:var(--radius-sm);">
        {#each filtered($availableMods, availableFilter) as m (m.modId + m.name)}
          <li style="display:flex; align-items:center; justify-content:space-between; gap:0.4rem; padding:0.4rem 0.6rem; border-bottom:1px solid var(--border);">
            <div style="min-width:0;">
              <div style="overflow:hidden; text-overflow:ellipsis; white-space:nowrap;">{m.name}</div>
              <div class="field-hint" style="font-family:var(--font-mono);">{m.modId} · {m.version}</div>
            </div>
            <div style="display:flex; gap:0.3rem; flex-shrink:0;">
              <button class="small icon-btn" onclick={() => openEdit(m)} disabled={busy} aria-label="Edit">✎</button>
              <button class="small icon-btn" onclick={() => onEnable(m)} disabled={busy} aria-label="Enable">→</button>
              <button class="small danger icon-btn" onclick={() => onRemove(m)} disabled={busy} aria-label="Remove">✕</button>
            </div>
          </li>
        {:else}
          <li class="field-hint" style="padding:0.6rem;">No available mods.</li>
        {/each}
      </ul>
    </div>

    <div>
      <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:0.4rem;">
        <strong>Enabled (load order)</strong>
        <button class="small" onclick={onDisableAll} disabled={busy}>← Disable all</button>
      </div>
      <input type="text" placeholder="Filter…" bind:value={enabledFilter} style="margin-bottom:0.5rem;" />
      <ul class="scrollbar-thin" style="list-style:none; margin:0; padding:0; height:300px; overflow-y:auto; border:1px solid var(--border); border-radius:var(--radius-sm);">
        {#each filtered($enabledMods, enabledFilter) as m (m.modId + m.name)}
          <li style="display:flex; align-items:center; justify-content:space-between; gap:0.4rem; padding:0.4rem 0.6rem; border-bottom:1px solid var(--border);">
            <div style="min-width:0;">
              <div style="overflow:hidden; text-overflow:ellipsis; white-space:nowrap;">{m.name} {m.required ? "★" : ""}</div>
              <div class="field-hint" style="font-family:var(--font-mono);">{m.modId} · {m.version}</div>
            </div>
            <div style="display:flex; gap:0.3rem; flex-shrink:0;">
              <button class="small icon-btn" onclick={() => onMove(m, -1)} disabled={busy || isFirst(m)} aria-label="Move up">↑</button>
              <button class="small icon-btn" onclick={() => onMove(m, 1)} disabled={busy || isLast(m)} aria-label="Move down">↓</button>
              <button class="small icon-btn" onclick={() => openEdit(m)} disabled={busy} aria-label="Edit">✎</button>
              <button class="small icon-btn" onclick={() => onDisable(m)} disabled={busy} aria-label="Disable">←</button>
            </div>
          </li>
        {:else}
          <li class="field-hint" style="padding:0.6rem;">No enabled mods.</li>
        {/each}
      </ul>
    </div>
  </div>
</div>

{#if showAddModal}
  <ModModal initial={editingMod} onSave={onModSaved} onClose={() => (showAddModal = false)} />
{/if}
