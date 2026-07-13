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
        else if (_item is DeviceInstance dev)
        {
            NameTextBox.Text = dev.CustomName;
            LocationTextBox.Text = dev.Location;
            IpTextBox.Text = dev.IpAddress;
            PortTextBox.Text = dev.Port.ToString();
            MacTextBox.Text = dev.MacAddress;
            LanCableTextBox.Text = dev.LanCable;
            LocationPanel.Visibility = Visibility.Visible;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_item is EditorImage img)
        {
            img.Name = NameTextBox.Text;
        }
        else if (_item is DeviceInstance dev)
        {
            dev.CustomName = NameTextBox.Text;
            dev.Location = LocationTextBox.Text;
            dev.IpAddress = IpTextBox.Text;
            if (int.TryParse(PortTextBox.Text, out int port)) dev.Port = port;
            dev.MacAddress = MacTextBox.Text;
            dev.LanCable = LanCableTextBox.Text;
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
