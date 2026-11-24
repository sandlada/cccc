using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>Corner rounding options for a polygon vertex.</summary>
    public readonly record struct CornerRounding(double Radius, double Smoothing = 0)
    {
        public static CornerRounding Unrounded { get { return new CornerRounding(0, 0); } }
    }

    /// <summary>
    /// A polygonal shape with optional rounded/smoothed corners. Immutable.
    /// </summary>
    public sealed class RoundedPolygon
    {
        private readonly ShapePoint designCenter;

        public RoundedPolygon(IReadOnlyList<Feature> features, ShapePoint center)
        {
            if (features == null) throw new ArgumentNullException(nameof(features));
            Features = features;
            designCenter = center;
            Cubics = FlattenCubics(features, center);

            // Contiguity validation matching RoundedPolygon.kt's init block.
            Cubic previous = Cubics[Cubics.Length - 1];
            foreach (Cubic cubic in Cubics)
            {
                if (Math.Abs(cubic.Anchor0X - previous.Anchor1X) > GeometryMathematics.DistanceEpsilon ||
                    Math.Abs(cubic.Anchor0Y - previous.Anchor1Y) > GeometryMathematics.DistanceEpsilon)
                {
                    throw new InvalidOperationException(
                        "RoundedPolygon must be contiguous, with the anchor points of all curves " +
                        "matching the anchor points of the preceding and succeeding cubics");
                }
                previous = cubic;
            }
        }

        public IReadOnlyList<Feature> Features { get; }

        public Cubic[] Cubics { get; }

        public ShapePoint Center { get { return designCenter; } }

        public double CenterX { get { return designCenter.X; } }

        public double CenterY { get { return designCenter.Y; } }

        /// <summary>Transforms (scales / rotates / translates) this polygon.</summary>
        public RoundedPolygon Transformed(Func<ShapePoint, ShapePoint> transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            Feature[] features = new Feature[Features.Count];
            for (int i = 0; i < features.Length; i++) features[i] = Features[i].Transformed(transform);
            return new RoundedPolygon(features, transform(designCenter));
        }

        /// <summary>
        /// Moves and resizes the shape to fit inside the (0,0) -> (1,1) unit
        /// square, centered when the bounds are non-square.
        /// </summary>
        public RoundedPolygon Normalized()
        {
            ShapeBounds bounds = CalculateBounds();
            double width = bounds.Right - bounds.Left;
            double height = bounds.Bottom - bounds.Top;
            double side = Math.Max(width, height);
            if (!(side > 0)) return this;
            double offsetX = (side - width) / 2 - bounds.Left;
            double offsetY = (side - height) / 2 - bounds.Top;
            return Transformed(point => new ShapePoint((point.X + offsetX) / side, (point.Y + offsetY) / side));
        }

        /// <summary>
        /// Square around the center that holds this shape under any rotation.
        /// </summary>
        public ShapeBounds CalculateMaxBounds()
        {
            double maxDistanceSquared = 0;
            foreach (Cubic cubic in Cubics)
            {
                double anchorDistance = GeometryMathematics.DistanceSquared(
                    cubic.Anchor0X - CenterX,
                    cubic.Anchor0Y - CenterY);
                ShapePoint middle = cubic.PointOnCurve(0.5);
                double middleDistance = GeometryMathematics.DistanceSquared(
                    middle.X - CenterX,
                    middle.Y - CenterY);
                maxDistanceSquared = Math.Max(maxDistanceSquared, Math.Max(anchorDistance, middleDistance));
            }
            double maxDistance = Math.Sqrt(maxDistanceSquared);
            return new ShapeBounds(
                CenterX - maxDistance,
                CenterY - maxDistance,
                CenterX + maxDistance,
                CenterY + maxDistance);
        }

        /// <summary>Axis-aligned bounds of the outline.</summary>
        public ShapeBounds CalculateBounds(bool approximate = true)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = -double.MaxValue;
            double maxY = -double.MaxValue;
            foreach (Cubic cubic in Cubics)
            {
                ShapeBounds bounds = cubic.CalculateBounds(approximate);
                minX = Math.Min(minX, bounds.Left);
                minY = Math.Min(minY, bounds.Top);
                maxX = Math.Max(maxX, bounds.Right);
                maxY = Math.Max(maxY, bounds.Bottom);
            }
            return new ShapeBounds(minX, minY, maxX, maxY);
        }

        private static Cubic[] FlattenCubics(IReadOnlyList<Feature> features, ShapePoint center)
        {
            List<Cubic> result = new List<Cubic>();
            Cubic? firstCubic = null;
            Cubic? lastCubic = null;
            Cubic[] firstFeatureSplitStart = null;
            Cubic[] firstFeatureSplitEnd = null;

            if (features.Count > 0 && features[0].Cubics.Count == 3)
            {
                Cubic centerCubic = features[0].Cubics[1];
                (Cubic start, Cubic end) = centerCubic.Split(0.5);
                firstFeatureSplitStart = new Cubic[] { features[0].Cubics[0], start };
                firstFeatureSplitEnd = new Cubic[] { end, features[0].Cubics[2] };
            }

            for (int i = 0; i <= features.Count; i++)
            {
                IReadOnlyList<Cubic> featureCubics;
                if (i == 0 && firstFeatureSplitEnd != null)
                {
                    featureCubics = firstFeatureSplitEnd;
                }
                else if (i == features.Count)
                {
                    if (firstFeatureSplitStart != null) featureCubics = firstFeatureSplitStart;
                    else break;
                }
                else
                {
                    featureCubics = features[i].Cubics;
                }

                foreach (Cubic cubic in featureCubics)
                {
                    if (!cubic.ZeroLength)
                    {
                        if (lastCubic.HasValue) result.Add(lastCubic.Value);
                        lastCubic = cubic;
                        if (!firstCubic.HasValue) firstCubic = cubic;
                    }
                    else if (lastCubic.HasValue)
                    {
                        Cubic patched = lastCubic.Value;
                        lastCubic = new Cubic(
                            patched.Anchor0X,
                            patched.Anchor0Y,
                            patched.Control0X,
                            patched.Control0Y,
                            patched.Control1X,
                            patched.Control1Y,
                            cubic.Anchor1X,
                            cubic.Anchor1Y);
                    }
                }
            }

            if (lastCubic.HasValue && firstCubic.HasValue)
            {
                Cubic last = lastCubic.Value;
                Cubic first = firstCubic.Value;
                result.Add(new Cubic(
                    last.Anchor0X,
                    last.Anchor0Y,
                    last.Control0X,
                    last.Control0Y,
                    last.Control1X,
                    last.Control1Y,
                    first.Anchor0X,
                    first.Anchor0Y));
            }
            else
            {
                result.Add(Cubic.Empty(center.X, center.Y));
            }
            return result.ToArray();
        }
    }
}
