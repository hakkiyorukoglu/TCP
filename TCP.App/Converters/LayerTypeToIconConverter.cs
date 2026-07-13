using System;
using System.Globalization;
using System.Windows.Data;

namespace TCP.App.Converters;

public class LayerTypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string type)
        {
            if (type == "Image") return "🖼";
            if (type == "Electronic") return "🔌";
        }
        return "📄";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
