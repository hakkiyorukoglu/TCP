namespace TCP.App.Models;

/// <summary>
/// AppSettings - Uygulama ayarları model class
/// 
/// TCP-0.8.1: Settings Persistence v1 (Local)
/// 
/// Bu model kullanıcı ayarlarını temsil eder.
/// JSON serialization için kullanılır.
/// 
/// Single Responsibility: Settings data model
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Seçili tema
    /// "Dark" veya "Light"
    /// Default: "Dark"
    /// </summary>
    public string Theme { get; set; } = "Dark";
    
    /// <summary>
    /// Son ziyaret edilen sayfa route'u
    /// "Home", "Electronics", "Simulation", "Editor", "Settings", "Info"
    /// Default: "Home"
    /// </summary>
    public string LastRoute { get; set; } = "Home";
    
    /// <summary>
    /// Sol panel genişliği (pixel)
    /// Electronics sayfasındaki sol board list paneli için
    /// Default: 240.0
    /// </summary>
    public double LeftPanelWidth { get; set; } = 240.0;
    
    /// <summary>
    /// Settings kategori listesi genişliği (pixel)
    /// Settings sayfasındaki sol kategori listesi için
    /// Default: 220.0
    /// </summary>
    public double SettingsCategoryWidth { get; set; } = 220.0;
}
