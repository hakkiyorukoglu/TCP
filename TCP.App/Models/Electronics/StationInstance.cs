using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// StationInstance - Represents a physical Mega/Controller node on the network.
/// </summary>
public class StationInstance : ILayerItem, INotifyPropertyChanged
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
    public string LayerType => "Station";
    
    [JsonIgnore]
    public string Type => "Mega (İstasyon)";

    public string Name { get; set; } = "Yeni İstasyon";
    public string IpAddress { get; set; } = "192.168.1.10";
    public int Port { get; set; } = 80;
    public string MacAddress { get; set; } = "00:00:00:00:00:00";
    public string RouterPort { get; set; } = "Port 1";

    /// <summary>
    /// Electronic components attached to this station's pins (Servos, RFIDs, etc.)
    /// </summary>
    public ObservableCollection<ComponentInstance> Components { get; set; } = new();

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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
