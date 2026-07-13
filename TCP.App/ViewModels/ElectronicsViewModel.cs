using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using TCP.App.Models.Electronics;
using TCP.App.Services;
using System;

namespace TCP.App.ViewModels;

public class ConnectionOption
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ElectronicsViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public ObservableCollection<BoardItem> Templates { get; }
    private BoardItem? _selectedTemplate;
    public BoardItem? SelectedTemplate
    {
        get => _selectedTemplate;
        set { _selectedTemplate = value; OnPropertyChanged(); }
    }

    // Modems List
    public ObservableCollection<ModemInstance> Modems => NetworkManager.Instance.Modems;

    private ModemInstance? _selectedModem;
    public ModemInstance? SelectedModem
    {
        get => _selectedModem;
        set 
        { 
            _selectedModem = value; 
            UpdateAvailableConnections();
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(SelectedIncomingId));
            OnPropertyChanged(nameof(SelectedOutgoingId));
        }
    }

    public ObservableCollection<ConnectionOption> AvailableConnections { get; } = new();

    private bool _isUpdatingConnections;

    public Guid? SelectedIncomingId
    {
        get => SelectedModem?.IncomingConnectionId;
        set
        {
            if (_isUpdatingConnections) return;
            if (SelectedModem != null && SelectedModem.IncomingConnectionId != value)
            {
                NetworkManager.Instance.SetIncomingConnection(SelectedModem.Id, value);
                UpdateAvailableConnections();
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedOutgoingId));
            }
        }
    }

    public Guid? SelectedOutgoingId
    {
        get => SelectedModem?.OutgoingConnectionId;
        set
        {
            if (_isUpdatingConnections) return;
            if (SelectedModem != null && SelectedModem.OutgoingConnectionId != value)
            {
                NetworkManager.Instance.SetOutgoingConnection(SelectedModem.Id, value);
                UpdateAvailableConnections();
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedIncomingId));
            }
        }
    }

    private void UpdateAvailableConnections()
    {
        if (_isUpdatingConnections) return;
        _isUpdatingConnections = true;

        try
        {
            var newList = new System.Collections.Generic.List<ConnectionOption>();
            newList.Add(new ConnectionOption { Id = null, Name = "Yok" });

            bool mainPcTaken = NetworkManager.Instance.Modems.Any(m => m.IncomingConnectionId == NetworkManager.Instance.MainPc.Id && m.Id != SelectedModem?.Id);
            if (!mainPcTaken)
            {
                newList.Add(new ConnectionOption { Id = NetworkManager.Instance.MainPc.Id, Name = NetworkManager.Instance.MainPc.Name });
            }

            foreach (var m in NetworkManager.Instance.Modems)
            {
                if (SelectedModem == null || m.Id != SelectedModem.Id)
                {
                    newList.Add(new ConnectionOption { Id = m.Id, Name = m.Name });
                }
            }

            AvailableConnections.Clear();
            foreach (var item in newList)
            {
                AvailableConnections.Add(item);
            }
            
            OnPropertyChanged(nameof(SelectedIncomingId));
            OnPropertyChanged(nameof(SelectedOutgoingId));
        }
        finally
        {
            _isUpdatingConnections = false;
        }
    }

    #region Create Modem Popup
    private bool _isCreateModemPopupOpen;
    public bool IsCreateModemPopupOpen { get => _isCreateModemPopupOpen; set { _isCreateModemPopupOpen = value; OnPropertyChanged(); } }
    
    private string _newModemName = "Yeni Modem";
    public string NewModemName { get => _newModemName; set { _newModemName = value; OnPropertyChanged(); } }
    
    private string _newModemIp = "192.168.1.1";
    public string NewModemIp { get => _newModemIp; set { _newModemIp = value; OnPropertyChanged(); } }
    
    private string _newModemMac = "00:00:00:00:00:00";
    public string NewModemMac { get => _newModemMac; set { _newModemMac = value; OnPropertyChanged(); } }

    public ICommand OpenCreateModemPopupCommand { get; }
    public ICommand CloseCreateModemPopupCommand { get; }
    public ICommand SaveModemCommand { get; }
    public ICommand EditModemCommand { get; }
    public ICommand DeleteModemCommand { get; }
    
    public ICommand SaveModemLinksCommand { get; }
    #endregion

    #region Create Station Popup
    private bool _isCreateStationPopupOpen;
    public bool IsCreateStationPopupOpen { get => _isCreateStationPopupOpen; set { _isCreateStationPopupOpen = value; OnPropertyChanged(); } }
    
    private string _newStationName = "Yeni İstasyon";
    public string NewStationName { get => _newStationName; set { _newStationName = value; OnPropertyChanged(); } }
    
    private string _newStationIp = "192.168.1.10";
    public string NewStationIp { get => _newStationIp; set { _newStationIp = value; OnPropertyChanged(); } }
    
    private int _newStationPort = 80;
    public int NewStationPort { get => _newStationPort; set { _newStationPort = value; OnPropertyChanged(); } }
    
    private string _newStationMac = "00:00:00:00:00:00";
    public string NewStationMac { get => _newStationMac; set { _newStationMac = value; OnPropertyChanged(); } }

    public ICommand OpenCreateStationPopupCommand { get; }
    public ICommand CloseCreateStationPopupCommand { get; }
    public ICommand SaveStationCommand { get; }
    public ICommand EditStationCommand { get; }
    public ICommand DeleteStationCommand { get; }
    #endregion

    #region Create Component Popup
    private bool _isCreateComponentPopupOpen;
    public bool IsCreateComponentPopupOpen { get => _isCreateComponentPopupOpen; set { _isCreateComponentPopupOpen = value; OnPropertyChanged(); } }

    private StationInstance? _stationForNewComponent;
    public StationInstance? StationForNewComponent { get => _stationForNewComponent; set { _stationForNewComponent = value; OnPropertyChanged(); } }

    private string _newComponentName = "Yeni Bileşen";
    public string NewComponentName { get => _newComponentName; set { _newComponentName = value; OnPropertyChanged(); } }
    
    private string _newComponentPin = "D2";
    public string NewComponentPin { get => _newComponentPin; set { _newComponentPin = value; OnPropertyChanged(); } }

    public ICommand OpenCreateComponentPopupCommand { get; }
    public ICommand CloseCreateComponentPopupCommand { get; }
    public ICommand SaveComponentCommand { get; }
    
    public ICommand DeleteComponentCommand { get; }
    public ICommand EditComponentCommand { get; }
    #endregion

    public ICommand AddTemplateCommand { get; }
    public ICommand EditTemplateCommand { get; }
    public ICommand DeleteTemplateCommand { get; }

    public ElectronicsViewModel()
    {
        Templates = new ObservableCollection<BoardItem>(BoardRegistry.Instance.GetAll());
        SelectedTemplate = Templates.FirstOrDefault();

        OpenCreateModemPopupCommand = new RelayCommand<object>(_ => OpenCreateModemPopup());
        CloseCreateModemPopupCommand = new RelayCommand<object>(_ => IsCreateModemPopupOpen = false);
        SaveModemCommand = new RelayCommand<object>(_ => SaveModem());
        EditModemCommand = new RelayCommand<object>(param => EditModem(param as ModemInstance));
        DeleteModemCommand = new RelayCommand<object>(param => DeleteModem(param as ModemInstance));
        SaveModemLinksCommand = new RelayCommand<object>(_ => SaveModemLinks());

        OpenCreateStationPopupCommand = new RelayCommand<object>(_ => OpenCreateStationPopup());
        CloseCreateStationPopupCommand = new RelayCommand<object>(_ => IsCreateStationPopupOpen = false);
        SaveStationCommand = new RelayCommand<object>(_ => SaveStation());
        EditStationCommand = new RelayCommand<StationInstance>(EditStation);
        DeleteStationCommand = new RelayCommand<StationInstance>(DeleteStation);

        OpenCreateComponentPopupCommand = new RelayCommand<StationInstance>(OpenCreateComponentPopup);
        CloseCreateComponentPopupCommand = new RelayCommand<object>(_ => IsCreateComponentPopupOpen = false);
        SaveComponentCommand = new RelayCommand<object>(_ => SaveComponent());
        
        DeleteComponentCommand = new RelayCommand<ComponentInstance>(DeleteComponent);
        EditComponentCommand = new RelayCommand<ComponentInstance>(EditComponent);
        
        AddTemplateCommand = new RelayCommand<object>(_ => AddTemplate());
        EditTemplateCommand = new RelayCommand<BoardItem>(EditTemplate);
        DeleteTemplateCommand = new RelayCommand<BoardItem>(DeleteTemplate);
    }
    
    public void RefreshTemplates()
    {
        Templates.Clear();
        foreach(var t in BoardRegistry.Instance.GetAll())
            Templates.Add(t);
        if (SelectedTemplate == null || !Templates.Contains(SelectedTemplate))
            SelectedTemplate = Templates.FirstOrDefault();
    }
    
    private void AddTemplate()
    {
        var newBoard = new BoardItem { Name = $"Yeni Elektronik {Templates.Count + 1}", Status = "Offline" };
        var window = new TCP.App.Views.LayerPropertiesWindow(newBoard);
        if (window.ShowDialog() == true)
        {
            BoardRegistry.Instance.Register(newBoard);
            BoardRegistry.Instance.SaveBoards();
            RefreshTemplates();
            SelectedTemplate = newBoard;
        }
    }
    
    private void EditTemplate(BoardItem? item)
    {
        if (item == null) return;
        var window = new TCP.App.Views.LayerPropertiesWindow(item);
        if (window.ShowDialog() == true)
        {
            BoardRegistry.Instance.SaveBoards();
            RefreshTemplates();
        }
    }
    
    private void DeleteTemplate(BoardItem? item)
    {
        if (item == null) return;
        BoardRegistry.Instance.Remove(item);
        RefreshTemplates();
    }

    private void OpenCreateModemPopup()
    {
        NewModemName = $"Modem {Modems.Count + 1}";
        NewModemIp = "192.168.1.1";
        NewModemMac = "00:00:00:00:00:00";
        IsCreateModemPopupOpen = true;
    }

    private void SaveModem()
    {
        if (string.IsNullOrWhiteSpace(NewModemName)) return;
        var m = new ModemInstance
        {
            Name = NewModemName,
            IpAddress = NewModemIp,
            MacAddress = NewModemMac
        };
        NetworkManager.Instance.AddModem(m);
        SelectedModem = m;
        IsCreateModemPopupOpen = false;
    }

    private void EditModem(ModemInstance? m)
    {
        if (m == null) return;
        var window = new TCP.App.Views.LayerPropertiesWindow(m);
        if (window.ShowDialog() == true)
        {
            NetworkManager.Instance.SaveModems();
            OnPropertyChanged(nameof(Modems));
        }
    }

    private void DeleteModem(ModemInstance? m)
    {
        if (m == null) return;
        NetworkManager.Instance.RemoveModem(m.Id);
    }

    private void SaveModemLinks()
    {
        NetworkManager.Instance.SaveNetwork();
        TerminalService.Instance.LogSuccess("Bağlantılar başarıyla kaydedildi.");
    }

    private void OpenCreateStationPopup()
    {
        if (SelectedModem == null) return;
        if (SelectedModem.Stations.Count >= 3)
        {
            TerminalService.Instance.LogWarning("Bir modem en fazla 3 istasyon yönetebilir!");
            return;
        }
        NewStationName = $"İstasyon {SelectedModem.Stations.Count + 1}";
        NewStationIp = "192.168.1.10";
        NewStationPort = 80;
        NewStationMac = "00:00:00:00:00:00";
        IsCreateStationPopupOpen = true;
    }

    private void SaveStation()
    {
        if (SelectedModem == null || string.IsNullOrWhiteSpace(NewStationName)) return;
        if (SelectedModem.Stations.Count >= 3)
        {
            TerminalService.Instance.LogWarning("Bir modem en fazla 3 istasyon yönetebilir!");
            IsCreateStationPopupOpen = false;
            return;
        }

        var st = new StationInstance
        {
            Name = NewStationName,
            IpAddress = NewStationIp,
            Port = NewStationPort,
            MacAddress = NewStationMac,
            RouterPort = $"Port {SelectedModem.Stations.Count + 1}"
        };
        SelectedModem.Stations.Add(st);
        NetworkManager.Instance.SaveModems();
        IsCreateStationPopupOpen = false;
    }

    private void EditStation(StationInstance? st)
    {
        if (st == null) return;
        var window = new TCP.App.Views.LayerPropertiesWindow(st);
        if (window.ShowDialog() == true)
        {
            NetworkManager.Instance.SaveModems();
        }
    }

    private void DeleteStation(StationInstance? st)
    {
        if (st == null || SelectedModem == null) return;
        SelectedModem.Stations.Remove(st);
        NetworkManager.Instance.SaveModems();
    }

    private void OpenCreateComponentPopup(StationInstance? st)
    {
        if (st == null || SelectedTemplate == null) return;
        StationForNewComponent = st;
        NewComponentName = $"{SelectedTemplate.Name} 1";
        NewComponentPin = "D2";
        IsCreateComponentPopupOpen = true;
    }

    private void SaveComponent()
    {
        if (StationForNewComponent == null || SelectedTemplate == null || string.IsNullOrWhiteSpace(NewComponentName)) return;
        
        if (StationForNewComponent.Components.Any(c => c.Pin == NewComponentPin))
        {
            TerminalService.Instance.LogWarning($"Hata: {NewComponentPin} pini bu istasyonda zaten kullanılıyor!");
            return;
        }

        var comp = new ComponentInstance
        {
            StationId = StationForNewComponent.Id,
            TemplateId = SelectedTemplate.Name,
            Name = NewComponentName,
            Pin = NewComponentPin
        };
        StationForNewComponent.Components.Add(comp);
        NetworkManager.Instance.SaveModems();
        IsCreateComponentPopupOpen = false;
    }
    
    private void DeleteComponent(ComponentInstance? comp)
    {
        if (comp == null || SelectedModem == null) return;
        foreach (var st in SelectedModem.Stations)
        {
            if (st.Components.Contains(comp))
            {
                st.Components.Remove(comp);
                NetworkManager.Instance.SaveModems();
                break;
            }
        }
    }

    private void EditComponent(ComponentInstance? comp)
    {
        if (comp == null) return;
        var window = new TCP.App.Views.LayerPropertiesWindow(comp);
        if (window.ShowDialog() == true)
        {
            NetworkManager.Instance.SaveModems();
        }
    }
}
