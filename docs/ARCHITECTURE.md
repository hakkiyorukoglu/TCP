# TCP Architecture

This document describes the architecture and design principles of TCP (Train Control Platform).

## MVVM Overview

TCP follows the **MVVM (Model-View-ViewModel)** pattern strictly:

- **View**: XAML files (`.xaml`) - Only UI, zero logic
- **ViewModel**: C# classes (`.cs`) - View state and commands
- **Model**: Data models and business logic (future: TCP.Core)

### MVVM Rules (STRICT)

- **Views**: ZERO logic (only UI binding)
- **ViewModels**: ZERO UI code (only logic)
- **Services**: ZERO UI reference (only service logic)
- **Navigation**: ONLY through NavigationService or MainWindow event handlers

## Single Source of Truth

Every piece of data has a single source of truth:

- **InfoContentProvider**: All Info panel content
- **SearchRegistry**: All search suggestions
- **BoardRegistry**: All board definitions
- **VersionManager**: Version metadata
- **SettingsPersistenceService**: Settings persistence

This principle prevents data inconsistencies and makes maintenance easier.

## Binding Rules

### Default Binding Mode

- **OneWay** is the default binding mode
- **TwoWay** binding ONLY for user input:
  - `SelectedItem` (ListBox, ComboBox)
  - `TextBox.Text` (when user edits)
- All bindings explicitly specify `Mode` (no implicit TwoWay)

### Apply Pattern

For readonly properties, prefer **OneWay binding + explicit Apply pattern**:

```csharp
// ViewModel
public string SomeProperty { get; private set; }

public ICommand ApplyCommand { get; }
private void Apply() { /* update property */ }
```

```xml
<!-- View -->
<Button Command="{Binding ApplyCommand}" Content="Apply"/>
<TextBlock Text="{Binding SomeProperty, Mode=OneWay}"/>
```

### Fallback Values

Use `FallbackValue` where appropriate:

```xml
<TextBlock Text="{Binding SomeProperty, Mode=OneWay, FallbackValue='No data'}"/>
```

## Error Guardrails (No-Crash Policy)

TCP follows a strict **no-crash policy**:

### Global Exception Handlers

- `DispatcherUnhandledException`: UI thread exceptions
- `AppDomain_UnhandledException`: Non-UI thread exceptions
- `TaskScheduler_UnobservedTaskException`: Task exceptions

All exceptions are:
1. **Logged** to `%AppData%/TCP/logs/app.log` via `AppLogger`
2. **Handled** (marked as handled to prevent crash)
3. **Notified** via toast notification (non-blocking)

### Exception Handling Rules

- All file I/O wrapped in `try-catch`
- All service calls have fallback behavior
- No exceptions bubble to UI
- Toast notifications for user feedback (no MessageBox dialogs)

## Service Layer

Services are **singleton services** for centralized data management:

- `VersionManager`: Version metadata (static class)
- `SearchRegistry`: Search suggestions (singleton)
- `BoardRegistry`: Electronics board definitions (singleton)
- `SettingsPersistenceService`: Local JSON persistence
- `ThemeService`: Theme loading/application
- `ShortcutsRegistry`: Keyboard shortcuts definitions
- `InfoContentProvider`: Info panel content (static methods)
- `NotificationService`: Toast notifications (singleton)
- `AppLogger`: Error logging

## ViewModel Layer

ViewModels manage view-specific state and commands:

- `MainViewModel`: Search state, navigation commands
- `HomeViewModel`: Home page state, map canvas commands
- `ElectronicsViewModel`: Board selection, board details
- `SettingsViewModel`: Settings categories, theme selection
- `InfoViewModel`: Info sections, content display

## Dependency Direction

Strict dependency direction:

```
UI (TCP.App) → Theming (TCP.Theming) → Core (TCP.Core)
```

**Reverse dependency is FORBIDDEN**:
- ❌ TCP.Core → TCP.App (WRONG)
- ❌ TCP.Theming → TCP.App (WRONG)

## Theme System

Token-based design system:

- **Token files**: Separate files for colors, brushes, spacing, radius, fonts, typography, icons
- **Theme variants**: Theme.Dark.xaml (Light theme exists but disabled)
- **SafeDefaults.xaml**: Fallback resources loaded first
- **DynamicResource**: All theme token bindings use `DynamicResource` (not `StaticResource`)
- **No hardcoded colors**: All styling comes from theme tokens

## Code Style

- **Turkish comments**: Detailed, educational comments in Turkish
- **Clean code principles**: SOLID (especially Single Responsibility)
- **MVVM strict separation**: No code-behind logic (except UI wiring)
- **Versioning**: Semantic versioning (TCP-Major.Minor.Patch)

## Versioning

Semantic versioning format: **TCP-Major.Minor.Patch**

- **Major**: Architectural changes
- **Minor**: New features
- **Patch**: Bugfixes/polish

`VersionManager` is the single source of truth for version metadata.

