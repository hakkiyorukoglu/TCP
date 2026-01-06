using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TCP.App.Services;

namespace TCP.App.ViewModels;

/// <summary>
/// EditorImageMode - Image display mode enum
/// TCP-1.0.2: Background Image Load (Editor)
/// </summary>
public enum EditorImageMode
{
    /// <summary>
    /// Fit mode: Image Stretch="Uniform", centered, no scrollbars
    /// </summary>
    Fit,
    
    /// <summary>
    /// Actual mode: ScrollViewer with Image Stretch="None", scrollbars Auto
    /// </summary>
    Actual
}

/// <summary>
/// EditorViewModel - Editor modülü ViewModel'i
/// 
/// TCP-1.0.2: Background Image Load (Editor)
/// 
/// Bu ViewModel EditorView'un data context'idir.
/// Editor modülü state'ini ve iş mantığını yönetir.
/// 
/// MVVM Pattern:
/// - View (EditorView.xaml) sadece UI gösterir
/// - ViewModel (EditorViewModel) tüm mantığı içerir
/// 
/// Single Responsibility: Editor modülü state ve iş mantığı yönetimi
/// </summary>
public class EditorViewModel : ViewModelBase, INotifyPropertyChanged
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
    /// Background image source
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private BitmapImage? _backgroundImage;
    public BitmapImage? BackgroundImage
    {
        get => _backgroundImage;
        private set
        {
            if (_backgroundImage != value)
            {
                _backgroundImage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasImage));
                OnPropertyChanged(nameof(HasNoImage));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }
    
    /// <summary>
    /// Background image filename (for display)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private string? _backgroundImageName;
    public string? BackgroundImageName
    {
        get => _backgroundImageName;
        private set
        {
            if (_backgroundImageName != value)
            {
                _backgroundImageName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }
    
    /// <summary>
    /// Has image loaded
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool HasImage => BackgroundImage != null;
    
    /// <summary>
    /// Has no image (for visibility binding)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool HasNoImage => !HasImage;
    
    /// <summary>
    /// Image display mode (Fit or Actual)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private EditorImageMode _imageMode = EditorImageMode.Fit;
    public EditorImageMode ImageMode
    {
        get => _imageMode;
        private set
        {
            if (_imageMode != value)
            {
                _imageMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFitMode));
                OnPropertyChanged(nameof(IsActualMode));
            }
        }
    }
    
    /// <summary>
    /// Is Fit mode (for binding convenience)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool IsFitMode => ImageMode == EditorImageMode.Fit;
    
    /// <summary>
    /// Is Actual mode (for binding convenience)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public bool IsActualMode => ImageMode == EditorImageMode.Actual;
    
    /// <summary>
    /// Status text (for display in toolbar)
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public string StatusText
    {
        get
        {
            if (!HasImage)
            {
                return "No image loaded";
            }
            return $"Image loaded: {BackgroundImageName ?? "Unknown"}";
        }
    }
    
    /// <summary>
    /// Load Image Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand LoadImageCommand { get; }
    
    /// <summary>
    /// Set Fit Mode Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand SetFitModeCommand { get; }
    
    /// <summary>
    /// Set Actual Mode Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand SetActualModeCommand { get; }
    
    /// <summary>
    /// Clear Image Command
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public ICommand ClearImageCommand { get; }
    
    /// <summary>
    /// Constructor - Initialize commands
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    public EditorViewModel()
    {
        LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
        SetFitModeCommand = new RelayCommand<object>(_ => SetFitMode());
        SetActualModeCommand = new RelayCommand<object>(_ => SetActualMode());
        ClearImageCommand = new RelayCommand<object>(_ => ClearImage());
    }
    
    /// <summary>
    /// Load Image command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// 
    /// Opens file dialog, loads image safely with CacheOption.OnLoad to prevent file lock.
    /// </summary>
    private void LoadImage()
    {
        try
        {
            // TCP-1.0.2: Open file dialog
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Title = "Load Background Image"
            };
            
            if (dialog.ShowDialog() == true)
            {
                // TCP-1.0.2: Load image safely
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Prevents file lock!
                bitmap.UriSource = new System.Uri(dialog.FileName);
                bitmap.EndInit();
                bitmap.Freeze(); // Thread-safe
                
                // TCP-1.0.2: Set properties
                BackgroundImage = bitmap;
                BackgroundImageName = System.IO.Path.GetFileName(dialog.FileName);
                
                // TCP-1.0.2: Show success toast
                NotificationService.Instance.ShowSuccess("Background image loaded", $"Loaded: {BackgroundImageName}");
            }
        }
        catch (Exception ex)
        {
            // TCP-1.0.2: Show error toast (no crash)
            NotificationService.Instance.ShowError("Failed to load image", ex.Message);
        }
    }
    
    /// <summary>
    /// Set Fit Mode command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private void SetFitMode()
    {
        try
        {
            ImageMode = EditorImageMode.Fit;
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions
        }
    }
    
    /// <summary>
    /// Set Actual Mode command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private void SetActualMode()
    {
        try
        {
            ImageMode = EditorImageMode.Actual;
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions
        }
    }
    
    /// <summary>
    /// Clear Image command implementation
    /// TCP-1.0.2: Background Image Load (Editor)
    /// </summary>
    private void ClearImage()
    {
        try
        {
            BackgroundImage = null;
            BackgroundImageName = null;
            
            // TCP-1.0.2: Show toast
            NotificationService.Instance.ShowInfo("Background image cleared", "Image removed from editor");
        }
        catch
        {
            // TCP-1.0.2: Safety - ignore exceptions
        }
    }
}

