using System;
using System.Windows;
using TCP.App.Services;

namespace TCP.App.Views.Dialogs;

public partial class CustomCodeWindow : Window
{
    private readonly Guid _targetNodeId;

    public CustomCodeWindow(Guid targetNodeId, string deviceName)
    {
        InitializeComponent();
        _targetNodeId = targetNodeId;
        Owner = Application.Current.MainWindow;
        
        TitleTextBlock.Text = $"Kod Yaz - {deviceName}";
        Title = $"Kod Yaz - {deviceName}";
        
        LoadCode();
    }

    private void LoadCode()
    {
        try
        {
            var code = ProjectManager.Instance.GetCustomCode(_targetNodeId);
            CodeTextBox.Text = code;
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Kod yüklenirken hata: {ex.Message}");
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ProjectManager.Instance.SaveCustomCode(_targetNodeId, CodeTextBox.Text);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Kod kaydedilirken hata: {ex.Message}");
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
