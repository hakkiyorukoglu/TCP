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
            OnPropertyChanged(nameof(RoutePathData));
            OnPropertyChanged(nameof(LeftRailPathData));
            OnPropertyChanged(nameof(RightRailPathData));
            OnPropertyChanged(nameof(DirectionPathData));
        }
    }

    public TrackRoute()
    {
        _nodes.CollectionChanged += Nodes_CollectionChanged;
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

    private void Nodes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        _cachedSegments = null;
        _cachedLength = -1;
        OnPropertyChanged(nameof(Nodes));
        OnPropertyChanged(nameof(RoutePathData));
        OnPropertyChanged(nameof(LeftRailPathData));
        OnPropertyChanged(nameof(RightRailPathData));
        OnPropertyChanged(nameof(DirectionPathData));
    }

    public void ForceUpdatePaths()
    {
        _cachedSegments = null;
        _cachedLength = -1;
        OnPropertyChanged(nameof(RoutePathData));
        OnPropertyChanged(nameof(LeftRailPathData));
        OnPropertyChanged(nameof(RightRailPathData));
        OnPropertyChanged(nameof(DirectionPathData));
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

                if (len1 < 1.0 || len2 < 1.0) continue; // Skip identical points

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

    private string GetOffsetPathData(double offset)
    {
        if (_nodes.Count == 0) return "";
        if (_nodes.Count == 1) return "";
        
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
        
        double defaultRadius = 40.0;
        var sb = new System.Text.StringBuilder();
        sb.Append($"M {offsetPoints[0].X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{offsetPoints[0].Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

        for (int i = 1; i < offsetPoints.Count - 1; i++)
        {
            var prev = offsetPoints[i - 1];
            var curr = offsetPoints[i];
            var next = offsetPoints[i + 1];

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

        var last = offsetPoints[offsetPoints.Count - 1];
        sb.Append($" L {last.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{last.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        
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

    [JsonIgnore]
    public string DirectionPathData
    {
        get
        {
            EnsureSegmentsCalculated();
            if (_cachedSegments == null || _cachedSegments.Count == 0 || _cachedLength <= 0) return "";
            
            var sb = new System.Text.StringBuilder();
            
            // Draw an arrow every 200 pixels
            double arrowSpacing = 200.0;
            double arrowSize = 12.0;
            
            // Eğer rota kısaysa en azından ortasına 1 tane ok çizelim
            double startOffset = (_cachedLength < arrowSpacing) ? (_cachedLength / 2) : (arrowSpacing / 2);
            
            for (double d = startOffset; d < _cachedLength; d += arrowSpacing)
            {
                var pt = GetPointAtDistance(d);
                var angle = GetAngleAtDistance(d);
                
                // Angle is in degrees. Convert to radians.
                double rad = angle * Math.PI / 180.0;
                
                // Direction vector
                var dir = new System.Windows.Vector(Math.Cos(rad), Math.Sin(rad));
                
                // Normal vector
                var normal = new System.Windows.Vector(-dir.Y, dir.X);
                
                // Arrow tip is at pt + dir * (arrowSize/2)
                var tip = new System.Windows.Point(pt.X + dir.X * (arrowSize / 2), pt.Y + dir.Y * (arrowSize / 2));
                
                // Arrow wings
                var back = new System.Windows.Point(pt.X - dir.X * (arrowSize / 2), pt.Y - dir.Y * (arrowSize / 2));
                var leftWing = back + normal * (arrowSize / 1.5);
                var rightWing = back - normal * (arrowSize / 1.5);
                
                sb.Append($"M {tip.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{tip.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
                sb.Append($"L {leftWing.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{leftWing.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
                sb.Append($"M {tip.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{tip.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
                sb.Append($"L {rightWing.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{rightWing.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
            }
            
            return sb.ToString();
        }
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
