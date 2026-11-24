using System;

namespace Cccc.MaterialShape
{
    /// <summary>One set of Material 3 motion tokens (spatial and effects springs).</summary>
    public sealed class MotionSchemeSet
    {
        public MotionSchemeSet(
            SpringParameters fastSpatial,
            SpringParameters defaultSpatial,
            SpringParameters slowSpatial,
            SpringParameters fastEffects,
            SpringParameters defaultEffects,
            SpringParameters slowEffects)
        {
            FastSpatial = fastSpatial;
            DefaultSpatial = defaultSpatial;
            SlowSpatial = slowSpatial;
            FastEffects = fastEffects;
            DefaultEffects = defaultEffects;
            SlowEffects = slowEffects;
        }

        public SpringParameters FastSpatial { get; }

        public SpringParameters DefaultSpatial { get; }

        public SpringParameters SlowSpatial { get; }

        public SpringParameters FastEffects { get; }

        public SpringParameters DefaultEffects { get; }

        public SpringParameters SlowEffects { get; }
    }
}
