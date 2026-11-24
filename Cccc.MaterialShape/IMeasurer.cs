using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Measures a cubic and finds the parameter at which a given measure is
    /// reached. Ported from <c>androidx.graphics.shapes.PolygonMeasure</c>.
    /// </summary>
    public interface IMeasurer
    {
        double MeasureCubic(Cubic cubic);

        double FindCubicCutPoint(Cubic cubic, double measure);
    }
}
