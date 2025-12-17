using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// MainViewModel - Ana pencere ViewModel'i
/// 
/// Bu ViewModel MainWindow'un data context'idir.
/// Navigation ve genel uygulama state'ini yönetir.
/// 
/// MVVM Pattern:
/// - View (MainWindow.xaml) sadece UI gösterir
/// - ViewModel (MainViewModel) tüm mantığı içerir
/// - Model (gelecekte TCP.Core'da) veri ve iş mantığını içerir
/// 
/// Single Responsibility: Ana pencere state ve navigation yönetimi
/// </summary>
public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    /// <summary>
    /// PropertyChanged event - UI binding'ler için
    /// MVVM pattern'de ViewModel property'leri değiştiğinde UI otomatik güncellenir
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// PropertyChanged event'ini tetikler
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    // Gelecekte navigation ve state property'leri buraya eklenecek
    // Şu an için minimal yapı - TCP-0.3'te navigation eklenecek
}
