using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TCP.App.Editor.Input;
using TCP.App.Editor.Viewport;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// EditorViewModel - Editor modülü ViewModel'i
/// 
/// TCP-1.0.1: Editor Foundation (Empty Scene)
/// TCP-1.0.2: ViewportState (World/Screen transform foundation)
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
    
    /// <summary>
    /// Viewport state instance
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private readonly ViewportState _viewport;
    
    /// <summary>
    /// Input router instance
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private readonly EditorInputRouter _inputRouter;
    
    /// <summary>
    /// Current cursor world position (for display)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private Point _cursorWorld;
    
    /// <summary>
    /// Current mouse screen position
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    private Point _currentMouseScreen;
    
    /// <summary>
    /// Pan state - is currently panning
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    private bool _isPanning;
    
    /// <summary>
    /// Pan start screen position
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    private Point _panStartScreen;
    
    /// <summary>
    /// Viewport size (for zoom/pan operations)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    private Size _viewportSize;
    
    /// <summary>
    /// Input debug display text
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private string _inputDebugDisplay;
    
    /// <summary>
    /// Request redraw event (for render refresh)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// 
    /// This event is raised when viewport changes require a visual redraw.
    /// </summary>
    public event Action? RequestRedraw;
    
    /// <summary>
    /// Raise RequestRedraw event (safe wrapper, public for View)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    public void RequestRedrawNow()
    {
        try
        {
            RequestRedraw?.Invoke();
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions during redraw request
        }
    }
    
    /// <summary>
    /// Raise RequestRedraw event (internal wrapper)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    private void OnRequestRedraw()
    {
        RequestRedrawNow();
    }
    
    /// <summary>
    /// Constructor - Initialize ViewportState and InputRouter
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    public EditorViewModel()
    {
        _viewport = new ViewportState();
        _inputRouter = new EditorInputRouter();
        _cursorWorld = new Point(0, 0);
        _currentMouseScreen = new Point(0, 0);
        _isPanning = false;
        _panStartScreen = new Point(0, 0);
        _viewportSize = new Size(800, 600); // Default
        _inputDebugDisplay = "Input: Routed";
        
        // TCP-1.0.3: Subscribe to router events
        WireInputRouter();
    }
    
    /// <summary>
    /// Wire input router events
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// TCP-1.0.4: Zoom + Pan - Implement zoom/pan behavior
    /// </summary>
    private void WireInputRouter()
    {
        try
        {
            // TCP-1.0.4: Subscribe to pointer moved event (update cursor + pan if active)
            _inputRouter.PointerMoved += (evt) =>
            {
                try
                {
                    // TCP-1.0.4: Safety guard - invalid viewport size
                    if (_viewportSize.Width <= 0 || _viewportSize.Height <= 0)
                    {
                        return;
                    }
                    
                    // TCP-1.0.4: Update current mouse screen position
                    _currentMouseScreen = evt.ScreenPoint;
                    
                    // TCP-1.0.4: Update cursor world coordinates
                    _cursorWorld = _viewport.ScreenToWorld(evt.ScreenPoint, _viewportSize);
                    
                    // TCP-1.0.4: Safety guard - NaN/Infinity check
                    if (double.IsNaN(_cursorWorld.X) || double.IsNaN(_cursorWorld.Y) ||
                        double.IsInfinity(_cursorWorld.X) || double.IsInfinity(_cursorWorld.Y))
                    {
                        _cursorWorld = new Point(0, 0);
                    }
                    
                    // TCP-1.0.4: Handle panning (middle mouse drag)
                    if (_isPanning)
                    {
                        // TCP-1.0.4: Calculate screen delta since last move
                        var screenDelta = new Vector(
                            evt.ScreenPoint.X - _panStartScreen.X,
                            evt.ScreenPoint.Y - _panStartScreen.Y);
                        
                        // TCP-1.0.4: Apply pan
                        _viewport.PanBy(screenDelta, _viewportSize);
                        
                        // TCP-1.0.4: Update pan start for next move
                        _panStartScreen = evt.ScreenPoint;
                        
                        // TCP-1.0.4: Update cursor world after pan
                        _cursorWorld = _viewport.ScreenToWorld(evt.ScreenPoint, _viewportSize);
                        
                        // TCP-1.0.4: Request redraw
                        OnRequestRedraw();
                    }
                    
                    // TCP-1.0.4: Update UI on UI thread
                    Application.Current?.Dispatcher.BeginInvoke(() =>
                    {
                        OnPropertyChanged(nameof(CursorWorldDisplay));
                    }, DispatcherPriority.Normal);
                }
                catch
                {
                    // TCP-1.0.4: Safety - ignore exceptions during pointer move
                }
            };
            
            // TCP-1.0.4: Subscribe to pointer down event (start panning on middle button)
            _inputRouter.PointerDown += (evt) =>
            {
                try
                {
                    // TCP-1.0.4: Start panning on middle mouse button
                    if (evt.Button == MouseButton.Middle)
                    {
                        _isPanning = true;
                        _panStartScreen = evt.ScreenPoint;
                    }
                }
                catch
                {
                    // TCP-1.0.4: Safety - ignore exceptions during pointer down
                }
            };
            
            // TCP-1.0.4: Subscribe to pointer up event (stop panning on middle button)
            _inputRouter.PointerUp += (evt) =>
            {
                try
                {
                    // TCP-1.0.4: Stop panning on middle mouse button release
                    if (evt.Button == MouseButton.Middle)
                    {
                        _isPanning = false;
                    }
                }
                catch
                {
                    // TCP-1.0.4: Safety - ignore exceptions during pointer up
                }
            };
            
            // TCP-1.0.4: Subscribe to wheel changed event (zoom)
            _inputRouter.WheelChanged += (evt) =>
            {
                try
                {
                    // TCP-1.0.4: Safety guard - invalid viewport size
                    if (_viewportSize.Width <= 0 || _viewportSize.Height <= 0)
                    {
                        return;
                    }
                    
                    // TCP-1.0.4: Zoom at cursor position
                    _viewport.ZoomAt(evt.ScreenPoint, evt.Delta, _viewportSize);
                    
                    // TCP-1.0.4: Update cursor world coordinates after zoom
                    _cursorWorld = _viewport.ScreenToWorld(evt.ScreenPoint, _viewportSize);
                    
                    // TCP-1.0.4: Safety guard - NaN/Infinity check
                    if (double.IsNaN(_cursorWorld.X) || double.IsNaN(_cursorWorld.Y) ||
                        double.IsInfinity(_cursorWorld.X) || double.IsInfinity(_cursorWorld.Y))
                    {
                        _cursorWorld = new Point(0, 0);
                    }
                    
                    // TCP-1.0.4: Request redraw
                    OnRequestRedraw();
                    
                    // TCP-1.0.4: Update UI on UI thread
                    Application.Current?.Dispatcher.BeginInvoke(() =>
                    {
                        OnPropertyChanged(nameof(ZoomDisplay));
                        OnPropertyChanged(nameof(CursorWorldDisplay));
                        UpdateInputDebugDisplay();
                    }, DispatcherPriority.Normal);
                }
                catch
                {
                    // TCP-1.0.4: Safety - ignore exceptions during wheel change
                }
            };
            
            // TCP-1.0.3: Subscribe to key down event (update debug display)
            _inputRouter.KeyDown += (evt) =>
            {
                try
                {
                    UpdateInputDebugDisplay();
                }
                catch
                {
                    // TCP-1.0.3: Safety - ignore exceptions during key down
                }
            };
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during router wiring
        }
    }
    
    /// <summary>
    /// Update input debug display
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private void UpdateInputDebugDisplay()
    {
        try
        {
            var router = _inputRouter;
            var left = router.LeftButton == MouseButtonState.Pressed ? 1 : 0;
            var right = router.RightButton == MouseButtonState.Pressed ? 1 : 0;
            var middle = router.MiddleButton == MouseButtonState.Pressed ? 1 : 0;
            var ctrl = router.IsCtrlDown ? 1 : 0;
            var shift = router.IsShiftDown ? 1 : 0;
            var alt = router.IsAltDown ? 1 : 0;
            var wheel = router.LastWheelDelta != 0 ? router.LastWheelDelta.ToString("+0;-0") : "0";
            var key = router.LastKey?.ToString() ?? "None";
            
            _inputDebugDisplay = $"L:{left} R:{right} M:{middle}  Ctrl:{ctrl} Shift:{shift} Alt:{alt}  Wheel:{wheel}  Key:{key}";
            
            // TCP-1.0.3: Update UI on UI thread
            Application.Current?.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(InputDebugDisplay));
            }, DispatcherPriority.Normal);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during debug display update
        }
    }
    
    /// <summary>
    /// Viewport state (readonly)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public ViewportState Viewport => _viewport;
    
    /// <summary>
    /// Input router (readonly)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public EditorInputRouter InputRouter => _inputRouter;
    
    /// <summary>
    /// Editor title
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// </summary>
    public string Title => "Editor";
    
    /// <summary>
    /// Status text (shown in StatusBar)
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// </summary>
    public string StatusText => "Editor ready";
    
    /// <summary>
    /// Current version (from VersionManager)
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// </summary>
    public string CurrentVersion => VersionManager.DisplayVersion;
    
    /// <summary>
    /// Zoom display (e.g. "Zoom: 100%")
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// TCP-1.0.4: Zoom + Pan - Updated format
    /// </summary>
    public string ZoomDisplay
    {
        get
        {
            try
            {
                // TCP-1.0.4: Safety guard - invalid zoom
                if (double.IsNaN(_viewport.Zoom) || double.IsInfinity(_viewport.Zoom) || _viewport.Zoom <= 0)
                {
                    return "Zoom: 100%";
                }
                
                var zoomPercent = (int)(_viewport.Zoom * 100);
                return $"Zoom: {zoomPercent}%";
            }
            catch
            {
                // TCP-1.0.4: Safety - return default on format error
                return "Zoom: 100%";
            }
        }
    }
    
    /// <summary>
    /// Cursor world coordinates display (e.g. "X: 0.00  Y: 0.00")
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// TCP-1.0.4: Zoom + Pan - Safe formatting with guards
    /// </summary>
    public string CursorWorldDisplay
    {
        get
        {
            try
            {
                // TCP-1.0.4: Safety guard - NaN/Infinity check
                var x = double.IsNaN(_cursorWorld.X) || double.IsInfinity(_cursorWorld.X) ? 0.0 : _cursorWorld.X;
                var y = double.IsNaN(_cursorWorld.Y) || double.IsInfinity(_cursorWorld.Y) ? 0.0 : _cursorWorld.Y;
                
                return $"X: {x:0.##}  Y: {y:0.##}";
            }
            catch
            {
                // TCP-1.0.4: Safety - return default on format error
                return "X: 0.00  Y: 0.00";
            }
        }
    }
    
    
    /// <summary>
    /// Update viewport size (called from View)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// TCP-1.0.4: Zoom + Pan - Store viewport size for zoom/pan operations
    /// </summary>
    public void UpdateViewportSize(Size size)
    {
        try
        {
            // TCP-1.0.4: Safety guard - invalid size
            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }
            
            _viewportSize = size;
            _viewport.SetViewportSize(size);
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during viewport size update
        }
    }
    
    /// <summary>
    /// Is panning (readonly, for UI cursor change)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// </summary>
    public bool IsPanning => _isPanning;
    
    /// <summary>
    /// Input debug display (e.g. "L:0 R:0 M:1  Ctrl:0 Shift:0 Alt:0  Wheel:+120")
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public string InputDebugDisplay
    {
        get => _inputDebugDisplay;
        private set
        {
            if (_inputDebugDisplay != value)
            {
                _inputDebugDisplay = value;
                OnPropertyChanged();
            }
        }
    }
}
