using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TCP.App.Data.Entities;

public class ScenarioDb
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "Yeni Senaryo";
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    // Şimdilik WPF modellerini bozmamak adına state'i JSON serialize edip tutuyoruz.
    public string NetworkStateJson { get; set; } = "{}";
    public string EditorLayoutJson { get; set; } = "{}";
    
    // İleride ilişkisel olarak C# scriptleri eklenecek
    public List<CustomCodeDb> CustomCodes { get; set; } = new();
}
