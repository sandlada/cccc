using System;
using System.Collections.Generic;
using System.Linq;

namespace Cccc.MaterialShape
{
    internal static class FeatureMapping
    {
        private sealed class DistanceVertex
        {
            internal double Distance;
            internal FeatureAtProgress First;
            internal FeatureAtProgress Second;
        }

        /// <summary>
        /// Creates the mapping between the corner features of two shapes.
        /// </summary>
        internal static DoubleMapper FeatureMapper(
            IReadOnlyList<FeatureAtProgress> features1,
            IReadOnlyList<FeatureAtProgress> features2)
        {
            List<FeatureAtProgress> filtered1 = new List<FeatureAtProgress>();
            List<FeatureAtProgress> filtered2 = new List<FeatureAtProgress>();
            foreach (FeatureAtProgress feature in features1)
                if (feature.Feature is FeatureCorner) filtered1.Add(feature);
            foreach (FeatureAtProgress feature in features2)
                if (feature.Feature is FeatureCorner) filtered2.Add(feature);
            return new DoubleMapper(DoMapping(filtered1, filtered2));
        }

        /// <summary>
        /// Greedily pairs features from smallest to largest distance, ensuring no
        /// feature is used twice and no mapping crosses an existing one.
        /// </summary>
        internal static IReadOnlyList<(double Source, double Target)> DoMapping(
            IReadOnlyList<FeatureAtProgress> features1,
            IReadOnlyList<FeatureAtProgress> features2)
        {
            List<DistanceVertex> distanceVertices = new List<DistanceVertex>();
            foreach (FeatureAtProgress first in features1)
            {
                foreach (FeatureAtProgress second in features2)
                {
                    double distance = FeatureDistanceSquared(first.Feature, second.Feature);
                    if (distance != double.MaxValue)
                    {
                        distanceVertices.Add(new DistanceVertex
                        {
                            Distance = distance,
                            First = first,
                            Second = second,
                        });
                    }
                }
            }

            // OrderBy is stable, matching V8's stable Array.prototype.sort.
            List<DistanceVertex> ordered = distanceVertices.OrderBy(vertex => vertex.Distance).ToList();

            if (ordered.Count == 0) return DoubleMapper.Identity;
            if (ordered.Count == 1)
            {
                DistanceVertex only = ordered[0];
                return new (double Source, double Target)[]
                {
                    (only.First.Progress, only.Second.Progress),
                    (
                        GeometryMathematics.PositiveModulo(only.First.Progress + 0.5, 1),
                        GeometryMathematics.PositiveModulo(only.Second.Progress + 0.5, 1)),
                };
            }

            FeatureMappingBuilder helper = new FeatureMappingBuilder();
            foreach (DistanceVertex vertex in ordered) helper.AddMapping(vertex.First, vertex.Second);
            return helper.Mapping;
        }

        private static int BinarySearchByFirst(List<(double Source, double Target)> mapping, double value)
        {
            int low = 0;
            int high = mapping.Count - 1;
            while (low <= high)
            {
                int middle = (low + high) >> 1;
                double comparison = mapping[middle].Source - value;
                if (comparison < 0) low = middle + 1;
                else if (comparison > 0) high = middle - 1;
                else return middle;
            }
            return -(low + 1);
        }

        private sealed class FeatureMappingBuilder
        {
            private readonly HashSet<FeatureAtProgress> usedFirst = new HashSet<FeatureAtProgress>();
            private readonly HashSet<FeatureAtProgress> usedSecond = new HashSet<FeatureAtProgress>();
            private readonly List<(double Source, double Target)> mapping = new List<(double, double)>();

            internal IReadOnlyList<(double Source, double Target)> Mapping { get { return mapping; } }

            internal void AddMapping(FeatureAtProgress first, FeatureAtProgress second)
            {
                if (usedFirst.Contains(first) || usedSecond.Contains(second)) return;

                int index = BinarySearchByFirst(mapping, first.Progress);
                if (index >= 0) throw new InvalidOperationException("There can't be two features with the same progress");
                int insertionIndex = -index - 1;
                int count = mapping.Count;

                if (count >= 1)
                {
                    (double beforeSource, double beforeTarget) = mapping[(insertionIndex + count - 1) % count];
                    (double afterSource, double afterTarget) = mapping[insertionIndex % count];

                    if (GeometryMathematics.ProgressDistance(first.Progress, beforeSource) < GeometryMathematics.DistanceEpsilon ||
                        GeometryMathematics.ProgressDistance(first.Progress, afterSource) < GeometryMathematics.DistanceEpsilon ||
                        GeometryMathematics.ProgressDistance(second.Progress, beforeTarget) < GeometryMathematics.DistanceEpsilon ||
                        GeometryMathematics.ProgressDistance(second.Progress, afterTarget) < GeometryMathematics.DistanceEpsilon)
                    {
                        return;
                    }

                    if (count > 1 && !GeometryMathematics.ProgressInRange(second.Progress, beforeTarget, afterTarget)) return;
                }

                mapping.Insert(insertionIndex, (first.Progress, second.Progress));
                usedFirst.Add(first);
                usedSecond.Add(second);
            }
        }

        /// <summary>
        /// Distance between two features. Corners of opposite concavity never map
        /// (infinite distance).
        /// </summary>
        internal static double FeatureDistanceSquared(Feature first, Feature second)
        {
            if (first is FeatureCorner firstCorner &&
                second is FeatureCorner secondCorner &&
                firstCorner.IsConvex != secondCorner.IsConvex)
            {
                return double.MaxValue;
            }
            return GeometryMathematics.PointDistanceSquared(
                FeatureRepresentativePoint(first),
                FeatureRepresentativePoint(second));
        }

        /// <summary>Midpoint between the end anchors of a feature.</summary>
        internal static ShapePoint FeatureRepresentativePoint(Feature feature)
        {
            Cubic first = feature.Cubics[0];
            Cubic last = feature.Cubics[feature.Cubics.Count - 1];
            return new ShapePoint(
                (first.Anchor0X + last.Anchor1X) / 2,
                (first.Anchor0Y + last.Anchor1Y) / 2);
        }
    }
}
