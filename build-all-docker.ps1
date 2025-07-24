#!/usr/bin/env pwsh

param(
    [switch]$NoBuildCache = $false,
    [switch]$SkipTests = $false
)

Write-Host "Building all Docker variants for size comparison..." -ForegroundColor Green
Write-Host "This will take several minutes as we build 3 different image types." -ForegroundColor Yellow
Write-Host ""

$ErrorActionPreference = "Continue"  # Allow individual builds to fail

$variants = @(
    @{ Type = "scratch"; Description = "Ultra-minimal (scratch base)" },
    @{ Type = "distroless"; Description = "Minimal with CA certs (Google Distroless)" },
    @{ Type = "standard"; Description = "Standard minimal (Microsoft Chiseled Ubuntu)" }
)

$results = @()

foreach ($variant in $variants) {
    $type = $variant.Type
    $description = $variant.Description
    
    Write-Host "Building $type variant: $description" -ForegroundColor Cyan
    Write-Host "=" * 50 -ForegroundColor Gray
    
    try {
        $buildArgs = @{
            Type = $type
            Test = -not $SkipTests
            ShowSizes = $false
        }
        
        if ($NoBuildCache) {
            $buildArgs.NoBuildCache = $true
        }
        
        # Build the variant
        & .\build-docker.ps1 @buildArgs
        
        if ($LASTEXITCODE -eq 0) {
            # Get size information
            $tag = "dotnet-mcp-server2:$type"
            $sizeOutput = docker images $tag --format "{{.Size}}"
            $sizeBytes = docker inspect $tag --format='{{.Size}}' 2>$null
            
            if ($sizeBytes) {
                $sizeMB = [math]::Round([int64]$sizeBytes / 1MB, 2)
            } else {
                $sizeMB = "Unknown"
            }
            
            $results += [PSCustomObject]@{
                Type = $type
                Description = $description
                DockerSize = $sizeOutput
                ActualMB = $sizeMB
                Status = "✓ Success"
            }
            
            Write-Host "✓ $type build completed successfully" -ForegroundColor Green
        } else {
            $results += [PSCustomObject]@{
                Type = $type
                Description = $description
                DockerSize = "Failed"
                ActualMB = "Failed"
                Status = "✗ Failed"
            }
            Write-Host "✗ $type build failed" -ForegroundColor Red
        }
    } catch {
        $results += [PSCustomObject]@{
            Type = $type
            Description = $description
            DockerSize = "Error"
            ActualMB = "Error"
            Status = "✗ Error: $($_.Exception.Message)"
        }
        Write-Host "✗ $type build error: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Write-Host ""
}

# Display results summary
Write-Host "BUILD SUMMARY" -ForegroundColor Green
Write-Host "=" * 50 -ForegroundColor Green

$results | Format-Table -AutoSize

# Performance comparison
Write-Host "`nPERFORMANCE COMPARISON" -ForegroundColor Yellow
Write-Host "=" * 50 -ForegroundColor Yellow

Write-Host "Expected characteristics by type:" -ForegroundColor White
Write-Host ""
Write-Host "SCRATCH (Ultra-minimal):" -ForegroundColor Cyan
Write-Host "  ✓ Smallest size (~10-15 MB)" -ForegroundColor Green
Write-Host "  ✓ Fastest startup" -ForegroundColor Green
Write-Host "  ✓ Maximum security (no attack surface)" -ForegroundColor Green
Write-Host "  ⚠ Runs as root (limitation of scratch)" -ForegroundColor Yellow
Write-Host "  ⚠ No debugging capabilities" -ForegroundColor Yellow
Write-Host ""

Write-Host "DISTROLESS (Google):" -ForegroundColor Cyan
Write-Host "  ✓ Very small size (~12-18 MB)" -ForegroundColor Green
Write-Host "  ✓ Fast startup" -ForegroundColor Green
Write-Host "  ✓ Non-root execution" -ForegroundColor Green
Write-Host "  ✓ CA certificates included" -ForegroundColor Green
Write-Host "  ✓ Timezone data included" -ForegroundColor Green
Write-Host "  ⚠ No debugging capabilities" -ForegroundColor Yellow
Write-Host ""

Write-Host "STANDARD (Microsoft Chiseled):" -ForegroundColor Cyan
Write-Host "  ✓ Small size (~20-30 MB)" -ForegroundColor Green
Write-Host "  ✓ Non-root execution" -ForegroundColor Green
Write-Host "  ✓ Microsoft-supported base" -ForegroundColor Green
Write-Host "  ✓ Optimized for .NET AOT" -ForegroundColor Green
Write-Host "  ✓ Basic health check support" -ForegroundColor Green
Write-Host "  ✓ Better compatibility" -ForegroundColor Green
Write-Host ""

# Usage recommendations
Write-Host "USAGE RECOMMENDATIONS" -ForegroundColor Yellow
Write-Host "=" * 50 -ForegroundColor Yellow

Write-Host "Choose based on your requirements:" -ForegroundColor White
Write-Host ""
Write-Host "🏆 Production (maximum security):     " -NoNewline; Write-Host "scratch" -ForegroundColor Cyan
Write-Host "🚀 Production (best balance):         " -NoNewline; Write-Host "distroless" -ForegroundColor Cyan  
Write-Host "🔧 Production (enterprise/support):   " -NoNewline; Write-Host "standard" -ForegroundColor Cyan
Write-Host "🏠 Local development:                 " -NoNewline; Write-Host "standard" -ForegroundColor Cyan
Write-Host ""

# Claude Desktop configurations
Write-Host "CLAUDE DESKTOP CONFIGURATIONS" -ForegroundColor Yellow
Write-Host "=" * 50 -ForegroundColor Yellow

foreach ($result in $results | Where-Object { $_.Status -like "*Success*" }) {
    $type = $result.Type
    Write-Host "`n$($type.ToUpper()) variant:" -ForegroundColor Cyan
    Write-Host @"
{
  "mcpServers": {
    "dotnet-mcp-server2-$type": {
      "command": "docker",
      "args": ["run", "--rm", "-i", "dotnet-mcp-server2:$type"],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
      }
    }
  }
}
"@ -ForegroundColor White
}

Write-Host "`nAll builds completed!" -ForegroundColor Green
Write-Host "Use the configurations above in your Claude Desktop config." -ForegroundColor Cyan
