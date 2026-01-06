using System;

namespace TCP.App.Models;

/// <summary>
/// NotificationMessage - Toast/notification mesaj modeli
/// 
/// TCP-0.9.2: Notifications / Toasts v1
/// 
/// Bu model toast notification'ları temsil eder.
/// UI-only data model.
/// </summary>
public class NotificationMessage
{
    /// <summary>
    /// Notification başlığı
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Notification mesajı
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Notification tipi (Success, Warning, Error)
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public NotificationType Type { get; set; } = NotificationType.Success;
    
    /// <summary>
    /// Notification zaman damgası
    /// TCP-0.9.2: Notifications / Toasts v1
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// NotificationType - Notification tipi enum
/// TCP-0.9.2: Notifications / Toasts v1
/// TCP-1.0.1: Added Info type
/// </summary>
public enum NotificationType
{
    Success,
    Warning,
    Error,
    Info
}
