# Joke Program

A C# Windows Forms application that creates a humorous sequence of dialog boxes.

## Building the Application

### Prerequisites
- .NET 6.0 SDK or later
- Windows operating system

### Build Instructions

1. Open Command Prompt or PowerShell
2. Navigate to the JokeProgram directory
3. Run the following commands:

```bash
# Restore packages
dotnet restore

# Build the application
dotnet build --configuration Release

# Publish as a single executable file
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ./publish
```

The executable file will be created in the `./publish` folder as `JokeProgram.exe`.

### Alternative Build (Development)
For development/testing purposes:
```bash
dotnet run
```

## Features
- Downloads audio and image files from the provided URLs
- Displays a sequence of system-style dialog boxes
- Offers "Easy Way" and "Hard Way" options
- Hard Way: temporarily changes desktop wallpaper, plays audio 10 times, blocks input
- Automatically cleans up downloaded files when finished
- No permanent harm to the system

## Technical Details
- Built with Windows Forms
- Uses Windows API calls for wallpaper management and input blocking
- Downloads files to temporary directory
- Self-contained executable (no .NET runtime required on target machine)

## Warning
This is a joke/prank program. Use responsibly and only on systems you own or have permission to run it on.
