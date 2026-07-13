using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

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
    /// Keyboard shortcuts listesi
    /// TCP-0.8.2: Shortcuts Map (UI list) + Dark-only theme
    /// </summary>
    public ObservableCollection<ShortcutItem> Shortcuts { get; }
    
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
    /// Desteklenen diller
    /// </summary>
    public ObservableCollection<string> Languages { get; }

    private string _selectedLanguage;
    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }
    }

    private bool _showTerminal;
    public bool ShowTerminal
    {
        get => _showTerminal;
        set
        {
            if (_showTerminal != value)
            {
                _showTerminal = value;
                OnPropertyChanged(nameof(ShowTerminal));
            }
        }
    }

    /// <summary>
    /// Ayarları kaydet komutu
    /// </summary>
    public System.Windows.Input.ICommand SaveCommand { get; }

    /// <summary>
    /// Constructor - Initialize categories and load shortcuts
    /// TCP-0.8.0: Settings System v1
    /// TCP-0.8.2: Shortcuts Map (UI list) + Dark-only theme
    /// </summary>
    public SettingsViewModel()
    {
        Categories = new ObservableCollection<SettingsCategory>
        {
            new SettingsCategory { Id = "Appearance", Name = GetString("String.Appearance"), Description = GetString("String.ThemeDesc") },
            new SettingsCategory { Id = "Shortcuts", Name = GetString("String.Shortcuts"), Description = GetString("String.ShortcutsDesc") },
            new SettingsCategory { Id = "Paths", Name = GetString("String.Paths"), Description = GetString("String.PathsDesc") },
            new SettingsCategory { Id = "Performance", Name = GetString("String.Performance"), Description = GetString("String.PerfDesc") },
            new SettingsCategory { Id = "About", Name = GetString("String.About"), Description = GetString("String.AboutDesc") }
        };
        
        // TCP-0.8.2: Initialize shortcuts from registry
        var shortcutsRegistry = ShortcutsRegistry.Instance;
        Shortcuts = new ObservableCollection<ShortcutItem>(shortcutsRegistry.GetAll());
        
        var settings = App.SettingsService.Load();

        Languages = new ObservableCollection<string> { "tr-TR", "en-US" };
        
        SelectedLanguage = settings.Language;
        ShowTerminal = settings.ShowTerminal;

        SaveCommand = new RelayCommand<object>(_ => SaveSettings());
        
        // Default selection: İlk kategori
        SelectedCategory = Categories.Count > 0 ? Categories[0] : null;
    }

    private string GetString(string key)
    {
        return System.Windows.Application.Current.TryFindResource(key) as string ?? key;
    }

    private void SaveSettings()
    {
        var settings = App.SettingsService.Load();
        settings.Language = SelectedLanguage;
        settings.ShowTerminal = ShowTerminal;
        App.SettingsService.Save(settings);
        App.UpdateLoadedSettings(settings);
        
        LanguageService.ApplyLanguage(SelectedLanguage);
        TerminalService.Instance.SetVisibility(ShowTerminal);
        TerminalService.Instance.LogSuccess("Saved: Settings updated");
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
    /// Kategori ID'si (Trigger'lar için)
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Kategori adı (UI'da gösterilecek isim)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Kategori açıklaması
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

