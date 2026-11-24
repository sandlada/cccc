using System;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Windows
{
    internal interface IClipboardNativeAccess
    {
        bool Open(IntPtr window);
        void Close();
        uint Sequence { get; }
        uint OwnerProcess { get; }
        bool Available(uint format);
        IntPtr Data(uint format);
        int FormatCount { get; }
    }

    internal sealed class WindowsClipboardNativeAccess : IClipboardNativeAccess
    {
        internal static readonly WindowsClipboardNativeAccess Instance = new();

        public bool Open(IntPtr window) => WindowsNativeMethods.OpenClipboard(window);
        public void Close() => WindowsNativeMethods.CloseClipboard();
        public uint Sequence => WindowsNativeMethods.GetClipboardSequenceNumber();
        public uint OwnerProcess => WindowsNativeMethods.GetProcessIdForWindow(WindowsNativeMethods.GetClipboardOwner());
        public bool Available(uint format) => WindowsNativeMethods.IsClipboardFormatAvailable(format);
        public IntPtr Data(uint format) => WindowsNativeMethods.GetClipboardData(format);
        public int FormatCount => WindowsNativeMethods.CountClipboardFormats();
    }
}
