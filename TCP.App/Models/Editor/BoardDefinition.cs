namespace TCP.App.Models.Editor;

/// <summary>
/// BoardDefinition - Board definition model for Editor palette
/// 
/// TCP-1.0.3: Editor: Add board boxes from registry
/// 
/// This model represents a board definition that can be placed on the editor canvas.
/// Single source of truth: BoardRegistry (converted from BoardItem).
/// 
/// Single Responsibility: Board definition data model
/// </summary>
public class BoardDefinition
{
    /// <summary>
    /// Board ID (immutable, unique identifier)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// Display name (shown in UI)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;
    
    /// <summary>
    /// Board type (e.g., "Mega", "Nano", "RFID", "Servo")
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Type { get; init; } = string.Empty;
    
    /// <summary>
    /// Board status (placeholder: "Offline" / "Unknown")
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Status { get; init; } = "Offline";
    
    /// <summary>
    /// Optional notes
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string? Notes { get; init; }
}

