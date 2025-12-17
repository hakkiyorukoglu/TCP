using System.Windows;
using System.Windows.Input;
using TCP.App.Models;
using TCP.App.Services;

namespace TCP.App.Views.Components;

/// <summary>
/// ToastNotification.xaml.cs - Toast notification component code-behind
/// 
/// TCP-0.9.2: Notifications / Toasts v1
/// 
/// Minimal code-behind for toast interaction.
/// </summary>
public partial class ToastNotification : System.Windows.Controls.UserControl
{
    /// <summary>
    /// Notification dependency property
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public static readonly DependencyProperty NotificationProperty =
        DependencyProperty.Register(
            nameof(Notification),
            typeof(NotificationMessage),
            typeof(ToastNotification),
            new PropertyMetadata(null));
    
    /// <summary>
    /// Notification property
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public NotificationMessage Notification
    {
        get => (NotificationMessage)GetValue(NotificationProperty);
        set => SetValue(NotificationProperty, value);
    }
    
    /// <summary>
    /// Constructor
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public ToastNotification()
    {
        InitializeComponent();
        DataContext = this;
    }
    
    /// <summary>
    /// Toast click handler - dismiss notification
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private void Toast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (Notification != null)
        {
            NotificationService.Instance.Dismiss(Notification);
        }
    }
    
    /// <summary>
    /// Close button click handler
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true; // Prevent toast click
        if (Notification != null)
        {
            NotificationService.Instance.Dismiss(Notification);
        }
    }
}
