using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal sealed class CardBackgroundCache
    {
        private BackgroundFrame current;
        internal int BuildCount { get; private set; }
        internal int Count { get { return current == null ? 0 : 1; } }
        internal BackgroundFrame Get(int dpi)
        {
            return Get(dpi, CardBackgroundRenderer.LogicalWidth, CardBackgroundRenderer.LogicalHeight,
                CardOutline.CornerExtent, true, true);
        }

        internal BackgroundFrame Get(int dpi, int logicalWidth, int logicalHeight,
            double cornerExtent, bool showBorder, bool showShadow)
        {
            bool blend = ToastProfile.Compact && Themes.MaterialEnabled && Themes.Current.Transparent;
            if (current == null || current.Dpi != dpi || current.Dark != Themes.Current.Dark || current.Acrylic != Themes.UseAcrylic ||
                current.Compact != ToastProfile.Compact || current.TaskbarBlend != blend ||
                current.LogicalWidth != logicalWidth || current.LogicalHeight != logicalHeight ||
                Math.Abs(current.CornerExtent - cornerExtent) > 0.01 ||
                current.ShowBorder != showBorder || current.ShowShadow != showShadow)
            {
                BackgroundFrame replacement = CardBackgroundRenderer.Build(dpi, logicalWidth, logicalHeight,
                    cornerExtent, showBorder, showShadow);
                current = replacement; BuildCount++;
            }
            return current;
        }
    }
}
