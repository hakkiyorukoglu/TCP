using System.Collections.Generic;
using TCP.App.ViewModels;

namespace TCP.App.Services;

/// <summary>
/// IBoardRegistry - Board registry interface
/// 
/// TCP-0.6.0: Electronics Board Registry
/// 
/// Centralized registry for electronics boards.
/// Each board type registers itself with name, description, and summary data.
/// </summary>
public interface IBoardRegistry
{
    /// <summary>
    /// Get all registered boards
    /// </summary>
    IEnumerable<BoardItem> GetAll();
    
    /// <summary>
    /// Register a board
    /// </summary>
    void Register(BoardItem board);
    
    /// <summary>
    /// Get board by name
    /// </summary>
    BoardItem? GetByName(string name);
}
