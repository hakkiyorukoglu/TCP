using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TCP.App.Models;

namespace TCP.App.Services;

/// <summary>
/// NotificationService - Toast/notification yönetim servisi
/// 
/// TCP-0.9.2: Notifications / Toasts v1
/// 
/// Bu servis global toast notification'ları yönetir.
/// Singleton pattern kullanır.
/// 
/// Single Responsibility: Notification state ve lifecycle yönetimi
/// </summary>
public class NotificationService
{
    private static NotificationService? _instance;
    
    /// <summary>
    /// Singleton instance
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public static NotificationService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NotificationService();
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Aktif notification'lar listesi
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public ObservableCollection<NotificationMessage> ActiveNotifications { get; }
    
    /// <summary>
    /// Maksimum görünür notification sayısı
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private const int MaxVisibleNotifications = 3;
    
    /// <summary>
    /// Auto-dismiss süresi (milisaniye)
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private const int AutoDismissMilliseconds = 4000;
    
    /// <summary>
    /// Dispatcher timer için dispatcher
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private readonly DispatcherTimer _dismissTimer;
    
    /// <summary>
    /// Constructor - Private (singleton)
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private NotificationService()
    {
        ActiveNotifications = new ObservableCollection<NotificationMessage>();
        
        // TCP-0.9.2: Auto-dismiss timer
        _dismissTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100) // Check every 100ms
        };
        _dismissTimer.Tick += DismissTimer_Tick;
        _dismissTimer.Start();
    }
    
    /// <summary>
    /// Success notification göster
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public void ShowSuccess(string title, string message)
    {
        ShowNotification(title, message, NotificationType.Success);
    }
    
    /// <summary>
    /// Warning notification göster
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public void ShowWarning(string title, string message)
    {
        ShowNotification(title, message, NotificationType.Warning);
    }
    
    /// <summary>
    /// Error notification göster
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public void ShowError(string title, string message)
    {
        ShowNotification(title, message, NotificationType.Error);
    }
    
    /// <summary>
    /// Info notification göster
    /// TCP-1.0.1: Home Map Canvas (Empty) - Info toast support
    /// </summary>
    public void ShowInfo(string title, string message)
    {
        ShowNotification(title, message, NotificationType.Info);
    }
    
    /// <summary>
    /// Notification göster (internal)
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    private void ShowNotification(string title, string message, NotificationType type)
    {
        // UI thread'de çalıştığından emin ol
        Application.Current.Dispatcher.Invoke(() =>
        {
            var notification = new NotificationMessage
            {
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            };
            
            // Maksimum limit kontrolü - en eski notification'ı kaldır
            if (ActiveNotifications.Count >= MaxVisibleNotifications)
            {
                var oldest = ActiveNotifications.OrderBy(n => n.Timestamp).FirstOrDefault();
                if (oldest != null)
                {
                    ActiveNotifications.Remove(oldest);
                }
            }
            
            ActiveNotifications.Add(notification);
        });
    }
    
    /// <summary>
    /// Dismiss timer tick handler
    /// TCP-0.9.2: Notifications / Toasts v1
    /// 
    /// 4 saniyeden eski notification'ları otomatik kaldırır.
    /// </summary>
    private void DismissTimer_Tick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        var toRemove = ActiveNotifications
            .Where(n => (now - n.Timestamp).TotalMilliseconds >= AutoDismissMilliseconds)
            .ToList();
        
        foreach (var notification in toRemove)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ActiveNotifications.Remove(notification);
            });
        }
    }
    
    /// <summary>
    /// Notification'ı manuel olarak kaldır
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public void Dismiss(NotificationMessage notification)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ActiveNotifications.Remove(notification);
        });
    }
}
