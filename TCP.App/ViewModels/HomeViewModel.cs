using System.ComponentModel;
using System.Runtime.CompilerServices;
using TCP.App.Services;

namespace TCP.App.ViewModels;

public class HomeViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public HomeViewModel()
    {
    }
}
