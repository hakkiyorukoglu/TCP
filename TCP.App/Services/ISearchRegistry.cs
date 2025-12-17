using System.Collections.Generic;
using TCP.App.ViewModels;

namespace TCP.App.Services;

/// <summary>
/// ISearchRegistry - Search registry interface
/// 
/// TCP-0.5.2: Search Registry (Single Source of Truth)
/// 
/// Centralized registry for search suggestions.
/// Each page/module registers itself with title, keywords, and route.
/// </summary>
public interface ISearchRegistry
{
    /// <summary>
    /// Get all registered search items
    /// </summary>
    IEnumerable<SearchItem> GetAll();
    
    /// <summary>
    /// Register a search item
    /// </summary>
    void Register(SearchItem item);
    
    /// <summary>
    /// Check if a route is already registered
    /// </summary>
    bool IsRouteRegistered(string route);
}
