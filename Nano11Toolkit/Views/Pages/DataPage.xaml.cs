using Nano11Toolkit.Models;
using Nano11Toolkit.ViewModels.Pages;
using System.Windows;
using Wpf.Ui.Controls;

namespace Nano11Toolkit.Views.Pages
{
    public partial class DataPage : INavigableView<TogglablesViewModel>
    {
        public TogglablesViewModel ViewModel { get; }

        public DataPage(TogglablesViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.ToggleSwitch toggleSwitch && toggleSwitch.DataContext is TogglableEntry entry)
            {
                var viewModel = DataContext as TogglablesViewModel;
                viewModel?.ToggleCommand.Execute(toggleSwitch);
            }
        }
    }
}
