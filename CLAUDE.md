# CLAUDE.md

## Bug Reporting Workflow

This repo is monitored by an automated "bug-smashing" session that runs on a recurring schedule (aligned to the maintainer's usage-reset window, roughly every 5 hours).

**To report a bug for automatic triage:**
1. Open a GitHub issue describing the bug.
2. Label it `bug`.

**What happens next (automatic, no action needed from you):**
1. On its next run, the bug-smashing session picks up any open `bug`-labeled issue that isn't already labeled `claude-working`.
2. It labels the issue `claude-working` and comments that it's picked it up.
3. It investigates and implements a fix on the `claude/bug-triage-workflow-kkqq1k` branch.
4. It opens a PR against the default branch with `Fixes #<issue-number>` in the description, so merging the PR auto-closes the issue.
5. It comments on the issue summarizing the fix and linking the PR.
6. It watches the PR for CI failures and review feedback until it's mergeable.

If a bug needs a human judgment call, the bug-smashing session will flag it directly instead of guessing.
