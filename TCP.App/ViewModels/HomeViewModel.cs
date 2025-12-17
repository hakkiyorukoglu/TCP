using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// HomeViewModel - Ana sayfa ViewModel'i
/// 
/// Bu ViewModel HomeView'un data context'idir.
/// Ana sayfa içeriğini ve state'ini yönetir.
/// 
/// MVVM Pattern:
/// - View (HomeView.xaml) sadece UI gösterir
/// - ViewModel (HomeViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Ana sayfa state yönetimi
/// </summary>
public class HomeViewModel : ViewModelBase, INotifyPropertyChanged
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
    
    // Gelecekte home page property'leri buraya eklenecek
}
