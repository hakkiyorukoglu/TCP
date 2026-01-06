using System.Collections.Generic;
using System.Linq;
using TCP.App.Models.Editor;
using TCP.App.ViewModels;

namespace TCP.App.Services;

/// <summary>
/// BoardRegistry - Centralized electronics board registry
/// 
/// TCP-0.6.0: Electronics Board Registry
/// 
/// Singleton service that maintains all board definitions.
/// Prevents duplicate board names.
/// Application-scoped lifetime.
/// </summary>
public class BoardRegistry : IBoardRegistry
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    private static BoardRegistry? _instance;
    
    /// <summary>
    /// Get singleton instance
    /// </summary>
    public static BoardRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BoardRegistry();
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Internal list of boards
    /// </summary>
    private readonly List<BoardItem> _boards = new();
    
    /// <summary>
    /// Private constructor (singleton pattern)
    /// </summary>
    private BoardRegistry()
    {
    }
    
    /// <summary>
    /// Get all registered boards
    /// Returns read-only collection
    /// </summary>
    public IEnumerable<BoardItem> GetAll()
    {
        return _boards.AsReadOnly();
    }
    
    /// <summary>
    /// Register a board
    /// Prevents duplicate names
    /// </summary>
    public void Register(BoardItem board)
    {
        if (board == null)
        {
            throw new ArgumentNullException(nameof(board));
        }
        
        if (string.IsNullOrWhiteSpace(board.Name))
        {
            throw new ArgumentException("Board name cannot be null or empty", nameof(board));
        }
        
        // Prevent duplicate names
        if (_boards.Any(b => string.Equals(b.Name, board.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return; // Silently ignore duplicate registrations
        }
        
        _boards.Add(board);
    }
    
    /// <summary>
    /// Get board by name
    /// </summary>
    public BoardItem? GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }
        
        return _boards.FirstOrDefault(b => string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Get all boards as BoardDefinition list (for Editor palette)
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// 
    /// Converts BoardItem to BoardDefinition.
    /// Single source of truth: This registry.
    /// </summary>
    public IReadOnlyList<BoardDefinition> GetAllBoards()
    {
        return _boards.Select(item => new BoardDefinition
        {
            Id = item.Name, // Use Name as Id (immutable, unique)
            DisplayName = item.Name,
            Type = ExtractTypeFromName(item.Name),
            Status = item.Status,
            Notes = item.Description
        }).ToList().AsReadOnly();
    }
    
    /// <summary>
    /// Get board by ID
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public BoardDefinition? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }
        
        var item = _boards.FirstOrDefault(b => string.Equals(b.Name, id, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            return null;
        }
        
        return new BoardDefinition
        {
            Id = item.Name,
            DisplayName = item.Name,
            Type = ExtractTypeFromName(item.Name),
            Status = item.Status,
            Notes = item.Description
        };
    }
    
    /// <summary>
    /// Extract type from board name
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// 
    /// Examples:
    /// - "Arduino Mega" -> "Mega"
    /// - "Arduino Nano" -> "Nano"
    /// - "RFID Reader" -> "RFID"
    /// - "Servo Controller" -> "Servo"
    /// </summary>
    private static string ExtractTypeFromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Unknown";
        }
        
        // Remove common prefixes
        var type = name;
        if (type.StartsWith("Arduino ", StringComparison.OrdinalIgnoreCase))
        {
            type = type.Substring("Arduino ".Length);
        }
        else if (type.EndsWith(" Reader", StringComparison.OrdinalIgnoreCase))
        {
            type = type.Substring(0, type.Length - " Reader".Length);
        }
        else if (type.EndsWith(" Controller", StringComparison.OrdinalIgnoreCase))
        {
            type = type.Substring(0, type.Length - " Controller".Length);
        }
        
        return type;
    }
}
