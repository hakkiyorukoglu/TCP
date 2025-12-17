using System;
using System.Configuration;
using System.Data;
using System.Windows;
using TCP.App.Services;
using TCP.App.ViewModels;
using TCP.App.Models;
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
        
        // TCP-0.8.2: Register Shortcuts page
        registry.Register(new SearchItem
        {
            Title = "Shortcuts",
            Keywords = new[] { "shortcuts", "keyboard", "hotkeys", "keys", "ctrl", "f1" },
            Route = "Settings"
        });
        
        // Register Info page
        registry.Register(new SearchItem
        {
            Title = "Info",
            Keywords = new[] { "info", "about", "version", "changelog" },
            Route = "Info"
        });
        
        // TCP-0.9.0: Register additional Info routes
        registry.Register(new SearchItem
        {
            Title = "About TCP",
            Keywords = new[] { "about", "tcp", "overview" },
            Route = "Info"
        });
        
        registry.Register(new SearchItem
        {
            Title = "Version History",
            Keywords = new[] { "version", "history", "changelog", "versions" },
            Route = "Info"
        });
        
        registry.Register(new SearchItem
        {
            Title = "Architecture",
            Keywords = new[] { "architecture", "mvvm", "wpf", "design" },
            Route = "Info"
        });
        
        // TCP-0.6.0: Initialize BoardRegistry with hardcoded board data
        var boardRegistry = BoardRegistry.Instance;
        
        // Register Arduino Mega
        boardRegistry.Register(new BoardItem
        {
            Name = "Arduino Mega",
            Description = "ATmega2560 microcontroller board with 54 digital I/O pins",
            Status = "Offline",
            SummaryData = new Dictionary<string, string>
            {
                { "Port", "COM3" },
                { "IP Address", "192.168.0.10" },
                { "Status", "Offline" }
            }
        });
        
        // Register Arduino Nano
        boardRegistry.Register(new BoardItem
        {
            Name = "Arduino Nano",
            Description = "Compact ATmega328P board with 14 digital I/O pins",
            Status = "Offline",
            SummaryData = new Dictionary<string, string>
            {
                { "Port", "COM5" },
                { "IP Address", "192.168.0.11" },
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
                { "Port", "ETH0" },
                { "IP Address", "192.168.0.20" },
                { "Status", "Unknown" }
            }
        });
        
        // Register Servo Controller
        boardRegistry.Register(new BoardItem
        {
            Name = "Servo Controller",
            Description = "Multi-channel servo motor controller board",
            Status = "Offline",
            SummaryData = new Dictionary<string, string>
            {
                { "Port", "COM7" },
                { "IP Address", "192.168.0.30" },
                { "Status", "Offline" }
            }
        });
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
            NotificationService.Instance.ShowError("Unexpected Error", "An internal error occurred. The application recovered safely.");
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
                    NotificationService.Instance.ShowError("Unexpected Error", "An internal error occurred. The application recovered safely.");
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
                NotificationService.Instance.ShowError("Unexpected Error", "An internal error occurred. The application recovered safely.");
            });
        }
        catch
        {
            // Toast gösterilemezse sessizce fail eder
        }
    }
}

