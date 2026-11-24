using System;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Scalar and point helpers ported from <c>androidx.graphics.shapes</c>.
    /// Kept explicit so the C# results stay numerically aligned with the
    /// TypeScript reference implementation.
    /// </summary>
    internal static class GeometryMathematics
    {
        internal const double DistanceEpsilon = 1e-4;
        internal const double AngleEpsilon = 1e-6;
        internal const double RelaxedDistanceEpsilon = 5e-3;

        internal static double Hypotenuse(double x, double y)
        {
            double ax = Math.Abs(x), ay = Math.Abs(y);
            double max = ax > ay ? ax : ay;
            if (max == 0) return 0;
            double ratioX = ax / max;
            double ratioY = ay / max;
            return Math.Sqrt(ratioX * ratioX + ratioY * ratioY) * max;
        }

        internal static double DistanceSquared(double x, double y)
        {
            return x * x + y * y;
        }

        internal static double Square(double value)
        {
            return value * value;
        }

        internal static ShapePoint DirectionVector(double x, double y)
        {
            double length = Hypotenuse(x, y);
            if (!(length > 0)) throw new InvalidOperationException("Required distance greater than zero");
            return new ShapePoint(x / length, y / length);
        }

        internal static ShapePoint DirectionVectorAtAngle(double angleRadians)
        {
            return new ShapePoint(FreelyDistributableMathLibrary.Cos(angleRadians), FreelyDistributableMathLibrary.Sin(angleRadians));
        }

        internal static ShapePoint RadialToCartesian(double radius, double angleRadians, ShapePoint center)
        {
            ShapePoint direction = DirectionVectorAtAngle(angleRadians);
            return new ShapePoint(direction.X * radius + center.X, direction.Y * radius + center.Y);
        }

        internal static ShapePoint Rotate90(ShapePoint point)
        {
            return new ShapePoint(-point.Y, point.X);
        }

        internal static double DotProduct(ShapePoint a, ShapePoint b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        internal static double PointDistance(ShapePoint a, ShapePoint b)
        {
            return Hypotenuse(a.X - b.X, a.Y - b.Y);
        }

        internal static double PointDistanceSquared(ShapePoint a, ShapePoint b)
        {
            return DistanceSquared(a.X - b.X, a.Y - b.Y);
        }

        internal static bool IsClockwise(ShapePoint a, ShapePoint b)
        {
            return a.X * b.Y - a.Y * b.X > 0;
        }

        internal static double Interpolate(double start, double stop, double fraction)
        {
            return (1 - fraction) * start + fraction * stop;
        }

        internal static ShapePoint InterpolatePoints(ShapePoint start, ShapePoint stop, double fraction)
        {
            return new ShapePoint(
                Interpolate(start.X, stop.X, fraction),
                Interpolate(start.Y, stop.Y, fraction));
        }

        internal static double PositiveModulo(double value, double modulus)
        {
            return ((value % modulus) + modulus) % modulus;
        }

        internal static double ToRadians(double degrees)
        {
            return (degrees / 360) * 2 * Math.PI;
        }

        internal static double AngleDegrees(ShapePoint point)
        {
            return (FreelyDistributableMathLibrary.Atan2(point.Y, point.X) * 180) / Math.PI;
        }

        internal static ShapePoint RotateDegrees(ShapePoint point, double angle, ShapePoint center)
        {
            double radians = ToRadians(angle);
            double offsetX = point.X - center.X, offsetY = point.Y - center.Y;
            double cos = FreelyDistributableMathLibrary.Cos(radians), sin = FreelyDistributableMathLibrary.Sin(radians);
            return new ShapePoint(
                offsetX * cos - offsetY * sin + center.X,
                offsetX * sin + offsetY * cos + center.Y);
        }

        /// <summary>
        /// Whether C lies on the line through A and B, within <paramref name="tolerance"/>.
        /// </summary>
        internal static bool IsApproximatelyCollinear(
            double aX,
            double aY,
            double bX,
            double bY,
            double cX,
            double cY,
            double tolerance = DistanceEpsilon)
        {
            ShapePoint ab = Rotate90(new ShapePoint(bX - aX, bY - aY));
            ShapePoint ac = new ShapePoint(cX - aX, cY - aY);
            double product = Math.Abs(DotProduct(ab, ac));
            double relativeTolerance = tolerance * Hypotenuse(ab.X, ab.Y) * Hypotenuse(ac.X, ac.Y);
            return product < tolerance || product < relativeTolerance;
        }

        /// <summary>
        /// Approximates whether the corner at <paramref name="current"/> is concave
        /// or convex, based on the previous -> current / current -> next vectors.
        /// </summary>
        internal static bool IsConvex(ShapePoint previous, ShapePoint current, ShapePoint next)
        {
            return IsClockwise(current - previous, next - current);
        }

        /// <summary>
        /// Distance between two progress values. Progress wraps around, so a
        /// numeric difference of 0.99 counts as a distance of 0.01.
        /// </summary>
        internal static double ProgressDistance(double first, double second)
        {
            double distance = Math.Abs(first - second);
            return Math.Min(distance, 1 - distance);
        }

        /// <summary>
        /// Whether progress is inside the (possibly wrapping) range
        /// [progressFrom, progressTo].
        /// </summary>
        internal static bool ProgressInRange(double progress, double progressFrom, double progressTo)
        {
            return progressTo >= progressFrom
                ? progress >= progressFrom && progress <= progressTo
                : progress >= progressFrom || progress <= progressTo;
        }

        /// <summary>Rounds half up, matching JavaScript's Math.round for non-negative values.</summary>
        internal static double RoundLikeJavaScript(double value)
        {
            return Math.Floor(value + 0.5);
        }
    }
}
