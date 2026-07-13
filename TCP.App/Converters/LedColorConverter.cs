using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCP.App.Models.Electronics;

namespace TCP.App.Converters;

public class LedColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StationInstance)
        {
            return new SolidColorBrush(Colors.LimeGreen); // Mega -> Green
        }
        else if (value is ComponentInstance comp)
        {
            var template = comp.TemplateId?.ToLowerInvariant() ?? "";
            if (template.Contains("servo"))
                return new SolidColorBrush(Colors.Red); // Servo -> Red
            else if (template.Contains("rfid"))
                return new SolidColorBrush(Colors.DodgerBlue); // RFID -> Blue
            else
                return new SolidColorBrush(Colors.Orange); // Default
        }
        
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
