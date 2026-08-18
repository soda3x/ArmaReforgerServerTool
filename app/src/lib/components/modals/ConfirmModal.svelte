<script lang="ts">
  interface Props {
    title: string;
    body: string;
    confirmLabel?: string;
    danger?: boolean;
    onConfirm: () => void;
    onClose: () => void;
  }

  let { title, body, confirmLabel = "Confirm", danger = false, onConfirm, onClose }: Props = $props();

  function confirm() {
    onConfirm();
    onClose();
  }
</script>

<div class="modal-backdrop" role="presentation" onmousedown={(e) => e.target === e.currentTarget && onClose()}>
  <div class="modal" style="width:min(480px, 96vw);">
    <div class="modal-header">
      <h3>{title}</h3>
      <button class="icon-btn" onclick={onClose} aria-label="Close">✕</button>
    </div>

    <div class="modal-body">
      <p style="margin:0;">{body}</p>
    </div>

    <div class="modal-footer">
      <button onclick={onClose}>Cancel</button>
      <button class={danger ? "danger" : "primary"} onclick={confirm}>{confirmLabel}</button>
    </div>
  </div>
</div>
