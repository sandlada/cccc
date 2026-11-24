using System;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // Same bounded analytic curves as the approved interactive preview.
    internal sealed class CopyMotion
    {
        internal const double CardAmount = 0, CounterAmount = .18;
        internal const long Grow = 55, Return = 155, Duration = Grow + Return;
        internal const double Softness = 2.8;
        private long started;
        private double initial;
        internal bool Running { get; private set; }
        internal void Pulse(long now) { initial = Value(now); started = now; Running = true; }
        internal double Value(long now)
        {
            if (!Running) return 0;
            double elapsed = Math.Max(0, now - started);
            if (elapsed >= Duration) { Running = false; return 0; }
            if (elapsed < Grow) { double t = elapsed / Grow; return initial + (1 - initial) * (1 - Math.Pow(1 - t, 3)); }
            double u = Math.Min(1, (elapsed - Grow) / Return);
            return Math.Pow(1 - u, Softness) * (1 + Softness * u);
        }
        internal void Stop() { Running = false; initial = 0; }
    }
}
