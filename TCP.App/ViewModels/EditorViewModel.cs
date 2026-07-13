using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TCP.App.Models.Editor;
using TCP.App.Models.Electronics;
using TCP.App.Services;

namespace TCP.App.ViewModels;

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
    /// Background image source
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private BitmapImage? _backgroundImage;
    public BitmapImage? BackgroundImage
    {
        get => _backgroundImage;
        private set
        {
            if (_backgroundImage != value)
            {
                _backgroundImage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasImage));
                OnPropertyChanged(nameof(HasNoImage));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }
    
    /// <summary>
    /// Background image filename (for display)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private string? _backgroundImageName;
    public string? BackgroundImageName
    {
        get => _backgroundImageName;
        private set
        {
            if (_backgroundImageName != value)
            {
                _backgroundImageName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }
    
    /// <summary>
    /// Has image loaded
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool HasImage => BackgroundImage != null;
    
    private bool _isLocked;
    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (_isLocked != value)
            {
                _isLocked = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Has no image (for visibility binding)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool HasNoImage => !HasImage;
    
    /// <summary>
    /// Image display mode (Fit or Actual)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private EditorImageMode _imageMode = EditorImageMode.Fit;
    public EditorImageMode ImageMode
    {
        get => _imageMode;
        private set
        {
            if (_imageMode != value)
            {
                _imageMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFitMode));
                OnPropertyChanged(nameof(IsActualMode));
            }
        }
    }
    
    /// <summary>
    /// Is Fit mode (for binding convenience)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool IsFitMode => ImageMode == EditorImageMode.Fit;
    
    /// <summary>
    /// Is Actual mode (for binding convenience)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool IsActualMode => ImageMode == EditorImageMode.Actual;
    
    /// <summary>
    /// Status text (for display in toolbar)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public string StatusText
    {
        get
        {
            if (!HasImage)
            {
                return "No image loaded";
            }
            return $"Image loaded: {BackgroundImageName ?? "Unknown"}";
        }
    }
    
    /// <summary>
    /// Load Image Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand LoadImageCommand { get; }
    
    /// <summary>
    /// Set Fit Mode Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand SetFitModeCommand { get; }
    
    /// <summary>
    /// Set Actual Mode Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand SetActualModeCommand { get; }
    
    /// <summary>
    /// Clear Image Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand ClearImageCommand { get; }
    
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
        
        // TCP-1.0.2: Initialize image commands
        LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
        SetFitModeCommand = new RelayCommand<object>(_ => SetFitMode());
        SetActualModeCommand = new RelayCommand<object>(_ => SetActualMode());
        ClearImageCommand = new RelayCommand<object>(_ => ClearImage());
        
        // TCP-1.0.3: Initialize AddBoxCommand with CanExecute check
        AddBoxCommand = new RelayCommandWithCanExecute<object>(_ => AddBox(), () => SelectedPaletteBoard != null);
    }
    
    /// <summary>
    /// Load Image command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// 
    /// Opens file dialog, loads image safely with CacheOption.OnLoad to prevent file lock.
    /// </summary>
    private void LoadImage()
    {
        try
        {
            // TCP-1.0.2: Open file dialog
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Title = "Load Background Image"
            };
            
            if (dialog.ShowDialog() == true)
            {
                // TCP-1.0.2: Load image safely
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Prevents file lock!
                bitmap.UriSource = new System.Uri(dialog.FileName);
                bitmap.EndInit();
                bitmap.Freeze(); // Thread-safe
                
                // TCP-1.0.2: Set properties
                BackgroundImage = bitmap;
                BackgroundImageName = System.IO.Path.GetFileName(dialog.FileName);
                IsLocked = false;
                
                // TCP-1.0.2: Show success toast
                TerminalService.Instance.LogSuccess("Background image loaded: Loaded: {BackgroundImageName}");
            }
        }
        catch (Exception ex)
        {
            // TCP-1.0.2: Show error toast (no crash)
            TerminalService.Instance.LogError($"Failed to load image: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Set Fit Mode command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private void SetFitMode()
    {
        try
        {
            ImageMode = EditorImageMode.Fit;
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions
        }
    }
    
    /// <summary>
    /// Set Actual Mode command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private void SetActualMode()
    {
        try
        {
            ImageMode = EditorImageMode.Actual;
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions
        }
    }
    
    /// <summary>
    /// Clear Image command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private void ClearImage()
    {
        try
        {
            BackgroundImage = null;
            BackgroundImageName = null;
            
            // TCP-1.0.2: Show toast (Removed per user request)
            // TerminalService.Instance.ShowInfo("Background image cleared", "Image removed from editor");
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions
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


