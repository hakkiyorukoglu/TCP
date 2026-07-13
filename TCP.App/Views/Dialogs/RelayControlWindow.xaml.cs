using System;
using System.Linq;
using System.Windows;
using TCP.App.Models.Electronics;
using TCP.App.Services;

namespace TCP.App.Views.Dialogs;

public partial class RelayControlWindow : Window
{
    private readonly StationInstance _station;

    public RelayControlWindow(StationInstance station)
    {
        InitializeComponent();
        _station = station ?? throw new ArgumentNullException(nameof(station));
        
        StationNameText.Text = _station.Name;
        IpAddressText.Text = _station.IpAddress;

        // Populate PinComboBox
        var pinOptions = _station.Components.Select(c => new 
        { 
            DisplayName = $"{c.Name} ({c.Pin})", 
            Pin = c.Pin 
        }).ToList();
        
        if (!pinOptions.Any())
        {
            pinOptions.Add(new { DisplayName = "Manuel: 1", Pin = "1" });
            pinOptions.Add(new { DisplayName = "Manuel: 2", Pin = "2" });
            pinOptions.Add(new { DisplayName = "Manuel: 5", Pin = "5" });
        }
        
        PinComboBox.ItemsSource = pinOptions;
        PinComboBox.SelectedIndex = 0;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void Send_Click(object sender, RoutedEventArgs e)
    {
        if (PinComboBox.SelectedValue == null) return;
        
        string pin = PinComboBox.SelectedValue.ToString() ?? "";
        bool state = RadioOn.IsChecked == true;

        var button = sender as Wpf.Ui.Controls.Button;
        if (button != null) button.IsEnabled = false;

        bool success = await Esp32HttpClient.Instance.ToggleRelayAsync(_station.IpAddress, pin, state);

        if (button != null) button.IsEnabled = true;

        if (success)
        {
            MessageBox.Show($"Komut başarıyla gönderildi!\nPin: {pin}, Durum: {(state ? "1" : "0")}", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show($"Komut gönderilemedi. Lütfen bağlantıyı ve IP adresini kontrol edin.\nIP: {_station.IpAddress}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
