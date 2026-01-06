namespace TCP.App.Models.Editor;

/// <summary>
/// PlacedBoardBox - Placed board box on editor canvas
/// 
/// TCP-1.0.3: Editor: Add board boxes from registry
/// 
/// This model represents a board box that has been placed on the editor canvas.
/// 
/// Single Responsibility: Placed box data model
/// </summary>
public class PlacedBoardBox
{
    /// <summary>
    /// Box ID (immutable, unique identifier)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public Guid BoxId { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// Board ID (references BoardDefinition.Id)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string BoardId { get; init; } = string.Empty;
    
    /// <summary>
    /// Display name (copy for easy rendering)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;
    
    /// <summary>
    /// Board type
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Type { get; init; } = string.Empty;
    
    /// <summary>
    /// Board status
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public string Status { get; init; } = "Offline";
    
    /// <summary>
    /// X coordinate (world coords)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public double X { get; set; }
    
    /// <summary>
    /// Y coordinate (world coords)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public double Y { get; set; }
}

