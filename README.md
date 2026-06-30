# Longbow — Arma Reforger Mod Validator (Phase 1A MVP)

**The only mod validator you need.** Stop digging through logs. Stop guessing if your mods will work. Longbow tells you exactly what's wrong and fixes it automatically.

## Features

### Core (Phase 1A)
- ✅ **Mod Validator** — Detects missing dependencies, version conflicts, circular dependencies in seconds
- ✅ **Auto-fix** — Adds missing mods, reorders for correct load order automatically
- ✅ **Progress feedback** — See what's happening as validation runs
- ✅ **Steam Workshop integration** — Fetches real mod metadata (no guessing)
- ✅ **Save/load configs** — Keep your configurations, switch between them
- ✅ **Start button gating** — RED/disabled if invalid, GREEN/enabled if ready to launch
- ✅ **Results display** — Understand exactly what was fixed and why

### What it solves
- No more `"Mod X not found"` after 10 minutes of gameplay
- No more `"Mod Y requires version Z but you have W"`
- No more `"Why does my mod order matter?"` — it's handled
- No more log hunting to find the real error

## Requirements

- [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download)
- Arma Reforger Dedicated Server (any recent version)

## Building

```bash
# Build with Visual Studio 2022 (Release mode)
# Or from command line:
dotnet build -c Release
```

Output: `bin/Release/net8.0-windows/Longbow.exe`

## Contributing

Phase 1A is complete and shipped. If you have bug reports or feature requests, please open an issue on GitHub.

## License

See LICENSE.md

## Support

- **Community:** Discord (link in repo)
- **Bugs:** GitHub Issues
- **Feature requests:** Discussion forum
