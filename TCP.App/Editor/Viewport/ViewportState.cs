using System;
using System.Windows;

namespace TCP.App.Editor.Viewport;

/// <summary>
/// ViewportState - World/Screen coordinate transformation
/// 
/// TCP-1.0.2: ViewportState (World/Screen transform foundation)
/// 
/// Bu sınıf Editor için viewport state'ini ve world/screen dönüşümlerini yönetir.
/// 
/// Coordinate Convention:
/// - World origin (0,0) maps to the CENTER of the viewport when Pan=(0,0) and Zoom=1
/// - Pan is a world offset (world coordinates)
/// - Zoom: 1.0 = 100%, >1.0 = zoom in, <1.0 = zoom out
/// 
/// Single Responsibility: Viewport state ve coordinate transformation
/// </summary>
public class ViewportState
{
    /// <summary>
    /// Minimum zoom level
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public const double MinZoom = 0.05;
    
    /// <summary>
    /// Maximum zoom level
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public const double MaxZoom = 40.0;
    
    /// <summary>
    /// Pan offset (world coordinates)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// 
    /// Pan represents the world-space offset from the center.
    /// When Pan=(0,0) and Zoom=1, world origin (0,0) is at viewport center.
    /// </summary>
    private Vector _pan;
    
    /// <summary>
    /// Zoom level
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// 
    /// 1.0 = 100%, >1.0 = zoom in, <1.0 = zoom out
    /// </summary>
    private double _zoom;
    
    /// <summary>
    /// Viewport size (screen pixels)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    private Size _viewportSize;
    
    /// <summary>
    /// Constructor - Initialize with default values
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public ViewportState()
    {
        _pan = new Vector(0, 0);
        _zoom = 1.0;
        _viewportSize = new Size(800, 600); // Default size
    }
    
    /// <summary>
    /// Pan offset (world coordinates)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public Vector Pan
    {
        get => _pan;
        set => _pan = value;
    }
    
    /// <summary>
    /// Zoom level (clamped between MinZoom and MaxZoom)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public double Zoom
    {
        get => _zoom;
        set
        {
            // TCP-1.0.2: Clamp zoom to valid range
            _zoom = Math.Max(MinZoom, Math.Min(MaxZoom, value));
        }
    }
    
    /// <summary>
    /// Viewport size (screen pixels)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public Size ViewportSize
    {
        get => _viewportSize;
        set
        {
            // TCP-1.0.2: Safety guard - ensure non-negative size
            if (value.Width < 0 || value.Height < 0)
            {
                return;
            }
            _viewportSize = value;
        }
    }
    
    /// <summary>
    /// Set viewport size (convenience method)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public void SetViewportSize(Size size)
    {
        ViewportSize = size;
    }
    
    /// <summary>
    /// Convert screen point to world point
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// 
    /// Coordinate convention:
    /// - Screen origin (0,0) is top-left
    /// - World origin (0,0) maps to viewport center when Pan=(0,0) and Zoom=1
    /// - Pan is a world offset
    /// </summary>
    public Point ScreenToWorld(Point screen, Size viewportSize)
    {
        // TCP-1.0.2: Safety guard - invalid viewport size
        if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
        {
            return new Point(0, 0);
        }
        
        // TCP-1.0.2: Convert screen to world coordinates
        // Screen origin (0,0) is top-left
        // World origin (0,0) is at viewport center when Pan=(0,0) and Zoom=1
        
        // Step 1: Convert screen to viewport-relative coordinates (center as origin)
        var viewportCenterX = viewportSize.Width / 2.0;
        var viewportCenterY = viewportSize.Height / 2.0;
        var relativeX = screen.X - viewportCenterX;
        var relativeY = screen.Y - viewportCenterY;
        
        // Step 2: Apply inverse zoom (divide by zoom)
        var worldX = relativeX / _zoom;
        var worldY = relativeY / _zoom;
        
        // Step 3: Apply inverse pan (subtract pan offset)
        var worldPoint = new Point(worldX - _pan.X, worldY - _pan.Y);
        
        return worldPoint;
    }
    
    /// <summary>
    /// Convert world point to screen point
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// 
    /// Coordinate convention:
    /// - Screen origin (0,0) is top-left
    /// - World origin (0,0) maps to viewport center when Pan=(0,0) and Zoom=1
    /// </summary>
    public Point WorldToScreen(Point world, Size viewportSize)
    {
        // TCP-1.0.2: Safety guard - invalid viewport size
        if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
        {
            return new Point(0, 0);
        }
        
        // TCP-1.0.2: Convert world to screen coordinates
        // Step 1: Apply pan offset
        var worldX = world.X + _pan.X;
        var worldY = world.Y + _pan.Y;
        
        // Step 2: Apply zoom (multiply by zoom)
        var scaledX = worldX * _zoom;
        var scaledY = worldY * _zoom;
        
        // Step 3: Convert to screen coordinates (viewport center as origin -> top-left origin)
        var viewportCenterX = viewportSize.Width / 2.0;
        var viewportCenterY = viewportSize.Height / 2.0;
        var screenX = scaledX + viewportCenterX;
        var screenY = scaledY + viewportCenterY;
        
        return new Point(screenX, screenY);
    }
    
    /// <summary>
    /// Reset viewport to default state
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// </summary>
    public void Reset()
    {
        _pan = new Vector(0, 0);
        _zoom = 1.0;
    }
}
