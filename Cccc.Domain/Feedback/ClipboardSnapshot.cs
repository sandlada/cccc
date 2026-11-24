namespace Cccc.Domain.Feedback
{
    public sealed class ClipboardSnapshot
    {
        public ClipboardSequence Sequence;
        public uint OwnerProcess;
        public bool Supported;
        public string Preview = "";
        public string TypeLabel = "";
    }
}
