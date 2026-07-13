using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Linq;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

/// <summary>
/// ComponentInstance - Represents an electronic component attached to a Station (Mega).
/// E.g. Servo, RFID, Sensor.
/// </summary>
public class ComponentInstance : ILayerItem, INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private bool _isLocked;
    private bool _isSelected;
    private bool _isVisible = true;

    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Parent station ID
    /// </summary>
    public Guid StationId { get; set; }

    [JsonIgnore]
    public string LayerName => Name;
    
    [JsonIgnore]
    public string LayerType => "Component";

    /// <summary>
    /// Template/Type of the component (e.g. "Servo", "RFID Reader")
    /// </summary>
    private string _templateId = string.Empty;
    public string TemplateId
    {
        get => _templateId;
        set
        {
            if (_templateId != value)
            {
                _templateId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLed));
                OnPropertyChanged(nameof(LedColor));
            }
        }
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LayerName));
                OnPropertyChanged(nameof(IsLed));
                OnPropertyChanged(nameof(LedColor));
            }
        }
    }
    
    /// <summary>
    /// Description of the component
    /// </summary>
    public string Description { get; set; } = "Sensör / Modül";

    [JsonIgnore]
    public bool IsLed
    {
        get
        {
            var lowerName = Name?.ToLowerInvariant() ?? "";
            var lowerTemplate = TemplateId?.ToLowerInvariant() ?? "";
            var lowerDesc = Description?.ToLowerInvariant() ?? "";
            return lowerName.Contains("led") || lowerTemplate.Contains("led") || lowerDesc.Contains("led");
        }
    }

    [JsonIgnore]
    public string LedColor
    {
        get
        {
            var lower = Name?.ToLowerInvariant() ?? "";
            if (lower.Contains("kırmızı")) return "#FF0000";
            if (lower.Contains("yeşil")) return "#00FF00";
            if (lower.Contains("sarı")) return "#FFFF00";
            if (lower.Contains("turuncu")) return "#FFA500";
            if (lower.Contains("mavi")) return "#0000FF";
            return "#FFFFFF";
        }
    }

    private string _pin = string.Empty;
    public string Pin
    {
        get => _pin;
        set
        {
            _pin = value;
            // Parse numerical part if possible, e.g. "D2" -> 2, "A0" -> 0, else random pin 1..16
            int parsed = 0;
            if (!string.IsNullOrEmpty(value))
            {
                var numStr = new string(value.Where(char.IsDigit).ToArray());
                if (int.TryParse(numStr, out int n)) parsed = n;
            }
            ConnectedPin = parsed > 0 && parsed <= 16 ? parsed : 1;
        }
    }

    private bool _isPowered;
    [JsonIgnore]
    public bool IsPowered
    {
        get => _isPowered;
        set
        {
            if (_isPowered != value)
            {
                _isPowered = value;
                OnPropertyChanged();
            }
        }
    }

    public int ConnectedPin { get; set; } = 1;

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
    
    /// <summary>
    /// Derived Type property for UI display
    /// </summary>
    [JsonIgnore]
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
