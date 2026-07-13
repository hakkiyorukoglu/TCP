using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TCP.App.Models.Electronics;
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

        // TCP Custom Device Logic
        OpenCreatePopupCommand = new RelayCommand<object>(_ => { if (SelectedBoard != null) OpenCreatePopup(); });
        CloseCreatePopupCommand = new RelayCommand<object>(_ => CloseCreatePopup());
        SaveDeviceCommand = new RelayCommand<object>(_ => { if (CanSaveDevice()) SaveDevice(); });
    }

    /// <summary>
    /// User's saved devices list
    /// </summary>
    public ObservableCollection<DeviceInstance> MyDevices => DeviceManager.Instance.Devices;

    #region Create Device Popup State & Properties

    private bool _isCreatePopupOpen;
    public bool IsCreatePopupOpen
    {
        get => _isCreatePopupOpen;
        set { _isCreatePopupOpen = value; OnPropertyChanged(); }
    }

    private string _newDeviceName = string.Empty;
    public string NewDeviceName
    {
        get => _newDeviceName;
        set
        {
            _newDeviceName = value;
            OnPropertyChanged();
        }
    }

    private string _newDeviceIp = string.Empty;
    public string NewDeviceIp
    {
        get => _newDeviceIp;
        set { _newDeviceIp = value; OnPropertyChanged(); }
    }

    private int _newDevicePort = 80;
    public int NewDevicePort
    {
        get => _newDevicePort;
        set { _newDevicePort = value; OnPropertyChanged(); }
    }

    private string _newDeviceMac = string.Empty;
    public string NewDeviceMac
    {
        get => _newDeviceMac;
        set { _newDeviceMac = value; OnPropertyChanged(); }
    }

    private string _newDeviceLanCable = string.Empty;
    public string NewDeviceLanCable
    {
        get => _newDeviceLanCable;
        set { _newDeviceLanCable = value; OnPropertyChanged(); }
    }

    private string _newDeviceLocation = string.Empty;
    public string NewDeviceLocation
    {
        get => _newDeviceLocation;
        set { _newDeviceLocation = value; OnPropertyChanged(); }
    }

    public ICommand OpenCreatePopupCommand { get; }
    public ICommand CloseCreatePopupCommand { get; }
    public ICommand SaveDeviceCommand { get; }

    private void OpenCreatePopup()
    {
        if (SelectedBoard == null) return;
        
        NewDeviceName = $"{SelectedBoard.Name} Instance";
        NewDeviceIp = "192.168.1.10";
        NewDevicePort = 80;
        NewDeviceMac = "00:00:00:00:00:00";
        NewDeviceLanCable = "ETH-1";
        NewDeviceLocation = "Lab";
        IsCreatePopupOpen = true;
    }

    private void CloseCreatePopup()
    {
        IsCreatePopupOpen = false;
    }

    private bool CanSaveDevice()
    {
        return !string.IsNullOrWhiteSpace(NewDeviceName);
    }

    private void SaveDevice()
    {
        if (SelectedBoard == null || string.IsNullOrWhiteSpace(NewDeviceName)) return;

        var device = new DeviceInstance
        {
            TemplateId = SelectedBoard.Name,
            CustomName = NewDeviceName,
            IpAddress = NewDeviceIp,
            Port = NewDevicePort,
            MacAddress = NewDeviceMac,
            LanCable = NewDeviceLanCable,
            Location = NewDeviceLocation
        };

        DeviceManager.Instance.AddDevice(device);
        TerminalService.Instance.LogSuccess($"Device created: {NewDeviceName}");
        IsCreatePopupOpen = false;
    }

    #endregion
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
