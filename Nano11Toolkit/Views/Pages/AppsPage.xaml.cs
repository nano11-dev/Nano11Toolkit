using CommunityToolkit.Mvvm.ComponentModel;
using Nano11Toolkit.Models;
using Nano11Toolkit.ViewModels.Pages;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using Wpf.Ui.Controls;

namespace Nano11Toolkit.Views.Pages
{
    public partial class AppsPage : INavigableView<AppsViewModel>
    {
        public AppsViewModel ViewModel { get; }

        public AppsPage(AppsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is ApplicationEntry entry)
            {
                var viewModel = DataContext as AppsViewModel;
                viewModel?.InstallCommand.Execute(button);
            }
        }

        private void TempSpinner_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.ProgressRing spinner && spinner.DataContext is ApplicationEntry entry)
            {
                var viewModel = DataContext as AppsViewModel;
                viewModel?.RenameSpinnerCommand.Execute(spinner);
            }
        }

        private async void Button_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is ApplicationEntry entry)
            {
                if (entry == null)
                {
                    Console.WriteLine("DataContext is not an ApplicationEntry.");
                    return;
                }

                button.IsEnabled = false;
                button.Content = "...";
                try
                {
                    await CheckButton(button, entry);
                }
                catch (Exception ex)
                {
                    // Log or handle exception
                    Console.WriteLine($"Exception caught: {ex.Message}");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show($"Exception: {ex.Message}");
                    });
                }
            }
        }

        private async Task CheckButton(Wpf.Ui.Controls.Button b, ApplicationEntry e)
        {
            var viewModel = this.DataContext as AppsViewModel;

            if (viewModel == null)
            {
                Console.WriteLine("DataContext is not an AppsViewModel.");
                return;
            }

            // Perform any potentially long-running operation asynchronously
            var isInstalled = await Task.Run(() => viewModel.IsInstalled(e.WingetId));
            Debug.WriteLine(isInstalled.ToString());
            // Update the UI on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                b.IsEnabled = !isInstalled;
                if (isInstalled)
                {
                    b.Content = "Installed";
                }
                else
                {
                    b.Content = "Install";
                    b.Appearance = ControlAppearance.Primary;
                }
            });
        }
    }
}

