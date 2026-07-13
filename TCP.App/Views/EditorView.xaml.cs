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

    private bool _isDragging;
    private System.Windows.Point _clickPosition;

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel vm && vm.IsLocked) return;

        _isDragging = true;
        var draggableControl = sender as System.Windows.FrameworkElement;
        _clickPosition = e.GetPosition(this);
        draggableControl?.CaptureMouse();
        e.Handled = true; // Prevent viewport pan
    }

    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging) return;

        var draggableControl = sender as System.Windows.FrameworkElement;
        var currentPosition = e.GetPosition(this);

        var transformX = currentPosition.X - _clickPosition.X;
        var transformY = currentPosition.Y - _clickPosition.Y;

        var currentLeft = Canvas.GetLeft(ImageContainer);
        var currentTop = Canvas.GetTop(ImageContainer);

        if (double.IsNaN(currentLeft)) currentLeft = 0;
        if (double.IsNaN(currentTop)) currentTop = 0;

        Canvas.SetLeft(ImageContainer, currentLeft + transformX);
        Canvas.SetTop(ImageContainer, currentTop + transformY);

        _clickPosition = currentPosition;
        e.Handled = true;
    }

    private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging) return;
        _isDragging = false;
        var draggableControl = sender as System.Windows.FrameworkElement;
        draggableControl?.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void ThumbTL_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        var currentLeft = Canvas.GetLeft(ImageContainer);
        var currentTop = Canvas.GetTop(ImageContainer);
        
        if (double.IsNaN(currentLeft)) currentLeft = 0;
        if (double.IsNaN(currentTop)) currentTop = 0;
        
        double newWidth = ImageContainer.Width - e.HorizontalChange;
        double newHeight = ImageContainer.Height - e.VerticalChange;

        if (newWidth > 50)
        {
            ImageContainer.Width = newWidth;
            Canvas.SetLeft(ImageContainer, currentLeft + e.HorizontalChange);
        }
        if (newHeight > 50)
        {
            ImageContainer.Height = newHeight;
            Canvas.SetTop(ImageContainer, currentTop + e.VerticalChange);
        }
    }

    private void ThumbTR_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        var currentTop = Canvas.GetTop(ImageContainer);
        if (double.IsNaN(currentTop)) currentTop = 0;

        double newWidth = ImageContainer.Width + e.HorizontalChange;
        double newHeight = ImageContainer.Height - e.VerticalChange;

        if (newWidth > 50) ImageContainer.Width = newWidth;
        if (newHeight > 50)
        {
            ImageContainer.Height = newHeight;
            Canvas.SetTop(ImageContainer, currentTop + e.VerticalChange);
        }
    }

    private void ThumbBL_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        var currentLeft = Canvas.GetLeft(ImageContainer);
        if (double.IsNaN(currentLeft)) currentLeft = 0;

        double newWidth = ImageContainer.Width - e.HorizontalChange;
        double newHeight = ImageContainer.Height + e.VerticalChange;

        if (newWidth > 50)
        {
            ImageContainer.Width = newWidth;
            Canvas.SetLeft(ImageContainer, currentLeft + e.HorizontalChange);
        }
        if (newHeight > 50) ImageContainer.Height = newHeight;
    }

    private void ThumbBR_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        double newWidth = ImageContainer.Width + e.HorizontalChange;
        double newHeight = ImageContainer.Height + e.VerticalChange;

        if (newWidth > 50) ImageContainer.Width = newWidth;
        if (newHeight > 50) ImageContainer.Height = newHeight;
    }

    private bool _isBoxDragging;
    private System.Windows.Point _boxClickPosition;
    private TCP.App.Models.Electronics.DeviceInstance? _draggedBoxData;

    private void Box_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Electronics.DeviceInstance device)
        {
            if (device.IsLocked) return;

            _isBoxDragging = true;
            _draggedBoxData = device;
            _boxClickPosition = e.GetPosition(ViewportCanvas);
            el.CaptureMouse();
            e.Handled = true;
        }
    }

    private void Box_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isBoxDragging || _draggedBoxData == null || sender is not System.Windows.FrameworkElement el) return;

        var currentPosition = e.GetPosition(ViewportCanvas);
        var transformX = currentPosition.X - _boxClickPosition.X;
        var transformY = currentPosition.Y - _boxClickPosition.Y;

        _draggedBoxData.X += transformX;
        _draggedBoxData.Y += transformY;

        System.Windows.DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(el);
        while (parent != null && !(parent is ContentPresenter))
        {
            parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
        }
        
        if (parent is ContentPresenter cp)
        {
            Canvas.SetLeft(cp, _draggedBoxData.X);
            Canvas.SetTop(cp, _draggedBoxData.Y);
        }

        _boxClickPosition = currentPosition;
        e.Handled = true;
    }

    private void Box_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isBoxDragging) return;
        _isBoxDragging = false;
        
        if (sender is System.Windows.FrameworkElement el)
        {
            el.ReleaseMouseCapture();
        }
        
        TCP.App.Services.DeviceManager.Instance.SaveDevices();
        e.Handled = true;
    }
}

