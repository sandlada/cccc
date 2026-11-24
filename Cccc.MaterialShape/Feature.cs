using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>A named, contiguous group of cubics (an edge or a corner).</summary>
    public abstract class Feature
    {
        protected Feature(IReadOnlyList<Cubic> cubics)
        {
            if (cubics == null) throw new ArgumentNullException(nameof(cubics));
            if (cubics.Count == 0) throw new ArgumentException("Features need at least one cubic.", nameof(cubics));
            Cubics = cubics;
        }

        public IReadOnlyList<Cubic> Cubics { get; }

        public abstract Feature Transformed(Func<ShapePoint, ShapePoint> transform);
    }
}
