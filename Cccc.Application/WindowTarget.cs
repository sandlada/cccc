using System;

namespace Cccc.Application
{
    public sealed class WindowTarget
    {
        public long Handle;
        public uint ProcessId;

        public WindowTarget() { }

        public WindowTarget(long handle, uint processId)
        {
            Handle = handle;
            ProcessId = processId;
        }
    }
}
