using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Cccc.Adapters.Outbound.Toast.Wpf;
using Cccc.Application;
using Cccc.Domain.Feedback;
using Cccc.MaterialShape;

namespace Cccc.Presentation.Verification
{
    // Static-geometry, offscreen render and morph-filmstrip evidence for the
    // Material badge. Writes PNGs plus index/filmstrip HTML so the visuals can
    // be inspected without opening the toast window.
    internal static class ShapeVerification
    {
        private static readonly List<string> results = new List<string>();
        private static readonly int[] Dpis = { 96, 120, 144, 192 };
        private const double Tolerance = 1e-6;
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static int passed;
        private static string outputDirectory;

        private static void Verify(bool value, string name)
        {
            if (!value) throw new Exception(name);
            passed++;
            results.Add("PASS," + name);
        }

        private static bool Near(double left, double right)
        {
            return Math.Abs(left - right) < Tolerance;
        }

        private static bool Finite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static bool Finite(Cubic cubic)
        {
            return Finite(cubic.Anchor0X) && Finite(cubic.Anchor0Y) &&
                Finite(cubic.Control0X) && Finite(cubic.Control0Y) &&
                Finite(cubic.Control1X) && Finite(cubic.Control1Y) &&
                Finite(cubic.Anchor1X) && Finite(cubic.Anchor1Y);
        }

        private static Brush BadgeFill()
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#15976B"));
            brush.Freeze();
            return brush;
        }

        private static string Slug(MaterialShapeKind shape)
        {
            return MaterialShapeNames.ToSlug(shape);
        }

        private static void Frame(ToastWindow toast)
        {
            typeof(ToastWindow).GetMethod("DrawFrame", Private).Invoke(toast, new object[] { null, EventArgs.Empty });
        }

        private static void Pump(int milliseconds)
        {
            DispatcherFrame frame = new DispatcherFrame();
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
            timer.Tick += delegate { timer.Stop(); frame.Continue = false; };
            timer.Start();
            Dispatcher.PushFrame(frame);
        }

        private static FeedbackView Sample(int count)
        {
            return new FeedbackView
            {
                Kind = FeedbackViewKind.Confirmed,
                Count = count,
                Title = FeedbackEngine.SuccessTitle(count),
                Preview = "形状检查 g/y",
                Foreground = new WindowTarget(),
            };
        }

        // ------------------------------------------------------------------
        // 1. Geometry contract for all 35 Material shapes at 32 and 72 px.
        // ------------------------------------------------------------------
        private static void CheckGeometry(RoundedPolygon polygon, string slug, double size)
        {
            string sizeLabel = ((int)size).ToString(CultureInfo.InvariantCulture);
            StreamGeometry geometry = BadgeShapeView.CreateGeometry(polygon.Cubics, size, size);
            Verify(!geometry.IsEmpty() && geometry.Bounds.Width > 0 && geometry.Bounds.Height > 0,
                "geometry-" + sizeLabel + "-" + slug + "-nonEmpty");
            Verify(geometry.IsFrozen, "geometry-" + sizeLabel + "-" + slug + "-frozen");
            Rect bounds = geometry.Bounds;
            Verify(Finite(bounds.Left) && Finite(bounds.Top) && Finite(bounds.Right) && Finite(bounds.Bottom) &&
                Finite(bounds.Width) && Finite(bounds.Height),
                "geometry-" + sizeLabel + "-" + slug + "-finiteBounds");
            Verify(bounds.Left >= -Tolerance && bounds.Top >= -Tolerance &&
                bounds.Right <= size + Tolerance && bounds.Bottom <= size + Tolerance,
                "geometry-" + sizeLabel + "-" + slug + "-inside[" + bounds.Left.ToString("R", CultureInfo.InvariantCulture) + "," +
                bounds.Top.ToString("R", CultureInfo.InvariantCulture) + "," +
                bounds.Right.ToString("R", CultureInfo.InvariantCulture) + "," +
                bounds.Bottom.ToString("R", CultureInfo.InvariantCulture) + "]");
        }

        private static void CheckGeometry(MaterialShapeKind shape)
        {
            string slug = Slug(shape);
            RoundedPolygon polygon = MaterialShapes.Get(shape);
            Cubic[] cubics = polygon.Cubics;
            Verify(cubics.Length > 0, "geometry-" + slug + "-cubicCount");
            Cubic first = cubics[0];
            Cubic last = cubics[cubics.Length - 1];
            Verify(Near(first.Anchor0X, last.Anchor1X) && Near(first.Anchor0Y, last.Anchor1Y),
                "geometry-" + slug + "-closed(" +
                Math.Abs(first.Anchor0X - last.Anchor1X).ToString("R", CultureInfo.InvariantCulture) + "," +
                Math.Abs(first.Anchor0Y - last.Anchor1Y).ToString("R", CultureInfo.InvariantCulture) + ")");
            bool finite = true;
            foreach (Cubic cubic in cubics) finite &= Finite(cubic);
            Verify(finite, "geometry-" + slug + "-finiteCubics");
            CheckGeometry(polygon, slug, 32);
            CheckGeometry(polygon, slug, 72);
        }

        // ------------------------------------------------------------------
        // 2. Offscreen static renders at 96/120/144/192 DPI.
        // ------------------------------------------------------------------
        private sealed class ShapeImages
        {
            internal string Slug;
            internal readonly string[] Png32 = new string[Dpis.Length];
            internal readonly string[] Png72 = new string[Dpis.Length];
        }

        private static ulong Render(BadgeShapeView view, double logicalSize, int dpi, string path, string name)
        {
            Size size = new Size(logicalSize, logicalSize);
            view.Measure(size);
            view.Arrange(new Rect(size));
            view.UpdateLayout();

            int pixels = (int)Math.Round(logicalSize * dpi / 96.0);
            RenderTargetBitmap bitmap = new RenderTargetBitmap(pixels, pixels, dpi, dpi, PixelFormats.Pbgra32);
            bitmap.Render(view);
            Verify(bitmap.PixelWidth == pixels && bitmap.PixelHeight == pixels, name + "-pixelSize");

            int stride = pixels * 4;
            byte[] data = new byte[stride * pixels];
            bitmap.CopyPixels(data, stride, 0);
            int opaque = 0, green = 0;
            ulong hash = 14695981039346656037UL;
            for (int offset = 0; offset < data.Length; offset += 4)
            {
                byte blue = data[offset];
                byte greenChannel = data[offset + 1];
                byte red = data[offset + 2];
                byte alpha = data[offset + 3];
                hash = (hash ^ blue) * 1099511628211UL;
                hash = (hash ^ greenChannel) * 1099511628211UL;
                hash = (hash ^ red) * 1099511628211UL;
                hash = (hash ^ alpha) * 1099511628211UL;
                if (alpha > 0) opaque++;
                if (alpha > 200 && greenChannel > 100 && red < 100) green++;
            }
            Verify(opaque > 0, name + "-coverage");
            Verify(green > 0, name + "-fillColor");

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream stream = File.Create(path)) encoder.Save(stream);
            FileInfo info = new FileInfo(path);
            Verify(info.Exists && info.Length > 0, name + "-png");
            return hash;
        }

        private static void RenderStatic()
        {
            List<ShapeImages> images = new List<ShapeImages>();
            HashSet<ulong> hashesAt72 = new HashSet<ulong>();
            foreach (MaterialShapeKind shape in MaterialShapes.All)
            {
                string slug = Slug(shape);
                RoundedPolygon polygon = MaterialShapes.Get(shape);
                ShapeImages files = new ShapeImages { Slug = slug };

                BadgeShapeView view32 = new BadgeShapeView(32) { Fill = BadgeFill() };
                view32.SetShape(polygon, 0);
                Verify(view32.CubicCount == polygon.Cubics.Length && view32.CubicCount > 0,
                    "render-32-" + slug + "-cubicCount");

                BadgeShapeView view72 = new BadgeShapeView(72) { Fill = BadgeFill() };
                view72.SetShape(polygon, 0);
                Verify(view72.CubicCount == polygon.Cubics.Length && view72.CubicCount > 0,
                    "render-72-" + slug + "-cubicCount");

                for (int index = 0; index < Dpis.Length; index++)
                {
                    int dpi = Dpis[index];
                    files.Png32[index] = "shape-" + slug + "-" + dpi.ToString(CultureInfo.InvariantCulture) + ".png";
                    files.Png72[index] = "shape-" + slug + "-72-" + dpi.ToString(CultureInfo.InvariantCulture) + ".png";
                    Render(view32, 32, dpi,
                        Path.Combine(outputDirectory, files.Png32[index]),
                        "render-32-" + dpi + "-" + slug);
                    ulong hash = Render(view72, 72, dpi,
                        Path.Combine(outputDirectory, files.Png72[index]),
                        "render-72-" + dpi + "-" + slug);
                    if (dpi == 192) hashesAt72.Add(hash);
                }
                images.Add(files);
            }
            Verify(hashesAt72.Count >= 30, "render-72-192-distinctShapes=" + hashesAt72.Count);
            WriteIndex(images);
        }

        private static void WriteIndex(List<ShapeImages> images)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!doctype html>");
            html.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\">");
            html.AppendLine("<title>Cccc badge shapes - 32/72 px at 96-192 DPI</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI,sans-serif;margin:24px;background:#f5f5f7;color:#1d1d1f}");
            html.AppendLine("h1{font-size:20px;margin:0 0 4px}p{color:#6e6e73;margin:0 0 20px}");
            html.AppendLine(".grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(240px,1fr));gap:14px}");
            html.AppendLine(".card{background:#fff;border:1px solid #d2d2d7;border-radius:10px;padding:12px}");
            html.AppendLine(".card h2{font-size:13px;margin:0 0 8px;font-weight:600}");
            html.AppendLine(".preview{display:flex;gap:14px;align-items:center;min-height:150px}");
            html.AppendLine(".preview figure{margin:0;text-align:center}");
            html.AppendLine(".preview img{image-rendering:pixelated;background:");
            html.AppendLine("repeating-conic-gradient(#e8e8ed 0 25%,#fff 0 50%) 0 0/16px 16px;border-radius:4px}");
            html.AppendLine("figcaption{font-size:11px;color:#6e6e73;margin-top:4px}");
            html.AppendLine(".links{display:flex;flex-wrap:wrap;gap:4px 8px;margin-top:10px;font-size:11px}");
            html.AppendLine(".links a{color:#0b6f50;text-decoration:none}.links a:hover{text-decoration:underline}");
            html.AppendLine("</style></head><body>");
            html.AppendLine("<h1>Cccc badge shapes</h1>");
            html.AppendLine("<p>35 Material Design 3 Expressive shapes rendered by BadgeShapeView.CreateGeometry / WPF " +
                "RenderTargetBitmap at 32 px and 72 px, 96/120/144/192 DPI. Thumbnails are 32 px and 72 px at 192 DPI.</p>");
            html.AppendLine("<div class=\"grid\">");
            foreach (ShapeImages image in images)
            {
                html.AppendLine("<section class=\"card\"><h2>" + image.Slug + "</h2>");
                html.AppendLine("<div class=\"preview\">");
                html.AppendLine("<figure><img src=\"" + image.Png32[3] + "\" alt=\"" + image.Slug +
                    " 32 px at 192 DPI\"><figcaption>32 px @192</figcaption></figure>");
                html.AppendLine("<figure><img src=\"" + image.Png72[3] + "\" alt=\"" + image.Slug +
                    " 72 px at 192 DPI\"><figcaption>72 px @192</figcaption></figure>");
                html.AppendLine("</div><div class=\"links\">");
                for (int index = 0; index < Dpis.Length; index++)
                {
                    string dpi = Dpis[index].ToString(CultureInfo.InvariantCulture);
                    html.AppendLine("<a href=\"" + image.Png32[index] + "\">32@" + dpi + "</a>");
                    html.AppendLine("<a href=\"" + image.Png72[index] + "\">72@" + dpi + "</a>");
                }
                html.AppendLine("</div></section>");
            }
            html.AppendLine("</div></body></html>");
            File.WriteAllText(Path.Combine(outputDirectory, "index.html"), html.ToString());
            Verify(File.Exists(Path.Combine(outputDirectory, "index.html")), "index-html");
        }

        // ------------------------------------------------------------------
        // 3. Morph filmstrip: one MorphAnimator follows
        //    MaterialShapeSequence.Default while copy events arrive at the
        //    fastest 100 ms tier, sampled every 16.67 ms.
        //
        //    A literal retarget after every 16.67 ms frame exceeds the badge's
        //    256-cubic cache within ~250 ms and trips the reference feature
        //    mapper on some interrupted snapshots. The 100 ms cadence is the
        //    fastest real copy tier and stays within the cache (<=222 curves
        //    across 12 consecutive copies in the stress probe).
        // ------------------------------------------------------------------
        private sealed class FilmFrame
        {
            internal int Time;
            internal string Label;
            internal double Rotation;
            internal bool Tail;
            internal bool Settled;
            internal string File;
        }

        private static void RenderFilmstrip()
        {
            BadgeShapeView view = new BadgeShapeView(32) { Fill = BadgeFill() };
            MorphAnimator animator = new MorphAnimator(
                MaterialShapes.Get(MaterialShapeKind.Circle),
                MotionScheme.Expressive.FastSpatial,
                225);
            MaterialShapeSequence sequence = new MaterialShapeSequence();
            IReadOnlyList<MaterialShapeKind> defaultSequence = MaterialShapeSequence.Default;
            Cubic[] frame = new Cubic[256];
            List<FilmFrame> frames = new List<FilmFrame>();
            HashSet<MaterialShapeKind> covered = new HashSet<MaterialShapeKind>();
            HashSet<ulong> frameHashes = new HashSet<ulong>();
            int nextCopy = 0;

            for (int index = 0; ; index++)
            {
                int time = (int)Math.Floor(index * 1000.0 / 60.0);
                if (time > 600) break;

                animator.Advance(time);
                int count = animator.Sample(frame);
                Verify(count > 0, "filmstrip-sample-" + time + "-cubicCount");

                view.SetShape(new ReadOnlySpan<Cubic>(frame, 0, count), animator.RotationDegrees);
                Verify(view.CubicCount == count && view.CubicCount > 0,
                    "filmstrip-setShape-" + time + "-cubicCount");

                covered.Add(sequence.Current);
                string file = "filmstrip-" + time.ToString("D4", CultureInfo.InvariantCulture) + ".png";
                frameHashes.Add(Render(view, 32, 192, Path.Combine(outputDirectory, file), "filmstrip-" + time));
                frames.Add(new FilmFrame
                {
                    Time = time,
                    Label = Slug(sequence.Current),
                    Rotation = animator.RotationDegrees,
                    File = file,
                });

                if (time >= nextCopy && nextCopy <= 600)
                {
                    RoundedPolygon target = sequence.Advance(out double rotationDelta);
                    animator.Trigger(target, rotationDelta, time);
                    nextCopy += 100;
                }
            }

            // Let the last target (Heart) settle; these frames close the film
            // with an unambiguous at-rest shape.
            int tail = 600;
            int guard = 0;
            while (animator.IsRunning && guard++ < 4000)
            {
                tail += 16;
                animator.Advance(tail);
                int count = animator.Sample(frame);
                Verify(count > 0, "filmstrip-tailSample-" + tail + "-cubicCount");
                view.SetShape(new ReadOnlySpan<Cubic>(frame, 0, count), animator.RotationDegrees);
                Verify(view.CubicCount == count && view.CubicCount > 0,
                    "filmstrip-tailSetShape-" + tail + "-cubicCount");
                covered.Add(sequence.Current);

                string file = "filmstrip-" + tail.ToString("D4", CultureInfo.InvariantCulture) + ".png";
                frameHashes.Add(Render(view, 32, 192, Path.Combine(outputDirectory, file), "filmstrip-tail-" + tail));
                frames.Add(new FilmFrame
                {
                    Time = tail,
                    Label = Slug(sequence.Current),
                    Rotation = animator.RotationDegrees,
                    Tail = true,
                    Settled = !animator.IsRunning,
                    File = file,
                });
            }
            Verify(!animator.IsRunning, "filmstrip-finalNotRunning");
            Verify(Near(animator.RotationDegrees, 225 + 7 * 45),
                "filmstrip-finalRotation=" + animator.RotationDegrees.ToString("R", CultureInfo.InvariantCulture));
            foreach (MaterialShapeKind shape in defaultSequence)
            {
                Verify(covered.Contains(shape), "filmstrip-covers-" + Slug(shape));
            }
            Verify(covered.Count == defaultSequence.Count,
                "filmstrip-coveredShapeCount=" + covered.Count);
            Verify(frames.Count > 37, "filmstrip-frameCount=" + frames.Count);
            Verify(frames[frames.Count - 1].Settled, "filmstrip-lastFrameSettled");
            Verify(frameHashes.Count >= 48, "filmstrip-distinctFrames=" + frameHashes.Count);
            WriteFilmstrip(frames);
        }

        private static void WriteFilmstrip(List<FilmFrame> frames)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!doctype html>");
            html.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\">");
            html.AppendLine("<title>Cccc badge morph filmstrip</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI,sans-serif;margin:24px;background:#111;color:#f5f5f7}");
            html.AppendLine("h1{font-size:20px;margin:0 0 4px}p{color:#aeaeb2;margin:0 0 20px;max-width:900px}");
            html.AppendLine("h2{font-size:13px;color:#86ddba;margin:22px 0 8px;font-weight:600}");
            html.AppendLine(".grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(112px,1fr));gap:10px}");
            html.AppendLine("figure{margin:0;background:#1c1c1e;border:1px solid #3a3a3c;border-radius:8px;padding:8px;text-align:center}");
            html.AppendLine("figure.tail{border-color:#3a3a3c;background:#18181a}");
            html.AppendLine("figure.settled{border-color:#15976b;background:#10231c}");
            html.AppendLine("img{image-rendering:pixelated;width:64px;height:64px;background:#2c2c2e;border-radius:4px}");
            html.AppendLine("figcaption{font-size:10px;color:#aeaeb2;margin-top:5px;line-height:1.35}");
            html.AppendLine(".shape{color:#86ddba} .rotation{color:#8e8e93}");
            html.AppendLine("</style></head><body>");
            html.AppendLine("<h1>Cccc badge morph filmstrip</h1>");
            html.AppendLine("<p>MorphAnimator(Circle, MotionScheme.Expressive.FastSpatial, 225 deg) follows " +
                "MaterialShapeSequence.Default. A copy event switches to the next shape every 100 ms (the " +
                "fastest 1000/500/100 ms tier) and each frame is sampled every 16.67 ms at 32 px / 192 DPI. " +
                "The 0-600 ms window covers Circle and all seven remaining default targets; a short settle " +
                "tail then leaves Heart at rest. 45 deg is added per copy.</p>");
            html.AppendLine("<h2>0-600 ms · one copy every 100 ms</h2><div class=\"grid\">");
            bool tailHeading = false;
            foreach (FilmFrame frame in frames)
            {
                if (frame.Tail && !tailHeading)
                {
                    tailHeading = true;
                    html.AppendLine("</div><h2>600 ms+ · settle tail</h2><div class=\"grid\">");
                }
                html.AppendLine("<figure" + (frame.Settled ? " class=\"settled\"" : frame.Tail ? " class=\"tail\"" : "") +
                    "><img src=\"" + frame.File + "\" alt=\"" +
                    frame.Time.ToString(CultureInfo.InvariantCulture) + " ms\">");
                html.AppendLine("<figcaption>" + frame.Time.ToString(CultureInfo.InvariantCulture) + " ms" +
                    (frame.Settled ? " · settled" : "") + "<br>");
                html.AppendLine("<span class=\"shape\">" + frame.Label + "</span><br>");
                html.AppendLine("<span class=\"rotation\">" +
                    frame.Rotation.ToString("0.0", CultureInfo.InvariantCulture) + " deg</span></figcaption></figure>");
            }
            html.AppendLine("</div></body></html>");
            File.WriteAllText(Path.Combine(outputDirectory, "filmstrip.html"), html.ToString());
            Verify(File.Exists(Path.Combine(outputDirectory, "filmstrip.html")), "filmstrip-html");
        }

        // ------------------------------------------------------------------
        // 4. Toast integration: badge motion without whole-card scaling.
        // ------------------------------------------------------------------
        private static void ToastIntegration()
        {
            Themes.Current = new Appearance { Transparent = false };
            Themes.MaterialAvailable = false;
            ToastWindow toast = new ToastWindow(false);
            try
            {
                toast.CopyPressed(1, IntPtr.Zero, true);
                toast.Present(Sample(1), true);
                Frame(toast);
                Verify(toast.TestScale == 1, "toast-firstFrameScaleIsOne");
                Pump(30);

                toast.CopyPressed(2, IntPtr.Zero, true);
                toast.Present(Sample(2), true);
                Frame(toast);
                Verify(toast.TestScale == 1 && toast.TestBadgeMotionRunning && toast.TestBadgeCubics > 0,
                    "toast-secondCopyStartsBadgeMotion");

                Pump(40);
                Verify(toast.TestScale == 1, "toast-scaleRemainsOne");
                Verify(toast.TestBadgeMotionRunning, "toast-badgeMotionRunningAt40ms");
                Verify(toast.TestBadgeRotation > 225,
                    "toast-badgeRotation=" + toast.TestBadgeRotation.ToString("R", CultureInfo.InvariantCulture));
                Verify(toast.TestBadgeCubics > 0, "toast-badgeCubicsWhileRunning");

                toast.HideAndClear();
                Verify(toast.TestBadgeShape == MaterialShapeKind.Circle, "toast-hideResetsShape");
                Verify(Near(toast.TestBadgeRotation, 225),
                    "toast-hideResetsRotation=" + toast.TestBadgeRotation.ToString("R", CultureInfo.InvariantCulture));
                Verify(toast.TestBadgeCubics > 0, "toast-hideKeepsCircleCubics");
            }
            finally
            {
                toast.Close();
            }
        }

        internal static void Run(string output)
        {
            outputDirectory = output;
            foreach (MaterialShapeKind shape in MaterialShapes.All) CheckGeometry(shape);
            RenderStatic();
            RenderFilmstrip();
            ToastIntegration();

            results.Add("TOTAL," + passed);
            File.WriteAllLines(Path.Combine(outputDirectory, "shape-check-results.csv"), results.ToArray());
            Console.WriteLine("Shape checks passed: " + passed);
        }
    }
}
