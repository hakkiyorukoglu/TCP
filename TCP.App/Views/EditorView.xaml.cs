using System.Windows.Controls;
using System.Windows.Input;
using TCP.App.ViewModels;

namespace TCP.App.Views;

/// <summary>
/// EditorView.xaml.cs - Editor view code-behind
/// 
/// TCP-1.0.2: Background Image Load (Editor)
/// TCP-1.0.4: Background Image Load with Zoom/Pan
/// 
/// MVVM Pattern: Minimal code-behind, mouse event wiring için
/// </summary>
public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Mouse wheel handler - Zoom
    /// TCP-1.0.4: Zoom functionality
    /// </summary>
    private void ViewportContainer_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            viewModel.InputRouter.HandleMouseWheel(e);
        }
    }
    
    /// <summary>
    /// Mouse down handler - Start pan
    /// TCP-1.0.4: Pan functionality
    /// </summary>
    private void ViewportContainer_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            viewModel.InputRouter.HandleMouseDown(e);
        }
    }
    
    /// <summary>
    /// Mouse move handler - Pan
    /// TCP-1.0.4: Pan functionality
    /// </summary>
    private void ViewportContainer_MouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            viewModel.InputRouter.HandleMouseMove(e);
        }
    }
    
    /// <summary>
    /// Mouse up handler - Stop pan
    /// TCP-1.0.4: Pan functionality
    /// </summary>
    private void ViewportContainer_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            viewModel.InputRouter.HandleMouseUp(e);
        }
    }
}

