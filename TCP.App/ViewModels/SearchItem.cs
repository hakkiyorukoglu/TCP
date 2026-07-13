namespace TCP.App.ViewModels;

/// <summary>
/// SearchItem - Search suggestion model
/// 
/// TCP-0.5.2: Search Registry (Single Source of Truth)
/// </summary>
public class SearchItem
{
    /// <summary>
    /// Resource key for localization
    /// </summary>
    public string TitleKey { get; set; } = string.Empty;

    /// <summary>
    /// Display title shown in suggestions (Localized)
    /// </summary>
    public string Title => !string.IsNullOrEmpty(TitleKey) 
        ? (System.Windows.Application.Current?.TryFindResource(TitleKey) as string ?? TitleKey) 
        : string.Empty;
    
    /// <summary>
    /// Keywords for matching search queries
    /// Case-insensitive matching will be performed on these keywords
    /// </summary>
    public string[] Keywords { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Target route name for navigation
    /// </summary>
    public string Route { get; set; } = string.Empty;
    
    /// <summary>
    /// Display text (for backward compatibility with XAML)
    /// Returns Title
    /// </summary>
    public string DisplayText => Title;
    
    /// <summary>
    /// Target route (for backward compatibility with XAML)
    /// Returns Route
    /// </summary>
    public string TargetRoute => Route;
}
