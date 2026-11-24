using System;

namespace Cccc.Application
{
    public sealed class CopyFeedbackSignal
    {
        public int Count;
        public string CountText = "";
        public WindowTarget Source = new();
        public long CurrentTimeMilliseconds;
        public bool ShowPreview;
    }
}
