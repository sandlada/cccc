using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Inbound.Desktop
{
    public sealed class KeyboardHookListener : IDisposable
    {
        private readonly HandleKeyHandler handler;
        private readonly Dispatcher dispatcher;
        private readonly WindowsNativeMethods.WindowsHookCallback callback;
        private IntPtr hook;

        public KeyboardHookListener(HandleKeyHandler handler, Dispatcher dispatcher)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            this.handler = handler;
            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
            callback = Callback;
        }

        public void Start()
        {
            hook = WindowsNativeMethods.SetWindowsHookEx(13, callback, WindowsNativeMethods.GetModuleHandle(null), 0);
            if (hook == IntPtr.Zero) throw new Win32Exception();
        }

        private IntPtr Callback(int code, IntPtr message, IntPtr data)
        {
            if (code >= 0)
            {
                int type = message.ToInt32();
                if (type == WindowsNativeMethods.WindowMessageKeyDown || type == WindowsNativeMethods.WindowMessageSystemKeyDown ||
                    type == WindowsNativeMethods.WindowMessageKeyUp || type == WindowsNativeMethods.WindowMessageSystemKeyUp)
                {
                    var key = (WindowsNativeMethods.KeyboardHookData)Marshal.PtrToStructure(data, typeof(WindowsNativeMethods.KeyboardHookData));
                    bool down = type == WindowsNativeMethods.WindowMessageKeyDown || type == WindowsNativeMethods.WindowMessageSystemKeyDown;
                    var command = new HandleKeyCommand
                    {
                        Key = (int)key.Key,
                        Down = down,
                        Control = WindowsNativeMethods.IsKeyDown(0x11),
                        AltOrShiftOrWin = WindowsNativeMethods.IsKeyDown(0x10) || WindowsNativeMethods.IsKeyDown(0x12) ||
                            WindowsNativeMethods.IsKeyDown(0x5B) || WindowsNativeMethods.IsKeyDown(0x5C)
                    };
                    // Schedule before the source app processes the key. No clipboard
                    // access or visual work is performed in the low-level hook.
                    dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(delegate
                    {
                        handler.Handle(command);
                    }));
                }
            }
            return WindowsNativeMethods.CallNextHookEx(hook, code, message, data);
        }

        public void Dispose()
        {
            if (hook != IntPtr.Zero)
            {
                WindowsNativeMethods.UnhookWindowsHookEx(hook);
                hook = IntPtr.Zero;
            }
        }
    }
}
