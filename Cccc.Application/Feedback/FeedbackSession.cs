using Cccc.Domain.Feedback;

namespace Cccc.Application
{
    public sealed class FeedbackSession
    {
        private readonly FeedbackEngine engine = new();
        private readonly CopyKeyDetector keys = new();

        public int Generation { get; private set; }
        public bool Paused { get; private set; }
        public int Count => engine.Count;
        public bool HasPending => engine.HasPending;

        public void RecordCopy(long now, WindowTarget source, uint baseline) =>
            engine.CopyPressed(now, ToDomain(source), new(baseline));

        public Feedback Observe(ClipboardContent content, long eventTime, long now, WindowTarget foreground)
        {
            ClipboardSnapshot snapshot = new()
            {
                Sequence = new(content.Sequence),
                OwnerProcess = content.OwnerProcess,
                Supported = content.Supported,
                ContentKind = (ClipboardPayloadKind)content.ContentKind,
                TextPreview = content.TextPreview ?? "",
                FileName = content.FileName ?? "",
                FileCount = content.FileCount
            };
            return engine.Observe(snapshot, eventTime, now, ToDomain(foreground));
        }

        public Feedback Expire(long now, uint currentSequence) => engine.Expire(now, new(currentSequence));

        public void Reset(uint sequence)
        {
            engine.Reset(new(sequence));
            keys.Reset();
            Generation++;
        }

        public void SetPaused(bool paused, uint sequence)
        {
            Paused = paused;
            keys.Reset();
            engine.Reset(new(sequence));
            Generation++;
        }

        public bool FeedKey(int key, bool down, bool control, bool altOrShiftOrWin) =>
            keys.Feed(key, down, control, altOrShiftOrWin);

        public void ResetKeys() => keys.Reset();

        internal static WindowIdentity ToDomain(WindowTarget target) => new(target.Handle, target.ProcessId);
    }
}
