using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Nano11Toolkit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public class TogglableEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EnableCommand { get; set; }
        public string DisableCommand { get; set; }
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
        public string EnabledOutput { get; set; }
        public bool Enabled { get; set; }
    }
    public class ButtonEntry
    {
        public string Name { get; set; }
        public string Command { get; set; }
    }

    public sealed partial class TweaksPage : Page
    {
        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            //if (sender is Wpf.Ui.Controls.ToggleSwitch toggleSwitch && toggleSwitch.DataContext is TogglableEntry entry)
            //{
            //    var viewModel = DataContext as TogglablesViewModel;
            //    viewModel?.ToggleCommand.Execute(toggleSwitch);
            //}
        }

        async void ExecuteClick(Button button)
        {
            var dc = button.DataContext as ButtonEntry;
            var si = new ProcessStartInfo();
            si.CreateNoWindow = true;
            si.FileName = "cmd.exe";
            si.Arguments = "/c " + dc.Command;
            Process.Start(si);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(sender);
            Debug.WriteLine(sender.As<Button>().DataContext);
            if (sender is Button button && button.DataContext is ButtonEntry entry)
            {
                
                ExecuteClick(button);
            }
        }

        private ObservableCollection<ButtonEntry> buttons = new ObservableCollection<ButtonEntry>();
        public ObservableCollection<ButtonEntry> ButtonEntries { get { return buttons; } }
        public TweaksPage()
        {
            foreach(var item in System.Text.Json.JsonSerializer.Deserialize<ButtonEntry[]>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Buttons.json"))))
            {
                buttons.Add(item);
            }
            this.InitializeComponent();
            
        }
    }
}
