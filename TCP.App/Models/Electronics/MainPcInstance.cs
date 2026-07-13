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

    private bool _isPowered;
    [JsonIgnore]
    public bool IsPowered
    {
        get => _isPowered;
        set { if (_isPowered != value) { _isPowered = value; OnPropertyChanged(); } }
    }

    private string _terminalOutput = string.Empty;
    [JsonIgnore]
    public System.Collections.ObjectModel.ObservableCollection<string> ConsoleLogs { get; } = new();

    private string _script = string.Empty;
    public string Script
    {
        get => _script;
        set { if (_script != value) { _script = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Log(string message)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            ConsoleLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - {message}");
            if (ConsoleLogs.Count > 5) ConsoleLogs.RemoveAt(ConsoleLogs.Count - 1);
        });
    }

    public void ClearTerminal()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            ConsoleLogs.Clear();
        });
    }
}
