namespace Cccc.Domain.Feedback
{
    public enum FeedbackKind
    {
        Confirmed,
        Retained,
        Unconfirmed,
        Updated,
        Status
    }

    public sealed class Feedback
    {
        public FeedbackKind Kind;
        public string Title = "";
        public string Preview = "";
        public string TypeLabel = "";
        public int Count;
        public WindowIdentity Foreground;

        public CappedCount DisplayCount => new(Count);
    }
}
