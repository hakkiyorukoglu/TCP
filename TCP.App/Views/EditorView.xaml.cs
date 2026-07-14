using System;
using System.Linq;
using System.Windows;
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
            var pos = e.GetPosition((IInputElement)sender);
            viewModel.InputRouter.HandleMouseWheel(e, pos.X, pos.Y);
        }
    }
    
    private (double X, double Y, bool Magnetized) GetSnappedPosition(System.Windows.Point rawPos, EditorViewModel viewModel)
    {
        if (viewModel.IsOrthogonalMode && viewModel.IsDrawingRoute && viewModel.CurrentRoute != null && viewModel.CurrentRoute.Nodes.Any())
        {
            var lastNode = viewModel.CurrentRoute.Nodes.Last();
            double dx = Math.Abs(rawPos.X - lastNode.X);
            double dy = Math.Abs(rawPos.Y - lastNode.Y);
            if (dx > dy)
            {
                rawPos.Y = lastNode.Y;
            }
            else
            {
                rawPos.X = lastNode.X;
            }
        }

        double targetX = rawPos.X;
        double targetY = rawPos.Y;
        
        // Magnet logic: Snap to existing route nodes if close enough
        double snapRadius = 15.0;
        bool magnetized = false;
        
        foreach (var route in viewModel.Routes)
        {
            foreach (var node in route.Nodes)
            {
                double dx = rawPos.X - node.X;
                double dy = rawPos.Y - node.Y;
                if (Math.Sqrt(dx * dx + dy * dy) <= snapRadius)
                {
                    targetX = node.X;
                    targetY = node.Y;
                    magnetized = true;
                    break;
                }
            }
            if (magnetized) break;
        }
        
        // Grid snap if not magnetized
        if (!magnetized)
        {
            double gridSize = 20.0;
            targetX = Math.Round(rawPos.X / gridSize) * gridSize;
            targetY = Math.Round(rawPos.Y / gridSize) * gridSize;
        }

        return (targetX, targetY, magnetized);
    }

    /// <summary>
    /// Mouse down handler - Start pan or draw route
    /// TCP-1.0.4: Pan functionality
    /// </summary>
    private bool _isSelecting = false;
    private System.Windows.Point _selectionStartPoint;

    private void ViewportContainer_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                viewModel.InputRouter.HandleMouseDown(e);
            }
            else if (e.LeftButton == MouseButtonState.Pressed && viewModel.IsDrawingRoute)
            {
                var pos = e.GetPosition(ViewportCanvas);
                var snapped = GetSnappedPosition(pos, viewModel);

                viewModel.AddRouteNode(snapped.X, snapped.Y);
                e.Handled = true;
            }
            else if (e.LeftButton == MouseButtonState.Pressed && viewModel.IsDrawingLogicalRoute)
            {
                var pos = e.GetPosition(ViewportCanvas);
                var snapped = GetSnappedPosition(pos, viewModel);
                
                if (snapped.Magnetized)
                {
                    TCP.App.Models.Electronics.TrackNode? snappedNode = null;
                    foreach(var route in viewModel.Routes)
                    {
                        // Note: Using a small tolerance for floating point comparisons just in case, though they should be exact
                        snappedNode = route.Nodes.FirstOrDefault(n => Math.Abs(n.X - snapped.X) < 0.1 && Math.Abs(n.Y - snapped.Y) < 0.1);
                        if (snappedNode != null) break;
                    }
                    
                    if (snappedNode != null)
                    {
                        viewModel.AddLogicalRouteNode(snappedNode);
                    }
                }
                e.Handled = true;
            }
            else if (e.LeftButton == MouseButtonState.Pressed && viewModel.IsSelectionMode)
            {
                _isSelecting = true;
                _selectionStartPoint = e.GetPosition(ViewportCanvas);
                Canvas.SetLeft(SelectionRectangle, _selectionStartPoint.X);
                Canvas.SetTop(SelectionRectangle, _selectionStartPoint.Y);
                SelectionRectangle.Width = 0;
                SelectionRectangle.Height = 0;
                SelectionRectangle.Visibility = System.Windows.Visibility.Visible;
                e.Handled = true;
                if (sender is IInputElement inputElement) inputElement.CaptureMouse();
                
                // Clear existing selections if not holding shift/ctrl
                if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == ModifierKeys.None)
                {
                    foreach (var img in viewModel.EditorImages) img.IsSelected = false;
                    foreach (var box in viewModel.PlacedBoxes) box.IsSelected = false;
                    foreach (var route in viewModel.Routes) route.IsSelected = false;
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Unselect all if click empty space in normal mode
                foreach (var img in viewModel.EditorImages) img.IsSelected = false;
                foreach (var box in viewModel.PlacedBoxes) box.IsSelected = false;
                foreach (var route in viewModel.Routes) route.IsSelected = false;
            }
        }
    }
    
    private void ViewportContainer_MouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            viewModel.InputRouter.HandleMouseMove(e);
            
            if (viewModel.IsDrawingRoute || viewModel.IsDrawingLogicalRoute)
            {
                var pos = e.GetPosition(ViewportCanvas);
                var snapped = GetSnappedPosition(pos, viewModel);
                
                Canvas.SetLeft(SnapIndicator, snapped.X - SnapIndicator.Width / 2);
                Canvas.SetTop(SnapIndicator, snapped.Y - SnapIndicator.Height / 2);
                SnapIndicator.Visibility = System.Windows.Visibility.Visible;
                
                if (snapped.Magnetized)
                {
                    SnapIndicator.Fill = System.Windows.Media.Brushes.Cyan;
                    SnapIndicator.Opacity = 0.9;
                }
                else
                {
                    SnapIndicator.Fill = System.Windows.Media.Brushes.White;
                    SnapIndicator.Opacity = 0.5;
                }

                // Update DrawingPreviewLine
                if (viewModel.IsDrawingRoute && viewModel.CurrentRoute != null && viewModel.CurrentRoute.Nodes.Any())
                {
                    var lastNode = viewModel.CurrentRoute.Nodes.Last();
                    DrawingPreviewLine.X1 = lastNode.X;
                    DrawingPreviewLine.Y1 = lastNode.Y;
                    DrawingPreviewLine.X2 = snapped.X;
                    DrawingPreviewLine.Y2 = snapped.Y;
                    DrawingPreviewLine.Visibility = System.Windows.Visibility.Visible;
                }
                else if (viewModel.IsDrawingLogicalRoute && viewModel.CurrentLogicalRoute != null && viewModel.CurrentLogicalRoute.Nodes.Any())
                {
                    var lastNode = viewModel.CurrentLogicalRoute.Nodes.Last();
                    DrawingPreviewLine.X1 = lastNode.X;
                    DrawingPreviewLine.Y1 = lastNode.Y;
                    DrawingPreviewLine.X2 = snapped.X;
                    DrawingPreviewLine.Y2 = snapped.Y;
                    DrawingPreviewLine.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    DrawingPreviewLine.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                SnapIndicator.Visibility = System.Windows.Visibility.Collapsed;
                DrawingPreviewLine.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (_isSelecting && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(ViewportCanvas);
                
                var x = Math.Min(pos.X, _selectionStartPoint.X);
                var y = Math.Min(pos.Y, _selectionStartPoint.Y);
                var width = Math.Max(pos.X, _selectionStartPoint.X) - x;
                var height = Math.Max(pos.Y, _selectionStartPoint.Y) - y;
                
                Canvas.SetLeft(SelectionRectangle, x);
                Canvas.SetTop(SelectionRectangle, y);
                SelectionRectangle.Width = width;
                SelectionRectangle.Height = height;
                e.Handled = true;
            }
        }
    }
    
    private void ViewportContainer_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            if (viewModel.IsDrawingRoute)
            {
                viewModel.IsDrawingRoute = false;
                DrawingPreviewLine.Visibility = System.Windows.Visibility.Collapsed;
                e.Handled = true; // Prevent context menu
            }
            else if (viewModel.IsDrawingLogicalRoute)
            {
                viewModel.IsDrawingLogicalRoute = false;
                DrawingPreviewLine.Visibility = System.Windows.Visibility.Collapsed;
                e.Handled = true;
            }
        }
    }

    private void ViewportContainer_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            viewModel.InputRouter.HandleMouseUp(e);

            if (_isSelecting)
            {
                _isSelecting = false;
                if (sender is IInputElement inputElement) inputElement.ReleaseMouseCapture();
                SelectionRectangle.Visibility = System.Windows.Visibility.Collapsed;
                
                // Perform intersection logic
                var rectX = Canvas.GetLeft(SelectionRectangle);
                var rectY = Canvas.GetTop(SelectionRectangle);
                var rect = new System.Windows.Rect(rectX, rectY, SelectionRectangle.Width, SelectionRectangle.Height);
                
                // Select Images
                foreach(var img in viewModel.EditorImages)
                {
                    var imgRect = new System.Windows.Rect(img.X, img.Y, img.Width, img.Height);
                    if (rect.IntersectsWith(imgRect)) img.IsSelected = true;
                }
                
                // Select Boxes
                foreach(var box in viewModel.PlacedBoxes)
                {
                    // MainPcInstance, ModemInstance etc are 120x60 mostly, but let's use a default size if no width is present.
                    // However box is EditorComponentViewModel which has no Width. Let's use a default size 80x80 for bounding box
                    var approxBoxRect = new System.Windows.Rect(box.X - 40, box.Y - 40, 80, 80);
                    if (rect.IntersectsWith(approxBoxRect)) box.IsSelected = true;
                }

                // Select Routes (check if any node is inside)
                foreach(var route in viewModel.Routes)
                {
                    bool inside = false;
                    foreach(var node in route.Nodes)
                    {
                        if (rect.Contains(new System.Windows.Point(node.X, node.Y)))
                        {
                            inside = true;
                            break;
                        }
                    }
                    if (inside) route.IsSelected = true;
                }

                e.Handled = true;
            }
        }
    }

    private bool _isDragging;
    private System.Windows.Point _clickPosition;

    private TCP.App.Models.Editor.EditorImage? _draggedImage;

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel && viewModel.IsLiveMode) return;

        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Editor.EditorImage image)
        {
            if (image.IsLocked) return;

            if (DataContext is EditorViewModel vm)
            {
                vm.SelectImageCommand.Execute(image);
            }

            _isDragging = true;
            _draggedImage = image;
            _clickPosition = e.GetPosition(ViewportCanvas);
            el.CaptureMouse();
            e.Handled = true; // Prevent viewport pan
        }
    }

    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging || _draggedImage == null || sender is not System.Windows.FrameworkElement) return;

        var currentPosition = e.GetPosition(ViewportCanvas);
        var transformX = currentPosition.X - _clickPosition.X;
        var transformY = currentPosition.Y - _clickPosition.Y;

        _draggedImage.X += transformX;
        _draggedImage.Y += transformY;

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
        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Editor.EditorImage image)
        {
            double newWidth = image.Width - e.HorizontalChange;
            double newHeight = image.Height - e.VerticalChange;

            if (newWidth > 50)
            {
                image.Width = newWidth;
                image.X += e.HorizontalChange;
            }
            if (newHeight > 50)
            {
                image.Height = newHeight;
                image.Y += e.VerticalChange;
            }
        }
    }

    private void ThumbTR_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Editor.EditorImage image)
        {
            double newWidth = image.Width + e.HorizontalChange;
            double newHeight = image.Height - e.VerticalChange;

            if (newWidth > 50) image.Width = newWidth;
            if (newHeight > 50)
            {
                image.Height = newHeight;
                image.Y += e.VerticalChange;
            }
        }
    }

    private void ThumbBL_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Editor.EditorImage image)
        {
            double newWidth = image.Width - e.HorizontalChange;
            double newHeight = image.Height + e.VerticalChange;

            if (newWidth > 50)
            {
                image.Width = newWidth;
                image.X += e.HorizontalChange;
            }
            if (newHeight > 50) image.Height = newHeight;
        }
    }

    private void ThumbBR_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Editor.EditorImage image)
        {
            double newWidth = image.Width + e.HorizontalChange;
            double newHeight = image.Height + e.VerticalChange;

            if (newWidth > 50) image.Width = newWidth;
            if (newHeight > 50) image.Height = newHeight;
        }
    }

    private bool _isBoxDragging;
    private System.Windows.Point _boxClickPosition;
    private object? _draggedBoxData;

    private void Box_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        bool isLiveMode = DataContext is EditorViewModel vm && vm.IsLiveMode;

        if (sender is System.Windows.FrameworkElement el)
        {
            if (DataContext is EditorViewModel viewModel && el.DataContext is TCP.App.Models.Editor.ILayerItem layerItem)
            {
                viewModel.SelectedLayerItem = layerItem;
            }

            if (el.DataContext is TCP.App.Models.Electronics.RfidTagInstance rfidTag)
            {
                if (rfidTag.IsLocked) return;
                _draggedBoxData = rfidTag;
            }
            else
            {
                if (isLiveMode) return;

                if (el.DataContext is TCP.App.Models.Electronics.MainPcInstance pc)
                {
                    if (pc.IsLocked) return;
                    _draggedBoxData = pc;
                }
                else if (el.DataContext is TCP.App.Models.Electronics.ModemInstance modem)
                {
                    if (modem.IsLocked) return;
                    _draggedBoxData = modem;
                }
                else if (el.DataContext is TCP.App.Models.Electronics.StationInstance station)
                {
                    if (station.IsLocked) return;
                    _draggedBoxData = station;
                }
                else if (el.DataContext is TCP.App.Models.Electronics.ComponentInstance component)
                {
                    if (component.IsLocked) return;
                    _draggedBoxData = component;
                }
                else return;
            }

            _isBoxDragging = true;
            _boxClickPosition = e.GetPosition(ViewportCanvas);
            el.CaptureMouse();
            e.Handled = true;
        }
    }

    private void Box_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isBoxDragging || _draggedBoxData == null || sender is not System.Windows.FrameworkElement el) return;

        var currentPosition = e.GetPosition(ViewportCanvas);
        var transformX = currentPosition.X - _boxClickPosition.X;
        var transformY = currentPosition.Y - _boxClickPosition.Y;

        double newX = 0, newY = 0;

        if (_draggedBoxData is TCP.App.Models.Electronics.MainPcInstance pc)
        {
            pc.X += transformX;
            pc.Y += transformY;
            newX = pc.X;
            newY = pc.Y;
        }
        else if (_draggedBoxData is TCP.App.Models.Electronics.ModemInstance modem)
        {
            modem.X += transformX;
            modem.Y += transformY;
            newX = modem.X;
            newY = modem.Y;
        }
        else if (_draggedBoxData is TCP.App.Models.Electronics.StationInstance station)
        {
            station.X += transformX;
            station.Y += transformY;
            newX = station.X;
            newY = station.Y;
        }
        else if (_draggedBoxData is TCP.App.Models.Electronics.ComponentInstance component)
        {
            component.X += transformX;
            component.Y += transformY;
            newX = component.X;
            newY = component.Y;
        }
        else if (_draggedBoxData is TCP.App.Models.Electronics.RfidTagInstance rfidTag)
        {
            rfidTag.X += transformX;
            rfidTag.Y += transformY;
            newX = rfidTag.X;
            newY = rfidTag.Y;
        }

        System.Windows.DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(el);
        while (parent != null && !(parent is System.Windows.Controls.ContentPresenter))
        {
            parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
        }
        
        if (parent is System.Windows.Controls.ContentPresenter cp)
        {
            System.Windows.Controls.Canvas.SetLeft(cp, newX);
            System.Windows.Controls.Canvas.SetTop(cp, newY);
        }

        _boxClickPosition = currentPosition;
        e.Handled = true;
    }

    private void Box_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (!_isBoxDragging) return;
        _isBoxDragging = false;
        
        if (sender is System.Windows.FrameworkElement el)
        {
            el.ReleaseMouseCapture();
        }
        
        if (DataContext is EditorViewModel vm)
        {
            if (vm.IsLiveMode && _draggedBoxData is TCP.App.Models.Electronics.RfidTagInstance rfid)
            {
                // Check intersection with any RFID Reader
                var readers = vm.PlacedBoxes.OfType<TCP.App.Models.Electronics.ComponentInstance>().Where(c => c.TemplateId.Contains("RFID", StringComparison.OrdinalIgnoreCase));
                foreach (var reader in readers)
                {
                    // Simple distance check
                    double dx = reader.X - rfid.X;
                    double dy = reader.Y - rfid.Y;
                    if (Math.Sqrt(dx * dx + dy * dy) < 80.0) // 80 units radius for easier drop
                    {
                        vm.OnRfidScanned(rfid.Uid);
                        break;
                    }
                }
            }

            // Grid Snapping for dropped components
            if (_draggedBoxData != null && !vm.IsLiveMode)
            {
                double gridSize = 20.0;
                if (_draggedBoxData is TCP.App.Models.Electronics.MainPcInstance pc)
                {
                    pc.X = Math.Round(pc.X / gridSize) * gridSize;
                    pc.Y = Math.Round(pc.Y / gridSize) * gridSize;
                }
                else if (_draggedBoxData is TCP.App.Models.Electronics.ModemInstance modem)
                {
                    modem.X = Math.Round(modem.X / gridSize) * gridSize;
                    modem.Y = Math.Round(modem.Y / gridSize) * gridSize;
                }
                else if (_draggedBoxData is TCP.App.Models.Electronics.StationInstance station)
                {
                    station.X = Math.Round(station.X / gridSize) * gridSize;
                    station.Y = Math.Round(station.Y / gridSize) * gridSize;
                }
                else if (_draggedBoxData is TCP.App.Models.Electronics.ComponentInstance component)
                {
                    component.X = Math.Round(component.X / gridSize) * gridSize;
                    component.Y = Math.Round(component.Y / gridSize) * gridSize;
                }
                else if (_draggedBoxData is TCP.App.Models.Electronics.RfidTagInstance rfidTag)
                {
                    rfidTag.X = Math.Round(rfidTag.X / gridSize) * gridSize;
                    rfidTag.Y = Math.Round(rfidTag.Y / gridSize) * gridSize;
                }
            }
        }

        TCP.App.Services.NetworkManager.Instance.SaveNetwork();
        e.Handled = true;
    }

    private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is EditorViewModel viewModel && e.NewValue is TCP.App.Models.Editor.ILayerItem layerItem)
        {
            viewModel.SelectedLayerItem = layerItem;
        }
    }

    private bool _isDraggingNode;
    private TCP.App.Models.Electronics.TrackNode? _draggedNode;

    private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            if (viewModel.IsLiveMode) return;
            
            if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Electronics.TrackNode node)
            {
                if (viewModel.IsDrawingLogicalRoute)
                {
                    viewModel.AddLogicalRouteNode(node);
                    e.Handled = true;
                    return;
                }

                _isDraggingNode = true;
                _draggedNode = node;
                _clickPosition = e.GetPosition(ViewportCanvas);
                el.CaptureMouse();
                e.Handled = true;
            }
        }
    }

    private void Node_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDraggingNode || _draggedNode == null || sender is not System.Windows.FrameworkElement) return;

        var currentPosition = e.GetPosition(ViewportCanvas);
        var transformX = currentPosition.X - _clickPosition.X;
        var transformY = currentPosition.Y - _clickPosition.Y;

        _draggedNode.X += transformX;
        _draggedNode.Y += transformY;

        if (DataContext is EditorViewModel viewModel)
        {
            var parentRoute = viewModel.Routes.FirstOrDefault(r => r.Nodes.Contains(_draggedNode));
            parentRoute?.ForceUpdatePaths();
        }

        _clickPosition = currentPosition;
        e.Handled = true;
    }

    private void Node_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDraggingNode) return;
        _isDraggingNode = false;
        var el = sender as System.Windows.FrameworkElement;
        el?.ReleaseMouseCapture();
        e.Handled = true;
        
        if (DataContext is EditorViewModel viewModel && _draggedNode != null)
        {
            var snapped = GetSnappedPosition(new System.Windows.Point(_draggedNode.X, _draggedNode.Y), viewModel);
            _draggedNode.X = snapped.X;
            _draggedNode.Y = snapped.Y;
            
            var parentRoute = viewModel.Routes.FirstOrDefault(r => r.Nodes.Contains(_draggedNode));
            parentRoute?.ForceUpdatePaths();
            TCP.App.Services.NetworkManager.Instance.SaveNetwork();
        }
        _draggedNode = null;
    }

    private void Route_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.FrameworkElement el && el.DataContext is TCP.App.Models.Editor.ILayerItem layerItem)
        {
            if (DataContext is EditorViewModel viewModel)
            {
                viewModel.SelectedLayerItem = layerItem;
            }
        }
    }
}
