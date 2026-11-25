namespace Cccc.Domain.Feedback
{
    public enum ClipboardPayloadKind
    {
        Unsupported,
        Text,
        Files,
        Image
    }

    public enum FeedbackTitleKey
    {
        Copied,
        StillThere,
        ReallyCopied,
        DefinitelyCopied,
        GoPaste,
        ClipboardUpdated,
        RetainedPrevious,
        Unconfirmed,
        Status
    }
}
