using Nano11Toolkit.Models;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Nano11Toolkit.Views;
using System;

namespace Nano11Toolkit.ViewModels
{
    public partial class TweaksViewModel : ObservableObject
    {
        public static string te = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Togglables.json"));

        [ObservableProperty]
        private TogglableEntry[] toggleEntries = JsonSerializer.Deserialize<TogglableEntry[]>(te, Nano11Toolkit.Serialization.Nano11JsonContext.Default.TogglableEntryArray);

        public static string en = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Buttons.json"));

        [ObservableProperty]
        private ButtonEntry[] buttonEntries = JsonSerializer.Deserialize<ButtonEntry[]>(en, Nano11Toolkit.Serialization.Nano11JsonContext.Default.ButtonEntryArray);

        public TweaksViewModel()
        {
            ToggleCommand = new RelayCommand<ToggleSwitch>(OnToggle);
            ClickCommand = new RelayCommand<Button>(OnClick);
            InitializeEntries();
        }
        public ICommand ToggleCommand { get; }
        public ICommand ClickCommand { get; }

        private void OnClick(Button button)
        {
            if (button?.DataContext is ButtonEntry entry)
            {
                button.IsEnabled = false;
                ExecuteCommand(entry.Command);
                button.IsEnabled = true;
            }
        }

        private void OnToggle(ToggleSwitch toggleSwitch)
        {
            if (toggleSwitch?.DataContext is TogglableEntry entry)
            {
                bool? isChecked = toggleSwitch.IsOn;
                // Example logic: print the name of the toggled item and its checked state
                System.Diagnostics.Debug.WriteLine($"Toggled: {entry.Name}, IsChecked: {isChecked}");
                toggleSwitch.IsEnabled = false;
                if (!isChecked.HasValue)
                {
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
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Debug.WriteLine("Finished!");
        }

        private void InitializeEntries()
        {
            foreach (var entry in toggleEntries)
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