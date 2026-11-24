using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>A cubic annotated with its outline progress range and measured size.</summary>
    public sealed class MeasuredCubic
    {
        private readonly IMeasurer measurer;

        public MeasuredCubic(Cubic cubic, double startOutlineProgress, double endOutlineProgress, IMeasurer measurer)
        {
            if (endOutlineProgress < startOutlineProgress)
                throw new ArgumentException("endOutlineProgress is expected to be equal or greater than startOutlineProgress");
            Cubic = cubic;
            StartOutlineProgress = startOutlineProgress;
            EndOutlineProgress = endOutlineProgress;
            this.measurer = measurer;
            MeasuredSize = measurer.MeasureCubic(cubic);
        }

        public Cubic Cubic { get; }

        public double MeasuredSize { get; }

        public double StartOutlineProgress { get; private set; }

        public double EndOutlineProgress { get; private set; }

        public void UpdateProgressRange(double? startOutlineProgress = null, double? endOutlineProgress = null)
        {
            double start = startOutlineProgress ?? StartOutlineProgress;
            double end = endOutlineProgress ?? EndOutlineProgress;
            if (end < start)
                throw new ArgumentException("endOutlineProgress is expected to be equal or greater than startOutlineProgress");
            StartOutlineProgress = start;
            EndOutlineProgress = end;
        }

        public (MeasuredCubic First, MeasuredCubic Second) CutAtProgress(double cutOutlineProgress)
        {
            double boundedCut = Math.Min(EndOutlineProgress, Math.Max(StartOutlineProgress, cutOutlineProgress));
            double outlineProgressSize = EndOutlineProgress - StartOutlineProgress;
            double progressFromStart = boundedCut - StartOutlineProgress;
            double relativeProgress = progressFromStart / outlineProgressSize;
            double t = measurer.FindCubicCutPoint(Cubic, relativeProgress * MeasuredSize);
            if (!(t >= 0 && t <= 1)) throw new InvalidOperationException("Cubic cut point is expected to be between 0 and 1");
            (Cubic first, Cubic second) = Cubic.Split(t);
            return (
                new MeasuredCubic(first, StartOutlineProgress, boundedCut, measurer),
                new MeasuredCubic(second, boundedCut, EndOutlineProgress, measurer));
        }
    }
}
