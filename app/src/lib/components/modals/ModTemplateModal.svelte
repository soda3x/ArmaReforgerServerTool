<script lang="ts">
  import { onMount } from "svelte";
  import {
    type ModTemplate,
    type ModLists,
    listModTemplates,
    saveModTemplate,
    deleteModTemplate,
    applyModTemplate,
  } from "../../api";
  import { enabledMods } from "../../stores";

  interface Props {
    onApplied: (lists: ModLists) => void;
    onClose: () => void;
  }

  let { onApplied, onClose }: Props = $props();

  let templates = $state<ModTemplate[]>([]);
  let loading = $state(true);
  let busy = $state(false);
  let errorMsg = $state("");

  let newName = $state("");
  let newDescription = $state("");
  // Two ways to apply a template: as the whole loadout, or added on top of what's enabled.
  let replaceOnApply = $state(true);
  let confirmingDelete = $state<string | null>(null);

  onMount(() => {
    refresh();
  });

  async function refresh() {
    loading = true;
    errorMsg = "";
    try {
      templates = await listModTemplates();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }
  }

  async function run(fn: () => Promise<void>) {
    busy = true;
    errorMsg = "";
    try {
      await fn();
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      busy = false;
    }
  }

  async function onSave() {
    await run(async () => {
      templates = await saveModTemplate(newName, newDescription);
      newName = "";
      newDescription = "";
    });
  }

  async function onApply(name: string) {
    await run(async () => {
      onApplied(await applyModTemplate(name, replaceOnApply));
    });
  }

  async function onDelete(name: string) {
    await run(async () => {
      templates = await deleteModTemplate(name);
      confirmingDelete = null;
    });
  }

  function formatUpdated(iso: string): string {
    if (!iso) return "";
    const d = new Date(iso);
    return Number.isNaN(d.getTime()) ? "" : d.toLocaleDateString();
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal" style="width:min(720px, 96vw); max-height:90vh;">
    <div class="modal-header">
      <h3>Mod Templates</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>

    <div class="modal-body">
      {#if errorMsg}
        <p style="color:var(--danger);">{errorMsg}</p>
      {/if}

      <div class="card" style="margin-bottom:1rem;">
        <div class="section-title">Save current loadout</div>
        <p class="field-hint" style="margin-top:0;">
          Saves the {$enabledMods.length} enabled
          {$enabledMods.length === 1 ? "mod" : "mods"}, in their current load order.
        </p>
        <div class="grid-2" style="gap:0.6rem;">
          <div class="field-row">
            <label for="tpl-name">Template name</label>
            <input id="tpl-name" type="text" bind:value={newName} placeholder="e.g. Ops Night" />
          </div>
          <div class="field-row">
            <label for="tpl-desc">Description (optional)</label>
            <input id="tpl-desc" type="text" bind:value={newDescription} placeholder="What this set is for" />
          </div>
        </div>
        <button
          class="small primary"
          style="margin-top:0.6rem;"
          onclick={onSave}
          disabled={busy || newName.trim().length === 0 || $enabledMods.length === 0}
        >
          Save template
        </button>
      </div>

      <div style="display:flex; align-items:center; justify-content:space-between; gap:0.75rem; margin-bottom:0.5rem;">
        <strong>Saved templates</strong>
        <label class="toggle" style="margin:0;">
          <input type="checkbox" bind:checked={replaceOnApply} />
          <span class="switch"></span>
          <span>Replace enabled mods when applying</span>
        </label>
      </div>

      {#if loading}
        <p class="field-hint">Loading…</p>
      {:else if templates.length === 0}
        <p class="field-hint">
          No templates yet. Enable the mods you want, then save them above — or import someone
          else's list with Import… and save that.
        </p>
      {:else}
        <ul style="list-style:none; margin:0; padding:0; display:flex; flex-direction:column; gap:0.5rem;">
          {#each templates as t (t.name)}
            <li style="border:1px solid var(--border); border-radius:var(--radius-sm); padding:0.6rem;">
              <div style="display:flex; align-items:flex-start; justify-content:space-between; gap:0.75rem; flex-wrap:wrap;">
                <div style="min-width:0; flex:1;">
                  <div><strong>{t.name}</strong> <span class="badge">{t.mods.length} mods</span></div>
                  {#if t.description}
                    <div class="field-hint">{t.description}</div>
                  {/if}
                  <div class="field-hint">
                    {t.mods.map((m) => m.name).join(", ")}
                  </div>
                  {#if formatUpdated(t.updatedAt)}
                    <div class="field-hint">Saved {formatUpdated(t.updatedAt)}</div>
                  {/if}
                </div>
                <div style="display:flex; gap:0.3rem; flex-shrink:0;">
                  <button class="small primary" onclick={() => onApply(t.name)} disabled={busy}>Apply</button>
                  {#if confirmingDelete === t.name}
                    <button class="small danger" onclick={() => onDelete(t.name)} disabled={busy}>Confirm</button>
                    <button class="small" onclick={() => (confirmingDelete = null)} disabled={busy}>Cancel</button>
                  {:else}
                    <button class="small" onclick={() => (confirmingDelete = t.name)} disabled={busy}>Delete</button>
                  {/if}
                </div>
              </div>
            </li>
          {/each}
        </ul>
      {/if}
    </div>

    <div class="modal-footer">
      <button onclick={onClose}>Close</button>
    </div>
  </div>
</div>
