using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCP.App.Data.Entities;

public class CustomCodeDb
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid ScenarioId { get; set; }
    
    [ForeignKey("ScenarioId")]
    public ScenarioDb? Scenario { get; set; }
    
    public Guid TargetNodeId { get; set; } // Hangi cihaza ait olduğu (Modem, Station vs)
    
    public string ScriptType { get; set; } = "CSharp";
    
    public string CodeContent { get; set; } = "";
}
