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
    /// TCP-1.0.4: Zoom + Pan - Increased max zoom for better range
    /// </summary>
    public const double MaxZoom = 50.0;
    
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
    /// TCP-1.0.4: Zoom + Pan - Added NaN/Infinity guards
    /// </summary>
    public Vector Pan
    {
        get => _pan;
        set
        {
            // TCP-1.0.4: Safety guard - NaN/Infinity check
            if (double.IsNaN(value.X) || double.IsNaN(value.Y) ||
                double.IsInfinity(value.X) || double.IsInfinity(value.Y))
            {
                // TCP-1.0.4: Reset to safe default
                _pan = new Vector(0, 0);
                return;
            }
            
            _pan = value;
        }
    }
    
    /// <summary>
    /// Zoom level (clamped between MinZoom and MaxZoom)
    /// TCP-1.0.2: ViewportState (World/Screen transform foundation)
    /// TCP-1.0.4: Zoom + Pan - Added NaN/Infinity guards
    /// </summary>
    public double Zoom
    {
        get => _zoom;
        set
        {
            // TCP-1.0.4: Safety guard - NaN/Infinity check
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                // TCP-1.0.4: Reset to last valid value (or default)
                return;
            }
            
            // TCP-1.0.4: Ensure zoom never equals 0
            if (value <= 0)
            {
                value = MinZoom;
            }
            
            // TCP-1.0.2: Clamp zoom to valid range
            var clamped = Math.Max(MinZoom, Math.Min(MaxZoom, value));
            
            // TCP-1.0.4: Final NaN/Infinity check after clamp
            if (double.IsNaN(clamped) || double.IsInfinity(clamped))
            {
                clamped = 1.0; // Safe default
            }
            
            _zoom = clamped;
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
    
    /// <summary>
    /// Zoom at screen anchor point (keeps world point under cursor stable)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// 
    /// This method zooms while keeping the world point under the cursor stable.
    /// </summary>
    public void ZoomAt(Point screenAnchor, int wheelDelta, Size viewportSize)
    {
        try
        {
            // TCP-1.0.4: Safety guard - invalid viewport size
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
            {
                return;
            }
            
            // TCP-1.0.4: Safety guard - zero wheel delta (no-op)
            if (wheelDelta == 0)
            {
                return;
            }
            
            // TCP-1.0.4: Get world point under cursor before zoom
            var worldBefore = ScreenToWorld(screenAnchor, viewportSize);
            
            // TCP-1.0.4: Safety guard - NaN/Infinity check
            if (double.IsNaN(worldBefore.X) || double.IsNaN(worldBefore.Y) ||
                double.IsInfinity(worldBefore.X) || double.IsInfinity(worldBefore.Y))
            {
                return;
            }
            
            // TCP-1.0.4: Calculate zoom step (smooth exponential)
            var step = Math.Pow(1.0015, wheelDelta);
            
            // TCP-1.0.4: Safety guard - NaN/Infinity check for step
            if (double.IsNaN(step) || double.IsInfinity(step))
            {
                return;
            }
            
            // TCP-1.0.4: Apply zoom (clamp is handled by Zoom property setter)
            var oldZoom = _zoom;
            Zoom = _zoom * step;
            
            // TCP-1.0.4: Get world point under cursor after zoom
            var worldAfter = ScreenToWorld(screenAnchor, viewportSize);
            
            // TCP-1.0.4: Safety guard - NaN/Infinity check
            if (double.IsNaN(worldAfter.X) || double.IsNaN(worldAfter.Y) ||
                double.IsInfinity(worldAfter.X) || double.IsInfinity(worldAfter.Y))
            {
                // TCP-1.0.4: Revert zoom if transformation failed
                _zoom = oldZoom;
                return;
            }
            
            // TCP-1.0.4: Adjust pan to keep cursor pinned (worldBefore should map to same screen point)
            var worldDelta = new Vector(worldBefore.X - worldAfter.X, worldBefore.Y - worldAfter.Y);
            
            // TCP-1.0.4: Safety guard - NaN/Infinity check for delta
            if (double.IsNaN(worldDelta.X) || double.IsNaN(worldDelta.Y) ||
                double.IsInfinity(worldDelta.X) || double.IsInfinity(worldDelta.Y))
            {
                // TCP-1.0.4: Revert zoom if delta calculation failed
                _zoom = oldZoom;
                return;
            }
            
            // TCP-1.0.4: Apply pan adjustment
            _pan = new Vector(_pan.X + worldDelta.X, _pan.Y + worldDelta.Y);
            
            // TCP-1.0.4: Safety guard - Validate pan after update
            if (double.IsNaN(_pan.X) || double.IsNaN(_pan.Y) ||
                double.IsInfinity(_pan.X) || double.IsInfinity(_pan.Y))
            {
                // TCP-1.0.4: Reset pan if invalid
                _pan = new Vector(0, 0);
            }
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions during zoom
        }
    }
    
    /// <summary>
    /// Pan by screen delta (convert screen pixels to world delta)
    /// TCP-1.0.4: Zoom + Pan (working)
    /// 
    /// Pan direction: positive screenDelta moves viewport in opposite direction (drag moves content)
    /// </summary>
    public void PanBy(Vector screenDelta, Size viewportSize)
    {
        try
        {
            // TCP-1.0.4: Safety guard - invalid viewport size
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
            {
                return;
            }
            
            // TCP-1.0.4: Safety guard - zero delta (no-op)
            if (screenDelta.X == 0 && screenDelta.Y == 0)
            {
                return;
            }
            
            // TCP-1.0.4: Safety guard - invalid zoom
            if (_zoom <= 0 || double.IsNaN(_zoom) || double.IsInfinity(_zoom))
            {
                return;
            }
            
            // TCP-1.0.4: Convert screen delta to world delta
            // Screen pixels divided by zoom gives world units
            var worldDelta = new Vector(screenDelta.X / _zoom, screenDelta.Y / _zoom);
            
            // TCP-1.0.4: Safety guard - NaN/Infinity check
            if (double.IsNaN(worldDelta.X) || double.IsNaN(worldDelta.Y) ||
                double.IsInfinity(worldDelta.X) || double.IsInfinity(worldDelta.Y))
            {
                return;
            }
            
            // TCP-1.0.4: Apply pan (subtract because dragging moves content opposite to mouse)
            _pan = new Vector(_pan.X - worldDelta.X, _pan.Y - worldDelta.Y);
            
            // TCP-1.0.4: Safety guard - Validate pan after update
            if (double.IsNaN(_pan.X) || double.IsNaN(_pan.Y) ||
                double.IsInfinity(_pan.X) || double.IsInfinity(_pan.Y))
            {
                // TCP-1.0.4: Reset pan if invalid
                _pan = new Vector(0, 0);
            }
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions during pan
        }
    }
}
