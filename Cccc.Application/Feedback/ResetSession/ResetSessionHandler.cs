namespace Cccc.Application
{
    public sealed class ResetSessionHandler
    {
        private readonly FeedbackSession session;
        private readonly IClipboardReader clipboard;

        public ResetSessionHandler(FeedbackSession session, IClipboardReader clipboard)
        {
            this.session = session;
            this.clipboard = clipboard;
        }

        public void Handle()
        {
            session.Reset(clipboard.CurrentSequence);
        }
    }
}
