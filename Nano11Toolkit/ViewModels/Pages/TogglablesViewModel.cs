using Nano11Toolkit.Models;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Wpf.Ui.Controls;
using System.Diagnostics;
using System.IO;

namespace Nano11Toolkit.ViewModels.Pages
{
    public partial class TogglablesViewModel : ObservableObject
    {
        public static string jsonString =
            """
            [
                {
                    "Name": "Verbose Login",
                    "Description": "Toggles verbose informations in the login screen",
                    "EnableCommand": "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v verbosestatus /t REG_DWORD /d 1 /f",
                    "DisableCommand": "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v verbosestatus /t REG_DWORD /d 0 /f",
                    "QueryKey": "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System",
                    "QueryValue": "verbosestatus",
                    "EnabledOutput": "0x1",
                    "Enabled": false
                }
            ]
            """;

        [ObservableProperty]
        private TogglableEntry[] entries = JsonSerializer.Deserialize<TogglableEntry[]>(File.ReadAllText("ToggleTweaks.json"));

        public TogglablesViewModel()
        {
            ToggleCommand = new RelayCommand<ToggleSwitch>(OnToggle);
            InitializeEntries();
        }

        public ICommand ToggleCommand { get; }

        private void OnToggle(ToggleSwitch toggleSwitch)
        {
            if (toggleSwitch?.DataContext is TogglableEntry entry)
            {
                bool? isChecked = toggleSwitch.IsChecked;
                // Example logic: print the name of the toggled item and its checked state
                System.Diagnostics.Debug.WriteLine($"Toggled: {entry.Name}, IsChecked: {isChecked}");
                toggleSwitch.IsEnabled = false;
                if (!isChecked.HasValue) {
                    return;
                }
                if (isChecked == true) // I dont wanna cast it
                {
                    ExecuteCommand(entry.EnableCommand);
                }
                else
                {
                    ExecuteCommand(entry.DisableCommand);
                }
                toggleSwitch.IsEnabled = true;
            }
        }

        private void ExecuteCommand(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            Debug.WriteLine(stdout + stderr);
            Debug.WriteLine("Finished!");
        }

        private void InitializeEntries()
        {
            foreach (var entry in entries)
            {
                entry.Enabled = CheckIfEnabled(entry.QueryKey, entry.QueryValue, entry.EnabledOutput);
            }
        }

        private bool CheckIfEnabled(string regPath, string regValue, string enabledOutput)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "reg.exe";
            startInfo.Arguments = $"query \"{regPath}\" /v {regValue}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                Debug.WriteLine(stdout + stderr);
                if (stdout != null && stderr != null)
                {
                    return stdout.Contains(enabledOutput) || stderr.Contains(enabledOutput);
                }
            }
            return false;
        }
    }
}
