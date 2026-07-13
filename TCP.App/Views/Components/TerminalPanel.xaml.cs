using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCP.App.ViewModels;

namespace TCP.App.Views.Components
{
    public partial class TerminalPanel : UserControl
    {
        public TerminalPanel()
        {
            InitializeComponent();
            this.Loaded += TerminalPanel_Loaded;
        }

        private void TerminalPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TerminalViewModel vm)
            {
                vm.Messages.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Auto scroll to bottom
                if (TerminalOutput.Items.Count > 0)
                {
                    var lastItem = TerminalOutput.Items[TerminalOutput.Items.Count - 1];
                    TerminalOutput.ScrollIntoView(lastItem);
                }
            }
        }

        private void CommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is TerminalViewModel vm && vm.ExecuteCommand.CanExecute(null))
                {
                    vm.ExecuteCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TerminalViewModel vm)
            {
                vm.SetState(TerminalViewState.Minimized);
            }
        }

        private void BtnNormal_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TerminalViewModel vm)
            {
                vm.SetState(TerminalViewState.Normal);
            }
        }

        private void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TerminalViewModel vm)
            {
                vm.SetState(TerminalViewState.Expanded);
            }
        }
    }
}
