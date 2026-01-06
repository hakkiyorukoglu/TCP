# TCP Changelog

All notable changes to TCP (Train Control Platform) are documented in this file.

## TCP-1.0.2 — Background Image Load (Editor)

**Release Date**: [Current Date]

### Added

- **Editor Page Reintroduced**: Editor page restored with background image loading functionality
  - Load PNG/JPEG images via file dialog
  - Display image in editor area (locked, not movable)
  - Two display modes: Fit-to-area and Actual-size toggle
  - Editor toolbar with Load Image, Fit, Actual, and Clear buttons
  - Status text showing image load state

- **Image Loading**: Safe image loading with file lock prevention
  - Uses `BitmapImage` with `CacheOption.OnLoad` to prevent file locks
  - Image frozen after loading for thread safety
  - Toast notifications for success/error feedback

### Changed

- **Home Page**: Made intentionally empty (postponed until 2.0.0+)
  - Home route remains but shows empty content
  - No map canvas, no controls, no placeholders

- **Startup Route**: App now opens with Editor by default
  - Default route changed from Home to Editor
  - Editor tab added to TopBar navigation

- **Version**: Updated to TCP-1.0.2
  - VersionManager.CurrentVersion = "TCP-1.0.2"
  - VersionManager.StageName = "Background Image Load (Editor)"

### Technical Details

- EditorView and EditorViewModel recreated
- Image display uses OneWay binding (no TwoWay bindings)
- Fit mode: Image Stretch="Uniform", centered
- Actual mode: ScrollViewer with Image Stretch="None", scrollbars Auto
- No pan/zoom, no drawing tools, no selection (locked requirement)

---

## TCP-1.0.1 — Home Map Canvas (Empty)

**Release Date**: [Current Date]

### Added

- **Home Map Canvas Foundation**: Introduced empty map canvas UI with placeholder
  - Left/Main area: Large map canvas (Border/Card) with neutral background
  - Right panel: Fixed-width side panel with "Map" header and "Load Map Image" button
  - Canvas placeholder ready for image loading in TCP-1.0.2
  - Centered hint text: "Map canvas ready (TCP-1.0.1). Load image in TCP-1.0.2."

- **Load Map Image Command**: Added `LoadMapImageCommand` in HomeViewModel
  - Button click shows toast notification: "Not implemented in TCP-1.0.1. Coming in TCP-1.0.2."
  - No disabled controls with "Coming Soon" tooltips (explicit action feedback)

- **Documentation Structure**: Created `/docs` folder with professional structure
  - README.md - Project overview
  - CHANGELOG.md - Version history (this file)
  - ARCHITECTURE.md - Architecture and design principles
  - ROADMAP.md - Future development plans
  - SETUP.md - Setup and build instructions

### Removed

- **Editor Module**: Completely removed
  - TCP.App/Views/EditorView* (xaml + cs)
  - TCP.App/ViewModels/EditorViewModel*
  - TCP.App/Editor/ folder (if existed)
  - Editor navigation routes and tabs
  - Editor SearchRegistry entries

- **Simulation Module**: Completely removed
  - TCP.App/Views/SimulationView* (xaml + cs)
  - TCP.App/ViewModels/SimulationViewModel*
  - Simulation navigation routes and tabs
  - Simulation SearchRegistry entries

### Changed

- **Version**: Updated to TCP-1.0.1
  - VersionManager.CurrentVersion = "TCP-1.0.1"
  - VersionManager.StageName = "Home Map Canvas (Empty)"

- **InfoContentProvider**: Added TCP-1.0.1 entry to version history

- **Navigation**: Removed Editor and Simulation from TopBar navigation tabs
  - Only Home and Electronics tabs remain

- **Search Registry**: Removed Editor and Simulation search suggestions

### Technical Details

- All Editor/Simulation references removed from MainWindow.xaml.cs
- All Editor/Simulation references removed from App.xaml.cs
- Build and run verified: `dotnet clean`, `dotnet build`, `dotnet run` all succeed
- No compilation errors or broken references

---

## TCP-0.9.1 — Info panel & TXT export

### Added

- Comprehensive Info panel content and TXT export functionality
- Info panel now includes Overview, Architecture, Features, Version History, and Roadmap sections

---

## TCP-0.9.0 — Info Panel v1

### Added

- Info panel with version history and topbar navigation
- Clicking version text in TopBar opens Info page
- Sections: Overview, Architecture, Features, Version History

---

## TCP-0.8.2 — Shortcuts Map (UI list)

### Added

- Shortcuts Map UI list in Settings > Shortcuts
- Disabled theme switching - app now uses Dark theme only

---

## TCP-0.8.1 — Settings Persistence

### Added

- Local settings persistence
- Persisted: Theme, LastRoute, Panel widths
- Location: %AppData%/TCP/settings.json

---

## TCP-0.8.0 — Settings System v1

### Added

- Professional Settings page with category navigation

---

## TCP-0.7.x — UI Foundation

### Added

- Established theme system, navigation shell, and basic UI components
- Foundation for future features

---

[Previous versions...]

