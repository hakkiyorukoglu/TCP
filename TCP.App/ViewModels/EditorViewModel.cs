using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TCP.App.Models.Editor;
using TCP.App.Models.Electronics;
using TCP.App.Services;
using System.Linq;

namespace TCP.App.ViewModels;

public class ConnectionLine
{
    public ILayerItem Source { get; set; } = null!;
    public ILayerItem Target { get; set; } = null!;
    public string LineType { get; set; } = "Modem";
}

public enum EditorImageMode
{
    Fit,
    Actual
}

public class EditorViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly NetworkManager _networkManager;
    
    public ViewportState ViewportState { get; }
    public EditorInputRouter InputRouter { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public ObservableCollection<ILayerItem> Layers { get; } = new();

    private ILayerItem? _selectedLayerItem;
    public ILayerItem? SelectedLayerItem
    {
        get => _selectedLayerItem;
        set
        {
            if (_selectedLayerItem != value)
            {
                if (_selectedLayerItem != null) _selectedLayerItem.IsSelected = false;
                _selectedLayerItem = value;
                if (_selectedLayerItem != null) _selectedLayerItem.IsSelected = true;
                
                SelectedImage = _selectedLayerItem as EditorImage;
                
                OnPropertyChanged();
                if (RemoveSelectedLayerCommand is RelayCommandWithCanExecute<object> rcmd)
                    rcmd.RaiseCanExecuteChanged();
                if (EditLayerPropertiesCommand is RelayCommandWithCanExecute<object> ecmd)
                    ecmd.RaiseCanExecuteChanged();
            }
        }
    }
    
    public ICommand RemoveSelectedLayerCommand { get; }
    public ICommand EditLayerPropertiesCommand { get; }
    
    // Component Binding
    public ICommand BindComponentCommand { get; }
    public ICommand SaveComponentBindingCommand { get; }
    public ICommand CancelComponentBindingCommand { get; }

    private bool _isComponentBindPopupOpen;
    public bool IsComponentBindPopupOpen
    {
        get => _isComponentBindPopupOpen;
        set { if (_isComponentBindPopupOpen != value) { _isComponentBindPopupOpen = value; OnPropertyChanged(); } }
    }

    private ObservableCollection<ComponentInstance> _availableComponentsForBinding = new();
    public ObservableCollection<ComponentInstance> AvailableComponentsForBinding
    {
        get => _availableComponentsForBinding;
        set { _availableComponentsForBinding = value; OnPropertyChanged(); }
    }

    private ComponentInstance? _selectedComponentForBinding;
    public ComponentInstance? SelectedComponentForBinding
    {
        get => _selectedComponentForBinding;
        set { if (_selectedComponentForBinding != value) { _selectedComponentForBinding = value; OnPropertyChanged(); } }
    }

    private TrackNode? _bindingTargetNode;
    
    public ObservableCollection<EditorImage> EditorImages { get; } = new();

    private EditorImage? _selectedImage;
    public EditorImage? SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (_selectedImage != value)
            {
                _selectedImage = value;
                if (_selectedImage != null && SelectedLayerItem != _selectedImage)
                {
                    SelectedLayerItem = _selectedImage;
                }
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedImage));
                if (RemoveImageCommand is RelayCommandWithCanExecute<object> cmd)
                    cmd.RaiseCanExecuteChanged();
                if (RemoveSelectedLayerCommand is RelayCommandWithCanExecute<object> rcmd)
                    rcmd.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasSelectedImage => SelectedImage != null;

    public string StatusText => "Editor Ready";
    
    public ICommand LoadImageCommand { get; }
    public ICommand RemoveImageCommand { get; }

    #region Simulation
    private bool _isSimulationRunning;
    public bool IsSimulationRunning
    {
        get => _isSimulationRunning;
        set
        {
            if (_isSimulationRunning != value)
            {
                _isSimulationRunning = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand StartSimulationCommand { get; }
    public ICommand StopSimulationCommand { get; }

    private System.Collections.Generic.List<HardwareSimulationEngine> _activeEngines = new();
    private System.Collections.Generic.List<ArduinoGlobals> _activeGlobals = new();

    private void StartSimulation()
    {
        if (IsSimulationRunning) return;
        IsSimulationRunning = true;
        _activeEngines.Clear();
        _activeGlobals.Clear();

        var stations = PlacedBoxes.OfType<StationInstance>().ToList();
        var components = PlacedBoxes.OfType<ComponentInstance>().ToList();

        // Basit bir Ağ kuyruğu (Şimdilik tüm cihazlar aynı kuyruğu paylaşıyor)
        var networkBus = new System.Collections.Concurrent.ConcurrentQueue<string>();

        foreach (var station in stations)
        {
            var code = ProjectManager.Instance.GetCustomCode(station.Id);
            if (string.IsNullOrWhiteSpace(code)) continue;

            var engine = new HardwareSimulationEngine();
            string lastTag = null; // Debounce state for RFID
            
            var globals = new ArduinoGlobals
            {
                OnLog = (msg) => 
                {
                    station.Log(msg);
                },
                OnSend = (msg) => 
                {
                    networkBus.Enqueue(msg);
                },
                OnReceive = () => 
                {
                    return networkBus.TryDequeue(out var m) ? m : "";
                },
                OnDigitalWrite = (pin, val) =>
                {
                    // Find connected component
                    var comp = components.FirstOrDefault(c => c.StationId == station.Id && c.ConnectedPin == pin);
                    if (comp != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            comp.IsPowered = (val == 1); // 1 is HIGH
                            station.Log($"OUT: {comp.Name} ({pin}) -> {(val == 1 ? "HIGH" : "LOW")}");
                        });
                    }
                },
                OnRfidAvailable = (pin) =>
                {
                    var reader = components.FirstOrDefault(c => c.StationId == station.Id && c.ConnectedPin == pin);
                    if (reader != null)
                    {
                        var tag = PlacedBoxes.OfType<RfidTagInstance>().FirstOrDefault(t => 
                            Math.Abs(t.X - reader.X) < 50 && Math.Abs(t.Y - reader.Y) < 50);
                        if (tag == null) { lastTag = null; return false; }
                        return lastTag != tag.Uid;
                    }
                    return false;
                },
                OnReadRfid = (pin) =>
                {
                    var reader = components.FirstOrDefault(c => c.StationId == station.Id && c.ConnectedPin == pin);
                    if (reader != null)
                    {
                        var tag = PlacedBoxes.OfType<RfidTagInstance>().FirstOrDefault(t => 
                            Math.Abs(t.X - reader.X) < 50 && Math.Abs(t.Y - reader.Y) < 50);
                        if (tag == null) 
                        {
                            lastTag = null;
                            return "";
                        }
                        if (lastTag != tag.Uid)
                        {
                            lastTag = tag.Uid;
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                station.Log($"IN: RFID ({pin}) <- {tag.Uid}");
                            });
                            return tag.Uid;
                        }
                    }
                    return "";
                }
            };
            
            _activeGlobals.Add(globals);
            _activeEngines.Add(engine);
            engine.StartSimulation(code, globals);
            TerminalService.Instance.LogSuccess($"Simülasyon başlatıldı: {station.Name}");
        }

        // Also start Main PC script if it exists
        var mainPc = PlacedBoxes.OfType<MainPcInstance>().FirstOrDefault();
        if (mainPc != null)
        {
            var code = ProjectManager.Instance.GetCustomCode(mainPc.Id);
            if (!string.IsNullOrWhiteSpace(code))
            {
                var engine = new HardwareSimulationEngine();
                var globals = new ArduinoGlobals
                {
                    OnLog = (msg) => 
                    {
                        mainPc.Log(msg);
                    },
                    OnSend = (msg) => 
                    {
                        networkBus.Enqueue(msg);
                    },
                    OnReceive = () => 
                    {
                        return networkBus.TryDequeue(out var m) ? m : "";
                    }
                };
                
                _activeGlobals.Add(globals);
                _activeEngines.Add(engine);
                engine.StartSimulation(code, globals);
                TerminalService.Instance.LogSuccess($"Simülasyon başlatıldı: {mainPc.Name}");
            }
        }
    }

    public void OnRfidScanned(string uid)
    {
        foreach (var globals in _activeGlobals)
        {
            globals.InjectRfidScan(uid);
        }
        TerminalService.Instance.LogSuccess($"RFID Okundu: {uid}");
    }

    private void StopSimulation()
    {
        if (!IsSimulationRunning) return;
        
        foreach (var engine in _activeEngines)
        {
            engine.Dispose();
        }
        _activeEngines.Clear();
        _activeGlobals.Clear();
        _activeEngines.Clear();
        
        // Reset all components to unpowered
        var components = PlacedBoxes.OfType<ComponentInstance>().ToList();
        foreach (var comp in components)
        {
            comp.IsPowered = false;
        }

        IsSimulationRunning = false;
        TerminalService.Instance.LogSystem("Simülasyon durduruldu.");
    }
    #endregion

    public ICommand SelectImageCommand { get; }
    public ICommand SaveLayoutCommand { get; }
    public ICommand LoadLayoutCommand { get; }
    
    // Palette items
    public ObservableCollection<ILayerItem> PaletteItems { get; } = new();
    
    private ILayerItem? _selectedPaletteItem;
    public ILayerItem? SelectedPaletteItem
    {
        get => _selectedPaletteItem;
        set
        {
            if (_selectedPaletteItem != value)
            {
                _selectedPaletteItem = value;
                OnPropertyChanged();
                if (AddBoxCommand is RelayCommandWithCanExecute<object> cmd)
                    cmd.RaiseCanExecuteChanged();
            }
        }
    }
    
    // Items placed on canvas (Modems + Stations + Components)
    public ObservableCollection<ILayerItem> PlacedBoxes { get; }
    
    // Daisy-chain connection lines
    public ObservableCollection<ConnectionLine> DaisyChainLines { get; } = new();
    
    public ICommand AddBoxCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand OpenRelayControlCommand { get; }
    public ICommand OpenOtaUpdateCommand { get; }
    public ICommand RemoveRouteCommand { get; }
    public ICommand ToggleOrthogonalModeCommand { get; }
    public ICommand UndoRouteNodeCommand { get; }
    
    // Spline Drawing State
    public ObservableCollection<TrackRoute> Routes { get; } = new();
    
    private TrackRoute? _currentRoute;
    public TrackRoute? CurrentRoute
    {
        get => _currentRoute;
        set
        {
            if (_currentRoute != value)
            {
                _currentRoute = value;
                OnPropertyChanged();
            }
        }
    }
    
    private bool _isDrawingRoute;
    public bool IsDrawingRoute
    {
        get => _isDrawingRoute;
        set
        {
            if (_isDrawingRoute != value)
            {
                _isDrawingRoute = value;
                OnPropertyChanged();
                
                if (_isDrawingRoute)
                {
                    if (IsSelectionMode) IsSelectionMode = false;
                    CurrentRoute = new TrackRoute { Name = $"Rota {Routes.Count + 1}" };
                    Routes.Add(CurrentRoute);
                    SyncLayers();
                    TerminalService.Instance.LogInfo("Rota çizim modu aktif. Haritaya tıklayarak noktalar ekleyin.");
                }
                else
                {
                    if (CurrentRoute != null && CurrentRoute.Nodes.Count < 2)
                    {
                        Routes.Remove(CurrentRoute);
                        SyncLayers();
                        TerminalService.Instance.LogWarning("Rota en az 2 nokta içermelidir. İptal edildi.");
                    }
                    else
                    {
                        TerminalService.Instance.LogSuccess("Rota çizimi tamamlandı.");
                    }
                    CurrentRoute = null;
                }
            }
        }
    }
    
    private bool _isSelectionMode;
    public bool IsSelectionMode
    {
        get => _isSelectionMode;
        set
        {
            if (_isSelectionMode != value)
            {
                _isSelectionMode = value;
                OnPropertyChanged();
                
                if (_isSelectionMode && IsDrawingRoute)
                {
                    IsDrawingRoute = false; // Turn off drawing mode if selection mode is activated
                }
            }
        }
    }
    
    private bool _isOrthogonalMode;
    public bool IsOrthogonalMode
    {
        get => _isOrthogonalMode;
        set
        {
            if (_isOrthogonalMode != value)
            {
                _isOrthogonalMode = value;
                OnPropertyChanged();
            }
        }
    }
    
    public void AddRouteNode(double x, double y)
    {
        if (!IsDrawingRoute || CurrentRoute == null) return;
        
        var node = new TrackNode { X = x, Y = y };
        CurrentRoute.Nodes.Add(node);
        OnPropertyChanged(nameof(CurrentRoute)); // Trigger path redraw
        ((RelayCommandWithCanExecute<object>)UndoRouteNodeCommand).RaiseCanExecuteChanged();
    }
    
    private void UndoRouteNode()
    {
        if (CurrentRoute != null && CurrentRoute.Nodes.Any())
        {
            CurrentRoute.Nodes.RemoveAt(CurrentRoute.Nodes.Count - 1);
            OnPropertyChanged(nameof(CurrentRoute));
            ((RelayCommandWithCanExecute<object>)UndoRouteNodeCommand).RaiseCanExecuteChanged();
        }
    }
    
    private bool CanUndoRouteNode()
    {
        return IsDrawingRoute && CurrentRoute != null && CurrentRoute.Nodes.Any();
    }
    
    private bool _isLiveMode;
    public bool IsLiveMode
    {
        get => _isLiveMode;
        set
        {
            if (_isLiveMode != value)
            {
                _isLiveMode = value;
                OnPropertyChanged();
                
                if (_isLiveMode)
                {
                    LiveNetworkEngine.Instance.NetworkStatusUpdated += OnNetworkStatusUpdated;
                    LiveNetworkEngine.Instance.Start();
                    TerminalService.Instance.LogSuccess("Canlı Mod Aktif. Ağ pingleniyor...");
                }
                else
                {
                    LiveNetworkEngine.Instance.NetworkStatusUpdated -= OnNetworkStatusUpdated;
                    LiveNetworkEngine.Instance.Stop();
                    TerminalService.Instance.LogInfo("Canlı Mod Kapatıldı. Tasarım moduna dönüldü.");
                }
            }
        }
    }

    private void OnNetworkStatusUpdated()
    {
        // Status properties in models trigger UI updates automatically.
        // We can force a re-render of connection lines if needed.
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            SyncDaisyChainLines();
        });
    }
    
    public EditorViewModel()
    {
        _networkManager = NetworkManager.Instance;
        ViewportState = new ViewportState();
        InputRouter = new EditorInputRouter(ViewportState);

        StartSimulationCommand = new RelayCommand<object>(_ => StartSimulation());
        StopSimulationCommand = new RelayCommand<object>(_ => StopSimulation());

        LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
        
        BindComponentCommand = new RelayCommand<TrackNode>(OpenBindPopup);
        SaveComponentBindingCommand = new RelayCommand<object>(_ => SaveBinding());
        CancelComponentBindingCommand = new RelayCommand<object>(_ => IsComponentBindPopupOpen = false);
        
        PlacedBoxes = new ObservableCollection<ILayerItem>();
        

        
        // Populate initially placed boxes
        if (_networkManager.MainPc.X > 0 || _networkManager.MainPc.Y > 0 || _networkManager.MainPc.IsLocked)
        {
            PlacedBoxes.Add(_networkManager.MainPc);
        }

        foreach(var modem in _networkManager.Modems)
        {
            if (modem.X > 0 || modem.Y > 0 || modem.IsLocked)
                PlacedBoxes.Add(modem);

            foreach(var st in modem.Stations)
            {
                if (st.X > 0 || st.Y > 0 || st.IsLocked)
                    PlacedBoxes.Add(st);
                
                foreach(var comp in st.Components)
                {
                    if (comp.X > 0 || comp.Y > 0 || comp.IsLocked)
                        PlacedBoxes.Add(comp);
                }
            }
        }
        
        LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
        RemoveImageCommand = new RelayCommandWithCanExecute<object>(_ => RemoveImage(), () => SelectedImage != null);
        SelectImageCommand = new RelayCommand<object>(img => SelectedImage = img as EditorImage);
        
        // Save/Load layout file commands are removed since ProjectManager handles DB
        SaveLayoutCommand = new RelayCommand<object>(_ => { ProjectManager.Instance.SaveScenario(); });
        LoadLayoutCommand = new RelayCommand<object>(_ => { /* Deprecated */ });
        
        ProjectManager.Instance.GetEditorJsonFunc = () => EditorLayoutService.Instance.GetCurrentLayoutJson(EditorImages, PlacedBoxes, Routes);
        ProjectManager.Instance.LoadEditorJsonAction = json => LoadEditorFromJson(json);
        
        AddBoxCommand = new RelayCommandWithCanExecute<object>(_ => AddBox(), () => SelectedPaletteItem != null);
        RemoveSelectedLayerCommand = new RelayCommand<object>(param => 
        {
            if (!IsLiveMode) RemoveSelectedLayer(param as ILayerItem);
        });
        RemoveRouteCommand = new RelayCommand<object>(param => 
        {
            if (!IsLiveMode && param is TrackRoute route)
            {
                Routes.Remove(route);
                SyncLayers();
                TerminalService.Instance.LogInfo($"Rota silindi: {route.Name}");
            }
        });
        EditLayerPropertiesCommand = new RelayCommand<object>(param => EditLayerProperties(param as ILayerItem));
        RefreshCommand = new RelayCommand<object>(_ => RefreshEditor());
        
        ToggleOrthogonalModeCommand = new RelayCommand<object>(_ => 
        {
            IsOrthogonalMode = !IsOrthogonalMode;
            TerminalService.Instance.LogInfo(IsOrthogonalMode ? "Dik Açı (Orthogonal) Modu: AÇIK" : "Dik Açı (Orthogonal) Modu: KAPALI");
        });
        UndoRouteNodeCommand = new RelayCommandWithCanExecute<object>(_ => UndoRouteNode(), () => CanUndoRouteNode());

        OpenRelayControlCommand = new RelayCommand<object>(param => 
        {
            if (IsLiveMode && param is TCP.App.Models.Electronics.StationInstance station)
            {
                TerminalService.Instance.LogInfo($"Röle Kontrol paneli açılıyor: {station.Name} ({station.IpAddress})");
                var window = new TCP.App.Views.Dialogs.RelayControlWindow(station);
                window.ShowDialog();
            }
        });

        OpenOtaUpdateCommand = new RelayCommand<object>(param => 
        {
            if (IsLiveMode && param is TCP.App.Models.Electronics.StationInstance station)
            {
                TerminalService.Instance.LogInfo($"OTA Güncelleme arayüzü açılıyor: {station.Name} ({station.IpAddress})");
                var window = new TCP.App.Views.Dialogs.OtaUpdateWindow(station);
                window.ShowDialog();
            }
        });

        EditorImages.CollectionChanged += (s, e) => { SyncLayers(); ProjectManager.Instance.MarkDirty(); };
        PlacedBoxes.CollectionChanged += (s, e) => { SyncLayers(); ProjectManager.Instance.MarkDirty(); };
        Routes.CollectionChanged += (s, e) => { SyncLayers(); ProjectManager.Instance.MarkDirty(); };
        
        RefreshPaletteItems();
        _networkManager.NetworkChanged += OnNetworkChanged;
        ProjectManager.Instance.ScenarioLoaded += StopSimulation;
        
        SyncLayers();

        if (ProjectManager.Instance.CurrentScenario != null && !string.IsNullOrEmpty(ProjectManager.Instance.CurrentScenario.EditorLayoutJson))
        {
            LoadEditorFromJson(ProjectManager.Instance.CurrentScenario.EditorLayoutJson);
        }
    }

    private void RefreshLayout()
    {
        ProjectManager.Instance.MarkDirty();
    }
    
    private void OpenBindPopup(TrackNode? node)
    {
        if (node == null) return;
        _bindingTargetNode = node;
        
        AvailableComponentsForBinding.Clear();
        foreach (var modem in _networkManager.Modems)
        {
            foreach (var station in modem.Stations)
            {
                foreach (var component in station.Components)
                {
                    // Only Servos are typically used for track switches, but we can list all for now
                    if (component.Type.ToLower().Contains("servo"))
                    {
                        AvailableComponentsForBinding.Add(component);
                    }
                }
            }
        }
        
        SelectedComponentForBinding = AvailableComponentsForBinding.FirstOrDefault(c => c.Id == node.BoundComponentId);
        IsComponentBindPopupOpen = true;
    }

    private void SaveBinding()
    {
        if (_bindingTargetNode != null && SelectedComponentForBinding != null)
        {
            _bindingTargetNode.BoundComponentId = SelectedComponentForBinding.Id;
            RefreshLayout();
        }
        else if (_bindingTargetNode != null && SelectedComponentForBinding == null)
        {
            _bindingTargetNode.BoundComponentId = null;
            RefreshLayout();
        }
        IsComponentBindPopupOpen = false;
    }

    private void RefreshEditor()
    {
        OnNetworkChanged();
        TerminalService.Instance.LogSuccess("Editör güncellendi.");
    }

    private void OnNetworkChanged()
    {
        // 1. Refresh Palette
        RefreshPaletteItems();

        // 2. Remove Orphans
        var activeIds = new System.Collections.Generic.HashSet<Guid>();
        activeIds.Add(_networkManager.MainPc.Id);
        foreach (var m in _networkManager.Modems)
        {
            activeIds.Add(m.Id);
            foreach (var st in m.Stations)
            {
                activeIds.Add(st.Id);
                foreach (var c in st.Components)
                {
                    activeIds.Add(c.Id);
                }
            }
        }
        foreach (var rfid in _networkManager.RfidTags)
        {
            activeIds.Add(rfid.Id);
        }

        var toRemove = PlacedBoxes.Where(b => !activeIds.Contains(b.Id)).ToList();
        foreach (var item in toRemove)
        {
            PlacedBoxes.Remove(item);
        }

        // 3. Auto-add new stations/components if their parent modem/station is on map
        foreach (var m in _networkManager.Modems)
        {
            if (PlacedBoxes.Contains(m))
            {
                double stOffsetX = 160;
                foreach (var st in m.Stations)
                {
                    if (!PlacedBoxes.Contains(st))
                    {
                        st.X = m.X + stOffsetX;
                        st.Y = m.Y + 160;
                        PlacedBoxes.Add(st);
                    }
                    stOffsetX += 160;

                    double cmpOffsetX = 160;
                    foreach (var c in st.Components)
                    {
                        if (!PlacedBoxes.Contains(c))
                        {
                            c.X = st.X + cmpOffsetX;
                            c.Y = st.Y + 160;
                            PlacedBoxes.Add(c);
                        }
                        cmpOffsetX += 160;
                    }
                }
            }
        }

        SyncLayers();
    }

    private void RefreshPaletteItems()
    {
        PaletteItems.Clear();
        PaletteItems.Add(_networkManager.MainPc);
        foreach(var m in _networkManager.Modems) PaletteItems.Add(m);
        foreach(var rfid in _networkManager.RfidTags) PaletteItems.Add(rfid);
    }

    private void SyncLayers()
    {
        var selectedId = SelectedLayerItem?.Id;
        Layers.Clear();
        
        // 1. Group Images
        if (EditorImages.Any())
        {
            var imagesGroup = new LayerGroup("Arka Planlar");
            foreach (var img in EditorImages)
                imagesGroup.Children.Add(img);
            Layers.Add(imagesGroup);
        }

        // 1.5. Group Routes
        if (Routes.Any())
        {
            var routesGroup = new LayerGroup("Rotalar");
            foreach(var route in Routes)
                routesGroup.Children.Add(route);
            Layers.Add(routesGroup);
        }

        // 2. Group by Modem (Ağ)
        var grouped = PlacedBoxes
            .Where(b => b is ModemInstance || b is StationInstance || b is ComponentInstance || b is MainPcInstance || b is RfidTagInstance)
            .GroupBy(b => 
            {
                if (b is MainPcInstance p) return $"Ağ: {p.Name}";
                if (b is ModemInstance m) return $"Ağ: {m.Name}";
                if (b is StationInstance s) 
                {
                    var parent = _networkManager.Modems.FirstOrDefault(m => m.Stations.Contains(s));
                    return parent != null ? $"Ağ: {parent.Name}" : "Ağ: Bilinmeyen";
                }
                if (b is ComponentInstance c) 
                {
                    var parentStation = _networkManager.Modems.SelectMany(m => m.Stations).FirstOrDefault(st => st.Id == c.StationId);
                    var parentModem = _networkManager.Modems.FirstOrDefault(m => parentStation != null && m.Stations.Contains(parentStation));
                    return parentModem != null ? $"Ağ: {parentModem.Name}" : "Ağ: Bilinmeyen";
                }
                if (b is RfidTagInstance) return "RFID Etiketler";
                return "Diğer";
            })
            .OrderBy(g => g.Key);
        
        foreach (var group in grouped)
        {
            var layerGroup = new LayerGroup(group.Key);
            foreach (var box in group)
                layerGroup.Children.Add(box);
            Layers.Add(layerGroup);
        }

        SyncDaisyChainLines();

        if (selectedId.HasValue)
        {
            SelectedLayerItem = FindLayerById(selectedId.Value);
        }
    }

    private void SyncDaisyChainLines()
    {
        DaisyChainLines.Clear();
        foreach (var modem in _networkManager.Modems)
        {
            if (modem.IncomingConnectionId.HasValue)
            {
                ILayerItem? source = null;
                if (modem.IncomingConnectionId.Value == _networkManager.MainPc.Id)
                {
                    source = _networkManager.MainPc;
                }
                else
                {
                    source = _networkManager.Modems.FirstOrDefault(m => m.Id == modem.IncomingConnectionId.Value);
                }

                if (source != null)
                {
                    DaisyChainLines.Add(new ConnectionLine { Source = source, Target = modem, LineType = "Modem" });
                }
            }

            foreach(var st in modem.Stations)
            {
                // Draw line between modem and station unconditionally, like PaletteItems did
                DaisyChainLines.Add(new ConnectionLine { Source = modem, Target = st, LineType = "Station" });

                foreach(var c in st.Components)
                {
                    // Draw line between station and component unconditionally
                    DaisyChainLines.Add(new ConnectionLine { Source = st, Target = c, LineType = "Component" });
                }
            }
        }
    }

    private ILayerItem? FindLayerById(Guid id)
    {
        foreach(var layer in Layers)
        {
            if (layer.Id == id) return layer;
            if (layer is LayerGroup grp)
            {
                var child = grp.Children.FirstOrDefault(c => c.Id == id);
                if (child != null) return child;
            }
        }
        return null;
    }
    
    private void RemoveSelectedLayer(ILayerItem? item = null)
    {
        var selectedImages = EditorImages.Where(i => i.IsSelected).ToList();
        var selectedBoxes = PlacedBoxes.Where(b => b.IsSelected).ToList();
        var selectedRoutes = Routes.Where(r => r.IsSelected).ToList();
        
        // If item is null OR item is currently selected (and we have multiple selected), delete all selected
        bool isItemInSelection = item != null && item.IsSelected;
        
        if (item == null || isItemInSelection)
        {
            foreach (var img in selectedImages) RemoveLayerCore(img);
            foreach (var box in selectedBoxes) RemoveLayerCore(box);
            foreach (var route in selectedRoutes) RemoveLayerCore(route);
            
            if (selectedImages.Any() || selectedBoxes.Any() || selectedRoutes.Any())
                return;
        }

        // If we reach here, we are just deleting the specific passed item
        var target = item ?? SelectedLayerItem;
        if (target != null) RemoveLayerCore(target);
        
        SyncLayers();
    }

    private void RemoveLayerCore(ILayerItem target)
    {
        if (target is EditorImage img)
        {
            EditorImages.Remove(img);
            if (SelectedLayerItem == img) SelectedLayerItem = null;
        }
        else if (target is TrackRoute route)
        {
            Routes.Remove(route);
            if (SelectedLayerItem == route) SelectedLayerItem = null;
        }
        else if (target is MainPcInstance pc)
        {
            pc.X = 0; pc.Y = 0; pc.IsLocked = false;
            PlacedBoxes.Remove(pc);
            if (SelectedLayerItem == pc) SelectedLayerItem = null;
            NetworkManager.Instance.SaveNetwork();
        }
        else if (target is ModemInstance m)
        {
            m.X = 0; m.Y = 0; m.IsLocked = false;
            PlacedBoxes.Remove(m);
            // Also remove its stations and components from map
            foreach(var st in m.Stations)
            {
                st.X = 0; st.Y = 0; st.IsLocked = false;
                PlacedBoxes.Remove(st);
                foreach(var c in st.Components)
                {
                    c.X = 0; c.Y = 0; c.IsLocked = false;
                    PlacedBoxes.Remove(c);
                }
            }
            if (SelectedLayerItem == m) SelectedLayerItem = null;
            NetworkManager.Instance.SaveNetwork();
        }
        else if (target is StationInstance st)
        {
            st.X = 0; st.Y = 0; st.IsLocked = false;
            PlacedBoxes.Remove(st);
            // Remove its components
            foreach(var c in st.Components)
            {
                c.X = 0; c.Y = 0; c.IsLocked = false;
                PlacedBoxes.Remove(c);
            }
            if (SelectedLayerItem == st) SelectedLayerItem = null;
            NetworkManager.Instance.SaveModems();
        }
        else if (target is ComponentInstance comp)
        {
            comp.X = 0; comp.Y = 0; comp.IsLocked = false;
            PlacedBoxes.Remove(comp);
            if (SelectedLayerItem == comp) SelectedLayerItem = null;
            NetworkManager.Instance.SaveModems();
        }
        else if (target is RfidTagInstance rfid)
        {
            rfid.X = 0; rfid.Y = 0; rfid.IsLocked = false;
            PlacedBoxes.Remove(rfid);
            if (SelectedLayerItem == rfid) SelectedLayerItem = null;
            NetworkManager.Instance.SaveNetwork();
        }
        else if (target is LayerGroup grp)
        {
            foreach(var child in grp.Children.ToList())
            {
                if (child is EditorImage i) EditorImages.Remove(i);
                if (child is MainPcInstance mainPc)
                {
                    mainPc.X = 0; mainPc.Y = 0; mainPc.IsLocked = false;
                    PlacedBoxes.Remove(mainPc);
                }
                if (child is ModemInstance md)
                {
                    md.X = 0; md.Y = 0; md.IsLocked = false;
                    PlacedBoxes.Remove(md);
                }
                if (child is StationInstance s)
                {
                    s.X = 0; s.Y = 0; s.IsLocked = false;
                    PlacedBoxes.Remove(s);
                }
                if (child is ComponentInstance c)
                {
                    c.X = 0; c.Y = 0; c.IsLocked = false;
                    PlacedBoxes.Remove(c);
                }
                if (child is RfidTagInstance r)
                {
                    r.X = 0; r.Y = 0; r.IsLocked = false;
                    PlacedBoxes.Remove(r);
                }
            }
            if (SelectedLayerItem == grp) SelectedLayerItem = null;
            NetworkManager.Instance.SaveModems();
        }
    }

    private void EditLayerProperties(ILayerItem? item = null)
    {
        var target = item ?? SelectedLayerItem;
        if (target == null || target is LayerGroup) return;

        var window = new TCP.App.Views.LayerPropertiesWindow(target);
        if (window.ShowDialog() == true)
        {
            SyncLayers();
            NetworkManager.Instance.SaveModems();
        }
    }
    
    private void LoadImage()
    {
        try
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Title = "Load Background Image"
            };
            
            var owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog(owner) == true)
            {
                var filePath = dialog.FileName;
                var fileName = System.IO.Path.GetFileName(filePath);
                
                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                using var stream = new System.IO.MemoryStream(imageBytes);
                
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze(); 
                
                var newImage = new EditorImage
                {
                    FilePath = filePath,
                    Name = fileName,
                    ImageSource = bitmap,
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelHeight,
                    X = 0, Y = 0, Opacity = 1.0, IsLocked = false
                };
                
                EditorImages.Add(newImage);
                SelectedImage = newImage;
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load image: {ex.Message}");
        }
    }

    private void RemoveImage()
    {
        if (SelectedImage != null)
        {
            EditorImages.Remove(SelectedImage);
            SelectedImage = null;
        }
    }

    private void LoadEditorFromJson(string json)
    {
        var state = EditorLayoutService.Instance.LoadFromJson(json);
        if (state != null)
        {
            EditorImages.Clear();
            foreach (var img in state.Images) EditorImages.Add(img);

            Routes.Clear();
            foreach (var route in state.Routes) Routes.Add(route);

            PlacedBoxes.Clear();
            foreach (var stState in state.PlacedItems)
            {
                // Look for MainPc
                if (_networkManager.MainPc.Id == stState.ItemId)
                {
                    _networkManager.MainPc.X = stState.X; _networkManager.MainPc.Y = stState.Y; _networkManager.MainPc.IsLocked = stState.IsLocked;
                    PlacedBoxes.Add(_networkManager.MainPc);
                    continue;
                }

                // Look for Modems
                var modem = _networkManager.Modems.FirstOrDefault(m => m.Id == stState.ItemId);
                if (modem != null)
                {
                    modem.X = stState.X; modem.Y = stState.Y; modem.IsLocked = stState.IsLocked;
                    PlacedBoxes.Add(modem);
                    continue;
                }
                
                // Look for Stations
                var st = _networkManager.Modems.SelectMany(m => m.Stations).FirstOrDefault(s => s.Id == stState.ItemId);
                if (st != null)
                {
                    st.X = stState.X; st.Y = stState.Y; st.IsLocked = stState.IsLocked;
                    PlacedBoxes.Add(st);
                    continue;
                }
                
                // Look for Components
                foreach(var m in _networkManager.Modems)
                {
                    foreach(var station in m.Stations)
                    {
                        var comp = station.Components.FirstOrDefault(c => c.Id == stState.ItemId);
                        if (comp != null)
                        {
                            comp.X = stState.X; comp.Y = stState.Y; comp.IsLocked = stState.IsLocked;
                            PlacedBoxes.Add(comp);
                            break;
                        }
                    }
                }
                
                // Look for RFID Tags
                var rfid = _networkManager.RfidTags.FirstOrDefault(r => r.Id == stState.ItemId);
                if (rfid != null)
                {
                    rfid.X = stState.X; rfid.Y = stState.Y; rfid.IsLocked = stState.IsLocked;
                    PlacedBoxes.Add(rfid);
                    continue;
                }
            }
            
            SelectedImage = null;
            SelectedLayerItem = null;
        }
    }
    
    private void AddBox()
    {
        try
        {
            if (SelectedPaletteItem == null)
            {
                TerminalService.Instance.LogWarning("Lütfen paletten bir öğe seçin.");
                return;
            }

            double offsetX = 100.0;
            double offsetY = 100.0;
            int addedCount = 0;

            if (SelectedPaletteItem is MainPcInstance pc)
            {
                if (!PlacedBoxes.Contains(pc))
                {
                    pc.X = offsetX;
                    pc.Y = offsetY;
                    PlacedBoxes.Add(pc);
                    addedCount++;
                }
            }
            else if (SelectedPaletteItem is ModemInstance m)
            {
                if (!PlacedBoxes.Contains(m))
                {
                    m.X = offsetX;
                    m.Y = offsetY;
                    PlacedBoxes.Add(m);
                    addedCount++;
                    offsetX += 160;
                }

                foreach (var st in m.Stations)
                {
                    if (!PlacedBoxes.Contains(st))
                    {
                        st.X = offsetX;
                        st.Y = offsetY;
                        PlacedBoxes.Add(st);
                        addedCount++;
                        offsetX += 160;
                    }
                    
                    foreach (var comp in st.Components)
                    {
                        if (!PlacedBoxes.Contains(comp))
                        {
                            comp.X = offsetX;
                            comp.Y = offsetY;
                            PlacedBoxes.Add(comp);
                            addedCount++;
                            offsetX += 20;
                            offsetY += 20;
                        }
                    }
                }
            }
            else if (SelectedPaletteItem is RfidTagInstance rfid)
            {
                if (!PlacedBoxes.Contains(rfid))
                {
                    rfid.X = offsetX;
                    rfid.Y = offsetY;
                    PlacedBoxes.Add(rfid);
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                NetworkManager.Instance.SaveNetwork();
                string itemName = SelectedPaletteItem is MainPcInstance mainPcInst ? mainPcInst.Name : 
                                 (SelectedPaletteItem is ModemInstance mdInst ? mdInst.Name : 
                                 (SelectedPaletteItem is RfidTagInstance rfidInst ? rfidInst.Name : "Bilinmeyen"));
                TerminalService.Instance.LogSuccess($"{addedCount} öğe haritaya eklendi ({itemName})");
            }
            else
            {
                TerminalService.Instance.LogWarning("Bu ağdaki tüm öğeler zaten haritada bulunuyor.");
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Modem eklenemedi: {ex.Message}");
        }
    }
}
