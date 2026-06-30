# Longbow Phase 1A MVP — Public Repository

**Status:** ✅ COMPLETE — Mod validator shipped

**Repository:** https://github.com/Vpirate12/ArmaReforgerServerTool (Public)

---

## What's Here

Phase 1A MVP only: **Mod Validator + Auto-fix**

- Source code: `src/`
- Tests: `Longbow.Tests/`
- Build output: `bin/Release/net8.0-windows/Longbow.exe`
- License: `LICENSE.md`

## Features

- Validator detects missing deps, version conflicts, circular deps
- Auto-fix adds missing mods, reorders for correct load order
- Steam Workshop integration (real mod metadata)
- Check Mods button with progress feedback
- Start button gating (RED=invalid, GREEN=valid)
- Save/load configurations

## For Internal Development

**All Phase 1B/2 work and strategic planning has moved to a private repository:**

- **Private repo:** https://github.com/Vpirate12/Sentinel-Desktop (internal use only)
- Contains: Full roadmap, monetization strategy, Sentinel Link plans, all docs
- Access: Aaron + core team only

**Session protocol:** New sessions should pull from the private repo for development work.

## Building & Running

```bash
# Build
dotnet build -c Release

# Run
bin/Release/net8.0-windows/Longbow.exe
```

## Bug Reports

Found a bug in the validator? Open an issue on this repo.

## No New Features Here

Phase 1A is complete. Future development (Phase 1B+) happens in the private repository.

---

**Last updated:** 2026-06-30  
**Phase 1A shipped:** 2026-06-29
