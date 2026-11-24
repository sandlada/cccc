using System;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal sealed class CounterExpandMotion
    {
        internal const long Duration = 220;
        private long started;
        private double from;

        internal double Progress { get; private set; }
        internal double Target { get; private set; }
        internal bool Running { get; private set; }

        internal void Snap(double value)
        {
            Progress = Target = from = Math.Clamp(value, 0, 1);
            Running = false;
        }

        internal void AnimateTo(double target, long now)
        {
            target = Math.Clamp(target, 0, 1);
            if (!Running && Math.Abs(Progress - target) < 0.0001) return;
            if (Running && Math.Abs(Target - target) < 0.0001) return;
            from = Progress;
            Target = target;
            started = now;
            Running = true;
        }

        internal void Advance(long now)
        {
            if (!Running) return;
            double elapsed = Math.Max(0, now - started);
            if (elapsed >= Duration)
            {
                Progress = Target;
                Running = false;
                return;
            }
            double t = elapsed / Duration;
            double eased = 1 - Math.Pow(1 - t, 3);
            Progress = from + (Target - from) * eased;
        }
    }
}
