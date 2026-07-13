using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// Represents a floating RFID Tag that is not attached to any Station or Modem.
/// </summary>
public class RfidTagInstance : ILayerItem, INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private bool _isLocked;
    private bool _isSelected;
    private bool _isVisible = true;

    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public string LayerName => Name;
    
    [JsonIgnore]
    public string LayerType => "RFID Tag";

    public string Name { get; set; } = "RFID Etiket";
    public string Type { get; set; } = "RFID Tag";

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

    public bool IsVisible
    {
        get => _isVisible;
        set { if (_isVisible != value) { _isVisible = value; OnPropertyChanged(); } }
    }

    /// <summary>
    /// The unique identifier emitted by this RFID tag when scanned.
    /// </summary>
    public string Uid { get; set; } = "A1B2C3D4";

    public RfidTagInstance()
    {
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
