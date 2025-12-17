using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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
    /// Seçili tema (UI'da seçilen, henüz uygulanmamış)
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// 
    /// Setter'da theme apply edilmez.
    /// ApplyThemeCommand ile explicit olarak uygulanır.
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
            }
        }
    }
    
    /// <summary>
    /// Uygulanmış tema (şu anda aktif olan tema)
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// </summary>
    private string _appliedTheme = "Dark";
    public string AppliedTheme
    {
        get => _appliedTheme;
        private set
        {
            if (_appliedTheme != value)
            {
                _appliedTheme = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Apply Theme Command
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// 
    /// Bu command theme'i güvenli bir şekilde uygular:
    /// 1. SettingsView kapatılır (navigation back)
    /// 2. ThemeService.ApplyTheme() çağrılır (Dispatcher.Invoke ile)
    /// 3. Settings persist edilir
    /// </summary>
    public ICommand ApplyThemeCommand { get; }
    
    /// <summary>
    /// Theme apply event
    /// SettingsView kapatıldıktan sonra theme apply edilecek
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// </summary>
    public event Action<string>? ThemeApplyRequested;
    
    /// <summary>
    /// Constructor - Initialize categories and load theme
    /// TCP-0.8.0: Settings System v1
    /// TCP-0.8.1: Safe Theme Apply with Save Button
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
            _appliedTheme = settings.Theme;
        }
        else
        {
            _selectedTheme = "Dark"; // Default
            _appliedTheme = "Dark"; // Default
        }
        
        // TCP-0.8.1: Initialize ApplyThemeCommand
        ApplyThemeCommand = new RelayCommand<object>(_ => ApplyTheme());
    }
    
    /// <summary>
    /// Theme'i uygula (command execute)
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// 
    /// Bu metod:
    /// 1. SelectedTheme == AppliedTheme kontrolü yapar
    /// 2. ThemeApplyRequested event'ini tetikler (SettingsView kapatılacak)
    /// 3. Event handler'da theme apply edilecek
    /// 4. AppliedTheme'i günceller
    /// </summary>
    private void ApplyTheme()
    {
        // Eğer seçili tema zaten uygulanmışsa işlem yapma
        if (SelectedTheme == AppliedTheme)
        {
            return;
        }
        
        // AppliedTheme'i güncelle (UI feedback için)
        AppliedTheme = SelectedTheme;
        
        // ThemeApplyRequested event'ini tetikle
        // MainWindow'da bu event handle edilecek ve SettingsView kapatıldıktan sonra theme apply edilecek
        ThemeApplyRequested?.Invoke(SelectedTheme);
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
