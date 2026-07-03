# Longbow Docs

Documentation for the Longbow (ArmaReforgerServerTool) Phase 1A public repository — the mod
validator for Arma Reforger dedicated servers.

## Index

- [OVERVIEW.md](./OVERVIEW.md) — what the project is, tech stack, build/run/test commands,
  directory map, current status.
- [ARCHITECTURE.md](./ARCHITECTURE.md) — module breakdown, key flows (mod validation, auto-fix,
  Steam Workshop metadata), fragile points.
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) — dated log of real bugs and issues found in this
  codebase, with symptom / root cause / remedy for each.
- [PARAMETERS.md](./PARAMETERS.md) — reference for server configuration parameters (pre-existing
  doc, not maintained by this pass).

## Living-docs convention

These docs are maintained the same way the code is: **when a change to the code affects
behavior, architecture, or setup, update the relevant doc in the same change** — not as a
follow-up, not "later." A PR that changes how mod validation works but doesn't touch
ARCHITECTURE.md is incomplete.

### When something breaks

Add an entry to [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) with Symptom / Root cause / Remedy,
dated the day it was found or fixed. See the template at the bottom of that file.

**Never delete a resolved entry.** Mark it `Resolved` and leave it in place — the history of
what broke and why is as valuable as the current state. Entries only ever move between statuses;
they don't disappear.

### Status labels

- **Open** — known issue, not yet fixed.
- **Monitoring** — a fix or mitigation is in place, but it hasn't been fully verified (e.g. no
  build environment available to confirm, or the fix is new and unproven).
- **Resolved** — confirmed fixed. Left in the log for history.

## Scope note

This repo is the **public Phase 1A MVP only** (see [../HANDOFF.md](../HANDOFF.md)). Docs here
describe only what's actually implemented in this codebase. Phase 1B+/roadmap/business/
monetization content does not belong here — that lives in the private Sentinel-Desktop repo and
is out of scope for this documentation set.
