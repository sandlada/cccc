using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cccc.Platform.Windows
{
    public static class WindowsNativeMethods
    {
        public const int WindowMessageClipboardUpdate = 0x031D;
        public const int WindowMessageKeyDown = 0x100, WindowMessageKeyUp = 0x101;
        public const int WindowMessageSystemKeyDown = 0x104, WindowMessageSystemKeyUp = 0x105;
        public const int DwmWindowAttributeUseImmersiveDarkMode = 20;
        public const uint ClipboardFormatText = 1, ClipboardFormatBitmap = 2, ClipboardFormatDeviceIndependentBitmap = 8, ClipboardFormatUnicodeText = 13,
            ClipboardFormatFileDrop = 15, ClipboardFormatDeviceIndependentBitmapVersion5 = 17;

        public delegate IntPtr WindowsHookCallback(int code, IntPtr message, IntPtr data);

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookData
        {
            public uint Key, ScanCode, Flags, Time;
            public UIntPtr Extra;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowRectangle
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInformation
        {
            public uint Size;
            public WindowRectangle Monitor, Work;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AppBarData
        {
            public uint Size;
            public IntPtr WindowHandle;
            public uint CallbackMessage;
            public uint Edge;
            public WindowRectangle Rect;
            public IntPtr LParam;
        }

        public const uint AbmGetState = 0x00000004;
        public const uint AbmGetTaskbarPos = 0x00000005;
        public const uint AbsAutoHide = 0x00000001;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AddClipboardFormatListener(IntPtr windowHandle);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr windowHandle, int attribute, ref int value, int size);
        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr windowHandle);
        [DllImport("user32.dll")]
        public static extern uint GetClipboardSequenceNumber();
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardOwner();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr windowHandle);
        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();
        [DllImport("user32.dll")]
        public static extern bool IsClipboardFormatAvailable(uint format);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint format);
        [DllImport("user32.dll")]
        public static extern int CountClipboardFormats();
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr dataHandle);
        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr dataHandle);
        [DllImport("kernel32.dll")]
        public static extern UIntPtr GlobalSize(IntPtr dataHandle);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern uint DragQueryFile(IntPtr dropHandle, uint index, System.Text.StringBuilder buffer, uint size);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr windowHandle, out uint processId);
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int virtualKey);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int hook, WindowsHookCallback callback, IntPtr moduleHandle, uint threadId);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hook, int code, IntPtr message, IntPtr data);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        private static extern IntPtr GetWindowLong64(IntPtr windowHandle, int index);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        private static extern IntPtr SetWindowLong64(IntPtr windowHandle, int index, IntPtr value);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
        private static extern int GetWindowLong32(IntPtr windowHandle, int index);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
        private static extern int SetWindowLong32(IntPtr windowHandle, int index, int value);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr windowHandle, IntPtr insertAfter, int x, int y, int width, int height, uint flags);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr windowHandle, out WindowRectangle bounds);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr windowHandle, uint command);
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr windowHandle);
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr windowHandle, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetMonitorInfo(IntPtr monitorHandle, ref MonitorInformation information);
        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr windowHandle);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowName);
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr windowHandle);
        [DllImport("shell32.dll")]
        public static extern uint SHAppBarMessage(uint message, ref AppBarData data);

        public static bool IsTaskbarAutoHide()
        {
            AppBarData data = new AppBarData { Size = (uint)Marshal.SizeOf(typeof(AppBarData)) };
            return (SHAppBarMessage(AbmGetState, ref data) & AbsAutoHide) != 0;
        }

        public static long MonotonicMilliseconds
        {
            get { return (long)(Stopwatch.GetTimestamp() * (1000.0 / Stopwatch.Frequency)); }
        }

        public static bool IsKeyDown(int virtualKey) { return (GetAsyncKeyState(virtualKey) & 0x8000) != 0; }

        public static uint GetProcessIdForWindow(IntPtr windowHandle)
        {
            uint processId;
            GetWindowThreadProcessId(windowHandle, out processId);
            return processId;
        }

        public static long GetExtendedWindowStyle(IntPtr windowHandle)
        {
            return IntPtr.Size == 8 ? GetWindowLong64(windowHandle, -20).ToInt64() : GetWindowLong32(windowHandle, -20);
        }

        public static void SetPassiveWindowStyles(IntPtr windowHandle)
        {
            long style = GetExtendedWindowStyle(windowHandle) | 0x08000000L | 0x00000080L | 0x00000020L;
            SetExtendedWindowStyle(windowHandle, style);
        }

        public static void SetExtendedWindowStyle(IntPtr windowHandle, long style)
        {
            if (IntPtr.Size == 8) SetWindowLong64(windowHandle, -20, new IntPtr(style));
            else SetWindowLong32(windowHandle, -20, (int)style);
        }

        public static void SetWindowOwner(IntPtr windowHandle, IntPtr ownerWindowHandle)
        {
            if (IntPtr.Size == 8) SetWindowLong64(windowHandle, -8, ownerWindowHandle);
            else SetWindowLong32(windowHandle, -8, ownerWindowHandle.ToInt32());
            if (GetWindow(windowHandle, 4) != ownerWindowHandle)
                throw new System.ComponentModel.Win32Exception("Could not set the passive window owner.");
        }

        public static int GetWindowDpi(IntPtr windowHandle)
        {
            try { return (int)GetDpiForWindow(windowHandle); }
            catch (EntryPointNotFoundException) { }
            catch (DllNotFoundException) { }
            return 0;
        }
    }
}
