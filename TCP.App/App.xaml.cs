using System;
using System.Configuration;
using System.Data;
using System.Windows;
using TCP.App.Services;
using TCP.App.ViewModels;
using TCP.App.Models;
using TCP.App.Models.Electronics;
using System.Collections.Generic;

namespace TCP.App;

/// <summary>
/// Interaction logic for App.xaml
/// 
/// TCP-0.5.2: Search Registry initialization
/// TCP-0.8.1: Settings Persistence v1 (Local)
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Settings persistence service instance
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    private static SettingsPersistenceService? _settingsService;
    
    /// <summary>
    /// Loaded settings (cached)
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    private static AppSettings? _loadedSettings;
    
    /// <summary>
    /// Get loaded settings (for use in MainWindow)
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    public static AppSettings? LoadedSettings => _loadedSettings;
    
    /// <summary>
    /// Update loaded settings cache
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    public static void UpdateLoadedSettings(AppSettings settings)
    {
        _loadedSettings = settings;
    }
    
    /// <summary>
    /// Get settings service (for use in MainWindow)
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    public static SettingsPersistenceService SettingsService
    {
        get
        {
            if (_settingsService == null)
            {
                _settingsService = new SettingsPersistenceService();
            }
            return _settingsService;
        }
    }
    
    /// <summary>
    /// Application startup - Initialize SearchRegistry and load settings
    /// TCP-0.5.2: Register all search suggestions
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// TCP-0.9.3: Error Guardrails (No-crash policy) - Global exception handling
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // TCP-0.9.3: Error Guardrails (No-crash policy) - Global exception handlers
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        
        base.OnStartup(e);
        
        // TCP-0.8.1: Load settings on startup
        _settingsService = new SettingsPersistenceService();
        _loadedSettings = _settingsService.Load();
        
        // TCP-0.8.1 Hotfix-2: Theme loading moved to MainWindow Loaded event
        // This prevents StaticResource resolution failures at startup
        // Theme will be loaded AFTER MainWindow is shown
        
        // MainWindow açıldığında LastRoute'e navigate edilecek
        // MainWindow constructor'ında ApplyLoadedSettings() çağrılacak
        
        // Initialize SearchRegistry with all page registrations
        var registry = SearchRegistry.Instance;
        
        // Register Home page
        registry.Register(new SearchItem
        {
            TitleKey = "String.Home",
            Keywords = new[] { "home", "main", "start" },
            Route = "Home"
        });
        
        // Register Electronics page
        registry.Register(new SearchItem
        {
            TitleKey = "String.Electronics",
            Keywords = new[] { "electronics", "boards", "arduino", "mega", "nano" },
            Route = "Electronics"
        });
        
        // TCP-1.0.2: Register Editor page
        registry.Register(new SearchItem
        {
            TitleKey = "String.Editor",
            Keywords = new[] { "editor", "map", "background", "image" },
            Route = "Editor"
        });
        
        // Register Settings page
        registry.Register(new SearchItem
        {
            TitleKey = "String.Settings",
            Keywords = new[] { "settings", "preferences", "options" },
            Route = "Settings"
        });
        
        // TCP-0.8.2: Register Shortcuts page
        registry.Register(new SearchItem
        {
            TitleKey = "String.Shortcuts",
            Keywords = new[] { "shortcuts", "keyboard", "hotkeys", "keys", "ctrl", "f1" },
            Route = "Settings"
        });
        
        // Register Info page
        registry.Register(new SearchItem
        {
            TitleKey = "String.Info",
            Keywords = new[] { "info", "about", "version", "changelog" },
            Route = "Info"
        });
        
        // TCP-0.9.0: Register additional Info routes
        registry.Register(new SearchItem
        {
            TitleKey = "String.AboutTCP",
            Keywords = new[] { "about", "tcp", "overview" },
            Route = "Info"
        });
        
        registry.Register(new SearchItem
        {
            TitleKey = "String.VersionHistory",
            Keywords = new[] { "version", "history", "changelog", "versions" },
            Route = "Info"
        });
        
        registry.Register(new SearchItem
        {
            TitleKey = "String.Architecture",
            Keywords = new[] { "architecture", "mvvm", "wpf", "design" },
            Route = "Info"
        });
        
        // TCP-0.6.0: Initialize BoardRegistry with hardcoded board data
        var boardRegistry = BoardRegistry.Instance;
        boardRegistry.LoadBoards();
        
        if (!boardRegistry.GetAll().Any())
        {
            // Register WT32-ETH01
            boardRegistry.Register(new BoardItem
            {
                Name = "WT32-ETH01",
                Description = "ESP32 board with built-in Ethernet LAN8720 for wired connectivity",
                Status = "Offline",
                SummaryData = new Dictionary<string, string>
                {
                    { "Port", "ETH0" },
                    { "IP Address", "192.168.1.10" },
                    { "Status", "Offline" }
                }
            });
            

            
            // Register RFID Reader
            boardRegistry.Register(new BoardItem
            {
                Name = "RFID Reader",
                Description = "RFID reader/writer module for card detection",
                Status = "Offline",
                SummaryData = new Dictionary<string, string>
                {
                    { "Port", "ETH1" },
                    { "IP Address", "192.168.1.20" },
                    { "Status", "Unknown" }
                }
            });
            
            // Register Relay Controller
            boardRegistry.Register(new BoardItem
            {
                Name = "Relay Controller",
                Description = "Multi-channel relay board for high power switching",
                Status = "Offline",
                SummaryData = new Dictionary<string, string>
                {
                    { "Port", "ETH2" },
                    { "IP Address", "192.168.1.30" },
                    { "Status", "Offline" }
                }
            });
        }
    }
    
    /// <summary>
    /// Global exception handler - UI thread exceptions
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// 
    /// UI thread'de oluşan exception'ları yakalar.
    /// Exception'ı handle eder, loglar ve toast gösterir.
    /// App crash etmez.
    /// </summary>
    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // TCP-0.9.3: Error Guardrails (No-crash policy)
        // Exception'ı logla
        AppLogger.LogException(e.Exception, "DispatcherUnhandledException");
        
        // Exception'ı handle et (app crash etmesin)
        e.Handled = true;
        
        // TCP-0.9.3: Error Guardrails (No-crash policy) - Show error toast
        try
        {
            TerminalService.Instance.LogError("Unexpected Error: An internal error occurred. The application recovered safely.");
        }
        catch
        {
            // Toast gösterilemezse sessizce fail eder
        }
    }
    
    /// <summary>
    /// AppDomain unhandled exception handler
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// 
    /// Non-UI thread'de oluşan exception'ları yakalar.
    /// Exception'ı loglar ve toast gösterir.
    /// </summary>
    private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // TCP-0.9.3: Error Guardrails (No-crash policy)
        if (e.ExceptionObject is Exception ex)
        {
            AppLogger.LogException(ex, "AppDomain_UnhandledException");
            
            // UI thread'de toast göster
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TerminalService.Instance.LogError("Unexpected Error: An internal error occurred. The application recovered safely.");
                });
            }
            catch
            {
                // Toast gösterilemezse sessizce fail eder
            }
        }
    }
    
    /// <summary>
    /// TaskScheduler unobserved task exception handler
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// 
    /// Task exception'larını yakalar.
    /// Exception'ı loglar.
    /// </summary>
    private void TaskScheduler_UnobservedTaskException(object? sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
    {
        // TCP-0.9.3: Error Guardrails (No-crash policy)
        AppLogger.LogException(e.Exception, "TaskScheduler_UnobservedTaskException");
        
        // Exception'ı observed olarak işaretle (app crash etmesin)
        e.SetObserved();
        
        // UI thread'de toast göster
        try
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TerminalService.Instance.LogError("Unexpected Error: An internal error occurred. The application recovered safely.");
            });
        }
        catch
        {
            // Toast gösterilemezse sessizce fail eder
        }
    }
}


