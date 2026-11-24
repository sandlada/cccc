using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Edges lie between corners and have no vertex or concavity; their curves
    /// are straight lines.
    /// </summary>
    public sealed class FeatureEdge : Feature
    {
        public FeatureEdge(IReadOnlyList<Cubic> cubics)
            : base(cubics)
        {
        }

        public override Feature Transformed(Func<ShapePoint, ShapePoint> transform)
        {
            Cubic[] transformed = new Cubic[Cubics.Count];
            for (int i = 0; i < transformed.Length; i++) transformed[i] = Cubics[i].Transformed(transform);
            return new FeatureEdge(transformed);
        }
    }
}
