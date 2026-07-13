using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// MainPcInstance - Represents the main PC in the network topology (Root of the daisy-chain).
/// </summary>
public class MainPcInstance : ILayerItem, INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private bool _isLocked;
    private bool _isSelected;
    private bool _isVisible = true;

    // Fixed ID for the Main PC
    public Guid Id { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [JsonIgnore]
    public string LayerName => Name;
    
    [JsonIgnore]
    public string LayerType => "MainPC";
    
    [JsonIgnore]
    public string Type => "Ana PC (Sunucu)";

    public string Name { get; set; } = "Ana PC";
    public string IpAddress { get; set; } = "192.168.1.100";

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
