using Cccc.Domain.Feedback;

namespace Cccc.Application
{
    public sealed class RecordCopyAttemptHandler
    {
        private readonly FeedbackSession session;
        private readonly ISystemClock clock;
        private readonly IForegroundWindowQuery foreground;
        private readonly IClipboardReader clipboard;
        private readonly IFeedbackPresenter presenter;
        private readonly PreferencesService preferences;

        public RecordCopyAttemptHandler(FeedbackSession session, ISystemClock clock,
            IForegroundWindowQuery foreground, IClipboardReader clipboard,
            IFeedbackPresenter presenter, PreferencesService preferences)
        {
            this.session = session;
            this.clock = clock;
            this.foreground = foreground;
            this.clipboard = clipboard;
            this.presenter = presenter;
            this.preferences = preferences;
        }

        public RecordCopyAttemptResult Handle()
        {
            long now = clock.CurrentTimeMilliseconds;
            WindowTarget source = foreground.Current;
            uint baseline = clipboard.CurrentSequence;
            session.RecordCopy(now, source, baseline);
            presenter.CopyPressed(new()
            {
                Count = session.Count,
                CountText = new CappedCount(session.Count).Format(),
                Source = source,
                CurrentTimeMilliseconds = now,
                ShowPreview = preferences.Current.ShowPreview
            });
            return new()
            {
                Count = session.Count,
                Source = source,
                CurrentTimeMilliseconds = now
            };
        }
    }
}
