using System.Collections.Generic;
using System.Linq;

namespace TCP.App.Services;

/// <summary>
/// ShortcutsRegistry - Keyboard shortcuts registry
/// 
/// TCP-0.8.2: Shortcuts Map (UI list) + Dark-only theme
/// 
/// Singleton service that maintains all keyboard shortcuts.
/// Application-scoped lifetime.
/// 
/// Single Responsibility: Keyboard shortcuts data management
/// </summary>
public class ShortcutsRegistry
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    private static ShortcutsRegistry? _instance;
    
    /// <summary>
    /// Get singleton instance
    /// </summary>
    public static ShortcutsRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ShortcutsRegistry();
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Internal list of shortcut items
    /// </summary>
    private readonly List<ShortcutItem> _items = new();
    
    /// <summary>
    /// Private constructor (singleton pattern)
    /// TCP-0.8.2: Initialize with default shortcuts
    /// </summary>
    private ShortcutsRegistry()
    {
        // TCP-0.8.2: Register default shortcuts (UI-only, not yet wired)
        Register(new ShortcutItem
        {
            CommandName = "Command Palette",
            Keys = "Ctrl+K",
            Description = "Open global command/search palette"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Settings",
            Keys = "Ctrl+,",
            Description = "Open Settings"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Help / Info",
            Keys = "F1",
            Description = "Open Info/Help"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Search Focus",
            Keys = "Ctrl+F",
            Description = "Focus search box"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Quit",
            Keys = "Alt+F4",
            Description = "Close app"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Navigate Home",
            Keys = "Ctrl+1",
            Description = "Go to Home"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Navigate Electronics",
            Keys = "Ctrl+2",
            Description = "Go to Electronics"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Navigate Simulation",
            Keys = "Ctrl+3",
            Description = "Go to Simulation"
        });
        
        Register(new ShortcutItem
        {
            CommandName = "Navigate Editor",
            Keys = "Ctrl+4",
            Description = "Go to Editor"
        });
    }
    
    /// <summary>
    /// Get all registered shortcut items
    /// Returns read-only collection
    /// </summary>
    public IEnumerable<ShortcutItem> GetAll()
    {
        return _items.AsReadOnly();
    }
    
    /// <summary>
    /// Register a shortcut item
    /// </summary>
    public void Register(ShortcutItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        if (string.IsNullOrWhiteSpace(item.CommandName))
        {
            throw new ArgumentException("CommandName cannot be null or empty", nameof(item));
        }
        
        _items.Add(item);
    }
}

/// <summary>
/// ShortcutItem - Keyboard shortcut model class
/// 
/// TCP-0.8.2: Shortcuts Map (UI list) + Dark-only theme
/// </summary>
public class ShortcutItem
{
    /// <summary>
    /// Command name (e.g., "Command Palette")
    /// </summary>
    public string CommandName { get; set; } = string.Empty;
    
    /// <summary>
    /// Keyboard keys (e.g., "Ctrl+K")
    /// </summary>
    public string Keys { get; set; } = string.Empty;
    
    /// <summary>
    /// Description (e.g., "Open global command/search palette")
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
