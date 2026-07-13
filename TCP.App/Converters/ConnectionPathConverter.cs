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
        if (values.Length < 6 || 
            values[0] == null || values[1] == null ||
            !(values[2] is double sx) || 
            !(values[3] is double sy) || 
            !(values[4] is double cx) || 
            !(values[5] is double cy))
        {
            return Geometry.Empty;
        }

        // Only draw if both are placed on the canvas
        if ((sx == 0 && sy == 0) || (cx == 0 && cy == 0))
        {
            return Geometry.Empty;
        }

        var sourceItem = values[0];
        var targetItem = values[1];

        double startX = sx + 100;
        double startY = sy + 40;
        double endX = cx + 100;
        double endY = cy + 40;

        // Try to adjust if source is a Station and target is a Component
        if (sourceItem.GetType().Name == "StationInstance" && targetItem.GetType().Name == "ComponentInstance")
        {
            // Box is 160x110. Pins are at the bottom.
            int pin = 1;
            var connectedPinProp = targetItem.GetType().GetProperty("ConnectedPin");
            if (connectedPinProp != null)
            {
                pin = (int)connectedPinProp.GetValue(targetItem);
                if (pin < 1) pin = 1;
                if (pin > 16) pin = 16;
            }
            
            // Width = 160. ItemsControl width is ~ 156.
            // Pin width ~ 9.75.
            double pinOffset = 4 + (pin - 0.5) * 9.75; // exact center of the pin
            
            startX = sx + pinOffset;
            startY = sy + 110; // Exactly at the bottom edge
            
            // Connect to top-center of the component box (160x110)
            endX = cx + 80;
            endY = cy; // Top edge
        }
        else if (sourceItem.GetType().Name == "ModemInstance" && targetItem.GetType().Name == "StationInstance")
        {
            // Modem to station: Modem bottom center to Station top center
            startX = sx + 80;
            startY = sy + 110;
            endX = cx + 80;
            endY = cy;
        }

        // Create an elastic curved line (Bezier)
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(startX, startY) };
        
        // Control points for a nice curve
        double diffY = Math.Abs(endY - startY);
        var cp1 = new Point(startX, startY + diffY / 2);
        var cp2 = new Point(endX, endY - diffY / 2);
        
        figure.Segments.Add(new BezierSegment(cp1, cp2, new Point(endX, endY), true));
        geometry.Figures.Add(figure);

        return geometry;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
