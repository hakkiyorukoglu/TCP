using System;
using System.Collections.Generic;

namespace TCP.App.Models.Editor;

/// <summary>
/// Represents the serialized state of the Editor page.
/// Includes background images and the placed devices' coordinates.
/// </summary>
public class EditorLayoutState
{
    public List<EditorImage> Images { get; set; } = new();
    public List<PlacedDeviceState> PlacedDevices { get; set; } = new();
}

/// <summary>
/// Represents a device that has been placed on the editor canvas.
/// We only save the properties relevant to the canvas to map back to the global device.
/// </summary>
public class PlacedDeviceState
{
    public Guid DeviceId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool IsLocked { get; set; }
}
