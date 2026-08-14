<script lang="ts">
  interface Props {
    title: string;
    items: string[];
    placeholder?: string;
    onSave: (items: string[]) => void;
    onClose: () => void;
  }

  let { title, items, placeholder = "Add an entry…", onSave, onClose }: Props = $props();

  // `items` seeds the editable draft once when the modal opens; each open creates a fresh
  // instance of this component (see ListField's `{#if open}`), so it never changes mid-life.
  // svelte-ignore state_referenced_locally
  let draft = $state<string[]>([...items]);
  let newEntry = $state("");

  function addEntry() {
    const trimmed = newEntry.trim();
    if (trimmed.length === 0) return;
    draft = [...draft, trimmed];
    newEntry = "";
  }

  function removeEntry(idx: number) {
    draft = draft.filter((_, i) => i !== idx);
  }

  function save() {
    onSave(draft);
    onClose();
  }

  function onKeydown(e: KeyboardEvent) {
    if (e.key === "Enter") {
      e.preventDefault();
      addEntry();
    }
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal">
    <div class="modal-header">
      <h3>{title}</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>
    <div class="modal-body">
      <div style="display:flex; gap:0.5rem; margin-bottom:0.85rem;">
        <input type="text" {placeholder} bind:value={newEntry} onkeydown={onKeydown} />
        <button onclick={addEntry}>Add</button>
      </div>
      {#if draft.length === 0}
        <p class="field-hint">No entries yet.</p>
      {:else}
        <ul style="list-style:none; margin:0; padding:0; display:flex; flex-direction:column; gap:0.4rem;">
          {#each draft as entry, i (i)}
            <li style="display:flex; align-items:center; justify-content:space-between; gap:0.5rem; background:var(--bg-input); border:1px solid var(--border); border-radius:var(--radius-sm); padding:0.4rem 0.6rem;">
              <span style="word-break:break-all;">{entry}</span>
              <button class="small danger icon-btn" onclick={() => removeEntry(i)} aria-label="Remove">✕</button>
            </li>
          {/each}
        </ul>
      {/if}
    </div>
    <div class="modal-footer">
      <button onclick={onClose}>Cancel</button>
      <button class="primary" onclick={save}>Save</button>
    </div>
  </div>
</div>
