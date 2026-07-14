using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Models.Editor;
using System.Text.Json.Serialization;

namespace TCP.App.Models.Electronics;

public class TrackNode : ILayerItem
{
    private double _x;
    private double _y;
    private bool _isSelected;
    private bool _isLocked;
    private bool _isVisible = true;

    public Guid Id { get; set; } = Guid.NewGuid();
    
    [JsonIgnore]
    public string LayerName => "Track Node";
    
    [JsonIgnore]
    public string LayerType => "Node";

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
    
    private Guid? _boundComponentId;
    public Guid? BoundComponentId
    {
        get => _boundComponentId;
        set { if (_boundComponentId != value) { _boundComponentId = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
