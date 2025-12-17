namespace TCP.App.Models;

/// <summary>
/// VersionEntry - Versiyon geçmişi entry model class
/// 
/// TCP-0.9.0: Info Panel v1 + Version History UI
/// 
/// Bu model versiyon geçmişi bilgilerini temsil eder.
/// UI-only data model.
/// </summary>
public class VersionEntry
{
    /// <summary>
    /// Versiyon numarası (e.g., "TCP-0.9.0")
    /// </summary>
    public string Version { get; set; } = string.Empty;
    
    /// <summary>
    /// Stage adı / başlık (e.g., "Info Panel v1")
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Kısa açıklama (e.g., "Added Info panel with version history")
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
