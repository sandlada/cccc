using Cccc.Domain.Feedback;

namespace Cccc.Application
{
    internal static class FeedbackMapper
    {
        internal static FeedbackView ToView(Feedback feedback)
        {
            if (feedback is null) return null;
            return new()
            {
                Kind = (FeedbackViewKind)feedback.Kind,
                Title = feedback.Title,
                Preview = feedback.Preview,
                TypeLabel = feedback.TypeLabel,
                Count = feedback.Count,
                CountText = feedback.DisplayCount.Format(),
                Foreground = ToTarget(feedback.Foreground)
            };
        }

        internal static WindowTarget ToTarget(WindowIdentity identity) => new(identity.Handle, identity.ProcessId);
    }
}
