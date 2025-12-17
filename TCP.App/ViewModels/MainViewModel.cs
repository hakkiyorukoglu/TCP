using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// MainViewModel - Ana pencere ViewModel'i
/// 
/// TCP-0.5.1: Top-Right Search UI
/// 
/// Bu ViewModel MainWindow'un data context'idir.
/// Navigation ve genel uygulama state'ini yönetir.
/// 
/// MVVM Pattern:
/// - View (MainWindow.xaml) sadece UI gösterir
/// - ViewModel (MainViewModel) tüm mantığı içerir
/// - Model (gelecekte TCP.Core'da) veri ve iş mantığını içerir
/// 
/// Single Responsibility: Ana pencere state ve navigation yönetimi
/// </summary>
public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    /// <summary>
    /// PropertyChanged event - UI binding'ler için
    /// MVVM pattern'de ViewModel property'leri değiştiğinde UI otomatik güncellenir
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// PropertyChanged event'ini tetikler
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /// <summary>
    /// Search text - user input
    /// </summary>
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
                FilterSuggestions();
            }
        }
    }
    
    /// <summary>
    /// All available search suggestions (hardcoded)
    /// </summary>
    public ObservableCollection<SearchItem> Suggestions { get; }
    
    /// <summary>
    /// Filtered suggestions based on search text
    /// </summary>
    private ObservableCollection<SearchItem> _filteredSuggestions = new();
    public ObservableCollection<SearchItem> FilteredSuggestions
    {
        get => _filteredSuggestions;
        private set
        {
            if (_filteredSuggestions != value)
            {
                _filteredSuggestions = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Selected search item
    /// </summary>
    private SearchItem? _selectedSearchItem;
    public SearchItem? SelectedSearchItem
    {
        get => _selectedSearchItem;
        set
        {
            if (_selectedSearchItem != value)
            {
                _selectedSearchItem = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Is dropdown visible
    /// </summary>
    private bool _isDropdownVisible;
    public bool IsDropdownVisible
    {
        get => _isDropdownVisible;
        set
        {
            if (_isDropdownVisible != value)
            {
                _isDropdownVisible = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// Navigation action - triggered when search item is selected
    /// </summary>
    public event Action<string>? NavigateRequested;
    
    /// <summary>
    /// Select search item command
    /// </summary>
    public ICommand SelectSearchItemCommand { get; }
    
    /// <summary>
    /// Constructor - Initialize search suggestions
    /// </summary>
    public MainViewModel()
    {
        // Hardcoded search suggestions
        Suggestions = new ObservableCollection<SearchItem>
        {
            new SearchItem { DisplayText = "Go to Home", TargetRoute = "Home" },
            new SearchItem { DisplayText = "Go to Electronics", TargetRoute = "Electronics" },
            new SearchItem { DisplayText = "Go to Simulation", TargetRoute = "Simulation" },
            new SearchItem { DisplayText = "Go to Settings", TargetRoute = "Settings" },
            new SearchItem { DisplayText = "Open Info Panel", TargetRoute = "Info" }
        };
        
        FilteredSuggestions = new ObservableCollection<SearchItem>();
        
        SelectSearchItemCommand = new RelayCommand<SearchItem>(SelectSearchItem);
    }
    
    /// <summary>
    /// Filter suggestions based on search text
    /// Naive filtering: if SearchText.Length > 0, show all suggestions
    /// </summary>
    private void FilterSuggestions()
    {
        FilteredSuggestions.Clear();
        
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            IsDropdownVisible = false;
            return;
        }
        
        // Naive filtering: show all suggestions when typing
        foreach (var suggestion in Suggestions)
        {
            FilteredSuggestions.Add(suggestion);
        }
        
        IsDropdownVisible = FilteredSuggestions.Count > 0;
    }
    
    /// <summary>
    /// Select search item and navigate
    /// </summary>
    private void SelectSearchItem(SearchItem? item)
    {
        if (item == null) return;
        
        NavigateRequested?.Invoke(item.TargetRoute);
        SearchText = string.Empty;
        IsDropdownVisible = false;
        SelectedSearchItem = null;
    }
}

/// <summary>
/// RelayCommand - Simple ICommand implementation
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    
    public RelayCommand(Action<T?> execute)
    {
        _execute = execute;
    }
    
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter) => true;
    
    public void Execute(object? parameter)
    {
        _execute(parameter is T t ? t : default);
    }
}
