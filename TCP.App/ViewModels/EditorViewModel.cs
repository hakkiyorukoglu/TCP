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
    /// Input debug display text
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private string _inputDebugDisplay;
    
    /// <summary>
    /// Constructor - Initialize ViewportState and InputRouter
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public EditorViewModel()
    {
        _viewport = new ViewportState();
        _inputRouter = new EditorInputRouter();
        _cursorWorld = new Point(0, 0);
        _inputDebugDisplay = "Input: Routed";
        
        // TCP-1.0.3: Subscribe to router events
        WireInputRouter();
    }
    
    /// <summary>
    /// Wire input router events
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private void WireInputRouter()
    {
        try
        {
            // TCP-1.0.3: Subscribe to pointer moved event (update cursor world display)
            _inputRouter.PointerMoved += (evt) =>
            {
                try
                {
                    // TCP-1.0.3: Update cursor world coordinates using ViewportState
                    var viewportSize = _viewport.ViewportSize;
                    if (viewportSize.Width > 0 && viewportSize.Height > 0)
                    {
                        _cursorWorld = _viewport.ScreenToWorld(evt.ScreenPoint, viewportSize);
                        
                        // TCP-1.0.3: Update UI on UI thread
                        Application.Current?.Dispatcher.BeginInvoke(() =>
                        {
                            OnPropertyChanged(nameof(CursorWorldDisplay));
                        }, DispatcherPriority.Normal);
                    }
                }
                catch
                {
                    // TCP-1.0.3: Safety - ignore exceptions during pointer move
                }
            };
            
            // TCP-1.0.3: Subscribe to wheel changed event (update debug display)
            _inputRouter.WheelChanged += (evt) =>
            {
                try
                {
                    UpdateInputDebugDisplay();
                }
                catch
                {
                    // TCP-1.0.3: Safety - ignore exceptions during wheel change
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
    /// Zoom display (e.g. "100%")
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public string ZoomDisplay
    {
        get
        {
            var zoomPercent = (int)(_viewport.Zoom * 100);
            return $"{zoomPercent}%";
        }
    }
    
    /// <summary>
    /// Cursor world coordinates display (e.g. "X: 0.00  Y: 0.00")
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public string CursorWorldDisplay
    {
        get
        {
            return $"X: {_cursorWorld.X:F2}  Y: {_cursorWorld.Y:F2}";
        }
    }
    
    
    /// <summary>
    /// Update viewport size (called from View)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public void UpdateViewportSize(Size size)
    {
        try
        {
            _viewport.SetViewportSize(size);
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during viewport size update
        }
    }
    
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
