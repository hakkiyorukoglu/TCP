using System.Configuration;
using System.Data;
using System.Windows;

namespace TCP.App;

/// <summary>
/// Interaction logic for App.xaml
/// 
/// TCP-0.4: Clean startup - NO custom logic in App.xaml.cs
/// Startup stability için minimal code-behind.
/// </summary>
public partial class App : Application
{
    // TCP-0.4: Startup stability için OnStartup override'ı kaldırıldı
    // Tüm başlatma mantığı MainWindow constructor'ında yapılacak
}

