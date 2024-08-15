using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Nano11Toolkit.ViewModels;
using Nano11Toolkit.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Nano11Toolkit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
        public AppsPage()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ApplicationEntry entry)
            {
                var viewModel = DataContext as AppsViewModel;
                viewModel?.InstallCommand.Execute(button);
            }
        }

        private void TempSpinner_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ProgressRing spinner && spinner.DataContext is ApplicationEntry entry)
            {
                var viewModel = DataContext as AppsViewModel;
                viewModel?.RenameSpinnerCommand.Execute(spinner);
            }
        }


        private async void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ApplicationEntry entry)
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
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    System.Windows.MessageBox.Show($"Exception: {ex.Message}");
                    //});
                }
            }
        }

        public AppsViewModel viewModel = new AppsViewModel();

        private async Task CheckButton(Button b, ApplicationEntry e)
        {
            
            if (viewModel == null)
            {
                Console.WriteLine("DataContext is not an AppsViewModel.");
                return;
            }

            // Perform any potentially long-running operation asynchronously
            var isInstalled = await Task.Run(() => viewModel.IsInstalled(e.WingetId));
            Debug.WriteLine(isInstalled.ToString());
            // Update the UI on the UI thread
            DispatcherQueue.TryEnqueue(() =>
            {
                b.IsEnabled = !isInstalled;
                if (isInstalled)
                {
                    b.Content = "Installed";
                }
                else
                {
                    b.Content = "Install";
                }
            });
        }
    }
}
