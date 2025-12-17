using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// InfoPanelViewModel - Info panel ViewModel'i
/// 
/// Bu ViewModel InfoPanel'in data context'idir.
/// Versiyon bilgilerini ve uygulama hakkında bilgileri yönetir.
/// 
/// MVVM Pattern:
/// - View (InfoPanel.xaml) sadece UI gösterir
/// - ViewModel (InfoPanelViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Info panel state ve versiyon bilgisi yönetimi
/// </summary>
public class InfoPanelViewModel : ViewModelBase, INotifyPropertyChanged
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
    /// Versiyon bilgisi - VersionManager'dan alınır
    /// </summary>
    public string Version => VersionManager.CurrentVersion;
    
    /// <summary>
    /// Build time bilgisi - VersionManager'dan alınır
    /// </summary>
    public DateTime BuildTime => VersionManager.BuildTime;
    
    /// <summary>
    /// Stage adı - VersionManager'dan alınır
    /// </summary>
    public string StageName => VersionManager.StageName;
}
