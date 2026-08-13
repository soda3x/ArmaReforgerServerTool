<script lang="ts">
  import type { Mod } from "../../api";

  interface Props {
    initial?: Mod | null;
    onSave: (m: Mod) => void;
    onClose: () => void;
  }

  let { initial = null, onSave, onClose }: Props = $props();

  let modId = $state(initial?.modId ?? "");
  let name = $state(initial?.name ?? "");
  let version = $state(initial?.version ?? "latest");
  let required = $state(initial?.required ?? false);

  let error = $state("");

  function save() {
    if (modId.trim().length === 0 || name.trim().length === 0) {
      error = "Mod ID and Name are required.";
      return;
    }
    onSave({ modId: modId.trim(), name: name.trim(), version: version.trim() || "latest", required });
    onClose();
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal">
    <div class="modal-header">
      <h3>{initial ? "Edit Mod" : "Add Mod"}</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>
    <div class="modal-body">
      <div class="field-row">
        <label for="mod-id">Mod ID</label>
        <input id="mod-id" type="text" bind:value={modId} placeholder="e.g. 5D3608A38FE7726F" />
      </div>
      <div class="field-row">
        <label for="mod-name">Name</label>
        <input id="mod-name" type="text" bind:value={name} placeholder="Mod display name" />
      </div>
      <div class="field-row">
        <label for="mod-version">Version</label>
        <input id="mod-version" type="text" bind:value={version} placeholder="latest" />
        <span class="field-hint">Use "latest" to always track the newest version.</span>
      </div>
      <label class="toggle">
        <input type="checkbox" bind:checked={required} />
        <span class="switch"></span>
        <span>Required</span>
      </label>
      {#if error}
        <p style="color:var(--danger); margin-top:0.6rem;">{error}</p>
      {/if}
    </div>
    <div class="modal-footer">
      <button onclick={onClose}>Cancel</button>
      <button class="primary" onclick={save}>Save</button>
    </div>
  </div>
</div>
