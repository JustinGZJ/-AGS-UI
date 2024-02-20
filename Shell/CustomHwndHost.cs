using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Shell
{
    public class CustomHwndHost : HwndHost
    {
        private IntPtr _childHandle;
        const int GWL_STYLE = (-16);
        const int WS_CHILD = 0x40000000;

        public CustomHwndHost(IntPtr childHandle)
        {
            _childHandle = childHandle;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HandleRef href = new HandleRef();

            if (_childHandle == IntPtr.Zero) return href;
            SetWindowLong(this._childHandle, GWL_STYLE, WS_CHILD);
            SetParent(this._childHandle, hwndParent.Handle);
            href = new HandleRef(this, this._childHandle);
            return href;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            SetParent(_childHandle, IntPtr.Zero);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            AdjustChildSize(constraint);
            GetClientRect(_childHandle, out Rect rect);
            return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        private void AdjustChildSize(Size newSize)
        {
            MoveWindow(_childHandle, 0, 0, (int)newSize.Width, (int)newSize.Height, true);
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);
    }
}