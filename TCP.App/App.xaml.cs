using System.Configuration;
using System.Data;
using System.Windows;
using TCP.App.Services;
using TCP.App.ViewModels;
using System.Collections.Generic;

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
        // Exception handling for debugging
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        
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

