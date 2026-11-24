using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// A single cubic Bezier curve: two anchors and two control points.
    /// The atom of the morphing-shape engine. Immutable.
    /// </summary>
    public readonly struct Cubic
    {
        public Cubic(
            double anchor0X,
            double anchor0Y,
            double control0X,
            double control0Y,
            double control1X,
            double control1Y,
            double anchor1X,
            double anchor1Y)
        {
            Anchor0X = anchor0X;
            Anchor0Y = anchor0Y;
            Control0X = control0X;
            Control0Y = control0Y;
            Control1X = control1X;
            Control1Y = control1Y;
            Anchor1X = anchor1X;
            Anchor1Y = anchor1Y;
        }

        public double Anchor0X { get; }

        public double Anchor0Y { get; }

        public double Control0X { get; }

        public double Control0Y { get; }

        public double Control1X { get; }

        public double Control1Y { get; }

        public double Anchor1X { get; }

        public double Anchor1Y { get; }

        public ShapePoint Anchor0 { get { return new ShapePoint(Anchor0X, Anchor0Y); } }

        public ShapePoint Anchor1 { get { return new ShapePoint(Anchor1X, Anchor1Y); } }

        /// <summary>Point on the curve at parameter t (0 = anchor0, 1 = anchor1).</summary>
        public ShapePoint PointOnCurve(double t)
        {
            double u = 1 - t;
            return new ShapePoint(
                Anchor0X * (u * u * u) +
                    Control0X * (3 * t * u * u) +
                    Control1X * (3 * t * t * u) +
                    Anchor1X * (t * t * t),
                Anchor0Y * (u * u * u) +
                    Control0Y * (3 * t * u * u) +
                    Control1Y * (3 * t * t * u) +
                    Anchor1Y * (t * t * t));
        }

        /// <summary>Whether both anchors coincide (within epsilon).</summary>
        public bool ZeroLength
        {
            get
            {
                return Math.Abs(Anchor0X - Anchor1X) < GeometryMathematics.DistanceEpsilon &&
                    Math.Abs(Anchor0Y - Anchor1Y) < GeometryMathematics.DistanceEpsilon;
            }
        }

        /// <summary>
        /// Axis-aligned bounds. When <paramref name="approximate"/> is set, uses
        /// the control-point hull (Skia "fast" bounds); otherwise solves the
        /// derivative for the true extremal points.
        /// </summary>
        public ShapeBounds CalculateBounds(bool approximate = false)
        {
            if (ZeroLength)
                return new ShapeBounds(Anchor0X, Anchor0Y, Anchor0X, Anchor0Y);

            double minX = Math.Min(Anchor0X, Anchor1X);
            double minY = Math.Min(Anchor0Y, Anchor1Y);
            double maxX = Math.Max(Anchor0X, Anchor1X);
            double maxY = Math.Max(Anchor0Y, Anchor1Y);

            if (approximate)
            {
                return new ShapeBounds(
                    Math.Min(minX, Math.Min(Control0X, Control1X)),
                    Math.Min(minY, Math.Min(Control0Y, Control1Y)),
                    Math.Max(maxX, Math.Max(Control0X, Control1X)),
                    Math.Max(maxY, Math.Max(Control0Y, Control1Y)));
            }

            double xa = -Anchor0X + 3 * Control0X - 3 * Control1X + Anchor1X;
            double xb = 2 * Anchor0X - 4 * Control0X + 2 * Control1X;
            double xc = -Anchor0X + Control0X;

            if (Math.Abs(xa) < GeometryMathematics.DistanceEpsilon)
            {
                if (xb != 0)
                {
                    double t = (2 * xc) / (-2 * xb);
                    if (t >= 0 && t <= 1)
                    {
                        double value = PointOnCurve(t).X;
                        if (value < minX) minX = value;
                        if (value > maxX) maxX = value;
                    }
                }
            }
            else
            {
                double xs = xb * xb - 4 * xa * xc;
                if (xs >= 0)
                {
                    double root = Math.Sqrt(xs);
                    double t1 = (-xb + root) / (2 * xa);
                    if (t1 >= 0 && t1 <= 1)
                    {
                        double value = PointOnCurve(t1).X;
                        if (value < minX) minX = value;
                        if (value > maxX) maxX = value;
                    }
                    double t2 = (-xb - root) / (2 * xa);
                    if (t2 >= 0 && t2 <= 1)
                    {
                        double value = PointOnCurve(t2).X;
                        if (value < minX) minX = value;
                        if (value > maxX) maxX = value;
                    }
                }
            }

            double ya = -Anchor0Y + 3 * Control0Y - 3 * Control1Y + Anchor1Y;
            double yb = 2 * Anchor0Y - 4 * Control0Y + 2 * Control1Y;
            double yc = -Anchor0Y + Control0Y;

            if (Math.Abs(ya) < GeometryMathematics.DistanceEpsilon)
            {
                if (yb != 0)
                {
                    double t = (2 * yc) / (-2 * yb);
                    if (t >= 0 && t <= 1)
                    {
                        double value = PointOnCurve(t).Y;
                        if (value < minY) minY = value;
                        if (value > maxY) maxY = value;
                    }
                }
            }
            else
            {
                double ys = yb * yb - 4 * ya * yc;
                if (ys >= 0)
                {
                    double root = Math.Sqrt(ys);
                    double t1 = (-yb + root) / (2 * ya);
                    if (t1 >= 0 && t1 <= 1)
                    {
                        double value = PointOnCurve(t1).Y;
                        if (value < minY) minY = value;
                        if (value > maxY) maxY = value;
                    }
                    double t2 = (-yb - root) / (2 * ya);
                    if (t2 >= 0 && t2 <= 1)
                    {
                        double value = PointOnCurve(t2).Y;
                        if (value < minY) minY = value;
                        if (value > maxY) maxY = value;
                    }
                }
            }

            return new ShapeBounds(minX, minY, maxX, maxY);
        }

        /// <summary>Splits this curve in two at parameter t.</summary>
        public (Cubic First, Cubic Second) Split(double t)
        {
            double u = 1 - t;
            ShapePoint onCurve = PointOnCurve(t);
            Cubic first = new Cubic(
                Anchor0X,
                Anchor0Y,
                Anchor0X * u + Control0X * t,
                Anchor0Y * u + Control0Y * t,
                Anchor0X * (u * u) + Control0X * (2 * u * t) + Control1X * (t * t),
                Anchor0Y * (u * u) + Control0Y * (2 * u * t) + Control1Y * (t * t),
                onCurve.X,
                onCurve.Y);
            Cubic second = new Cubic(
                onCurve.X,
                onCurve.Y,
                Control0X * (u * u) + Control1X * (2 * u * t) + Anchor1X * (t * t),
                Control0Y * (u * u) + Control1Y * (2 * u * t) + Anchor1Y * (t * t),
                Control1X * u + Anchor1X * t,
                Control1Y * u + Anchor1Y * t,
                Anchor1X,
                Anchor1Y);
            return (first, second);
        }

        /// <summary>Reverses the control / anchor order of this curve.</summary>
        public Cubic Reverse()
        {
            return new Cubic(
                Anchor1X,
                Anchor1Y,
                Control1X,
                Control1Y,
                Control0X,
                Control0Y,
                Anchor0X,
                Anchor0Y);
        }

        /// <summary>Transforms all four points with the supplied function.</summary>
        public Cubic Transformed(Func<ShapePoint, ShapePoint> transform)
        {
            ShapePoint p0 = transform(new ShapePoint(Anchor0X, Anchor0Y));
            ShapePoint p1 = transform(new ShapePoint(Control0X, Control0Y));
            ShapePoint p2 = transform(new ShapePoint(Control1X, Control1Y));
            ShapePoint p3 = transform(new ShapePoint(Anchor1X, Anchor1Y));
            return new Cubic(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
        }

        /// <summary>Creates a collapsed zero-size cubic at (x, y).</summary>
        public static Cubic Empty(double x, double y)
        {
            return new Cubic(x, y, x, y, x, y, x, y);
        }

        /// <summary>Straight line with controls at one third and two thirds.</summary>
        public static Cubic StraightLine(double x0, double y0, double x1, double y1)
        {
            return new Cubic(
                x0,
                y0,
                GeometryMathematics.Interpolate(x0, x1, 1.0 / 3),
                GeometryMathematics.Interpolate(y0, y1, 1.0 / 3),
                GeometryMathematics.Interpolate(x0, x1, 2.0 / 3),
                GeometryMathematics.Interpolate(y0, y1, 2.0 / 3),
                x1,
                y1);
        }

        /// <summary>
        /// Cubic approximating the smaller circular arc between two points that
        /// are equidistant from the center.
        /// </summary>
        public static Cubic CircularArc(
            double centerX,
            double centerY,
            double x0,
            double y0,
            double x1,
            double y1)
        {
            ShapePoint p0d = GeometryMathematics.DirectionVector(x0 - centerX, y0 - centerY);
            ShapePoint p1d = GeometryMathematics.DirectionVector(x1 - centerX, y1 - centerY);
            ShapePoint rotatedP0 = GeometryMathematics.Rotate90(p0d);
            ShapePoint rotatedP1 = GeometryMathematics.Rotate90(p1d);
            bool isClockwise = GeometryMathematics.DotProduct(
                rotatedP0,
                new ShapePoint(x1 - centerX, y1 - centerY)) >= 0;
            double cosa = GeometryMathematics.DotProduct(p0d, p1d);
            if (cosa > 0.999) return StraightLine(x0, y0, x1, y1);
            double k =
                GeometryMathematics.Hypotenuse(x0 - centerX, y0 - centerY) *
                (4.0 / 3.0) *
                ((Math.Sqrt(2 * (1 - cosa)) - Math.Sqrt(1 - cosa * cosa)) / (1 - cosa)) *
                (isClockwise ? 1 : -1);
            return new Cubic(
                x0,
                y0,
                x0 + rotatedP0.X * k,
                y0 + rotatedP0.Y * k,
                x1 - rotatedP1.X * k,
                y1 - rotatedP1.Y * k,
                x1,
                y1);
        }
    }
}
