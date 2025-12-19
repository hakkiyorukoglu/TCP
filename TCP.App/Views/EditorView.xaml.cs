using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCP.App.Editor.Rendering;
using TCP.App.ViewModels;

namespace TCP.App.Views;

/// <summary>
/// EditorView.xaml.cs - Editor view code-behind
/// 
/// TCP-1.0.1: Editor Foundation (Empty Scene)
/// TCP-1.0.2: ViewportState (World/Screen transform foundation)
/// 
/// DataContext safety: Explicitly set DataContext to EditorViewModel if null.
/// Viewport size and mouse move tracking for cursor world coordinates.
/// </summary>
public partial class EditorView : System.Windows.Controls.UserControl
{
    /// <summary>
    /// Constructor - Initialize DataContext and wire events
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// TCP-1.0.2: ViewportState - Wire SizeChanged and MouseMove events
    /// </summary>
    public EditorView()
    {
        InitializeComponent();
        
        // TCP-1.0.1: DataContext safety - Set if null
        if (DataContext == null || !(DataContext is EditorViewModel))
        {
            DataContext = new EditorViewModel();
        }
        
        this.Loaded += EditorView_Loaded;
    }
    
    /// <summary>
    /// Origin crosshair overlay instance
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private OriginCrossOverlay? _originOverlay;
    
    /// <summary>
    /// Grid container for overlay (parent of EditorSurface)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private Grid? _overlayContainer;
    
    /// <summary>
    /// Loaded event handler - Ensure DataContext is set and wire events
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// TCP-1.0.2: ViewportState - Wire SizeChanged and MouseMove events
    /// </summary>
    private void EditorView_Loaded(object sender, RoutedEventArgs e)
    {
        // TCP-1.0.1: DataContext safety
        if (DataContext == null || !(DataContext is EditorViewModel))
        {
            DataContext = new EditorViewModel();
        }
        
        // TCP-1.0.2: Find overlay container (Grid that contains EditorSurface)
        _overlayContainer = EditorSurface?.Parent as Grid;
        
        // TCP-1.0.2: Wire viewport size changed event
        if (EditorSurface != null)
        {
            EditorSurface.SizeChanged += EditorSurface_SizeChanged;
            EditorSurface.MouseMove += EditorSurface_MouseMove;
            
            // TCP-1.0.2: Initialize viewport size
            if (DataContext is EditorViewModel viewModel)
            {
                viewModel.UpdateViewportSize(new Size(EditorSurface.ActualWidth, EditorSurface.ActualHeight));
                
                // TCP-1.0.2: Create and add origin crosshair overlay
                if (_overlayContainer != null)
                {
                    _originOverlay = new OriginCrossOverlay();
                    _overlayContainer.Children.Add(_originOverlay);
                    UpdateOriginOverlay(viewModel);
                }
            }
        }
    }
    
    /// <summary>
    /// Update origin overlay viewport
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private void UpdateOriginOverlay(EditorViewModel viewModel)
    {
        try
        {
            if (_originOverlay != null && EditorSurface != null)
            {
                var viewportSize = new Size(EditorSurface.ActualWidth, EditorSurface.ActualHeight);
                _originOverlay.SetViewport(viewModel.Viewport, viewportSize);
            }
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during overlay update
        }
    }
    
    /// <summary>
    /// EditorSurface SizeChanged event handler
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// 
    /// Updates viewport size when EditorSurface size changes.
    /// </summary>
    private void EditorSurface_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        try
        {
            // TCP-1.0.2: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.2: Safety guard - Invalid size
            if (e.NewSize.Width <= 0 || e.NewSize.Height <= 0)
            {
                return;
            }
            
            // TCP-1.0.2: Update viewport size
            viewModel.UpdateViewportSize(e.NewSize);
            
            // TCP-1.0.2: Update origin overlay
            UpdateOriginOverlay(viewModel);
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during size update
        }
    }
    
    /// <summary>
    /// EditorSurface MouseMove event handler
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// 
    /// Updates cursor world coordinates display as mouse moves.
    /// </summary>
    private void EditorSurface_MouseMove(object sender, MouseEventArgs e)
    {
        try
        {
            // TCP-1.0.2: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.2: Safety guard - EditorSurface null check
            if (EditorSurface == null)
            {
                return;
            }
            
            // TCP-1.0.2: Get mouse position relative to EditorSurface
            var screenPoint = e.GetPosition(EditorSurface);
            var viewportSize = new Size(EditorSurface.ActualWidth, EditorSurface.ActualHeight);
            
            // TCP-1.0.2: Safety guard - Invalid viewport size
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
            {
                return;
            }
            
            // TCP-1.0.2: Update cursor world coordinates
            viewModel.UpdateCursorWorld(screenPoint, viewportSize);
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during mouse move
        }
    }
}
