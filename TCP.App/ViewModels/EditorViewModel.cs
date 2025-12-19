using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
    /// Current cursor world position (for display)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private Point _cursorWorld;
    
    /// <summary>
    /// Constructor - Initialize ViewportState
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public EditorViewModel()
    {
        _viewport = new ViewportState();
        _cursorWorld = new Point(0, 0);
    }
    
    /// <summary>
    /// Viewport state (readonly)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public ViewportState Viewport => _viewport;
    
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
    /// Update cursor world position (called from View)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public void UpdateCursorWorld(Point screenPoint, Size viewportSize)
    {
        try
        {
            // TCP-1.0.2: Convert screen point to world point
            _cursorWorld = _viewport.ScreenToWorld(screenPoint, viewportSize);
            
            // TCP-1.0.2: Notify UI of property change
            OnPropertyChanged(nameof(CursorWorldDisplay));
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during coordinate conversion
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
}
