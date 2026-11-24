namespace Cccc.Domain.Preferences
{
    public enum ToastDisplayMode
    {
        Full = 0,
        IconOnly = 1,
        IconAndLabel = 2,
        IconAndContent = 3,
        LabelOnly = 4,
        ContentOnly = 5,
        LabelAndContent = 6
    }

    public static class ToastDisplayPolicy
    {
        public static bool ShowsIcon(ToastDisplayMode mode) =>
            mode is ToastDisplayMode.Full
                or ToastDisplayMode.IconOnly
                or ToastDisplayMode.IconAndLabel
                or ToastDisplayMode.IconAndContent;

        public static bool ShowsLabel(ToastDisplayMode mode) =>
            mode is ToastDisplayMode.Full
                or ToastDisplayMode.IconAndLabel
                or ToastDisplayMode.LabelOnly
                or ToastDisplayMode.LabelAndContent;

        public static bool ShowsContent(ToastDisplayMode mode) =>
            mode is ToastDisplayMode.Full
                or ToastDisplayMode.IconAndContent
                or ToastDisplayMode.ContentOnly
                or ToastDisplayMode.LabelAndContent;
    }
}
