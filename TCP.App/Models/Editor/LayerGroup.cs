using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TCP.App.Models.Editor;

/// <summary>
/// Represents a hierarchical group of layers (e.g. "Backgrounds" or "Table 1")
/// </summary>
public class LayerGroup : ILayerItem
{
    private string _layerName;
    private bool _isSelected;
    private bool _isLocked;
    private bool _isVisible = true;

    public Guid Id { get; } = Guid.NewGuid();

    public string LayerType => "Group";

    public string LayerName
    {
        get => _layerName;
        set { if (_layerName != value) { _layerName = value; OnPropertyChanged(); } }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
    }

    public bool IsLocked
    {
        get => _isLocked;
        set 
        { 
            if (_isLocked != value) 
            { 
                _isLocked = value; 
                OnPropertyChanged();
                
                // Cascade down
                foreach(var child in Children)
                {
                    child.IsLocked = value;
                }
            } 
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set 
        { 
            if (_isVisible != value) 
            { 
                _isVisible = value; 
                OnPropertyChanged();
                
                // Cascade down
                foreach(var child in Children)
                {
                    child.IsVisible = value;
                }
            } 
        }
    }

    public double X { get; set; }
    public double Y { get; set; }

    /// <summary>
    /// Child layers in this group
    /// </summary>
    public ObservableCollection<ILayerItem> Children { get; } = new();

    public LayerGroup(string name)
    {
        _layerName = name;
        Children.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Children));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
