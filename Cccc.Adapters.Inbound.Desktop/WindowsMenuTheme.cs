using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Cccc.Adapters.Inbound.Desktop
{
    public static class WindowsMenuTheme
    {
        // HMENU drawing remains entirely with Windows. These optional uxtheme
        // exports select its process-local light/dark menu theme, as used by
        // Microsoft PowerToys. They are not a documented compatibility contract.
        // https://github.com/microsoft/PowerToys/blob/main/src/modules/ZoomIt/ZoomIt/Utility.cpp
        private delegate int SetApplicationMode(int mode);
        private delegate void ThemeAction();
        private static bool initialized;
        private static IntPtr module;
        private static SetApplicationMode setApplicationMode;
        private static ThemeAction flushMenus, refreshPolicy;
        private static int previousMode = -1;

        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            // Before build 18362, ordinal 135 has a different signature.
            Version version = Environment.OSVersion.Version;
            if (version.Major < 10 || version.Build < 18362) return;
            module = LoadLibraryEx("uxtheme.dll", IntPtr.Zero, 0x00000800);
            if (module == IntPtr.Zero) return;
            IntPtr mode = GetProcAddress(module, new IntPtr(135));
            IntPtr flush = GetProcAddress(module, new IntPtr(136));
            IntPtr refresh = GetProcAddress(module, new IntPtr(104));
            if (mode != IntPtr.Zero) setApplicationMode = (SetApplicationMode)Marshal.GetDelegateForFunctionPointer(mode, typeof(SetApplicationMode));
            if (flush != IntPtr.Zero) flushMenus = (ThemeAction)Marshal.GetDelegateForFunctionPointer(flush, typeof(ThemeAction));
            if (refresh != IntPtr.Zero) refreshPolicy = (ThemeAction)Marshal.GetDelegateForFunctionPointer(refresh, typeof(ThemeAction));
        }

        public static void ApplyDarkMode(bool dark)
        {
            Initialize();
            if (setApplicationMode == null) return; // Keep the platform's native menu on older Windows.
            int mode = SystemParameters.HighContrast ? 0 : dark ? 2 : 3; // Default / ForceDark / ForceLight.
            if (mode == previousMode) return;
            if (refreshPolicy != null) refreshPolicy();
            setApplicationMode(mode);
            if (flushMenus != null) flushMenus();
            previousMode = mode;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW")]
        private static extern IntPtr LoadLibraryEx(string file, IntPtr reserved, uint flags);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr module, IntPtr ordinal);
    }
}
