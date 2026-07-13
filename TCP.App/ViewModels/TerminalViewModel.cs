using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TCP.App.Models;
using TCP.App.Services;

namespace TCP.App.ViewModels
{
    public enum TerminalViewState
    {
        Minimized,
        Normal,
        Expanded
    }

    public class TerminalViewModel : INotifyPropertyChanged
    {
        private string _inputText = string.Empty;
        private TerminalViewState _viewState = TerminalViewState.Normal;
        private bool _isVisible = true;

        public ObservableCollection<TerminalMessage> Messages => TerminalService.Instance.Messages;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }

        public TerminalViewState ViewState
        {
            get => _viewState;
            set
            {
                if (_viewState != value)
                {
                    _viewState = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsExpanded));
                    OnPropertyChanged(nameof(IsMinimized));
                    OnPropertyChanged(nameof(IsNormal));
                }
            }
        }

        public bool IsExpanded => ViewState == TerminalViewState.Expanded;
        public bool IsMinimized => ViewState == TerminalViewState.Minimized;
        public bool IsNormal => ViewState == TerminalViewState.Normal;

        // Command to execute input
        public ICommand ExecuteCommand { get; }

        public TerminalViewModel()
        {
            ExecuteCommand = new RelayCommand<object>(Execute);
            
            // Read initial state
            var settings = App.LoadedSettings ?? new AppSettings();
            IsVisible = settings.ShowTerminal;

            // Listen for changes
            TerminalService.Instance.VisibilityChanged += (visible) => 
            {
                IsVisible = visible;
            };
        }

        private void Execute(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(InputText)) return;
            
            TerminalService.Instance.ExecuteCommand(InputText);
            InputText = string.Empty;
        }

        // State change commands
        public void SetState(TerminalViewState state)
        {
            ViewState = state;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
