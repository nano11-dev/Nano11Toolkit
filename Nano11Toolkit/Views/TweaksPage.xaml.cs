using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using WinRT;
using Nano11Toolkit.Models;
using Nano11Toolkit.ViewModels;
using Microsoft.UI.Composition;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Nano11Toolkit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class TweaksPage : Page
    {
        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch && toggleSwitch.DataContext is TogglableEntry entry)
            {
                viewModel.ToggleCommand.Execute(toggleSwitch);
            }
        }
     

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(sender);
            Debug.WriteLine(sender.As<Button>().DataContext);
            if (sender is Button button && button.DataContext is ButtonEntry entry)
            {
                viewModel.ClickCommand.Execute(button);
            }
        }

        public TweaksViewModel viewModel = new TweaksViewModel();

        private ObservableCollection<ButtonEntry> buttons = new ObservableCollection<ButtonEntry>();
        public ObservableCollection<ButtonEntry> ButtonEntries { get { return buttons; } }

        private ObservableCollection<TogglableEntry> togglables = new ObservableCollection<TogglableEntry>();
        public ObservableCollection<TogglableEntry> TogglableEntries { get { return togglables; } }

        public TweaksPage()
        {
            foreach (var entry in viewModel.ToggleEntries)
            {
                TogglableEntries.Add(entry);
            }

            foreach (var entry in viewModel.ButtonEntries)
            {
                ButtonEntries.Add(entry);
            }

            Debug.WriteLine("Hi!");
            foreach(var entry in ButtonEntries)
            {
                Debug.WriteLine(entry);
            }

            foreach (var entry in TogglableEntries)
            {
                Debug.WriteLine(entry);
            }

            this.InitializeComponent();
            
        }
    }
}
