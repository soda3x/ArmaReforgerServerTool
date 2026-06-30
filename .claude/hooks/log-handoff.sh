#!/usr/bin/env bash
# SubagentStop hook = audit trail. Records each agent hand-off as it completes,
# so you can see the assembly line run end to end (and review it later).
# No jq dependency — parses the agent name with grep/sed so it runs on a bare box.

input="$(cat)"

agent="$(printf '%s' "$input" \
  | grep -oE '"agent_type"[[:space:]]*:[[:space:]]*"[^"]*"' \
  | sed -E 's/.*"agent_type"[[:space:]]*:[[:space:]]*"([^"]*)".*/\1/')"
[ -z "$agent" ] && agent="unknown"

echo "[$(date '+%Y-%m-%d %H:%M:%S')] subagent finished: ${agent}" \
  >> "$CLAUDE_PROJECT_DIR/.claude/handoff-log.txt"
exit 0
