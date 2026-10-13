param(
    [string]$GamePath = "D:\SteamLibrary\steamapps\common\Last Epoch",
    [string]$OutputFile = ""
)
$ErrorActionPreference = "Stop"
$repoPath = Split-Path -Parent $PSScriptRoot
if (!$OutputFile) { $OutputFile = Join-Path $repoPath "Build\ItemNativeProbe.zip" }
$project = Join-Path $repoPath "tools\NativeItemProbe\NativeItemProbe.csproj"
dotnet run --project $project -c Release -- $GamePath $OutputFile
if ($LASTEXITCODE -ne 0) { throw "Native item probe failed; the game and installed mod were not modified." }
Write-Host "Attach this diagnostic archive: $OutputFile"
