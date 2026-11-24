using System;

namespace Cccc.MaterialShape
{
    /// <summary>A spring's value and velocity at a point in time.</summary>
    public readonly record struct SpringState(double Value, double Velocity);
}
