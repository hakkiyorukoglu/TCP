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
public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
{
    /// <summary>
    /// Mevcut route name (settings persistence için)
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// TCP-1.0.2: Default route changed to Editor
    /// </summary>
    private string _currentRoute = "Editor";
    
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
        
        // TCP-0.8.1: Safe Theme Apply with Save Button
        changeTracker.RegisterChange(
            category: "Settings / Bugfix",
            description: "Fixed StaticResource crash by introducing explicit Apply Theme workflow. Theme change now requires explicit Apply button click and is applied safely after SettingsView is closed."
        );
        
        // TCP-0.8.1 Hotfix-2: Startup Safe Resources
        changeTracker.RegisterChange(
            category: "Startup / Bugfix",
            description: "Fixed StaticResource resolution failure at startup. Theme dictionaries are now loaded AFTER MainWindow is shown (in Loaded event), preventing startup crashes."
        );
        
        // TCP-0.8.1-HOTFIX: Fix StaticResource crash + make theme resources non-fatal
        changeTracker.RegisterChange(
            category: "Startup / Bugfix",
            description: "Fixed StaticResourceExtension crash by: 1) Creating SafeDefaults.xaml with fallback resources, 2) Converting all theme token StaticResource to DynamicResource, 3) Loading SafeDefaults in App.xaml before theme dictionaries. Theme resources are now non-fatal."
        );
        
        // TCP-0.8.2: Shortcuts Map (UI list) + Dark-only theme
        changeTracker.RegisterChange(
            category: "Settings / Feature",
            description: "Added Shortcuts Map UI list in Settings > Shortcuts. Disabled theme switching - app now uses Dark theme only. Theme selector removed from Appearance page."
        );
        
        // TCP-0.9.0: Info Panel v1 + Version History UI
        changeTracker.RegisterChange(
            category: "Info / Feature",
            description: "Added Info panel with version history and topbar navigation. Clicking version text in TopBar opens Info page. Sections: Overview, Architecture, Features, Version History."
        );
        
        // TCP-0.8.1 Hotfix-2: Load theme AFTER window is loaded
        // This prevents StaticResource resolution failures at startup
        this.Loaded += MainWindow_Loaded;
        
        // TCP-0.8.1: Apply loaded settings (LastRoute navigation)
        ApplyLoadedSettings();
        
        // TCP-0.5.1: Search navigation event handler
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.NavigateRequested += MainViewModel_NavigateRequested;
        }
    }
    
    /// <summary>
    /// MainWindow Loaded event handler
    /// TCP-0.8.1 Hotfix-2: Startup Safe Resources
    /// 
    /// Theme'i window gösterildikten SONRA yükler.
    /// Bu sayede StaticResource resolution failure'ı önlenir.
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Theme'i güvenli bir şekilde yükle
        ThemeService.LoadInitialTheme();
        
        // Language'i güvenli bir şekilde yükle
        var lang = App.LoadedSettings?.Language ?? "tr-TR";
        LanguageService.ApplyLanguage(lang);

        // Set App window reference if not already set
        if (Application.Current.MainWindow != this)
        {
            Application.Current.MainWindow = this;
        }

        // TCP-1.0.2: Always start on Home page
        _currentRoute = "Home";
        NavigateToView(new HomeView(), "Home");
        SetActiveTab(HomeTab);
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (ProjectManager.Instance.IsDirty)
        {
            var result = MessageBox.Show(
                "Senaryoda kaydedilmemiş değişiklikler var. Kaydetmek istiyor musunuz?",
                "Uyarı",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                ProjectManager.Instance.SaveScenario();
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
        }
        
        base.OnClosing(e);
    }
    
    /// <summary>
    /// Loaded settings'i apply et
    /// TCP-0.8.1: Settings Persistence v1 (Local)
    /// TCP-0.9.0-fix1: Info access & startup route fix
    /// TCP-1.0.2: Startup route changed to Editor
    /// 
    /// Startup'ta Editor'a navigate eder.
    /// LastRoute artık startup'ta kullanılmıyor (UX fix).
    /// Panel widths restore edilecek (gelecekte)
    /// </summary>
    private void ApplyLoadedSettings()
    {
        // TCP-1.0.2: Always start on Editor page for consistent UX
        // LastRoute is still saved on navigation changes, but not used at startup
        _currentRoute = "Editor";
        NavigateToView(new EditorView(), "Editor");
        SetActiveTab(EditorTab);
    }
    
    /// <summary>
    /// Search navigation handler - route'a göre navigate et
    /// TCP-0.5.1: Top-Right Search UI
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// TCP-1.0.2: Editor route added
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
            case "Editor":
                NavigateToView(new EditorView(), "Editor");
                SetActiveTab(EditorTab);
                break;
            case "Simulation":
                NavigateToView(new TCP.App.Views.Pages.SimulationView(), "Simulation");
                SetActiveTab(SimulationTab);
                break;
            case "Settings":
                var settingsViewFromSearch = new SettingsView();
                NavigateToView(settingsViewFromSearch, "Settings");
                // Note: Settings tab is not in TopBar navigation tabs, so no SetActiveTab call
                break;
            case "Info":
                var infoViewFromSearch = new InfoView();
                NavigateToView(infoViewFromSearch, "Info");
                // Note: Info is not in TopBar navigation tabs, so no SetActiveTab call
                break;
        }
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
    /// Navigation: Editor tab click handler
    /// TCP-1.0.2: Editor reintroduced
    /// </summary>
    private void EditorTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new EditorView(), "Editor");
        SetActiveTab(EditorTab);
    }
    
    /// <summary>
    /// Navigation: Simulation tab click handler
    /// </summary>
    private void SimulationTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        NavigateToView(new TCP.App.Views.Pages.SimulationView(), "Simulation");
        SetActiveTab(SimulationTab);
    }
    
    /// <summary>
    /// Navigation: Save icon click handler
    /// TCP-0.8.3: Save project manually
    /// </summary>
    private void SaveIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ProjectManager.Instance.SaveScenario();
    }

    /// <summary>
    /// Navigation: Settings icon click handler
    /// TCP-0.8.0.1: Settings Navigation Enabled
    /// TCP-0.8.1: Settings Persistence v1 (Local) - Route save edilir
    /// TCP-0.8.2: Theme switching disabled - no ThemeApplyRequested handler needed
    /// </summary>
    private void SettingsIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var settingsView = new SettingsView();
        NavigateToView(settingsView, "Settings");
        // Note: Settings is not in TopBar navigation tabs, so no SetActiveTab call
    }
    
    /// <summary>
    /// Navigation: Version text click handler
    /// TCP-0.9.0: Info Panel v1 + Version History UI
    /// Opens Info page and selects Overview section
    /// </summary>
    private void VersionText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var infoView = new InfoView();
        NavigateToView(infoView, "Info");
        // Note: Info is not in TopBar navigation tabs, so no SetActiveTab call
    }
    
    /// <summary>
    /// Navigation: Info icon click handler
    /// TCP-0.9.0-fix1: Info access & startup route fix
    /// Opens Info page when Info icon is clicked
    /// </summary>
    private void InfoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var infoView = new InfoView();
        NavigateToView(infoView, "Info");
        // Note: Info is not in TopBar navigation tabs, so no SetActiveTab call
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
            
            // We removed the notification here so it doesn't pop up on every tab change
        }
        catch
        {
            // Save başarısız olursa sessizce fail eder
            // UI'ya exception throw edilmez
        }
    }
    
    /// <summary>
    /// Aktif tab'ı set et (visual feedback için)
    /// TCP-1.0.1: Removed Editor/Simulation tabs
    /// TCP-1.0.2: Editor tab added back
    /// </summary>
    private void SetActiveTab(TextBlock activeTab)
    {
        // Tüm tab'ları secondary color'a set et
        HomeTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        ElectronicsTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        EditorTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        SimulationTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Secondary");
        
        // Aktif tab'ı primary color'a set et
        activeTab.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Text.Primary");
    }
    
    /// <summary>
    /// Search ListBox selection handler
    /// TCP-0.5.1: Top-Right Search UI
    /// </summary>
    private void SearchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is SearchItem item)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectSearchItemCommand.Execute(item);
                
                // Clear selection so clicking the same item again works next time
                listBox.SelectedItem = null;
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
    
    /// <summary>
    /// Search TextBox GotFocus handler
    /// TCP-1.0.4: Show all suggestions when focused empty
    /// </summary>
    private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            // Trigger FilterSuggestions to show all items if empty, or filter if not
            // We can just set IsDropdownVisible to true directly if FilteredSuggestions has items
            // but setting SearchText to itself will trigger the setter and FilterSuggestions()
            // To avoid unnecessary property changes, let's call a method or just set IsDropdownVisible
            // Wait, we can't call private FilterSuggestions(). 
            // So we'll just set it to true if we have text, or force a refresh by setting empty to empty
            if (string.IsNullOrEmpty(viewModel.SearchText))
            {
                // Force an update to populate the list
                viewModel.SearchText = " ";
                viewModel.SearchText = "";
            }
            else
            {
                viewModel.IsDropdownVisible = true;
            }
        }
    }
}