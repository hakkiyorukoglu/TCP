using System.Windows.Controls;
using TCP.App.ViewModels;

namespace TCP.App.Views.Pages;

public partial class SimulationView : UserControl
{
    public SimulationView()
    {
        InitializeComponent();
        DataContext = new SimulationViewModel();
    }
}
