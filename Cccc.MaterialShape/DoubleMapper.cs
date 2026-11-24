using System;
using System.Collections.Generic;
using System.Linq;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Maps the rounded corners of two shapes onto each other and produces a
    /// progress-to-progress <see cref="DoubleMapper"/> that <see cref="Morph"/>
    /// uses to align the outlines. Ported from <c>FeatureMapping.kt</c> and
    /// <c>FloatMapping.kt</c>.
    /// </summary>
    public sealed class DoubleMapper
    {
        private readonly double[] sourceValues;
        private readonly double[] targetValues;

        public DoubleMapper(IReadOnlyList<(double Source, double Target)> mappings)
        {
            List<(double Source, double Target)> unique = new List<(double Source, double Target)>(mappings.Count);
            foreach ((double source, double target) in mappings)
            {
                if (unique.Count > 0)
                {
                    (double previousSource, double previousTarget) = unique[unique.Count - 1];
                    if (GeometryMathematics.ProgressDistance(source, previousSource) < GeometryMathematics.DistanceEpsilon ||
                        GeometryMathematics.ProgressDistance(target, previousTarget) < GeometryMathematics.DistanceEpsilon)
                    {
                        continue;
                    }
                }
                unique.Add((source, target));
            }

            // The validation also checks the wrap-around pair (last -> first).
            while (unique.Count >= 2 &&
                   (GeometryMathematics.ProgressDistance(unique[0].Source, unique[unique.Count - 1].Source) < GeometryMathematics.DistanceEpsilon ||
                    GeometryMathematics.ProgressDistance(unique[0].Target, unique[unique.Count - 1].Target) < GeometryMathematics.DistanceEpsilon))
            {
                unique.RemoveAt(unique.Count - 1);
            }

            if (unique.Count < 2)
            {
                unique = new List<(double Source, double Target)>(IdentityMapping);
            }

            sourceValues = new double[unique.Count];
            targetValues = new double[unique.Count];
            for (int i = 0; i < unique.Count; i++)
            {
                sourceValues[i] = unique[i].Source;
                targetValues[i] = unique[i].Target;
            }
            ValidateProgress(sourceValues);
            ValidateProgress(targetValues);
        }

        public double Map(double x)
        {
            return LinearMap(sourceValues, targetValues, x);
        }

        public double MapBack(double x)
        {
            return LinearMap(targetValues, sourceValues, x);
        }

        private static readonly (double Source, double Target)[] IdentityMapping =
        {
            (0, 0),
            (0.5, 0.5),
        };

        internal static IReadOnlyList<(double Source, double Target)> Identity { get { return IdentityMapping; } }

        /// <summary>
        /// Maps a value along a piecewise-linear wrapping mapping defined by the
        /// source and target progress lists.
        /// </summary>
        public static double LinearMap(double[] xValues, double[] yValues, double x)
        {
            if (!(x >= 0 && x <= 1)) throw new ArgumentOutOfRangeException(nameof(x), "Invalid progress: " + x);
            int segmentStartIndex = -1;
            for (int index = 0; index < xValues.Length; index++)
            {
                if (GeometryMathematics.ProgressInRange(x, xValues[index], xValues[(index + 1) % xValues.Length]))
                {
                    segmentStartIndex = index;
                    break;
                }
            }
            if (segmentStartIndex < 0) throw new InvalidOperationException("Progress was not covered by the mapping.");
            int segmentEndIndex = (segmentStartIndex + 1) % xValues.Length;
            double segmentSizeX = GeometryMathematics.PositiveModulo(
                xValues[segmentEndIndex] - xValues[segmentStartIndex],
                1);
            double segmentSizeY = GeometryMathematics.PositiveModulo(
                yValues[segmentEndIndex] - yValues[segmentStartIndex],
                1);
            double positionInSegment = segmentSizeX < 0.001
                ? 0.5
                : GeometryMathematics.PositiveModulo(x - xValues[segmentStartIndex], 1) / segmentSizeX;
            return GeometryMathematics.PositiveModulo(
                yValues[segmentStartIndex] + segmentSizeY * positionInSegment,
                1);
        }

        /// <summary>
        /// Verifies that progress values are in [0, 1) and monotonically
        /// increasing apart from at most one wrap-around.
        /// </summary>
        public static void ValidateProgress(IReadOnlyList<double> progressValues)
        {
            double previous = progressValues[progressValues.Count - 1];
            int wraps = 0;
            foreach (double current in progressValues)
            {
                if (!(current >= 0 && current < 1))
                    throw new ArgumentException("FloatMapping - Progress outside of range");
                if (!(GeometryMathematics.ProgressDistance(current, previous) > GeometryMathematics.DistanceEpsilon))
                    throw new ArgumentException("FloatMapping - Progress repeats a value");
                if (current < previous)
                {
                    wraps++;
                    if (wraps > 1) throw new ArgumentException("FloatMapping - Progress wraps more than once");
                }
                previous = current;
            }
        }
    }
}
