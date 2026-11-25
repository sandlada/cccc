using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // Explorer is queried, never parented, injected into, resized or restyled.
    // UIA providers can stall: only one background query may exist, and the UI
    // never waits for it. Live bar geometry is independent of UIA freshness.
    internal static class TaskbarPlacement
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr FindWindow(string className, string title);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int RegisterWindowMessage(string message);
        internal static readonly int RecreatedMessage = RegisterWindowMessage("TaskbarCreated");
        private sealed class Snapshot
        {
            internal WindowsNativeMethods.WindowRectangle Monitor;
            internal Rect Bar;
            internal Rect[] Occupied;
            internal long Time;
        }
        private static readonly object Gate = new object();
        private static Snapshot snapshot;
        private static bool busy;
        private static int generation;
        private static long lastRequest = -10000;
        private static TaskbarPlacementDecision lastDecision = TaskbarPlacementDecision.WaitingFirstShow;
        internal static TaskbarPlacementDecision LastDecision { get { return lastDecision; } }

        internal static void Invalidate()
        {
            lock (Gate) { snapshot = null; generation++; lastRequest = -10000; }
        }

        internal static void RequestRefresh()
        {
            int expected;
            lock (Gate)
            {
                long now = WindowsNativeMethods.MonotonicMilliseconds;
                if (busy || now - lastRequest < 2000) return;
                busy = true; lastRequest = now; expected = generation;
            }
            Thread probe = new Thread(delegate()
            {
                Snapshot result = null;
                try { result = Probe(); }
                catch (Exception error)
                {
                    // No window names, clipboard content or provider messages.
                    Trace.TraceError("Cccc taskbar probe: {0} (0x{1:X8})", error.GetType().Name, error.HResult);
                }
                finally
                {
                    lock (Gate)
                    {
                        if (generation == expected && result != null) snapshot = result;
                        busy = false;
                    }
                }
            });
            probe.IsBackground = true;
            probe.Name = "Cccc taskbar snapshot";
            probe.SetApartmentState(ApartmentState.MTA);
            probe.Start();
        }

        private static Snapshot Probe()
        {
            long started = WindowsNativeMethods.MonotonicMilliseconds;
            IntPtr window = FindWindow("Shell_TrayWnd", null);
            if (window == IntPtr.Zero || !IsWindowVisible(window)) return null;
            WindowsNativeMethods.WindowRectangle nativeBounds;
            if (!WindowsNativeMethods.GetWindowRect(window, out nativeBounds)) return null;
            Rect bar = ToRect(nativeBounds);
            WindowsNativeMethods.MonitorInformation info = new WindowsNativeMethods.MonitorInformation { Size = (uint)Marshal.SizeOf(typeof(WindowsNativeMethods.MonitorInformation)) };
            if (!WindowsNativeMethods.GetMonitorInfo(WindowsNativeMethods.MonitorFromWindow(window, 2), ref info)) return null;
            if (!ValidBottomBar(ToRect(info.Monitor), ToRect(info.Work), bar)) return null;
            AutomationElement root = AutomationElement.FromHandle(window);
            if (root == null) return null;
            CacheRequest cache = new CacheRequest { TreeScope = TreeScope.Element };
            cache.Add(AutomationElement.IsOffscreenProperty);
            cache.Add(AutomationElement.BoundingRectangleProperty);
            cache.Add(AutomationElement.ControlTypeProperty);
            root = root.GetUpdatedCache(cache);
            Queue<AutomationElement> pending = new Queue<AutomationElement>();
            pending.Enqueue(root);
            List<Rect> occupied = new List<Rect>();
            int visited = 0;
            TreeWalker walker = TreeWalker.ControlViewWalker;
            while (pending.Count != 0)
            {
                if (++visited > 256 || WindowsNativeMethods.MonotonicMilliseconds - started > 1000) return null; // partial results are unsafe
                AutomationElement item = pending.Dequeue();
                AutomationElement.AutomationElementInformation current = item.Cached;
                if (!current.IsOffscreen && item != root)
                {
                    Rect r = current.BoundingRectangle;
                    ControlType type = current.ControlType;
                    bool leaf = type == ControlType.Button || type == ControlType.Text || type == ControlType.Edit ||
                        type == ControlType.ListItem || type == ControlType.MenuItem || type == ControlType.Image;
                    if (leaf && !r.IsEmpty && r.Width > 0 && r.Height > 0 && r.IntersectsWith(bar))
                    {
                        if (r.Width >= bar.Width * .8) return null;
                        r.Intersect(bar); occupied.Add(r);
                    }
                }
                for (AutomationElement child = walker.GetFirstChild(item, cache); child != null; child = walker.GetNextSibling(child, cache))
                {
                    if (pending.Count + visited >= 256 || WindowsNativeMethods.MonotonicMilliseconds - started > 1000) return null;
                    pending.Enqueue(child);
                }
            }
            if (occupied.Count < 3 || WindowsNativeMethods.MonotonicMilliseconds - started > 1000) return null;
            return new Snapshot { Monitor = info.Monitor, Bar = bar, Occupied = occupied.ToArray(), Time = started };
        }

        internal static Point Resolve(WindowsNativeMethods.WindowRectangle monitor, WindowsNativeMethods.WindowRectangle work, int dpi)
        {
            Snapshot value;
            lock (Gate) value = snapshot;
            // These local window-manager calls do not enumerate or wait for an
            // accessibility provider. A cold UIA request no longer forces an
            // above-taskbar placement for the entire copy sequence.
            Rect bar = Rect.Empty;
            IntPtr window = FindWindow("Shell_TrayWnd", null);
            WindowsNativeMethods.WindowRectangle bounds;
            if (window != IntPtr.Zero && IsWindowVisible(window) && WindowsNativeMethods.GetWindowRect(window, out bounds)) bar = ToRect(bounds);
            Rect[] occupied = value != null && SameMonitor(monitor, value.Monitor) && value.Bar == bar ? value.Occupied : null;
            TaskbarPlacementDecision decision;
            Point result = ResolveForBar(ToRect(monitor), ToRect(work), bar, occupied, dpi / 96.0, out decision);
            if (occupied != null && WindowsNativeMethods.MonotonicMilliseconds - value.Time > 5000 && decision == TaskbarPlacementDecision.InsideAvoidingControls)
                decision = TaskbarPlacementDecision.InsideCachedRefreshing;
            lastDecision = decision;
            return result;
        }

        internal static bool ValidBottomBar(Rect monitor, Rect work, Rect bar)
        {
            return !bar.IsEmpty && bar.Width > 0 && bar.Height > 0 &&
                Math.Abs(bar.Left - monitor.Left) <= 2 && Math.Abs(bar.Right - monitor.Right) <= 2 &&
                Math.Abs(bar.Bottom - monitor.Bottom) <= 2 && bar.Top >= monitor.Top &&
                work.Bottom <= bar.Top + 2; // auto-hide leaves no reserved work area
        }

        // Pure decision logic used by the offscreen checks. Unknown controls
        // deliberately use a trial anchor, not a claim of a verified empty slot.
        internal static Point ResolveForBar(Rect monitor, Rect work, Rect bar, Rect[] occupied, double scale, out TaskbarPlacementDecision decision)
        {
            Point fallback = new Point(work.Left + 16 * scale, work.Bottom - 42 * scale);
            if (!ValidBottomBar(monitor, work, bar))
            { decision = TaskbarPlacementDecision.AboveNotOnMonitorOrHidden; return fallback; }
            Point inside;
            if (!TryFindSlot(bar, occupied ?? new Rect[0], scale, out inside))
            { decision = TaskbarPlacementDecision.AboveInsufficientSpace; return fallback; }
            decision = occupied == null ? TaskbarPlacementDecision.InsideTrialAnchor : TaskbarPlacementDecision.InsideAvoidingControls;
            return inside;
        }

        // Pure geometry: checks include pulse growth.
        internal static bool TryFindSlot(Rect bar, Rect[] occupied, double scale, out Point position)
        {
            position = new Point();
            if (scale <= 0 || bar.Width <= 0 || bar.Height < (32 * 1.06 + 4) * scale) return false;
            double bleed = 8 * scale, width = 225 * scale, gap = 8 * scale;
            double cursor = bar.Left + 8 * scale;
            double limit = bar.Right - 120 * scale; // also reserve the notification area
            List<Rect> sorted = new List<Rect>(occupied);
            sorted.Sort(delegate(Rect a, Rect b) { return a.Left.CompareTo(b.Left); });
            foreach (Rect r in sorted)
            {
                if (!r.IntersectsWith(bar)) continue;
                if (cursor + width + 2 * bleed + gap <= r.Left) break;
                if (cursor < r.Right + gap) cursor = r.Right + gap;
            }
            if (cursor + width + 2 * bleed > limit) return false;
            position = new Point(cursor + bleed, bar.Top + (bar.Height - 32 * scale) / 2);
            return true;
        }

        private static Rect ToRect(WindowsNativeMethods.WindowRectangle value)
        { return value.Right > value.Left && value.Bottom > value.Top ? new Rect(value.Left, value.Top, value.Right - value.Left, value.Bottom - value.Top) : Rect.Empty; }
        private static bool SameMonitor(WindowsNativeMethods.WindowRectangle a, WindowsNativeMethods.WindowRectangle b)
        { return a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom; }
    }
}
