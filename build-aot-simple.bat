@echo off
setlocal enabledelayedexpansion

echo ====================================
echo DotNetMcpServer2 AOT Build Script
echo ====================================
echo.

:: Check if dotnet is available
dotnet --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK is not installed or not in PATH
    echo Please install .NET 9.0 SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo .NET SDK version:
dotnet --version
echo.

:: Change to project directory
cd /d "%~dp0\DotNetMcpServer2"
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Could not change to project directory
    pause
    exit /b 1
)

:: Choose runtime
echo Available runtime targets:
echo 1. Windows x64 (win-x64)
echo 2. Windows ARM64 (win-arm64)
echo 3. Linux x64 (linux-x64)
echo 4. macOS x64 (osx-x64)
echo 5. macOS ARM64 (osx-arm64)
echo.

set /p choice="Select runtime (1-5, default is 1): "
if "%choice%"=="" set choice=1

if "%choice%"=="1" set runtime=win-x64
if "%choice%"=="2" set runtime=win-arm64
if "%choice%"=="3" set runtime=linux-x64
if "%choice%"=="4" set runtime=osx-x64
if "%choice%"=="5" set runtime=osx-arm64

if "%runtime%"=="" (
    echo ERROR: Invalid choice
    pause
    exit /b 1
)

echo.
echo Building for runtime: %runtime%
echo.

:: Clean previous builds
echo [1/4] Cleaning previous builds...
dotnet clean --configuration Release --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Clean failed
    pause
    exit /b 1
)

:: Restore packages
echo [2/4] Restoring packages...
dotnet restore --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Restore failed
    pause
    exit /b 1
)

:: Build AOT
echo [3/4] Building AOT executable...
set output_path=..\publish\%runtime%
dotnet publish --configuration Release --runtime %runtime% --self-contained true --output "%output_path%" --verbosity minimal

if %ERRORLEVEL% NEQ 0 (
    echo ERROR: AOT build failed
    pause
    exit /b 1
)

:: Verify output
echo [4/4] Verifying build output...

if "%runtime:~0,3%"=="win" (
    set exe_name=DotNetMcpServer2.exe
) else (
    set exe_name=DotNetMcpServer2
)

set exe_path=%output_path%\%exe_name%

if not exist "%exe_path%" (
    echo ERROR: Executable not found at %exe_path%
    pause
    exit /b 1
)

:: Get file size
for %%I in ("%exe_path%") do set file_size=%%~zI
set /a file_size_mb=%file_size% / 1048576

echo.
echo ====================================
echo BUILD COMPLETED SUCCESSFULLY!
echo ====================================
echo Runtime: %runtime%
echo Output: %exe_path%
echo Size: %file_size_mb% MB (%file_size% bytes)
echo.

:: Quick test
echo Testing executable...
"%exe_path%" --version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ✓ Executable test passed
) else (
    echo ⚠ Executable test failed - but build completed
)

echo.
echo To use in Claude Desktop, update your config with:
echo   "command": "%cd%\%exe_path%"
echo.

cd /d "%~dp0"
pause
