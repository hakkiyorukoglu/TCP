using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TCP.App.Models.Editor;

/// <summary>
/// PlacedBoardBox - Placed board box on editor canvas
/// 
/// TCP-1.0.3: Editor: Add board boxes from registry
/// 
/// This model represents a board box that has been placed on the editor canvas.
/// 
/// Single Responsibility: Placed box data model
/// </summary>
public class PlacedBoardBox : ILayerItem
{
    private double _x;
    private double _y;
    private bool _isSelected;
    private bool _isLocked;
    private bool _isVisible = true;

    /// <summary>
    /// Box ID (immutable, unique identifier)
    /// </summary>
    public Guid BoxId { get; init; } = Guid.NewGuid();
    
    [JsonIgnore]
    public Guid Id => BoxId;
    
    [JsonIgnore]
    public string LayerName => DisplayName;
    
    [JsonIgnore]
    public string LayerType => "Electronic";
    
    /// <summary>
    /// Board ID (references BoardDefinition.Id)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string BoardId { get; init; } = string.Empty;
    
    /// <summary>
    /// Display name (copy for easy rendering)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;
    
    /// <summary>
    /// Board type
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Type { get; init; } = string.Empty;
    
    /// <summary>
    /// Board status
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Status { get; init; } = "Offline";
    
    /// <summary>
    /// X coordinate (world coords)
    /// </summary>
    public double X
    {
        get => _x;
        set { if (_x != value) { _x = value; OnPropertyChanged(); } }
    }
    
    /// <summary>
    /// Y coordinate (world coords)
    /// </summary>
    public double Y
    {
        get => _y;
        set { if (_y != value) { _y = value; OnPropertyChanged(); } }
    }

    [JsonIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
    }

    public bool IsLocked
    {
        get => _isLocked;
        set { if (_isLocked != value) { _isLocked = value; OnPropertyChanged(); } }
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

