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
                    CurrentRoute = new TrackRoute { Name = $"Rota {Routes.Count + 1}" };
                    Routes.Add(CurrentRoute);
                    TerminalService.Instance.LogInfo("Rota çizim modu aktif. Haritaya tıklayarak noktalar ekleyin.");
                }
                else
                {
                    if (CurrentRoute != null && CurrentRoute.Nodes.Count < 2)
                    {
                        Routes.Remove(CurrentRoute);
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
    
    public void AddRouteNode(double x, double y)
    {
        if (!IsDrawingRoute || CurrentRoute == null) return;
        
        var node = new TrackNode { X = x, Y = y };
        CurrentRoute.Nodes.Add(node);
        OnPropertyChanged(nameof(CurrentRoute)); // Trigger path redraw
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
        ViewportState = new ViewportState();
        InputRouter = new EditorInputRouter(ViewportState);
        _networkManager = NetworkManager.Instance;
        
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
        SaveLayoutCommand = new RelayCommand<object>(_ => SaveLayout());
        LoadLayoutCommand = new RelayCommand<object>(_ => LoadLayout());
        
        AddBoxCommand = new RelayCommandWithCanExecute<object>(_ => AddBox(), () => SelectedPaletteItem != null);
        RemoveSelectedLayerCommand = new RelayCommand<object>(param => 
        {
            if (!IsLiveMode) RemoveSelectedLayer(param as ILayerItem);
        });
        EditLayerPropertiesCommand = new RelayCommand<object>(param => EditLayerProperties(param as ILayerItem));
        RefreshCommand = new RelayCommand<object>(_ => RefreshEditor());
        
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

        EditorImages.CollectionChanged += (s, e) => SyncLayers();
        PlacedBoxes.CollectionChanged += (s, e) => SyncLayers();
        
        RefreshPaletteItems();
        _networkManager.NetworkChanged += OnNetworkChanged;
        
        SyncLayers();
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
                        stOffsetX += 160;
                    }

                    double cmpOffsetX = 160;
                    foreach (var c in st.Components)
                    {
                        if (!PlacedBoxes.Contains(c))
                        {
                            c.X = st.X + cmpOffsetX;
                            c.Y = st.Y + 160;
                            PlacedBoxes.Add(c);
                            cmpOffsetX += 160;
                        }
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

        // 2. Group by Modem (Ağ)
        var grouped = PlacedBoxes
            .Where(b => b is ModemInstance || b is StationInstance || b is ComponentInstance || b is MainPcInstance)
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

                if (source != null && PlacedBoxes.Contains(source) && PlacedBoxes.Contains(modem))
                {
                    DaisyChainLines.Add(new ConnectionLine { Source = source, Target = modem, LineType = "Modem" });
                }
            }

            foreach(var st in modem.Stations)
            {
                if (PlacedBoxes.Contains(modem) && PlacedBoxes.Contains(st))
                {
                    DaisyChainLines.Add(new ConnectionLine { Source = modem, Target = st, LineType = "Station" });
                }

                foreach(var c in st.Components)
                {
                    if (PlacedBoxes.Contains(st) && PlacedBoxes.Contains(c))
                    {
                        DaisyChainLines.Add(new ConnectionLine { Source = st, Target = c, LineType = "Component" });
                    }
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
        var target = item ?? SelectedLayerItem;
        if (target == null) return;

        if (target is EditorImage img)
        {
            EditorImages.Remove(img);
            if (SelectedLayerItem == img) SelectedLayerItem = null;
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

    private void SaveLayout()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "TCP Layout (*.tcplayout)|*.tcplayout|JSON Files (*.json)|*.json",
            DefaultExt = "tcplayout",
            Title = "Save Editor Layout"
        };

        if (dialog.ShowDialog() == true)
        {
            EditorLayoutService.Instance.SaveLayout(dialog.FileName, EditorImages, PlacedBoxes, Routes);
        }
    }

    private void LoadLayout()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "TCP Layout (*.tcplayout)|*.tcplayout|JSON Files (*.json)|*.json",
            Title = "Load Editor Layout"
        };

        if (dialog.ShowDialog() == true)
        {
            var state = EditorLayoutService.Instance.LoadLayout(dialog.FileName);
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
                }
                
                SelectedImage = null;
                SelectedLayerItem = null;
            }
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
                            offsetX += 160;
                        }
                    }
                }
            }

            if (addedCount > 0)
            {
                NetworkManager.Instance.SaveNetwork();
                string itemName = SelectedPaletteItem is MainPcInstance mainPcInst ? mainPcInst.Name : 
                                 (SelectedPaletteItem is ModemInstance mdInst ? mdInst.Name : "Bilinmeyen");
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
