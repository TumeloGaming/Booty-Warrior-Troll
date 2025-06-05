#!/bin/bash

echo "Building Joke Program..."

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed or not in PATH"
    echo "Please install .NET 6.0 SDK or later from https://dotnet.microsoft.com/download"
    exit 1
fi

echo "Restoring packages..."
dotnet restore

echo "Building application..."
dotnet build --configuration Release

echo "Publishing as single executable..."
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ./publish

echo ""
echo "Build complete!"
echo "Executable file: ./publish/JokeProgram.exe"
echo ""
