using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // This HWND has no WPF render target and no GDI background brush. Only the
    // Windows compositor draws into it, so window messages cannot clear it white.
    internal sealed class CompositionWindow : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate IntPtr WindowProcedure(IntPtr windowHandle, uint message, IntPtr wordParameter, IntPtr longParameter);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WindowClassRegistration
        {
            internal uint Size, Style;
            internal IntPtr Procedure;
            internal int ClassExtra, WindowExtra;
            internal IntPtr Instance, Icon, Cursor, Background;
            [MarshalAs(UnmanagedType.LPWStr)] internal string MenuName;
            [MarshalAs(UnmanagedType.LPWStr)] internal string ClassName;
            internal IntPtr SmallIcon;
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern ushort RegisterClassEx(ref WindowClassRegistration definition);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateWindowEx(uint extendedStyle, string className, string title, uint style,
            int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr data);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr DefWindowProc(IntPtr windowHandle, uint message, IntPtr wordParameter, IntPtr longParameter);
        [DllImport("user32.dll")] private static extern bool DestroyWindow(IntPtr windowHandle);
        private const string ClassName = "Cccc.PassiveComposition.v211";
        private static readonly WindowProcedure Procedure = ProcessMessage;
        private static bool registered;
        private IntPtr handle;
        internal IntPtr Handle { get { return handle; } }
        internal static Action<IntPtr, uint> ObservedMessage;
        internal CompositionWindow()
        {
            if (!registered)
            {
                WindowClassRegistration definition = new WindowClassRegistration { Size = (uint)Marshal.SizeOf(typeof(WindowClassRegistration)),
                    Procedure = Marshal.GetFunctionPointerForDelegate(Procedure), Instance = WindowsNativeMethods.GetModuleHandle(null),
                    ClassName = ClassName, Background = IntPtr.Zero };
                if (RegisterClassEx(ref definition) == 0 && Marshal.GetLastWin32Error() != 1410)
                    throw new Win32Exception();
                registered = true;
            }
            // NOACTIVATE | NOREDIRECTIONBITMAP | TOOLWINDOW | TRANSPARENT | TOPMOST.
            handle = CreateWindowEx(0x082000A8, ClassName, "Cccc " + ToastProfile.Label + " Acrylic", 0x80000000,
                0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, WindowsNativeMethods.GetModuleHandle(null), IntPtr.Zero);
            if (handle == IntPtr.Zero) throw new Win32Exception();
        }
        private static IntPtr ProcessMessage(IntPtr windowHandle, uint message, IntPtr wordParameter, IntPtr longParameter)
        {
            if (ObservedMessage != null) ObservedMessage(windowHandle, message);
            if (message == 0x14) return new IntPtr(1); // No GDI background erase.
            if (message == 0x21) return new IntPtr(3); // MA_NOACTIVATE.
            if (message == 0x84) return new IntPtr(-1); // HTTRANSPARENT.
            return DefWindowProc(windowHandle, message, wordParameter, longParameter);
        }
        public void Dispose()
        { if (handle != IntPtr.Zero) { DestroyWindow(handle); handle = IntPtr.Zero; } }
    }
}
