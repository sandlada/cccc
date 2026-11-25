using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Cccc.Platform.Windows;

using Cccc.Application;
using Cccc.Domain.Feedback;
using Cccc.MaterialShape;
using Cccc.Domain.Preferences;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal sealed class ToastWindow : Window
    {
        internal static double OuterWidth { get { return CardBackgroundRenderer.LogicalWidth; } }
        internal static double OuterHeight { get { return CardBackgroundRenderer.LogicalHeight; } }
        internal double ActiveOuterWidth { get { return ToastProfile.Compact ? CardBackgroundRenderer.LogicalWidth : baseCardWidth + (expandedCardWidth - baseCardWidth) * counterExpand.Progress + 64; } }
        internal double ActiveOuterHeight { get { return maxOuterHeight; } }
        private readonly Grid surface;
        private readonly Grid counterContent;
        private readonly Grid body;
        private readonly ColumnDefinition iconCol, iconGapCol, textCol, counterCol;
        private readonly StackPanel copyPanel;
        private readonly Image background;
        private readonly Canvas backgroundSlices;
        private readonly Image bgLeft, bgMiddle, bgRight;
        private readonly Border card, badge, counter;
        private readonly SampledText title, detail, symbol, times;
        private readonly SampledText[] glyphs;
        private readonly TextPixelWorkspace textPixels = new TextPixelWorkspace();
        private readonly Dictionary<string, Brush> palette = new Dictionary<string, Brush>();
        private readonly System.Windows.Shapes.Path checkMark;
        private static readonly Geometry CheckGeometry = MakeCheckGeometry();
        private readonly DispatcherTimer hideTimer;
        private readonly CardBackgroundCache backgrounds = new CardBackgroundCache();
        private readonly CopyMotion motion = new CopyMotion();
        private readonly CounterExpandMotion counterExpand = new CounterExpandMotion();
        private readonly TitleTransition titleTransition = new TitleTransition();
        private readonly ToastWindowMotion windowMotion = new ToastWindowMotion();
        private readonly ScaleTransform countScale = new ScaleTransform(1, 1);
        private readonly ScaleTransform windowScale = new ScaleTransform(1, 1);
        private ToastDisplayMode displayMode = ToastDisplayMode.Full;
        private ToastPosition windowPosition = ToastPosition.BottomLeft;
        private int offsetX;
        private int offsetY;
        private bool adaptToTaskbar = true;
        private ScreenRect lastEffectiveWorkArea;
        private readonly DispatcherTimer taskbarWatchTimer;
        private int cornerRadius = ToastAppearancePolicy.DefaultCornerRadius;
        private bool fullCornerRadius;
        private int counterCornerRadius = ToastAppearancePolicy.DefaultCounterCornerRadius;
        private bool fullCounterCornerRadius;
        private int cardPadding = ToastAppearancePolicy.DefaultPadding;
        private bool showBorder = true;
        private bool showShadow = true;
        private readonly ILocalizer localizer;
        private string configuredFontFamily = ToastAppearancePolicy.DefaultFontFamily;
        private FontFamily toastFontFamily = EmbeddedFonts.Family;
        private int maxOuterWidth = CardBackgroundRenderer.LogicalWidth;
        private int maxOuterHeight = CardBackgroundRenderer.LogicalHeight;
        private double baseCardWidth = 298;
        private double expandedCardWidth = 360;
        private double cardHeight = 80;
        private double resolvedCornerExtent = CardOutline.CornerExtent;
        private readonly BadgeShapeView badgeShape;
        private readonly MorphAnimator badgeMotion;
        private readonly MaterialShapeSequence badgeSequence;
        private readonly Cubic[] badgeCubics = new Cubic[1024];
        private double lastBadgeAdvance = double.NegativeInfinity;
        private const double InitialBadgeRotation = 225;
        private const double BadgeSpinDegreesPerSecond = 45;
        private AcrylicHost acrylic;
        private bool rendering, preparing, pendingEntrance, pendingHit, currentPreview = true;
        private bool deferredPulse;
        private long lastCopySignal;
        private double lastCopyQueueMilliseconds;
        private int preparedRevision;
        internal string MaterialFailure { get; private set; }
        internal bool HasNativeMaterial { get { return acrylic != null && Themes.UseAcrylic; } }
        private IntPtr windowHandle, anchorWindow;
        private int revision;
        private bool positioning, refreshQueued, closed;
        private bool taskbarSpotPinned;
        private Point taskbarSpot;
        private IntPtr taskbarMonitor;
        private int taskbarDpi;
        internal FeedbackView Current { get; private set; }
        internal IntPtr Handle { get { return new WindowInteropHelper(this).EnsureHandle(); } }
        internal int BackgroundBuildCount { get { return backgrounds.BuildCount; } }
        internal int BackgroundCacheCount { get { return backgrounds.Count; } }
        internal BackgroundFrame CurrentBackground { get; private set; }
        internal double LastPreparationMilliseconds { get; private set; }
        internal bool TestFailMaterialCreation;
        internal int TestMaterialAttempts;
        internal Action<long, bool> TestFrameObserved;
        internal bool TestPendingEntrance { get { return pendingEntrance; } }
        internal bool TestPendingHit { get { return pendingHit; } }
        internal bool TestMotionRunning { get { return motion.Running; } }
        internal bool TestFramesRunning { get { return rendering; } }
        // The whole-card scale is retired. The constant is kept for the
        // material diagnostics CSV, which records the surface scale per frame.
        internal double TestScale { get { return 1; } }
        internal double TestBadgeRotation { get { return badgeMotion.RotationDegrees; } }
        internal bool TestBadgeMotionRunning { get { return badgeMotion.IsRunning; } }
        internal int TestBadgeCubics { get { return badgeShape.CubicCount; } }
        internal double TestBadgeSpinAngle { get { return badgeShape.AmbientAngle; } }
        internal bool TestBadgeSpinRunning { get { return badgeShape.AmbientRunning; } }
        internal MaterialShapeKind TestBadgeShape { get { return badgeSequence.Current; } }
        internal IntPtr TestMaterialHandle { get { return acrylic == null ? IntPtr.Zero : acrylic.Handle; } }

        private bool previewKeepVisible;
        internal Func<Size, Point?> PreviewPositionProvider { get; set; }
        internal bool PreviewKeepVisible
        {
            get { return previewKeepVisible; }
            set { previewKeepVisible = value; if (value) hideTimer.Stop(); }
        }

        private Brush Brush(string value)
        {
            Brush existing;
            if (palette.TryGetValue(value, out existing)) return existing;
            SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
            color.Freeze(); palette.Add(value, color); return color;
        }

        private static Geometry MakeCheckGeometry()
        {
            Geometry geometry = Geometry.Parse("M3,9 L7,13 L15,5");
            geometry.Freeze();
            return geometry;
        }

        internal ToastWindow() : this(Themes.MaterialEnabled, null) { }
        internal ToastWindow(bool materialEnabled, ILocalizer localizer = null)
        {
            this.localizer = localizer ?? new LocalizationService();
            Themes.MaterialEnabled = materialEnabled;
            if (ToastProfile.Compact) Themes.MaterialAvailable = false;
            Title = this.localizer.Format("toast.window_title", ToastProfile.Label);
            Width = OuterWidth; Height = OuterHeight;
            WindowStyle = WindowStyle.None; ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true; Background = Brushes.Transparent;
            ShowInTaskbar = false; ShowActivated = false; Topmost = true;
            Focusable = false; IsHitTestVisible = false;
            WindowStartupLocation = WindowStartupLocation.Manual;
            UseLayoutRounding = true; SnapsToDevicePixels = true;
            FontFamily = EmbeddedFonts.Family;
            FontWeight = EmbeddedFonts.BodyWeight;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Grayscale);
            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
            surface = new Grid { Width = maxOuterWidth, Height = maxOuterHeight, RenderTransform = windowScale, RenderTransformOrigin = new Point(0.5, 0.5) };
            background = new Image { HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top, Stretch = Stretch.Fill, SnapsToDevicePixels = true };
            RenderOptions.SetBitmapScalingMode(background, BitmapScalingMode.NearestNeighbor);
            surface.Children.Add(background);
            backgroundSlices = new Canvas { HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top, Visibility = Visibility.Collapsed };
            bgLeft = new Image { Stretch = Stretch.Fill, SnapsToDevicePixels = true };
            bgMiddle = new Image { Stretch = Stretch.Fill, SnapsToDevicePixels = true };
            bgRight = new Image { Stretch = Stretch.Fill, SnapsToDevicePixels = true };
            RenderOptions.SetBitmapScalingMode(bgLeft, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(bgMiddle, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(bgRight, BitmapScalingMode.NearestNeighbor);
            backgroundSlices.Children.Add(bgLeft);
            backgroundSlices.Children.Add(bgMiddle);
            backgroundSlices.Children.Add(bgRight);
            surface.Children.Add(backgroundSlices);
            // Transparent content host. All card pixels are in the background layer;
            // no parent Effect; each text element owns its four-times glyph cache.
            card = new Border { Margin = new Thickness(32), Padding = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
                ClipToBounds = true };
            bool compact = ToastProfile.Compact;
            body = new Grid { Margin = compact ? new Thickness(6, 0, 5, 0) : new Thickness(19, 0, 15, 0) };
            iconCol = new ColumnDefinition { Width = new GridLength(compact ? 18 : 34) };
            iconGapCol = new ColumnDefinition { Width = new GridLength(compact ? 8 : 13) };
            textCol = new ColumnDefinition { Width = compact ? new GridLength(1, GridUnitType.Star) : new GridLength(211) };
            counterCol = new ColumnDefinition { Width = new GridLength(compact ? 44 : 68) };
            body.ColumnDefinitions.Add(iconCol);
            body.ColumnDefinitions.Add(iconGapCol);
            body.ColumnDefinitions.Add(textCol);
            body.ColumnDefinitions.Add(counterCol);
            badge = new Border { Width = compact ? 18 : 32, Height = compact ? 18 : 32,
                VerticalAlignment = VerticalAlignment.Center };
            badgeShape = new BadgeShapeView(compact ? 18 : 32) { Fill = Brush("#15976B") };
            badgeSequence = CreateBadgeSequence(compact);
            badgeMotion = new MorphAnimator(
                MaterialShapes.Get(MaterialShapeKind.Circle),
                MotionScheme.Expressive.FastSpatial,
                InitialBadgeRotation);
            badgeShape.SetShape(badgeMotion.SettledShape, badgeMotion.RotationDegrees);
            symbol = new SampledText(textPixels) { Text = "!", FontFamily = EmbeddedFonts.Family,
                FontSize = compact ? 13 : 19, FontWeight = EmbeddedFonts.TitleWeight, Foreground = Brush("#DDF6EB"),
                HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, -1, 0, 0), Visibility = Visibility.Hidden };
            checkMark = new System.Windows.Shapes.Path { Data = CheckGeometry, Width = compact ? 12 : 18, Height = compact ? 12 : 18,
                Stroke = Brush("#DDF6EB"), StrokeThickness = compact ? 1.5 : 2, Stretch = compact ? Stretch.Uniform : Stretch.None,
                StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round, HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center, SnapsToDevicePixels = false };
            Grid badgeContent = new Grid();
            badgeContent.Children.Add(checkMark); badgeContent.Children.Add(symbol);
            Grid badgeVisuals = new Grid();
            badgeVisuals.Children.Add(badgeShape);
            badgeVisuals.Children.Add(badgeContent);
            badge.Child = badgeVisuals; body.Children.Add(badge);
            copyPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            title = new SampledText(textPixels) { FontSize = 14, FontWeight = EmbeddedFonts.TitleWeight,
                Foreground = Brush("#1D1D1F"), TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.NoWrap };
            detail = new SampledText(textPixels) { FontSize = 12, FontWeight = EmbeddedFonts.BodyWeight, Foreground = Brush("#6E6E73"),
                Margin = new Thickness(0, 4, 0, 0), TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.NoWrap };
            if (compact) detail.Visibility = Visibility.Collapsed;
            copyPanel.Children.Add(title); copyPanel.Children.Add(detail);
            Grid.SetColumn(copyPanel, 2); body.Children.Add(copyPanel);
            counter = new Border { CornerRadius = new CornerRadius(compact ? 10 : 9), Background = Brush("#E9E9ED"),
                Padding = compact ? new Thickness(0) : new Thickness(4, 5, 4, 5), Margin = new Thickness(8, 0, 0, 0),
                Width = compact ? 36 : 48, Height = compact ? 20 : 28, HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center, Visibility = Visibility.Hidden };
            FontFamily counterFont = new FontFamily("Bahnschrift, Segoe UI");
            times = new SampledText(textPixels) { FontFamily = counterFont, FontSize = 12, FontWeight = FontWeights.SemiBold, Foreground = Brush("#1D1D1F") };
            times.HorizontalAlignment = HorizontalAlignment.Left;
            times.VerticalAlignment = VerticalAlignment.Center;
            glyphs = new SampledText[] { title, detail, times, symbol };
            // Hinting is specified on the text itself, including detached exports;
            // do not rely on a window-level setting reaching every text visual.
            foreach (SampledText text in glyphs)
            {
                text.Culture = this.localizer.CurrentCulture;
                TextOptions.SetTextFormattingMode(text, TextFormattingMode.Display);
                TextOptions.SetTextRenderingMode(text, TextRenderingMode.Grayscale);
                TextOptions.SetTextHintingMode(text, TextHintingMode.Fixed);
            }
            // Natural counter widths keep the whole sampled glyph group centered.
            TextOptions.SetTextFormattingMode(times, TextFormattingMode.Ideal);
            counterContent = new Grid { Height = compact ? 16 : 18, HorizontalAlignment = HorizontalAlignment.Center };
            counterContent.Children.Add(times);
            counter.RenderTransform = countScale; counter.RenderTransformOrigin = new Point(.5, .5);
            counter.Child = counterContent; Grid.SetColumn(counter, 3); body.Children.Add(counter);
            card.Child = body; surface.Children.Add(card); Content = surface;
            SourceInitialized += delegate
            {
                windowHandle = new WindowInteropHelper(this).Handle;
                WindowsNativeMethods.SetPassiveWindowStyles(windowHandle);
                TryMaterial();
                HwndSource.FromHwnd(windowHandle).AddHook(delegate(IntPtr window, int message, IntPtr w, IntPtr l, ref bool handled)
                {
                    if (message == 0x21) { handled = true; return new IntPtr(3); }
                    if (message == 0x84) { handled = true; return new IntPtr(-1); }
                    if (message == 0x007E || message == 0x001A || message == TaskbarPlacement.RecreatedMessage)
                    { TaskbarPlacement.Invalidate(); taskbarSpotPinned = false; CheckTaskbarChange(); }
                    if (message == 0x02E0 || message == 0x007E || message == 0x001A) QueueDpiRefresh();
                    // WPF must process WM_DPICHANGED itself; do not mark it handled.
                    return IntPtr.Zero;
                });
            };
            Closed += delegate
            {
                closed = true; pendingEntrance = pendingHit = deferredPulse = false;
                revision++; hideTimer.Stop(); taskbarWatchTimer.Stop(); motion.Stop(); windowMotion.Reset(0); StopFrames();
                foreach (SampledText text in glyphs) text.ClearCache();
                textPixels.Release();
                if (acrylic != null) acrylic.Dispose();
            };
            hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            hideTimer.Tick += delegate { FadeOut(); };
            taskbarWatchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
            taskbarWatchTimer.Tick += delegate { CheckTaskbarChange(); };
            RecalculateCardGeometry();
        }

        internal void ApplyPersonalization(PreferencesModel preferences)
        {
            if (closed || preferences == null) return;
            displayMode = (ToastDisplayMode)preferences.DisplayMode;
            cornerRadius = preferences.CornerRadius;
            fullCornerRadius = preferences.FullCornerRadius;
            counterCornerRadius = preferences.CounterCornerRadius;
            fullCounterCornerRadius = preferences.FullCounterCornerRadius;
            cardPadding = preferences.Padding;
            showBorder = preferences.ShowBorder;
            showShadow = preferences.ShowShadow;
            windowPosition = (ToastPosition)preferences.Position;
            offsetX = preferences.OffsetX;
            offsetY = preferences.OffsetY;
            adaptToTaskbar = preferences.AdaptToTaskbar;
            configuredFontFamily = preferences.FontFamily;
            ApplyTypographyForCurrentLanguage();
            RecalculateCardGeometry();
            if (windowHandle != IntPtr.Zero) Position(anchorWindow, snap: !IsVisible);
            if (IsVisible && Current != null) PrepareVisibleText();
        }

        internal void ApplyLanguage()
        {
            if (closed) return;
            Title = localizer.Format("toast.window_title", ToastProfile.Label);
            foreach (SampledText text in glyphs)
                text.Culture = localizer.CurrentCulture;
            ApplyTypographyForCurrentLanguage();
            if (IsVisible && Current != null) PrepareVisibleText();
        }

        private void ApplyTypographyForCurrentLanguage()
        {
            FontFamily nextFont = EmbeddedFonts.ResolveFontFamily(
                configuredFontFamily,
                localizer.CurrentLanguage?.FontFamily);
            if (!Equals(toastFontFamily, nextFont))
            {
                toastFontFamily = nextFont;
                FontFamily = nextFont;
                title.FontFamily = nextFont;
                detail.FontFamily = nextFont;
                symbol.FontFamily = nextFont;
            }
        }

        private void RecalculateCardGeometry()
        {
            double counterH = ToastProfile.Compact ? 20 : 28;
            counter.CornerRadius = new CornerRadius(ToastProfile.Compact && !fullCounterCornerRadius && counterCornerRadius == ToastAppearancePolicy.DefaultCounterCornerRadius
                ? 10
                : ToastAppearancePolicy.ResolveCounterCornerRadius(counterCornerRadius, fullCounterCornerRadius, counterH));
            if (ToastProfile.Compact)
            {
                maxOuterWidth = CardBackgroundRenderer.LogicalWidth;
                maxOuterHeight = CardBackgroundRenderer.LogicalHeight;
                baseCardWidth = 225;
                expandedCardWidth = 225;
                cardHeight = 32;
                resolvedCornerExtent = ToastAppearancePolicy.ResolveCornerRadius(
                    cornerRadius, fullCornerRadius, 225, 32);
                Width = maxOuterWidth;
                Height = maxOuterHeight;
                surface.Width = maxOuterWidth;
                surface.Height = maxOuterHeight;
                card.Width = 225;
                card.Height = 32;
                badge.Visibility = Visibility.Visible;
                title.Visibility = Visibility.Visible;
                detail.Visibility = Visibility.Collapsed;
                return;
            }

            bool showIcon = ToastDisplayPolicy.ShowsIcon(displayMode);
            bool showLabel = ToastDisplayPolicy.ShowsLabel(displayMode);
            bool showContent = ToastDisplayPolicy.ShowsContent(displayMode);
            int pad = ToastAppearancePolicy.ClampPadding(cardPadding);

            double iconW = showIcon ? 34 : 0;
            double gapW = showIcon && (showLabel || showContent) ? 13 : 0;
            double textW = showContent ? 211 : (showLabel ? 84 : 0);
            cardHeight = ToastAppearancePolicy.ComputeCardHeight(displayMode, pad);

            double bodyPadLeft = Math.Max(2, pad - 1);
            double bodyPadRight = Math.Max(2, pad - 1);
            double defaultBase = bodyPadLeft + 1 + iconW + gapW + textW + bodyPadRight + 1;
            // Keep default Full mode at 360x80 (424x144 outer) for exact parity.
            double counterSlot = (displayMode == ToastDisplayMode.Full && pad == ToastAppearancePolicy.DefaultPadding)
                ? 360 - defaultBase
                : 62;
            baseCardWidth = defaultBase;
            expandedCardWidth = baseCardWidth + counterSlot;
            resolvedCornerExtent = ToastAppearancePolicy.ResolveCornerRadius(
                cornerRadius, fullCornerRadius, baseCardWidth, cardHeight);

            maxOuterWidth = (int)Math.Ceiling(expandedCardWidth) + 64;
            maxOuterHeight = (int)Math.Ceiling(cardHeight) + 64;

            Width = maxOuterWidth;
            Height = maxOuterHeight;
            surface.Width = maxOuterWidth;
            surface.Height = maxOuterHeight;
            card.Height = cardHeight;
            body.Margin = new Thickness(bodyPadLeft, 0, bodyPadRight, 0);
            iconCol.Width = new GridLength(iconW);
            iconGapCol.Width = new GridLength(gapW);
            textCol.Width = new GridLength(textW);
            counterCol.Width = new GridLength(counterSlot);

            badge.Visibility = showIcon ? Visibility.Visible : Visibility.Collapsed;
            copyPanel.Visibility = (showLabel || showContent) ? Visibility.Visible : Visibility.Collapsed;
            title.Visibility = showLabel ? Visibility.Visible : Visibility.Collapsed;
            detail.Visibility = showContent ? Visibility.Visible : Visibility.Collapsed;
            detail.Margin = showLabel ? new Thickness(0, 4, 0, 0) : new Thickness(0);

            ApplyCardWidthFrame();
        }

        private void ApplyCardWidthFrame()
        {
            double counterH = ToastProfile.Compact ? 20 : 28;
            counter.CornerRadius = new CornerRadius(ToastProfile.Compact && !fullCounterCornerRadius && counterCornerRadius == ToastAppearancePolicy.DefaultCounterCornerRadius
                ? 10
                : ToastAppearancePolicy.ResolveCounterCornerRadius(counterCornerRadius, fullCounterCornerRadius, counterH));
            if (ToastProfile.Compact)
            {
                card.Width = 225;
                counter.Opacity = Current != null && Current.Count >= 2 ? 1 : 0;
                background.Opacity = 1;
                backgroundSlices.Visibility = Visibility.Collapsed;
                return;
            }
            double p = counterExpand.Progress;
            double currentCardW = baseCardWidth + (expandedCardWidth - baseCardWidth) * p;
            double currentOuterW = currentCardW + 64;
            card.Width = currentCardW;
            counter.Opacity = p <= 0.01 ? 0 : Math.Clamp((p - 0.1) / 0.9, 0, 1);

            if (HasNativeMaterial)
            {
                background.Opacity = 1;
                backgroundSlices.Visibility = Visibility.Collapsed;
                try { acrylic.UpdateCardWidth(currentOuterW, resolvedCornerExtent); }
                catch (Exception error) { MaterialFault(error); }
            }
            else if (CurrentBackground != null && CurrentBackground.LeftSlice != null &&
                     currentCardW < expandedCardWidth - 0.05)
            {
                double visualScale = VisualTreeHelper.GetDpi(this).DpiScaleX;
                if (visualScale <= 0) visualScale = 1;
                double leftW = CurrentBackground.LeftSliceWidth / visualScale;
                double rightW = CurrentBackground.RightSliceWidth / visualScale;
                double frameH = CurrentBackground.Height / visualScale;
                double midW = Math.Max(0, currentOuterW - leftW - rightW);

                bgLeft.Source = CurrentBackground.LeftSlice;
                bgLeft.Width = leftW; bgLeft.Height = frameH;
                Canvas.SetLeft(bgLeft, 0); Canvas.SetTop(bgLeft, 0);

                bgMiddle.Source = CurrentBackground.MiddleSlice;
                bgMiddle.Width = midW; bgMiddle.Height = frameH;
                Canvas.SetLeft(bgMiddle, leftW); Canvas.SetTop(bgMiddle, 0);

                bgRight.Source = CurrentBackground.RightSlice;
                bgRight.Width = rightW; bgRight.Height = frameH;
                Canvas.SetLeft(bgRight, leftW + midW); Canvas.SetTop(bgRight, 0);

                backgroundSlices.Width = currentOuterW;
                backgroundSlices.Height = frameH;
                background.Opacity = 0;
                backgroundSlices.Visibility = Visibility.Visible;
            }
            else
            {
                background.Opacity = 1;
                backgroundSlices.Visibility = Visibility.Collapsed;
            }
        }

        private void SetContent(FeedbackView value, bool preview)
        {
            Current = value;
            currentPreview = preview;
            detail.Text = preview || value.Kind == FeedbackViewKind.Status ? value.Preview : value.TypeLabel;
            counter.Visibility = value.Count >= 2 ? Visibility.Visible : Visibility.Hidden;
            string countText = string.IsNullOrEmpty(value.CountText)
                ? new CappedCount(value.Count).Format()
                : value.CountText;
            times.Text = "×" + countText;
            LayoutCounter();
            bool warning = value.Kind == FeedbackViewKind.Unconfirmed;
            bool neutral = value.Kind == FeedbackViewKind.Updated || value.Kind == FeedbackViewKind.Retained;
            Appearance theme = Themes.Current;
            title.Foreground = Brush(theme.Text); detail.Foreground = Brush(theme.Detail);
            counter.Background = Brush(ToastProfile.Compact && Themes.MaterialEnabled && theme.Transparent ?
                (theme.Dark ? "#18FFFFFF" : "#10000000") : theme.Counter);
            times.Foreground = Brush(theme.CountText);
            checkMark.Visibility = warning || neutral ? Visibility.Hidden : Visibility.Visible;
            symbol.Visibility = warning || neutral ? Visibility.Visible : Visibility.Hidden;
            symbol.Text = warning ? "!" : "·";
            checkMark.Stroke = Brush(theme.Green);
            symbol.Foreground = Brush(warning ? (theme.Dark ? "#FFD383" : "#AE7E25") : neutral ? (theme.Dark ? "#AECBBC" : "#548274") : theme.Green);
            badgeShape.Fill = Brush(warning ? (theme.Dark ? "#554025" : "#FFF0D1") : neutral ? (theme.Dark ? "#2B4237" : "#E7F1EE") : theme.Badge);
            if (value.Kind != FeedbackViewKind.Confirmed)
            {
                // Warning, neutral and status states do not celebrate a copy.
                // Return the badge to the resting circle without a spring.
                badgeSequence.Reset();
                badgeMotion.SnapTo(MaterialShapes.Get(MaterialShapeKind.Circle), InitialBadgeRotation);
                badgeShape.SetShape(badgeMotion.SettledShape, badgeMotion.RotationDegrees);
            }
        }

        private void UpdateTitle(string text, bool animate)
        {
            titleTransition.Request(text, WindowsNativeMethods.MonotonicMilliseconds, animate);
            ApplyTitleFrame();
            if (titleTransition.Running) StartFrames();
        }

        private void ApplyTitleFrame()
        {
            if (title.Text != titleTransition.Text) title.Text = titleTransition.Text;
            title.Opacity = titleTransition.Opacity;
        }

        private void TryMaterial()
        {
            if (ToastProfile.Compact || windowHandle == IntPtr.Zero || acrylic != null || !Themes.MaterialEnabled || !Themes.Current.Transparent) return;
            try
            {
                TestMaterialAttempts++;
                if (TestFailMaterialCreation) throw new InvalidOperationException("Injected material creation failure.");
                acrylic = new AcrylicHost(windowHandle); Themes.MaterialAvailable = true; MaterialFailure = null;
            }
            catch (Exception error) { Themes.MaterialAvailable = false; MaterialFailure = error.GetType().Name + " (0x" + error.HResult.ToString("X8") + ")"; }
        }

        private void ReleaseMaterial()
        {
            AcrylicHost old = acrylic; acrylic = null; Themes.MaterialAvailable = false;
            background.Visibility = Visibility.Visible;
            if (old == null) return;
            try { old.Dispose(); }
            catch (Exception error) { MaterialFailure = error.GetType().Name + " (0x" + error.HResult.ToString("X8") + ")"; }
        }

        internal void SetMaterialEnabled(bool enabled)
        {
            if (closed) return;
            HideAndClear();
            Themes.MaterialEnabled = enabled;
            Themes.Refresh();
            MaterialFailure = null;
            if (!enabled || !Themes.Current.Transparent) ReleaseMaterial();
            else if (windowHandle != IntPtr.Zero) TryMaterial();
            if (windowHandle != IntPtr.Zero) Position(anchorWindow);
        }

        private void MaterialFault(Exception error)
        {
            Console.WriteLine("MATERIAL_FAULT=" + error);
            Themes.MaterialAvailable = false;
            background.Visibility = Visibility.Visible;
            MaterialFailure = error.GetType().Name + " (0x" + error.HResult.ToString("X8") + ")";
            if (acrylic != null) { try { acrylic.Dispose(); } catch (Exception) { } acrylic = null; }
        }

        internal void RefreshAppearance()
        {
            if (closed) return;
            if (!Themes.MaterialEnabled || !Themes.Current.Transparent) ReleaseMaterial();
            else if (windowHandle != IntPtr.Zero) TryMaterial();
            if (Current != null) SetContent(Current, currentPreview);
            if (windowHandle != IntPtr.Zero) Position(anchorWindow);
            if (IsVisible && Current != null) PrepareVisibleText();
        }

        internal void CopyPressed(int count, IntPtr foreground, bool preview)
        { CopyPressed(count, foreground, preview, WindowsNativeMethods.MonotonicMilliseconds); }

        internal void CopyPressed(int count, IntPtr foreground, bool preview, long pressedAt, string countText = null, string expectedTitle = null)
        {
            if (closed) return;
            lastCopySignal = WindowsNativeMethods.MonotonicMilliseconds;
            lastCopyQueueMilliseconds = Math.Max(0, lastCopySignal - pressedAt);
            // Keep the last actual result steady while awaiting the new one.
            // A hidden card first appears with the result, without a blank or
            // provisional message. First appearance is fade-only; later key
            // presses may pulse the already present copy feedback.
            bool repeat = count >= 2 && IsVisible && Current != null && Current.Kind != FeedbackViewKind.Status;
            deferredPulse = repeat;
            if (!repeat) pendingHit = false;
            if (!IsVisible || pendingEntrance || Current == null || Current.Kind == FeedbackViewKind.Status) return;
            deferredPulse = false;
            revision++;
            // Signal title motion before content layout, positioning, or any
            // asynchronous clipboard read. Do not choose an unverified title.
            string targetTitle = string.IsNullOrEmpty(expectedTitle)
                ? FeedbackMapper.ResolveTitle(FeedbackEngine.SuccessTitleKey(count), localizer)
                : expectedTitle;
            if (titleTransition.Target != targetTitle)
            {
                titleTransition.Prepare(lastCopySignal);
                ApplyTitleFrame(); StartFrames();
            }
            Current.Count = count;
            Current.CountText = string.IsNullOrEmpty(countText)
                ? new CappedCount(count).Format()
                : countText;
            SetContent(Current, preview);
            bool wantsCounter = count >= 2;
            counterExpand.AnimateTo(wantsCounter ? 1 : 0, lastCopySignal);
            ApplyCardWidthFrame();
            Position(foreground);
            PrepareVisibleText();
            hideTimer.Stop();
            if (repeat) PlayHit(); else RestoreFade();
            if (counterExpand.Running) StartFrames();
            StartHideTimer();
        }

        private void PlayHit()
        {
            // The first subsequent rendering callback owns the pulse epoch.
            // Preparing glyphs must not eat into the 55 ms grow stage.
            pendingHit = true; StartFrames();
        }

        private void PrepareVisibleText()
        {
            Stopwatch watch = Stopwatch.StartNew();
            preparing = true;
            try
            {
                surface.Measure(new Size(maxOuterWidth, maxOuterHeight));
                surface.Arrange(new Rect(0, 0, maxOuterWidth, maxOuterHeight));
                surface.UpdateLayout();
                foreach (SampledText text in glyphs)
                {
                    if (text.Visibility != Visibility.Visible) continue;
                    if (text == times && counter.Visibility != Visibility.Visible) continue;
                    text.Moving = text == times && (pendingHit || motion.Running);
                    text.PrepareForRender();
                }
                preparedRevision = revision;
            }
            finally { preparing = false; watch.Stop(); LastPreparationMilliseconds = watch.Elapsed.TotalMilliseconds; }
        }

        private void RestoreFade()
        {
            if (windowMotion.FadingOut || (!windowMotion.FadeRunning && Opacity < 1))
                windowMotion.BeginIn(Opacity, WindowsNativeMethods.MonotonicMilliseconds);
        }

        private void LayoutCounter()
        {
            // Center the entire visible counter text rather than a fixed-width box.
            Size available = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            times.Measure(available);
            counterContent.Width = times.DesiredSize.Width;
        }

        internal void Present(FeedbackView value, bool preview)
        {
            if (value == null || closed) return;
            bool playHit = value.Kind != FeedbackViewKind.Status && value.Count >= 2 && (pendingHit || deferredPulse);
            if (!playHit) pendingHit = false;
            deferredPulse = false;
            revision++;
            hideTimer.Stop();
            bool alreadyVisible = IsVisible && !pendingEntrance;
            bool animateTitle = alreadyVisible && !(Current != null && Current.Kind == FeedbackViewKind.Status && value.Kind != FeedbackViewKind.Status);
            if (!alreadyVisible) { Opacity = 0; windowMotion.Reset(0); }
            SetContent(value, preview);
            bool wantsCounter = value.Kind != FeedbackViewKind.Status && value.Count >= 2;
            if (!alreadyVisible) counterExpand.Snap(wantsCounter ? 1 : 0);
            else counterExpand.AnimateTo(wantsCounter ? 1 : 0, WindowsNativeMethods.MonotonicMilliseconds);
            ApplyCardWidthFrame();
            UpdateTitle(value.Title, animateTitle);
            // Position and build for the real target DPI while the new card is hidden.
            Position(value.Foreground == null ? IntPtr.Zero : new IntPtr(value.Foreground.Handle), snap: !alreadyVisible);
            if (!alreadyVisible)
            {
                pendingEntrance = true;
                if (HasNativeMaterial) acrylic.SetMotion(windowMotion.Scale, 0);
                if (!IsVisible) Show();
                // Ensure WPF's source/visual DPI is fully committed after first Show.
                Position(value.Foreground == null ? IntPtr.Zero : new IntPtr(value.Foreground.Handle), snap: true);
            }
            if (!ToastProfile.Compact && adaptToTaskbar) taskbarWatchTimer.Start();
            else taskbarWatchTimer.Stop();
            if (playHit) PlayHit();
            // The blob keeps turning while the card is visible, so the badge
            // reads as alive even between copies. Repeated Present calls do not
            // restart the clock.
            badgeShape.StartAmbientSpin(BadgeSpinDegreesPerSecond);
            PrepareVisibleText();
            if (alreadyVisible) { RestoreFade(); StartHideTimer(); }
            StartFrames();
        }

        private int EffectiveDpi()
        {
            int dpi = windowHandle == IntPtr.Zero ? 0 : WindowsNativeMethods.GetWindowDpi(windowHandle);
            if (dpi <= 0) dpi = (int)Math.Round(VisualTreeHelper.GetDpi(this).DpiScaleX * 96);
            return dpi > 0 ? dpi : 96;
        }

        private void ApplyBackground(int dpi, double visualScale)
        {
            LayoutCounter();
            BackgroundFrame frame = backgrounds.Get(dpi, maxOuterWidth, maxOuterHeight,
                resolvedCornerExtent, showBorder, showShadow);
            background.Visibility = Visibility.Visible;
            if (HasNativeMaterial)
            {
                try { acrylic.SetBackground(frame); background.Visibility = Visibility.Hidden; }
                catch (Exception error) { MaterialFault(error); frame = backgrounds.Get(dpi, maxOuterWidth, maxOuterHeight, resolvedCornerExtent, showBorder, showShadow); }
            }
            background.Source = frame.Bitmap;
            // WPF device scaling * this logical extent equals exact bitmap pixels.
            // This avoids an extra image resample even at fractional display scales.
            background.Width = frame.Width / visualScale;
            background.Height = frame.Height / visualScale;
            CurrentBackground = frame;
            ApplyCardWidthFrame();
        }

        internal void Position(IntPtr foreground, bool snap = true)
        {
            if (closed || positioning) return;
            positioning = true;
            anchorWindow = foreground;
            try
            {
                IntPtr handle = Handle;
                IntPtr monitor = WindowsNativeMethods.MonitorFromWindow(foreground, 2);
                WindowsNativeMethods.MonitorInformation info = new WindowsNativeMethods.MonitorInformation { Size = (uint)Marshal.SizeOf(typeof(WindowsNativeMethods.MonitorInformation)) };
                if (!WindowsNativeMethods.GetMonitorInfo(monitor, ref info)) return;
                if (WindowsNativeMethods.MonitorFromWindow(handle, 2) != monitor)
                    WindowsNativeMethods.SetWindowPos(handle, IntPtr.Zero, info.Work.Left + 4, info.Work.Top + 4, 0, 0,
                        0x0001 | 0x0004 | 0x0010 | 0x0200);
                int dpi = EffectiveDpi();
                double scale = dpi / 96.0;
                int width = (int)Math.Round(maxOuterWidth * scale), height = (int)Math.Round(maxOuterHeight * scale);
                (double targetX, double targetY) = ComputeTargetOrigin(monitor, info, dpi, scale);
                windowMotion.SetDirection(windowPosition, scale, ToastProfile.Compact);
                if (snap || !IsVisible || PreviewPositionProvider != null)
                {
                    windowMotion.SnapPosition(targetX, targetY);
                }
                else
                {
                    windowMotion.SetTargetPosition(targetX, targetY, WindowsNativeMethods.MonotonicMilliseconds);
                    StartFrames();
                }

                int x = (int)Math.Round(windowMotion.RenderX);
                int y = (int)Math.Round(windowMotion.RenderY);
                ApplyWindowBounds(handle, x, y, width, height, dpi);
                dpi = EffectiveDpi();
                ApplyBackground(dpi, VisualTreeHelper.GetDpi(this).DpiScaleX);
                if (acrylic != null)
                {
                    try { if (HasNativeMaterial && IsVisible) acrylic.Show(); else acrylic.Hide(); }
                    catch (Exception error) { MaterialFault(error); ApplyBackground(dpi, VisualTreeHelper.GetDpi(this).DpiScaleX); }
                }
            }
            finally { positioning = false; }
        }

        private (double X, double Y) ComputeTargetOrigin(
            IntPtr monitor,
            WindowsNativeMethods.MonitorInformation info,
            int dpi,
            double scale)
        {
            if (PreviewPositionProvider != null)
            {
                Point? previewPosition = PreviewPositionProvider(new Size(maxOuterWidth, maxOuterHeight));
                if (previewPosition.HasValue)
                    return (previewPosition.Value.X * scale, previewPosition.Value.Y * scale);
            }

            if (ToastProfile.Compact)
            {
                TaskbarPlacement.RequestRefresh();
                // A background result must not move an ongoing copy sequence.
                if (!IsVisible || !taskbarSpotPinned || taskbarMonitor != monitor || taskbarDpi != dpi)
                {
                    taskbarSpot = TaskbarPlacement.Resolve(info.Monitor, info.Work, dpi);
                    taskbarSpotPinned = true; taskbarMonitor = monitor; taskbarDpi = dpi;
                }
                return (taskbarSpot.X - 32 * scale, taskbarSpot.Y - 32 * scale);
            }

            ScreenRect effectiveWork = TaskbarGeometry.GetEffectiveWorkArea(info.Monitor, info.Work, adaptToTaskbar, scale);
            lastEffectiveWorkArea = effectiveWork;
            double currentCardW = baseCardWidth + (expandedCardWidth - baseCardWidth) * counterExpand.Progress;
            ScreenPoint cardTopLeft = ToastPlacementPolicy.ComputeCardTopLeft(
                windowPosition, effectiveWork, currentCardW * scale, cardHeight * scale, offsetX, offsetY, scale);
            return (cardTopLeft.X - 32 * scale, cardTopLeft.Y - 32 * scale);
        }

        private void ApplyWindowBounds(IntPtr handle, int x, int y, int width, int height, int dpi)
        {
            if (acrylic != null)
            {
                try
                {
                    double currentCardW = ToastProfile.Compact ? 225 : baseCardWidth + (expandedCardWidth - baseCardWidth) * counterExpand.Progress;
                    acrylic.Position(x, y, width, height, dpi, resolvedCornerExtent, currentCardW + 64);
                }
                catch (Exception error) { MaterialFault(error); }
            }
            WindowsNativeMethods.WindowRectangle currentBounds;
            if (!WindowsNativeMethods.GetWindowRect(handle, out currentBounds) || currentBounds.Left != x || currentBounds.Top != y ||
                currentBounds.Right - currentBounds.Left != width || currentBounds.Bottom - currentBounds.Top != height)
                WindowsNativeMethods.SetWindowPos(handle, IntPtr.Zero, x, y, width, height, 0x0010 | 0x0200 | 0x0004);
        }

        private void CheckTaskbarChange()
        {
            if (closed || !IsVisible || ToastProfile.Compact || !adaptToTaskbar || PreviewPositionProvider != null || windowHandle == IntPtr.Zero)
                return;
            IntPtr monitor = WindowsNativeMethods.MonitorFromWindow(anchorWindow, 2);
            WindowsNativeMethods.MonitorInformation info = new WindowsNativeMethods.MonitorInformation { Size = (uint)Marshal.SizeOf(typeof(WindowsNativeMethods.MonitorInformation)) };
            if (!WindowsNativeMethods.GetMonitorInfo(monitor, ref info)) return;
            int dpi = EffectiveDpi();
            double scale = dpi / 96.0;
            ScreenRect effectiveWork = TaskbarGeometry.GetEffectiveWorkArea(info.Monitor, info.Work, adaptToTaskbar, scale);
            if (effectiveWork == lastEffectiveWorkArea) return;
            lastEffectiveWorkArea = effectiveWork;
            double currentCardW = baseCardWidth + (expandedCardWidth - baseCardWidth) * counterExpand.Progress;
            ScreenPoint cardTopLeft = ToastPlacementPolicy.ComputeCardTopLeft(
                windowPosition, effectiveWork, currentCardW * scale, cardHeight * scale, offsetX, offsetY, scale);
            windowMotion.SetTargetPosition(cardTopLeft.X - 32 * scale, cardTopLeft.Y - 32 * scale, WindowsNativeMethods.MonotonicMilliseconds);
            StartFrames();
        }

        private void QueueDpiRefresh()
        {
            if (closed || refreshQueued) return;
            refreshQueued = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
            {
                refreshQueued = false;
                if (!closed && windowHandle != IntPtr.Zero) Position(anchorWindow);
            }));
        }

        private void FadeOut()
        {
            hideTimer.Stop();
            windowMotion.BeginOut(Opacity, WindowsNativeMethods.MonotonicMilliseconds); StartFrames();
        }

        private void StartHideTimer()
        {
            if (previewKeepVisible) { hideTimer.Stop(); return; }
            hideTimer.Start();
        }

        private void StartFrames()
        {
            if (!rendering) { CompositionTarget.Rendering += DrawFrame; rendering = true; }
            RenderOptions.SetBitmapScalingMode(background, BitmapScalingMode.HighQuality);
        }
        private void StopFrames()
        {
            if (rendering) { CompositionTarget.Rendering -= DrawFrame; rendering = false; }
            RenderOptions.SetBitmapScalingMode(background, BitmapScalingMode.NearestNeighbor);
        }
        private void DrawFrame(object sender, EventArgs args)
        {
            if (closed || preparing) return;
            if (!IsVisible || Current == null) { StopFrames(); return; }
            bool started = pendingEntrance || pendingHit;
            if (started || preparedRevision != revision)
            {
                // Recheck layout in case a queued DPI/layout notification ran
                // after Present. Cache hits perform no glyph rasterization.
                PrepareVisibleText();
            }
            long now = WindowsNativeMethods.MonotonicMilliseconds;
            if (pendingEntrance)
            {
                pendingEntrance = false;
                windowMotion.BeginIn(Opacity, now); StartHideTimer();
            }
            else if (pendingHit && windowMotion.FadingOut) windowMotion.BeginIn(Opacity, now);
            bool badgeHit = pendingHit;
            bool badgeWasRunning = badgeMotion.IsRunning;
            if (pendingHit)
            {
                pendingHit = false;
                motion.Pulse(now);
                if (Current != null && Current.Kind == FeedbackViewKind.Confirmed)
                {
                    RoundedPolygon target = badgeSequence.Advance(out double rotationDelta);
                    double gap = now - lastBadgeAdvance;
                    if (gap < 140)
                    {
                        // Very fast bursts snap to the next shape. Spring
                        // snapshots would otherwise compound their curve count
                        // without bound; this keeps 100 ms copy bursts safe.
                        badgeMotion.SnapTo(target, badgeMotion.RotationDegrees + rotationDelta);
                    }
                    else
                    {
                        badgeMotion.Trigger(target, rotationDelta, now);
                    }
                    lastBadgeAdvance = now;
                }
            }
            badgeMotion.Advance(now);
            if (badgeHit || badgeWasRunning || badgeMotion.IsRunning)
            {
                int badgeCount = badgeMotion.Sample(badgeCubics);
                badgeShape.SetShape(
                    new ReadOnlySpan<Cubic>(badgeCubics, 0, badgeCount),
                    badgeMotion.RotationDegrees);
            }
            if (TestFrameObserved != null) TestFrameObserved(now, started && preparedRevision == revision);
            double value = motion.Value(now);
            titleTransition.Advance(now); ApplyTitleFrame();
            bool expandWasRunning = counterExpand.Running;
            counterExpand.Advance(now);
            if (expandWasRunning || counterExpand.Running)
            {
                ApplyCardWidthFrame();
                if (!ToastProfile.Compact && PreviewPositionProvider == null && windowHandle != IntPtr.Zero)
                {
                    IntPtr monitor = WindowsNativeMethods.MonitorFromWindow(anchorWindow, 2);
                    WindowsNativeMethods.MonitorInformation info = new WindowsNativeMethods.MonitorInformation { Size = (uint)Marshal.SizeOf(typeof(WindowsNativeMethods.MonitorInformation)) };
                    if (WindowsNativeMethods.GetMonitorInfo(monitor, ref info))
                    {
                        int dpi = EffectiveDpi();
                        double scale = dpi / 96.0;
                        (double tx, double ty) = ComputeTargetOrigin(monitor, info, dpi, scale);
                        windowMotion.SnapPosition(tx, ty);
                    }
                }
            }
            countScale.ScaleX = countScale.ScaleY = 1 + CopyMotion.CounterAmount * value;
            bool windowMotionWasRunning = windowMotion.Running;
            if (windowMotionWasRunning)
            {
                windowMotion.Advance(now);
                Opacity = windowMotion.Opacity;
                double activeScale = ToastProfile.Compact ? 1.0 : windowMotion.Scale;
                windowScale.ScaleX = windowScale.ScaleY = activeScale;
                if (windowHandle != IntPtr.Zero)
                {
                    int dpi = EffectiveDpi();
                    double scale = dpi / 96.0;
                    int width = (int)Math.Round(maxOuterWidth * scale), height = (int)Math.Round(maxOuterHeight * scale);
                    ApplyWindowBounds(windowHandle, (int)Math.Round(windowMotion.RenderX), (int)Math.Round(windowMotion.RenderY), width, height, dpi);
                }
                if (!windowMotion.FadeRunning && windowMotion.FadingOut) { HideAndClear(); return; }
            }
            if (acrylic != null)
            {
                try { acrylic.SetMotion(ToastProfile.Compact ? 1.0 : windowMotion.Scale, Opacity); }
                catch (Exception error) { MaterialFault(error); ApplyBackground(EffectiveDpi(), VisualTreeHelper.GetDpi(this).DpiScaleX); }
            }
            times.Moving = motion.Running;
            if (!motion.Running && !badgeMotion.IsRunning && !windowMotion.Running &&
                !titleTransition.Running && !counterExpand.Running && !pendingHit && !pendingEntrance) StopFrames();
        }

        internal void HideAndClear()
        {
            deferredPulse = pendingEntrance = pendingHit = false;
            revision++;
            hideTimer.Stop();
            taskbarWatchTimer.Stop();
            windowMotion.Reset(0); motion.Stop(); counterExpand.Snap(0); StopFrames();
            windowScale.ScaleX = windowScale.ScaleY = 1.0;
            titleTransition.Reset(""); ApplyTitleFrame();
            badgeShape.StopAmbientSpin();
            badgeSequence.Reset();
            badgeMotion.SnapTo(MaterialShapes.Get(MaterialShapeKind.Circle), InitialBadgeRotation);
            badgeShape.SetShape(badgeMotion.SettledShape, badgeMotion.RotationDegrees);
            lastBadgeAdvance = double.NegativeInfinity;
            countScale.ScaleX = countScale.ScaleY = 1;
            if (acrylic != null) acrylic.Hide();
            Hide(); Opacity = 0;
            Current = null;
            title.Text = detail.Text = times.Text = "";
            textPixels.Idle();
            long retainedPixels = textPixels.RetainedBytes;
            foreach (SampledText text in glyphs) retainedPixels += text.SurfacePixelBytes;
            bool reuseEmptySurfaces = retainedPixels <= 4 * 1024 * 1024;
            foreach (SampledText text in glyphs)
            {
                text.Moving = false;
                if (reuseEmptySurfaces) text.ParkForIdle(textPixels.FinalPixels);
                else text.ClearCache();
            }
            if (!reuseEmptySurfaces) textPixels.Release();
        }

        internal void RenderSample(string path)
        {
            RenderSample(path, new FeedbackView { Kind = FeedbackViewKind.Confirmed, Title = localizer.Get("feedback.title.go_paste"),
                Preview = localizer.Get("toast.sample.preview"), Count = 12 }, true);
        }

        internal void RenderSample(string path, FeedbackView sample, bool preview)
        { RenderSample(path, sample, preview, 96); }

        internal void RenderSample(string path, FeedbackView sample, bool preview, int dpi)
        {
            // Export a detached copy, never change DPI on the live HWND visual tree.
            ToastWindow exporter = new ToastWindow(Themes.MaterialEnabled, localizer);
            try
            {
                double scale = dpi / 96.0;
                VisualTreeHelper.SetRootDpi(exporter.surface, new DpiScale(scale, scale));
                exporter.SetContent(sample, preview);
                exporter.counterExpand.Snap(sample.Count >= 2 ? 1 : 0);
                exporter.UpdateTitle(sample.Title, false);
                exporter.ApplyBackground(dpi, scale);
                exporter.surface.Measure(new Size(exporter.maxOuterWidth, exporter.maxOuterHeight));
                exporter.surface.Arrange(new Rect(0, 0, exporter.maxOuterWidth, exporter.maxOuterHeight));
                exporter.surface.UpdateLayout();
                BackgroundFrame frame = exporter.CurrentBackground;
                RenderTargetBitmap bitmap = new RenderTargetBitmap(frame.Width, frame.Height, dpi, dpi, PixelFormats.Pbgra32);
                bitmap.Render(exporter.surface);
                PngBitmapEncoder encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (FileStream stream = File.Create(path)) encoder.Save(stream);
            }
            finally { exporter.Close(); }
        }

        internal bool TestRendering { get { return rendering; } }
        internal bool TestMaterialVisible { get { return acrylic != null && acrylic.Visible; } }
        internal bool TestTitleAwaitingResult { get { return titleTransition.AwaitingResult; } }
        internal long TestCopySignal { get { return lastCopySignal; } }
        internal double TestCopyQueueMilliseconds { get { return lastCopyQueueMilliseconds; } }
        internal sealed class LayoutSnapshot
        {
            internal Rect Title, Detail, Counter, Group, Number, Badge;
            internal string NumberText;
            internal double NumberFont;
            internal string CounterFont;
            internal double TitleFont, DetailFont;
            internal TextRenderingMode RenderingMode;
            internal TextHintingMode HintingMode;
        }
        internal bool VerifyFontBinding()
        {
            return title.FontFamily == toastFontFamily && detail.FontFamily == toastFontFamily &&
                symbol.FontFamily == toastFontFamily && title.FontWeight.ToOpenTypeWeight() == 650 &&
                symbol.FontWeight.ToOpenTypeWeight() == 650 && detail.FontWeight.ToOpenTypeWeight() == 500 &&
                title.FontSize == 14 && detail.FontSize == 12 && times.FontSize == 12;
        }
        internal LayoutSnapshot MeasureLayout(int count, int dpi)
        {
            double scale = dpi / 96.0;
            VisualTreeHelper.SetRootDpi(surface, new DpiScale(scale, scale));
            SetContent(new FeedbackView { Kind = FeedbackViewKind.Confirmed, Title = FeedbackMapper.ResolveTitle(FeedbackEngine.SuccessTitleKey(count), localizer),
                Preview = "Fixed preview text", Count = count, CountText = new CappedCount(count).Format() }, true);
            counterExpand.Snap(count >= 2 ? 1 : 0);
            ApplyCardWidthFrame();
            UpdateTitle(Current.Title, false);
            surface.Measure(new Size(maxOuterWidth, maxOuterHeight));
            surface.Arrange(new Rect(0, 0, maxOuterWidth, maxOuterHeight)); surface.UpdateLayout();
            return new LayoutSnapshot { Badge = MeasureBounds(badge), Title = MeasureBounds(title), Detail = MeasureBounds(detail),
                Counter = MeasureBounds(counter), Group = MeasureBounds(counterContent), Number = MeasureBounds(times),
                NumberText = times.Text,
                NumberFont = times.FontSize,
                CounterFont = times.FontFamily.Source,
                TitleFont = title.FontSize, DetailFont = detail.FontSize,
                RenderingMode = TextOptions.GetTextRenderingMode(title), HintingMode = TextOptions.GetTextHintingMode(title) };
        }
        internal bool VerifyCheckMark(FeedbackViewKind kind)
        {
            SetContent(new FeedbackView { Kind = kind, Title = localizer.Get("feedback.title.copied"), Preview = "test", Count = 6 }, true);
            bool success = kind != FeedbackViewKind.Unconfirmed && kind != FeedbackViewKind.Updated && kind != FeedbackViewKind.Retained;
            return checkMark.Data.IsFrozen && checkMark.Width == 18 && checkMark.Height == 18 &&
                checkMark.StrokeThickness == 2 && checkMark.StrokeStartLineCap == PenLineCap.Round &&
                checkMark.StrokeEndLineCap == PenLineCap.Round && checkMark.StrokeLineJoin == PenLineJoin.Round &&
                checkMark.Visibility == (success ? Visibility.Visible : Visibility.Hidden) &&
                symbol.Visibility == (success ? Visibility.Hidden : Visibility.Visible) &&
                ((SolidColorBrush)checkMark.Stroke).Color == (Color)ColorConverter.ConvertFromString(Themes.Current.Green);
        }
        internal bool VerifyTitleReset()
        {
            UpdateTitle("old", false); UpdateTitle("new", true);
            bool started = rendering && titleTransition.Running;
            HideAndClear();
            return started && !titleTransition.Running && title.Text == "" && title.Opacity == 1 && !rendering;
        }
        private static MaterialShapeSequence CreateBadgeSequence(bool compact)
        {
            if (compact)
            {
                return new MaterialShapeSequence(new[] { MaterialShapeKind.Circle, MaterialShapeKind.SoftBurst }, 45);
            }

            List<MaterialShapeKind> shapes = new List<MaterialShapeKind>(MaterialShapeSequence.Default.Count);
            foreach (MaterialShapeKind shape in MaterialShapeSequence.Default)
            {
                if (shape != MaterialShapeKind.Heart)
                {
                    shapes.Add(shape);
                }
            }
            return new MaterialShapeSequence(shapes, 45);
        }

        private Rect MeasureBounds(FrameworkElement element)
        { return element.TransformToAncestor(surface).TransformBounds(new Rect(new Point(), element.RenderSize)); }

    }
}

