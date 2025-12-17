using System.Windows;
using System.Windows.Input;

namespace TCP.App;

/// <summary>
/// MainWindow.xaml.cs - Ana pencere code-behind
/// 
/// Bu dosya MainWindow'un davranışlarını yönetir:
/// - Window butonları (minimize, maximize/restore, close)
/// - Pencere sürükleme (WindowStyle="None" olduğu için)
/// 
/// ÖNEMLİ: 
/// - UI mantığı burada minimal tutulur
/// - Gelecekte MVVM pattern ile ViewModel'lere taşınabilir
/// - Single Responsibility: Sadece window yönetimi
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Constructor - Pencere başlatma
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        
        // Pencere sürükleme için TopBar'a MouseDown event'i ekle
        // WindowStyle="None" olduğu için manuel sürükleme implementasyonu gerekli
        this.MouseDown += MainWindow_MouseDown;
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
    /// Pencereyi simge durumuna küçültür
    /// </summary>
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    
    /// <summary>
    /// Maximize/Restore butonu click handler
    /// Pencereyi büyütür veya normale döndürür
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
    /// Uygulamayı kapatır
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}