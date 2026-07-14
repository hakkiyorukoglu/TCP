# TCP Changelog

All notable changes to TCP (Train Control Platform) are documented in this file.

## TCP-2.9.1 — Editor UI/UX and Database Integrity Fixes

**Release Date**: 2026-07-14

### Fixed & Improved
- **Pivot Zoom (UI)**: Farenin tekerleğiyle yakınlaştırma yapıldığında oluşan merkez odaklanma sorunu (XAML `RenderTransformOrigin`) düzeltilerek imlece göre (Pivot Zoom) çalışması sağlandı.
- **Rota Çizim Mantığı**: Sağ tık (Right Click) yapıldığında çizim iptali yerine çizimin tamamlanması (FinishRoute) sağlandı. Ayrıca kısa rotalarda okların (Direction Arrows) hemen belirmemesi sorunu düzeltildi.
- **Veritabanı ve JSON Serileştirme (Core)**: SQLite kaydı esnasında derin nesnelerin (Modem, Station vb.) `$id` ve `$ref` kurgularını kaybetmemesi için `ReferenceHandler.Preserve` mimarisine geçildi. Aktif senaryo yokken oluşan "Hayalet Otomatik Kayıt" (Zombie Autosave) bug'ı silindi.

---

## TCP-2.6.0 — Spline Routing & Simulation Module

**Release Date**: [Current Date]

### Added
- **Spline Routing (Rota Çizimi)**: `EditorView`'de yer alan "Rota Çiz (Spline)" aracı ile noktalar halinde serbest ray hatları çizebilme.
- **Track Models**: `TrackRoute` ve `TrackNode` data modelleri ile rota organizasyonu.
- **Project Serialization**: Çizilen rotaların `.tcp` JSON dosyası içerisinde `Routes` başlığında Editor Layout olarak serileştirilmesi (Kalıcı kayıt).
- **Simulation View (Simülasyon UI)**: "Simülasyon" sekmesi ana shell üzerinde aktif edildi.
- **UI Bugfixes**: `SimulationView`'de bulunan `Page` hatası düzeltildi (`UserControl`'e dönüştürüldü) ve XML kapanış etiket hataları temizlendi.
- **ViewModel Updates**: `SimulationViewModel` artık sistemin ana `ViewModelBase`'ini kullanıyor ve `INotifyPropertyChanged` desteğine sahip.

---

## TCP-2.5.0 — OTA Update & Relay HTTP Control

**Release Date**: [Current Date]

### Added
- **Esp32HttpClient Service**: Created an asynchronous singleton service to execute direct HTTP GET/POST calls to ESP32 boards.
- **Relay Control Window**: Added a UI dialog allowing users to toggle specific station relay pins (`GET /relay?pin=X&state=Y`).
- **OTA Update Window**: Added a UI dialog allowing users to browse and deploy `.bin` firmware updates via `multipart/form-data` POST request directly to the station.
- **Live Mode Integration**: Both tools are now accessible from the context menu of stations placed on the Editor Map during Live Mode.

---

## TCP-2.4.2 — ESP32 WT32-ETH01 Preparations & Wi-Fi Removal

**Release Date**: [Current Date]

### Changed
- **Wi-Fi Module Removed**: Removed "ESP32 WROOM" from default registry to enforce wired LAN connectivity only.

### Fixed
- **Connection Refresh Bug**: Fixed bug where incoming/outgoing modem connections would be set to null due to UI ComboBox refresh sequence. Connections are now synced rather than cleared.
- **Save State**: Auto-saving implemented for connection adjustments.
- **Build Errors**: Resolved `RefreshNetworkStatus` and `OutgoingConnectionId` naming mismatch in Live Mode and Editor.

---

## TCP-2.4.1 — Connection Sync & UI Bugfixes

**Release Date**: [Current Date]

### Added
- **Manual Save & Refresh Controls**: Added explicit "Kaydet" button for modem incoming/outgoing connections to force save to local JSON. Added explicit "Yenile" button in Editor toolbar to force-sync and redraw all visual representations of the network on the canvas.

### Fixed
- **ComboBox Duplication Bug**: Fixed a recursive collection modification bug where the `AvailableConnections` list would duplicate items endlessly due to WPF automatically setting values to `null` when the item source is cleared. Protected by `_isUpdatingConnections` state flag.

---

## TCP-2.4.0 — Logical Flaws Fixes & Network Elastic Links

**Release Date**: [Current Date]

### Added

- **Elastic Connections (Daisy-Chain)**: Visual elastic strings correctly depicting network hierarchy in Editor map. Neon blue for Modem->Modem, Orange dashed for Modem->Station, and Gray dashed for Station->Component.
- **Auto-Sync Hierarchy**: Deleting a network item from Electronics view now safely and cleanly removes them from the Editor map. When a Modem is on the map, newly added stations/components auto-deploy to the map.
- **Main PC Constraint**: Enforced a single outgoing connection restriction for the Main PC node.
- **Pin Collision Prevention**: Safe check preventing the addition of multiple components to the same pin within a single Station.

---

## TCP-2.3.0 — 3-Column Layout & TreeView Layer Management

**Release Date**: [Current Date]

### Added

- **3-Column Layout**: Improved Editor interface with a separate left panel for the Palette, center area for the Editor map, and right panel for Layers.
- **TreeView Hierarchy**: Converted flat layer list into a hierarchical TreeView grouped by device location. Unassigned devices appear in "Atanmamış".
- **Context Menu Actions**: Right-click on devices in the Layer Tree to safely delete them from the Editor or open their properties.
- **Detailed Properties Screen**: Edit CustomName, Location, IP Address, Port, MAC Address, and LAN Cable directly from the Editor via the properties context menu window.

---

## TCP-2.2.0 — Editor Layers Panel

**Release Date**: [Current Date]

### Added

- **Photoshop-style Layers Panel**: A unified layers tree list on the right side of the editor.
- **Unified Layering**: Both Editor Images and custom Electronic Devices share the same `ILayerItem` abstraction.
- **Visibility & Locking**: Toggle visibility (Eye icon) and edit-locking (Lock icon) for any layer directly from the layers list.
- **Canvas Synchronization**: Bi-directional selection state synchronization between the canvas objects and the layers panel.
- **Selection Highlights**: Selected placed electronic boxes show a clear blue border (`Brush.Accent.Primary`) for visual feedback.


## TCP-2.1.0 — Editor Background & Transparency Features

**Release Date**: [Current Date]

### Added

- **Multi-Image Support**: Users can now load multiple background images into the editor canvas simultaneously.
- **Original Aspect Ratio Preservation**: Images strictly preserve their aspect ratios. Large files decode efficiently via MemoryStream bypassing WPF rendering limits without freezing the UI.
- **Opacity Control**: Added an opacity slider to make overlay maps or references semi-transparent.
- **Page Persistence**: "Save Page" and "Load Page" functionalities to store all Editor Image objects to a JSON state file.
- **AutoCAD-Style Canvas**: A subtle grid drawing brush has been set as the background for the canvas viewport.

### Fixed

- **WPF Deadlock in Image Decoding**: Fully migrated `BitmapImage` decoding to a synchronous `MemoryStream` execution on the UI thread, bypassing MTA threading COM lockouts and XamlParse exceptions due to dynamic data triggers on item generation.

---

## TCP-2.0.0 — Custom Devices & Editor Mapping

**Release Date**: [Current Date]

### Added

- **My Devices Tab**: A new tab in Electronics view to manage custom instances of devices.
- **Device Management**: Add, delete, and duplicate custom device instances. Devices include IP, Port, MAC address, LAN settings, and user-assigned names.
- **Editor Integration**: The palette now features user-created custom devices alongside default registry definitions.

---

## TCP-1.0.3 — Editor: Add board boxes from registry

**Release Date**: [Current Date]

### Added

- **Editor Palette**: Palette panel sourced from Electronics registry (single source of truth)
  - Left panel (240px fixed width) with board list from BoardRegistry
  - Each board shows DisplayName, Type badge, and Status
  - "Add to Map" button (enabled only when board is selected)

- **Placed Board Boxes**: Box cards on editor canvas
  - PlacedBoardBox model with BoxId, BoardId, DisplayName, Type, Status, X, Y
  - Boxes rendered as cards on Canvas overlay (on top of background image)
  - Each box displays DisplayName (bold), Type badge, and Status
  - Boxes positioned at X, Y coordinates (currently centered at 0,0)

- **BoardDefinition Model**: Board definition model for Editor palette
  - Id, DisplayName, Type, Status, Notes fields
  - Converted from BoardItem (BoardRegistry) - single source of truth

- **BoardRegistry Extensions**: Added GetAllBoards() and GetById() methods
  - Converts BoardItem to BoardDefinition
  - Type extracted from board name (e.g., "Arduino Mega" -> "Mega")

### Changed

- **Editor Layout**: 2-column layout (Palette left, Editor area right)
  - Palette panel with board list and "Add to Map" button
  - Editor area with background image + Canvas overlay for boxes

- **EditorViewModel**: Added palette and box management
  - PaletteBoards: ObservableCollection<BoardDefinition> from registry
  - PlacedBoxes: ObservableCollection<PlacedBoardBox>
  - SelectedPaletteBoard: For Add button selection
  - AddBoxCommand: Creates new PlacedBoardBox (with CanExecute check)

- **Version**: Updated to TCP-1.0.3
  - VersionManager.CurrentVersion = "TCP-1.0.3"
  - VersionManager.StageName = "Editor: Add board boxes from registry"

### Technical Details

- BoardRegistry is single source of truth (used by both Electronics and Editor)
- No hardcoded palette items - all boards come from registry
- AddBoxCommand uses RelayCommandWithCanExecute for button enable/disable
- Boxes rendered on Canvas overlay (IsHitTestVisible=False for now)
- No drag-drop, no pan/zoom, no selection (as per requirements)

---

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

