using Cccc.Domain.Preferences;

namespace Cccc.Application
{
    public enum DisplayMode
    {
        Full = 0,
        IconOnly = 1,
        IconAndLabel = 2,
        IconAndContent = 3,
        LabelOnly = 4,
        ContentOnly = 5,
        LabelAndContent = 6
    }

    public static class AppearanceBounds
    {
        public const int MinCornerRadius = ToastAppearancePolicy.MinCornerRadius;
        public const int MaxCornerRadius = ToastAppearancePolicy.MaxCornerRadius;
        public const int DefaultCornerRadius = ToastAppearancePolicy.DefaultCornerRadius;

        public const int MinCounterCornerRadius = ToastAppearancePolicy.MinCounterCornerRadius;
        public const int MaxCounterCornerRadius = ToastAppearancePolicy.MaxCounterCornerRadius;
        public const int DefaultCounterCornerRadius = ToastAppearancePolicy.DefaultCounterCornerRadius;

        public const int MinPadding = ToastAppearancePolicy.MinPadding;
        public const int MaxPadding = ToastAppearancePolicy.MaxPadding;
        public const int DefaultPadding = ToastAppearancePolicy.DefaultPadding;
        public const string DefaultFontFamily = ToastAppearancePolicy.DefaultFontFamily;

        public static int ClampCornerRadius(int value) => ToastAppearancePolicy.ClampCornerRadius(value);
        public static int ClampCounterCornerRadius(int value) => ToastAppearancePolicy.ClampCounterCornerRadius(value);
        public static int ClampPadding(int value) => ToastAppearancePolicy.ClampPadding(value);
        public static string NormalizeFontFamily(string value) => ToastAppearancePolicy.NormalizeFontFamily(value);
        public static double ComputeCardHeight(DisplayMode mode, int padding) =>
            ToastAppearancePolicy.ComputeCardHeight((ToastDisplayMode)mode, padding);
    }
}
