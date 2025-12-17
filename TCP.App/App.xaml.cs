using System.Configuration;
using System.Data;
using System.Windows;
using TCP.App.Services;
using TCP.App.ViewModels;

namespace TCP.App;

/// <summary>
/// Interaction logic for App.xaml
/// 
/// TCP-0.5.2: Search Registry initialization
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Application startup - Initialize SearchRegistry
    /// TCP-0.5.2: Register all search suggestions
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Initialize SearchRegistry with all page registrations
        var registry = SearchRegistry.Instance;
        
        // Register Home page
        registry.Register(new SearchItem
        {
            Title = "Home",
            Keywords = new[] { "home", "main", "start" },
            Route = "Home"
        });
        
        // Register Electronics page
        registry.Register(new SearchItem
        {
            Title = "Electronics",
            Keywords = new[] { "electronics", "boards", "arduino", "mega", "nano" },
            Route = "Electronics"
        });
        
        // Register Simulation page
        registry.Register(new SearchItem
        {
            Title = "Simulation",
            Keywords = new[] { "simulation", "simulate", "run" },
            Route = "Simulation"
        });
        
        // Register Settings page
        registry.Register(new SearchItem
        {
            Title = "Settings",
            Keywords = new[] { "settings", "preferences", "options" },
            Route = "Settings"
        });
        
        // Register Info page
        registry.Register(new SearchItem
        {
            Title = "Info",
            Keywords = new[] { "info", "about", "version", "changelog" },
            Route = "Info"
        });
    }
}

