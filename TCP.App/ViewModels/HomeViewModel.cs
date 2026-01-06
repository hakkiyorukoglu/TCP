using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// HomeViewModel - Ana sayfa ViewModel'i
/// 
/// TCP-1.0.1: Home Map Canvas (Empty)
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
    
    /// <summary>
    /// Load Map Image Command
    /// TCP-1.0.1: Home Map Canvas (Empty)
    /// 
    /// Bu command "Load Map Image" butonuna tıklandığında çalışır.
    /// TCP-1.0.1'de sadece toast mesajı gösterir (implementasyon TCP-1.0.2'de gelecek).
    /// </summary>
    public ICommand LoadMapImageCommand { get; }
    
    /// <summary>
    /// Constructor - Initialize commands
    /// TCP-1.0.1: Home Map Canvas (Empty)
    /// </summary>
    public HomeViewModel()
    {
        // TCP-1.0.1: Initialize LoadMapImageCommand
        LoadMapImageCommand = new RelayCommand<object>(_ => LoadMapImage());
    }
    
    /// <summary>
    /// Load Map Image command implementation
    /// TCP-1.0.1: Home Map Canvas (Empty)
    /// 
    /// TCP-1.0.1'de sadece toast mesajı gösterir.
    /// Gerçek implementasyon TCP-1.0.2'de gelecek.
    /// </summary>
    private void LoadMapImage()
    {
        try
        {
            // TCP-1.0.1: Show info toast (not implemented yet)
            NotificationService.Instance.ShowInfo(
                "Not implemented", 
                "Not implemented in TCP-1.0.1. Coming in TCP-1.0.2."
            );
        }
        catch
        {
            // TCP-1.0.1: Safety - ignore exceptions during toast display
        }
    }
}
