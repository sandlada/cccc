using System;

namespace Cccc.Application
{
    public sealed class FeedbackView
    {
        public FeedbackViewKind Kind;
        public string Title = "";
        public string Preview = "";
        public string TypeLabel = "";
        public int Count;
        public string CountText = "";
        public WindowTarget Foreground = new();
    }
}
