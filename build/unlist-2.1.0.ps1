# Unlist v2.1.0 across all Flags.Icons.* packages on nuget.org.
#
# v2.1.0 was an unintended release: it was supposed to ship as v3.0.0 but the
# `Release-As: 3.0.0` footer from the squash-merged commit on main ended up
# in the middle of the body (followed by Co-Authored-By: and then more commit
# blocks from the multi-commit squash), so release-please's footer parser
# didn't see it as a trailer and fell back to deriving the version from
# conventional-commit types (`feat:` → minor bump from 2.0.0).
#
# nuget.org doesn't allow hard delete; `nuget delete` actually performs an
# unlist — the package stays restorable for existing consumers but no longer
# appears in search results, the package page warns about unlisting, and new
# `dotnet add package` calls without a pinned version skip past it.
#
# Run once with your nuget.org API key:
#   .\build\unlist-2.1.0.ps1 -ApiKey YOUR_NUGET_API_KEY
#
# Dry run (prints what would happen, doesn't call the API):
#   .\build\unlist-2.1.0.ps1 -ApiKey YOUR_NUGET_API_KEY -WhatIf

param(
    [Parameter(Mandatory = $true)]
    [string]$ApiKey,

    [string]$Version = '2.1.0',

    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'

$packages = @(
    'Flags.Icons',
    'Flags.Icons.Avalonia',
    'Flags.Icons.Eto',
    'Flags.Icons.MAUI',
    'Flags.Icons.MewUI',
    'Flags.Icons.Uno',
    'Flags.Icons.WinForms',
    'Flags.Icons.WinUi',
    'Flags.Icons.WPF'
)

$source = 'https://api.nuget.org/v3/index.json'

# Track partial failures so a single unlist error fails the whole script —
# silent partial cleanups otherwise mask "we thought we unlisted everything
# but actually 3 packages are still listed" CI / manual-run regressions.
$failedPackages = @()

foreach ($pkg in $packages) {
    Write-Host "Unlisting $pkg $Version ..." -ForegroundColor Cyan
    # Use $nugetArgs (not $args) — $args is a PowerShell automatic variable
    # holding the script's own argument array; clobbering it breaks anyone
    # who later references it (and confuses linters).
    $nugetArgs = @('delete', $pkg, $Version, '-Source', $source, '-ApiKey', $ApiKey, '-NonInteractive')
    if ($WhatIf) {
        # Redact the API key before logging so the dry-run output is safe to
        # paste into issues / CI artefacts / pair-debugging sessions.
        $safeArgs = @()
        for ($i = 0; $i -lt $nugetArgs.Length; $i++) {
            if ($nugetArgs[$i] -eq '-ApiKey' -and ($i + 1) -lt $nugetArgs.Length) {
                $safeArgs += '-ApiKey'
                $safeArgs += '***'
                $i++
            } else {
                $safeArgs += $nugetArgs[$i]
            }
        }
        Write-Host "  [WhatIf] nuget $($safeArgs -join ' ')" -ForegroundColor Yellow
        continue
    }
    & nuget @nugetArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Failed (exit $LASTEXITCODE) — continuing with remaining packages." -ForegroundColor Red
        $failedPackages += $pkg
    }
}

if ($failedPackages.Count -gt 0) {
    Write-Host "`n$($failedPackages.Count) package(s) failed to unlist: $($failedPackages -join ', ')" -ForegroundColor Red
    throw "One or more packages failed to unlist — re-run for the failed entries above before considering v$Version cleaned up."
}

Write-Host "`nDone. Verify on nuget.org that $Version no longer appears in the package version list (it stays installable but hidden from search)." -ForegroundColor Green
