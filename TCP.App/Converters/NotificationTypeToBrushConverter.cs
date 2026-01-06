using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCP.App.Models;

namespace TCP.App.Converters;

/// <summary>
/// NotificationTypeToBrushConverter - Notification type'dan brush'a converter
/// 
/// TCP-0.9.2: Notifications / Toasts v1
/// 
/// NotificationType enum'ını uygun brush'a çevirir.
/// </summary>
public class NotificationTypeToBrushConverter : IValueConverter
{
    /// <summary>
    /// Convert - NotificationType'dan Brush'a
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => System.Windows.Application.Current.TryFindResource("Brush.Accent.Success") as SolidColorBrush ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80)),
                NotificationType.Warning => System.Windows.Application.Current.TryFindResource("Brush.Accent.Warning") as SolidColorBrush ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 152, 0)),
                NotificationType.Error => System.Windows.Application.Current.TryFindResource("Brush.Accent.Error") as SolidColorBrush ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 67, 54)),
                NotificationType.Info => System.Windows.Application.Current.TryFindResource("Brush.Accent.Primary") as SolidColorBrush ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 150, 243)),
                _ => System.Windows.Application.Current.TryFindResource("Brush.Surface") as SolidColorBrush ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 51, 51))
            };
        }
        
        // Fallback
        return System.Windows.Application.Current.TryFindResource("Brush.Surface") as SolidColorBrush ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 51, 51));
    }
    
    /// <summary>
    /// ConvertBack - Not implemented (OneWay binding)
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("NotificationTypeToBrushConverter is OneWay only");
    }
}

/// <summary>
/// NotificationTypeToIconConverter - Notification type'dan icon'a converter
/// 
/// TCP-0.9.2: Notifications / Toasts v1
/// 
/// NotificationType enum'ını uygun icon karakterine çevirir.
/// </summary>
public class NotificationTypeToIconConverter : IValueConverter
{
    /// <summary>
    /// Convert - NotificationType'dan Icon string'e
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "✓",
                NotificationType.Warning => "⚠",
                NotificationType.Error => "✕",
                NotificationType.Info => "ℹ",
                _ => "ℹ"
            };
        }
        
        // Fallback
        return "ℹ";
    }
    
    /// <summary>
    /// ConvertBack - Not implemented (OneWay binding)
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("NotificationTypeToIconConverter is OneWay only");
    }
}
