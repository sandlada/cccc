using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal static class CardBackgroundRenderer
    {
        internal static int LogicalWidth { get { return ToastProfile.Compact ? 289 : 424; } }
        internal static int LogicalHeight { get { return ToastProfile.Compact ? 96 : 144; } }
        private const int Sampling = 2;

        internal static BackgroundFrame Build(int dpi)
        {
            return Build(dpi, LogicalWidth, LogicalHeight, CardOutline.CornerExtent, true, true);
        }

        internal static BackgroundFrame Build(int dpi, int logicalWidth, int logicalHeight,
            double cornerExtent, bool showBorder, bool showShadow)
        {
            if (dpi <= 0 || dpi > 960) throw new ArgumentOutOfRangeException("dpi");
            Stopwatch watch = Stopwatch.StartNew();
            double scale = dpi / 96.0;
            int width = (int)Math.Round(logicalWidth * scale), height = (int)Math.Round(logicalHeight * scale);
            int highWidth = width * Sampling, highHeight = height * Sampling;
            int margin = (int)Math.Round(32 * scale);
            Rect bounds = new Rect(margin + .5, margin + .5, width - 2 * margin - 1, height - 2 * margin - 1);
            double effectiveCorner = ToastProfile.Compact && Math.Abs(cornerExtent - CardOutline.CornerExtent) < 0.001
                ? bounds.Height / 2 / scale
                : Math.Min(Math.Max(0, cornerExtent), Math.Min(bounds.Width, bounds.Height) / 2 / scale);
            Geometry outline = CardOutline.Create(bounds, scale, effectiveCorner);
            bool taskbarBlend = ToastProfile.Compact && Themes.MaterialEnabled && Themes.Current.Transparent;
            double taskbarShadow = taskbarBlend ? Themes.Current.TaskbarShadow : 0;

            // The drawing coordinates here are FINAL physical pixels. Only this
            // detached visual is rendered at 2x; text is never in this bitmap.
            DrawingVisual visual = new DrawingVisual();
            VisualTreeHelper.SetRootDpi(visual, new DpiScale(Sampling, Sampling));
            using (DrawingContext draw = visual.RenderOpen())
            {
                Color color = Themes.Current.Fill;
                color.A = Themes.UseAcrylic ? (byte)Math.Round(255 * Themes.Current.Tint) : (byte)255;
                if (taskbarBlend)
                {
                    // User-selected tint over the taskbar's existing material;
                    // never create another backdrop sampler or add grain.
                    color = Themes.Current.TaskbarFill;
                    color.A = (byte)Math.Round(255 * Themes.Current.TaskbarTint);
                }
                SolidColorBrush fill = new SolidColorBrush(color); fill.Freeze();
                Pen pen = null;
                if (showBorder && !ToastProfile.Compact)
                {
                    SolidColorBrush line = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Themes.Current.Rim)); line.Freeze();
                    pen = new Pen(line, 1); pen.Freeze();
                }
                draw.DrawGeometry(fill, pen, outline);
            }
            RenderTargetBitmap high = new RenderTargetBitmap(highWidth, highHeight, 192, 192, PixelFormats.Pbgra32);
            high.Render(visual);
            byte[] body = new byte[highWidth * highHeight * 4];
            high.CopyPixels(body, highWidth * 4, 0);

            // A shadow-only alpha layer derived from exactly the same contour.
            // For the ordinary card, three separable box passes approximate
            // sigma=3 logical pixels with the existing 15% shadow opacity.
            // The compact light surface uses the separately selected weak shadow.
            // Running sums make each blur pass O(pixel count), independent of DPI.
            bool drawShadow = showShadow && (!ToastProfile.Compact || taskbarShadow > 0);
            byte[] alpha = drawShadow ? new byte[highWidth * highHeight] : null;
            byte[] temporary = drawShadow ? new byte[alpha.Length] : null;
            double coverageTint = taskbarBlend ? Themes.Current.TaskbarTint : (Themes.UseAcrylic ? Themes.Current.Tint : 1);
            if (drawShadow)
                for (int i = 0; i < alpha.Length; i++) alpha[i] = (byte)Math.Min(255, Math.Round(body[i * 4 + 3] / coverageTint));
            int radius = Math.Max(1, (int)Math.Round(ToastProfile.Compact ? 3.5 * scale * Sampling : 3 * scale * Sampling - .5));
            if (drawShadow)
                for (int pass = 0; pass < 3; pass++) BoxBlur(alpha, temporary, highWidth, highHeight, radius);
            int offset = (int)Math.Round(3 / Math.Sqrt(2) * scale * Sampling);
            // Match the tuning preview's weak vertical shadow: 0px 2px 7px.
            // The existing separable blur is a software approximation of that
            // browser blur, cached once per appearance/DPI, never per frame.
            double shadowStrength = !drawShadow ? 0 : (ToastProfile.Compact ? taskbarShadow : .15);
            byte[] pixels = ToastProfile.Compact ? CompositeAndDownsample(body, alpha, highWidth, highHeight,
                0, (int)Math.Round(2 * scale * Sampling), shadowStrength, coverageTint) :
                CompositeAndDownsample(body, alpha, highWidth, highHeight, offset, offset, shadowStrength,
                    Themes.UseAcrylic ? Themes.Current.Tint : 1);
            // 1.2% extra grain, cached with the background. Preserve premultiplied
            // alpha and avoid affecting the transparent shadow outside the card.
            for (int y = margin + 2; !ToastProfile.Compact && Themes.UseAcrylic && y < height - margin - 2; y++)
                for (int x = margin + 2; x < width - margin - 2; x++)
                {
                    int p = (y * width + x) * 4; if (pixels[p + 3] < 150) continue;
                    uint hash = unchecked((uint)(x * 374761393 + y * 668265263)); hash = (hash ^ (hash >> 13)) * 1274126177;
                    double grain = (((hash >> 24) / 255.0) - .5) * 255 * .012 * pixels[p + 3] / 255.0;
                    for (int channel = 0; channel < 3; channel++) pixels[p + channel] = (byte)Math.Max(0, Math.Min(pixels[p + 3], Math.Round(pixels[p + channel] + grain)));
                }
            BitmapSource bitmap = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Pbgra32, null, pixels, width * 4);
            bitmap.Freeze();

            BitmapSource leftSlice = null, middleSlice = null, rightSlice = null;
            int leftSliceWidth = 0, rightSliceWidth = 0;
            if (!ToastProfile.Compact)
            {
                int cornerPx = (int)Math.Ceiling(effectiveCorner * scale);
                int capPx = Math.Min(width / 2 - 1, margin + cornerPx + 2);
                if (capPx > 0 && capPx * 2 < width)
                {
                    leftSlice = new CroppedBitmap(bitmap, new Int32Rect(0, 0, capPx, height));
                    leftSlice.Freeze();
                    middleSlice = new CroppedBitmap(bitmap, new Int32Rect(capPx, 0, 1, height));
                    middleSlice.Freeze();
                    rightSlice = new CroppedBitmap(bitmap, new Int32Rect(width - capPx, 0, capPx, height));
                    rightSlice.Freeze();
                    leftSliceWidth = capPx;
                    rightSliceWidth = capPx;
                }
            }

            watch.Stop();
            return new BackgroundFrame { Bitmap = bitmap, LeftSlice = leftSlice, MiddleSlice = middleSlice, RightSlice = rightSlice,
                LeftSliceWidth = leftSliceWidth, RightSliceWidth = rightSliceWidth, Outline = outline, Dpi = dpi, Width = width,
                Height = height, LogicalWidth = logicalWidth, LogicalHeight = logicalHeight, CornerExtent = effectiveCorner,
                ShowBorder = showBorder, ShowShadow = showShadow, BuildMilliseconds = watch.Elapsed.TotalMilliseconds,
                HighBufferBytes = body.Length, Dark = Themes.Current.Dark, Acrylic = Themes.UseAcrylic,
                Compact = ToastProfile.Compact, TaskbarBlend = ToastProfile.Compact && Themes.MaterialEnabled && Themes.Current.Transparent };
        }

        private static void BoxBlur(byte[] values, byte[] temporary, int width, int height, int radius)
        {
            int diameter = 2 * radius + 1;
            for (int y = 0; y < height; y++)
            {
                int start = y * width, sum = 0;
                for (int x = 0; x <= radius && x < width; x++) sum += values[start + x];
                for (int x = 0; x < width; x++)
                {
                    temporary[start + x] = (byte)((sum + diameter / 2) / diameter);
                    int remove = x - radius, add = x + radius + 1;
                    if (remove >= 0) sum -= values[start + remove];
                    if (add < width) sum += values[start + add];
                }
            }
            for (int x = 0; x < width; x++)
            {
                int sum = 0;
                for (int y = 0; y <= radius && y < height; y++) sum += temporary[y * width + x];
                for (int y = 0; y < height; y++)
                {
                    values[y * width + x] = (byte)((sum + diameter / 2) / diameter);
                    int remove = y - radius, add = y + radius + 1;
                    if (remove >= 0) sum -= temporary[remove * width + x];
                    if (add < height) sum += temporary[add * width + x];
                }
            }
        }

        internal static byte[] CompositeAndDownsample(byte[] body, byte[] shadow, int width, int height, int offset)
        {
            return CompositeAndDownsample(body, shadow, width, height, offset, offset, .15,
                Themes.UseAcrylic ? Themes.Current.Tint : 1);
        }

        private static byte[] CompositeAndDownsample(byte[] body, byte[] shadow, int width, int height,
            int offsetX, int offsetY, double shadowStrength, double coverageTint)
        {
            int outputWidth = width / 2, outputHeight = height / 2;
            byte[] output = new byte[outputWidth * outputHeight * 4];
            for (int y = 0; y < outputHeight; y++)
                for (int x = 0; x < outputWidth; x++)
                {
                    int blue = 0, green = 0, red = 0, alpha = 0;
                    for (int sy = 0; sy < 2; sy++)
                        for (int sx = 0; sx < 2; sx++)
                        {
                            int highX = x * 2 + sx, highY = y * 2 + sy;
                            int index = (highY * width + highX) * 4;
                            int shadowX = highX - offsetX, shadowY = highY - offsetY;
                            int shadowAlpha = shadowStrength > 0 && shadowX >= 0 && shadowY >= 0 && shadowX < width && shadowY < height ?
                                (int)(shadow[shadowY * width + shadowX] * shadowStrength + .5) : 0;
                            int coverage = Math.Min(255, (int)Math.Round(body[index + 3] / coverageTint));
                            int inverse = 255 - coverage;
                            int visibleShadow = (shadowAlpha * inverse + 127) / 255;
                            blue += body[index];
                            green += body[index + 1];
                            red += body[index + 2];
                            alpha += body[index + 3] + visibleShadow;
                        }
                    int target = (y * outputWidth + x) * 4;
                    output[target] = (byte)((blue + 2) / 4);
                    output[target + 1] = (byte)((green + 2) / 4);
                    output[target + 2] = (byte)((red + 2) / 4);
                    output[target + 3] = (byte)((alpha + 2) / 4);
                }
            return output;
        }
    }
}
