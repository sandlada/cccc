using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>Approximates arc length by splitting each cubic into segments.</summary>
    public sealed class LengthMeasurer : IMeasurer
    {
        private const int Segments = 3;

        public double MeasureCubic(Cubic cubic)
        {
            return ClosestProgressTo(cubic, double.PositiveInfinity).Measure;
        }

        public double FindCubicCutPoint(Cubic cubic, double measure)
        {
            return ClosestProgressTo(cubic, measure).Progress;
        }

        private static (double Progress, double Measure) ClosestProgressTo(Cubic cubic, double threshold)
        {
            double total = 0;
            double remainder = threshold;
            ShapePoint previous = new ShapePoint(cubic.Anchor0X, cubic.Anchor0Y);
            for (int i = 1; i <= Segments; i++)
            {
                double progress = (double)i / Segments;
                ShapePoint current = cubic.PointOnCurve(progress);
                double segment = GeometryMathematics.PointDistance(current, previous);
                if (segment >= remainder)
                {
                    return (progress - (1 - remainder / segment) / Segments, threshold);
                }
                remainder -= segment;
                total += segment;
                previous = current;
            }
            return (1, total);
        }
    }
}
