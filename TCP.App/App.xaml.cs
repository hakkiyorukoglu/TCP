using System.Configuration;
using System.Data;
using System.Windows;
using TCP.App.Services;

namespace TCP.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Uygulama başlatıldığında çağrılır
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // TCP-0.3: Theme System v1 değişikliğini kaydet
        var changeTracker = new ChangeTracker();
        changeTracker.RegisterChange(
            category: "UI / Architecture",
            description: "Introduced token-based theme system with Autodesk-inspired dark variant."
        );
    }
}

