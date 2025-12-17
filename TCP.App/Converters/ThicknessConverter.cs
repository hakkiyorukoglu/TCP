using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCP.App;

/// <summary>
/// ThicknessConverter - Spacing token'larını Thickness'e çevirir
/// 
/// TCP-0.7.0: Theme Tokens v1 (Design Tokens)
/// 
/// Spacing token'larını Margin/Padding için kullanmak için converter.
/// </summary>
public class ThicknessConverter : IValueConverter
{
    public static readonly ThicknessConverter Instance = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double spacing)
        {
            // Parameter format: "left,top,right,bottom" veya tek değer (tüm kenarlar için)
            if (parameter is string paramStr)
            {
                var parts = paramStr.Split(',');
                if (parts.Length == 4)
                {
                    // left,top,right,bottom
                    var left = parts[0].Trim() == "spacing" ? spacing : double.Parse(parts[0]);
                    var top = parts[1].Trim() == "spacing" ? spacing : double.Parse(parts[1]);
                    var right = parts[2].Trim() == "spacing" ? spacing : double.Parse(parts[2]);
                    var bottom = parts[3].Trim() == "spacing" ? spacing : double.Parse(parts[3]);
                    return new Thickness(left, top, right, bottom);
                }
                else if (parts.Length == 2)
                {
                    // horizontal,vertical
                    var horizontal = parts[0].Trim() == "spacing" ? spacing : double.Parse(parts[0]);
                    var vertical = parts[1].Trim() == "spacing" ? spacing : double.Parse(parts[1]);
                    return new Thickness(horizontal, vertical, horizontal, vertical);
                }
            }
            // Tek değer - tüm kenarlar için
            return new Thickness(spacing);
        }
        return new Thickness(0);
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// CornerRadiusConverter - Radius token'larını CornerRadius struct'ına çevirir
/// 
/// TCP-0.7.0: Theme Tokens v1 (Design Tokens)
/// </summary>
public class CornerRadiusConverter : IValueConverter
{
    public static readonly CornerRadiusConverter Instance = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double radius)
        {
            return new CornerRadius(radius);
        }
        return new CornerRadius(0);
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
