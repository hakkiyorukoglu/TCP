using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// DeviceInstance - Represents a user-created electronic card instance.
/// 
/// Contains both device configuration properties (Name, IP, Port, etc.)
/// and Editor map properties (X, Y, IsLocked).
/// </summary>
public class DeviceInstance : ILayerItem
{
    private double _x;
    private double _y;
    private bool _isLocked;
    private bool _isSelected;
    private bool _isVisible = true;

    /// <summary>
    /// Unique identifier for this device instance.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public string LayerName => CustomName;
    
    [JsonIgnore]
    public string LayerType => "Electronic";

    /// <summary>
    /// The template/board name it was created from (e.g. "Arduino Mega").
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Custom name given by the user (e.g. "Main Controller").
    /// </summary>
    public string CustomName { get; set; } = string.Empty;

    /// <summary>
    /// IP Address (e.g. "192.168.1.10").
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Port Number (e.g. 8080).
    /// </summary>
    public int Port { get; set; } = 80;

    /// <summary>
    /// MAC Address (e.g. "00:1B:44:11:3A:B7").
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;

    /// <summary>
    /// Physical LAN cable or port identifier (e.g. "ETH-1").
    /// </summary>
    public string LanCable { get; set; } = string.Empty;

    /// <summary>
    /// Physical location of the device (e.g. "Server Room 1").
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// X coordinate on the Editor canvas.
    /// </summary>
    public double X
    {
        get => _x;
        set { if (_x != value) { _x = value; OnPropertyChanged(); } }
    }

    /// <summary>
    /// Y coordinate on the Editor canvas.
    /// </summary>
    public double Y
    {
        get => _y;
        set { if (_y != value) { _y = value; OnPropertyChanged(); } }
    }

    /// <summary>
    /// Whether the device is locked in place on the Editor canvas.
    /// </summary>
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
    /// Type property (derived from TemplateId for UI display like "Mega", "Nano", etc.)
    /// </summary>
    public string Type 
    {
        get
        {
            if (string.IsNullOrWhiteSpace(TemplateId)) return "Unknown";
            var t = TemplateId;
            if (t.StartsWith("Arduino ", StringComparison.OrdinalIgnoreCase)) t = t.Substring("Arduino ".Length);
            else if (t.EndsWith(" Reader", StringComparison.OrdinalIgnoreCase)) t = t.Substring(0, t.Length - " Reader".Length);
            else if (t.EndsWith(" Controller", StringComparison.OrdinalIgnoreCase)) t = t.Substring(0, t.Length - " Controller".Length);
            return t;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
