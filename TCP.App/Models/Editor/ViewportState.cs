using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace TCP.App.Models.Editor;

/// <summary>
/// ViewportState - Viewport transformation state
/// TCP-1.0.4: Background Image Load with Zoom/Pan
/// 
/// Bu model viewport'un zoom ve pan state'ini tutar.
/// Transform binding için WPF TransformGroup döner.
/// </summary>
public class ViewportState : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private double _zoomLevel = 1.0;
    private double _panX = 0.0;
    private double _panY = 0.0;

    /// <summary>
    /// Zoom level (0.1 to 10.0)
    /// TCP-1.0.4: Zoom functionality
    /// </summary>
    public double ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            // Clamp zoom level between 0.1 and 10.0
            var clamped = Math.Max(0.1, Math.Min(10.0, value));
            if (Math.Abs(_zoomLevel - clamped) > 0.001)
            {
                _zoomLevel = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Transform));
            }
        }
    }

    /// <summary>
    /// Pan X offset
    /// TCP-1.0.4: Pan functionality
    /// </summary>
    public double PanX
    {
        get => _panX;
        set
        {
            if (Math.Abs(_panX - value) > 0.001)
            {
                _panX = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Transform));
            }
        }
    }

    /// <summary>
    /// Pan Y offset
    /// TCP-1.0.4: Pan functionality
    /// </summary>
    public double PanY
    {
        get => _panY;
        set
        {
            if (Math.Abs(_panY - value) > 0.001)
            {
                _panY = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Transform));
            }
        }
    }

    /// <summary>
    /// Get WPF Transform for binding
    /// TCP-1.0.4: Transform binding
    /// 
    /// Returns TransformGroup with Scale + Translate transforms.
    /// </summary>
    public Transform Transform
    {
        get
        {
            var group = new TransformGroup();
            
            // Scale transform (zoom)
            group.Children.Add(new ScaleTransform(_zoomLevel, _zoomLevel));
            
            // Translate transform (pan)
            group.Children.Add(new TranslateTransform(_panX, _panY));
            
            return group;
        }
    }

    /// <summary>
    /// Reset viewport to default state
    /// TCP-1.0.4: Reset functionality
    /// </summary>
    public void Reset()
    {
        ZoomLevel = 1.0;
        PanX = 0.0;
        PanY = 0.0;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
