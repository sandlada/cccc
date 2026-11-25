using System;
using System.Runtime.InteropServices;
using Cccc.Domain.Preferences;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal static class TaskbarGeometry
    {
        internal static ScreenRect GetEffectiveWorkArea(
            WindowsNativeMethods.WindowRectangle monitor,
            WindowsNativeMethods.WindowRectangle systemWork,
            bool adaptToTaskbar,
            double scale)
        {
            ScreenRect monitorRect = ToScreenRect(monitor);
            if (!adaptToTaskbar || monitorRect.IsEmpty) return monitorRect;

            ScreenRect workRect = ToScreenRect(systemWork);
            ScreenRect taskbarRect = FindTaskbarOnMonitor(monitorRect);
            return ComputeEffectiveWorkArea(monitorRect, workRect, taskbarRect, adaptToTaskbar, scale);
        }

        internal static ScreenRect ComputeEffectiveWorkArea(
            ScreenRect monitor,
            ScreenRect systemWork,
            ScreenRect taskbarRect,
            bool adaptToTaskbar,
            double scale)
        {
            if (!adaptToTaskbar || monitor.IsEmpty) return monitor;
            if (scale <= 0) scale = 1;

            double left = monitor.Left;
            double top = monitor.Top;
            double right = monitor.Right;
            double bottom = monitor.Bottom;

            if (!systemWork.IsEmpty)
            {
                left = Math.Max(left, systemWork.Left);
                top = Math.Max(top, systemWork.Top);
                right = Math.Min(right, systemWork.Right);
                bottom = Math.Min(bottom, systemWork.Bottom);
            }

            if (!taskbarRect.IsEmpty)
            {
                ScreenRect inter = Intersect(monitor, taskbarRect);
                double autoHidePeekThreshold = 3.0 * scale;
                if (!inter.IsEmpty && inter.Width > autoHidePeekThreshold && inter.Height > autoHidePeekThreshold)
                {
                    bool horizontal = inter.Width >= inter.Height && inter.Width >= monitor.Width * 0.5;
                    bool vertical = inter.Height > inter.Width && inter.Height >= monitor.Height * 0.5;
                    if (horizontal)
                    {
                        double distTop = Math.Abs(inter.Top - monitor.Top);
                        double distBottom = Math.Abs(monitor.Bottom - inter.Bottom);
                        if (distTop <= distBottom)
                            top = Math.Max(top, inter.Bottom);
                        else
                            bottom = Math.Min(bottom, inter.Top);
                    }
                    else if (vertical)
                    {
                        double distLeft = Math.Abs(inter.Left - monitor.Left);
                        double distRight = Math.Abs(monitor.Right - inter.Right);
                        if (distLeft <= distRight)
                            left = Math.Max(left, inter.Right);
                        else
                            right = Math.Min(right, inter.Left);
                    }
                }
            }

            if (right <= left || bottom <= top) return monitor;
            return new ScreenRect(left, top, right, bottom);
        }

        private static ScreenRect FindTaskbarOnMonitor(ScreenRect monitor)
        {
            IntPtr primary = WindowsNativeMethods.FindWindow("Shell_TrayWnd", null);
            if (TryGetTaskbarRectForMonitor(primary, monitor, out ScreenRect primaryRect))
                return primaryRect;

            IntPtr secondary = IntPtr.Zero;
            for (int i = 0; i < 16; i++)
            {
                secondary = WindowsNativeMethods.FindWindowEx(IntPtr.Zero, secondary, "Shell_SecondaryTrayWnd", null);
                if (secondary == IntPtr.Zero) break;
                if (TryGetTaskbarRectForMonitor(secondary, monitor, out ScreenRect secRect))
                    return secRect;
            }

            return default;
        }

        private static bool TryGetTaskbarRectForMonitor(IntPtr hwnd, ScreenRect monitor, out ScreenRect rect)
        {
            rect = default;
            if (hwnd == IntPtr.Zero || !WindowsNativeMethods.IsWindowVisible(hwnd)) return false;
            if (!WindowsNativeMethods.GetWindowRect(hwnd, out WindowsNativeMethods.WindowRectangle bounds)) return false;
            ScreenRect candidate = ToScreenRect(bounds);
            if (candidate.IsEmpty) return false;
            ScreenRect inter = Intersect(monitor, candidate);
            if (inter.IsEmpty) return false;
            rect = candidate;
            return true;
        }

        private static ScreenRect Intersect(ScreenRect a, ScreenRect b)
        {
            double l = Math.Max(a.Left, b.Left);
            double t = Math.Max(a.Top, b.Top);
            double r = Math.Min(a.Right, b.Right);
            double bt = Math.Min(a.Bottom, b.Bottom);
            return r > l && bt > t ? new ScreenRect(l, t, r, bt) : default;
        }

        private static ScreenRect ToScreenRect(WindowsNativeMethods.WindowRectangle value)
        {
            return value.Right > value.Left && value.Bottom > value.Top
                ? new ScreenRect(value.Left, value.Top, value.Right, value.Bottom)
                : default;
        }
    }
}
