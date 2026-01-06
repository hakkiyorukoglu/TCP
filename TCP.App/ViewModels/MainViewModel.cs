using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    /// Search registry - Single source of truth for search suggestions
    /// TCP-0.5.2: Search Registry
    /// </summary>
    private readonly ISearchRegistry _searchRegistry;
    
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
    /// Constructor - Initialize search suggestions from registry
    /// TCP-0.5.2: Search Registry (Single Source of Truth)
    /// </summary>
    public MainViewModel()
    {
        // Get search registry instance
        _searchRegistry = SearchRegistry.Instance;
        
        FilteredSuggestions = new ObservableCollection<SearchItem>();
        
        SelectSearchItemCommand = new RelayCommand<SearchItem>(SelectSearchItem);
    }
    
    /// <summary>
    /// Filter suggestions based on search text
    /// TCP-0.5.2: Match Title OR Keywords (case-insensitive)
    /// </summary>
    private void FilterSuggestions()
    {
        FilteredSuggestions.Clear();
        
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            IsDropdownVisible = false;
            return;
        }
        
        var searchTextLower = SearchText.ToLowerInvariant();
        var allItems = _searchRegistry.GetAll();
        
        // Filter: Match Title OR any Keyword (case-insensitive contains check)
        var matched = allItems.Where(item =>
        {
            // Match title
            if (item.Title.ToLowerInvariant().Contains(searchTextLower))
            {
                return true;
            }
            
            // Match any keyword
            if (item.Keywords != null && item.Keywords.Any(keyword =>
                keyword.ToLowerInvariant().Contains(searchTextLower)))
            {
                return true;
            }
            
            return false;
        });
        
        foreach (var item in matched)
        {
            FilteredSuggestions.Add(item);
        }
        
        IsDropdownVisible = FilteredSuggestions.Count > 0;
    }
    
    /// <summary>
    /// Select search item and navigate
    /// TCP-0.5.2: Use Route property
    /// </summary>
    private void SelectSearchItem(SearchItem? item)
    {
        if (item == null) return;
        
        NavigateRequested?.Invoke(item.Route);
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

/// <summary>
/// RelayCommandWithCanExecute - ICommand implementation with CanExecute support
/// TCP-1.0.3: Editor: Add board boxes from registry
/// </summary>
public class RelayCommandWithCanExecute<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<bool> _canExecute;
    
    public RelayCommandWithCanExecute(Action<T?> execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter) => _canExecute();
    
    public void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            _execute(parameter is T t ? t : default);
        }
    }
    
    /// <summary>
    /// Raise CanExecuteChanged event
    /// TCP-1.0.3: Editor: Add board boxes from registry
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}