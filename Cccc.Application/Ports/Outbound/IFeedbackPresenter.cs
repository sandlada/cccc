namespace Cccc.Application
{
    public interface IFeedbackPresenter
    {
        void CopyPressed(CopyFeedbackSignal signal);
        void Present(FeedbackView feedback);
        void Hide();
    }
}
