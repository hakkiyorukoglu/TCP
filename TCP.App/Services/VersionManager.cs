using System;

namespace TCP.App.Services;

/// <summary>
/// VersionManager - Versiyon yönetim servisi
/// 
/// Bu servis uygulamanın versiyon bilgilerini yönetir.
/// VersionInfo yerine kullanılır (TCP-0.2'de güncellendi).
/// 
/// Single Responsibility: Versiyon metadata yönetimi
/// </summary>
public static class VersionManager
{
    /// <summary>
    /// Mevcut versiyon
    /// Semantic versioning formatında: TCP-Major.Minor.Patch
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// 
    /// IMPORTANT: Bu değer tek yerden güncellenir - tüm UI otomatik güncellenir
    /// </summary>
    public static string CurrentVersion => "TCP-1.0.3";
    
    /// <summary>
    /// Build time
    /// Uygulamanın derlendiği zaman
    /// </summary>
    public static DateTime BuildTime => DateTime.Now;
    
    /// <summary>
    /// Stage adı
    /// Mevcut geliştirme aşaması
    /// </summary>
    public static string StageName => "Editor: Add board boxes from registry";
    
    /// <summary>
    /// Display versiyon
    /// UI'da gösterilmek üzere formatlanmış versiyon
    /// </summary>
    public static string DisplayVersion => CurrentVersion;
}
