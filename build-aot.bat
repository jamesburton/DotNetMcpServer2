@echo off
echo Building DotNetMcpServer2 with AOT compilation...

:: Clean previous builds
dotnet clean --configuration Release

:: Restore packages
dotnet restore

:: Build for Windows x64 (change RuntimeIdentifier as needed)
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ./publish/win-x64

if %ERRORLEVEL% EQU 0 (
    echo.
    echo AOT build completed successfully!
    echo Output directory: ./publish/win-x64
    echo Executable: ./publish/win-x64/DotNetMcpServer2.exe
    
    :: Show file size
    for %%I in (./publish/win-x64/DotNetMcpServer2.exe) do echo File size: %%~zI bytes
) else (
    echo.
    echo Build failed with error code %ERRORLEVEL%
)

pause
