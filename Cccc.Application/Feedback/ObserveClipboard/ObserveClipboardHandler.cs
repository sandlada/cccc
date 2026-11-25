using System.Threading;
using System.Threading.Tasks;
using Cccc.Domain.Feedback;

namespace Cccc.Application
{
    public sealed class ObserveClipboardHandler
    {
        private readonly FeedbackSession session;
        private readonly ISystemClock clock;
        private readonly IClipboardReader clipboard;
        private readonly IForegroundWindowQuery foreground;
        private readonly IFeedbackPresenter presenter;
        private readonly ILocalizer localizer;

        public ObserveClipboardHandler(FeedbackSession session, ISystemClock clock,
            IClipboardReader clipboard, IForegroundWindowQuery foreground,
            IFeedbackPresenter presenter, ILocalizer localizer = null)
        {
            this.session = session;
            this.clock = clock;
            this.clipboard = clipboard;
            this.foreground = foreground;
            this.presenter = presenter;
            this.localizer = localizer;
        }

        public async Task<FeedbackView> HandleAsync(long eventTime, CancellationToken cancellationToken)
        {
            int expectedGeneration = session.Generation;
            uint sequence = clipboard.CurrentSequence;
            long deadline = eventTime + FeedbackEngine.ConfirmMilliseconds;
            ClipboardContent content = await clipboard.ReadAsync(sequence, deadline, cancellationToken)
                .ConfigureAwait(true);
            if (content is null || session.Generation != expectedGeneration) return null;
            if (clock.CurrentTimeMilliseconds - eventTime >= FeedbackEngine.ConfirmMilliseconds) return null;

            Feedback feedback = session.Observe(content, eventTime, clock.CurrentTimeMilliseconds, foreground.Current);
            if (feedback is null) return null;

            FeedbackView view = FeedbackMapper.ToView(feedback, localizer);
            presenter.Present(view);
            return view;
        }
    }
}
