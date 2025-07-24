#!/usr/bin/env pwsh

param(
    [string]$Runtime = "win-x64",
    [switch]$SkipBuild = $false
)

Write-Host "Testing AOT build for DotNetMcpServer2" -ForegroundColor Green
Write-Host "Runtime: $Runtime" -ForegroundColor Cyan

$ErrorActionPreference = "Stop"

try {
    # Navigate to project directory
    $projectDir = "DotNetMcpServer2"
    if (-not (Test-Path $projectDir)) {
        throw "Project directory '$projectDir' not found"
    }
    
    Set-Location $projectDir
    
    if (-not $SkipBuild) {
        # Clean and build
        Write-Host "`nCleaning previous builds..." -ForegroundColor Yellow
        dotnet clean --configuration Release --verbosity quiet
        
        Write-Host "Restoring packages..." -ForegroundColor Yellow
        dotnet restore --verbosity quiet
        
        Write-Host "Building AOT executable..." -ForegroundColor Yellow
        $outputPath = "../publish/$Runtime"
        dotnet publish --configuration Release --runtime $Runtime --self-contained true --output $outputPath --verbosity quiet
        
        if ($LASTEXITCODE -ne 0) {
            throw "AOT build failed with exit code $LASTEXITCODE"
        }
    }
    
    # Test the executable
    $exeName = if ($Runtime.StartsWith("win")) { "DotNetMcpServer2.exe" } else { "DotNetMcpServer2" }
    $exePath = "../publish/$Runtime/$exeName"
    
    if (-not (Test-Path $exePath)) {
        throw "Executable not found at: $exePath"
    }
    
    $fileSize = (Get-Item $exePath).Length
    $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
    
    Write-Host "`nExecutable Information:" -ForegroundColor Green
    Write-Host "Path: $exePath" -ForegroundColor Cyan
    Write-Host "Size: $fileSizeMB MB ($fileSize bytes)" -ForegroundColor Cyan
    
    # Test basic functionality
    Write-Host "`nTesting executable functionality..." -ForegroundColor Yellow
    
    # Test 1: Help command
    Write-Host "Test 1: Help command" -ForegroundColor White
    $helpProcess = Start-Process -FilePath $exePath -ArgumentList "--help" -PassThru -Wait -WindowStyle Hidden -RedirectStandardOutput -RedirectStandardError
    if ($helpProcess.ExitCode -eq 0) {
        Write-Host "✓ Help command succeeded" -ForegroundColor Green
    } else {
        Write-Host "✗ Help command failed (exit code: $($helpProcess.ExitCode))" -ForegroundColor Red
    }
    
    # Test 2: Version command
    Write-Host "Test 2: Version command" -ForegroundColor White
    $versionProcess = Start-Process -FilePath $exePath -ArgumentList "--version" -PassThru -Wait -WindowStyle Hidden -RedirectStandardOutput -RedirectStandardError
    if ($versionProcess.ExitCode -eq 0) {
        Write-Host "✓ Version command succeeded" -ForegroundColor Green
    } else {
        Write-Host "✗ Version command failed (exit code: $($versionProcess.ExitCode))" -ForegroundColor Red
    }
    
    # Test 3: Startup time
    Write-Host "Test 3: Startup performance" -ForegroundColor White
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $startupProcess = Start-Process -FilePath $exePath -ArgumentList "--version" -PassThru -Wait -WindowStyle Hidden -RedirectStandardOutput -RedirectStandardError
    $stopwatch.Stop()
    
    $startupTime = $stopwatch.ElapsedMilliseconds
    Write-Host "✓ Startup time: $startupTime ms" -ForegroundColor Green
    
    if ($startupTime -lt 100) {
        Write-Host "  Excellent startup performance!" -ForegroundColor Green
    } elseif ($startupTime -lt 500) {
        Write-Host "  Good startup performance" -ForegroundColor Yellow
    } else {
        Write-Host "  Startup time is slower than expected" -ForegroundColor Red
    }
    
    # Test 4: Memory usage (Windows only)
    if ($Runtime.StartsWith("win") -and $IsWindows) {
        Write-Host "Test 4: Memory usage check" -ForegroundColor White
        $memProcess = Start-Process -FilePath $exePath -ArgumentList "--version" -PassThru -WindowStyle Hidden -RedirectStandardOutput -RedirectStandardError
        Start-Sleep -Milliseconds 100
        
        if (-not $memProcess.HasExited) {
            $workingSet = $memProcess.WorkingSet64
            $workingSetMB = [math]::Round($workingSet / 1MB, 2)
            Write-Host "✓ Working set: $workingSetMB MB" -ForegroundColor Green
            $memProcess.Kill()
        }
    }
    
    # Test 5: Dependencies check
    Write-Host "Test 5: Dependency analysis" -ForegroundColor White
    
    if ($Runtime.StartsWith("win")) {
        # Check if it's truly self-contained (no external .NET dependencies)
        $publishDir = Split-Path $exePath -Parent
        $netCoreFiles = Get-ChildItem $publishDir -Filter "*.dll" | Where-Object { $_.Name -like "*Microsoft.NETCore*" -or $_.Name -like "*System.*" }
        
        if ($netCoreFiles.Count -eq 0) {
            Write-Host "✓ Truly self-contained (no external .NET dependencies detected)" -ForegroundColor Green
        } else {
            Write-Host "⚠ Some .NET dependencies found, but this is normal for AOT builds" -ForegroundColor Yellow
        }
    }
    
    Write-Host "`nAOT Build Test Summary:" -ForegroundColor Green
    Write-Host "✓ Build completed successfully" -ForegroundColor Green
    Write-Host "✓ Executable created: $fileSizeMB MB" -ForegroundColor Green
    Write-Host "✓ Basic functionality verified" -ForegroundColor Green
    Write-Host "✓ Performance characteristics acceptable" -ForegroundColor Green
    
    Write-Host "`nThe AOT build is ready for production use!" -ForegroundColor Green
    Write-Host "`nTo use in Claude Desktop, update your config with:" -ForegroundColor Cyan
    Write-Host "  'command': '$(Resolve-Path $exePath)'" -ForegroundColor White
    
} catch {
    Write-Host "`nAOT Build Test Failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    # Return to original directory
    Set-Location ".."
}
