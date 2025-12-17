using System.Collections.Generic;
using TCP.App.Models;

namespace TCP.App.Services;

/// <summary>
/// InfoContentProvider - Info panel içeriği için tek kaynak
/// 
/// TCP-0.9.1: Info panel & TXT export
/// 
/// Bu servis hem Info Panel UI hem de TXT Export için içerik sağlar.
/// Single Source of Truth prensibi: Tüm içerik buradan gelir.
/// 
/// Single Responsibility: Info içeriği yönetimi
/// </summary>
public static class InfoContentProvider
{
    /// <summary>
    /// Project Overview metni
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static string GetOverview()
    {
        return "TCP (Train Control Platform) is a professional WPF application designed for train control and simulation. " +
               "The application provides a modular architecture with support for electronics board management, code editing, simulation, and comprehensive settings management. " +
               "TCP solves the problem of managing complex rail layout designs, simulating train movements, and monitoring electronics boards in a unified platform. " +
               "Target use cases include rail layout design, train movement simulation, and real-time electronics monitoring.";
    }
    
    /// <summary>
    /// Architecture açıklaması
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static List<string> GetArchitecturePoints()
    {
        return new List<string>
        {
            "WPF (.NET 8) - Modern Windows desktop UI framework",
            "MVVM Pattern - Strict separation of UI, logic, and data",
            "Token-based Theming - Consistent design system with Dark theme",
            "Registry-driven Navigation & Search - Centralized routing and search suggestions",
            "Modular Electronics Model - Board registry system for hardware abstraction",
            "Service Layer - Singleton services for centralized data management (VersionManager, SearchRegistry, BoardRegistry, SettingsPersistenceService)",
            "Future Simulation & Hardware Bridge - Planned architecture for real-time train control and hardware integration"
        };
    }
    
    /// <summary>
    /// Implemented features listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static List<string> GetImplementedFeatures()
    {
        return new List<string>
        {
            "Navigation shell with top bar and content area",
            "Electronics registry with board management",
            "Settings system with category navigation",
            "Theme system (Dark theme only)",
            "Global search with autocomplete suggestions",
            "Info panel with version history",
            "Versioning display system",
            "Local settings persistence (JSON)",
            "Shortcuts map UI"
        };
    }
    
    /// <summary>
    /// In Progress features listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static List<string> GetInProgressFeatures()
    {
        return new List<string>
        {
            "Simulation logic core",
            "Code editor for electronics boards",
            "Persistence expansion (project files, board configurations)"
        };
    }
    
    /// <summary>
    /// Planned features listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static List<string> GetPlannedFeatures()
    {
        return new List<string>
        {
            "Real hardware integration (Arduino, RFID, Servo controllers)",
            "Live train tracking and position monitoring",
            "Servo and switch control interface",
            "Advanced simulation with physics engine",
            "Multi-board coordination",
            "Network communication protocols"
        };
    }
    
    /// <summary>
    /// Roadmap summary
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static List<string> GetRoadmap()
    {
        return new List<string>
        {
            "UI Foundation → DONE",
            "Settings & Persistence → DONE",
            "Info Panel & Documentation → DONE",
            "Simulation Core → NEXT",
            "Hardware Bridge → LATER",
            "Advanced Features → FUTURE"
        };
    }
    
    /// <summary>
    /// Version history entries
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static List<VersionEntry> GetVersionHistory()
    {
        return new List<VersionEntry>
        {
            new VersionEntry
            {
                Version = "TCP-0.9.1",
                Title = "Info panel & TXT export",
                Description = "Added comprehensive Info panel content and TXT export functionality. Info panel now includes Overview, Architecture, Features, Version History, and Roadmap sections."
            },
            new VersionEntry
            {
                Version = "TCP-0.9.0",
                Title = "Info Panel v1",
                Description = "Added Info panel with version history and topbar navigation. Clicking version text in TopBar opens Info page. Sections: Overview, Architecture, Features, Version History."
            },
            new VersionEntry
            {
                Version = "TCP-0.8.2",
                Title = "Shortcuts Map (UI list)",
                Description = "Added Shortcuts Map UI list in Settings > Shortcuts. Disabled theme switching - app now uses Dark theme only."
            },
            new VersionEntry
            {
                Version = "TCP-0.8.1",
                Title = "Settings Persistence",
                Description = "Introduced local settings persistence. Persisted: Theme, LastRoute, Panel widths. Location: %AppData%/TCP/settings.json"
            },
            new VersionEntry
            {
                Version = "TCP-0.8.0",
                Title = "Settings System v1",
                Description = "Introduced professional Settings page with category navigation"
            },
            new VersionEntry
            {
                Version = "TCP-0.7.x",
                Title = "UI Foundation",
                Description = "Established theme system, navigation shell, and basic UI components. Foundation for future features."
            }
        };
    }
    
    /// <summary>
    /// TXT export için formatlanmış içerik oluştur
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public static string GenerateTxtContent()
    {
        var lines = new List<string>();
        
        // Header
        lines.Add("-----------------------------------");
        lines.Add("# TCP — Train Control Platform");
        lines.Add("-----------------------------------");
        lines.Add("");
        
        // Project Overview
        lines.Add("## Project Overview");
        lines.Add(GetOverview());
        lines.Add("");
        
        // Architecture Overview
        lines.Add("## Architecture Overview");
        var archPoints = GetArchitecturePoints();
        foreach (var point in archPoints)
        {
            lines.Add($"- {point}");
        }
        lines.Add("");
        
        // Feature List
        lines.Add("## Feature List");
        lines.Add("### Implemented");
        foreach (var feature in GetImplementedFeatures())
        {
            lines.Add($"- {feature}");
        }
        lines.Add("");
        lines.Add("### In Progress");
        foreach (var feature in GetInProgressFeatures())
        {
            lines.Add($"- {feature}");
        }
        lines.Add("");
        lines.Add("### Planned");
        foreach (var feature in GetPlannedFeatures())
        {
            lines.Add($"- {feature}");
        }
        lines.Add("");
        
        // Version History
        lines.Add("## Version History");
        var versions = GetVersionHistory();
        foreach (var version in versions)
        {
            lines.Add($"- {version.Version} → {version.Title}: {version.Description}");
        }
        lines.Add("");
        
        // Roadmap Summary
        lines.Add("## Roadmap Summary");
        var roadmap = GetRoadmap();
        foreach (var item in roadmap)
        {
            lines.Add($"- {item}");
        }
        lines.Add("");
        lines.Add("-----------------------------------");
        lines.Add("END OF FILE");
        lines.Add("-----------------------------------");
        
        return string.Join("\r\n", lines);
    }
    
    /// <summary>
    /// Cursor Context - AI reviewer için proje durumu
    /// TCP-1.0.0: UI FOUNDATION RELEASE
    /// 
    /// Bu metod AI (Cursor) için projenin tam durumunu açıklar.
    /// Teknik, net, ve gelecek yön için beklentileri içerir.
    /// </summary>
    public static string GetCursorContext()
    {
        return @"====================================
TCP (Train Control Platform) — Cursor Context
====================================

This document provides a comprehensive overview of the TCP project state as of TCP-1.0.0 (UI FOUNDATION RELEASE).
It is written for AI reviewers (like Cursor) to understand the project architecture, constraints, and future direction.

====================================
1. PROJECT OVERVIEW
====================================

What TCP Is:
TCP is a professional WPF desktop application designed for train control and simulation. The application provides a unified platform for managing rail layout designs, simulating train movements, and monitoring electronics boards.

Why It Exists:
TCP solves the problem of managing complex rail layout designs, simulating train movements, and monitoring electronics boards in a unified, professional desktop application. It bridges the gap between design, simulation, and real hardware control.

Target Usage:
- Editor: Visual rail layout design and configuration
- Simulation: Train movement simulation with physics
- Real Hardware: Integration with Arduino, RFID readers, Servo controllers for live train control

====================================
2. CURRENT ARCHITECTURE
====================================

Application Type:
- WPF (.NET 8) desktop application
- MVVM pattern strictly enforced
- No code-behind logic (except UI wiring)
- No business logic in Views

Shell Layout:
- MainWindow.xaml is the application shell
- Fixed TopBar (48px height) with navigation tabs, search, settings, info icons
- Main Content area (star height) for view switching
- StatusBar slot (28px height, currently empty)
- Toast notification overlay (top-right, non-blocking)

Navigation System:
- Simple view-switching using ContentControl
- Route-based navigation (string routes: ""Home"", ""Electronics"", ""Settings"", ""Info"", etc.)
- Navigation handled in MainWindow.xaml.cs via event handlers
- LastRoute persistence (saved to settings.json, but startup always goes to Home)

Search System:
- Global top-right search box with autocomplete dropdown
- SearchRegistry singleton service (single source of truth for search suggestions)
- Each page registers itself with Title, Keywords[], Route
- Filtering: case-insensitive contains check on Title or Keywords
- Selecting suggestion navigates to route

Services vs ViewModels Responsibility:
- Services: Singleton services for centralized data management
  - VersionManager: Version metadata
  - SearchRegistry: Search suggestions
  - BoardRegistry: Electronics board definitions
  - SettingsPersistenceService: Local JSON persistence
  - ThemeService: Theme loading/application
  - ShortcutsRegistry: Keyboard shortcuts definitions
  - InfoContentProvider: Info panel content (static methods)
  - NotificationService: Toast notifications
  - AppLogger: Error logging
- ViewModels: View-specific state and commands
  - MainViewModel: Search state, navigation commands
  - ElectronicsViewModel: Board selection, board details
  - SettingsViewModel: Settings categories, theme selection
  - InfoViewModel: Info sections, content display
- Views: Zero logic, only UI binding

Theme System:
- Token-based design system
- Separate token files: ThemeColors.xaml, ThemeBrushes.xaml, ThemeSpacing.xaml, ThemeRadius.xaml, ThemeFontSizes.xaml, ThemeTypography.xaml, ThemeIcons.xaml
- Theme variants: Theme.Dark.xaml (Light theme exists but is disabled in TCP-0.8.2+)
- SafeDefaults.xaml: Fallback resources loaded first to prevent StaticResource crashes
- All theme token bindings use DynamicResource (not StaticResource) for runtime safety
- No hardcoded colors anywhere in UI

Error Guardrails Approach:
- Global exception handlers: DispatcherUnhandledException, AppDomain_UnhandledException, TaskScheduler_UnobservedTaskException
- All exceptions are logged to %AppData%/TCP/logs/app.log via AppLogger
- Exceptions trigger non-blocking error toast notifications
- Application NEVER crashes due to unhandled exceptions (all marked as handled)
- No MessageBox dialogs for errors (toast only)

====================================
3. IMPLEMENTED FEATURES (as of TCP-1.0.0)
====================================

Navigation:
- Shell layout with TopBar, Content area, StatusBar slot
- View switching: Home, Electronics, Simulation (placeholder), Editor (placeholder), Settings, Info
- Route-based navigation with persistence
- TopBar navigation tabs with visual feedback

Search Registry:
- Centralized SearchRegistry singleton
- Autocomplete dropdown with keyboard navigation
- Route navigation on selection
- Case-insensitive filtering

Electronics Page:
- Left panel: Board list (Arduino Mega, Nano, RFID Reader, Servo Controller)
- Right panel: Board detail cards (reusable BoardDetailCard component)
- KeyValueRow component for key-value pairs
- Code editor placeholder panel (UI only, no compilation)
- BoardRegistry singleton for board definitions

Settings System:
- Category-based navigation (Appearance, Shortcuts, Paths, Performance, About)
- Two-column layout: fixed-width category list + scrollable content area
- Local JSON persistence (%AppData%/TCP/settings.json)
- Persisted: Theme (Dark only), LastRoute, LeftPanelWidth, SettingsCategoryWidth
- Shortcuts map UI (keyboard shortcuts list)

Info System:
- Info panel with section navigation (Overview, Architecture, Features, Version History, Roadmap, Cursor)
- InfoContentProvider: Single source of truth for all Info content
- Version history display
- Version display in TopBar (clickable, navigates to Info)

TXT Export:
- Export full project info as TXT file
- Export Cursor context as separate TXT file
- SaveFileDialog with default filenames
- Toast notifications on success/error/cancel

Toast Notifications:
- Global NotificationService singleton
- Non-blocking toast overlay (top-right)
- Auto-dismiss after 4 seconds
- Maximum 3 visible toasts
- Types: Success, Warning, Error
- ToastNotification reusable component

Versioning UX:
- VersionManager singleton exposes CurrentVersion, StageName, DisplayVersion
- Version visible in TopBar
- Clicking version navigates to Info panel
- Version history in Info panel

====================================
4. TECHNOLOGY STACK
====================================

.NET Version:
- .NET 8 (latest LTS)

WPF:
- Windows Presentation Foundation for desktop UI
- XAML for UI markup
- Data binding (OneWay by default, TwoWay only where needed)

MVVM:
- Strict MVVM pattern
- ViewModels inherit ViewModelBase (INotifyPropertyChanged)
- RelayCommand for ICommand implementation
- No code-behind logic (except UI event wiring)

Local JSON Persistence:
- System.Text.Json (no Newtonsoft)
- Settings saved to %AppData%/TCP/settings.json
- Error handling: graceful fallback if file corrupted or missing

No External Frameworks:
- No MVVM framework (custom ViewModelBase, RelayCommand)
- No dependency injection container
- No external logging framework (custom AppLogger)
- No external UI libraries (pure WPF)

====================================
5. KNOWN LIMITATIONS
====================================

UI-Only Features:
- Electronics page: Board list and details are UI-only (no serial/network communication)
- Code editor: Placeholder TextBox (no syntax highlighting, no compilation)
- Simulation page: Empty placeholder
- Editor page: Empty placeholder

Placeholder Features:
- Board status: Always shows ""Offline"" (hardcoded)
- Board connection info: Placeholder values (COM3, IP addresses)
- Code editor: Multiline TextBox with placeholder text

Intentionally Deferred:
- Real hardware integration (Arduino, RFID, Servo controllers)
- Simulation logic and physics engine
- Code compilation and validation
- Network communication protocols
- Multi-board coordination
- Advanced simulation features

====================================
6. DESIGN RULES FOLLOWED
====================================

Binding Rules:
- OneWay binding is the default
- TwoWay binding ONLY for user input (SelectedItem, TextBox.Text with user editing)
- All bindings explicitly specify Mode (no implicit TwoWay)
- FallbackValue used where appropriate (e.g., ""No board selected"")

Token-Based Theming:
- NO hardcoded colors in XAML
- NO hardcoded font sizes
- NO hardcoded spacing/margins (use tokens)
- All styling comes from theme tokens
- DynamicResource for theme tokens (StaticResource only for converters, non-theme resources)

Single Source of Truth:
- InfoContentProvider: All Info panel content
- SearchRegistry: All search suggestions
- BoardRegistry: All board definitions
- VersionManager: Version metadata
- SettingsPersistenceService: Settings persistence

No Hardcoded UI Strings in Logic:
- All UI strings come from InfoContentProvider or ViewModels
- No magic strings in code-behind
- Route names are consistent strings (""Home"", ""Electronics"", etc.)

Error Handling:
- All file I/O wrapped in try-catch
- All service calls have fallback behavior
- No exceptions bubble to UI
- Toast notifications for user feedback

====================================
7. WHAT CURSOR (AI) NEEDS TO KNOW
====================================

Assumptions:
- User expects incremental, non-breaking changes
- User prefers explicit, detailed comments in Turkish
- User follows clean code principles (SOLID, especially Single Responsibility)
- User wants minimal refactoring (only what's explicitly requested)
- User wants stable builds (no compilation errors)

Constraints:
- DO NOT refactor unrelated code
- DO NOT remove unused-looking code unless explicitly requested
- DO NOT change architecture without permission
- DO NOT modify formatting/imports outside scope
- DO NOT add features not explicitly requested
- DO NOT use external frameworks unless already in use
- DO NOT break existing functionality

Future Direction Expectations:
- Next major milestone: Simulation Core (TCP-2.0.0)
- Hardware Bridge: Real Arduino/RFID/Servo integration
- Code Editor: Syntax highlighting, compilation, validation
- Advanced Features: Multi-board coordination, network protocols
- All future features must follow existing MVVM pattern
- All future features must use theme tokens
- All future features must have error guardrails

Code Style:
- Turkish comments (detailed, educational)
- Clean code principles
- SOLID (especially Single Responsibility)
- MVVM strict separation
- No code-behind logic (except UI wiring)

Versioning:
- Semantic versioning: TCP-Major.Minor.Patch
- Major: Architectural changes
- Minor: New features
- Patch: Bugfixes/polish
- VersionManager is single source of truth

====================================
END OF CURSOR CONTEXT
====================================";
    }
    
    /// <summary>
    /// Cursor Context TXT export için formatlanmış içerik
    /// TCP-1.0.0: UI FOUNDATION RELEASE
    /// </summary>
    public static string GenerateCursorContextTxt()
    {
        return GetCursorContext();
    }
}
