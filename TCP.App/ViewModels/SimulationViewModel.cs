using System.Collections.ObjectModel;
using System.Windows.Input;
using TCP.App.Models.Electronics;
using TCP.App.Services;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TCP.App.ViewModels;

public class SimulationViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private string _simulationStatus = "Hazır. Simülasyonu başlatmak için Play tuşuna basın.";
    public string SimulationStatus
    {
        get => _simulationStatus;
        set { if (_simulationStatus != value) { _simulationStatus = value; OnPropertyChanged(); } }
    }

    private bool _isRunning = false;
    public bool IsRunning
    {
        get => _isRunning;
        set { if (_isRunning != value) { _isRunning = value; OnPropertyChanged(); } }
    }

    public ObservableCollection<object> SimulationItems { get; } = new();

    public ICommand StartSimulationCommand { get; }
    public ICommand StopSimulationCommand { get; }

    public SimulationViewModel()
    {
        StartSimulationCommand = new RelayCommandWithCanExecute<object>(
            _ => StartSimulation(), 
            () => !IsRunning);
            
        StopSimulationCommand = new RelayCommandWithCanExecute<object>(
            _ => StopSimulation(), 
            () => IsRunning);
    }

    private void StartSimulation()
    {
        IsRunning = true;
        SimulationStatus = "Simülasyon çalışıyor...";
        (StartSimulationCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
        (StopSimulationCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
    }

    private void StopSimulation()
    {
        IsRunning = false;
        SimulationStatus = "Simülasyon durduruldu.";
        (StartSimulationCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
        (StopSimulationCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
    }
}
