namespace Cccc.Application
{
    public sealed class ChangePreferencesCommand
    {
        public bool? ShowPreview;
        public bool? UseMaterial;
        public LayoutMode? Layout;
        public DisplayMode? DisplayMode;
        public int? CornerRadius;
        public bool? FullCornerRadius;
        public int? CounterCornerRadius;
        public bool? FullCounterCornerRadius;
        public int? Padding;
        public bool? ShowBorder;
        public bool? ShowShadow;
        public string FontFamily;
        public WindowPosition? Position;
        public int? OffsetX;
        public int? OffsetY;
        public bool? AdaptToTaskbar;
        public string Language;
    }
}
