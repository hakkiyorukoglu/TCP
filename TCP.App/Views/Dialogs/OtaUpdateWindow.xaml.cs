using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using TCP.App.Models.Electronics;
using TCP.App.Services;

namespace TCP.App.Views.Dialogs;

public partial class OtaUpdateWindow : Window
{
    private readonly StationInstance _station;
    private string _selectedFilePath = string.Empty;

    public OtaUpdateWindow(StationInstance station)
    {
        InitializeComponent();
        _station = station ?? throw new ArgumentNullException(nameof(station));
        
        StationNameText.Text = _station.Name;
        IpAddressText.Text = $"http://{_station.IpAddress}";
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Firmware Dosyası Seçin",
            Filter = "Binary Files (*.bin)|*.bin|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        if (openFileDialog.ShowDialog() == true)
        {
            _selectedFilePath = openFileDialog.FileName;
            FilePathTextBox.Text = _selectedFilePath;
            UploadButton.IsEnabled = true;
            StatusText.Text = "Dosya seçildi. Yüklemeye hazır.";
            StatusText.Foreground = (System.Windows.Media.Brush)FindResource("Brush.Text.Primary");
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void Upload_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
        {
            MessageBox.Show("Geçerli bir dosya seçilmedi!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        UploadButton.IsEnabled = false;
        FilePathTextBox.IsEnabled = false;
        
        StatusText.Text = "Bağlanıyor ve dosya yükleniyor, lütfen bekleyin...";
        StatusText.Foreground = (System.Windows.Media.Brush)FindResource("Brush.Palette.Orange");
        UploadProgress.Value = 0;

        // Progress reporter
        var progress = new Progress<int>(percent =>
        {
            UploadProgress.Value = percent;
            if (percent == 100)
            {
                StatusText.Text = "Yükleme tamamlandı! Cihaz yeniden başlatılıyor.";
                StatusText.Foreground = (System.Windows.Media.Brush)FindResource("Brush.Palette.Green");
            }
        });

        bool success = await Esp32HttpClient.Instance.UploadOtaFirmwareAsync(_station.IpAddress, _selectedFilePath, progress);

        if (success)
        {
            MessageBox.Show("Yazılım başarıyla yüklendi!\nİstasyon şimdi yeniden başlayacak.", "OTA Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        else
        {
            StatusText.Text = "Yükleme sırasında hata oluştu!";
            StatusText.Foreground = (System.Windows.Media.Brush)FindResource("Brush.Palette.Red");
            UploadProgress.Value = 0;
            MessageBox.Show("Yükleme işlemi başarısız oldu. Terminal loglarını veya IP adresini kontrol edin.", "OTA Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            
            UploadButton.IsEnabled = true;
            FilePathTextBox.IsEnabled = true;
        }
    }
}
