param(
    [string]$GamePath = "D:\SteamLibrary\steamapps\common\Last Epoch",
    [switch]$ExportNativeProbe
)

$ErrorActionPreference = "Stop"
$branch = "fix/exulis-specific-corruption"
$repoPath = Split-Path -Parent $PSScriptRoot

function Assert-LastEpochExited {
    foreach ($gameProcess in @(Get-Process -Name "Last Epoch" -ErrorAction SilentlyContinue)) {
        try {
            $gameProcess.Refresh()
            # Windows can retain a process entry after it has exited.
            # Check termination state instead of treating the name as proof of life.
            if (!$gameProcess.HasExited) {
                throw "Close Last Epoch before installing the testing build. Active PID: $($gameProcess.Id)."
            }
        } finally {
            $gameProcess.Dispose()
        }
    }
}

Push-Location $repoPath
try {
    Assert-LastEpochExited
    if (!(Test-Path -LiteralPath (Join-Path $GamePath "Mods") -PathType Container)) {
        throw "The game Mods folder was not found. Check -GamePath."
    }
    $changes = git status --porcelain --untracked-files=no
    if ($LASTEXITCODE -ne 0) { throw "Cannot read repository status." }
    if ($changes) { throw "Commit or stash tracked changes before changing branches." }
    git fetch origin
    if ($LASTEXITCODE -ne 0) { throw "Fetch failed." }
    git show-ref --verify --quiet "refs/heads/$branch"
    if ($LASTEXITCODE -eq 0) {
        git switch $branch
    } else {
        git switch --track "origin/$branch"
    }
    if ($LASTEXITCODE -ne 0) { throw "Branch switch failed." }
    git pull --ff-only origin $branch
    if ($LASTEXITCODE -ne 0) { throw "Update failed." }
    $localHead = git rev-parse HEAD
    if ($LASTEXITCODE -ne 0) { throw "Cannot read the local revision." }
    $remoteHead = git rev-parse "origin/$branch"
    if ($LASTEXITCODE -ne 0 -or $localHead -ne $remoteHead) {
        throw "The local branch differs from origin; nothing was installed."
    }

    dotnet build .\LastEpoch_Hud\LastEpoch_Hud.csproj -c Release "-p:LastEpochPath=$GamePath"
    if ($LASTEXITCODE -ne 0) { throw "Build failed; installed DLL was not replaced." }
    $modDll = Join-Path $repoPath "Build\Release\net6.0\LastEpoch_Hud.dll"
    $previousGamePath = $env:LAST_EPOCH_PATH
    $previousModDll = $env:LAST_EPOCH_MOD_DLL
    try {
        $env:LAST_EPOCH_PATH = $GamePath
        $env:LAST_EPOCH_MOD_DLL = $modDll
        dotnet run --project .\LastEpoch_Hud.Tests
        if ($LASTEXITCODE -ne 0) { throw "Tests failed; installed DLL was not replaced." }
    } finally {
        $env:LAST_EPOCH_PATH = $previousGamePath
        $env:LAST_EPOCH_MOD_DLL = $previousModDll
    }
    if (!(Test-Path -LiteralPath $modDll -PathType Leaf)) {
        throw "Built DLL was not found; nothing was installed."
    }
    Assert-LastEpochExited
    $installedDll = Join-Path $GamePath "Mods\LastEpoch_Hud.dll"
    if (Test-Path -LiteralPath $installedDll) {
        $stamp = Get-Date -Format "yyyyMMdd_HHmmss"
        Copy-Item -LiteralPath $installedDll -Destination "$installedDll.before-exulis-$stamp"
    }
    Copy-Item -LiteralPath $modDll -Destination $installedDll -Force
    if ($ExportNativeProbe) {
        $nativeProbe = Join-Path $repoPath "Build\ItemNativeProbe.zip"
        $cppOutput = Join-Path $GamePath "MelonLoader\Dependencies\Il2CppAssemblyGenerator\Cpp2IL\cpp2il_out"
        if ((Test-Path -LiteralPath $cppOutput -PathType Container) -and
            (Test-Path -LiteralPath (Join-Path $GamePath "GameAssembly.dll") -PathType Leaf)) {
            dotnet run --project .\tools\NativeItemProbe\NativeItemProbe.csproj -c Release -- $GamePath $nativeProbe
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Attach $nativeProbe to diagnose item packing and tooltip visibility."
            } else {
                Write-Warning "Native probe export failed. The tested DLL was installed; run scripts/Export-ItemNativeProbe.ps1 separately."
            }
        } else {
            Write-Warning "Native probe needs GameAssembly.dll and cpp2il_out. Native item and tooltip investigation needs these files."
        }
    }
    Write-Host "Installed Exulis test build $localHead."
    Write-Host "Select Exulis: choose two distinct modifiers from its native pool, with independent tier/roll values."
    Write-Host "Confirm Corrupted stays on and LP/Weaver's Will stay disabled. Pick up, equip, change zones and relog; verify both modifiers and values."
    Write-Host "Test no added ordinary affixes, then add ordinary affixes in Legal/Illegal modes within the seven-affix saved limit."
    Write-Host "Illegal mode: test both native modifiers + four ordinary affixes + Sealed (seven total), including a T8 regular seal. Add Primordial too: Drop should disable with an 8/7 warning directly beneath it; remove one ordinary affix to return to seven."
    Write-Host "Regression: Unsated Rage and Withstand the Elements must retain their fixed T1/max-roll selectors; ordinary corruption on other items is unchanged."
} finally {
    Pop-Location
}
