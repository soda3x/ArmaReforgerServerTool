<script lang="ts">
  interface Props {
    label?: string;
    value: boolean;
    disabled?: boolean;
    hint?: string;
  }

  let { label = "", value = $bindable(false), disabled = false, hint = "" }: Props = $props();
</script>

<div class="field-row">
  <label class="toggle">
    <input type="checkbox" bind:checked={value} {disabled} />
    <span class="switch"></span>
    {#if label}<span>{label}</span>{/if}
  </label>
  {#if hint}
    <span class="field-hint">{hint}</span>
  {/if}
</div>
