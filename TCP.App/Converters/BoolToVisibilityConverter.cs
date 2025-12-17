using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCP.App;

/// <summary>
/// BoolToVisibilityConverter - Boolean to Visibility converter
/// TCP-0.5.1: Top-Right Search UI
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public static readonly BoolToVisibilityConverter Instance = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}

/// <summary>
/// IsNullOrEmptyConverter - String null/empty check converter
/// </summary>
public class IsNullOrEmptyConverter : IValueConverter
{
    public static readonly IsNullOrEmptyConverter Instance = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        return true;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
