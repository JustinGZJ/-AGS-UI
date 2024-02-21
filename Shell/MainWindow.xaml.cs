using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using Path = System.IO.Path;

namespace Shell
{
    public partial class MainWindow
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);

        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.InvalidateVisual();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private List<Process> _processes = new();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var workdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            //  ScanDirectoryForExeFiles(this.tabControl,Path.Combine(workdir,"SCADA"));
            var exeFiles = LoadExeFilesAsync(Path.Combine(workdir, "SCADA"));
            ConcurrentBag<Process> processBag = new ConcurrentBag<Process>();
            await Task.Run(() =>
            {
                Parallel.ForEach(exeFiles, (s, state) =>
                {
                    Process process = WaitForProcessStart(s);
                    processBag.Add(process);
                });
                _processes = processBag.ToList();
            });

            foreach (var process in processBag)
            {
                var newTabItem = new LayoutDocument
                {
                    Title = "loading..."
                };
                this.documentPane.Children.Add(newTabItem);
                EmbedApplicationInTabControl(process, newTabItem);
            }

            this.Status.Content = "";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (var process in _processes)
            {
                if (!process.HasExited)
                    process.CloseMainWindow();
            }

            base.OnClosing(e);
        }


        private string[] LoadExeFilesAsync(string directoryPath)
        {
            try
            {
                // Get all .exe files in the current directory
                var exeFiles = Directory.EnumerateFiles(directoryPath, "*.exe")
                    .Select(Path.GetFullPath)
                    .ToArray();

                // Get all subdirectories in the current directory
                var subdirectories = Directory.GetDirectories(directoryPath);

                // Recursively load each subdirectory for .exe files
                var subResults = subdirectories.Select(LoadExeFilesAsync);
                return exeFiles.Concat(subResults.SelectMany(x => x)).ToArray();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., access denied) if necessary
                return new string[0];
            }
        }


        private Process WaitForProcessStart(string appPath)
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(appPath) ?? string.Empty;
            process.StartInfo.FileName = appPath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.EnableRaisingEvents = true;
            //  process.Exited += Process_Exited;
            process.Start();

            // Wait for the application to start and load
            while (process.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(100);
                process.Refresh(); // Update the process information

                // Check if the process has exited
                if (process.HasExited)
                {
                    return process;
                }
            }

            return process;
        }

        private void EmbedApplicationInTabControl(Process process, LayoutDocument tabItem)
        {
            if (process.HasExited) return;
            // Set the external application as a child window
            int style = GetWindowLong(process.MainWindowHandle, GWL_STYLE);
            style |= WS_CHILD;
            SetWindowLong(process.MainWindowHandle, GWL_STYLE, style);

            // Create a CustomHwndHost instance with the external application's window handle
            CustomHwndHost hwndHost = new CustomHwndHost(process.MainWindowHandle);
            tabItem.Title = process.MainWindowTitle;
            // Set the content of the TabItem to the CustomHwndHost instance
            tabItem.Content = hwndHost;
        }


        // private Process EmbedApplication(string appPath, TabItem tabItem)
        // {
        //     Process process = new Process();
        //     process.StartInfo.WorkingDirectory = Path.GetDirectoryName(appPath);
        //     process.StartInfo.FileName = appPath;
        //     process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        //     process.EnableRaisingEvents = true;
        //     //  process.Exited += Process_Exited;
        //     process.Start();
        //
        //     // Wait for the application to start and load
        //     while (process.MainWindowHandle == IntPtr.Zero)
        //     {
        //         Thread.Sleep(100);
        //         process.Refresh(); // Update the process information
        //
        //         // Check if the process has exited
        //         if (process.HasExited)
        //         {
        //             return process;
        //         }
        //     }
        //
        //     if (!process.HasExited)
        //     {
        //         // Set the external application as a child window
        //         int style = GetWindowLong(process.MainWindowHandle, GWL_STYLE);
        //         style |= WS_CHILD;
        //         SetWindowLong(process.MainWindowHandle, GWL_STYLE, style);
        //
        //         // Create a CustomHwndHost instance with the external application's window handle
        //         CustomHwndHost hwndHost = new CustomHwndHost(process.MainWindowHandle);
        //         tabItem.Header = process.MainWindowTitle;
        //         // Set the content of the TabItem to the CustomHwndHost instance
        //         tabItem.Content = hwndHost;
        //     }
        //
        //     return process;
        // }
    }
}