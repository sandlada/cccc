namespace Cccc.Application
{
    public sealed class ExpireAttemptHandler
    {
        private readonly FeedbackSession session;
        private readonly ISystemClock clock;
        private readonly IClipboardReader clipboard;
        private readonly IFeedbackPresenter presenter;
        private readonly ILocalizer localizer;

        public ExpireAttemptHandler(FeedbackSession session, ISystemClock clock,
            IClipboardReader clipboard, IFeedbackPresenter presenter, ILocalizer localizer = null)
        {
            this.session = session;
            this.clock = clock;
            this.clipboard = clipboard;
            this.presenter = presenter;
            this.localizer = localizer;
        }

        public FeedbackView Handle()
        {
            long now = clock.CurrentTimeMilliseconds;
            uint sequence = session.HasPending ? clipboard.CurrentSequence : 0;
            var feedback = session.Expire(now, sequence);
            if (feedback is null) return null;

            FeedbackView view = FeedbackMapper.ToView(feedback, localizer);
            presenter.Present(view);
            return view;
        }
    }
}
