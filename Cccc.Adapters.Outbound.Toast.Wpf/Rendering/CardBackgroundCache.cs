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
            bool blend = ToastProfile.Compact && Themes.MaterialEnabled && Themes.Current.Transparent;
            if (current == null || current.Dpi != dpi || current.Dark != Themes.Current.Dark || current.Acrylic != Themes.UseAcrylic ||
                current.Compact != ToastProfile.Compact || current.TaskbarBlend != blend)
            {
                BackgroundFrame replacement = CardBackgroundRenderer.Build(dpi);
                current = replacement; BuildCount++;
            }
            return current;
        }
    }
}
