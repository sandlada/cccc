using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// A polygon whose cubics are annotated with their measured size and outline
    /// progress, with feature representatives kept alongside.
    /// </summary>
    public sealed class MeasuredPolygon
    {
        private readonly IMeasurer measurer;
        private readonly MeasuredCubic[] measuredCubics;

        private MeasuredPolygon(
            IMeasurer measurer,
            FeatureAtProgress[] features,
            Cubic[] cubics,
            double[] outlineProgress)
        {
            if (outlineProgress.Length != cubics.Length + 1)
                throw new ArgumentException("Outline progress size is expected to be the cubics size + 1");
            if (outlineProgress[0] != 0) throw new ArgumentException("First outline progress value is expected to be zero");
            if (outlineProgress[outlineProgress.Length - 1] != 1)
                throw new ArgumentException("Last outline progress value is expected to be one");
            this.measurer = measurer;
            Features = features;

            List<MeasuredCubic> result = new List<MeasuredCubic>();
            double startOutlineProgress = 0;
            for (int index = 0; index < cubics.Length; index++)
            {
                if (outlineProgress[index + 1] - outlineProgress[index] > GeometryMathematics.DistanceEpsilon)
                {
                    result.Add(new MeasuredCubic(cubics[index], startOutlineProgress, outlineProgress[index + 1], measurer));
                    startOutlineProgress = outlineProgress[index + 1];
                }
            }
            MeasuredCubic last = result[result.Count - 1];
            last.UpdateProgressRange(last.StartOutlineProgress, 1);
            measuredCubics = result.ToArray();
        }

        public FeatureAtProgress[] Features { get; }

        public int Size { get { return measuredCubics.Length; } }

        public MeasuredCubic this[int index] { get { return measuredCubics[index]; } }

        public MeasuredCubic GetOrNull(int index)
        {
            return index >= 0 && index < measuredCubics.Length ? measuredCubics[index] : null;
        }

        public MeasuredPolygon CutAndShift(double cuttingPoint)
        {
            if (!(cuttingPoint >= 0 && cuttingPoint <= 1))
                throw new ArgumentException("Cutting point is expected to be between 0 and 1");
            if (cuttingPoint < GeometryMathematics.DistanceEpsilon) return this;

            int targetIndex = -1;
            for (int i = 0; i < measuredCubics.Length; i++)
            {
                if (cuttingPoint >= measuredCubics[i].StartOutlineProgress &&
                    cuttingPoint <= measuredCubics[i].EndOutlineProgress)
                {
                    targetIndex = i;
                    break;
                }
            }
            if (targetIndex < 0) throw new InvalidOperationException("Cutting point was not found on the outline.");
            MeasuredCubic target = measuredCubics[targetIndex];
            (MeasuredCubic beforeCut, MeasuredCubic afterCut) = target.CutAtProgress(cuttingPoint);

            Cubic[] retCubics = new Cubic[measuredCubics.Length + 1];
            retCubics[0] = afterCut.Cubic;
            for (int i = 1; i < measuredCubics.Length; i++)
            {
                retCubics[i] = measuredCubics[(i + targetIndex) % measuredCubics.Length].Cubic;
            }
            retCubics[retCubics.Length - 1] = beforeCut.Cubic;

            double[] retOutlineProgress = new double[measuredCubics.Length + 2];
            for (int index = 0; index < retOutlineProgress.Length; index++)
            {
                if (index == 0)
                {
                    retOutlineProgress[index] = 0;
                }
                else if (index == measuredCubics.Length + 1)
                {
                    retOutlineProgress[index] = 1;
                }
                else
                {
                    int cubicIndex = (targetIndex + index - 1) % measuredCubics.Length;
                    retOutlineProgress[index] = GeometryMathematics.PositiveModulo(
                        measuredCubics[cubicIndex].EndOutlineProgress - cuttingPoint,
                        1);
                }
            }

            FeatureAtProgress[] shiftedFeatures = new FeatureAtProgress[Features.Length];
            for (int i = 0; i < Features.Length; i++)
            {
                shiftedFeatures[i] = new FeatureAtProgress(
                    GeometryMathematics.PositiveModulo(Features[i].Progress - cuttingPoint, 1),
                    Features[i].Feature);
            }

            return new MeasuredPolygon(measurer, shiftedFeatures, retCubics, retOutlineProgress);
        }

        public static MeasuredPolygon Measure(IMeasurer measurer, RoundedPolygon polygon)
        {
            List<Cubic> cubics = new List<Cubic>();
            List<(Feature Feature, int Index)> featureToCubic = new List<(Feature, int)>();
            foreach (Feature feature in polygon.Features)
            {
                for (int cubicIndex = 0; cubicIndex < feature.Cubics.Count; cubicIndex++)
                {
                    if (feature is FeatureCorner && cubicIndex == (int)Math.Floor(feature.Cubics.Count / 2.0))
                    {
                        featureToCubic.Add((feature, cubics.Count));
                    }
                    cubics.Add(feature.Cubics[cubicIndex]);
                }
            }

            double[] measures = new double[cubics.Count + 1];
            double totalMeasure = 0;
            for (int i = 0; i < cubics.Count; i++)
            {
                double measured = measurer.MeasureCubic(cubics[i]);
                if (!(measured >= 0)) throw new InvalidOperationException("Measured cubic is expected to be greater or equal to zero");
                totalMeasure += measured;
                measures[i + 1] = totalMeasure;
            }

            double[] outlineProgress = new double[measures.Length];
            for (int i = 0; i < measures.Length; i++) outlineProgress[i] = measures[i] / totalMeasure;

            FeatureAtProgress[] features = new FeatureAtProgress[featureToCubic.Count];
            for (int i = 0; i < features.Length; i++)
            {
                (Feature feature, int index) = featureToCubic[i];
                features[i] = new FeatureAtProgress(
                    GeometryMathematics.PositiveModulo((outlineProgress[index] + outlineProgress[index + 1]) / 2, 1),
                    feature);
            }

            return new MeasuredPolygon(measurer, features, cubics.ToArray(), outlineProgress);
        }
    }
}
