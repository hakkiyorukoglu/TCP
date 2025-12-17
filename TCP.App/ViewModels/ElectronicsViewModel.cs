using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// ElectronicsViewModel - Electronics modülü ViewModel'i
/// 
/// TCP-0.5: Electronics Page Skeleton
/// 
/// Bu ViewModel ElectronicsView'un data context'idir.
/// Electronics modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (ElectronicsView.xaml) sadece UI gösterir
/// - ViewModel (ElectronicsViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Electronics modülü state ve iş mantığı yönetimi
/// </summary>
public class ElectronicsViewModel : ViewModelBase, INotifyPropertyChanged
{
    /// <summary>
    /// PropertyChanged event - UI binding'ler için
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
    /// Board listesi - ObservableCollection MVVM binding için
    /// </summary>
    public ObservableCollection<BoardItem> Boards { get; }
    
    /// <summary>
    /// Seçili board - UI binding için
    /// </summary>
    private BoardItem? _selectedBoard;
    public BoardItem? SelectedBoard
    {
        get => _selectedBoard;
        set
        {
            if (_selectedBoard != value)
            {
                _selectedBoard = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedBoardSummaryData)); // Summary data'yı da güncelle
            }
        }
    }
    
    /// <summary>
    /// Selected board summary data as KeyValuePair list for ItemsControl binding
    /// TCP-0.6.0: Summary cards support
    /// </summary>
    public ObservableCollection<KeyValuePair<string, string>> SelectedBoardSummaryData
    {
        get
        {
            var collection = new ObservableCollection<KeyValuePair<string, string>>();
            if (SelectedBoard?.SummaryData != null)
            {
                foreach (var kvp in SelectedBoard.SummaryData)
                {
                    collection.Add(kvp);
                }
            }
            return collection;
        }
    }
    
    /// <summary>
    /// Board registry - Single source of truth for board definitions
    /// TCP-0.6.0: Electronics Board Registry
    /// </summary>
    private readonly IBoardRegistry _boardRegistry;
    
    /// <summary>
    /// Constructor - Initialize boards from registry
    /// TCP-0.6.0: Board Registry (Single Source of Truth)
    /// </summary>
    public ElectronicsViewModel()
    {
        // Get board registry instance
        _boardRegistry = BoardRegistry.Instance;
        
        // Load boards from registry
        Boards = new ObservableCollection<BoardItem>(_boardRegistry.GetAll());
        
        // Default selection: İlk board
        SelectedBoard = Boards.Count > 0 ? Boards[0] : null;
    }
}

/// <summary>
/// BoardItem - Board model class
/// 
/// TCP-0.6.0: Electronics Board Registry
/// Extended with summary card data
/// </summary>
public class BoardItem
{
    /// <summary>
    /// Board adı
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Board açıklaması
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Board durumu (Offline/Online)
    /// </summary>
    public string Status { get; set; } = "Offline";
    
    /// <summary>
    /// Summary card data - Key/Value pairs for summary cards
    /// TCP-0.6.0: Summary cards support
    /// </summary>
    public Dictionary<string, string> SummaryData { get; set; } = new();
}
