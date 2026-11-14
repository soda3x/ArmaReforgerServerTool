# Translating Longbow

Longbow's UI text lives in the JSON files in this folder, separate from the compiled program.
Adding a language means adding one file here — no Visual Studio, no build.

## Quick start

1. Copy `strings_en.json` to `strings_<code>.json`, using your language's
   [BCP-47](https://en.wikipedia.org/wiki/IETF_language_tag) code (`pt`, `pt-BR`, `fr`, `de`...).
2. Fill in `meta` and translate every value under `strings`. Keys stay in English — don't
   rename, add, or remove them.
3. Run `Validate-Translations.ps1` against your file (see below).
4. Send the file in.

A missing key just falls back to English for that string — it won't crash the app.

## `meta`

```json
{
  "meta": {
    "code": "pt-BR",
    "name": "Portuguese (Brazil)",
    "nativeName": "Português (Brasil)",
    "pluralRule": "english",
    "contributors": ["your name or handle"],
    "basedOnVersion": "1.3.0"
  },
  "strings": { ... }
}
```

- `code` — BCP-47 tag. Also the file name and the value saved in `properties.json`.
- `name` — shown to English speakers in the language picker.
- `nativeName` — shown to speakers of your language, e.g. "Português (Brasil)".
- `pluralRule` — `english`, `romance`, or `none`. See **Plurals** below.
- `contributors` — your name/handle.
- `basedOnVersion` — Longbow version (`version.txt`) your translation matches.

## Rules

- Don't touch the keys, only the values. Renaming a key breaks the lookup silently — you get
  English back, no error.
- Keep every `{placeholder}` — same spelling, same case. They're replaced by literal text
  match, so `{count}` and `{Count}` are different to the code. Reorder them in the sentence if
  your grammar needs it, never translate the name inside the braces.
- Line breaks are `\r\n`, not `\n`.
- Don't translate scenario/mission names, file paths, URLs, version numbers, or raw exception
  text from .NET/the game server.
- Same English word in two different keys doesn't mean same translation — that's usually why
  they're separate keys (gender, formality, verb vs. noun). Translate each on its own.

## Key naming (reference, you don't write these)

`<area>.<element>.<type>`:

| `<type>` | What it is |
|---|---|
| `.label` | static label next to a field |
| `.button` | button text |
| `.tooltip` | hover tooltip |
| `.title` | window / group box / dialog title |
| `.tab` | tab name |
| `.message` | dialog body / message box |
| `.status` | status-bar text, updated live |
| `.option` | combo box or list item |
| `.placeholder` | ghost text in an empty field |
| `.description` | permanent explanatory text under a control (always visible, not a tooltip) |
| `.column` | data grid column header |
| `.format` | template string with more than one `{placeholder}` |

## Plurals

A few values are an object instead of a string, because the count changes the grammar:

```json
"main.playerCount.status": {
  "one": "{count} connected player",
  "other": "{count} connected players"
}
```

| `pluralRule` | Categories | Example languages |
|---|---|---|
| `english` | `one` (exactly 1), `other` (everything else) | English, German, Portuguese |
| `romance` | `one` (0 and 1), `other` (everything else) | French |
| `none` | `other` only | Japanese, Korean, Chinese |

Fill in only the categories your rule uses. Missing one falls back to English for that count.

## Validating

```powershell
./scripts/Validate-Translations.ps1 -TargetPath ArmaReforgerServerTool/Resources/Strings/strings_pt-BR.json
```

Checks missing/extra keys, placeholder mismatches, plural category coverage, and `meta`. Omit
`-TargetPath` to check every `strings_*.json` at once.

`scripts/Generate-PseudoLocale.ps1` builds `strings_qps.json`, a fake language that inflates
English ~40% and wraps it in `[ ]` — useful if you want to see whether a long translation fits
before submitting it. Layout fixes aren't your job, but flagging where your language runs long
in the PR helps.
