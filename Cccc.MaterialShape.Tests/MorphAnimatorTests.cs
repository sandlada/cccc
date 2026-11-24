using System;
using System.Collections.Generic;
using System.Globalization;
using Cccc.MaterialShape;
using Xunit;

namespace Cccc.MaterialShape.Tests
{
    public sealed class MorphAnimatorTests
    {
        private static readonly SpringParameters SpringParameters = MotionScheme.Expressive.DefaultSpatial;
        private static readonly double[] ConvergenceTimes = { 0, 16, 50, 100, 250, 500, 1000 };

        private const int OutlineFlattenStepsPerCubic = 512;
        private const int OutlineComparisonSamples = 256;

        [Fact]
        public void Morph_settles_exactly_on_target()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);

            Assert.Same(circle, animator.SettledShape);
            Assert.Same(circle, animator.CurrentTarget);
            Assert.False(animator.IsRunning);
            AssertCubicsClose(circle.Cubics, SampleCubics(animator), 1e-9, "rest sample");

            animator.Trigger(heart, 0, 0);
            Assert.True(animator.IsRunning);
            Assert.Same(heart, animator.CurrentTarget);

            Assert.True(animator.Advance(0));
            Assert.True(animator.Advance(16));
            Assert.True(animator.Advance(50));
            Assert.True(animator.Advance(100));
            Assert.True(animator.Advance(250));
            Assert.False(animator.Advance(500));
            Assert.False(animator.Advance(1000));

            Assert.False(animator.IsRunning);
            Assert.Same(heart, animator.SettledShape);
            Assert.Same(heart, animator.CurrentTarget);
            AssertCubicsClose(heart.Cubics, SampleCubics(animator), 1e-9, "settled sample");
        }

        [Fact]
        public void Reverse_trigger_is_continuous_and_returns_to_start()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);

            animator.Trigger(heart, 0, 0);
            animator.Advance(0);
            animator.Advance(16);
            animator.Advance(50);
            Assert.True(animator.IsRunning);

            Cubic[] before = SampleCubics(animator);
            animator.Trigger(circle, 0, 50);
            Cubic[] after = SampleCubics(animator);

            Assert.True(animator.IsRunning);
            Assert.Same(circle, animator.CurrentTarget);
            AssertCubicsClose(before, after, 1e-6, "reverse switch");

            foreach (double time in new double[] { 100, 250, 500, 1000, 2000 })
                animator.Advance(time);

            Assert.False(animator.IsRunning);
            Assert.Same(circle, animator.SettledShape);
            Assert.Same(circle, animator.CurrentTarget);
            AssertCubicsClose(circle.Cubics, SampleCubics(animator), 1e-9, "reverse settled sample");
        }

        [Fact]
        public void Third_shape_trigger_does_not_jump()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            RoundedPolygon puffyDiamond = MaterialShapes.Get(MaterialShapeKind.PuffyDiamond);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);

            animator.Trigger(heart, 0, 0);
            animator.Advance(0);
            animator.Advance(16);
            animator.Advance(50);
            Assert.True(animator.IsRunning);

            Cubic[] before = SampleCubics(animator);
            animator.Trigger(puffyDiamond, 0, 50);
            Cubic[] after = SampleCubics(animator);

            Assert.True(animator.IsRunning);
            Assert.Same(puffyDiamond, animator.CurrentTarget);
            AssertSameOutline(before, after, 1e-6, "third-shape switch");

            foreach (double time in new double[] { 66, 100, 250, 500, 1000, 2000 })
                animator.Advance(time);

            Assert.False(animator.IsRunning);
            Assert.Same(puffyDiamond, animator.SettledShape);
            Assert.Same(puffyDiamond, animator.CurrentTarget);
            AssertCubicsClose(puffyDiamond.Cubics, SampleCubics(animator), 1e-9, "third-shape settled sample");
        }

        [Fact]
        public void Rotation_accumulates_and_settles_then_snap_resets()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            RoundedPolygon puffyDiamond = MaterialShapes.Get(MaterialShapeKind.PuffyDiamond);
            RoundedPolygon softBurst = MaterialShapes.Get(MaterialShapeKind.SoftBurst);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters, 10);

            AssertClose(10, animator.RotationDegrees, 0, "initial rotation");

            animator.Trigger(heart, 45, 0);
            Assert.True(animator.IsRunning);
            animator.Advance(0);
            animator.Advance(50);
            double afterFirstTrigger = animator.RotationDegrees;
            Assert.True(
                afterFirstTrigger > 10,
                "rotation should move towards its target, got " + afterFirstTrigger.ToString("R", CultureInfo.InvariantCulture));

            animator.Trigger(puffyDiamond, 45, 50);
            Assert.True(
                Math.Abs(animator.RotationDegrees - afterFirstTrigger) <= 1e-9,
                "retargeting must not snap the current rotation");

            foreach (double time in new double[] { 66, 100, 250, 500, 1000, 2000 })
                animator.Advance(time);

            Assert.False(animator.IsRunning);
            AssertClose(100, animator.RotationDegrees, 1e-9, "accumulated rotation");

            animator.SnapTo(softBurst, 30);
            Assert.False(animator.IsRunning);
            AssertClose(30, animator.RotationDegrees, 0, "snapped rotation");
            Assert.Same(softBurst, animator.SettledShape);
            Assert.Same(softBurst, animator.CurrentTarget);
            AssertCubicsClose(softBurst.Cubics, SampleCubics(animator), 1e-9, "snap sample");

            Assert.False(animator.Advance(5000));
            AssertClose(30, animator.RotationDegrees, 0, "snapped rotation after advance");
        }

        [Fact]
        public void Trigger_on_settled_shape_only_rotates()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);

            animator.Trigger(circle, 90, 0);
            Assert.True(animator.IsRunning);
            Assert.Same(circle, animator.CurrentTarget);
            Assert.Same(circle, animator.SettledShape);

            animator.Advance(0);
            animator.Advance(16);
            animator.Advance(50);
            animator.Advance(250);
            animator.Advance(1000);

            Assert.False(animator.IsRunning);
            Assert.Same(circle, animator.SettledShape);
            AssertClose(90, animator.RotationDegrees, 1e-9, "rotation-only trigger");
            AssertCubicsClose(circle.Cubics, SampleCubics(animator), 1e-9, "rotation-only sample");
        }

        [Fact]
        public void Sample_throws_when_destination_is_too_small()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);

            Assert.Throws<ArgumentException>(() => animator.Sample(new Cubic[1]));

            animator.Trigger(heart, 0, 0);
            animator.Advance(0);
            Assert.Throws<ArgumentException>(() => animator.Sample(new Cubic[1]));
        }

        [Fact]
        public void Sample_is_allocation_free()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);
            animator.Trigger(heart, 0, 0);
            animator.Advance(0);
            animator.Advance(50);

            Cubic[] destination = new Cubic[256];
            for (int i = 0; i < 1000; i++) animator.Sample(destination);

            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100; i++) animator.Sample(destination);
            long after = GC.GetAllocatedBytesForCurrentThread();
            Assert.Equal(0L, after - before);

            animator.Advance(2000);
            Assert.False(animator.IsRunning);
            for (int i = 0; i < 1000; i++) animator.Sample(destination);

            before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100; i++) animator.Sample(destination);
            after = GC.GetAllocatedBytesForCurrentThread();
            Assert.Equal(0L, after - before);
        }

        [Fact]
        public void Reverse_then_forward_continues_the_same_morph()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters);

            animator.Trigger(heart, 0, 0);
            animator.Advance(0);
            animator.Advance(16);
            animator.Advance(50);
            animator.Trigger(circle, 0, 50);
            animator.Advance(66);
            animator.Advance(100);
            Assert.True(animator.IsRunning);

            Cubic[] before = SampleCubics(animator);
            animator.Trigger(heart, 0, 100);
            Cubic[] after = SampleCubics(animator);
            AssertCubicsClose(before, after, 1e-6, "forward switch");
            Assert.Same(heart, animator.CurrentTarget);

            foreach (double time in new double[] { 116, 250, 500, 1000, 2000 })
                animator.Advance(time);

            Assert.False(animator.IsRunning);
            Assert.Same(heart, animator.SettledShape);
            Assert.Same(heart, animator.CurrentTarget);
            AssertCubicsClose(heart.Cubics, SampleCubics(animator), 1e-9, "forward settled sample");
        }

        [Fact]
        public void Identical_time_sequences_produce_identical_frames()
        {
            List<(double Rotation, Cubic[] Cubics)> first = RunScenario();
            List<(double Rotation, Cubic[] Cubics)> second = RunScenario();

            Assert.Equal(first.Count, second.Count);
            for (int frame = 0; frame < first.Count; frame++)
            {
                Assert.Equal(first[frame].Rotation, second[frame].Rotation);
                AssertCubicsClose(first[frame].Cubics, second[frame].Cubics, 0, "frame " + frame);
            }
        }

        private static List<(double Rotation, Cubic[] Cubics)> RunScenario()
        {
            RoundedPolygon circle = MaterialShapes.Get(MaterialShapeKind.Circle);
            RoundedPolygon heart = MaterialShapes.Get(MaterialShapeKind.Heart);
            RoundedPolygon puffyDiamond = MaterialShapes.Get(MaterialShapeKind.PuffyDiamond);
            MorphAnimator animator = new MorphAnimator(circle, SpringParameters, 15);
            List<(double Rotation, Cubic[] Cubics)> frames = new List<(double Rotation, Cubic[] Cubics)>();

            void Capture(double nowMs)
            {
                animator.Advance(nowMs);
                frames.Add((animator.RotationDegrees, SampleCubics(animator)));
            }

            animator.Trigger(heart, 30, 0);
            Capture(0);
            Capture(16);
            Capture(50);
            animator.Trigger(puffyDiamond, 45, 50);
            Capture(66);
            Capture(100);
            Capture(250);
            animator.Trigger(circle, -20, 250);
            Capture(300);
            Capture(500);
            Capture(1000);
            return frames;
        }

        private static Cubic[] SampleCubics(MorphAnimator animator)
        {
            Cubic[] buffer = new Cubic[256];
            int count = animator.Sample(buffer);
            Cubic[] result = new Cubic[count];
            Array.Copy(buffer, result, count);
            return result;
        }

        private static void AssertCubicsClose(Cubic[] expected, Cubic[] actual, double tolerance, string context)
        {
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Cubic first = expected[i];
                Cubic second = actual[i];
                AssertClose(first.Anchor0X, second.Anchor0X, tolerance, context + " cubic " + i + " anchor0x");
                AssertClose(first.Anchor0Y, second.Anchor0Y, tolerance, context + " cubic " + i + " anchor0y");
                AssertClose(first.Control0X, second.Control0X, tolerance, context + " cubic " + i + " control0x");
                AssertClose(first.Control0Y, second.Control0Y, tolerance, context + " cubic " + i + " control0y");
                AssertClose(first.Control1X, second.Control1X, tolerance, context + " cubic " + i + " control1x");
                AssertClose(first.Control1Y, second.Control1Y, tolerance, context + " cubic " + i + " control1y");
                AssertClose(first.Anchor1X, second.Anchor1X, tolerance, context + " cubic " + i + " anchor1x");
                AssertClose(first.Anchor1Y, second.Anchor1Y, tolerance, context + " cubic " + i + " anchor1y");
            }
        }

        /// <summary>
        /// Compares two outlines as closed curves rather than as cubic arrays:
        /// an interrupted morph snapshots the live shape through
        /// <see cref="Morph.ToPolygon"/>, which re-segments the same geometry
        /// into a different number of cubics.
        /// </summary>
        private static void AssertSameOutline(Cubic[] expected, Cubic[] actual, double tolerance, string context)
        {
            ShapePoint[] first = ResampleByPerimeter(expected);
            ShapePoint[] second = ResampleByPerimeter(actual);

            double best = double.MaxValue;
            for (int offset = 0; offset < OutlineComparisonSamples; offset++)
            {
                double worst = 0;
                for (int sample = 0; sample < OutlineComparisonSamples; sample++)
                {
                    ShapePoint a = first[sample];
                    ShapePoint b = second[(sample + offset) % OutlineComparisonSamples];
                    double delta = GeometryMathematics.Hypotenuse(a.X - b.X, a.Y - b.Y);
                    if (delta > worst) worst = delta;
                    if (worst >= best) break;
                }
                if (worst < best) best = worst;
            }

            Assert.True(
                best <= tolerance,
                context + ": outlines differ by " + best.ToString("R", CultureInfo.InvariantCulture));
        }

        private static ShapePoint[] ResampleByPerimeter(Cubic[] cubics)
        {
            int denseCount = cubics.Length * OutlineFlattenStepsPerCubic + 1;
            ShapePoint[] dense = new ShapePoint[denseCount];
            int index = 0;
            foreach (Cubic cubic in cubics)
            {
                for (int step = 0; step < OutlineFlattenStepsPerCubic; step++)
                    dense[index++] = cubic.PointOnCurve((double)step / OutlineFlattenStepsPerCubic);
            }
            dense[index] = cubics[cubics.Length - 1].Anchor1;

            double[] cumulative = new double[denseCount];
            double total = 0;
            for (int i = 1; i < denseCount; i++)
            {
                total += GeometryMathematics.Hypotenuse(dense[i].X - dense[i - 1].X, dense[i].Y - dense[i - 1].Y);
                cumulative[i] = total;
            }

            ShapePoint[] result = new ShapePoint[OutlineComparisonSamples];
            int segment = 1;
            for (int sample = 0; sample < OutlineComparisonSamples; sample++)
            {
                double target = total * sample / OutlineComparisonSamples;
                while (segment < denseCount - 1 && cumulative[segment] < target) segment++;
                double span = cumulative[segment] - cumulative[segment - 1];
                double progress = span > 0 ? (target - cumulative[segment - 1]) / span : 0;
                result[sample] = new ShapePoint(
                    dense[segment - 1].X + (dense[segment].X - dense[segment - 1].X) * progress,
                    dense[segment - 1].Y + (dense[segment].Y - dense[segment - 1].Y) * progress);
            }
            return result;
        }

        private static void AssertClose(double expected, double actual, double tolerance, string message)
        {
            double delta = Math.Abs(expected - actual);
            Assert.True(
                delta <= tolerance,
                message + ": expected " + expected.ToString("R", CultureInfo.InvariantCulture) +
                ", actual " + actual.ToString("R", CultureInfo.InvariantCulture) +
                ", delta " + delta.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
