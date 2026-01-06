# TCP Setup Guide

This guide explains how to set up and build TCP (Train Control Platform).

## Requirements

### .NET SDK

- **.NET 8 SDK** (or later)
- Download from: https://dotnet.microsoft.com/download

### Operating System

- **Windows OS** (required for WPF)
- WPF is Windows-only, so TCP cannot run on Linux or macOS

### Development Tools (Optional)

- **Visual Studio 2022** (recommended)
- **Visual Studio Code** with C# extension (alternative)
- **JetBrains Rider** (alternative)

## Build Instructions

### Using Command Line

```bash
# Navigate to solution directory
cd TCP

# Clean previous builds
dotnet clean

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project TCP.App
```

### Using Visual Studio

1. Open `TCP.sln` in Visual Studio 2022
2. Right-click solution → **Restore NuGet Packages**
3. Press **F5** to build and run
4. Or: **Build** → **Build Solution** (Ctrl+Shift+B)

## Project Structure

```
TCP/
├── TCP.App/              # WPF Application (UI Layer)
│   ├── Shell/            # MainWindow
│   ├── Views/            # XAML views
│   ├── ViewModels/       # ViewModels
│   ├── Services/         # Services
│   └── Models/           # Data models
│
├── TCP.Theming/          # Theme system
│   └── Themes/           # Theme files
│
├── TCP.Core/             # Core business logic (future)
│
└── docs/                  # Documentation
```

## Common Issues

### MSB3027 Error (File Lock)

**Problem**: Build fails with "The process cannot access the file because it is being used by another process"

**Solution**:
1. Close any running TCP.exe process
2. Close Visual Studio if it's running the app
3. Try building again

### Missing .NET SDK

**Problem**: `dotnet` command not found

**Solution**:
1. Install .NET 8 SDK from https://dotnet.microsoft.com/download
2. Restart terminal/command prompt
3. Verify: `dotnet --version` should show 8.x.x

### Build Errors

**Problem**: Compilation errors after pulling changes

**Solution**:
1. Clean solution: `dotnet clean`
2. Restore packages: `dotnet restore`
3. Rebuild: `dotnet build`

### Theme Resources Not Found

**Problem**: StaticResource/DynamicResource errors at runtime

**Solution**:
1. Ensure `SafeDefaults.xaml` is loaded in App.xaml
2. Theme dictionaries are loaded AFTER MainWindow is shown
3. Check that all theme token bindings use `DynamicResource` (not `StaticResource`)

## Running Tests

Currently, TCP does not have unit tests. Future versions will include test projects.

## Debugging

### Visual Studio

1. Set breakpoints in code
2. Press **F5** to start debugging
3. Use **Debug** → **Windows** → **Output** to see logs

### Command Line

```bash
# Run with debug symbols
dotnet run --project TCP.App --configuration Debug
```

## Settings Location

TCP stores settings in:

- **Windows**: `%AppData%\TCP\settings.json`
- **Logs**: `%AppData%\TCP\logs\app.log`

## Getting Help

- Check [ARCHITECTURE.md](ARCHITECTURE.md) for architecture details
- Check [CHANGELOG.md](CHANGELOG.md) for version history
- Check [ROADMAP.md](ROADMAP.md) for future plans

