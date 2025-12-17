namespace TCP.App.ViewModels;

/// <summary>
/// SearchItem - Search suggestion model
/// 
/// TCP-0.5.1: Top-Right Search UI
/// </summary>
public class SearchItem
{
    /// <summary>
    /// Display text shown in suggestions
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;
    
    /// <summary>
    /// Target route name for navigation
    /// </summary>
    public string TargetRoute { get; set; } = string.Empty;
}
