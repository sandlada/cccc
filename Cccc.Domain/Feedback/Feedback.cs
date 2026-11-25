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
        public FeedbackTitleKey TitleKey = FeedbackTitleKey.Copied;
        public ClipboardPayloadKind ContentKind = ClipboardPayloadKind.Unsupported;
        public string TextPreview = "";
        public string FileName = "";
        public int FileCount;
        public int Count;
        public WindowIdentity Foreground;

        public CappedCount DisplayCount => new(Count);
    }
}
