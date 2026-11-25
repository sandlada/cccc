using System;

namespace Cccc.Domain.Preferences
{
    public enum ToastPosition
    {
        TopLeft = 0,
        TopCenter = 1,
        TopRight = 2,
        MiddleLeft = 3,
        MiddleRight = 4,
        BottomLeft = 5,
        BottomCenter = 6,
        BottomRight = 7
    }

    public readonly record struct ScreenRect(double Left, double Top, double Right, double Bottom)
    {
        public double Width => Math.Max(0, Right - Left);
        public double Height => Math.Max(0, Bottom - Top);
        public bool IsEmpty => Right <= Left || Bottom <= Top;
    }

    public readonly record struct ScreenPoint(double X, double Y);

    public static class ToastPlacementPolicy
    {
        public const int EdgeMarginDip = 20;
        public const int MinOffset = -400;
        public const int MaxOffset = 400;
        public const int DefaultOffset = 0;

        public static int ClampOffset(int value) =>
            Math.Clamp(value, MinOffset, MaxOffset);

        public static ScreenPoint ComputeCardTopLeft(
            ToastPosition position,
            ScreenRect effectiveWorkArea,
            double cardWidth,
            double cardHeight,
            int offsetX,
            int offsetY,
            double scale)
        {
            if (scale <= 0) scale = 1;
            double margin = EdgeMarginDip * scale;
            double clampedW = Math.Max(0, cardWidth);
            double clampedH = Math.Max(0, cardHeight);

            double baseX = position switch
            {
                ToastPosition.TopLeft or ToastPosition.MiddleLeft or ToastPosition.BottomLeft =>
                    effectiveWorkArea.Left + margin,
                ToastPosition.TopCenter or ToastPosition.BottomCenter =>
                    effectiveWorkArea.Left + (effectiveWorkArea.Width - clampedW) / 2.0,
                ToastPosition.TopRight or ToastPosition.MiddleRight or ToastPosition.BottomRight =>
                    effectiveWorkArea.Right - margin - clampedW,
                _ => effectiveWorkArea.Left + margin
            };

            double baseY = position switch
            {
                ToastPosition.TopLeft or ToastPosition.TopCenter or ToastPosition.TopRight =>
                    effectiveWorkArea.Top + margin,
                ToastPosition.MiddleLeft or ToastPosition.MiddleRight =>
                    effectiveWorkArea.Top + (effectiveWorkArea.Height - clampedH) / 2.0,
                ToastPosition.BottomLeft or ToastPosition.BottomCenter or ToastPosition.BottomRight =>
                    effectiveWorkArea.Bottom - margin - clampedH,
                _ => effectiveWorkArea.Bottom - margin - clampedH
            };

            double x = baseX + ClampOffset(offsetX) * scale;
            double y = baseY + ClampOffset(offsetY) * scale;

            double minX = effectiveWorkArea.Left;
            double maxX = Math.Max(minX, effectiveWorkArea.Right - clampedW);
            double minY = effectiveWorkArea.Top;
            double maxY = Math.Max(minY, effectiveWorkArea.Bottom - clampedH);

            return new ScreenPoint(
                Math.Clamp(x, minX, maxX),
                Math.Clamp(y, minY, maxY));
        }

        public static ScreenPoint GetEntranceSlideVector(ToastPosition position)
        {
            return position switch
            {
                ToastPosition.TopLeft or ToastPosition.TopCenter or ToastPosition.TopRight =>
                    new ScreenPoint(0, -1),
                ToastPosition.BottomLeft or ToastPosition.BottomCenter or ToastPosition.BottomRight =>
                    new ScreenPoint(0, 1),
                ToastPosition.MiddleLeft => new ScreenPoint(-1, 0),
                ToastPosition.MiddleRight => new ScreenPoint(1, 0),
                _ => new ScreenPoint(0, 1)
            };
        }
    }
}
