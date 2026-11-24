using System;

namespace Cccc.Application
{
    public sealed class ClipboardContent
    {
        public uint Sequence;
        public uint OwnerProcess;
        public bool Supported;
        public string Preview = "";
        public string TypeLabel = "";
    }
}
