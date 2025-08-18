# Fix all .md files to be valid UTF-8 and normalize smart punctuation to ASCII.
# Run from repo root or call via the companion .bat file.

param(
    [string]$Root
)

$ErrorActionPreference = 'Stop'

if (-not $Root -or -not (Test-Path -LiteralPath $Root)) {
    $Root = $PSScriptRoot
}

Write-Host "===== Normalizing all Markdown (*.md) files to UTF-8 (no BOM) ====="

$files = Get-ChildItem -LiteralPath $Root -Recurse -Filter *.md -File
if (-not $files) {
    Write-Host 'No .md files found.'
    exit 0
}

$utf8Strict = New-Object System.Text.UTF8Encoding($false, $true)
$utf8NoBom  = New-Object System.Text.UTF8Encoding($false)

foreach ($f in $files) {
    try {
        $bytes = [System.IO.File]::ReadAllBytes($f.FullName)
    } catch {
        Write-Warning ("Skip (read failed): {0}" -f $f.FullName)
        continue
    }

    try {
        $text = $utf8Strict.GetString($bytes)
    } catch {
        # Fallback for legacy encodings (e.g., Windows-1252)
        $text = [System.Text.Encoding]::GetEncoding(1252).GetString($bytes)
    }

    # Normalize punctuation and invisible characters using .NET string Replace
    $text = $text.Replace([char]0x2018, "'")  # left single quote
    $text = $text.Replace([char]0x2019, "'")  # right single quote
    $text = $text.Replace([char]0x201C, '"')  # left double quote
    $text = $text.Replace([char]0x201D, '"')  # right double quote
    $text = $text.Replace([char]0x2013, '-')   # en dash
    $text = $text.Replace([char]0x2014, '-')   # em dash
    $text = $text.Replace([char]0x00A0, ' ')   # non-breaking space
    $text = $text.Replace([char]0x2022, '-')   # bullet
    $text = $text.Replace([string][char]0x200B, '')    # zero-width space -> remove
    $text = $text.Replace([string][char]0xFEFF, '')    # BOM char -> remove
    $text = $text.Replace([char]0xFFFD, '?')   # replacement char

    try { [System.IO.File]::Copy($f.FullName, $f.FullName + '.bak', $true) } catch {}

    try {
        [System.IO.File]::WriteAllText($f.FullName, $text, $utf8NoBom)
        Write-Host ("Fixed: {0}" -f $f.FullName)
    } catch {
        Write-Error ("Write failed: {0} -> {1}" -f $f.FullName, $_.Exception.Message)
    }
}

Write-Host 'Done. All Markdown files normalized to UTF-8.'
