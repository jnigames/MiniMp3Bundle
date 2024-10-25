param (
    [Parameter(Mandatory=$true)] [string]$SourceFile,
    [Parameter(Mandatory=$true)] [string]$OutFile,
    [Parameter(Mandatory=$true)] [ValidateSet("32", "64")] [string]$Arch
)

$outputDir = "build"
if (-not (Test-Path $outputDir)) {
    New-Item -Path $outputDir -ItemType Directory | Out-Null
}

$vsWherePath = Join-Path -Path ([Environment]::GetEnvironmentVariable("ProgramFiles(x86)")) -ChildPath "Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vsWherePath)) {
    Write-Host "vswhere not found! Ensure Visual studio is installed."
    exit 1
}

$vsInstallPath = & $vsWherePath -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
if (-not $vsInstallPath) {
    Write-Host "Visual Studio Installation not found!"
    exit 1
}

if ($Arch -eq "64") {
    $vcvarsPath = "$vsInstallPath\VC\Auxiliary\Build\vcvars64.bat"
} elseif ($Arch -eq "32") {
    $vcvarsPath = "$vsInstallPath\VC\Auxiliary\Build\vcvars32.bat"    
}

$objFile = Join-Path $outputDir ([System.IO.Path]::GetFileNameWithoutExtension($SourceFile) + ".obj")
$clCmd = "cl /LD /O2 /DNDEBUG $($SourceFile) /Fo:$($objFile) /Fe:build\$($OutFile)"
function Run-CommandInEnv {
    param (
        [string]$vcvarsPath,
        [string]$command
    )

    $cmd = "`"$vcvarsPath`" && $command"
    $process = Start-Process -FilePath "cmd.exe" -ArgumentList "/c $cmd" -NoNewWindow -Wait -PassThru
    return $process.ExitCode
}

Write-Host "Building $Arch-bit $OutFile..."
$res = Run-CommandInEnv -vcvarsPath $vcvarsPath -command $clCmd
if ($res -ne 0) {
    Write-Host "Build failed!"
    exit 1
}
