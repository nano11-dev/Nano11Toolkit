using Nano11Toolkit.ViewModels.Pages;
using Wpf.Ui.Controls;
using Microsoft.VisualBasic;

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

class DeviceStatus
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX(uint dummy)
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            dwMemoryLoad = 0;
            ullTotalPhys = 0;
            ullAvailPhys = 0;
            ullTotalPageFile = 0;
            ullAvailPageFile = 0;
            ullTotalVirtual = 0;
            ullAvailVirtual = 0;
            ullAvailExtendedVirtual = 0;
        }
    }

    public static ulong GetInstalledRAM()
    {
        MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX(0);
        if (GlobalMemoryStatusEx(ref memStatus))
        {
            return memStatus.ullTotalPhys;
        }
        else
        {
            throw new InvalidOperationException("Error retrieving memory info.");
        }
    }

}


namespace Nano11Toolkit.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var build = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion").GetValue("CurrentBuildNumber");
            if (Int32.Parse(build.ToString()) < 9780)
            {
                Version.Content = $"Windows Version: Windows Legacy ({Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DisplayVersion")})";
            }

            if (Int32.Parse(build.ToString()) > 9780)
            {
                Version.Content = $"Windows Version: Windows 10 {Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DisplayVersion")}";
            }
            if (Int32.Parse(build.ToString()) > 21380) {
                Version.Content = $"Windows Version: Windows 11 {Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DisplayVersion")}";
            }
            
            CPU.Content = "CPU: " + Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0").GetValue("ProcessorNameString");
            RAM.Content = $"Installed RAM: {DeviceStatus.GetInstalledRAM() / (1000*1000*1000)} GB";
            Build.Content = $"Windows Build: {build}";
            
        }
    }
}
