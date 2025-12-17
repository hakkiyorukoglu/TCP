using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// SimulationViewModel - Simulation modülü ViewModel'i
/// 
/// Bu ViewModel SimulationView'un data context'idir.
/// Simulation modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (SimulationView.xaml) sadece UI gösterir
/// - ViewModel (SimulationViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Simulation modülü state ve iş mantığı yönetimi
/// </summary>
public class SimulationViewModel : ViewModelBase, INotifyPropertyChanged
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
    
    // Gelecekte simulation modülü property'leri buraya eklenecek
}
