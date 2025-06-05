@echo off
echo Building Joke Program...

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: .NET SDK is not installed or not in PATH
    echo Please install .NET 6.0 SDK or later from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo Restoring packages...
dotnet restore

echo Building application...
dotnet build --configuration Release

echo Publishing as single executable...
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ./publish

echo.
echo Build complete!
echo Executable file: ./publish/JokeProgram.exe
echo.
pause
