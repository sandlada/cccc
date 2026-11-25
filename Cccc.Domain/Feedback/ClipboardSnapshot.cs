namespace Cccc.Domain.Feedback
{
    public sealed class ClipboardSnapshot
    {
        public ClipboardSequence Sequence;
        public uint OwnerProcess;
        public bool Supported;
        public ClipboardPayloadKind ContentKind = ClipboardPayloadKind.Unsupported;
        public string TextPreview = "";
        public string FileName = "";
        public int FileCount;
    }
}
