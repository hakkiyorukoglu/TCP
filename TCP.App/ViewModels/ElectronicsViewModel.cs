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
            }
        }
    }
    
    /// <summary>
    /// Constructor - Hardcoded data ile başlatma
    /// TCP-0.5: Placeholder data for UI testing
    /// </summary>
    public ElectronicsViewModel()
    {
        // Hardcoded board listesi (placeholder data)
        Boards = new ObservableCollection<BoardItem>
        {
            new BoardItem { Name = "Arduino Mega", Description = "ATmega2560 microcontroller board with 54 digital I/O pins", Status = "Offline" },
            new BoardItem { Name = "Arduino Nano", Description = "Compact ATmega328P board with 14 digital I/O pins", Status = "Offline" },
            new BoardItem { Name = "RFID Module", Description = "RFID reader/writer module for card detection", Status = "Offline" },
            new BoardItem { Name = "Servo Controller", Description = "Multi-channel servo motor controller board", Status = "Offline" }
        };
        
        // Default selection: İlk board
        SelectedBoard = Boards.Count > 0 ? Boards[0] : null;
    }
}

/// <summary>
/// BoardItem - Board model class
/// 
/// TCP-0.5: Simple model for board data
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
}
