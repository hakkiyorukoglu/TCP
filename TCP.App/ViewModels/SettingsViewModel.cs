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
    /// Constructor - Initialize categories and load shortcuts
    /// TCP-0.8.0: Settings System v1
    /// TCP-0.8.2: Shortcuts Map (UI list) + Dark-only theme
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
        
        // TCP-0.8.2: Initialize shortcuts from registry
        var shortcutsRegistry = ShortcutsRegistry.Instance;
        Shortcuts = new ObservableCollection<ShortcutItem>(shortcutsRegistry.GetAll());
        
        // Default selection: İlk kategori
        SelectedCategory = Categories.Count > 0 ? Categories[0] : null;
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
