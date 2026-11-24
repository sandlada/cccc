using System;

namespace Cccc.Domain.Preferences
{
    public static class ToastAppearancePolicy
    {
        public const int MinCornerRadius = 0;
        public const int MaxCornerRadius = 48;
        public const int DefaultCornerRadius = 26;

        public const int MinPadding = 4;
        public const int MaxPadding = 36;
        public const int DefaultPadding = 20;

        public static int ClampCornerRadius(int value) =>
            Math.Clamp(value, MinCornerRadius, MaxCornerRadius);

        public static int ClampPadding(int value) =>
            Math.Clamp(value, MinPadding, MaxPadding);

        public static double ResolveCornerRadius(int cornerRadius, bool fullCornerRadius, double cardWidth, double cardHeight)
        {
            double maxAllowed = Math.Max(0, Math.Min(cardWidth, cardHeight) / 2.0);
            if (fullCornerRadius) return maxAllowed;
            return Math.Min(ClampCornerRadius(cornerRadius), maxAllowed);
        }
    }
}
