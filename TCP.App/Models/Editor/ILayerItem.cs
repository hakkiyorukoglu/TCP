using System;
using System.ComponentModel;

namespace TCP.App.Models.Editor;

/// <summary>
/// Unified interface for items that appear in the Editor's Layer Panel
/// </summary>
public interface ILayerItem : INotifyPropertyChanged
{
    /// <summary>
    /// Unique identifier for the layer item
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Display name shown in the layer panel
    /// </summary>
    string LayerName { get; }
    
    /// <summary>
    /// Type of the layer (e.g., "Image", "Electronic")
    /// </summary>
    string LayerType { get; }
    
    /// <summary>
    /// Is the item selected?
    /// </summary>
    bool IsSelected { get; set; }
    
    /// <summary>
    /// Is the item locked (preventing movement)?
    /// </summary>
    bool IsLocked { get; set; }
    
    /// <summary>
    /// Is the item visible on the canvas?
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// X coordinate on the canvas
    /// </summary>
    double X { get; set; }

    /// <summary>
    /// Y coordinate on the canvas
    /// </summary>
    double Y { get; set; }
}
