using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal static class CardOutline
    {
        internal const double CornerExtent = 26;
        internal const double FlattenTolerance = 0.02; // Final physical pixels.

        internal static Point[] Controls(double c)
        {
            // Extended endpoint tangents soften the straight-to-curve junction.
            // Equal endpoint control spacing preserves zero endpoint curvature.
            return new Point[] { new Point(0, 0), new Point(.36 * c, 0), new Point(.72 * c, 0),
                new Point(c, .28 * c), new Point(c, .64 * c), new Point(c, c) };
        }

        internal static Point Evaluate(Point[] controls, double t)
        {
            Point[] values = (Point[])controls.Clone();
            for (int n = values.Length - 1; n > 0; n--)
                for (int i = 0; i < n; i++) values[i] = Lerp(values[i], values[i + 1], t);
            return values[0];
        }

        internal static List<Point> Flatten(Point[] controls, double tolerance)
        {
            List<Point> result = new List<Point> { controls[0] };
            FlattenInto(controls, tolerance, 0, result);
            return result;
        }

        private static void FlattenInto(Point[] p, double tolerance, int depth, List<Point> points)
        {
            double error = 0;
            Vector chord = p[p.Length - 1] - p[0];
            double length = chord.Length;
            for (int i = 1; i < p.Length - 1; i++)
            {
                Vector v = p[i] - p[0];
                double distance = length > 0 ? Math.Abs(Vector.CrossProduct(chord, v)) / length : v.Length;
                error = Math.Max(error, distance);
            }
            // A Bezier curve lies in its control hull. This bounds deviation from
            // the chord for this monotonic convex corner, rather than guessing steps.
            if (error <= tolerance || depth >= 20) { points.Add(p[p.Length - 1]); return; }
            Point[] row = (Point[])p.Clone(), left = new Point[p.Length], right = new Point[p.Length];
            left[0] = row[0]; right[p.Length - 1] = row[p.Length - 1];
            for (int n = p.Length - 1, level = 1; n > 0; n--, level++)
            {
                for (int i = 0; i < n; i++) row[i] = Lerp(row[i], row[i + 1], .5);
                left[level] = row[0]; right[p.Length - 1 - level] = row[n - 1];
            }
            FlattenInto(left, tolerance, depth + 1, points);
            FlattenInto(right, tolerance, depth + 1, points);
        }

        private static Point Lerp(Point a, Point b, double t)
        { return new Point(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t); }

        internal static Geometry Create(Rect rect, double scale)
        {
            if (ToastProfile.Compact)
            {
                RectangleGeometry capsule = new RectangleGeometry(rect, rect.Height / 2, rect.Height / 2);
                capsule.Freeze(); return capsule;
            }
            double c = Math.Min(CornerExtent * scale, Math.Min(rect.Width, rect.Height) / 2);
            List<Point> corner = Flatten(Controls(c), FlattenTolerance);
            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(new Point(rect.Left + c, rect.Top), true, true);
                context.LineTo(new Point(rect.Right - c, rect.Top), true, false);
                AddCorner(context, corner, new Point(rect.Right - c, rect.Top), 0);
                context.LineTo(new Point(rect.Right, rect.Bottom - c), true, false);
                AddCorner(context, corner, new Point(rect.Right, rect.Bottom - c), 1);
                context.LineTo(new Point(rect.Left + c, rect.Bottom), true, false);
                AddCorner(context, corner, new Point(rect.Left + c, rect.Bottom), 2);
                context.LineTo(new Point(rect.Left, rect.Top + c), true, false);
                AddCorner(context, corner, new Point(rect.Left, rect.Top + c), 3);
            }
            geometry.Freeze(); return geometry;
        }

        private static void AddCorner(StreamGeometryContext context, List<Point> points, Point start, int turn)
        {
            for (int i = 1; i < points.Count; i++)
            {
                Point p = points[i];
                Point rotated = turn == 0 ? p : turn == 1 ? new Point(-p.Y, p.X) :
                    turn == 2 ? new Point(-p.X, -p.Y) : new Point(p.Y, -p.X);
                context.LineTo(new Point(start.X + rotated.X, start.Y + rotated.Y), true, true);
            }
        }
    }
}
