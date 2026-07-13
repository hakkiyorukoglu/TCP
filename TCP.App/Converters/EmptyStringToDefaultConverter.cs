using System;
using System.Globalization;
using System.Windows.Data;

namespace TCP.App.Converters;

public class EmptyStringToDefaultConverter : IValueConverter
{
    public string DefaultValue { get; set; } = "Atanmamış";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return DefaultValue;

        return value.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
