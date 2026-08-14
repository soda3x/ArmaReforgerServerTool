<script lang="ts">
  interface Props {
    title: string;
    text: string;
    onSave: (text: string) => void;
    onClose: () => void;
  }

  let { title, text, onSave, onClose }: Props = $props();

  // `text` seeds the editable draft once when the modal opens; each open creates a fresh
  // instance of this component (see TextBlockField's `{#if open}`), so it never changes
  // mid-life.
  // svelte-ignore state_referenced_locally
  let draft = $state(text);
  let error = $state("");

  function save() {
    try {
      JSON.parse(draft);
    } catch (e) {
      error = e instanceof Error ? e.message : "Invalid JSON";
      return;
    }
    error = "";
    onSave(draft);
    onClose();
  }

  function format() {
    try {
      draft = JSON.stringify(JSON.parse(draft), null, 2);
      error = "";
    } catch (e) {
      error = e instanceof Error ? e.message : "Invalid JSON";
    }
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal" style="width:min(720px, 92vw);">
    <div class="modal-header">
      <h3>{title}</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>
    <div class="modal-body">
      <textarea rows="16" bind:value={draft} spellcheck="false"></textarea>
      {#if error}
        <p style="color:var(--danger); margin-top:0.5rem;">{error}</p>
      {/if}
    </div>
    <div class="modal-footer">
      <button onclick={format}>Format JSON</button>
      <span style="flex:1"></span>
      <button onclick={onClose}>Cancel</button>
      <button class="primary" onclick={save}>Save</button>
    </div>
  </div>
</div>
