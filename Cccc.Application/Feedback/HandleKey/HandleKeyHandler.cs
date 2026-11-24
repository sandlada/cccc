namespace Cccc.Application
{
    public sealed class HandleKeyHandler
    {
        private readonly FeedbackSession session;
        private readonly RecordCopyAttemptHandler recordCopy;

        public HandleKeyHandler(FeedbackSession session, RecordCopyAttemptHandler recordCopy)
        {
            this.session = session;
            this.recordCopy = recordCopy;
        }

        public bool Handle(HandleKeyCommand command)
        {
            bool detected = session.FeedKey(command.Key, command.Down, command.Control,
                command.AltOrShiftOrWin);
            if (!detected || session.Paused) return false;
            recordCopy.Handle();
            return true;
        }
    }
}
