using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// ModemInstance - Represents a physical TP-Link Switch (Modem/Distributor) on the network.
/// </summary>
public class ModemInstance : ILayerItem, INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private bool _isLocked;
    private bool _isSelected;
    private bool _isVisible = true;
    private NetworkStatus _status = NetworkStatus.Offline;

    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public string LayerName => Name;
    
    [JsonIgnore]
    public string LayerType => "Modem";
    
    [JsonIgnore]
    public string Type => "TP-Link LS1005G (Modem)";

    public string Name { get; set; } = "Yeni Modem";
    public string IpAddress { get; set; } = "192.168.1.1"; // Modems might not have IPs natively if unmanaged, but user asked for it.
    public string MacAddress { get; set; } = "00:00:00:00:00:00";

    /// <summary>
    /// Stations (Masalar) attached to this Modem (Max 3)
    /// </summary>
    public ObservableCollection<StationInstance> Stations { get; set; } = new();

    public Guid? IncomingConnectionId { get; set; }
    public Guid? OutgoingConnectionId { get; set; }

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

    [JsonIgnore]
    public NetworkStatus Status
    {
        get => _status;
        set { if (_status != value) { _status = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
