using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>A feature together with the outline progress at which it sits.</summary>
    public sealed class FeatureAtProgress
    {
        public FeatureAtProgress(double progress, Feature feature)
        {
            Progress = progress;
            Feature = feature;
        }

        public double Progress { get; }

        public Feature Feature { get; }
    }
}
