using System;
using System.Collections.Generic;

namespace TCP.App.Models.Editor;

/// <summary>
/// Represents the serialized state of the Editor page.
/// </summary>
public class EditorLayoutState
{
    public List<EditorImage> Images { get; set; } = new();
    public List<PlacedItemState> PlacedItems { get; set; } = new();
}

/// <summary>
/// Represents a station or component that has been placed on the editor canvas.
/// </summary>
public class PlacedItemState
{
    public Guid ItemId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool IsLocked { get; set; }
}
