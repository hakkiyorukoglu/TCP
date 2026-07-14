using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// Kör Otonom Tren (Blind Autonomous Train) - TCP.App
/// Nano tabanlı, dışarıdan sadece start hedefi alıp kendi RFID/Laser sensörleriyle ilerleyen tren nesnesi.
/// </summary>
public class TrainInstance : ILayerItem, INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private bool _isLocked;
    private bool _isSelected;
    private bool _isVisible = true;

    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public string LayerName => Name;

    [JsonIgnore]
    public string LayerType => "Train";

    [JsonIgnore]
    public string Type => "Otonom Tren (Arduino Nano)";

    public string Name { get; set; } = "Tren 1";
    
    // RFID tag that identifies this train (e.g. read by track-side readers)
    public string RfidTag { get; set; } = "TRAIN_01";

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
    public double ZIndex { get; set; } = 300; // Trenler en üstte görünsün

    // Fizik ve Simülasyon parametreleri (Veritabanına kaydedilmesine gerek yok)
    [JsonIgnore]
    public double CurrentSpeed { get; set; } = 0;
    
    [JsonIgnore]
    public double TargetSpeed { get; set; } = 0;
    
    [JsonIgnore]
    public double Acceleration { get; set; } = 50.0; // Birim/saniye kare
    
    [JsonIgnore]
    public Guid? CurrentRouteId { get; set; } // Hangi rotanın üzerinde
    
    [JsonIgnore]
    public int CurrentRouteSegmentIndex { get; set; } // Rotanın hangi spline parçası üzerinde
    
    [JsonIgnore]
    public double CurrentSegmentT { get; set; } // Spline üzerindeki t değeri (0.0 - 1.0)
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
