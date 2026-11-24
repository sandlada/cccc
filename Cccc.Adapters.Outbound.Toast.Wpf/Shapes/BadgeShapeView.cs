using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Cccc.MaterialShape;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    /// <summary>
    /// Vector host for the morphing badge. The shape is drawn in a local box,
    /// rotated around the box center; the check mark stays a sibling so it never
    /// rotates or scales with the blob.
    /// </summary>
    /// <remarks>
    /// Two rotations stack: the per-copy spring rotation is applied while
    /// drawing the outline, and an ambient spin uses a WPF animation on the
    /// element's render transform. The ambient clock runs on the composition
    /// thread, so the card does not need a managed frame callback while the
    /// badge idles and spins.
    /// </remarks>
    internal sealed class BadgeShapeView : FrameworkElement
    {
        private readonly double diameter;
        private readonly Cubic[] cubics = new Cubic[1024];
        private readonly RotateTransform ambientSpin = new RotateTransform();
        private int count;
        private Brush fill = Brushes.Transparent;
        private double rotationDegrees;
        private double ambientSpeed;

        internal BadgeShapeView(double diameter)
        {
            this.diameter = diameter;
            Width = diameter;
            Height = diameter;
            IsHitTestVisible = false;
            SnapsToDevicePixels = false;
            RenderTransformOrigin = new Point(.5, .5);
            RenderTransform = ambientSpin;
        }

        internal Brush Fill
        {
            get { return fill; }
            set
            {
                if (ReferenceEquals(fill, value)) return;
                fill = value ?? Brushes.Transparent;
                InvalidateVisual();
            }
        }

        internal double RotationDegrees
        {
            get { return rotationDegrees; }
            set
            {
                if (rotationDegrees == value) return;
                rotationDegrees = value;
                InvalidateVisual();
            }
        }

        internal int CubicCount { get { return count; } }

        /// <summary>Current ambient-spin angle in degrees.</summary>
        internal double AmbientAngle { get { return ambientSpin.Angle; } }

        internal bool AmbientRunning { get { return ambientSpin.HasAnimatedProperties; } }

        /// <summary>
        /// Starts an endless linear rotation of the blob (never the check mark).
        /// Repeating 0 → 360 keeps every revolution seamless. Calling this again
        /// with the same speed does not restart the clock, so repeated copy
        /// events do not make the badge jump back to zero.
        /// </summary>
        internal void StartAmbientSpin(double degreesPerSecond)
        {
            if (!(degreesPerSecond > 0))
            {
                StopAmbientSpin();
                return;
            }
            if (AmbientRunning && ambientSpeed == degreesPerSecond) return;
            ambientSpeed = degreesPerSecond;
            DoubleAnimation spin = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(360 / degreesPerSecond))
            {
                RepeatBehavior = RepeatBehavior.Forever,
            };
            ambientSpin.BeginAnimation(RotateTransform.AngleProperty, spin);
        }

        internal void StopAmbientSpin()
        {
            ambientSpeed = 0;
            ambientSpin.BeginAnimation(RotateTransform.AngleProperty, null);
            ambientSpin.Angle = 0;
        }

        internal void SetShape(RoundedPolygon polygon, double rotation)
        {
            if (polygon == null)
            {
                count = 0;
                rotationDegrees = rotation;
                InvalidateVisual();
                return;
            }
            SetShape(polygon.Cubics, rotation);
        }

        internal void SetShape(ReadOnlySpan<Cubic> source, double rotation)
        {
            if (source.Length > cubics.Length)
                throw new ArgumentException("The badge shape exceeds the cached cubic capacity.", nameof(source));
            source.CopyTo(cubics);
            count = source.Length;
            rotationDegrees = rotation;
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(diameter, diameter);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (count == 0 || fill == null) return;
            double width = ActualWidth > 0 ? ActualWidth : diameter;
            double height = ActualHeight > 0 ? ActualHeight : diameter;
            double size = Math.Min(width, height);
            if (!(size > 0)) return;

            StreamGeometry geometry = CreateGeometry(new ReadOnlySpan<Cubic>(cubics, 0, count), width, height);
            drawingContext.PushTransform(new RotateTransform(rotationDegrees, width / 2, height / 2));
            drawingContext.DrawGeometry(fill, null, geometry);
            drawingContext.Pop();
        }

        /// <summary>
        /// Builds the filled, closed outline in pixel coordinates. Exposed to the
        /// offscreen checks so geometry can be validated without a window.
        /// </summary>
        internal static StreamGeometry CreateGeometry(ReadOnlySpan<Cubic> source, double width, double height)
        {
            if (source.Length == 0) return new StreamGeometry();
            double size = Math.Min(width, height);
            double offsetX = (width - size) / 2;
            double offsetY = (height - size) / 2;

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                Cubic first = source[0];
                context.BeginFigure(
                    new Point(offsetX + first.Anchor0X * size, offsetY + first.Anchor0Y * size),
                    true,
                    true);
                for (int i = 0; i < source.Length; i++)
                {
                    Cubic cubic = source[i];
                    context.BezierTo(
                        new Point(offsetX + cubic.Control0X * size, offsetY + cubic.Control0Y * size),
                        new Point(offsetX + cubic.Control1X * size, offsetY + cubic.Control1Y * size),
                        new Point(offsetX + cubic.Anchor1X * size, offsetY + cubic.Anchor1Y * size),
                        true,
                        false);
                }
            }
            geometry.Freeze();
            return geometry;
        }
    }
}
