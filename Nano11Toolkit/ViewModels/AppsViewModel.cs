using Nano11Toolkit.Models;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using Windows.UI.Popups;

namespace Nano11Toolkit.ViewModels
{
    public partial class AppsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ApplicationEntry[] entries;

        public ICommand InstallCommand { get; }
        public ICommand RenameSpinnerCommand { get; }

        public AppsViewModel()
        {
            // Load entries from JSON file
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Apps.json");
            entries = JsonSerializer.Deserialize<ApplicationEntry[]>(File.ReadAllText(jsonFilePath));

            // Initialize commands
            InstallCommand = new RelayCommand<Button>(OnClick);
            RenameSpinnerCommand = new RelayCommand<ProgressRing>(RenameSpinner);
        }

        public void OnClick(Button button)
        {
            Debug.WriteLine("Clicked something");
            if (button?.DataContext is ApplicationEntry entry)
            {
                button.Content = "Installing...";
                button.IsEnabled = false;

               
                // Run the installation on a background thread
                Task.Run(async () => await InstallApp(button, entry, entry.WingetId));
            }
        }

        private async Task InstallApp(Button b, ApplicationEntry entry, string appId)
        {
            InstallWingetPackage(appId);
            DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            // Use DispatcherQueue to update UI elements on the UI threa
            
        }



        private void InstallWingetPackage(string packageId)
        {
            var si = new ProcessStartInfo
            {
                FileName = "winget.exe",
                Arguments = $"install -e --id {packageId}",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var proc = Process.Start(si);
            proc.WaitForExit();

            Debug.WriteLine($"winget.exe install -e --id {packageId}");
            Debug.WriteLine(proc.StandardOutput.ReadToEnd());
            Debug.WriteLine(proc.StandardError.ReadToEnd());
        }

        public bool IsInstalled(string packageId)
        {
            var si = new ProcessStartInfo
            {
                FileName = "winget.exe",
                Arguments = $"list -e -q {packageId}",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var proc = Process.Start(si);
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd().Contains(packageId);
        }

        public void RenameSpinner(ProgressRing spinner)
        {
            if (spinner?.DataContext is ApplicationEntry entry)
            {
                spinner.Name = entry.Id + "_spinner";
                Debug.WriteLine(spinner?.Name.ToString());
                spinner.Visibility = Visibility.Collapsed;
            }
        }
    }
}
