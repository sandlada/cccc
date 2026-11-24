using System;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // One title at a time: completely fade out before replacing the text.
    // Time is supplied by the caller so rapid updates need no extra timer.
    internal sealed class TitleTransition
    {
        internal const long OutDuration = 20, InDuration = 45;
        private enum Phase { Still, Out, In }
        private Phase phase;
        private long started;
        private double from;
        internal string Text { get; private set; }
        internal string Target { get; private set; }
        internal double Opacity { get; private set; }
        internal bool Running { get { return phase != Phase.Still; } }
        internal bool AwaitingResult { get; private set; }

        internal TitleTransition() { Reset(""); }

        internal void Reset(string text)
        {
            Text = Target = text ?? "";
            Opacity = from = 1;
            phase = Phase.Still;
            AwaitingResult = false;
        }

        // Start responding at the key event, while keeping the last verified
        // title. A slow source lets that title fade back in rather than leaving
        // the title blank or prematurely displaying a success message.
        internal void Prepare(long now)
        {
            Advance(now);
            AwaitingResult = true;
            if (phase == Phase.Out) return;
            from = Opacity;
            started = now;
            phase = Phase.Out;
        }

        internal void Request(string text, long now, bool animate)
        {
            text = text ?? "";
            if (!animate) { Reset(text); return; }
            Advance(now);
            AwaitingResult = false;
            if (text == Target) return;
            Target = text;
            // While fading out, replace the destination without restarting.
            if (phase == Phase.Out) return;
            from = Opacity;
            started = now;
            phase = Phase.Out;
        }

        internal void Advance(long now)
        {
            if (phase == Phase.Out)
            {
                double elapsed = Math.Max(0, now - started);
                if (elapsed < OutDuration)
                {
                    Opacity = from * (1 - Smooth(elapsed / OutDuration));
                    return;
                }
                Opacity = 0;
                Text = Target;
                started += OutDuration;
                phase = Phase.In;
            }
            if (phase == Phase.In)
            {
                double elapsed = Math.Max(0, now - started);
                Opacity = Smooth(elapsed / InDuration);
                if (elapsed >= InDuration) { Opacity = 1; phase = Phase.Still; }
            }
        }

        private static double Smooth(double t)
        { t = Math.Min(1, Math.Max(0, t)); return t * t * (3 - 2 * t); }
    }
}
