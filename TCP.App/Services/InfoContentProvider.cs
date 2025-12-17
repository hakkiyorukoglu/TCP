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
}
