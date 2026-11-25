using System;

namespace Cccc.Application
{
    public sealed class PreferencesModel
    {
        public bool ShowPreview = true;
        public bool UseMaterial = true;
        public LayoutMode Layout = LayoutMode.Card;
        public DisplayMode DisplayMode = DisplayMode.Full;
        public int CornerRadius = AppearanceBounds.DefaultCornerRadius;
        public bool FullCornerRadius = false;
        public int CounterCornerRadius = AppearanceBounds.DefaultCounterCornerRadius;
        public bool FullCounterCornerRadius = false;
        public int Padding = AppearanceBounds.DefaultPadding;
        public bool ShowBorder = true;
        public bool ShowShadow = true;
    }
}
