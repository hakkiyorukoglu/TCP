using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// ElectronicsViewModel - Electronics modülü ViewModel'i
/// 
/// Bu ViewModel ElectronicsView'un data context'idir.
/// Electronics modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (ElectronicsView.xaml) sadece UI gösterir
/// - ViewModel (ElectronicsViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Electronics modülü state ve iş mantığı yönetimi
/// </summary>
public class ElectronicsViewModel : ViewModelBase, INotifyPropertyChanged
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
    
    // Gelecekte electronics modülü property'leri buraya eklenecek
}
