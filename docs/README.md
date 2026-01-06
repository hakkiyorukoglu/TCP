# TCP — Train Control Platform

TCP (Train Control Platform) is a professional WPF desktop application designed for train control and rail layout management.

## What TCP Is Now

As of **TCP-1.0.1**, TCP provides:

- **UI Foundation**: Complete navigation shell with top bar, content area, and status bar slot
- **Home Map Canvas**: Foundation for map-based rail layout design (empty canvas ready for image loading in TCP-1.0.2)
- **Electronics Management**: Board registry system for hardware abstraction
- **Settings System**: Category-based settings with local JSON persistence
- **Info Panel**: Comprehensive project documentation and version history
- **Search System**: Global search with autocomplete suggestions
- **Theme System**: Token-based design system with Dark theme

## What's Removed

In **TCP-1.0.1**, the following modules were removed:

- **Editor Module**: Removed completely (Views, ViewModels, Editor folder)
- **Simulation Module**: Removed completely (Views, ViewModels)

These modules were removed to focus on the Home Map Canvas foundation. Future features will be built on this stable base.

## How to Run

### Requirements

- .NET 8 SDK (or later)
- Windows OS (WPF requirement)

### Build and Run

```bash
# Clean previous builds
dotnet clean

# Build the solution
dotnet build

# Run the application
dotnet run --project TCP.App
```

### Common Issues

**MSB3027 Error (File Lock)**: If you get a file lock error during build, close any running TCP.exe process and try again.

## Folder Structure

```
TCP/
├── TCP.App/              # WPF Application (UI Layer)
│   ├── Shell/            # MainWindow (application shell)
│   ├── Views/            # XAML view files
│   ├── ViewModels/       # ViewModel classes
│   ├── Services/         # Application services
│   └── Models/           # Data models
│
├── TCP.Theming/          # Theme system
│   └── Themes/           # Theme files (tokens, variants)
│
├── TCP.Core/             # Core business logic (future)
│
└── docs/                 # Documentation
    ├── README.md         # This file
    ├── CHANGELOG.md      # Version history
    ├── ARCHITECTURE.md   # Architecture overview
    ├── ROADMAP.md        # Future plans
    └── SETUP.md          # Setup instructions
```

## Documentation

For detailed information, see:

- [CHANGELOG.md](CHANGELOG.md) - Version history
- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture and design principles
- [ROADMAP.md](ROADMAP.md) - Future development plans
- [SETUP.md](SETUP.md) - Setup and build instructions

## License

[Add license information here]

