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
        public string FontFamily = AppearanceBounds.DefaultFontFamily;
        public WindowPosition Position = WindowPosition.BottomLeft;
        public int OffsetX = PositionBounds.DefaultOffset;
        public int OffsetY = PositionBounds.DefaultOffset;
        public bool AdaptToTaskbar = true;
        public string Language = LanguageSettings.SystemTag;
    }
}
