<script lang="ts">
  import { onMount } from "svelte";
  import {
    type Diagnostic,
    type CommonIssue,
    listDiagnostics,
    listCommonIssues,
  } from "../../api";

  interface Props {
    onClose: () => void;
  }

  let { onClose }: Props = $props();

  let diagnostics = $state<Diagnostic[]>([]);
  let issues = $state<CommonIssue[]>([]);
  let loading = $state(true);
  let errorMsg = $state("");
  let tab = $state<"issues" | "errors">("issues");

  onMount(() => {
    load();
  });

  async function load() {
    loading = true;
    errorMsg = "";
    try {
      [diagnostics, issues] = await Promise.all([listDiagnostics(), listCommonIssues()]);
    } catch (e) {
      errorMsg = e instanceof Error ? e.message : String(e);
    } finally {
      loading = false;
    }
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal" style="width:min(820px, 96vw); max-height:90vh;">
    <div class="modal-header">
      <h3>Troubleshooting</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>

    <div class="modal-body">
      {#if errorMsg}
        <p style="color:var(--danger);">{errorMsg}</p>
      {/if}

      <div style="display:flex; gap:0.3rem; margin-bottom:0.9rem;">
        <button class={tab === "issues" ? "small primary" : "small"} onclick={() => (tab = "issues")}>
          Common problems
        </button>
        <button class={tab === "errors" ? "small primary" : "small"} onclick={() => (tab = "errors")}>
          Error messages
        </button>
      </div>

      {#if loading}
        <p class="field-hint">Loading…</p>
      {:else if tab === "issues"}
        <p class="field-hint" style="margin-top:0;">
          Problems that leave no error in the log — the server runs, but something still isn't right.
        </p>
        <ul style="list-style:none; margin:0; padding:0; display:flex; flex-direction:column; gap:0.9rem;">
          {#each issues as issue (issue.id)}
            <li>
              <strong>{issue.symptom}</strong>
              <ul style="margin:0.35rem 0 0 0; padding-left:1.1rem; display:flex; flex-direction:column; gap:0.25rem;">
                {#each issue.causes as cause (cause)}
                  <li class="field-hint">{cause}</li>
                {/each}
              </ul>
            </li>
          {/each}
        </ul>
      {:else}
        <p class="field-hint" style="margin-top:0;">
          Errors Longbow recognises in the server log. When one of these appears, the explanation
          below is added to the log automatically.
        </p>
        <ul style="list-style:none; margin:0; padding:0; display:flex; flex-direction:column; gap:0.9rem;">
          {#each diagnostics as d (d.id)}
            <li>
              <strong>{d.title}</strong>
              <div class="field-hint" style="margin-top:0.2rem;">{d.meaning}</div>
              <div style="margin-top:0.3rem;">{d.fix}</div>
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
