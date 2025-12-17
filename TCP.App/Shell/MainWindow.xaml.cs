using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using TCP.App.Views;
using TCP.App.Services;
using TCP.App.ViewModels;
using TCP.App.Models;

namespace TCP.App;

/// <summary>
/// MainWindow.xaml.cs - Application Shell code-behind
/// 
/// TCP-0.4: Navigation Shell + Stable Startup
/// TCP-0.8.1: Settings Persistence v1 (Local)
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
    /// Mevcut route name (settings persistence için)
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    private string _currentRoute = "Home";
    
    /// <summary>
    /// Constructor - Pencere başlatma
    /// TCP-0.4: Startup stability için minimal logic
    /// TCP-0.8.1: Settings Persistence v1 (Local)
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
        
        // TCP-0.8.0.1: Settings Navigation Enabled
        changeTracker.RegisterChange(
            category: "Navigation",
            description: "Settings route was previously marked as Coming Soon and is now enabled."
        );
        
        // TCP-0.8.1: Settings Persistence v1 (Local)
        changeTracker.RegisterChange(
            category: "Settings / Persistence",
            description: "Introduced local settings persistence. Persisted: Theme, LastRoute, Panel widths. Location: %AppData%/TCP/settings.json"
        );
        
        // TCP-0.8.1: Theme Selection Fix
        changeTracker.RegisterChange(
            category: "Settings / UI",
            description: "Fixed non-working theme selection in Settings > Appearance. Theme can now be selected and applied immediately without app restart."
        );
        
        // Pencere sürükleme için MouseDown event'i ekle
        // WindowStyle="None" olduğu için manuel sürükleme implementasyonu gerekli
        this.MouseDown += MainWindow_MouseDown;
        
        // TCP-0.8.1: Apply loaded settings (LastRoute navigation)
        ApplyLoadedSettings();
        
        // TCP-0.5.1: Search navigation event handler
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.NavigateRequested += MainViewModel_NavigateRequested;
        }
    }
    
    /// <summary>
    /// Loaded settings'i apply et
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// - LastRoute'e navigate et
    /// - Panel widths restore edilecek (gelecekte)
    /// </summary>
    private void ApplyLoadedSettings()
    {
        var settings = App.LoadedSettings;
        if (settings == null)
        {
            // Settings yoksa default: Home
            NavigateToView(new HomeView(), "Home");
            SetActiveTab(HomeTab);
            return;
        }
        
        // LastRoute'e navigate et
        var lastRoute = settings.LastRoute ?? "Home";
        _currentRoute = lastRoute;
        
        switch (lastRoute)
        {
            case "Home":
                NavigateToView(new HomeView(), "Home");
                SetActiveTab(HomeTab);
                break;
            case "Electronics":
                NavigateToView(new ElectronicsView(), "Electronics");
                SetActiveTab(ElectronicsTab);
                break;
            case "Simulation":
                NavigateToView(new SimulationView(), "Simulation");
                SetActiveTab(SimulationTab);
                break;
            case "Editor":
                NavigateToView(new EditorView(), "Editor");
                SetActiveTab(EditorTab);
                break;
            case "Settings":
                NavigateToView(new SettingsView(), "Settings");
                // Settings tab yok, SetActiveTab çağrılmaz
                break;
            default:
                // Unknown route → default: Home
                NavigateToView(new HomeView(), "Home");
                SetActiveTab(HomeTab);
                break;
        }
    }
    
    /// <summary>
    /// Search navigation handler - route'a göre navigate et
    /// TCP-0.5.1: Top-Right Search UI
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// </summary>
    private void MainViewModel_NavigateRequested(string route)
    {
        switch (route)
        {
            case "Home":
                NavigateToView(new HomeView(), "Home");
                SetActiveTab(HomeTab);
                break;
            case "Electronics":
                NavigateToView(new ElectronicsView(), "Electronics");
                SetActiveTab(ElectronicsTab);
                break;
            case "Simulation":
                NavigateToView(new SimulationView(), "Simulation");
                SetActiveTab(SimulationTab);
                break;
            case "Settings":
                var settingsViewFromSearch = new SettingsView();
                // TCP-0.8.1: ThemeApplyRequested event handler bağla
                if (settingsViewFromSearch.DataContext is SettingsViewModel settingsViewModelFromSearch)
                {
                    settingsViewModelFromSearch.ThemeApplyRequested += SettingsViewModel_ThemeApplyRequested;
                }
                NavigateToView(settingsViewFromSearch, "Settings");
                // Note: Settings tab is not in TopBar navigation tabs, so no SetActiveTab call
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
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// </summary>
    private void HomeTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new HomeView(), "Home");
        SetActiveTab(HomeTab);
    }
    
    /// <summary>
    /// Navigation: Electronics tab click handler
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// </summary>
    private void ElectronicsTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new ElectronicsView(), "Electronics");
        SetActiveTab(ElectronicsTab);
    }
    
    /// <summary>
    /// Navigation: Simulation tab click handler
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// </summary>
    private void SimulationTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new SimulationView(), "Simulation");
        SetActiveTab(SimulationTab);
    }
    
    /// <summary>
    /// Navigation: Editor tab click handler
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// </summary>
    private void EditorTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new EditorView(), "Editor");
        SetActiveTab(EditorTab);
    }
    
    /// <summary>
    /// Navigation: Settings icon click handler
    /// TCP-0.8.0.1: Settings Navigation Enabled
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// TCP-0.8.1: Safe Theme Apply with Save Button - ThemeApplyRequested event handler
    /// </summary>
    private void SettingsIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var settingsView = new SettingsView();
        
        // TCP-0.8.1: ThemeApplyRequested event handler bağla
        if (settingsView.DataContext is SettingsViewModel settingsViewModel)
        {
            settingsViewModel.ThemeApplyRequested += SettingsViewModel_ThemeApplyRequested;
        }
        
        NavigateToView(settingsView, "Settings");
        // Note: Settings is not in TopBar navigation tabs, so no SetActiveTab call
    }
    
    /// <summary>
    /// ThemeApplyRequested event handler
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// 
    /// Bu handler:
    /// 1. SettingsView'ı kapatır (Home'a navigate eder)
    /// 2. Dispatcher.Invoke ile theme'i güvenli bir şekilde apply eder
    /// 3. Settings'i persist eder
    /// </summary>
    private void SettingsViewModel_ThemeApplyRequested(string themeName)
    {
        // SettingsView'ı kapat (Home'a navigate et)
        NavigateToView(new HomeView(), "Home");
        SetActiveTab(HomeTab);
        
        // Theme'i güvenli bir şekilde apply et (Dispatcher.Invoke ile)
        // Bu sayede SettingsView kapatıldıktan sonra theme apply edilir
        this.Dispatcher.Invoke(() =>
        {
            try
            {
                ThemeService.ApplyTheme(themeName);
                
                // Settings'i kaydet
                var settings = App.LoadedSettings ?? new AppSettings();
                settings.Theme = themeName;
                App.SettingsService.Save(settings);
                App.UpdateLoadedSettings(settings);
            }
            catch
            {
                // Exception durumunda sessizce fail eder
                // App crash etmez
            }
        });
    }
    
    /// <summary>
    /// View'e navigate et (basit ContentControl switching)
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// </summary>
    private void NavigateToView(UserControl view, string routeName)
    {
        ContentArea.Content = view;
        
        // Route değiştiyse save et
        if (_currentRoute != routeName)
        {
            _currentRoute = routeName;
            SaveCurrentRoute();
        }
    }
    
    /// <summary>
    /// Mevcut route'u settings'e kaydet
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// </summary>
    private void SaveCurrentRoute()
    {
        try
        {
            var settings = App.LoadedSettings ?? new AppSettings();
            settings.LastRoute = _currentRoute;
            
            App.SettingsService.Save(settings);
            
            // App.LoadedSettings cache'ini güncelle
            App.UpdateLoadedSettings(settings);
        }
        catch
        {
            // Save başarısız olursa sessizce fail eder
            // UI'ya exception throw edilmez
        }
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