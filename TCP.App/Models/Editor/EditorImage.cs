using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace TCP.App.Models.Editor;

/// <summary>
/// EditorImage - Represents a background or reference image placed on the editor canvas.
/// Supports multiple images, aspect ratio preservation, opacity, and locking.
/// </summary>
public class EditorImage : INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private double _width = 400;
    private double _height = 300;
    private double _opacity = 1.0;
    private bool _isLocked;
    private bool _isSelected;

    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Path to the original image file (used for saving/loading states)
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double X
    {
        get => _x;
        set { if (_x != value) { _x = value; OnPropertyChanged(); } }
    }

    public double Y
    {
        get => _y;
        set { if (_y != value) { _y = value; OnPropertyChanged(); } }
    }

    public double Width
    {
        get => _width;
        set { if (_width != value) { _width = value; OnPropertyChanged(); } }
    }

    public double Height
    {
        get => _height;
        set { if (_height != value) { _height = value; OnPropertyChanged(); } }
    }

    public double Opacity
    {
        get => _opacity;
        set { if (Math.Abs(_opacity - value) > 0.001) { _opacity = value; OnPropertyChanged(); } }
    }

    public bool IsLocked
    {
        get => _isLocked;
        set { if (_isLocked != value) { _isLocked = value; OnPropertyChanged(); } }
    }

    [JsonIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
    }

    /// <summary>
    /// The actual loaded WPF BitmapImage object. Ignored during JSON serialization.
    /// </summary>
    [JsonIgnore]
    public BitmapImage? ImageSource { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
