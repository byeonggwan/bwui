# Builds the BWUI sample widget with the C# compiler that ships with Windows (.NET Framework 4.x).
#   .\build.ps1              build bin\BwuiDisk.exe
#   .\build.ps1 -Snapshots   also render docs\*.png and docs\contrast.txt
#   .\build.ps1 -Run         build and start the sample
param([switch]$Snapshots, [switch]$Run)
$ErrorActionPreference = 'Stop'
$fw = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319'
$out = Join-Path $PSScriptRoot 'bin'
New-Item -ItemType Directory -Force $out | Out-Null

$refs = 'WPF\PresentationFramework.dll', 'WPF\PresentationCore.dll', 'WPF\WindowsBase.dll', 'System.Xaml.dll' |
    ForEach-Object { "/r:$fw\$_" }
$src = @(Get-ChildItem "$PSScriptRoot\BWUI\*.cs") + @(Get-ChildItem "$PSScriptRoot\sample\*.cs") | ForEach-Object { $_.FullName }
$exe = Join-Path $out 'BwuiDisk.exe'

Get-Process BwuiDisk -ErrorAction SilentlyContinue | Stop-Process -Force
& "$fw\csc.exe" /nologo /codepage:65001 /optimize+ /target:winexe "/win32manifest:$PSScriptRoot\BWUI\bwui.manifest" "/out:$exe" @refs @src
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
Write-Host "Built $exe"

if ($Snapshots) {
    $docs = Join-Path $PSScriptRoot 'docs'
    New-Item -ItemType Directory -Force $docs | Out-Null
    $jobs = @(
        @('sample-light.png', 'light'), @('sample-dark.png', 'dark'),
        @('sample-rings-light.png', 'light', 'ring'), @('sample-rings-dark.png', 'dark', 'ring'),
        @('gallery-light.png', 'light', 'gallery'), @('gallery-dark.png', 'dark', 'gallery'))
    foreach ($j in $jobs) {
        $a = @('--snapshot', (Join-Path $docs $j[0])) + $j[1..($j.Count - 1)]
        Start-Process $exe -ArgumentList $a -Wait
    }
    Start-Process $exe -ArgumentList '--contrast', (Join-Path $docs 'contrast.txt') -Wait
    Write-Host "Snapshots written to $docs"
}
if ($Run) { Start-Process $exe }
