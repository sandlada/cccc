namespace Cccc.Domain.Feedback
{
    // Pure state machine. Time and clipboard observations are supplied by the host.
    public sealed class FeedbackEngine
    {
        public const long BurstMilliseconds = 3000;
        public const long ConfirmMilliseconds = 1500;
        private long lastPress = -10000;
        private long burstStart;
        private WindowIdentity burstWindow;
        private int count;
        private Attempt pending;
        private ClipboardSnapshot confirmed;
        private ClipboardSequence lastObserved;

        private sealed class Attempt
        {
            internal long Start;
            internal ClipboardSequence Baseline;
            internal WindowIdentity Window;
            internal int Count;
        }

        public bool HasPending => pending is not null;
        public int Count => count;

        public void Reset(ClipboardSequence sequence)
        {
            pending = null;
            confirmed = null;
            lastObserved = sequence;
            count = 0;
            lastPress = -10000;
            burstWindow = default;
        }

        public void CopyPressed(long now, WindowIdentity window, ClipboardSequence baseline)
        {
            if (window != burstWindow || now - lastPress > BurstMilliseconds || count == 0)
            {
                count = 0;
                burstStart = now;
                confirmed = null;
            }
            else if (confirmed is not null && confirmed.Sequence != baseline)
            {
                confirmed = null;
            }
            if (count < int.MaxValue) count++;
            burstWindow = window;
            lastPress = now;
            pending = new()
            {
                Start = now,
                Baseline = baseline,
                Window = window,
                Count = count
            };
        }

        public Feedback Observe(ClipboardSnapshot snapshot, long eventTime, long now, WindowIdentity foreground)
        {
            if (snapshot is null || snapshot.Sequence == lastObserved) return null;
            lastObserved = snapshot.Sequence;

            if (pending is not null)
            {
                // A read can complete after the next key press. Remember a verified
                // earlier result without claiming that the newest key press worked.
                if (eventTime < pending.Start)
                {
                    if (snapshot.Supported && eventTime >= burstStart &&
                        snapshot.Sequence == pending.Baseline &&
                        snapshot.OwnerProcess != 0 && snapshot.OwnerProcess == pending.Window.ProcessId)
                        confirmed = snapshot;
                    return null;
                }
                if (now - pending.Start >= ConfirmMilliseconds)
                    return Expire(now, snapshot.Sequence);
                if (snapshot.Sequence == pending.Baseline) return null;

                Attempt attempt = pending;
                pending = null;
                bool matchesSource = snapshot.OwnerProcess != 0 &&
                    snapshot.OwnerProcess == attempt.Window.ProcessId;
                if (snapshot.Supported && matchesSource)
                {
                    confirmed = snapshot;
                    return FromSnapshot(snapshot, FeedbackKind.Confirmed,
                        SuccessTitleKey(attempt.Count), attempt.Count, attempt.Window);
                }
                confirmed = null;
                return FromSnapshot(snapshot, FeedbackKind.Updated, FeedbackTitleKey.ClipboardUpdated, 0, foreground);
            }

            // No Ctrl+C: e.g. a context-menu copy, screenshot, or another app.
            count = 0;
            confirmed = null;
            lastPress = -10000;
            return FromSnapshot(snapshot, snapshot.Supported ? FeedbackKind.Confirmed : FeedbackKind.Updated,
                snapshot.Supported ? FeedbackTitleKey.Copied : FeedbackTitleKey.ClipboardUpdated, 0, foreground);
        }

        public Feedback Expire(long now, ClipboardSequence currentSequence)
        {
            if (pending is null)
            {
                if (now - lastPress > BurstMilliseconds) { confirmed = null; count = 0; }
                return null;
            }
            if (now - pending.Start < ConfirmMilliseconds) return null;
            Attempt attempt = pending;
            pending = null;
            if (confirmed is not null && confirmed.Sequence == currentSequence &&
                confirmed.Sequence == attempt.Baseline)
                return FromSnapshot(confirmed, FeedbackKind.Retained, FeedbackTitleKey.RetainedPrevious,
                    attempt.Count, attempt.Window);
            confirmed = null;
            return new()
            {
                Kind = FeedbackKind.Unconfirmed,
                TitleKey = FeedbackTitleKey.Unconfirmed,
                ContentKind = ClipboardPayloadKind.Unsupported,
                Count = attempt.Count,
                Foreground = attempt.Window
            };
        }

        private static Feedback FromSnapshot(ClipboardSnapshot value, FeedbackKind kind,
            FeedbackTitleKey titleKey, int count, WindowIdentity foreground)
        {
            return new()
            {
                Kind = kind,
                TitleKey = titleKey,
                ContentKind = value.ContentKind,
                TextPreview = value.TextPreview ?? "",
                FileName = value.FileName ?? "",
                FileCount = value.FileCount,
                Count = count,
                Foreground = foreground
            };
        }

        public static FeedbackTitleKey SuccessTitleKey(int times) => times switch
        {
            1 => FeedbackTitleKey.Copied,
            2 => FeedbackTitleKey.StillThere,
            3 => FeedbackTitleKey.ReallyCopied,
            4 => FeedbackTitleKey.DefinitelyCopied,
            >= 5 => FeedbackTitleKey.GoPaste,
            _ => FeedbackTitleKey.Copied
        };
    }
}
