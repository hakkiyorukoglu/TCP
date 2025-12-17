using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using TCP.App.Views;
using TCP.App.Services;
using TCP.App.ViewModels;

namespace TCP.App;

/// <summary>
/// MainWindow.xaml.cs - Application Shell code-behind
/// 
/// TCP-0.4: Navigation Shell + Stable Startup
/// 
/// Bu dosya MainWindow'un davranışlarını yönetir:
/// - Window butonları (minimize, maximize/restore, close)
/// - Pencere sürükleme (WindowStyle="None" olduğu için)
/// - Navigation (basit View switching)
/// 
/// ÖNEMLİ: 
/// - UI mantığı burada minimal tutulur
/// - Navigation basit ve stable (NO complex router)
/// - Constructor MUST call InitializeComponent() (startup stability)
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Constructor - Pencere başlatma
    /// TCP-0.4: Startup stability için minimal logic
    /// </summary>
    public MainWindow()
    {
        // CRITICAL: InitializeComponent() MUST be called first
        InitializeComponent();
        
        // TCP-0.4: Navigation Shell değişikliğini kaydet
        var changeTracker = new ChangeTracker();
        changeTracker.RegisterChange(
            category: "UI / Architecture",
            description: "Introduced application shell with top navigation and stable startup."
        );
        
        // Pencere sürükleme için MouseDown event'i ekle
        // WindowStyle="None" olduğu için manuel sürükleme implementasyonu gerekli
        this.MouseDown += MainWindow_MouseDown;
        
        // Default view: HomeView (zaten XAML'de set edilmiş)
        // Navigation başlangıçta Home tab'ı aktif göster
        SetActiveTab(HomeTab);
        
        // TCP-0.5.1: Search navigation event handler
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.NavigateRequested += MainViewModel_NavigateRequested;
        }
    }
    
    /// <summary>
    /// Search navigation handler - route'a göre navigate et
    /// TCP-0.5.1: Top-Right Search UI
    /// </summary>
    private void MainViewModel_NavigateRequested(string route)
    {
        switch (route)
        {
            case "Home":
                NavigateToView(new HomeView());
                SetActiveTab(HomeTab);
                break;
            case "Electronics":
                NavigateToView(new ElectronicsView());
                SetActiveTab(ElectronicsTab);
                break;
            case "Simulation":
                NavigateToView(new SimulationView());
                SetActiveTab(SimulationTab);
                break;
            case "Settings":
                // Settings view henüz yok, placeholder
                // NavigateToView(new SettingsView());
                break;
            case "Info":
                // Info view henüz yok, placeholder
                // NavigateToView(new InfoPanel());
                break;
        }
    }
    
    /// <summary>
    /// Pencere sürükleme event handler
    /// TopBar'dan veya boş alanlardan pencereyi sürüklemek için
    /// </summary>
    private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Sadece sol tık ile sürükleme
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
    
    /// <summary>
    /// Minimize butonu click handler
    /// </summary>
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    
    /// <summary>
    /// Maximize/Restore butonu click handler
    /// </summary>
    private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
            MaximizeRestoreButton.Content = "□";
        }
        else
        {
            this.WindowState = WindowState.Maximized;
            MaximizeRestoreButton.Content = "❐";
        }
    }
    
    /// <summary>
    /// Close butonu click handler
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    /// <summary>
    /// Navigation: Home tab click handler
    /// </summary>
    private void HomeTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new HomeView());
        SetActiveTab(HomeTab);
    }
    
    /// <summary>
    /// Navigation: Electronics tab click handler
    /// </summary>
    private void ElectronicsTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new ElectronicsView());
        SetActiveTab(ElectronicsTab);
    }
    
    /// <summary>
    /// Navigation: Simulation tab click handler
    /// </summary>
    private void SimulationTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new SimulationView());
        SetActiveTab(SimulationTab);
    }
    
    /// <summary>
    /// Navigation: Editor tab click handler
    /// </summary>
    private void EditorTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new EditorView());
        SetActiveTab(EditorTab);
    }
    
    /// <summary>
    /// View'e navigate et (basit ContentControl switching)
    /// </summary>
    private void NavigateToView(UserControl view)
    {
        ContentArea.Content = view;
    }
    
    /// <summary>
    /// Aktif tab'ı set et (visual feedback için)
    /// </summary>
    private void SetActiveTab(TextBlock activeTab)
    {
        // Tüm tab'ları secondary color'a set et
        HomeTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        ElectronicsTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        SimulationTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        EditorTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        
        // Aktif tab'ı primary color'a set et
        activeTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Primary");
    }
    
    /// <summary>
    /// Search ListBox mouse click handler
    /// TCP-0.5.1: Top-Right Search UI
    /// </summary>
    private void SearchListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is SearchItem item)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectSearchItemCommand.Execute(item);
            }
        }
    }
    
    /// <summary>
    /// Search ListBox keyboard handler (Arrow keys, Enter, Esc)
    /// TCP-0.5.1: Top-Right Search UI
    /// </summary>
    private void SearchListBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (sender is ListBox listBox && DataContext is MainViewModel viewModel)
        {
            if (e.Key == Key.Enter)
            {
                if (listBox.SelectedItem is SearchItem item)
                {
                    viewModel.SelectSearchItemCommand.Execute(item);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                viewModel.SearchText = string.Empty;
                viewModel.IsDropdownVisible = false;
                e.Handled = true;
            }
            // Arrow keys handled automatically by ListBox
        }
    }
}