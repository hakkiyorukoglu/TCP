using System.Windows;
using TCP.App.ViewModels;

namespace TCP.App.Views;

/// <summary>
/// EditorView.xaml.cs - Editor view code-behind
/// 
/// TCP-1.0.1: Editor Foundation (Empty Scene)
/// 
/// DataContext safety: Explicitly set DataContext to EditorViewModel if null.
/// </summary>
public partial class EditorView : System.Windows.Controls.UserControl
{
    /// <summary>
    /// Constructor - Initialize DataContext
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// </summary>
    public EditorView()
    {
        InitializeComponent();
        
        // TCP-1.0.1: DataContext safety - Set if null
        if (DataContext == null || !(DataContext is EditorViewModel))
        {
            DataContext = new EditorViewModel();
        }
        
        this.Loaded += EditorView_Loaded;
    }
    
    /// <summary>
    /// Loaded event handler - Ensure DataContext is set
    /// TCP-1.0.1: Editor Foundation (Empty Scene)
    /// </summary>
    private void EditorView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext == null || !(DataContext is EditorViewModel))
        {
            DataContext = new EditorViewModel();
        }
    }
}
