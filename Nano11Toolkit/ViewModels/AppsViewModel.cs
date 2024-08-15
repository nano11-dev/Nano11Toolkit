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

namespace Nano11Toolkit.ViewModels
{
    public partial class AppsViewModel : ObservableObject
    {
        [ObservableProperty]
        public ApplicationEntry[] entries = JsonSerializer.Deserialize<ApplicationEntry[]>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Apps.json")));

        public ICommand InstallCommand { get; }
        public ICommand RenameSpinnerCommand { get; }

        private void InstallWingetPackage(string PackageId)
        {
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "winget.exe";
            si.Arguments = $"install -e --id {PackageId}";
            si.CreateNoWindow = true;
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            si.RedirectStandardInput = true;
            var proc = Process.Start(si);
            proc.WaitForExit();
            Debug.WriteLine($"winget.exe install -e --id {PackageId}");
            Debug.WriteLine(proc.StandardOutput.ReadToEnd());
            Debug.WriteLine(proc.StandardError.ReadToEnd());

            return;
        }

        private async Task InstallApp(Button b, Grid parent, ApplicationEntry entry, string AppId)
        {
            InstallWingetPackage(AppId);
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    b.IsEnabled = false;
            //    b.Content = "Installed!";
            //    foreach (Control child in parent.Children)
            //    {
            //        if (child.Name == entry.Id + "_spinner")
            //        {
            //            child.Visibility = System.Windows.Visibility.Hidden;
            //            break;
            //        }
            //    }
            //});
        }

        public bool IsInstalled(string PackageId)
        {
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "winget.exe";
            si.Arguments = $"list -e -q {PackageId}";
            si.CreateNoWindow = true;
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            si.RedirectStandardInput = true;
            var proc = Process.Start(si);
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd().Contains(PackageId);
        }

        private void OnClick(Button button)
        {
            if (button?.DataContext is ApplicationEntry entry)
            {
                button.Content = "Installing...";
                button.IsEnabled = false;
                var parent = (Grid)button.Parent;
                foreach (Control child in parent.Children)
                {
                    if (child.Name == entry.Id + "_spinner")
                    {
                        child.Visibility = Visibility.Visible;
                        break;
                    }
                }
                Task.Run(async () => InstallApp(button, parent, entry, entry.WingetId));
            }
        }

        private void RenameSpinner(ProgressRing spinner)
        {
            if (spinner?.DataContext is ApplicationEntry entry)
            {
                spinner.Name = entry.Id + "_spinner";
                spinner.Visibility = Visibility.Collapsed;
            }
        }

        public AppsViewModel()
        {
            InstallCommand = new RelayCommand<Button>(OnClick);
            RenameSpinnerCommand = new RelayCommand<ProgressRing>(RenameSpinner);
        }
    }
}
