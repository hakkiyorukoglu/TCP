using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// SettingsViewModel - Settings modülü ViewModel'i
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
    
    // Gelecekte settings modülü property'leri buraya eklenecek
}
