using System.Windows;
using System.Windows.Input;
using TCP.App.Models.Editor;

namespace TCP.App.Services;

/// <summary>
/// EditorInputRouter - Mouse input handler for viewport zoom/pan
/// TCP-1.0.4: Background Image Load with Zoom/Pan
/// 
/// Bu servis mouse input'larını handle eder ve ViewportState'i günceller.
/// - Mouse wheel: Zoom in/out
/// - Middle mouse drag: Pan
/// 
/// MVVM Pattern: UI referansı yok, sadece mantık.
/// </summary>
public class EditorInputRouter
{
    private readonly ViewportState _viewportState;
    private bool _isPanning = false;
    private Point _lastPanPosition;

    /// <summary>
    /// Constructor
    /// TCP-1.0.4: Initialize with ViewportState reference
    /// </summary>
    public EditorInputRouter(ViewportState viewportState)
    {
        _viewportState = viewportState ?? throw new ArgumentNullException(nameof(viewportState));
    }

    /// <summary>
    /// Handle mouse wheel event (zoom)
    /// TCP-1.0.4: Zoom functionality
    /// 
    /// Mouse wheel UP: Zoom in (1.1x multiplier)
    /// Mouse wheel DOWN: Zoom out (0.9x multiplier)
    /// </summary>
    public void HandleMouseWheel(MouseWheelEventArgs e)
    {
        try
        {
            if (e.Delta > 0)
            {
                // Zoom in
                _viewportState.ZoomLevel *= 1.1;
            }
            else if (e.Delta < 0)
            {
                // Zoom out
                _viewportState.ZoomLevel *= 0.9;
            }

            e.Handled = true;
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions
        }
    }

    /// <summary>
    /// Handle mouse down event (start pan)
    /// TCP-1.0.4: Pan functionality
    /// 
    /// Middle mouse button: Start panning
    /// </summary>
    public void HandleMouseDown(MouseButtonEventArgs e)
    {
        try
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isPanning = true;
                _lastPanPosition = e.GetPosition(null);
                e.Handled = true;
            }
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions
        }
    }

    /// <summary>
    /// Handle mouse move event (pan)
    /// TCP-1.0.4: Pan functionality
    /// 
    /// If panning: Update pan offset based on mouse delta
    /// </summary>
    public void HandleMouseMove(MouseEventArgs e)
    {
        try
        {
            if (_isPanning && e.MiddleButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(null);
                var delta = currentPosition - _lastPanPosition;

                _viewportState.PanX += delta.X;
                _viewportState.PanY += delta.Y;

                _lastPanPosition = currentPosition;
                e.Handled = true;
            }
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions
        }
    }

    /// <summary>
    /// Handle mouse up event (stop pan)
    /// TCP-1.0.4: Pan functionality
    /// 
    /// Middle mouse button released: Stop panning
    /// </summary>
    public void HandleMouseUp(MouseButtonEventArgs e)
    {
        try
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _isPanning = false;
                e.Handled = true;
            }
        }
        catch
        {
            // TCP-1.0.4: Safety - ignore exceptions
        }
    }
}
