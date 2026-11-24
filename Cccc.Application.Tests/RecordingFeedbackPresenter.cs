using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class RecordingFeedbackPresenter : IFeedbackPresenter
    {
        public CopyFeedbackSignal Copy;
        public FeedbackView Presented;
        public int HideCount;

        public void CopyPressed(CopyFeedbackSignal signal) { Copy = signal; }
        public void Present(FeedbackView feedback) { Presented = feedback; }
        public void Hide() { HideCount++; }
    }
}
