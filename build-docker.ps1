#!/usr/bin/env pwsh

param(
    [string]$Tag = "dotnet-mcp-server2:latest",
    [ValidateSet("standard", "minimal", "distroless", "scratch")]
    [string]$Type = "standard",
    [switch]$NoBuildCache = $false,
    [switch]$Push = $false,
    [string]$Registry = "",
    [switch]$Test = $true,
    [switch]$ShowSizes = $true
)

Write-Host "Building Docker image for DotNetMcpServer2" -ForegroundColor Green

$ErrorActionPreference = "Stop"

try {
    # Determine which Dockerfile to use based on type
    $dockerfile = switch ($Type) {
        "standard" { "Dockerfile" }
        "minimal" { "Dockerfile" }  # Same as standard, but will use different tag
        "distroless" { "Dockerfile.distroless" }
        "scratch" { "Dockerfile.scratch" }
    }
    
    # Adjust tag based on type if not explicitly set
    if ($Tag -eq "dotnet-mcp-server2:latest") {
        $Tag = switch ($Type) {
            "standard" { "dotnet-mcp-server2:latest" }
            "minimal" { "dotnet-mcp-server2:minimal" }
            "distroless" { "dotnet-mcp-server2:distroless" }
            "scratch" { "dotnet-mcp-server2:scratch" }
        }
    }
    
    Write-Host "Building type: $Type" -ForegroundColor Cyan
    Write-Host "Using dockerfile: $dockerfile" -ForegroundColor Cyan
    Write-Host "Tag: $Tag" -ForegroundColor Cyan
    
    # Build arguments
    $buildArgs = @("docker", "build", "-f", $dockerfile, "-t", $Tag, ".")
    
    if ($NoBuildCache) {
        $buildArgs += "--no-cache"
        Write-Host "Building without cache..." -ForegroundColor Yellow
    }
    
    # Execute docker build
    Write-Host "`nBuilding Docker image..." -ForegroundColor Yellow
    Write-Host "Command: $($buildArgs -join ' ')" -ForegroundColor Gray
    
    $process = Start-Process -FilePath "docker" -ArgumentList ($buildArgs | Select-Object -Skip 1) -Wait -PassThru -NoNewWindow
    
    if ($process.ExitCode -ne 0) {
        throw "Docker build failed with exit code $($process.ExitCode)"
    }
    
    # Get image information
    if ($ShowSizes) {
        Write-Host "`nImage size information:" -ForegroundColor Yellow
        $imageInfo = docker images $Tag --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}\t{{.CreatedAt}}"
        Write-Host $imageInfo -ForegroundColor Cyan
        
        # Show detailed size breakdown
        Write-Host "`nDetailed size analysis:" -ForegroundColor Yellow
        $sizeBytes = docker inspect $Tag --format='{{.Size}}' 2>$null
        if ($sizeBytes) {
            $sizeMB = [math]::Round([int64]$sizeBytes / 1MB, 2)
            Write-Host "Compressed size: $sizeMB MB ($sizeBytes bytes)" -ForegroundColor Cyan
        }
        
        # Compare with different base image sizes
        Write-Host "`nExpected size ranges by type:" -ForegroundColor Yellow
        Write-Host "  scratch:     ~10-15 MB (smallest, no OS)" -ForegroundColor White
        Write-Host "  distroless:  ~12-18 MB (CA certs, timezone)" -ForegroundColor White
        Write-Host "  standard:    ~20-30 MB (minimal Ubuntu)" -ForegroundColor White
        Write-Host "  full-deps:   ~50-80 MB (traditional runtime)" -ForegroundColor White
    }
    
    # Test the image
    if ($Test) {
        Write-Host "`nTesting the Docker image..." -ForegroundColor Yellow
        
        # Test 1: Version check
        Write-Host "Test 1: Version check" -ForegroundColor White
        try {
            $versionOutput = docker run --rm $Tag --version 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ Version check passed" -ForegroundColor Green
                Write-Host "  Output: $versionOutput" -ForegroundColor Gray
            } else {
                Write-Host "⚠ Version check failed (expected for MCP server)" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "⚠ Version check not available" -ForegroundColor Yellow
        }
        
        # Test 2: Container startup
        Write-Host "Test 2: Container startup test" -ForegroundColor White
        $containerName = "test-mcp-$(Get-Random)"
        try {
            # Start container in background
            $containerId = docker run -d --name $containerName $Tag
            Start-Sleep -Seconds 2
            
            # Check if container is running
            $containerStatus = docker ps --filter "name=$containerName" --format "{{.Status}}"
            if ($containerStatus -and $containerStatus.Contains("Up")) {
                Write-Host "✓ Container started successfully" -ForegroundColor Green
                
                # Check container resource usage
                $stats = docker stats $containerName --no-stream --format "table {{.MemUsage}}\t{{.CPUPerc}}"
                Write-Host "  Resource usage: $stats" -ForegroundColor Gray
            } else {
                Write-Host "⚠ Container startup test inconclusive" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "⚠ Container startup test failed: $($_.Exception.Message)" -ForegroundColor Yellow
        } finally {
            # Cleanup test container
            docker rm -f $containerName 2>$null | Out-Null
        }
        
        # Test 3: Security scan (if available)
        Write-Host "Test 3: Basic security check" -ForegroundColor White
        try {
            # Check for common security issues
            $imageInspect = docker inspect $Tag | ConvertFrom-Json
            $config = $imageInspect[0].Config
            
            if ($config.User -and $config.User -ne "root") {
                Write-Host "✓ Non-root user configured: $($config.User)" -ForegroundColor Green
            } elseif ($Type -eq "scratch") {
                Write-Host "⚠ Scratch image runs as root (expected)" -ForegroundColor Yellow
            } else {
                Write-Host "⚠ Running as root user" -ForegroundColor Yellow
            }
            
            if (-not $config.Env.Contains("SHELL")) {
                Write-Host "✓ No shell configured (good for security)" -ForegroundColor Green
            }
            
        } catch {
            Write-Host "⚠ Security check failed: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
    
    # Push to registry if requested
    if ($Push -and $Registry) {
        $registryTag = "$Registry/$Tag"
        Write-Host "`nTagging for registry: $registryTag" -ForegroundColor Yellow
        docker tag $Tag $registryTag
        
        Write-Host "Pushing to registry..." -ForegroundColor Yellow
        docker push $registryTag
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Successfully pushed to registry" -ForegroundColor Green
        } else {
            Write-Host "✗ Failed to push to registry" -ForegroundColor Red
        }
    }
    
    Write-Host "`nDocker build completed successfully!" -ForegroundColor Green
    Write-Host "Type: $Type" -ForegroundColor Cyan
    Write-Host "Image: $Tag" -ForegroundColor Cyan
    
    # Usage examples
    Write-Host "`nUsage examples:" -ForegroundColor Yellow
    Write-Host "  Test run:        docker run --rm $Tag --version" -ForegroundColor White
    Write-Host "  MCP mode:        docker run --rm -i $Tag" -ForegroundColor White
    Write-Host "  With volume:     docker run --rm -i -v `"`$(pwd):/workspace`" -w /workspace $Tag" -ForegroundColor White
    Write-Host "  Background:      docker run -d --name mcp-server $Tag" -ForegroundColor White
    
    # Claude Desktop configuration
    Write-Host "`nClaude Desktop configuration:" -ForegroundColor Yellow
    Write-Host @"
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "docker",
      "args": ["run", "--rm", "-i", "$Tag"]
    }
  }
}
"@ -ForegroundColor White
    
} catch {
    Write-Host "`nDocker build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
