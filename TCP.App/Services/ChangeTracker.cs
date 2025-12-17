using System;
using System.Collections.Generic;

namespace TCP.App.Services;

/// <summary>
/// ChangeTracker - Değişiklik takip servisi
/// 
/// Bu servis uygulama içindeki değişiklikleri takip eder.
/// Undo/Redo, dirty state, save kontrolü gibi işlevler için kullanılacak.
/// 
/// Single Responsibility: Değişiklik state yönetimi
/// </summary>
public class ChangeTracker
{
    /// <summary>
    /// Değişiklik var mı?
    /// </summary>
    public bool HasChanges { get; private set; }
    
    /// <summary>
    /// Değişiklik event'i
    /// </summary>
    public event System.Action<bool>? Changed;
    
    /// <summary>
    /// Değişiklik kayıtları
    /// Mimari değişiklikler ve önemli güncellemeler burada kaydedilir
    /// </summary>
    private readonly List<ChangeRecord> _changeHistory = new();
    
    /// <summary>
    /// Değişiklik durumunu işaretle
    /// </summary>
    public void MarkChanged(bool hasChanges)
    {
        HasChanges = hasChanges;
        Changed?.Invoke(hasChanges);
    }
    
    /// <summary>
    /// Değişiklikleri temizle (saved)
    /// </summary>
    public void ClearChanges()
    {
        MarkChanged(false);
    }
    
    /// <summary>
    /// Değişiklik kaydı ekle
    /// Mimari değişiklikler ve önemli güncellemeler için
    /// </summary>
    public void RegisterChange(string category, string description)
    {
        _changeHistory.Add(new ChangeRecord
        {
            Timestamp = DateTime.Now,
            Category = category,
            Description = description
        });
    }
    
    /// <summary>
    /// Değişiklik kayıtlarını al
    /// </summary>
    public IReadOnlyList<ChangeRecord> GetChangeHistory() => _changeHistory;
}

/// <summary>
/// ChangeRecord - Değişiklik kaydı
/// </summary>
public class ChangeRecord
{
    public DateTime Timestamp { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
