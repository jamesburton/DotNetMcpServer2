$ErrorActionPreference = "Continue"

Write-Host "Testing dotnet build..." -ForegroundColor Green

try {
    Set-Location "C:\Development\MCP\DotNetMcpServer2\DotNetMcpServer2"
    
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    
    $buildResult = & dotnet build /p:Platform=x64 2>&1
    $exitCode = $LASTEXITCODE
    
    Write-Host "Build output:" -ForegroundColor Cyan
    $buildResult | ForEach-Object { Write-Host $_ }
    
    Write-Host "Exit code: $exitCode" -ForegroundColor $(if ($exitCode -eq 0) { "Green" } else { "Red" })
    
    if ($exitCode -eq 0) {
        Write-Host "Build succeeded!" -ForegroundColor Green
    } else {
        Write-Host "Build failed!" -ForegroundColor Red
    }
}
catch {
    Write-Host "Error running build: $($_.Exception.Message)" -ForegroundColor Red
}
