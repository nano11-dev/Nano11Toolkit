using ColorCode.Compilation.Languages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Intrinsics.Arm;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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

namespace Nano11Toolkit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public static long GetTotalUsedRAM()
        {

            using var memCounter = new PerformanceCounter("Memory", "Available Bytes");
            // Get the total physical memory available
            long availableBytes = memCounter.RawValue;

            // Total physical memory is typically available in the "Memory" category

            // Calculate used memory
            ulong usedMemoryBytes = DeviceStatus.GetInstalledRAM() - (ulong)availableBytes;
            return (long)usedMemoryBytes;
        }

        public HomePage()
        {
            this.InitializeComponent();
            var build = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion").GetValue("CurrentBuildNumber");
            if (Int32.Parse(build.ToString()) < 9780)
            {
                Version.Text = $"Windows Version: Windows Legacy ({Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DisplayVersion")})";
            }

            if (Int32.Parse(build.ToString()) > 9780)
            {
                Version.Text = $"Windows Version: Windows 10 {Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DisplayVersion")}";
            }
            if (Int32.Parse(build.ToString()) > 21380)
            {
                Version.Text = $"Windows Version: Windows 11 {Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DisplayVersion")}";
            }

            CPU.Text = "CPU: " + Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0").GetValue("ProcessorNameString");
            RAM.Text = $"Installed RAM: {DeviceStatus.GetInstalledRAM() / (1000 * 1000 * 1000)} GB";
            Build.Text = $"Windows Build: {build}";
        }

        public long UsedRAM = GetTotalUsedRAM() / (long)DeviceStatus.GetInstalledRAM();
    }
}
