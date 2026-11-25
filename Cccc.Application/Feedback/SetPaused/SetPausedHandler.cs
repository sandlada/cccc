namespace Cccc.Application
{
    public sealed class SetPausedHandler
    {
        private readonly FeedbackSession session;
        private readonly IClipboardReader clipboard;
        private readonly IForegroundWindowQuery foreground;
        private readonly IFeedbackPresenter presenter;
        private readonly ILocalizer localizer;

        public SetPausedHandler(FeedbackSession session, IClipboardReader clipboard,
            IForegroundWindowQuery foreground, IFeedbackPresenter presenter,
            ILocalizer localizer = null)
        {
            this.session = session;
            this.clipboard = clipboard;
            this.foreground = foreground;
            this.presenter = presenter;
            this.localizer = localizer;
        }

        public SetPausedResult Handle(bool paused)
        {
            ILocalizer l10n = localizer ?? new LocalizationService();
            session.SetPaused(paused, clipboard.CurrentSequence);
            presenter.Hide();
            FeedbackView feedback = new()
            {
                Kind = FeedbackViewKind.Status,
                Title = l10n.Get(paused ? "status.paused.title" : "status.resumed.title"),
                Preview = l10n.Get(paused ? "status.paused.detail" : "status.resumed.detail"),
                Foreground = foreground.Current
            };
            presenter.Present(feedback);
            return new() { Paused = paused, Feedback = feedback };
        }
    }
}
