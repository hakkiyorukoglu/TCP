using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;
using TCP.App.Models;

namespace TCP.App.ViewModels;

/// <summary>
/// SettingsViewModel - Settings modülü ViewModel'i
/// 
/// TCP-0.8.0: Settings System v1
/// TCP-0.8.1: Theme Selection Fix
/// 
/// Bu ViewModel SettingsView'un data context'idir.
/// Settings modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (SettingsView.xaml) sadece UI gösterir
/// - ViewModel (SettingsViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Settings modülü state ve iş mantığı yönetimi
/// </summary>
public class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
{
    /// <summary>
    /// PropertyChanged event - UI binding'ler için
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// PropertyChanged event'ini tetikler
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /// <summary>
    /// Settings kategorileri listesi
    /// TCP-0.8.0: Settings System v1
    /// </summary>
    public ObservableCollection<SettingsCategory> Categories { get; }
    
    /// <summary>
    /// Mevcut temalar listesi
    /// TCP-0.8.1: Theme Selection Fix
    /// </summary>
    public ObservableCollection<string> AvailableThemes { get; }
    
    /// <summary>
    /// Seçili kategori
    /// TCP-0.8.0: Settings System v1
    /// </summary>
    private SettingsCategory? _selectedCategory;
    public SettingsCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Seçili tema
    /// TCP-0.8.1: Theme Selection Fix
    /// 
    /// Setter'da:
    /// - ThemeService.ApplyTheme() çağrılır (immediate apply)
    /// - SettingsPersistenceService.Save() çağrılır (persist)
    /// </summary>
    private string _selectedTheme = "Dark";
    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value && !string.IsNullOrWhiteSpace(value))
            {
                _selectedTheme = value;
                OnPropertyChanged();
                
                // TCP-0.8.1: Theme Selection Fix
                // Theme'i hemen uygula
                try
                {
                    ThemeService.ApplyTheme(value);
                    
                    // Settings'i kaydet
                    var settings = App.LoadedSettings ?? new AppSettings();
                    settings.Theme = value;
                    App.SettingsService.Save(settings);
                    App.UpdateLoadedSettings(settings);
                }
                catch
                {
                    // Exception durumunda sessizce fail eder
                    // App crash etmez
                }
            }
        }
    }
    
    /// <summary>
    /// Constructor - Initialize categories and load theme
    /// TCP-0.8.0: Settings System v1
    /// TCP-0.8.1: Theme Selection Fix
    /// </summary>
    public SettingsViewModel()
    {
        Categories = new ObservableCollection<SettingsCategory>
        {
            new SettingsCategory { Name = "Appearance", Description = "Theme and visual settings" },
            new SettingsCategory { Name = "Shortcuts", Description = "Keyboard shortcuts" },
            new SettingsCategory { Name = "Paths", Description = "Default paths and directories" },
            new SettingsCategory { Name = "Performance", Description = "Performance options" },
            new SettingsCategory { Name = "About", Description = "Application information" }
        };
        
        // TCP-0.8.1: Initialize available themes
        AvailableThemes = new ObservableCollection<string> { "Dark", "Light" };
        
        // Default selection: İlk kategori
        SelectedCategory = Categories.Count > 0 ? Categories[0] : null;
        
        // TCP-0.8.1: Load theme from settings
        var settings = App.LoadedSettings;
        if (settings != null && !string.IsNullOrWhiteSpace(settings.Theme))
        {
            _selectedTheme = settings.Theme;
        }
        else
        {
            _selectedTheme = "Dark"; // Default
        }
    }
}

/// <summary>
/// SettingsCategory - Settings kategori model class
/// 
/// TCP-0.8.0: Settings System v1
/// </summary>
public class SettingsCategory
{
    /// <summary>
    /// Kategori adı
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Kategori açıklaması
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
