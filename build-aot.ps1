#!/usr/bin/env pwsh

Write-Host "Building DotNetMcpServer2 with AOT compilation..." -ForegroundColor Green

# Define runtime identifiers for different platforms
$runtimeIds = @{
    "Windows x64" = "win-x64"
    "Windows ARM64" = "win-arm64"
    "Linux x64" = "linux-x64"
    "Linux ARM64" = "linux-arm64"
    "macOS x64" = "osx-x64"
    "macOS ARM64 (Apple Silicon)" = "osx-arm64"
}

# Get target runtime from parameter or prompt user
param(
    [string]$Runtime = ""
)

if (-not $Runtime) {
    Write-Host "`nAvailable runtime targets:" -ForegroundColor Yellow
    $i = 1
    foreach ($key in $runtimeIds.Keys) {
        Write-Host "  $i. $key ($($runtimeIds[$key]))"
        $i++
    }
    
    $selection = Read-Host "`nSelect runtime (1-$($runtimeIds.Count)) or enter runtime identifier directly"
    
    if ($selection -match '^\d+$' -and [int]$selection -le $runtimeIds.Count) {
        $Runtime = @($runtimeIds.Values)[[int]$selection - 1]
    } else {
        $Runtime = $selection
    }
}

Write-Host "`nBuilding for runtime: $Runtime" -ForegroundColor Cyan

try {
    # Navigate to project directory
    Set-Location "DotNetMcpServer2"
    
    # Clean previous builds
    Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
    dotnet clean --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "Clean failed" }
    
    # Restore packages
    Write-Host "Restoring packages..." -ForegroundColor Yellow
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
    
    # Build with AOT
    Write-Host "Publishing with AOT..." -ForegroundColor Yellow
    $outputPath = "../publish/$Runtime"
    dotnet publish --configuration Release --runtime $Runtime --self-contained true --output $outputPath
    if ($LASTEXITCODE -ne 0) { throw "Publish failed" }
    
    # Get executable name based on runtime
    $exeName = if ($Runtime.StartsWith("win")) { "DotNetMcpServer2.exe" } else { "DotNetMcpServer2" }
    $exePath = Join-Path $outputPath $exeName
    
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length
        $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
        
        Write-Host "`nAOT build completed successfully!" -ForegroundColor Green
        Write-Host "Runtime: $Runtime" -ForegroundColor Cyan
        Write-Host "Output directory: $outputPath" -ForegroundColor Cyan
        Write-Host "Executable: $exePath" -ForegroundColor Cyan
        Write-Host "File size: $fileSizeMB MB ($fileSize bytes)" -ForegroundColor Cyan
        
        # Test if it runs
        Write-Host "`nTesting executable..." -ForegroundColor Yellow
        $testProcess = Start-Process -FilePath $exePath -ArgumentList "--help" -PassThru -Wait -WindowStyle Hidden
        if ($testProcess.ExitCode -eq 0) {
            Write-Host "Executable test passed!" -ForegroundColor Green
        } else {
            Write-Host "Warning: Executable test failed with exit code $($testProcess.ExitCode)" -ForegroundColor Yellow
        }
    } else {
        throw "Executable not found at expected location: $exePath"
    }
    
} catch {
    Write-Host "`nBuild failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    # Return to original directory
    Set-Location ".."
}

Write-Host "`nBuild script completed." -ForegroundColor Green
