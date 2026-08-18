// Classifies a server-log line by severity, purely from the markers the engine itself already
// writes into the line (e.g. "BACKEND   (E): ...", "SCRIPT    (W): ..."), plus the small set of
// prefixes Longbow's own emitted lines use. This is a display aid — colouring and filtering —
// not a parser anything else depends on, so a line that doesn't match any marker is just "info"
// rather than being treated as an error.

export type LogSeverity = "error" | "warning" | "hint" | "info";

export function classifyLogLine(line: string): LogSeverity {
  // The diagnostics catalog (services::diagnostics on the Rust side) emits its explanations as
  // "— Title — meaning fix", appended right after the timestamp — distinct enough from the
  // engine's own log format that it's safe to key off literally.
  if (/:\s+—\s/.test(line)) return "hint";
  if (line.includes("(E):")) return "error";
  if (line.includes("(W):")) return "warning";
  if (line.includes("Warning:")) return "warning";
  return "info";
}
