using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

public class TrackRoute : ILayerItem
{
    private string _name = "Yeni Rota";
    private bool _isSelected;
    private bool _isLocked;
    private bool _isVisible = true;

    public Guid Id { get; set; } = Guid.NewGuid();
    
    [JsonIgnore]
    public string LayerName => Name;
    
    [JsonIgnore]
    public string LayerType => "Route";

    public string Name
    {
        get => _name;
        set { if (_name != value) { _name = value; OnPropertyChanged(); } }
    }

    // Rotayı oluşturan noktalar (Spline / Line segmentleri için)
    public ObservableCollection<TrackNode> Nodes { get; set; } = new();

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
