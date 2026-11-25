using Cccc.Domain.Feedback;

namespace Cccc.Application
{
    public static class FeedbackMapper
    {
        public static FeedbackView ToView(Feedback feedback, ILocalizer localizer)
        {
            if (feedback is null) return null;
            ILocalizer l10n = localizer ?? DefaultLocalizer.Instance;

            string title = ResolveTitle(feedback.TitleKey, l10n);
            (string preview, string typeLabel) = ResolvePayload(feedback, l10n);

            return new()
            {
                Kind = (FeedbackViewKind)feedback.Kind,
                Title = title,
                Preview = preview,
                TypeLabel = typeLabel,
                Count = feedback.Count,
                CountText = feedback.DisplayCount.Format(),
                Foreground = ToTarget(feedback.Foreground)
            };
        }

        public static string ResolveTitle(FeedbackTitleKey titleKey, ILocalizer localizer)
        {
            ILocalizer l10n = localizer ?? DefaultLocalizer.Instance;
            return titleKey switch
            {
                FeedbackTitleKey.Copied => l10n.Get("feedback.title.copied"),
                FeedbackTitleKey.StillThere => l10n.Get("feedback.title.still_there"),
                FeedbackTitleKey.ReallyCopied => l10n.Get("feedback.title.really_copied"),
                FeedbackTitleKey.DefinitelyCopied => l10n.Get("feedback.title.definitely_copied"),
                FeedbackTitleKey.GoPaste => l10n.Get("feedback.title.go_paste"),
                FeedbackTitleKey.ClipboardUpdated => l10n.Get("feedback.title.clipboard_updated"),
                FeedbackTitleKey.RetainedPrevious => l10n.Get("feedback.title.retained_previous"),
                FeedbackTitleKey.Unconfirmed => l10n.Get("feedback.title.unconfirmed"),
                _ => l10n.Get("feedback.title.copied")
            };
        }

        public static string ResolveTaskbarPlacement(TaskbarPlacementDecision decision, ILocalizer localizer)
        {
            ILocalizer l10n = localizer ?? DefaultLocalizer.Instance;
            return decision switch
            {
                TaskbarPlacementDecision.AboveNotOnMonitorOrHidden => l10n.Get("taskbar.placement.above_hidden"),
                TaskbarPlacementDecision.AboveInsufficientSpace => l10n.Get("taskbar.placement.above_no_space"),
                TaskbarPlacementDecision.InsideTrialAnchor => l10n.Get("taskbar.placement.inside_trial"),
                TaskbarPlacementDecision.InsideAvoidingControls => l10n.Get("taskbar.placement.inside_avoiding"),
                TaskbarPlacementDecision.InsideCachedRefreshing => l10n.Get("taskbar.placement.inside_cached"),
                _ => l10n.Get("taskbar.placement.waiting")
            };
        }

        internal static WindowTarget ToTarget(WindowIdentity identity) => new(identity.Handle, identity.ProcessId);

        private static (string Preview, string TypeLabel) ResolvePayload(Feedback feedback, ILocalizer l10n)
        {
            if (feedback.Kind == FeedbackKind.Unconfirmed)
            {
                return (
                    l10n.Get("feedback.unconfirmed.preview"),
                    l10n.Get("feedback.unconfirmed.type")
                );
            }

            return feedback.ContentKind switch
            {
                ClipboardPayloadKind.Text => (
                    string.IsNullOrEmpty(feedback.TextPreview)
                        ? l10n.Get("clipboard.blank_text")
                        : feedback.TextPreview,
                    l10n.Get("clipboard.type.text")
                ),
                ClipboardPayloadKind.Image => (
                    l10n.Get("clipboard.type.image"),
                    l10n.Get("clipboard.type.image")
                ),
                ClipboardPayloadKind.Files => ResolveFilesPayload(feedback.FileName, feedback.FileCount, l10n),
                _ => (
                    l10n.Get("clipboard.preview.unsupported"),
                    l10n.Get("clipboard.type.other")
                )
            };
        }

        private static (string Preview, string TypeLabel) ResolveFilesPayload(string fileName, int fileCount, ILocalizer l10n)
        {
            string shortName = PreviewText.Shorten(fileName);
            if (string.IsNullOrEmpty(shortName))
                shortName = l10n.Get("clipboard.type.file");

            if (fileCount > 1)
            {
                return (
                    l10n.Format("clipboard.preview.files_count", shortName, fileCount),
                    l10n.Format("clipboard.type.files_count", fileCount)
                );
            }

            return (shortName, l10n.Get("clipboard.type.file"));
        }

        private static class DefaultLocalizer
        {
            internal static readonly LocalizationService Instance = new();
        }
    }
}
