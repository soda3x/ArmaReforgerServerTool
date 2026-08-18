<script lang="ts">
  import { onMount } from "svelte";
  import {
    type WorkshopAssetSummary,
    type WorkshopAssetDetail,
    type WorkshopDependency,
    type Mod,
    type ModLists,
    searchWorkshopMods,
    getWorkshopModDetails,
    addModsWithDependencies,
  } from "../../api";

  interface Props {
    onApplied: (lists: ModLists) => void;
    onClose: () => void;
  }

  let { onApplied, onClose }: Props = $props();

  // The API returns 16 rows/page (confirmed against the live site); used only to render "Page N
  // of ~M" — actual Prev/Next enablement below doesn't depend on this being exactly right.
  const PAGE_SIZE = 16;

  let query = $state("");
  let sort = $state<"popular" | "newest">("popular");
  let page = $state(1);
  let rows = $state<WorkshopAssetSummary[]>([]);
  let count = $state(0);
  let loading = $state(true);
  let errorMsg = $state("");

  // Detail/add-to-server view state.
  let selected = $state<WorkshopAssetDetail | null>(null);
  let detailLoading = $state(false);
  let detailError = $state("");
  let addBusy = $state(false);
  let addedIds = $state<Set<string>>(new Set());

  // Debounced search: a generation counter discards a response if a newer search has since
  // been issued (e.g. the user kept typing while the first request was still in flight).
  let searchGeneration = 0;
  let debounceHandle: ReturnType<typeof setTimeout> | undefined;

  function scheduleSearch(resetPage: boolean) {
    if (resetPage) page = 1;
    clearTimeout(debounceHandle);
    debounceHandle = setTimeout(runSearch, 350);
  }

  async function runSearch() {
    const generation = ++searchGeneration;
    loading = true;
    errorMsg = "";
    try {
      const result = await searchWorkshopMods(query.trim() || null, page, sort === "newest" ? "newest" : null);
      if (generation !== searchGeneration) return; // superseded by a newer search
      rows = result.rows;
      count = result.count;
    } catch (e) {
      if (generation !== searchGeneration) return;
      errorMsg = e instanceof Error ? e.message : String(e);
      rows = [];
      count = 0;
    } finally {
      if (generation === searchGeneration) loading = false;
    }
  }

  // Consistent with the rest of the app: mount-time fetches are deferred to onMount rather than
  // called at script top-level (see ConfigurationTab.svelte for why — a one-time webview-startup
  // IPC race). Not actually reachable this late (the modal only opens well after the app has
  // been running for a while), but there's no reason to be the one exception to the pattern.
  onMount(() => {
    runSearch();
  });

  function onQueryInput(value: string) {
    query = value;
    scheduleSearch(true);
  }

  function onSortChange(next: "popular" | "newest") {
    if (sort === next) return;
    sort = next;
    scheduleSearch(true);
  }

  function prevPage() {
    if (page <= 1 || loading) return;
    page -= 1;
    runSearch();
  }

  function nextPage() {
    if (loading || rows.length === 0) return;
    page += 1;
    runSearch();
  }

  async function openDetail(id: string) {
    detailLoading = true;
    detailError = "";
    selected = null;
    try {
      selected = await getWorkshopModDetails(id);
    } catch (e) {
      detailError = e instanceof Error ? e.message : String(e);
    } finally {
      detailLoading = false;
    }
  }

  function closeDetail() {
    selected = null;
    detailError = "";
  }

  // "latest" is the sentinel this app's Mod format uses for "no specific version pinned" (see
  // Mod::LATEST_MOD_VER_STR on the Rust side) — the same convention manually adding a mod uses.
  function toMod(id: string, name: string): Mod {
    return { modId: id, name, version: "latest", required: false };
  }

  async function addToServer(asset: WorkshopAssetDetail) {
    addBusy = true;
    detailError = "";
    try {
      const mods = [toMod(asset.id, asset.name), ...asset.dependencies.map((d) => toMod(d.id, d.name))];
      // One atomic backend call for the mod + its whole dependency chain (can be a dozen-plus
      // entries) rather than an addMod/enableMod round trip per mod — that would leave the set
      // partially added on any single failure, and had no single authoritative result to sync
      // the shared Available/Enabled stores from, so the lists wouldn't reliably reflect what
      // was actually added until (if ever) something else happened to re-fetch them.
      const lists = await addModsWithDependencies(mods);
      onApplied(lists);
      const next = new Set(addedIds);
      for (const mod of mods) next.add(mod.modId);
      addedIds = next;
    } catch (e) {
      detailError = e instanceof Error ? e.message : String(e);
    } finally {
      addBusy = false;
    }
  }

  function formatSize(bytes: number): string {
    if (bytes >= 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024 * 1024)).toFixed(2)} GB`;
    if (bytes >= 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    if (bytes >= 1024) return `${(bytes / 1024).toFixed(0)} KB`;
    return `${bytes} B`;
  }

  function formatRating(r: number): string {
    return `${Math.round(r * 100)}%`;
  }

  const totalPages = $derived(count > 0 ? Math.max(1, Math.ceil(count / PAGE_SIZE)) : 1);
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal" style="width:min(960px, 96vw); max-height:90vh;">
    <div class="modal-header">
      <h3>Browse Workshop</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>

    <div class="modal-body">
      {#if selected || detailLoading}
        <!-- Detail view -->
        <button class="small" onclick={closeDetail} style="margin-bottom:0.75rem;">← Back to results</button>

        {#if detailLoading}
          <p class="field-hint">Loading mod details…</p>
        {:else if detailError && !selected}
          <p style="color:var(--danger);">{detailError}</p>
        {:else if selected}
          <div style="display:flex; gap:1rem; flex-wrap:wrap;">
            {#if selected.previewUrls.length > 0}
              <img
                src={selected.previewUrls[0]}
                alt={selected.name}
                style="width:320px; max-width:100%; border-radius:var(--radius-sm); object-fit:cover; aspect-ratio:16/9;"
              />
            {/if}
            <div style="flex:1; min-width:240px;">
              <h2 style="margin:0 0 0.2rem 0;">{selected.name}</h2>
              <p class="field-hint" style="margin:0 0 0.5rem 0;">
                by {selected.authorUsername} · v{selected.currentVersionNumber} · {formatSize(selected.currentVersionSize)}
              </p>
              <div style="display:flex; gap:0.4rem; align-items:center; margin-bottom:0.6rem;">
                <span class="badge online">{formatRating(selected.averageRating)} rated</span>
                <span class="badge">{selected.subscriberCount.toLocaleString()} subscribers</span>
              </div>
              {#if selected.tags.length > 0}
                <div style="display:flex; gap:0.3rem; flex-wrap:wrap; margin-bottom:0.75rem;">
                  {#each selected.tags as tag (tag)}
                    <span class="badge">{tag}</span>
                  {/each}
                </div>
              {/if}
              <p class="field-hint" style="font-family:var(--font-mono); margin-bottom:0.75rem;">{selected.id}</p>

              {#if selected.dependencies.length > 0}
                <div style="margin-bottom:0.75rem;">
                  <div class="field-hint" style="margin-bottom:0.3rem;">
                    Requires {selected.dependencies.length}
                    {selected.dependencies.length === 1 ? "other mod" : "other mods"}:
                  </div>
                  <ul style="list-style:none; margin:0; padding:0; display:flex; flex-direction:column; gap:0.15rem;">
                    {#each selected.dependencies as dep (dep.id)}
                      <li style="display:flex; justify-content:space-between; gap:0.6rem; font-size:0.85rem;">
                        <span>{dep.name}</span>
                        <span class="field-hint">{formatSize(dep.totalFileSize)}</span>
                      </li>
                    {/each}
                  </ul>
                </div>
              {/if}

              {#if detailError}
                <p style="color:var(--danger);">{detailError}</p>
              {/if}

              {#if addedIds.has(selected.id)}
                <span class="badge online">Added to server ✓</span>
              {:else}
                <button class="primary" onclick={() => selected && addToServer(selected)} disabled={addBusy}>
                  {addBusy
                    ? "Adding…"
                    : selected.dependencies.length > 0
                      ? `Add mod + ${selected.dependencies.length} ${selected.dependencies.length === 1 ? "dependency" : "dependencies"}`
                      : "Add to server"}
                </button>
              {/if}
            </div>
          </div>

          {#if selected.summary}
            <p style="margin-top:1rem;">{selected.summary}</p>
          {/if}
          {#if selected.description}
            <p style="white-space:pre-wrap; margin-top:0.5rem;">{selected.description}</p>
          {/if}
          {#if selected.license}
            <p class="field-hint" style="margin-top:0.75rem;">License: {selected.license}</p>
          {/if}
        {/if}
      {:else}
        <!-- Search/browse view -->
        <div style="display:flex; gap:0.6rem; margin-bottom:0.75rem; flex-wrap:wrap;">
          <input
            type="text"
            placeholder="Search workshop mods…"
            value={query}
            oninput={(e) => onQueryInput((e.target as HTMLInputElement).value)}
            style="flex:1; min-width:200px;"
          />
          <div style="display:flex; gap:0.3rem;">
            <button class={sort === "popular" ? "primary small" : "small"} onclick={() => onSortChange("popular")}>Popular</button>
            <button class={sort === "newest" ? "primary small" : "small"} onclick={() => onSortChange("newest")}>Newest</button>
          </div>
        </div>

        {#if errorMsg}
          <p style="color:var(--danger);">{errorMsg}</p>
        {/if}

        {#if loading}
          <p class="field-hint">Loading…</p>
        {:else if rows.length === 0}
          <p class="field-hint">No mods found.</p>
        {:else}
          <div class="workshop-grid">
            {#each rows as asset (asset.id)}
              <button class="workshop-card" onclick={() => openDetail(asset.id)}>
                {#if asset.thumbnailUrl}
                  <img src={asset.thumbnailUrl} alt="" loading="lazy" />
                {:else}
                  <div class="workshop-card-noimg"></div>
                {/if}
                <div class="workshop-card-body">
                  <div class="workshop-card-title">{asset.name}</div>
                  <div class="field-hint">by {asset.authorUsername}</div>
                  <div class="field-hint">{formatRating(asset.averageRating)} · {formatSize(asset.currentVersionSize)}</div>
                </div>
                {#if addedIds.has(asset.id)}
                  <span class="badge online" style="position:absolute; top:0.4rem; right:0.4rem;">Added ✓</span>
                {/if}
              </button>
            {/each}
          </div>

          <div style="display:flex; align-items:center; justify-content:center; gap:1rem; margin-top:1rem;">
            <button class="small" onclick={prevPage} disabled={page <= 1 || loading}>Previous</button>
            <span class="field-hint">Page {page} of ~{totalPages} ({count.toLocaleString()} results)</span>
            <button class="small" onclick={nextPage} disabled={loading || rows.length === 0}>Next</button>
          </div>
        {/if}
      {/if}
    </div>

    <div class="modal-footer">
      <button onclick={onClose}>Close</button>
    </div>
  </div>
</div>

<style>
  .workshop-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 0.75rem;
  }

  .workshop-card {
    position: relative;
    display: flex;
    flex-direction: column;
    text-align: left;
    padding: 0;
    overflow: hidden;
    border: 1px solid var(--border);
    border-radius: var(--radius-sm);
    background: var(--bg-input);
  }

  .workshop-card:hover {
    border-color: var(--border-strong);
  }

  .workshop-card img,
  .workshop-card-noimg {
    width: 100%;
    aspect-ratio: 16 / 9;
    object-fit: cover;
    background: var(--bg-elevated);
  }

  .workshop-card-body {
    padding: 0.5rem 0.6rem;
    display: flex;
    flex-direction: column;
    gap: 0.1rem;
  }

  .workshop-card-title {
    font-weight: 600;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
</style>
