namespace Cccc.Application
{
    public sealed class ShowStatusHandler
    {
        private readonly IForegroundWindowQuery foreground;
        private readonly IFeedbackPresenter presenter;

        public ShowStatusHandler(IForegroundWindowQuery foreground, IFeedbackPresenter presenter)
        {
            this.foreground = foreground;
            this.presenter = presenter;
        }

        public FeedbackView Handle(string title, string detail)
        {
            FeedbackView feedback = new()
            {
                Kind = FeedbackViewKind.Status,
                Title = title,
                Preview = detail,
                Foreground = foreground.Current
            };
            presenter.Present(feedback);
            return feedback;
        }
    }
}
