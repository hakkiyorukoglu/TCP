using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCP.App.Models.Electronics;

namespace TCP.App.Converters;

public class RoutePathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable<TrackNode> nodes && nodes.Any())
        {
            var points = nodes.Select(n => new Point(n.X, n.Y)).ToList();
            if (points.Count == 0) return Geometry.Empty;

            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure { StartPoint = points[0] };

            if (points.Count > 1)
            {
                var polyLineSegment = new PolyLineSegment();
                for (int i = 1; i < points.Count; i++)
                {
                    polyLineSegment.Points.Add(points[i]);
                }
                pathFigure.Segments.Add(polyLineSegment);
            }

            pathGeometry.Figures.Add(pathFigure);
            return pathGeometry;
        }

        return Geometry.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
