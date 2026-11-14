<#
.SYNOPSIS
    Generates a pseudo-localization pack (strings_qps.json) from strings_en.json.

.DESCRIPTION
    "qps" is not a real language - it exists to answer one question: is any user-facing
    text still hard-coded in English, bypassing the translation system entirely?

    Every string gets three treatments at once:
      1. Every letter is swapped for an accented look-alike, so any text that renders in
         plain ASCII English was never routed through Strings.Get() in the first place.
      2. The text is padded ~40% longer, simulating how much German (a realistic target
         language) tends to expand relative to English - this surfaces layout that will
         truncate or overlap before a human translator ever touches it.
      3. The whole string is wrapped in [ ]. If the closing ] is cut off on screen, that
         control's text is being clipped.

    Named placeholders ({count}, {path}, ...) and dictionary keys are left untouched -
    only translatable values are transformed. Never edit strings_qps.json by hand; run
    this script again after strings_en.json changes.

    Accented characters are built from Unicode code points rather than typed literally,
    so this file parses correctly regardless of which codepage a given PowerShell host
    assumes for .ps1 source (PowerShell 5.1 does not reliably auto-detect UTF-8 without
    a BOM).

.PARAMETER SourcePath
    Path to strings_en.json. Defaults to the project's Resources/Strings folder.

.PARAMETER OutputPath
    Where to write strings_qps.json. Defaults alongside the source file.

.EXAMPLE
    ./scripts/Generate-PseudoLocale.ps1
#>

[CmdletBinding()]
param(
    [string]$SourcePath = (Join-Path $PSScriptRoot "..\ArmaReforgerServerTool\Resources\Strings\strings_en.json"),
    [string]$OutputPath = (Join-Path $PSScriptRoot "..\ArmaReforgerServerTool\Resources\Strings\strings_qps.json")
)

$ErrorActionPreference = "Stop"

function U {
    param([int]$CodePoint)
    return [char]::ConvertFromUtf32($CodePoint)
}

# Accented look-alike for every ASCII letter, keyed by code point (not by a literal
# character) - see the encoding note above. Deliberately covers consonants too, not just
# vowels: a pseudo-locale that only touches vowels lets whole consonant-heavy words slip
# through looking "close enough" to English at a glance.
$charMap = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
$pairs = @(
    @('a', 0x00E0), @('b', 0x00DF), @('c', 0x00E7), @('d', 0x00F0), @('e', 0x00E9)
    @('f', 0x0192), @('g', 0x011F), @('h', 0x0125), @('i', 0x00EE), @('j', 0x0135)
    @('k', 0x0137), @('l', 0x013C), @('m', 0x0271), @('n', 0x00F1), @('o', 0x00F6)
    @('p', 0x00FE), @('r', 0x0159), @('s', 0x0161), @('t', 0x0163)
    @('u', 0x00FC), @('v', 0x1E7D), @('w', 0x0175), @('y', 0x00FD), @('z', 0x017E)
    @('A', 0x00C4), @('B', 0x0181), @('C', 0x00C7), @('D', 0x00D0), @('E', 0x00C9)
    @('F', 0x0191), @('G', 0x011E), @('H', 0x0124), @('I', 0x00CE), @('J', 0x0134)
    @('K', 0x0136), @('L', 0x013B), @('M', 0x1E40), @('N', 0x00D1), @('O', 0x00D6)
    @('P', 0x00DE), @('R', 0x0158), @('S', 0x0160), @('T', 0x0162)
    @('U', 0x00DC), @('V', 0x1E7C), @('W', 0x0174), @('Y', 0x00DD), @('Z', 0x017D)
)
foreach ($pair in $pairs) {
    $charMap[$pair[0]] = U $pair[1]
}

# Padding filler, cycled to whatever length is needed - deliberately NOT plain ASCII,
# so it also fails the "did this actually route through Strings.Get" check.
$fillerChars = @((U 0x00C4), (U 0x00E4), (U 0x00D6), (U 0x00F6), (U 0x00DC), (U 0x00FC), (U 0x00DE), (U 0x00DF))

function ConvertTo-PseudoText {
    param([string]$Text)

    if ([string]::IsNullOrEmpty($Text)) {
        return $Text
    }

    # Split on {placeholder} tokens, transforming everything EXCEPT the tokens themselves.
    $pattern = [char]0x7B + '[^{}]+' + [char]0x7D  # "{[^{}]+}" built without literal braces,
                                                     # since PowerShell treats bare { } as script blocks in some contexts
    $tokenPattern = '(' + $pattern + ')'
    $segments = [System.Text.RegularExpressions.Regex]::Split($Text, $tokenPattern)
    $wholeTokenRegex = [System.Text.RegularExpressions.Regex]::new('^' + $pattern + '$')

    $transformed = [System.Text.StringBuilder]::new()

    foreach ($segment in $segments) {
        if ($wholeTokenRegex.IsMatch($segment)) {
            [void]$transformed.Append($segment)
            continue
        }
        foreach ($ch in $segment.GetEnumerator()) {
            $key = [string]$ch
            if ($charMap.ContainsKey($key)) {
                [void]$transformed.Append($charMap[$key])
            }
            else {
                [void]$transformed.Append($ch)
            }
        }
    }

    $result = $transformed.ToString()

    # Pad ~40% longer, measured on the untransformed text so short and long strings both
    # expand proportionally rather than by a fixed amount.
    $padLength = [Math]::Max(1, [Math]::Ceiling($Text.Length * 0.4))
    $pad = [System.Text.StringBuilder]::new()
    for ($i = 0; $i -lt $padLength; $i++) {
        [void]$pad.Append($fillerChars[$i % $fillerChars.Length])
    }

    return '[' + $result + ' ' + $pad.ToString() + ']'
}

function ConvertTo-PseudoValue {
    param($Value)

    if ($Value -is [string]) {
        return ConvertTo-PseudoText $Value
    }

    # Plural-form object: {"one": "...", "other": "..."} - transform each category's text,
    # keep the category names (they're not user-facing) untouched.
    if ($Value -is [System.Management.Automation.PSCustomObject]) {
        $out = [ordered]@{}
        foreach ($prop in $Value.PSObject.Properties) {
            $out[$prop.Name] = ConvertTo-PseudoText $prop.Value
        }
        return [PSCustomObject]$out
    }

    return $Value
}

if (-not (Test-Path $SourcePath)) {
    throw "Source file not found: $SourcePath"
}

$source = Get-Content -Raw -Path $SourcePath | ConvertFrom-Json

$outStrings = [ordered]@{}
foreach ($prop in $source.strings.PSObject.Properties) {
    $outStrings[$prop.Name] = ConvertTo-PseudoValue $prop.Value
}

$output = [ordered]@{
    meta    = [ordered]@{
        code            = "qps"
        name            = "Pseudo (QA)"
        nativeName      = '[' + (U 0x00DE) + (U 0x0161) + (U 0x00E9) + (U 0x00FB) + (U 0x00F0) + (U 0x00F6) + ']'
        pluralRule      = $source.meta.pluralRule
        contributors    = @("i18n tooling")
        basedOnVersion  = $source.meta.basedOnVersion
    }
    strings = $outStrings
}

# ConvertTo-Json escapes non-ASCII by default unless told not to; keep the accented
# characters literal in the file so it's readable/diffable, not \uXXXX noise.
$json = $output | ConvertTo-Json -Depth 10
$json = [System.Text.RegularExpressions.Regex]::Replace($json, '\\u([0-9a-fA-F]{4})', {
    param($m) [char]::ConvertFromUtf32([Convert]::ToInt32($m.Groups[1].Value, 16))
})

# UTF-8 without BOM, matching strings_en.json.
[System.IO.File]::WriteAllText($OutputPath, $json, [System.Text.UTF8Encoding]::new($false))

Write-Host "Wrote $($outStrings.Count) pseudo-localized keys to $OutputPath"
