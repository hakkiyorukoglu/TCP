using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Models;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// InfoViewModel - Info modülü ViewModel'i
/// 
/// TCP-0.9.0: Info Panel v1 + Version History UI
/// 
/// Bu ViewModel InfoView'un data context'idir.
/// Info modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (InfoView.xaml) sadece UI gösterir
/// - ViewModel (InfoViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Info modülü state ve iş mantığı yönetimi
/// </summary>
public class InfoViewModel : ViewModelBase, INotifyPropertyChanged
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
    /// Info sections listesi
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// </summary>
    public ObservableCollection<string> Sections { get; }
    
    /// <summary>
    /// Seçili section
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// </summary>
    private string _selectedSection = "Overview";
    public string SelectedSection
    {
        get => _selectedSection;
        set
        {
            if (_selectedSection != value && !string.IsNullOrWhiteSpace(value))
            {
                _selectedSection = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Versiyon geçmişi listesi
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// </summary>
    public ObservableCollection<VersionEntry> VersionHistory { get; }
    
    /// <summary>
    /// Mevcut versiyon bilgisi (VersionManager'dan)
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// </summary>
    public string CurrentVersion => VersionManager.CurrentVersion;
    
    /// <summary>
    /// Stage adı (VersionManager'dan)
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// </summary>
    public string StageName => VersionManager.StageName;
    
    /// <summary>
    /// Constructor - Initialize sections and version history
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// </summary>
    public InfoViewModel()
    {
        Sections = new ObservableCollection<string>
        {
            "Overview",
            "Architecture",
            "Features",
            "Version History"
        };
        
        // TCP-0.9.0: Initialize version history (UI-only data)
        VersionHistory = new ObservableCollection<VersionEntry>
        {
            new VersionEntry
            {
                Version = "TCP-0.9.0",
                Title = "Info Panel v1",
                Description = "Added Info panel with version history and topbar navigation"
            },
            new VersionEntry
            {
                Version = "TCP-0.8.2",
                Title = "Shortcuts Map (UI list)",
                Description = "Added Shortcuts Map UI list in Settings > Shortcuts. Disabled theme switching - app now uses Dark theme only."
            },
            new VersionEntry
            {
                Version = "TCP-0.8.1",
                Title = "Settings Persistence",
                Description = "Introduced local settings persistence. Persisted: Theme, LastRoute, Panel widths. Location: %AppData%/TCP/settings.json"
            },
            new VersionEntry
            {
                Version = "TCP-0.8.0",
                Title = "Settings System v1",
                Description = "Introduced professional Settings page with category navigation"
            }
        };
        
        // Default selection: Overview
        SelectedSection = "Overview";
    }
}
