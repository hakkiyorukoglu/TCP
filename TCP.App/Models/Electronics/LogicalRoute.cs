using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TCP.App.Models.Editor;

namespace TCP.App.Models.Electronics;

public class LogicalRoute : ILayerItem
{
    private string _name = "Yeni Rota Yönü";
    private bool _isSelected;
    private bool _isLocked;
    private bool _isVisible = true;

    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid ParentTrackId { get; set; }
    public bool IsReversed { get; set; }
    public Guid StartNodeId { get; set; }
    public Guid EndNodeId { get; set; }

    [JsonIgnore]
    public string LayerName => Name;
    
    [JsonIgnore]
    public string LayerType => "LogicalRoute";

    public double X { get; set; }
    public double Y { get; set; }

    public string Name
    {
        get => _name;
        set { if (_name != value) { _name = value; OnPropertyChanged(); } }
    }

    private ObservableCollection<TrackNode> _nodes = new();
    
    [JsonIgnore]
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
            ForceUpdatePaths();
        }
    }

    public LogicalRoute()
    {
        _nodes.CollectionChanged += Nodes_CollectionChanged;
    }

    private void Node_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TrackNode.X) || e.PropertyName == nameof(TrackNode.Y))
        {
            ForceUpdatePaths();
        }
    }

    private void Nodes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (TrackNode node in e.OldItems)
            {
                node.PropertyChanged -= Node_PropertyChanged;
            }
        }
        if (e.NewItems != null)
        {
            foreach (TrackNode node in e.NewItems)
            {
                node.PropertyChanged += Node_PropertyChanged;
            }
        }
        ForceUpdatePaths();
    }
    
    private List<PathSegment>? _cachedSegments;
    private double _cachedLength = -1;
    private const double DEFAULT_RADIUS = 40.0;

    private void EnsureSegmentsCalculated()
    {
        if (_cachedSegments == null)
        {
            _cachedSegments = PathMath.GenerateSegments(Nodes, DEFAULT_RADIUS);
            _cachedLength = _cachedSegments.Sum(s => s.Length);
        }
    }

    public double GetLength()
    {
        EnsureSegmentsCalculated();
        return _cachedLength;
    }

    public (double X, double Y) GetPointAtDistance(double distance)
    {
        EnsureSegmentsCalculated();
        if (_cachedSegments == null || _cachedSegments.Count == 0) return (0, 0);
        
        if (distance <= 0) return (_cachedSegments[0].Start.X, _cachedSegments[0].Start.Y);
        if (distance >= _cachedLength) return (_cachedSegments[^1].End.X, _cachedSegments[^1].End.Y);

        var seg = _cachedSegments.LastOrDefault(s => s.StartDistance <= distance) ?? _cachedSegments[0];
        double t = (distance - seg.StartDistance) / seg.Length;
        t = Math.Clamp(t, 0.0, 1.0);
        var pt = seg.GetPoint(t);
        return (pt.X, pt.Y);
    }

    public double GetAngleAtDistance(double distance)
    {
        EnsureSegmentsCalculated();
        if (_cachedSegments == null || _cachedSegments.Count == 0) return 0;
        
        distance = Math.Clamp(distance, 0, _cachedLength);
        var seg = _cachedSegments.LastOrDefault(s => s.StartDistance <= distance) ?? _cachedSegments[0];
        
        double t = (distance - seg.StartDistance) / seg.Length;
        t = Math.Clamp(t, 0.0, 1.0);
        var tangent = seg.GetTangent(t);
        
        return Math.Atan2(tangent.Y, tangent.X) * 180.0 / Math.PI;
    }

    public void ForceUpdatePaths()
    {
        _cachedSegments = null;
        _cachedLength = -1;
        OnPropertyChanged(nameof(Nodes));
        OnPropertyChanged(nameof(RoutePathData));
        OnPropertyChanged(nameof(DirectionPathData));
    }

    [JsonIgnore]
    public string RoutePathData
    {
        get
        {
            if (_nodes.Count == 0) return "";
            if (_nodes.Count == 1) return $"M {_nodes[0].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{_nodes[0].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            
            double defaultRadius = 40.0;
            var sb = new System.Text.StringBuilder();
            
            var p0 = new System.Windows.Point(_nodes[0].X, _nodes[0].Y);
            sb.Append($"M {p0.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{p0.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            for (int i = 1; i < _nodes.Count - 1; i++)
            {
                var prev = new System.Windows.Point(_nodes[i - 1].X, _nodes[i - 1].Y);
                var curr = new System.Windows.Point(_nodes[i].X, _nodes[i].Y);
                var next = new System.Windows.Point(_nodes[i + 1].X, _nodes[i + 1].Y);

                System.Windows.Vector v1 = prev - curr;
                System.Windows.Vector v2 = next - curr;
                
                double len1 = v1.Length;
                double len2 = v2.Length;

                if (len1 < 1.0 || len2 < 1.0) continue;

                double r = Math.Min(defaultRadius, Math.Min(len1 / 2.1, len2 / 2.1));

                v1.Normalize();
                v2.Normalize();

                var pStart = curr + v1 * r;
                var pEnd = curr + v2 * r;

                sb.Append($" L {pStart.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{pStart.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
                sb.Append($" Q {curr.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{curr.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} {pEnd.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{pEnd.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }

            var last = _nodes[_nodes.Count - 1];
            sb.Append($" L {last.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{last.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            return sb.ToString();
        }
    }

    [JsonIgnore]
    public string DirectionPathData
    {
        get
        {
            EnsureSegmentsCalculated();
            if (_cachedSegments == null || _cachedSegments.Count == 0 || _cachedLength <= 0) return "";
            
            var sb = new System.Text.StringBuilder();
            
            double arrowSpacing = 200.0;
                double arrowSize = 20.0;
                
                double startOffset = (_cachedLength < arrowSpacing) ? (_cachedLength / 2) : (arrowSpacing / 2);
                
                for (double d = startOffset; d < _cachedLength; d += arrowSpacing)
                {
                    var pt = GetPointAtDistance(d);
                    var angle = GetAngleAtDistance(d);
                    
                    double rad = angle * Math.PI / 180.0;
                    var dir = new System.Windows.Vector(Math.Cos(rad), Math.Sin(rad));
                    var normal = new System.Windows.Vector(-dir.Y, dir.X);
                    
                    var tip = new System.Windows.Point(pt.X + dir.X * (arrowSize / 2), pt.Y + dir.Y * (arrowSize / 2));
                    
                    var back = new System.Windows.Point(pt.X - dir.X * (arrowSize / 2), pt.Y - dir.Y * (arrowSize / 2));
                    var leftWing = back + normal * (arrowSize / 1.5);
                    var rightWing = back - normal * (arrowSize / 1.5);
                    
                    sb.Append($"M {tip.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{tip.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
                    sb.Append($"L {leftWing.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{leftWing.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
                    sb.Append($"L {rightWing.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{rightWing.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} Z ");
                }
                
                return sb.ToString();
        }
    }

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
