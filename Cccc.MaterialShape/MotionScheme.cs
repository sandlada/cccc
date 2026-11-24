using System;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Exact Material 3 <c>MotionScheme</c> spring presets from
    /// <c>ExpressiveMotionTokens</c> and <c>StandardMotionTokens</c>.
    /// </summary>
    public static class MotionScheme
    {
        public static MotionSchemeSet Expressive { get; } = new MotionSchemeSet(
            new SpringParameters(800, 0.6),
            new SpringParameters(380, 0.8),
            new SpringParameters(200, 0.8),
            new SpringParameters(3800, 1.0),
            new SpringParameters(1600, 1.0),
            new SpringParameters(800, 1.0));

        public static MotionSchemeSet Standard { get; } = new MotionSchemeSet(
            new SpringParameters(1400, 0.9),
            new SpringParameters(700, 0.9),
            new SpringParameters(300, 0.9),
            new SpringParameters(3800, 1.0),
            new SpringParameters(1600, 1.0),
            new SpringParameters(800, 1.0));

        public static SpringParameters Resolve(MotionSchemeKind scheme, MotionSpeed speed)
        {
            MotionSchemeSet group = scheme == MotionSchemeKind.Standard ? Standard : Expressive;
            switch (speed)
            {
                case MotionSpeed.Fast: return group.FastSpatial;
                case MotionSpeed.Slow: return group.SlowSpatial;
                default: return group.DefaultSpatial;
            }
        }
    }
}
