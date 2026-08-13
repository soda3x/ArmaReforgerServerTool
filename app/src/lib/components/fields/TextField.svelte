<script lang="ts">
  interface Props {
    label?: string;
    value: string;
    placeholder?: string;
    hint?: string;
    disabled?: boolean;
    password?: boolean;
  }

  import { uid } from "../../uid";

  let { label = "", value = $bindable(""), placeholder = "", hint = "", disabled = false, password = false }: Props = $props();

  const id = uid("text");
</script>

<div class="field-row">
  {#if label}
    <label for={id}>{label}</label>
  {/if}
  {#if password}
    <input {id} type="password" bind:value {placeholder} {disabled} />
  {:else}
    <input {id} type="text" bind:value {placeholder} {disabled} />
  {/if}
  {#if hint}
    <span class="field-hint">{hint}</span>
  {/if}
</div>
