using System;

namespace Cccc.Application
{
    public sealed class RecordCopyAttemptResult
    {
        public int Count;
        public WindowTarget Source = new();
        public long CurrentTimeMilliseconds;
    }
}
