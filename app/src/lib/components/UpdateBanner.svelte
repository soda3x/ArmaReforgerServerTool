<script lang="ts">
  import { onMount } from "svelte";
  import { check, type Update } from "@tauri-apps/plugin-updater";
  import { relaunch } from "@tauri-apps/plugin-process";

  let update: Update | null = $state(null);
  let status: "idle" | "downloading" | "installing" | "error" = $state("idle");
  let progress = $state(0);
  let errorMsg = $state("");

  onMount(() => {
    check()
      .then((found) => {
        update = found;
      })
      .catch(() => {
        // Silent: a failed background check (offline, GitHub unreachable, etc.) shouldn't
        // interrupt the user.
      });
  });

  async function onUpdate() {
    if (!update) return;
    status = "downloading";
    errorMsg = "";
    let downloaded = 0;
    let total = 0;
    try {
      await update.downloadAndInstall((event) => {
        switch (event.event) {
          case "Started":
            total = event.data.contentLength ?? 0;
            break;
          case "Progress":
            downloaded += event.data.chunkLength;
            progress = total > 0 ? Math.round((downloaded / total) * 100) : 0;
            break;
          case "Finished":
            status = "installing";
            break;
        }
      });
      await relaunch();
    } catch (e) {
      status = "error";
      errorMsg = e instanceof Error ? e.message : String(e);
    }
  }
</script>

{#if update}
  <div class="update-banner">
    <span>
      {#if status === "idle"}
        A newer version of Longbow is available: <strong>{update.version}</strong>
      {:else if status === "downloading"}
        Downloading update{progress > 0 ? ` (${progress}%)` : ""}…
      {:else if status === "installing"}
        Installing update, restarting…
      {:else}
        Update failed: {errorMsg}
      {/if}
    </span>
    {#if status === "idle" || status === "error"}
      <button onclick={onUpdate}>Update</button>
    {/if}
  </div>
{/if}

<style>
  .update-banner {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 1rem;
    padding: 0.5rem 1rem;
    background: var(--accent-dim);
    border-bottom: 1px solid var(--accent);
    color: var(--text);
    font-size: 0.9rem;
  }
  .update-banner button {
    background: var(--accent);
    border-color: var(--accent);
    color: #fff;
    padding: 0.25rem 0.9rem;
  }
  .update-banner button:hover {
    background: var(--accent-hover);
    border-color: var(--accent-hover);
  }
</style>
