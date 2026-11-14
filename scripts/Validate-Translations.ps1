<#
.SYNOPSIS
    Validates one or all strings_<code>.json language packs against strings_en.json.

.DESCRIPTION
    A translator can hand-edit JSON without ever opening Visual Studio, which means the
    usual compiler safety net (missing member, type mismatch) never runs on their file.
    This script is that safety net. For each target language file it checks, against
    strings_en.json as the source of truth:

      1. Missing keys  - present in English, absent from the target. Longbow silently
         falls back to English for these, so this is a coverage gap, not a crash - but
         still worth flagging before a release.
      2. Extra keys    - present in the target but not in English. Usually a typo'd key
         (it will never be looked up) or a leftover from a renamed/removed English key.
      3. Placeholder mismatches - every {name} in the English value must appear, spelled
         and cased identically, in the target value (see Resources/Strings/README.md).
      4. Plural-form structure - a key that is a plural object in English
         ({"one": ..., "other": ...}) must stay a plural object in the target, must carry
         the categories its own meta.pluralRule requires (see the README's plural table),
         and each category's placeholders are checked the same way as (3).
      5. Type-suffix vocabulary - the "<type>" segment of every key (the part after the
         last '.') must be one of the closed set documented in i18n-system.md section 3.2
         and mirrored in the README. This mainly guards against a translator accidentally
         renaming a key while "fixing" it.
      6. meta completeness - code/name/nativeName present, pluralRule is one of the three
         known rules, and a soft warning if basedOnVersion is behind strings_en.json's.

    Warnings are reported but do not fail the run. Missing keys, extra keys, placeholder
    mismatches, bad plural structure, and unknown type suffixes do.

.PARAMETER TargetPath
    A single strings_<code>.json file to validate. If omitted, every strings_*.json file
    in the Resources/Strings folder is validated (strings_en.json is always skipped as a
    target - it's the reference, not something to compare against itself).

.PARAMETER StringsDirectory
    Where to look for strings_en.json and, when -TargetPath is omitted, every sibling
    strings_*.json. Defaults to the project's Resources/Strings folder.

.EXAMPLE
    ./scripts/Validate-Translations.ps1
    Validates every language pack next to strings_en.json.

.EXAMPLE
    ./scripts/Validate-Translations.ps1 -TargetPath ArmaReforgerServerTool/Resources/Strings/strings_pt-BR.json
    Validates a single in-progress translation.

.NOTES
    Exit code 0 = no errors (warnings may still be present). Exit code 1 = at least one
    error, suitable for a CI check on pull requests that touch a strings_*.json file.
#>

[CmdletBinding()]
param(
    [string]$TargetPath,
    [string]$StringsDirectory = (Join-Path $PSScriptRoot "..\ArmaReforgerServerTool\Resources\Strings")
)

$ErrorActionPreference = "Stop"

# Closed vocabulary for the "<type>" suffix of a key (<area>.<element>.<type>).
# Mirrors i18n-system.md section 3.2 and Resources/Strings/README.md - update all three
# together if a new type is ever introduced.
$knownTypes = @(
    "label", "button", "tooltip", "title", "tab", "message", "status",
    "option", "placeholder", "description", "column", "format"
)

# Category names a value's own pluralRule requires. Mirrors LocalisationManager's
# PluralRules class (Managers/LocalisationManager.cs) - both must stay in sync.
$pluralCategoriesByRule = @{
    "english" = @("one", "other")
    "romance" = @("one", "other")
    "none"    = @("other")
}

function Get-Placeholders {
    param([string]$Text)
    $matches = [System.Text.RegularExpressions.Regex]::Matches($Text, '\{([^{}]+)\}')
    $names = foreach ($m in $matches) { $m.Groups[1].Value }
    return @($names | Sort-Object -Unique)
}

function Test-SameSet {
    param([string[]]$A, [string[]]$B)
    $diffA = @($A | Where-Object { $B -notcontains $_ })
    $diffB = @($B | Where-Object { $A -notcontains $_ })
    return ($diffA.Count -eq 0 -and $diffB.Count -eq 0)
}

function Get-TypeSuffix {
    param([string]$Key)
    $idx = $Key.LastIndexOf(".")
    if ($idx -lt 0) { return $Key }
    return $Key.Substring($idx + 1)
}

function Test-LanguagePack {
    param(
        [string]$Path,
        $EnglishSource
    )

    $errors = [System.Collections.Generic.List[string]]::new()
    $warnings = [System.Collections.Generic.List[string]]::new()
    $fileName = Split-Path -Leaf $Path

    if (-not (Test-Path $Path)) {
        $errors.Add("File not found.")
        return [PSCustomObject]@{ File = $fileName; Errors = $errors; Warnings = $warnings }
    }

    try {
        $target = Get-Content -Raw -Path $Path | ConvertFrom-Json
    }
    catch {
        $errors.Add("Not valid JSON: $($_.Exception.Message)")
        return [PSCustomObject]@{ File = $fileName; Errors = $errors; Warnings = $warnings }
    }

    # --- meta ---
    if (-not $target.meta) {
        $errors.Add("Missing 'meta' block.")
    }
    else {
        foreach ($field in @("code", "name", "nativeName", "pluralRule")) {
            $value = $target.meta.$field
            if ([string]::IsNullOrWhiteSpace($value)) {
                $errors.Add("meta.$field is missing or empty.")
            }
        }

        $rule = $target.meta.pluralRule
        if ($rule -and -not $pluralCategoriesByRule.ContainsKey($rule)) {
            $known = ($pluralCategoriesByRule.Keys | Sort-Object) -join ", "
            $errors.Add("meta.pluralRule '$rule' is not one of the known rules ($known).")
            $rule = "english"  # fall back so the rest of the pass can still run
        }
        elseif (-not $rule) {
            $rule = "english"
        }

        if ($target.meta.basedOnVersion -and $EnglishSource.meta.basedOnVersion -and
            $target.meta.basedOnVersion -ne $EnglishSource.meta.basedOnVersion) {
            $warnings.Add("meta.basedOnVersion is '$($target.meta.basedOnVersion)', " +
                "but strings_en.json is now at '$($EnglishSource.meta.basedOnVersion)'. " +
                "Re-check for new or changed keys.")
        }
    }

    if (-not $target.strings) {
        $errors.Add("Missing 'strings' block.")
        return [PSCustomObject]@{ File = $fileName; Errors = $errors; Warnings = $warnings }
    }

    $requiredCategories = $pluralCategoriesByRule[$rule]

    $targetKeys = @{}
    foreach ($prop in $target.strings.PSObject.Properties) { $targetKeys[$prop.Name] = $prop.Value }

    $englishKeys = @{}
    foreach ($prop in $EnglishSource.strings.PSObject.Properties) { $englishKeys[$prop.Name] = $prop.Value }

    # --- missing / extra keys, and per-key checks ---
    foreach ($key in $englishKeys.Keys) {
        $typeSuffix = Get-TypeSuffix $key
        if ($knownTypes -notcontains $typeSuffix) {
            # Reference file itself has drifted from the closed vocabulary - flag once,
            # under the target's report, since that's where a human will see it.
            $warnings.Add("Key '$key' (in strings_en.json) has an unknown type suffix '.$typeSuffix'.")
        }

        if (-not $targetKeys.ContainsKey($key)) {
            $errors.Add("Missing key: $key")
            continue
        }

        $enValue = $englishKeys[$key]
        $targetValue = $targetKeys[$key]
        $enIsPlural = $enValue -is [System.Management.Automation.PSCustomObject]
        $targetIsPlural = $targetValue -is [System.Management.Automation.PSCustomObject]

        if ($enIsPlural -ne $targetIsPlural) {
            if ($enIsPlural) {
                $errors.Add("Key '$key' should be a plural object ({`"one`": ..., `"other`": ...}) but is a plain string.")
            }
            else {
                $errors.Add("Key '$key' should be a plain string but is a plural object.")
            }
            continue
        }

        if (-not $enIsPlural) {
            $enPlaceholders = Get-Placeholders $enValue
            $targetPlaceholders = Get-Placeholders $targetValue
            if (-not (Test-SameSet $enPlaceholders $targetPlaceholders)) {
                $errors.Add("Key '$key' placeholder mismatch: English has [$($enPlaceholders -join ', ')], " +
                    "translation has [$($targetPlaceholders -join ', ')].")
            }
            continue
        }

        # Plural object: union of English categories' placeholders is the expected set
        # for every category (they should all describe the same value, just different
        # grammatical forms of the surrounding sentence).
        $expectedPlaceholders = @()
        foreach ($prop in $enValue.PSObject.Properties) {
            $expectedPlaceholders += Get-Placeholders $prop.Value
        }
        $expectedPlaceholders = @($expectedPlaceholders | Sort-Object -Unique)

        $targetCategories = @($targetValue.PSObject.Properties | ForEach-Object { $_.Name })

        foreach ($category in $requiredCategories) {
            if ($targetCategories -notcontains $category) {
                $errors.Add("Key '$key' is missing required plural category '$category' for pluralRule '$rule'.")
                continue
            }
            $categoryText = $targetValue.$category
            $categoryPlaceholders = Get-Placeholders $categoryText
            if (-not (Test-SameSet $expectedPlaceholders $categoryPlaceholders)) {
                $errors.Add("Key '$key' category '$category' placeholder mismatch: expected " +
                    "[$($expectedPlaceholders -join ', ')], got [$($categoryPlaceholders -join ', ')].")
            }
        }

        foreach ($category in $targetCategories) {
            if ($requiredCategories -notcontains $category) {
                $warnings.Add("Key '$key' has plural category '$category', which pluralRule '$rule' never selects " +
                    "(it will always be ignored for this language).")
            }
        }
    }

    foreach ($key in $targetKeys.Keys) {
        if (-not $englishKeys.ContainsKey($key)) {
            $errors.Add("Extra key not present in strings_en.json: $key")
        }
    }

    return [PSCustomObject]@{ File = $fileName; Errors = $errors; Warnings = $warnings }
}

# --- entry point ---

$englishPath = Join-Path $StringsDirectory "strings_en.json"
if (-not (Test-Path $englishPath)) {
    throw "Reference file not found: $englishPath"
}
$englishSource = Get-Content -Raw -Path $englishPath | ConvertFrom-Json

if ($TargetPath) {
    $targets = @((Resolve-Path $TargetPath).Path)
}
else {
    $targets = Get-ChildItem -Path $StringsDirectory -Filter "strings_*.json" |
        Where-Object { $_.Name -ne "strings_en.json" } |
        ForEach-Object { $_.FullName }

    if ($targets.Count -eq 0) {
        Write-Host "No strings_*.json files found next to strings_en.json - nothing to validate."
        exit 0
    }
}

$hadErrors = $false

foreach ($target in $targets) {
    $result = Test-LanguagePack -Path $target -EnglishSource $englishSource

    Write-Host ""
    Write-Host "== $($result.File) =="

    if ($result.Errors.Count -eq 0 -and $result.Warnings.Count -eq 0) {
        Write-Host "  OK - no issues."
        continue
    }

    foreach ($err in $result.Errors) {
        Write-Host "  [ERROR] $err" -ForegroundColor Red
    }
    foreach ($warn in $result.Warnings) {
        Write-Host "  [WARN]  $warn" -ForegroundColor Yellow
    }

    if ($result.Errors.Count -gt 0) {
        $hadErrors = $true
    }
}

Write-Host ""

if ($hadErrors) {
    Write-Host "Validation failed." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "Validation passed (warnings, if any, are informational)." -ForegroundColor Green
    exit 0
}
