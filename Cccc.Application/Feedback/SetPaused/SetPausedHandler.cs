namespace Cccc.Application
{
    public sealed class SetPausedHandler
    {
        private readonly FeedbackSession session;
        private readonly IClipboardReader clipboard;
        private readonly IForegroundWindowQuery foreground;
        private readonly IFeedbackPresenter presenter;

        public SetPausedHandler(FeedbackSession session, IClipboardReader clipboard,
            IForegroundWindowQuery foreground, IFeedbackPresenter presenter)
        {
            this.session = session;
            this.clipboard = clipboard;
            this.foreground = foreground;
            this.presenter = presenter;
        }

        public SetPausedResult Handle(bool paused)
        {
            session.SetPaused(paused, clipboard.CurrentSequence);
            presenter.Hide();
            FeedbackView feedback = new()
            {
                Kind = FeedbackViewKind.Status,
                Title = paused ? "Cccc 已暂停" : "Cccc 已恢复",
                Preview = paused ? "需要时可从托盘恢复提示" : "现在复制试试吧",
                Foreground = foreground.Current
            };
            presenter.Present(feedback);
            return new() { Paused = paused, Feedback = feedback };
        }
    }
}
