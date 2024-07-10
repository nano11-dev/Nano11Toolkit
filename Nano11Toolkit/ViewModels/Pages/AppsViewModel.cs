using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Nano11Toolkit.Models;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Wpf.Ui.Controls;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace Nano11Toolkit.ViewModels.Pages
{
    public partial class AppsViewModel : ObservableObject
    {
        [ObservableProperty]
        public ApplicationEntry[] entries = JsonSerializer.Deserialize<ApplicationEntry[]>(File.ReadAllText("Apps.json"));

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

        private async Task InstallApp(Wpf.Ui.Controls.Button b, Grid parent, ApplicationEntry entry, string AppId)
        {
            InstallWingetPackage(AppId);
            Application.Current.Dispatcher.Invoke(() =>
            {
                b.IsEnabled = false;
                b.Content = "Installed!";
                foreach (Control child in parent.Children)
                {
                    if (child.Name == entry.Id + "_spinner")
                    {
                        child.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    }
                }
            });
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

        private void OnClick(Wpf.Ui.Controls.Button button)
        {
            if (button?.DataContext is ApplicationEntry entry)
            {
                button.Content = "Installing...";
                button.IsEnabled = false;
                var parent = (Grid)button.Parent;
                foreach(Control child in parent.Children)
                {
                    if (child.Name == entry.Id + "_spinner")
                    {
                        child.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                }
                Task.Run(async () => InstallApp(button, parent, entry, entry.WingetId));
            }
        }

        private void RenameSpinner(Wpf.Ui.Controls.ProgressRing spinner)
        {
            if (spinner?.DataContext is ApplicationEntry entry)
            {
                spinner.Name = entry.Id + "_spinner";
                spinner.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public AppsViewModel()
        {
            InstallCommand = new RelayCommand<Wpf.Ui.Controls.Button>(OnClick);
            RenameSpinnerCommand = new RelayCommand<Wpf.Ui.Controls.ProgressRing>(RenameSpinner);
        }
    }
}
