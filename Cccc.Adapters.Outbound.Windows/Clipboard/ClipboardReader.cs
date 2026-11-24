using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Windows
{
    internal static class ClipboardReader
    {
        // Only standard clipboard handles are read. No OLE deserialization, file reads,
        // bitmap decoding, HTML loading, or custom-format execution is performed.
        internal static ClipboardContent Read(IntPtr window, uint expected)
        {
            return Read(window, expected, WindowsClipboardNativeAccess.Instance);
        }

        internal static ClipboardContent Read(IntPtr window, uint expected, IClipboardNativeAccess access)
        {
            if (!access.Open(window)) return null;
            try
            {
                uint before = access.Sequence;
                if (before != expected) return null;
                ClipboardContent result = new()
                {
                    Sequence = before,
                    OwnerProcess = access.OwnerProcess,
                    Preview = "暂不支持预览",
                    TypeLabel = "其他格式"
                };
                if (access.Available(WindowsNativeMethods.ClipboardFormatFileDrop))
                {
                    IntPtr handle = access.Data(WindowsNativeMethods.ClipboardFormatFileDrop);
                    if (handle == IntPtr.Zero) return null;
                    uint count = WindowsNativeMethods.DragQueryFile(handle, uint.MaxValue, null, 0);
                    if (count == 0) return null;
                    StringBuilder name = new(32768);
                    if (WindowsNativeMethods.DragQueryFile(handle, 0, name, (uint)name.Capacity) == 0) return null;
                    string file = Path.GetFileName(name.ToString().TrimEnd('\\'));
                    if (string.IsNullOrEmpty(file)) file = name.ToString();
                    result.Preview = PreviewText.Shorten(file) + (count > 1 ? $" · 共 {count} 项" : "");
                    result.TypeLabel = count > 1 ? $"文件 · {count} 项" : "文件";
                    result.Supported = true;
                }
                else if (access.Available(WindowsNativeMethods.ClipboardFormatUnicodeText) || access.Available(WindowsNativeMethods.ClipboardFormatText))
                {
                    bool unicode = access.Available(WindowsNativeMethods.ClipboardFormatUnicodeText);
                    IntPtr handle = access.Data(unicode ? WindowsNativeMethods.ClipboardFormatUnicodeText : WindowsNativeMethods.ClipboardFormatText);
                    if (handle == IntPtr.Zero) return null;
                    ulong size = WindowsNativeMethods.GlobalSize(handle).ToUInt64();
                    if (size == 0) return null;
                    IntPtr pointer = WindowsNativeMethods.GlobalLock(handle);
                    if (pointer == IntPtr.Zero) return null;
                    try
                    {
                        int maximum = (int)Math.Min(size / (unicode ? 2UL : 1UL), 65536UL);
                        int length = 0;
                        while (length < maximum && (unicode ? Marshal.ReadInt16(pointer, length * 2) != 0 :
                            Marshal.ReadByte(pointer, length) != 0)) length++;
                        string text = unicode ? Marshal.PtrToStringUni(pointer, length) :
                            Marshal.PtrToStringAnsi(pointer, length);
                        result.Preview = PreviewText.Shorten(text);
                    }
                    finally { WindowsNativeMethods.GlobalUnlock(handle); }
                    result.TypeLabel = "文字";
                    result.Supported = true;
                }
                else if (access.Available(WindowsNativeMethods.ClipboardFormatDeviceIndependentBitmapVersion5) || access.Available(WindowsNativeMethods.ClipboardFormatDeviceIndependentBitmap) ||
                    access.Available(WindowsNativeMethods.ClipboardFormatBitmap))
                {
                    uint format = access.Available(WindowsNativeMethods.ClipboardFormatDeviceIndependentBitmapVersion5) ? WindowsNativeMethods.ClipboardFormatDeviceIndependentBitmapVersion5 :
                        access.Available(WindowsNativeMethods.ClipboardFormatDeviceIndependentBitmap) ? WindowsNativeMethods.ClipboardFormatDeviceIndependentBitmap : WindowsNativeMethods.ClipboardFormatBitmap;
                    if (access.Data(format) == IntPtr.Zero) return null;
                    result.Preview = result.TypeLabel = "图片";
                    result.Supported = true;
                }
                else if (access.FormatCount <= 0) return null;
                return access.Sequence == before ? result : null;
            }
            finally { access.Close(); }
        }
    }
}
