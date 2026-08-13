<script lang="ts">
  import TextBlockModal from "../modals/TextBlockModal.svelte";

  // `value` is an arbitrary JSON value (object/array/etc), edited as pretty-printed text.
  interface Props {
    label?: string;
    value: unknown;
    hint?: string;
    onChange: (value: unknown) => void;
  }

  let { label = "", value, hint = "", onChange }: Props = $props();

  let open = $state(false);

  function text(): string {
    try {
      return JSON.stringify(value, null, 2);
    } catch {
      return "{}";
    }
  }
</script>

<div class="field-row">
  {#if label}
    <span class="field-label">{label}</span>
  {/if}
  <div>
    <button class="small" onclick={() => (open = true)} aria-label={label ? `Edit ${label}` : "Edit JSON"}>Edit JSON…</button>
  </div>
  {#if hint}
    <span class="field-hint">{hint}</span>
  {/if}
</div>

{#if open}
  <TextBlockModal
    title={label || "Edit JSON"}
    text={text()}
    onSave={(t) => {
      try {
        onChange(JSON.parse(t));
      } catch {
        // TextBlockModal already validates before calling onSave; this is a defensive no-op.
      }
    }}
    onClose={() => (open = false)}
  />
{/if}
