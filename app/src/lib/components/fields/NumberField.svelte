<script lang="ts">
  interface Props {
    label?: string;
    value: number;
    min?: number;
    max?: number;
    step?: number;
    hint?: string;
    disabled?: boolean;
  }

  import { uid } from "../../uid";

  let { label = "", value = $bindable(0), min, max, step = 1, hint = "", disabled = false }: Props = $props();

  const id = uid("num");

  function onInput(e: Event) {
    const raw = (e.target as HTMLInputElement).value;
    const n = raw === "" ? 0 : Number(raw);
    if (!Number.isNaN(n)) {
      value = n;
    }
  }

  function clamp() {
    if (min !== undefined && value < min) value = min;
    if (max !== undefined && value > max) value = max;
  }
</script>

<div class="field-row">
  {#if label}
    <label for={id}>{label}</label>
  {/if}
  <input {id} type="number" {min} {max} {step} {disabled} value={value} oninput={onInput} onblur={clamp} />
  {#if hint}
    <span class="field-hint">{hint}</span>
  {:else if min !== undefined && max !== undefined}
    <span class="field-hint">Range: {min} – {max}</span>
  {/if}
</div>
