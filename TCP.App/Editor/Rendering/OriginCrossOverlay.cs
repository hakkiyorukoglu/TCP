using System.Windows;
using System.Windows.Media;
using TCP.App.Editor.Viewport;

namespace TCP.App.Editor.Rendering;

/// <summary>
/// OriginCrossOverlay - Debug overlay for world origin (0,0)
/// 
/// TCP-1.0.2: ViewportState (World/Screen transform foundation)
/// 
/// Draws a small crosshair at world origin (0,0) to prove transforms work.
/// 
/// Single Responsibility: Origin crosshair rendering
/// </summary>
public class OriginCrossOverlay : FrameworkElement
{
    /// <summary>
    /// Viewport state (for coordinate transformation)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private ViewportState? _viewport;
    
    /// <summary>
    /// Viewport size (for coordinate transformation)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private Size _viewportSize;
    
    /// <summary>
    /// Set viewport state
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public void SetViewport(ViewportState? viewport, Size viewportSize)
    {
        _viewport = viewport;
        _viewportSize = viewportSize;
        
        // TCP-1.0.2: Set element size to match viewport
        Width = viewportSize.Width;
        Height = viewportSize.Height;
        
        InvalidateVisual();
    }
    
    /// <summary>
    /// Render origin crosshair
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    protected override void OnRender(DrawingContext dc)
    {
        try
        {
            // TCP-1.0.2: Safety guard - Viewport null check
            if (_viewport == null)
            {
                return;
            }
            
            // TCP-1.0.2: Safety guard - Invalid viewport size
            if (_viewportSize.Width <= 0 || _viewportSize.Height <= 0)
            {
                return;
            }
            
            // TCP-1.0.2: Convert world origin (0,0) to screen coordinates
            var worldOrigin = new Point(0, 0);
            var screenOrigin = _viewport.WorldToScreen(worldOrigin, _viewportSize);
            
            // TCP-1.0.2: Safety guard - NaN/Infinity check
            if (double.IsNaN(screenOrigin.X) || double.IsNaN(screenOrigin.Y) ||
                double.IsInfinity(screenOrigin.X) || double.IsInfinity(screenOrigin.Y))
            {
                return;
            }
            
            // TCP-1.0.2: Get theme brush for crosshair
            var brush = Application.Current.TryFindResource("Brush.Accent.Primary") as SolidColorBrush;
            if (brush == null)
            {
                brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(79, 195, 247));
            }
            
            var pen = new Pen(brush, 1.0);
            
            // TCP-1.0.2: Draw crosshair (2 short lines crossing at origin)
            var crossSize = 10.0; // 10 pixels
            var startX = screenOrigin.X - crossSize / 2;
            var endX = screenOrigin.X + crossSize / 2;
            var startY = screenOrigin.Y - crossSize / 2;
            var endY = screenOrigin.Y + crossSize / 2;
            
            // Horizontal line
            dc.DrawLine(pen, new Point(startX, screenOrigin.Y), new Point(endX, screenOrigin.Y));
            
            // Vertical line
            dc.DrawLine(pen, new Point(screenOrigin.X, startY), new Point(screenOrigin.X, endY));
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions during rendering
        }
    }
}
