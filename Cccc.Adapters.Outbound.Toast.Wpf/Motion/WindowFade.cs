using System;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // Whole-window fading is independent of copy pulses. Reversing a fade uses
    // the last displayed opacity, never an unconditional jump to one.
    internal sealed class WindowFade
    {
        internal const double InDuration = 200, OutDuration = 220;
        private long started;
        private double from, duration;
        internal double Opacity { get; private set; }
        internal bool Running { get; private set; }
        internal bool FadingOut { get; private set; }
        internal void Reset(double value)
        { Opacity = Math.Max(0, Math.Min(1, value)); Running = FadingOut = false; }
        internal void BeginIn(double current, long now)
        {
            Reset(current); from = Opacity; started = now;
            duration = InDuration * (1 - from); Running = duration > 0;
        }
        internal void BeginOut(double current, long now)
        {
            Reset(current); from = Opacity; started = now;
            duration = OutDuration; FadingOut = Running = true;
        }
        internal void Advance(long now)
        {
            if (!Running) return;
            double t = Math.Max(0, Math.Min(1, (now - started) / duration));
            double eased = t * t * (3 - 2 * t);
            Opacity = FadingOut ? from * (1 - eased) : from + (1 - from) * eased;
            if (t >= 1) Running = false;
        }
    }
}
