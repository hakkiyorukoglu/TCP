using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TCP.App.Converters;

public class ConnectionPathConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 4 || 
            !(values[0] is double sx) || 
            !(values[1] is double sy) || 
            !(values[2] is double cx) || 
            !(values[3] is double cy))
        {
            return Geometry.Empty;
        }

        // Only draw if both are placed on the canvas
        if ((sx == 0 && sy == 0) || (cx == 0 && cy == 0))
        {
            return Geometry.Empty;
        }

        // Add offset to point to the center of the boxes (approx 80px width, 30px height offset)
        double startX = sx + 100;
        double startY = sy + 40;
        double endX = cx + 100;
        double endY = cy + 40;

        // Create an elastic curved line (Bezier)
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(startX, startY) };
        
        // Control points for a nice curve
        double diffX = Math.Abs(endX - startX);
        var cp1 = new Point(startX + diffX / 2, startY);
        var cp2 = new Point(endX - diffX / 2, endY);
        
        figure.Segments.Add(new BezierSegment(cp1, cp2, new Point(endX, endY), true));
        geometry.Figures.Add(figure);

        return geometry;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
