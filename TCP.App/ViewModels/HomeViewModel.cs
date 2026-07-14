using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TCP.App.Data.Entities;
using TCP.App.Services;
using TCP.App.Models.Electronics;

namespace TCP.App.ViewModels;

public class HomeViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<ScenarioDb> Scenarios { get; } = new();
    
    // Expose Live Network Data for Monitoring
    public ObservableCollection<ModemInstance> Modems => NetworkManager.Instance.Modems;

    private string _newScenarioName = string.Empty;
    public string NewScenarioName
    {
        get => _newScenarioName;
        set
        {
            if (_newScenarioName != value)
            {
                _newScenarioName = value;
                OnPropertyChanged();
                (CreateScenarioCommand as RelayCommandWithCanExecute<object>)?.RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand LoadScenarioCommand { get; }
    public ICommand DeleteScenarioCommand { get; }
    public ICommand CreateScenarioCommand { get; }

    public HomeViewModel()
    {
        LoadScenarioCommand = new RelayCommand<ScenarioDb>(LoadScenario);
        DeleteScenarioCommand = new RelayCommand<ScenarioDb>(DeleteScenario);
        CreateScenarioCommand = new RelayCommandWithCanExecute<object>(
            _ => CreateScenario(),
            () => !string.IsNullOrWhiteSpace(NewScenarioName)
        );

        RefreshScenarios();
    }

    private void RefreshScenarios()
    {
        Scenarios.Clear();
        var all = ProjectManager.Instance.GetAllScenarios();
        foreach (var s in all)
        {
            Scenarios.Add(s);
        }
    }

    private void CreateScenario()
    {
        if (string.IsNullOrWhiteSpace(NewScenarioName)) return;

        ProjectManager.Instance.CreateNewScenario(NewScenarioName);
        NewScenarioName = string.Empty;
        RefreshScenarios();
        
        // Go to editor automatically? We can tell the user they are now editing it.
        TerminalService.Instance.LogSuccess("Yeni senaryo oluşturuldu ve yüklendi. Tasarıma başlayabilirsiniz.");
    }

    private void LoadScenario(ScenarioDb? scenario)
    {
        if (scenario == null) return;
        
        if (ProjectManager.Instance.IsDirty)
        {
            // Simple approach: we could prompt here as well, but MainWindow does it on close.
            // For now, let's just log a warning or overwrite. 
            // In a full app, we would open a MessageBox here too.
        }

        ProjectManager.Instance.LoadScenario(scenario.Id);
        TerminalService.Instance.LogSuccess($"Senaryo '{scenario.Name}' yüklendi.");
    }

    private void DeleteScenario(ScenarioDb? scenario)
    {
        if (scenario == null) return;
        ProjectManager.Instance.DeleteScenario(scenario.Id);
        RefreshScenarios();
    }
}
