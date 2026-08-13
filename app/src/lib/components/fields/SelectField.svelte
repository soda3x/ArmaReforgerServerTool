<script lang="ts">
  interface Option {
    value: string;
    label: string;
  }

  interface Props {
    label?: string;
    value: string;
    options: Option[];
    disabled?: boolean;
    hint?: string;
  }

  import { uid } from "../../uid";

  let { label = "", value = $bindable(""), options, disabled = false, hint = "" }: Props = $props();

  const id = uid("select");
</script>

<div class="field-row">
  {#if label}
    <label for={id}>{label}</label>
  {/if}
  <select {id} bind:value {disabled}>
    {#each options as opt (opt.value)}
      <option value={opt.value}>{opt.label}</option>
    {/each}
  </select>
  {#if hint}
    <span class="field-hint">{hint}</span>
  {/if}
</div>
