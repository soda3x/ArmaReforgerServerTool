#!/usr/bin/env bash
# PreToolUse hook on the Task tool = human-in-the-loop gate before a subagent starts.
#
# Policy: auto-allow the read-only advisory agents (architect, code-reviewer,
# qa-test-runner, security-auditor, docs-scribe) so the assembly line keeps moving,
# and ASK for approval before any agent that can write code, run commands, or deploy.
#
# To gate EVERY hand-off instead, see the note at the bottom.

input="$(cat)"

# Agents that can modify the repo or run commands — these get gated.
GATED='implementer|devops-pipeline'

if printf '%s' "$input" | grep -Eq "\"($GATED)\""; then
  # Force an approval prompt before this agent runs.
  cat <<'JSON'
{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"ask","permissionDecisionReason":"Write-capable agent about to start. Approve before it picks up the hand-off."}}
JSON
  exit 0
fi

# Advisory / read-only agents proceed without interruption.
exit 0

# ---- Variant: pause before EVERY agent ----
# Replace the whole if-block above with just the JSON heredoc + exit 0, so every
# Task delegation asks for approval regardless of which agent it is.
