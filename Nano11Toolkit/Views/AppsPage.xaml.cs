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
using System.Collections.ObjectModel;

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
            foreach (var item in viewModel.Entries)
            {
                Apps.Add(item);
            }
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked stuff");
            if (sender is Button button && button.DataContext is ApplicationEntry entry)
            {
                Debug.WriteLine("Clicked some something");
                await Install(button, entry);
            }
        }

        private void TempSpinner_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ProgressRing spinner && spinner.DataContext is ApplicationEntry entry)
            {
                viewModel?.RenameSpinner(spinner);
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
                    Debug.WriteLine("Checking " +  entry.Name);
                    await CheckButton(button, entry);
                }
                catch (Exception ex)
                {
                    // Log or handle exception
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }

        public AppsViewModel viewModel = new AppsViewModel();
        public ObservableCollection<ApplicationEntry> Apps = new ObservableCollection<ApplicationEntry>();

        private async Task Install(Button b, ApplicationEntry entry)
        {
            Debug.WriteLine("Clicked something");
            DispatcherQueue.TryEnqueue(() =>
            {
                b.Content = "Installing";
                var parent = b.Parent as Grid;

                foreach (var item in parent.Children)
                {
                    Debug.WriteLine(item);
                    try
                    {
                        ProgressRing ring = item as ProgressRing;
                        item.Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        Debug.WriteLine("Not a progressring");
                    }
                }
            });

                // Run the installation on a background thread
            var isCompleted = await Task.Run(async () => viewModel.InstallWingetPackage(entry.WingetId));
            Debug.WriteLine($"{isCompleted.ToString()}");
            DispatcherQueue.TryEnqueue(() =>
            {
                var p = b.Parent as Grid;
                foreach (var item in p.Children)
                {
                    Debug.WriteLine(item);
                    try
                    {
                        ProgressRing ring = item as ProgressRing;
                        if (ring != null)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Not a progressring");
                    }
                }
                if (isCompleted)
                {
                    b.Content = "Installed!";
                    var parent = b.Parent as Grid;
                    ProgressRing spinner = parent.FindName(entry.Id + "_spinner") as ProgressRing;
                    spinner.Visibility = Visibility.Collapsed;
                }
                else
                {
                    b.Content = "Failed :(";
                }
            });
        }
        

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
