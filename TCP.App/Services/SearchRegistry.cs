using System.Collections.Generic;
using System.Linq;
using TCP.App.ViewModels;

namespace TCP.App.Services;

/// <summary>
/// SearchRegistry - Centralized search suggestion registry
/// 
/// TCP-0.5.2: Search Registry (Single Source of Truth)
/// 
/// Singleton service that maintains all search suggestions.
/// Prevents duplicate routes.
/// Application-scoped lifetime.
/// </summary>
public class SearchRegistry : ISearchRegistry
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    private static SearchRegistry? _instance;
    
    /// <summary>
    /// Get singleton instance
    /// </summary>
    public static SearchRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SearchRegistry();
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Internal list of search items
    /// </summary>
    private readonly List<SearchItem> _items = new();
    
    /// <summary>
    /// Private constructor (singleton pattern)
    /// </summary>
    private SearchRegistry()
    {
    }
    
    /// <summary>
    /// Get all registered search items
    /// Returns read-only collection
    /// </summary>
    public IEnumerable<SearchItem> GetAll()
    {
        return _items.AsReadOnly();
    }
    
    /// <summary>
    /// Register a search item
    /// Prevents duplicate routes
    /// </summary>
    public void Register(SearchItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        if (string.IsNullOrWhiteSpace(item.Route))
        {
            throw new ArgumentException("Route cannot be null or empty", nameof(item));
        }
        
        // Prevent duplicate routes
        if (IsRouteRegistered(item.Route))
        {
            return; // Silently ignore duplicate registrations
        }
        
        _items.Add(item);
    }
    
    /// <summary>
    /// Check if a route is already registered
    /// </summary>
    public bool IsRouteRegistered(string route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return false;
        }
        
        return _items.Any(item => string.Equals(item.Route, route, StringComparison.OrdinalIgnoreCase));
    }
}
