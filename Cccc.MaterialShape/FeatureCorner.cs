using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Corners contain the curves describing how the corner is rounded, plus a
    /// flag indicating whether the corner is convex.
    /// </summary>
    public sealed class FeatureCorner : Feature
    {
        public FeatureCorner(IReadOnlyList<Cubic> cubics, bool convex)
            : base(cubics)
        {
            IsConvex = convex;
        }

        public bool IsConvex { get; }

        public override Feature Transformed(Func<ShapePoint, ShapePoint> transform)
        {
            Cubic[] transformed = new Cubic[Cubics.Count];
            for (int i = 0; i < transformed.Length; i++) transformed[i] = Cubics[i].Transformed(transform);
            return new FeatureCorner(transformed, IsConvex);
        }
    }
}
