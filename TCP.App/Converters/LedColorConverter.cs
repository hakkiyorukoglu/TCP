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
        if (value is MainPcInstance)
        {
            return new SolidColorBrush(Colors.LimeGreen);
        }
        else if (value is ModemInstance modem)
        {
            return GetColor(modem.Status);
        }
        else if (value is StationInstance station)
        {
            return GetColor(station.Status);
        }
        else if (value is ComponentInstance comp)
        {
            if (comp.IsPowered)
            {
                return new SolidColorBrush(Colors.LimeGreen); // Powered up -> Green
            }

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

    private Brush GetColor(NetworkStatus status)
    {
        return status switch
        {
            NetworkStatus.Online => new SolidColorBrush(Colors.LimeGreen),
            NetworkStatus.Offline => new SolidColorBrush(Colors.Red),
            NetworkStatus.Unreachable => new SolidColorBrush(Colors.Gray),
            NetworkStatus.Error => new SolidColorBrush(Colors.Orange),
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
