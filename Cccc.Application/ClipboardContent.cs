namespace Cccc.Application
{
    public enum ClipboardContentKind
    {
        Unsupported,
        Text,
        Files,
        Image
    }

    public sealed class ClipboardContent
    {
        public uint Sequence;
        public uint OwnerProcess;
        public bool Supported;
        public ClipboardContentKind ContentKind = ClipboardContentKind.Unsupported;
        public string TextPreview = "";
        public string FileName = "";
        public int FileCount;
    }
}
