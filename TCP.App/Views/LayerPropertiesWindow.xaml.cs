using System.Windows;
using TCP.App.Models.Editor;
using TCP.App.Models.Electronics;

namespace TCP.App.Views;

public partial class LayerPropertiesWindow : Window
{
    private ILayerItem _item;

    public LayerPropertiesWindow(ILayerItem item)
    {
        InitializeComponent();
        _item = item;
        Owner = Application.Current.MainWindow;

        if (_item is EditorImage img)
        {
            NameTextBox.Text = img.Name;
        }
        else if (_item is StationInstance station)
        {
            NameTextBox.Text = station.Name;
            IpTextBox.Text = station.IpAddress;
            PortTextBox.Text = station.Port.ToString();
            MacTextBox.Text = station.MacAddress;
            RouterPortTextBox.Text = station.RouterPort;
            StationPanel.Visibility = Visibility.Visible;
        }
        else if (_item is ComponentInstance component)
        {
            NameTextBox.Text = component.Name;
            PinTextBox.Text = component.Pin;
            ComponentPanel.Visibility = Visibility.Visible;
        }
    }

    private BoardItem? _boardItem;

    public LayerPropertiesWindow(BoardItem boardItem)
    {
        InitializeComponent();
        _boardItem = boardItem;
        Owner = Application.Current.MainWindow;

        NameTextBox.Text = boardItem.Name;
        DescriptionTextBox.Text = boardItem.Description;
        TemplatePanel.Visibility = Visibility.Visible;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_item is EditorImage img)
        {
            img.Name = NameTextBox.Text;
        }
        else if (_item is StationInstance station)
        {
            station.Name = NameTextBox.Text;
            station.IpAddress = IpTextBox.Text;
            if (int.TryParse(PortTextBox.Text, out int port)) station.Port = port;
            station.MacAddress = MacTextBox.Text;
            station.RouterPort = RouterPortTextBox.Text;
        }
        else if (_item is ComponentInstance component)
        {
            component.Name = NameTextBox.Text;
            component.Pin = PinTextBox.Text;
        }
        else if (_boardItem != null)
        {
            _boardItem.Name = NameTextBox.Text;
            _boardItem.Description = DescriptionTextBox.Text;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
