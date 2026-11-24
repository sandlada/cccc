using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Ordered copy-feedback shape sequence. Mirrors the Material 3 expressive
    /// default sequence used by the reference web component, with a 45° rotation
    /// step per copy.
    /// </summary>
    public sealed class MaterialShapeSequence
    {
        public static IReadOnlyList<MaterialShapeKind> Default { get; } = new MaterialShapeKind[]
        {
            MaterialShapeKind.Circle,
            MaterialShapeKind.SoftBurst,
            MaterialShapeKind.Cookie9Sided,
            MaterialShapeKind.Pentagon,
            MaterialShapeKind.Pill,
            MaterialShapeKind.Sunny,
            MaterialShapeKind.Cookie4Sided,
            MaterialShapeKind.Heart,
        };

        private readonly IReadOnlyList<MaterialShapeKind> sequence;
        private int index;

        public MaterialShapeSequence(IReadOnlyList<MaterialShapeKind> sequence = null, double rotationPerStep = 45)
        {
            this.sequence = sequence ?? Default;
            if (this.sequence.Count == 0)
                throw new ArgumentException("A shape sequence needs at least one shape.", nameof(sequence));
            RotationPerStep = rotationPerStep;
            index = 0;
            Current = this.sequence[0];
        }

        public MaterialShapeKind Current { get; private set; }

        public RoundedPolygon CurrentPolygon { get { return MaterialShapes.Get(Current); } }

        public double RotationPerStep { get; }

        /// <summary>Advances to the next shape and returns its polygon.</summary>
        public RoundedPolygon Advance(out double rotationDelta)
        {
            if (sequence.Count > 1) index = (index + 1) % sequence.Count;
            Current = sequence[index];
            rotationDelta = RotationPerStep;
            return MaterialShapes.Get(Current);
        }

        /// <summary>Returns to the first shape without animating.</summary>
        public RoundedPolygon Reset()
        {
            index = 0;
            Current = sequence[0];
            return MaterialShapes.Get(Current);
        }
    }
}
