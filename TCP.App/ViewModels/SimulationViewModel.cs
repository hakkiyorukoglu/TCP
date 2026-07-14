using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using System;
using TCP.App.Models.Electronics;
using TCP.App.Services;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TCP.App.ViewModels;

public class SimulationViewModel : ViewModelBase, INotifyPropertyChanged
{
    private DispatcherTimer? _gameLoopTimer;
    private DateTime _lastFrameTime;

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
    public ObservableCollection<TrainInstance> Trains { get; } = new();

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
            
            Trains.Clear();
            foreach(var t in NetworkManager.Instance.Trains)
            {
                Trains.Add(t);
                // Eğer trene atanmış bir Route yoksa veya üzerinde değilse, rastgele ilk Route'a koy
                if (Routes.Any())
                {
                    var firstRoute = Routes.First();
                    if (firstRoute.Nodes.Any())
                    {
                        t.X = firstRoute.Nodes.First().X;
                        t.Y = firstRoute.Nodes.First().Y;
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
        
        _lastFrameTime = DateTime.Now;
        if (_gameLoopTimer == null)
        {
            _gameLoopTimer = new DispatcherTimer();
            _gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60fps
            _gameLoopTimer.Tick += GameLoopTimer_Tick;
        }
        _gameLoopTimer.Start();
    }

    private void StopSimulation()
    {
        IsRunning = false;
        SimulationStatus = "Simülasyon durduruldu.";
        (StartSimulationCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
        (StopSimulationCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
        
        _gameLoopTimer?.Stop();
    }
    
    private void GameLoopTimer_Tick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        var dt = (now - _lastFrameTime).TotalSeconds;
        _lastFrameTime = now;

        UpdateTrains(dt);
    }
    
    private void UpdateTrains(double dt)
    {
        double speed = 100.0; // piksel/saniye hızı
        
        foreach (var train in Trains)
        {
            if (!Routes.Any()) continue;
            
            // Çok basit bir hareket simülasyonu: 
            // Şimdilik sadece ilk route'un node'ları arasında git gel yapacak.
            var route = Routes.First();
            if (route.Nodes.Count < 2) continue;
            
            // Eğer tren node'lara göre progress'i yoksa, bunu tutacak bir property lazım.
            // Biz şimdilik mesafe bazlı lineer interpolasyon yapalım.
            // Treni ilk node'dan son node'a doğru hareket ettirelim
            
            // (İleride bunu TrackNode segmentleri ve Spline üzerinde PathMath kullanarak güncelleyeceğiz.)
            // Basitlik için sadece ilk ve son node arası gidiyor diyelim
            var startNode = route.Nodes.First();
            var endNode = route.Nodes.Last();
            
            double dx = endNode.X - train.X;
            double dy = endNode.Y - train.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            
            if (dist > 2)
            {
                train.X += (dx / dist) * speed * dt;
                train.Y += (dy / dist) * speed * dt;
            }
            else
            {
                // Başa dön
                train.X = startNode.X;
                train.Y = startNode.Y;
            }
        }
    }
}
