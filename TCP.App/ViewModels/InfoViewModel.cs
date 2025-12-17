using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TCP.App.Models;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// InfoViewModel - Info modülü ViewModel'i
/// 
/// TCP-0.9.1: Info panel & TXT export
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
    /// TCP-0.9.1: Info panel & TXT export - Roadmap section eklendi
    /// </summary>
    public ObservableCollection<string> Sections { get; }
    
    /// <summary>
    /// Seçili section
    /// TCP-0.9.1: Info panel & TXT export
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
    /// TCP-0.9.1: Info panel & TXT export - InfoContentProvider'dan gelir
    /// </summary>
    public ObservableCollection<VersionEntry> VersionHistory { get; }
    
    /// <summary>
    /// Overview metni
    /// TCP-0.9.1: Info panel & TXT export
    /// TCP-0.9.1a: Info panel DataContext fix - Property backing field for safety
    /// </summary>
    private string? _overviewText;
    public string OverviewText
    {
        get
        {
            if (string.IsNullOrEmpty(_overviewText))
            {
                try
                {
                    _overviewText = InfoContentProvider.GetOverview();
                }
                catch
                {
                    _overviewText = "Info data failed to load. Please check InfoContentProvider.";
                }
            }
            return _overviewText ?? string.Empty;
        }
    }
    
    /// <summary>
    /// Architecture points listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public ObservableCollection<string> ArchitecturePoints { get; }
    
    /// <summary>
    /// Implemented features listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public ObservableCollection<string> ImplementedFeatures { get; }
    
    /// <summary>
    /// In Progress features listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public ObservableCollection<string> InProgressFeatures { get; }
    
    /// <summary>
    /// Planned features listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public ObservableCollection<string> PlannedFeatures { get; }
    
    /// <summary>
    /// Roadmap items listesi
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public ObservableCollection<string> RoadmapItems { get; }
    
    /// <summary>
    /// Mevcut versiyon bilgisi (VersionManager'dan)
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public string CurrentVersion => VersionManager.CurrentVersion;
    
    /// <summary>
    /// Stage adı (VersionManager'dan)
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public string StageName => VersionManager.StageName;
    
    /// <summary>
    /// Export TXT command
    /// TCP-0.9.1: Info panel & TXT export
    /// </summary>
    public ICommand ExportTxtCommand { get; }
    
    /// <summary>
    /// Export Cursor Context TXT command
    /// TCP-1.0.0: UI FOUNDATION RELEASE
    /// </summary>
    public ICommand ExportCursorContextCommand { get; }
    
    /// <summary>
    /// Cursor context metni backing field
    /// TCP-1.0.0: UI FOUNDATION RELEASE
    /// </summary>
    private string? _cursorContextText;
    
    /// <summary>
    /// Cursor context metni
    /// TCP-1.0.0: UI FOUNDATION RELEASE
    /// </summary>
    public string CursorContextText
    {
        get
        {
            if (string.IsNullOrEmpty(_cursorContextText))
            {
                try
                {
                    _cursorContextText = InfoContentProvider.GetCursorContext();
                }
                catch
                {
                    _cursorContextText = "Cursor context failed to load. Please check InfoContentProvider.";
                }
            }
            return _cursorContextText ?? string.Empty;
        }
    }
    
    /// <summary>
    /// Constructor - Initialize sections and content from InfoContentProvider
    /// TCP-0.9.1: Info panel & TXT export
    /// TCP-0.9.1a: Info panel DataContext fix - Force immediate initialization
    /// </summary>
    public InfoViewModel()
    {
        // TCP-0.9.1a: Initialize OverviewText immediately (no lazy loading)
        try
        {
            _overviewText = InfoContentProvider.GetOverview();
        }
        catch
        {
            _overviewText = "Info data failed to load. Please check InfoContentProvider.";
        }
        
        Sections = new ObservableCollection<string>
        {
            "Overview",
            "Architecture",
            "Features",
            "Version History",
            "Roadmap",
            "Cursor"
        };
        
        // TCP-0.9.1: Initialize content from InfoContentProvider (single source of truth)
        // TCP-0.9.1a: All collections MUST be initialized immediately (no lazy loading)
        try
        {
            VersionHistory = new ObservableCollection<VersionEntry>(InfoContentProvider.GetVersionHistory());
            ArchitecturePoints = new ObservableCollection<string>(InfoContentProvider.GetArchitecturePoints());
            ImplementedFeatures = new ObservableCollection<string>(InfoContentProvider.GetImplementedFeatures());
            InProgressFeatures = new ObservableCollection<string>(InfoContentProvider.GetInProgressFeatures());
            PlannedFeatures = new ObservableCollection<string>(InfoContentProvider.GetPlannedFeatures());
            RoadmapItems = new ObservableCollection<string>(InfoContentProvider.GetRoadmap());
        }
        catch
        {
            // TCP-0.9.1a: Fallback to empty collections if provider fails
            VersionHistory = new ObservableCollection<VersionEntry>();
            ArchitecturePoints = new ObservableCollection<string>();
            ImplementedFeatures = new ObservableCollection<string>();
            InProgressFeatures = new ObservableCollection<string>();
            PlannedFeatures = new ObservableCollection<string>();
            RoadmapItems = new ObservableCollection<string>();
        }
        
        // TCP-0.9.1: Export TXT command
        ExportTxtCommand = new RelayCommand<object>(_ => ExportTxt());
        
        // TCP-1.0.0: Export Cursor Context TXT command
        ExportCursorContextCommand = new RelayCommand<object>(_ => ExportCursorContext());
        
        // TCP-1.0.0: Initialize Cursor context text immediately
        try
        {
            _cursorContextText = InfoContentProvider.GetCursorContext();
        }
        catch
        {
            _cursorContextText = "Cursor context failed to load. Please check InfoContentProvider.";
        }
        
        // Default selection: Overview
        SelectedSection = "Overview";
    }
    
    /// <summary>
    /// TXT export işlemi
    /// TCP-0.9.1: Info panel & TXT export
    /// TCP-0.9.2: Notifications / Toasts v1 - Export notifications fix
    /// </summary>
    private void ExportTxt()
    {
        try
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "TCP_Project_Info.txt",
                DefaultExt = "txt"
            };
            
            // TCP-0.9.2: Notifications / Toasts v1 - Export notifications fix
            // Dialog aç ve kullanıcı seçimini kontrol et
            var dialogResult = saveDialog.ShowDialog();
            
            if (dialogResult == true)
            {
                // Kullanıcı dosya seçti ve OK'a tıkladı
                try
                {
                    var content = InfoContentProvider.GenerateTxtContent();
                    System.IO.File.WriteAllText(saveDialog.FileName, content);
                    
                    // TCP-0.9.2: Notifications / Toasts v1 - Success notification
                    NotificationService.Instance.ShowSuccess("Export Completed", "Project information was exported as TXT.");
                }
                catch (Exception ex)
                {
                    // TCP-0.9.2: Notifications / Toasts v1 - Error notification
                    NotificationService.Instance.ShowError("Export Failed", ex.Message);
                }
            }
            else
            {
                // TCP-0.9.2: Notifications / Toasts v1 - Cancel notification (optional but clean)
                // Kullanıcı dialog'u iptal etti
                NotificationService.Instance.ShowWarning("Export Canceled", "TXT export was canceled by the user.");
            }
        }
        catch (Exception ex)
        {
            // TCP-0.9.2: Notifications / Toasts v1 - Error notification
            // Dialog açılırken veya başka bir hata oluştu
            NotificationService.Instance.ShowError("Export Failed", ex.Message);
        }
    }
    
    /// <summary>
    /// Cursor Context TXT export işlemi
    /// TCP-1.0.0: UI FOUNDATION RELEASE
    /// </summary>
    private void ExportCursorContext()
    {
        try
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = $"TCP_Cursor_Context_{VersionManager.CurrentVersion}.txt",
                DefaultExt = "txt"
            };
            
            var dialogResult = saveDialog.ShowDialog();
            
            if (dialogResult == true)
            {
                try
                {
                    var content = InfoContentProvider.GenerateCursorContextTxt();
                    System.IO.File.WriteAllText(saveDialog.FileName, content);
                    
                    NotificationService.Instance.ShowSuccess("Export Completed", "Cursor context was exported as TXT.");
                }
                catch (Exception ex)
                {
                    NotificationService.Instance.ShowError("Export Failed", ex.Message);
                }
            }
            else
            {
                NotificationService.Instance.ShowWarning("Export Canceled", "Cursor context export was canceled by the user.");
            }
        }
        catch (Exception ex)
        {
            NotificationService.Instance.ShowError("Export Failed", ex.Message);
        }
    }
}

