using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using TCP.App.Views;
using TCP.App.Services;

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
        HomeTab.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondaryBrush");
        ElectronicsTab.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondaryBrush");
        SimulationTab.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondaryBrush");
        EditorTab.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondaryBrush");
        
        // Aktif tab'ı primary color'a set et
        activeTab.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimaryBrush");
    }
}