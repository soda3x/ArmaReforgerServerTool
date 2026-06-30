#!/usr/bin/env bash
# SessionStart hook: inject a recap of the previous session so Claude resumes with
# context already in hand. Pulls the recent agent hand-off log and the current
# HANDOFF.md state. Phrased as factual state (not instructions) so it reads as context.
#
# Note: since Claude Code 2.1.0 this lands silently in context, not as a printed banner.
# Open the session and ask "where did we leave off?" — Claude answers from this.

LOG="$CLAUDE_PROJECT_DIR/.claude/handoff-log.txt"
HANDOFF="$CLAUDE_PROJECT_DIR/HANDOFF.md"

recap="Recent team activity from previous sessions (most recent agent hand-offs):"
if [ -f "$LOG" ]; then
  recap="$recap"$'\n'"$(tail -n 12 "$LOG")"
else
  recap="$recap"$'\n'"(no hand-off log yet — first run)"
fi

if [ -f "$HANDOFF" ]; then
  recap="$recap"$'\n\n'"Current HANDOFF.md state (in progress / next steps):"$'\n'"$(sed -n '1,40p' "$HANDOFF")"
fi

# SessionStart injects stdout into context. Keep it bounded (10k char cap applies).
printf '%s\n' "$recap"
exit 0
