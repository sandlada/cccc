using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Morphs between two <see cref="RoundedPolygon"/> outlines by matching their
    /// corners and cutting cubics until both outlines align one-to-one. Ported
    /// from <c>androidx.graphics.shapes.Morph</c>.
    /// </summary>
    public sealed class Morph
    {
        private readonly RoundedPolygon start;
        private readonly RoundedPolygon end;
        private readonly (Cubic Start, Cubic End)[] morphMatch;
        private readonly double[] startPoints;
        private readonly double[] endPoints;
        private readonly ShapePoint startCenter;
        private readonly ShapePoint endCenter;

        public Morph(RoundedPolygon start, RoundedPolygon end)
        {
            this.start = start ?? throw new ArgumentNullException(nameof(start));
            this.end = end ?? throw new ArgumentNullException(nameof(end));
            morphMatch = MatchCubics(start, end);
            startPoints = new double[morphMatch.Length * 8];
            endPoints = new double[morphMatch.Length * 8];
            for (int i = 0; i < morphMatch.Length; i++)
            {
                CopyPoints(morphMatch[i].Start, startPoints, i * 8);
                CopyPoints(morphMatch[i].End, endPoints, i * 8);
            }
            startCenter = CubicsAreaCentroid(start.Cubics);
            endCenter = CubicsAreaCentroid(end.Cubics);
        }

        public RoundedPolygon StartPolygon { get { return start; } }

        public RoundedPolygon EndPolygon { get { return end; } }

        public int CurveCount { get { return morphMatch.Length; } }

        /// <summary>Union axis-aligned bounds of start and end.</summary>
        public ShapeBounds CalculateBounds(bool approximate = true)
        {
            ShapeBounds first = start.CalculateBounds(approximate);
            ShapeBounds second = end.CalculateBounds(approximate);
            return new ShapeBounds(
                Math.Min(first.Left, second.Left),
                Math.Min(first.Top, second.Top),
                Math.Max(first.Right, second.Right),
                Math.Max(first.Bottom, second.Bottom));
        }

        /// <summary>Union rotation-safe max bounds of start and end.</summary>
        public ShapeBounds CalculateMaxBounds()
        {
            ShapeBounds first = start.CalculateMaxBounds();
            ShapeBounds second = end.CalculateMaxBounds();
            return new ShapeBounds(
                Math.Min(first.Left, second.Left),
                Math.Min(first.Top, second.Top),
                Math.Max(first.Right, second.Right),
                Math.Max(first.Bottom, second.Bottom));
        }

        /// <summary>Interpolated area centroid at the given progress.</summary>
        public ShapePoint CenterAt(double progress)
        {
            return new ShapePoint(
                GeometryMathematics.Interpolate(startCenter.X, endCenter.X, progress),
                GeometryMathematics.Interpolate(startCenter.Y, endCenter.Y, progress));
        }

        /// <summary>
        /// Morphed shape at the given progress as a closed list of cubics.
        /// Values outside [0, 1] extrapolate (for spring overshoot).
        /// </summary>
        public Cubic[] AsCubics(double progress)
        {
            Cubic[] result = new Cubic[CurveCount];
            Sample(progress, result);
            return result;
        }

        /// <summary>
        /// Allocation-free variant of <see cref="AsCubics"/>. Returns the number
        /// of cubics written. The destination must have room for
        /// <see cref="CurveCount"/> entries.
        /// </summary>
        public int Sample(double progress, Span<Cubic> destination)
        {
            int count = morphMatch.Length;
            if (destination.Length < count)
                throw new ArgumentException("Destination is smaller than the morph's curve count.", nameof(destination));

            Cubic first = default(Cubic);
            Cubic last = default(Cubic);
            for (int i = 0; i < count; i++)
            {
                int offset = i * 8;
                Cubic cubic = new Cubic(
                    GeometryMathematics.Interpolate(startPoints[offset + 0], endPoints[offset + 0], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 1], endPoints[offset + 1], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 2], endPoints[offset + 2], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 3], endPoints[offset + 3], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 4], endPoints[offset + 4], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 5], endPoints[offset + 5], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 6], endPoints[offset + 6], progress),
                    GeometryMathematics.Interpolate(startPoints[offset + 7], endPoints[offset + 7], progress));

                if (i == 0)
                {
                    first = cubic;
                }
                else
                {
                    destination[i - 1] = last;
                }
                last = cubic;
            }

            if (count > 0)
            {
                destination[count - 1] = new Cubic(
                    last.Anchor0X,
                    last.Anchor0Y,
                    last.Control0X,
                    last.Control0Y,
                    last.Control1X,
                    last.Control1Y,
                    first.Anchor0X,
                    first.Anchor0Y);
            }
            return count;
        }

        /// <summary>
        /// Captures the exact live intermediate shape at the given progress so an
        /// interrupted morph can transition to a new target without snapping.
        /// </summary>
        public RoundedPolygon ToPolygon(double progress)
        {
            if (progress == 0) return start;
            if (progress == 1) return end;

            Cubic[] raw = AsCubics(progress);
            List<Cubic> nonZero = new List<Cubic>(raw.Length);
            foreach (Cubic cubic in raw)
                if (!cubic.ZeroLength) nonZero.Add(cubic);
            if (nonZero.Count == 0) return progress < 0.5 ? start : end;

            Cubic[] cubics = new Cubic[nonZero.Count];
            for (int i = 0; i < nonZero.Count; i++)
            {
                Cubic cubic = nonZero[i];
                Cubic next = nonZero[(i + 1) % nonZero.Count];
                if (cubic.Anchor1X == next.Anchor0X && cubic.Anchor1Y == next.Anchor0Y)
                {
                    cubics[i] = cubic;
                }
                else
                {
                    cubics[i] = new Cubic(
                        cubic.Anchor0X,
                        cubic.Anchor0Y,
                        cubic.Control0X,
                        cubic.Control0Y,
                        cubic.Control1X,
                        cubic.Control1Y,
                        next.Anchor0X,
                        next.Anchor0Y);
                }
            }

            List<Feature> features = new List<Feature>();
            for (int i = 0; i < cubics.Length; i++)
            {
                Cubic previous = cubics[(i + cubics.Length - 1) % cubics.Length];
                Cubic current = cubics[i];
                bool isStraight =
                    GeometryMathematics.IsApproximatelyCollinear(
                        current.Anchor0X,
                        current.Anchor0Y,
                        current.Control0X,
                        current.Control0Y,
                        current.Anchor1X,
                        current.Anchor1Y,
                        GeometryMathematics.RelaxedDistanceEpsilon) &&
                    GeometryMathematics.IsApproximatelyCollinear(
                        current.Anchor0X,
                        current.Anchor0Y,
                        current.Control1X,
                        current.Control1Y,
                        current.Anchor1X,
                        current.Anchor1Y,
                        GeometryMathematics.RelaxedDistanceEpsilon);

                if (isStraight)
                {
                    if (!GeometryMathematics.IsApproximatelyCollinear(
                            previous.Anchor0X,
                            previous.Anchor0Y,
                            current.Anchor0X,
                            current.Anchor0Y,
                            current.Anchor1X,
                            current.Anchor1Y,
                            GeometryMathematics.RelaxedDistanceEpsilon))
                    {
                        bool isConvex = GeometryMathematics.IsConvex(
                            new ShapePoint(previous.Anchor0X, previous.Anchor0Y),
                            new ShapePoint(current.Anchor0X, current.Anchor0Y),
                            new ShapePoint(current.Anchor1X, current.Anchor1Y));
                        features.Add(new FeatureCorner(
                            new Cubic[] { Cubic.Empty(current.Anchor0X, current.Anchor0Y) },
                            isConvex));
                    }
                    features.Add(new FeatureEdge(new Cubic[] { current }));
                }
                else
                {
                    ShapePoint middle = current.PointOnCurve(0.5);
                    bool isConvex = GeometryMathematics.IsConvex(
                        new ShapePoint(current.Anchor0X, current.Anchor0Y),
                        middle,
                        new ShapePoint(current.Anchor1X, current.Anchor1Y));
                    features.Add(new FeatureCorner(new Cubic[] { current }, isConvex));
                }
            }

            return new RoundedPolygon(features, CenterAt(progress));
        }

        /// <summary>Center of the control-point (approximate) bounds of a cubic list.</summary>
        public static ShapePoint CubicsBoundsCenter(IReadOnlyList<Cubic> cubics)
        {
            if (cubics.Count == 0) throw new ArgumentException("Cannot compute bounds of an empty cubic list", nameof(cubics));
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = -double.MaxValue;
            double maxY = -double.MaxValue;
            foreach (Cubic cubic in cubics)
            {
                minX = Math.Min(minX, Math.Min(cubic.Anchor0X, Math.Min(cubic.Control0X, Math.Min(cubic.Control1X, cubic.Anchor1X))));
                minY = Math.Min(minY, Math.Min(cubic.Anchor0Y, Math.Min(cubic.Control0Y, Math.Min(cubic.Control1Y, cubic.Anchor1Y))));
                maxX = Math.Max(maxX, Math.Max(cubic.Anchor0X, Math.Max(cubic.Control0X, Math.Max(cubic.Control1X, cubic.Anchor1X))));
                maxY = Math.Max(maxY, Math.Max(cubic.Anchor0Y, Math.Max(cubic.Control0Y, Math.Max(cubic.Control1Y, cubic.Anchor1Y))));
            }
            return new ShapePoint((minX + maxX) / 2, (minY + maxY) / 2);
        }

        /// <summary>Area centroid of a closed cubic outline via Green's theorem.</summary>
        public static ShapePoint CubicsAreaCentroid(IReadOnlyList<Cubic> cubics, int stepsPerCubic = 8)
        {
            if (cubics.Count == 0) throw new ArgumentException("Cannot compute centroid of an empty cubic list", nameof(cubics));
            double twiceArea = 0;
            double centroidX = 0;
            double centroidY = 0;
            foreach (Cubic cubic in cubics)
            {
                double previousX = cubic.Anchor0X;
                double previousY = cubic.Anchor0Y;
                for (int step = 1; step <= stepsPerCubic; step++)
                {
                    ShapePoint point = cubic.PointOnCurve((double)step / stepsPerCubic);
                    double cross = previousX * point.Y - point.X * previousY;
                    twiceArea += cross;
                    centroidX += (previousX + point.X) * cross;
                    centroidY += (previousY + point.Y) * cross;
                    previousX = point.X;
                    previousY = point.Y;
                }
            }
            if (Math.Abs(twiceArea) < 1e-12) return CubicsBoundsCenter(cubics);
            double factor = 1 / (3 * twiceArea);
            return new ShapePoint(centroidX * factor, centroidY * factor);
        }

        private static (Cubic Start, Cubic End)[] MatchCubics(RoundedPolygon first, RoundedPolygon second)
        {
            MeasuredPolygon measuredFirst = MeasuredPolygon.Measure(new LengthMeasurer(), first);
            MeasuredPolygon measuredSecond = MeasuredPolygon.Measure(new LengthMeasurer(), second);
            DoubleMapper doubleMapper = FeatureMapping.FeatureMapper(measuredFirst.Features, measuredSecond.Features);

            double secondCutPoint = doubleMapper.Map(0);
            MeasuredPolygon firstPolygon = measuredFirst;
            MeasuredPolygon secondPolygon = measuredSecond.CutAndShift(secondCutPoint);

            List<(Cubic, Cubic)> result = new List<(Cubic, Cubic)>();
            int firstIndex = 0;
            int secondIndex = 0;
            MeasuredCubic firstCubic = firstPolygon.GetOrNull(firstIndex++);
            MeasuredCubic secondCubic = secondPolygon.GetOrNull(secondIndex++);
            while (firstCubic != null && secondCubic != null)
            {
                double firstEnd = firstIndex == firstPolygon.Size ? 1 : firstCubic.EndOutlineProgress;
                double secondEnd = secondIndex == secondPolygon.Size
                    ? 1
                    : doubleMapper.MapBack(GeometryMathematics.PositiveModulo(
                        secondCubic.EndOutlineProgress + secondCutPoint,
                        1));
                double minimum = Math.Min(firstEnd, secondEnd);

                MeasuredCubic firstSegment;
                MeasuredCubic nextFirst;
                if (firstEnd > minimum + GeometryMathematics.AngleEpsilon)
                {
                    (MeasuredCubic cutFirst, MeasuredCubic cutSecond) = firstCubic.CutAtProgress(minimum);
                    firstSegment = cutFirst;
                    nextFirst = cutSecond;
                }
                else
                {
                    firstSegment = firstCubic;
                    nextFirst = firstPolygon.GetOrNull(firstIndex++);
                }

                MeasuredCubic secondSegment;
                MeasuredCubic nextSecond;
                if (secondEnd > minimum + GeometryMathematics.AngleEpsilon)
                {
                    (MeasuredCubic cutFirst, MeasuredCubic cutSecond) = secondCubic.CutAtProgress(
                        GeometryMathematics.PositiveModulo(doubleMapper.Map(minimum) - secondCutPoint, 1));
                    secondSegment = cutFirst;
                    nextSecond = cutSecond;
                }
                else
                {
                    secondSegment = secondCubic;
                    nextSecond = secondPolygon.GetOrNull(secondIndex++);
                }

                result.Add((firstSegment.Cubic, secondSegment.Cubic));
                firstCubic = nextFirst;
                secondCubic = nextSecond;
            }
            if (firstCubic != null || secondCubic != null)
                throw new InvalidOperationException("Expected both polygons' cubics to be fully matched");
            return result.ToArray();
        }

        private static void CopyPoints(Cubic cubic, double[] destination, int offset)
        {
            destination[offset + 0] = cubic.Anchor0X;
            destination[offset + 1] = cubic.Anchor0Y;
            destination[offset + 2] = cubic.Control0X;
            destination[offset + 3] = cubic.Control0Y;
            destination[offset + 4] = cubic.Control1X;
            destination[offset + 5] = cubic.Control1Y;
            destination[offset + 6] = cubic.Anchor1X;
            destination[offset + 7] = cubic.Anchor1Y;
        }
    }
}
