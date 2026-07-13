using System;

namespace TCP.App.Models.Electronics;

/// <summary>
/// DeviceInstance - Represents a user-created electronic card instance.
/// 
/// Contains both device configuration properties (Name, IP, Port, etc.)
/// and Editor map properties (X, Y, IsLocked).
/// </summary>
public class DeviceInstance
{
    /// <summary>
    /// Unique identifier for this device instance.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

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
    public double X { get; set; }

    /// <summary>
    /// Y coordinate on the Editor canvas.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Whether the device is locked in place on the Editor canvas.
    /// </summary>
    public bool IsLocked { get; set; }

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
}
