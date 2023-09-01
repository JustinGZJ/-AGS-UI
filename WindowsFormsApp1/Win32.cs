using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Win32
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
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);


        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;
        private const int WS_MINIMIZE = 0x20000000;
        private const int WS_MAXIMIZE = 0x01000000;
        private const int WS_SYSMENU = 0x00080000;


        public static void EmbedApplication(string appPath, TabPage tabPage)
        {
            Process process = new Process();
            process.StartInfo.FileName = appPath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.Start();

            // Wait for the application to start and load
            System.Threading.Thread.Sleep(6000);

            // Set the application's parent window to the TabPage
            SetParent(process.MainWindowHandle, tabPage.Handle);

            // Remove the border and title bar
            int style = GetWindowLong(process.MainWindowHandle, GWL_STYLE);
            style = style & ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE | WS_SYSMENU);
            SetWindowLong(process.MainWindowHandle, GWL_STYLE, style);

            // Resize the embedded application to fit the TabPage
            MoveWindow(process.MainWindowHandle, 0, 0, tabPage.Width, tabPage.Height, true);

            // Set the TabPage's text to the application's title
            tabPage.Text = process.MainWindowTitle;
           ResizeFitTab(tabPage);
        }

        public static void FindExeFiles(string folderPath, List<string> exeFiles)
        {
            // Get exe files in the current folder
            exeFiles.AddRange(Directory.GetFiles(folderPath, "*.exe"));

            // Get subdirectories in the current folder
            var subdirectories = Directory.GetDirectories(folderPath);

            // Recursively find exe files in each subdirectory
            foreach (string subdirectory in subdirectories)
            {
                FindExeFiles(subdirectory, exeFiles);
            }
        }

        public static void ResizeFitTab(TabPage tabPage)
        {
            IntPtr hWnd = FindWindowEx(tabPage.Handle, IntPtr.Zero, null, null);
            if (hWnd != IntPtr.Zero)
            {
                MoveWindow(hWnd, 0, 0, tabPage.Width, tabPage.Height, true);
            }
        }


    }
}