using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TCP.App.Models.Editor;
using TCP.App.Models.Electronics;
using TCP.App.Services;
using System.Linq;namespace TCP.App.ViewModels;

/// <summary>
/// EditorImageMode - Image display mode enum
/// TCP-1.0.2: Background Image Load (Editor)
/// </summary>
public enum EditorImageMode
{
    /// <summary>
    /// Fit mode: Image Stretch="Uniform", centered, no scrollbars
    /// </summary>
    Fit,
    
    /// <summary>
    /// Actual mode: ScrollViewer with Image Stretch="None", scrollbars Auto
    /// </summary>
    Actual
}

/// <summary>
/// EditorViewModel - Editor modülü ViewModel'i
/// 
/// TCP-1.0.2: Background Image Load (Editor)
/// TCP-1.0.3: Editor: Add board boxes from registry
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
    /// Device manager - Single source of truth for user devices
    /// </summary>
    private readonly DeviceManager _deviceManager;
    
    /// <summary>
    /// Viewport state - Zoom and pan transformation
    /// TCP-1.0.4: Background Image Load with Zoom/Pan
    /// </summary>
    public ViewportState ViewportState { get; }
    
    /// <summary>
    /// Input router - Mouse input handler for viewport
    /// TCP-1.0.4: Background Image Load with Zoom/Pan
    /// </summary>
    public EditorInputRouter InputRouter { get; }
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
    /// Unified collection of layers (Images and Electronics)
    /// </summary>
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
                SelectedBox = _selectedLayerItem as DeviceInstance;
                
                OnPropertyChanged();
                if (RemoveSelectedLayerCommand is RelayCommandWithCanExecute<object> rcmd)
                {
                    rcmd.RaiseCanExecuteChanged();
                }
                if (EditLayerPropertiesCommand is RelayCommandWithCanExecute<object> ecmd)
                {
                    ecmd.RaiseCanExecuteChanged();
                }
            }
        }
    }
    
    public ICommand RemoveSelectedLayerCommand { get; }
    public ICommand EditLayerPropertiesCommand { get; }
    
    /// <summary>
    /// Collection of background images on the editor
    /// </summary>
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
                {
                    cmd.RaiseCanExecuteChanged();
                }
                if (RemoveSelectedLayerCommand is RelayCommandWithCanExecute<object> rcmd)
                {
                    rcmd.RaiseCanExecuteChanged();
                }
            }
        }
    }

    public bool HasSelectedImage => SelectedImage != null;

    /// <summary>
    /// Status text (for display in toolbar)
    /// </summary>
    public string StatusText => "Editor Ready";
    
    public ICommand LoadImageCommand { get; }
    public ICommand RemoveImageCommand { get; }
    public ICommand SelectImageCommand { get; }
    public ICommand SaveLayoutCommand { get; }
    public ICommand LoadLayoutCommand { get; }
    
    /// <summary>
    /// Palette boards (from device manager)
    /// </summary>
    public ObservableCollection<DeviceInstance> PaletteBoards => _deviceManager.Devices;
    
    /// <summary>
    /// Selected palette board (for Add button)
    /// </summary>
    private DeviceInstance? _selectedPaletteBoard;
    public DeviceInstance? SelectedPaletteBoard
    {
        get => _selectedPaletteBoard;
        set
        {
            if (_selectedPaletteBoard != value)
            {
                _selectedPaletteBoard = value;
                OnPropertyChanged();
                // TCP-1.0.3: Update AddBoxCommand CanExecute
                if (AddBoxCommand is RelayCommandWithCanExecute<object> cmd)
                {
                    cmd.RaiseCanExecuteChanged();
                }
            }
        }
    }
    
    /// <summary>
    /// Placed devices on editor canvas
    /// </summary>
    public ObservableCollection<DeviceInstance> PlacedBoxes { get; }
    
    /// <summary>
    /// Selected device (optional for highlight)
    /// </summary>
    private DeviceInstance? _selectedBox;
    public DeviceInstance? SelectedBox
    {
        get => _selectedBox;
        set
        {
            if (_selectedBox != value)
            {
                _selectedBox = value;
                if (_selectedBox != null && SelectedLayerItem != _selectedBox)
                {
                    SelectedLayerItem = _selectedBox;
                }
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Add Box Command
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public ICommand AddBoxCommand { get; }
    
    /// <summary>
    /// Constructor - Initialize commands and palette
    /// TCP-1.0.2: Background Image Load (Editor)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public EditorViewModel()
    {
        // TCP-1.0.4: Initialize viewport state and input router
        ViewportState = new ViewportState();
        InputRouter = new EditorInputRouter(ViewportState);
        
        // TCP-Custom: Initialize device manager
        _deviceManager = DeviceManager.Instance;
        
        // PlacedBoxes currently maps to Devices that are on map. For now, we can just say ALL devices are placed or they maintain their state.
        // Let's assume all devices added to map are in PlacedBoxes.
        PlacedBoxes = new ObservableCollection<DeviceInstance>();
        
        // Load initially placed boxes
        foreach(var device in _deviceManager.Devices)
        {
            if (device.X > 0 || device.Y > 0 || device.IsLocked)
            {
                PlacedBoxes.Add(device);
            }
        }
        
        LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
        RemoveImageCommand = new RelayCommandWithCanExecute<object>(_ => RemoveImage(), () => SelectedImage != null);
        SelectImageCommand = new RelayCommand<object>(img => SelectedImage = img as EditorImage);
        SaveLayoutCommand = new RelayCommand<object>(_ => SaveLayout());
        LoadLayoutCommand = new RelayCommand<object>(_ => LoadLayout());
        
        // TCP-1.0.3: Initialize AddBoxCommand with CanExecute check
        AddBoxCommand = new RelayCommandWithCanExecute<object>(_ => AddBox(), () => SelectedPaletteBoard != null);
        RemoveSelectedLayerCommand = new RelayCommand<object>(param => RemoveSelectedLayer(param as ILayerItem));
        EditLayerPropertiesCommand = new RelayCommand<object>(param => EditLayerProperties(param as ILayerItem));

        // Sync layers on collection change
        EditorImages.CollectionChanged += (s, e) => SyncLayers();
        PlacedBoxes.CollectionChanged += (s, e) => SyncLayers();
        SyncLayers();
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
            {
                imagesGroup.Children.Add(img);
            }
            Layers.Add(imagesGroup);
        }

        // 2. Group Devices by Location
        var locationGroups = PlacedBoxes.GroupBy(b => string.IsNullOrWhiteSpace(b.Location) ? "Atanmamış" : b.Location)
                                        .OrderBy(g => g.Key);
        
        foreach (var group in locationGroups)
        {
            var layerGroup = new LayerGroup(group.Key);
            foreach (var box in group)
            {
                layerGroup.Children.Add(box);
            }
            Layers.Add(layerGroup);
        }

        if (selectedId.HasValue)
        {
            SelectedLayerItem = FindLayerById(selectedId.Value);
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
        else if (target is DeviceInstance dev)
        {
            dev.X = 0;
            dev.Y = 0;
            dev.IsLocked = false;
            PlacedBoxes.Remove(dev);
            if (SelectedLayerItem == dev) SelectedLayerItem = null;
            TCP.App.Services.DeviceManager.Instance.SaveDevices();
        }
        else if (target is LayerGroup grp)
        {
            foreach(var child in grp.Children.ToList())
            {
                if (child is EditorImage i) EditorImages.Remove(i);
                if (child is DeviceInstance d)
                {
                    d.X = 0;
                    d.Y = 0;
                    d.IsLocked = false;
                    PlacedBoxes.Remove(d);
                }
            }
            if (SelectedLayerItem == grp) SelectedLayerItem = null;
            TCP.App.Services.DeviceManager.Instance.SaveDevices();
        }
    }

    private void EditLayerProperties(ILayerItem? item = null)
    {
        var target = item ?? SelectedLayerItem;
        if (target == null || target is LayerGroup) return;

        var window = new TCP.App.Views.LayerPropertiesWindow(target);
        if (window.ShowDialog() == true)
        {
            // If location changed or name changed, sync layers to refresh tree
            SyncLayers();
            TCP.App.Services.DeviceManager.Instance.SaveDevices();
        }
    }
    
    private void LoadImage()
    {
        try
        {
            TerminalService.Instance.LogInfo("Opening image selection dialog...");
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
                TerminalService.Instance.LogInfo($"Loading image {fileName} (Sync MemoryStream)...");
                
                // Read fully into memory on UI thread. This avoids ALL MTA thread deadlocks 
                // and WPF internal background downloader deadlocks.
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
                    X = 0,
                    Y = 0,
                    Opacity = 1.0,
                    IsLocked = false
                };
                
                EditorImages.Add(newImage);
                SelectedImage = newImage;
                
                TerminalService.Instance.LogSuccess($"Image loaded successfully: {newImage.Name}");
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
            EditorLayoutService.Instance.SaveLayout(dialog.FileName, EditorImages, PlacedBoxes);
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
                foreach (var img in state.Images)
                {
                    EditorImages.Add(img);
                }

                PlacedBoxes.Clear();
                foreach (var devState in state.PlacedDevices)
                {
                    var globalDev = _deviceManager.Devices.FirstOrDefault(d => d.Id == devState.DeviceId);
                    if (globalDev != null)
                    {
                        globalDev.X = devState.X;
                        globalDev.Y = devState.Y;
                        globalDev.IsLocked = devState.IsLocked;
                        PlacedBoxes.Add(globalDev);
                    }
                }
                
                SelectedImage = null;
                SelectedBox = null;
            }
        }
    }
    
    /// <summary>
    /// Add Box command implementation
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// 
    /// Adds a new PlacedBoardBox centered in the visible editor area (X,Y = 0,0 for now).
    /// </summary>
    private void AddBox()
    {
        try
        {
            // TCP-1.0.3: Safety guard - require selected palette board
            if (SelectedPaletteBoard == null)
            {
                TerminalService.Instance.LogWarning("Select a board first: Please select a board from the palette before adding.");
                return;
            }
            
            if (PlacedBoxes.Contains(SelectedPaletteBoard))
            {
                TerminalService.Instance.LogWarning("Device is already on the map.");
                return;
            }
            
            // Set initial position if not already placed
            if (SelectedPaletteBoard.X == 0 && SelectedPaletteBoard.Y == 0)
            {
                SelectedPaletteBoard.X = 100.0;
                SelectedPaletteBoard.Y = 100.0;
            }
            
            // Add to collection
            PlacedBoxes.Add(SelectedPaletteBoard);
            DeviceManager.Instance.SaveDevices();
            
            TerminalService.Instance.LogSuccess($"Added to map: {SelectedPaletteBoard.CustomName}");
        }
        catch (Exception ex)
        {
            // TCP-1.0.3: Safety - show error toast (no crash)
            TerminalService.Instance.LogError($"Failed to add box: {ex.Message}");
        }
    }
}


