using System;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Immutable 2D point in the shape coordinate system (y grows downward,
    /// matching Canvas, Compose, and WPF).
    /// </summary>
    public readonly record struct ShapePoint(double X, double Y)
    {
        public static ShapePoint operator +(ShapePoint a, ShapePoint b)
        {
            return new ShapePoint(a.X + b.X, a.Y + b.Y);
        }

        public static ShapePoint operator -(ShapePoint a, ShapePoint b)
        {
            return new ShapePoint(a.X - b.X, a.Y - b.Y);
        }

        public static ShapePoint operator -(ShapePoint value)
        {
            return new ShapePoint(-value.X, -value.Y);
        }

        public static ShapePoint operator *(ShapePoint value, double factor)
        {
            return new ShapePoint(value.X * factor, value.Y * factor);
        }

        public static ShapePoint operator *(double factor, ShapePoint value)
        {
            return new ShapePoint(value.X * factor, value.Y * factor);
        }

        public static ShapePoint operator /(ShapePoint value, double divisor)
        {
            return new ShapePoint(value.X / divisor, value.Y / divisor);
        }
    }

    /// <summary>Axis-aligned bounds, in left/top/right/bottom order.</summary>
    public readonly record struct ShapeBounds(double Left, double Top, double Right, double Bottom)
    {
        public double Width { get { return Right - Left; } }

        public double Height { get { return Bottom - Top; } }

        public ShapePoint Center
        {
            get { return new ShapePoint((Left + Right) / 2, (Top + Bottom) / 2); }
        }
    }
}
