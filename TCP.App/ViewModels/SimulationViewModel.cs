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

    public ObservableCollection<TCP.App.Models.Editor.EditorImage> EditorImages { get; } = new();
    public ObservableCollection<TCP.App.Models.Editor.ILayerItem> PlacedBoxes { get; } = new();
    public ObservableCollection<TrackRoute> Routes { get; } = new();
    public ObservableCollection<ConnectionLine> DaisyChainLines { get; } = new();

    public ICommand StartSimulationCommand { get; }
    public ICommand StopSimulationCommand { get; }
    public ICommand LoadLayoutCommand { get; }

    public SimulationViewModel()
    {
        StartSimulationCommand = new RelayCommandWithCanExecute<object>(
            _ => StartSimulation(), 
            () => !IsRunning);
            
        StopSimulationCommand = new RelayCommandWithCanExecute<object>(
            _ => StopSimulation(), 
            () => IsRunning);

        LoadLayoutCommand = new RelayCommand<object>(_ => LoadLayout());

        // Ekrana girildiğinde aktif senaryonun haritasını otomatik yükle
        LoadLayout();
    }

    private void LoadLayout()
    {
        var currentScenario = ProjectManager.Instance.CurrentScenario;
        if (currentScenario == null)
        {
            SimulationStatus = "Hata: Yüklü bir senaryo bulunamadı.";
            return;
        }

        if (string.IsNullOrWhiteSpace(currentScenario.EditorLayoutJson) || currentScenario.EditorLayoutJson == "{}")
        {
            SimulationStatus = "Hata: Senaryo içinde kayıtlı bir harita/şablon yok.";
            return;
        }

        var state = EditorLayoutService.Instance.LoadFromJson(currentScenario.EditorLayoutJson);
        if (state != null)
        {
            EditorImages.Clear();
            foreach (var img in state.Images) EditorImages.Add(img);

            Routes.Clear();
            foreach (var route in state.Routes) Routes.Add(route);

            PlacedBoxes.Clear();
            var _networkManager = NetworkManager.Instance;
            foreach (var stState in state.PlacedItems)
            {
                if (_networkManager.MainPc.Id == stState.ItemId)
                {
                    _networkManager.MainPc.X = stState.X; _networkManager.MainPc.Y = stState.Y;
                    PlacedBoxes.Add(_networkManager.MainPc);
                    continue;
                }
                var modem = System.Linq.Enumerable.FirstOrDefault(_networkManager.Modems, m => m.Id == stState.ItemId);
                if (modem != null)
                {
                    modem.X = stState.X; modem.Y = stState.Y;
                    PlacedBoxes.Add(modem);
                    continue;
                }
                var st = System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.SelectMany(_networkManager.Modems, m => m.Stations), s => s.Id == stState.ItemId);
                if (st != null)
                {
                    st.X = stState.X; st.Y = stState.Y;
                    PlacedBoxes.Add(st);
                    continue;
                }
                foreach(var m in _networkManager.Modems)
                {
                    foreach(var station in m.Stations)
                    {
                        var comp = System.Linq.Enumerable.FirstOrDefault(station.Components, c => c.Id == stState.ItemId);
                        if (comp != null)
                        {
                            comp.X = stState.X; comp.Y = stState.Y;
                            PlacedBoxes.Add(comp);
                            break;
                        }
                    }
                }
            }
            SyncDaisyChainLines();
            SimulationStatus = $"Senaryo '{currentScenario.Name}' haritası yüklendi.";
        }
        else
        {
            SimulationStatus = "Hata: Harita JSON'dan çözümlenemedi.";
        }
    }

    private void SyncDaisyChainLines()
    {
        var _networkManager = NetworkManager.Instance;
        DaisyChainLines.Clear();
        foreach (var modem in _networkManager.Modems)
        {
            if (modem.IncomingConnectionId.HasValue)
            {
                TCP.App.Models.Editor.ILayerItem? source = null;
                if (modem.IncomingConnectionId.Value == _networkManager.MainPc.Id)
                    source = _networkManager.MainPc;
                else
                    source = System.Linq.Enumerable.FirstOrDefault(_networkManager.Modems, m => m.Id == modem.IncomingConnectionId.Value);

                if (source != null && PlacedBoxes.Contains(source) && PlacedBoxes.Contains(modem))
                    DaisyChainLines.Add(new ConnectionLine { Source = source, Target = modem, LineType = "Modem" });
            }

            foreach(var st in modem.Stations)
            {
                if (PlacedBoxes.Contains(modem) && PlacedBoxes.Contains(st))
                    DaisyChainLines.Add(new ConnectionLine { Source = modem, Target = st, LineType = "Station" });

                foreach(var c in st.Components)
                {
                    if (PlacedBoxes.Contains(st) && PlacedBoxes.Contains(c))
                        DaisyChainLines.Add(new ConnectionLine { Source = st, Target = c, LineType = "Component" });
                }
            }
        }
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
