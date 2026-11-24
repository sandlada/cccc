using System;

namespace Cccc.MaterialShape
{
    /// <summary>Stiffness / damping-ratio parameters of a spring (mass is always 1).</summary>
    public readonly record struct SpringParameters(double Stiffness, double DampingRatio);
}
