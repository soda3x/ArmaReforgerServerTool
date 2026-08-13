<script lang="ts">
  import ListEditorModal from "../modals/ListEditorModal.svelte";

  interface Props {
    label?: string;
    value: string[];
    placeholder?: string;
    hint?: string;
  }

  let { label = "", value = $bindable([]), placeholder = "Add an entry…", hint = "" }: Props = $props();

  let open = $state(false);
</script>

<div class="field-row">
  {#if label}
    <span class="field-label">{label}</span>
  {/if}
  <div style="display:flex; align-items:center; gap:0.6rem;">
    <span class="badge">{value.length} {value.length === 1 ? "entry" : "entries"}</span>
    <button class="small" onclick={() => (open = true)} aria-label={label ? `Edit ${label}` : "Edit list"}>Edit…</button>
  </div>
  {#if hint}
    <span class="field-hint">{hint}</span>
  {/if}
</div>

{#if open}
  <ListEditorModal
    title={label || "Edit list"}
    items={value}
    {placeholder}
    onSave={(items) => (value = items)}
    onClose={() => (open = false)}
  />
{/if}
