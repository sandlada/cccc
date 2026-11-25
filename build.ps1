param(
    [string]$Configuration = 'Debug',
    [string]$FontDirectory = (Join-Path $PSScriptRoot 'assets/fonts'),
    [string]$OutputDirectory = (Join-Path $PSScriptRoot 'artifacts/build'),
    [string]$PackageDirectory = (Join-Path $PSScriptRoot 'artifacts/release'),
    [switch]$Package,
    [switch]$SelfContained,
    [switch]$CheckQuality,
    [switch]$CheckAnimation,
    [switch]$CheckTaskbar,
    [switch]$CheckMaterial,
    [switch]$CheckShape,
    [switch]$Uninstall,
    [switch]$Deep
)
$ErrorActionPreference = 'Stop'

# Builds the .NET 10 solution and copies the complete Cccc.Desktop output (Cccc.exe
# plus every dependent DLL and runtime file) to the output directory. Optional
# checks run the test projects and the offscreen presentation check executable.
$solution = Join-Path $PSScriptRoot 'Cccc.sln'
$targetFramework = 'net10.0-windows10.0.19041.0'
$checksExecutable = Join-Path $PSScriptRoot ('Cccc.Presentation.Verification/bin/' + $Configuration + '/' + $targetFramework + '/Cccc.Presentation.Verification.exe')

# Runtime identity that keeps this fork separate from the upstream CCCC install.
# Must match Cccc.Desktop/ProductIdentity.cs and RegistryStartupStore.EntryName.
# Windows treats registry value names case-insensitively, so a case-only rename
# would still collide with upstream's "CCCC" value.
$productIdentity = 'Sandlada.Cccc'

function Get-FullPath([string]$Path) {
    if ([System.IO.Path]::IsPathRooted($Path)) { return [System.IO.Path]::GetFullPath($Path) }
    return [System.IO.Path]::GetFullPath((Join-Path (Get-Location).Path $Path))
}

function Invoke-Executable([string]$FilePath, [string[]]$Arguments) {
    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) { throw ('{0} failed with exit code {1}.' -f $FilePath, $LASTEXITCODE) }
}

# Refuses paths that are too shallow to delete safely: a drive root, a single
# top-level folder, or the repository itself and its parents.
function Assert-SafeRemovalPath([string]$Path) {
    $full = Get-FullPath $Path
    $root = [System.IO.Path]::GetPathRoot($full)
    $relative = if ($root) { $full.Substring($root.Length).TrimEnd([char[]]@('\', '/')) } else { '' }
    $segments = @($relative.Split([char[]]@('\', '/'), [System.StringSplitOptions]::RemoveEmptyEntries))
    if ($segments.Count -lt 2) { throw ('Refusing to remove a path that is too shallow: ' + $full) }
    $repository = Get-FullPath $PSScriptRoot
    if ($full -eq $repository -or $repository.StartsWith($full.TrimEnd([char[]]@('\', '/')) + '\',
            [System.StringComparison]::OrdinalIgnoreCase)) {
        throw ('Refusing to remove the repository or one of its parents: ' + $full)
    }
    return $full
}

# Only touches the fork's own startup entry, and only when it still points at
# this build output. An entry pointing elsewhere (for example upstream CCCC) is
# left alone.
function Remove-StartupEntry {
    $runKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run'
    $existing = Get-ItemProperty -Path $runKey -Name $productIdentity -ErrorAction SilentlyContinue
    if ($null -eq $existing) {
        Write-Output ('Startup entry ' + $productIdentity + ' was not present.')
        return
    }
    $value = [string]$existing.PSObject.Properties[$productIdentity].Value
    $expected = [System.IO.Path]::GetFullPath((Join-Path (Get-FullPath $OutputDirectory) 'Cccc.exe'))
    if ($value -and -not [string]::Equals($value.Trim('"'), $expected,
            [System.StringComparison]::OrdinalIgnoreCase)) {
        Write-Output ('Startup entry ' + $productIdentity + ' points elsewhere and was left untouched: ' + $value)
        return
    }
    try {
        Remove-ItemProperty -Path $runKey -Name $productIdentity -ErrorAction Stop
        Write-Output ('Removed the ' + $productIdentity + ' startup entry.')
    }
    catch {
        throw ('Could not remove the ' + $productIdentity + ' startup entry: ' + $_.Exception.Message)
    }
}

# Matches by full executable path: Get-Process -Name is case-insensitive and
# would also hit the upstream CCCC executable.
function Stop-RunningApp {
    $expected = [System.IO.Path]::GetFullPath((Join-Path (Get-FullPath $OutputDirectory) 'Cccc.exe'))
    $running = @(Get-Process -Name 'Cccc' -ErrorAction SilentlyContinue | Where-Object {
            try {
                $_.Path -and [string]::Equals([System.IO.Path]::GetFullPath($_.Path), $expected,
                    [System.StringComparison]::OrdinalIgnoreCase)
            }
            catch { $false }
        })
    if ($running.Count -eq 0) { return }
    foreach ($process in $running) {
        try { $process.CloseMainWindow() | Out-Null } catch { }
    }
    Start-Sleep -Milliseconds 750
    foreach ($process in $running) {
        try { $process | Stop-Process -Force -ErrorAction SilentlyContinue } catch { }
    }
    Start-Sleep -Milliseconds 250
    Write-Output ('Stopped {0} running Cccc process(es).' -f $running.Count)
}

function Remove-ProjectBuildFolders {
    foreach ($projectDirectory in Get-ChildItem -LiteralPath $PSScriptRoot -Directory) {
        $project = Get-ChildItem -LiteralPath $projectDirectory.FullName -Filter '*.csproj' -File -ErrorAction SilentlyContinue
        if (-not $project) { continue }
        foreach ($name in @('bin', 'obj')) {
            $path = Join-Path $projectDirectory.FullName $name
            if (Test-Path -LiteralPath $path) {
                Remove-Item -LiteralPath $path -Recurse -Force
            }
        }
    }
}

# Removes everything Cccc leaves outside its own folder plus local build output.
# The application folder itself is never deleted; delete it by hand when done.
if ($Uninstall) {
    Stop-RunningApp
    Remove-StartupEntry
    $settingsRoot = Assert-SafeRemovalPath ([System.IO.Path]::Combine(
        [System.Environment]::GetFolderPath('LocalApplicationData'), $productIdentity))
    if (Test-Path -LiteralPath $settingsRoot) {
        Remove-Item -LiteralPath $settingsRoot -Recurse -Force
        Write-Output ('Removed settings folder: ' + $settingsRoot)
    }
    else {
        Write-Output 'Settings folder was not present.'
    }
    $outputRoot = Assert-SafeRemovalPath $OutputDirectory
    if (Test-Path -LiteralPath $outputRoot) {
        Remove-Item -LiteralPath $outputRoot -Recurse -Force
        Write-Output ('Removed build output: ' + $outputRoot)
    }
    if ($Deep) {
        & dotnet clean $solution -c $Configuration --nologo
        if ($LASTEXITCODE -ne 0) { throw ('dotnet clean failed with exit code {0}.' -f $LASTEXITCODE) }
        Remove-ProjectBuildFolders
        Write-Output 'Removed bin and obj folders.'
    }
    Write-Output 'Uninstall complete. Delete the Cccc program folder by hand.'
    exit 0
}

$needsDevBuild = (-not $Package) -or $CheckQuality -or $CheckTaskbar -or $CheckAnimation -or $CheckMaterial -or $CheckShape
$fontRoot = Get-FullPath $FontDirectory

if ($needsDevBuild) {
    # Fonts are resolved from system/user-configured font families rather than embedded in the binary.
    & dotnet build $solution -c $Configuration --nologo ("-p:FontDirectory=" + $fontRoot)
    if ($LASTEXITCODE -ne 0) { throw ('dotnet build failed with exit code {0}.' -f $LASTEXITCODE) }

    $appOutput = Join-Path $PSScriptRoot ('Cccc.Desktop/bin/' + $Configuration + '/' + $targetFramework)
    $builtExecutable = Join-Path $appOutput 'Cccc.exe'
    if (-not (Test-Path -LiteralPath $builtExecutable -PathType Leaf)) {
        throw ('Build output not found: ' + $builtExecutable)
    }
    $outputRoot = Get-FullPath $OutputDirectory
    $outputRoot = Assert-SafeRemovalPath $outputRoot
    if (Test-Path -LiteralPath $outputRoot) { Remove-Item -LiteralPath $outputRoot -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null
    Copy-Item -Path (Join-Path $appOutput '*') -Destination $outputRoot -Recurse -Force
    $executable = Join-Path $outputRoot 'Cccc.exe'
    if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
        throw ('Copy failed: ' + $executable)
    }
    Write-Output ('Built ' + $executable)
}

function Invoke-Checks([string]$Mode, [string[]]$Arguments = @()) {
    if (-not (Test-Path -LiteralPath $checksExecutable -PathType Leaf)) {
        throw ('Presentation checks executable not found: ' + $checksExecutable)
    }
    Invoke-Executable $checksExecutable (@($Mode) + $Arguments)
}

if ($CheckQuality) {
    & dotnet test $solution -c $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { throw ('dotnet test failed with exit code {0}.' -f $LASTEXITCODE) }
    Write-Output 'Quality checks passed.'
}

if ($CheckTaskbar) {
    Invoke-Checks 'taskbar'
    Write-Output 'Taskbar checks passed.'
}

if ($CheckAnimation) {
    Invoke-Checks 'animation' (Join-Path $outputRoot 'presentation-checks')
    Write-Output 'Animation checks passed.'
}

if ($CheckMaterial) {
    Invoke-Checks 'material' @('lifecycle')
    Write-Output 'Material checks passed.'
}

if ($CheckShape) {
    Invoke-Checks 'shape' (Join-Path $outputRoot 'shape-checks')
    Write-Output 'Shape checks passed.'
}

function Get-ProjectVersion {
    $propsPath = Join-Path $PSScriptRoot 'Directory.Build.props'
    if (Test-Path -LiteralPath $propsPath -PathType Leaf) {
        [xml]$props = Get-Content -LiteralPath $propsPath -Raw
        $versionNode = $props.SelectSingleNode('//PropertyGroup/Version')
        if ($versionNode -and -not [string]::IsNullOrWhiteSpace($versionNode.InnerText)) {
            return $versionNode.InnerText.Trim()
        }
    }
    return '0.0.0'
}

if ($Package) {
    $publishConfiguration = if ($PSBoundParameters.ContainsKey('Configuration')) { $Configuration } else { 'Release' }
    $isSelfContained = [bool]$SelfContained
    $version = Get-ProjectVersion
    $suffix = if ($isSelfContained) { 'win-x64-self-contained' } else { 'win-x64' }
    $packageRoot = Assert-SafeRemovalPath $PackageDirectory
    if (-not (Test-Path -LiteralPath $packageRoot)) {
        New-Item -ItemType Directory -Force -Path $packageRoot | Out-Null
    }
    $stagingRoot = Join-Path $packageRoot '_staging'
    $zipName = ('Cccc-{0}-{1}.zip' -f $version, $suffix)
    $zipPath = Join-Path $packageRoot $zipName

    if (Test-Path -LiteralPath $stagingRoot) { Remove-Item -LiteralPath $stagingRoot -Recurse -Force }
    if (Test-Path -LiteralPath $zipPath) { Remove-Item -LiteralPath $zipPath -Force }

    $desktopProject = Join-Path $PSScriptRoot 'Cccc.Desktop/Cccc.Desktop.csproj'
    $scArg = if ($isSelfContained) { 'true' } else { 'false' }

    try {
        & dotnet publish $desktopProject -c $publishConfiguration -r win-x64 --self-contained $scArg -o $stagingRoot --nologo `
            ('-p:FontDirectory=' + $fontRoot) '-p:DebugType=none' '-p:DebugSymbols=false'
        if ($LASTEXITCODE -ne 0) { throw ('dotnet publish failed with exit code {0}.' -f $LASTEXITCODE) }

        $publishedExe = Join-Path $stagingRoot 'Cccc.exe'
        if (-not (Test-Path -LiteralPath $publishedExe -PathType Leaf)) {
            throw ('Published executable not found: ' + $publishedExe)
        }

        Get-ChildItem -LiteralPath $stagingRoot -Include '*.pdb', '*.runtimeconfig.dev.json' -Recurse -File -ErrorAction SilentlyContinue |
            Remove-Item -Force

        $licensePath = Join-Path $PSScriptRoot 'LICENSE'
        if (Test-Path -LiteralPath $licensePath -PathType Leaf) {
            Copy-Item -LiteralPath $licensePath -Destination (Join-Path $stagingRoot 'LICENSE') -Force
        }

        Add-Type -AssemblyName System.IO.Compression.FileSystem
        [System.IO.Compression.ZipFile]::CreateFromDirectory(
            $stagingRoot,
            $zipPath,
            [System.IO.Compression.CompressionLevel]::Optimal,
            $false
        )
    }
    finally {
        if (Test-Path -LiteralPath $stagingRoot) { Remove-Item -LiteralPath $stagingRoot -Recurse -Force }
    }

    $sizeMb = [Math]::Round((Get-Item -LiteralPath $zipPath).Length / 1MB, 2)
    Write-Output ('Packaged {0} ({1} MB)' -f $zipPath, $sizeMb)
}
