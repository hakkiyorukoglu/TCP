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

    public double X { get; set; }
    public double Y { get; set; }

    public string Name
    {
        get => _name;
        set { if (_name != value) { _name = value; OnPropertyChanged(); } }
    }

    private ObservableCollection<TrackNode> _nodes = new();
    public ObservableCollection<TrackNode> Nodes
    {
        get => _nodes;
        set
        {
            if (_nodes != null)
                _nodes.CollectionChanged -= Nodes_CollectionChanged;
            
            _nodes = value ?? new ObservableCollection<TrackNode>();
            
            if (_nodes != null)
                _nodes.CollectionChanged += Nodes_CollectionChanged;
                
            OnPropertyChanged();
        }
    }

    public TrackRoute()
    {
        _nodes.CollectionChanged += Nodes_CollectionChanged;
    }

    private void Nodes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Nodes));
        OnPropertyChanged(nameof(RoutePathData));
        OnPropertyChanged(nameof(LeftRailPathData));
        OnPropertyChanged(nameof(RightRailPathData));
    }

    private bool _isCenterLineVisible = true;
    public bool IsCenterLineVisible
    {
        get => _isCenterLineVisible;
        set { if (_isCenterLineVisible != value) { _isCenterLineVisible = value; OnPropertyChanged(); } }
    }

    [JsonIgnore]
    public string RoutePathData
    {
        get
        {
            if (_nodes.Count == 0) return "";
            var sb = new System.Text.StringBuilder();
            sb.Append($"M {_nodes[0].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{_nodes[0].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            for (int i = 1; i < _nodes.Count; i++)
            {
                sb.Append($" L {_nodes[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{_nodes[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
            return sb.ToString();
        }
    }

    private string GetOffsetPathData(double offset)
    {
        if (_nodes.Count < 2) return "";
        
        var points = _nodes.Select(n => new System.Windows.Point(n.X, n.Y)).ToList();
        var offsetPoints = new System.Collections.Generic.List<System.Windows.Point>();
        
        for (int i = 0; i < points.Count; i++)
        {
            System.Windows.Vector dir1 = new System.Windows.Vector(0,0);
            System.Windows.Vector dir2 = new System.Windows.Vector(0,0);
            
            if (i > 0)
            {
                var d = points[i] - points[i-1];
                d.Normalize();
                dir1 = d;
            }
            if (i < points.Count - 1)
            {
                var d = points[i+1] - points[i];
                d.Normalize();
                dir2 = d;
            }
            
            System.Windows.Vector tangent;
            if (i == 0) tangent = dir2;
            else if (i == points.Count - 1) tangent = dir1;
            else
            {
                tangent = dir1 + dir2;
                tangent.Normalize();
            }
            
            var normal = new System.Windows.Vector(-tangent.Y, tangent.X);
            double miter = 1.0;
            
            if (i > 0 && i < points.Count - 1)
            {
                var n1 = new System.Windows.Vector(-dir1.Y, dir1.X);
                double dot = normal.X * n1.X + normal.Y * n1.Y;
                if (Math.Abs(dot) > 0.1) miter = 1.0 / dot;
                
                if (miter > 3.0) miter = 3.0;
                if (miter < -3.0) miter = -3.0;
            }
            
            var offsetPoint = points[i] + normal * (offset * miter);
            offsetPoints.Add(offsetPoint);
        }
        
        var sb = new System.Text.StringBuilder();
        sb.Append($"M {offsetPoints[0].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{offsetPoints[0].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        for (int i = 1; i < offsetPoints.Count; i++)
        {
            sb.Append($" L {offsetPoints[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{offsetPoints[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        }
        return sb.ToString();
    }

    [JsonIgnore]
    public string LeftRailPathData => GetOffsetPathData(-6);

    [JsonIgnore]
    public string RightRailPathData => GetOffsetPathData(6);

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
