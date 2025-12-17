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
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Exception handling for debugging
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        
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
        
        // Register Info page
        registry.Register(new SearchItem
        {
            Title = "Info",
            Keywords = new[] { "info", "about", "version", "changelog" },
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
    /// Unhandled exception handler - Debug için
    /// </summary>
    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // Hata mesajını göster
        MessageBox.Show($"Hata: {e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}", 
                       "TCP - Unhandled Exception", 
                       MessageBoxButton.OK, 
                       MessageBoxImage.Error);
        
        // Uygulamanın kapanmasını engelle (debug için)
        e.Handled = true;
    }
}

