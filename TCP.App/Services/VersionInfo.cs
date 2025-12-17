using System;

namespace TCP.App.Services;

/// <summary>
/// VersionInfo - Uygulama versiyon bilgilerini sağlar
/// 
/// Bu sınıf uygulamanın versiyon metadata'sını tutar ve UI'da gösterilmesi için
/// statik property'ler sağlar. MVVM pattern'de ViewModel'ler bu bilgileri kullanabilir.
/// 
/// Single Responsibility: Sadece versiyon bilgilerini yönetir.
/// </summary>
public static class VersionInfo
{
    /// <summary>
    /// Uygulama versiyonu
    /// Semantic versioning formatında: TCP-Major.Minor
    /// </summary>
    public static string Version => "TCP-0.1";
    
    /// <summary>
    /// Sürüm adı
    /// Her sürüm için açıklayıcı bir isim
    /// </summary>
    public static string ReleaseName => "Foundation Shell";
    
    /// <summary>
    /// Derleme zamanı
    /// Uygulamanın derlendiği zamanı gösterir
    /// Not: Production'da bu değer build time'da set edilebilir
    /// </summary>
    public static DateTime BuildTime => DateTime.Now;
    
    /// <summary>
    /// Tam versiyon string'i
    /// UI'da gösterilmek üzere formatlanmış versiyon bilgisi
    /// </summary>
    public static string DisplayVersion => Version;
}
