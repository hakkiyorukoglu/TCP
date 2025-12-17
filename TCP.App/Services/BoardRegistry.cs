using System.Collections.Generic;
using System.Linq;
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
}
