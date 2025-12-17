using System.Windows;
using TCP.App.ViewModels;
using TCP.App.Services;

namespace TCP.App.Views;

/// <summary>
/// InfoView.xaml.cs - Info panel view code-behind
/// 
/// TCP-0.9.1a: Info panel DataContext fix
/// 
/// Bu dosya InfoView'un code-behind'idir.
/// DataContext'in her zaman geçerli olduğundan emin olur.
/// 
/// MVVM Pattern: Minimal code-behind, sadece DataContext safety için
/// </summary>
public partial class InfoView : System.Windows.Controls.UserControl
{
    /// <summary>
    /// Constructor - Force DataContext initialization
    /// TCP-0.9.1a: Info panel DataContext fix
    /// 
    /// DataContext'in her zaman geçerli olduğundan emin olur.
    /// Eğer XAML'de set edilmemişse veya null ise, burada set edilir.
    /// </summary>
    public InfoView()
    {
        InitializeComponent();
        
        // TCP-0.9.1a: Force DataContext safety
        // Eğer DataContext null ise veya InfoViewModel değilse, yeni bir tane oluştur
        if (DataContext == null || !(DataContext is InfoViewModel))
        {
            DataContext = new InfoViewModel();
        }
        
        // TCP-0.9.1a: Loaded event'inde de kontrol et (double safety)
        this.Loaded += InfoView_Loaded;
    }
    
    /// <summary>
    /// Loaded event handler - DataContext safety check
    /// TCP-0.9.1a: Info panel DataContext fix
    /// 
    /// View yüklendiğinde DataContext'in geçerli olduğundan emin olur.
    /// </summary>
    private void InfoView_Loaded(object sender, RoutedEventArgs e)
    {
        // TCP-0.9.1a: Double-check DataContext
        if (DataContext == null || !(DataContext is InfoViewModel))
        {
            DataContext = new InfoViewModel();
        }
    }
}
