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
        // TCP-1.0.3: Wire input router events
        if (EditorSurface != null)
        {
            EditorSurface.SizeChanged += EditorSurface_SizeChanged;
            
            // TCP-1.0.3: Wire input router events (replace direct MouseMove handler)
            EditorSurface.MouseMove += EditorSurface_MouseMove;
            EditorSurface.MouseDown += EditorSurface_MouseDown;
            EditorSurface.MouseUp += EditorSurface_MouseUp;
            EditorSurface.MouseWheel += EditorSurface_MouseWheel;
            EditorSurface.KeyDown += EditorSurface_KeyDown;
            EditorSurface.KeyUp += EditorSurface_KeyUp;
            
            // TCP-1.0.3: Ensure editor surface can receive keyboard focus
            EditorSurface.Focusable = true;
            
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
    /// TCP-1.0.3: EditorInputRouter - Forward to router
    /// </summary>
    private void EditorSurface_MouseMove(object sender, MouseEventArgs e)
    {
        try
        {
            // TCP-1.0.3: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.3: Safety guard - EditorSurface null check
            if (EditorSurface == null)
            {
                return;
            }
            
            // TCP-1.0.3: Get mouse position and state
            var screenPoint = e.GetPosition(EditorSurface);
            var modifiers = Keyboard.Modifiers;
            var leftButton = e.LeftButton;
            var rightButton = e.RightButton;
            var middleButton = e.MiddleButton;
            
            // TCP-1.0.3: Forward to input router
            viewModel.InputRouter.OnMouseMove(screenPoint, modifiers, leftButton, rightButton, middleButton);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse move
        }
    }
    
    /// <summary>
    /// EditorSurface MouseDown event handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private void EditorSurface_MouseDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            // TCP-1.0.3: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.3: Safety guard - EditorSurface null check
            if (EditorSurface == null)
            {
                return;
            }
            
            // TCP-1.0.3: Focus editor surface for keyboard input
            EditorSurface.Focus();
            
            // TCP-1.0.3: Get mouse position and state
            var screenPoint = e.GetPosition(EditorSurface);
            var button = e.ChangedButton;
            var modifiers = Keyboard.Modifiers;
            var leftButton = e.LeftButton;
            var rightButton = e.RightButton;
            var middleButton = e.MiddleButton;
            
            // TCP-1.0.3: Forward to input router
            viewModel.InputRouter.OnMouseDown(screenPoint, button, modifiers, leftButton, rightButton, middleButton);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse down
        }
    }
    
    /// <summary>
    /// EditorSurface MouseUp event handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private void EditorSurface_MouseUp(object sender, MouseButtonEventArgs e)
    {
        try
        {
            // TCP-1.0.3: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.3: Safety guard - EditorSurface null check
            if (EditorSurface == null)
            {
                return;
            }
            
            // TCP-1.0.3: Get mouse position and state
            var screenPoint = e.GetPosition(EditorSurface);
            var button = e.ChangedButton;
            var modifiers = Keyboard.Modifiers;
            var leftButton = e.LeftButton;
            var rightButton = e.RightButton;
            var middleButton = e.MiddleButton;
            
            // TCP-1.0.3: Forward to input router
            viewModel.InputRouter.OnMouseUp(screenPoint, button, modifiers, leftButton, rightButton, middleButton);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse up
        }
    }
    
    /// <summary>
    /// EditorSurface MouseWheel event handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// 
    /// NOTE: TCP-1.0.3 does NOT implement zoom/pan behavior yet.
    /// Wheel events are only collected and displayed in debug.
    /// </summary>
    private void EditorSurface_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        try
        {
            // TCP-1.0.3: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.3: Safety guard - EditorSurface null check
            if (EditorSurface == null)
            {
                return;
            }
            
            // TCP-1.0.3: Get mouse position and state
            var screenPoint = e.GetPosition(EditorSurface);
            var delta = e.Delta;
            var modifiers = Keyboard.Modifiers;
            var leftButton = e.LeftButton;
            var rightButton = e.RightButton;
            var middleButton = e.MiddleButton;
            
            // TCP-1.0.3: Forward to input router
            // NOTE: No zoom/pan behavior in TCP-1.0.3 - just collect event
            viewModel.InputRouter.OnMouseWheel(screenPoint, delta, modifiers, leftButton, rightButton, middleButton);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse wheel
        }
    }
    
    /// <summary>
    /// EditorSurface KeyDown event handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private void EditorSurface_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            // TCP-1.0.3: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.3: Get key and state
            var key = e.Key;
            var isRepeat = e.IsRepeat;
            var modifiers = Keyboard.Modifiers;
            
            // TCP-1.0.3: Forward to input router
            viewModel.InputRouter.OnKeyDown(key, isRepeat, modifiers);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during key down
        }
    }
    
    /// <summary>
    /// EditorSurface KeyUp event handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    private void EditorSurface_KeyUp(object sender, KeyEventArgs e)
    {
        try
        {
            // TCP-1.0.3: Safety guard - ViewModel null check
            if (DataContext is not EditorViewModel viewModel)
            {
                return;
            }
            
            // TCP-1.0.3: Get key and state
            var key = e.Key;
            var isRepeat = e.IsRepeat;
            var modifiers = Keyboard.Modifiers;
            
            // TCP-1.0.3: Forward to input router
            viewModel.InputRouter.OnKeyUp(key, isRepeat, modifiers);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during key up
        }
    }
}
