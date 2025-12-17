using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// EditorViewModel - Editor modülü ViewModel'i
/// 
/// Bu ViewModel EditorView'un data context'idir.
/// Editor modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (EditorView.xaml) sadece UI gösterir
/// - ViewModel (EditorViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Editor modülü state ve iş mantığı yönetimi
/// </summary>
public class EditorViewModel : ViewModelBase, INotifyPropertyChanged
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
    
    // Gelecekte editor modülü property'leri buraya eklenecek
}
