using System;

namespace Cccc.Domain.Preferences
{
    public static class ToastAppearancePolicy
    {
        public const int MinCornerRadius = 0;
        public const int MaxCornerRadius = 64;
        public const int DefaultCornerRadius = 26;

        public const int MinCounterCornerRadius = 0;
        public const int MaxCounterCornerRadius = 14;
        public const int DefaultCounterCornerRadius = 9;

        public const int MinPadding = 4;
        public const int MaxPadding = 36;
        public const int DefaultPadding = 20;

        public const int MaxFontFamilyLength = 256;
        public const string DefaultFontFamily =
            "Noto Sans, Noto Sans SC, Noto Sans TC, Noto Sans JP, Noto Sans KR, Segoe UI, Microsoft YaHei UI, Microsoft JhengHei UI, Yu Gothic UI, Malgun Gothic";

        public static string NormalizeFontFamily(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DefaultFontFamily;
            string cleaned = value.Replace('\r', ' ').Replace('\n', ' ').Trim();
            if (cleaned.Length == 0) return DefaultFontFamily;
            return cleaned.Length > MaxFontFamilyLength ? cleaned.Substring(0, MaxFontFamilyLength) : cleaned;
        }

        public static int ClampCornerRadius(int value) =>
            Math.Clamp(value, MinCornerRadius, MaxCornerRadius);

        public static int ClampCounterCornerRadius(int value) =>
            Math.Clamp(value, MinCounterCornerRadius, MaxCounterCornerRadius);

        public static int ClampPadding(int value) =>
            Math.Clamp(value, MinPadding, MaxPadding);

        public static double ResolveCounterCornerRadius(int cornerRadius, bool fullCornerRadius, double counterHeight)
        {
            double maxAllowed = Math.Max(0, counterHeight / 2.0);
            if (fullCornerRadius) return maxAllowed;
            return Math.Min(ClampCounterCornerRadius(cornerRadius), maxAllowed);
        }

        public static double ComputeCardHeight(ToastDisplayMode mode, int padding)
        {
            int pad = ClampPadding(padding);
            bool showIcon = ToastDisplayPolicy.ShowsIcon(mode);
            bool showLabel = ToastDisplayPolicy.ShowsLabel(mode);
            bool showContent = ToastDisplayPolicy.ShowsContent(mode);
            double contentH = (showLabel && showContent) ? 40 : (showLabel ? 22 : (showContent ? 18 : 32));
            return Math.Max(contentH + 2 * pad, showIcon ? 32 + 2 * pad : 24 + 2 * pad);
        }

        public static double ResolveCornerRadius(int cornerRadius, bool fullCornerRadius, double cardWidth, double cardHeight)
        {
            double maxAllowed = Math.Max(0, Math.Min(cardWidth, cardHeight) / 2.0);
            if (fullCornerRadius) return maxAllowed;
            return Math.Min(ClampCornerRadius(cornerRadius), maxAllowed);
        }
    }
}
