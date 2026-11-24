using System;
using Cccc.MaterialShape;
using Xunit;

namespace Cccc.MaterialShape.Tests
{
    public sealed class RapidMorphTests
    {
        [Fact]
        public void Rapid_retargeting_every_100ms_follows_the_default_sequence()
        {
            RunRapidRetargeting(100.0);
        }

        [Fact]
        public void Rapid_retargeting_every_83ms_follows_the_default_sequence()
        {
            RunRapidRetargeting(83.0);
        }

        private static void RunRapidRetargeting(double intervalMs)
        {
            MorphAnimator animator = new MorphAnimator(
                MaterialShapes.Get(MaterialShapeKind.Circle),
                MotionScheme.Expressive.FastSpatial,
                225);
            MaterialShapeSequence sequence = new MaterialShapeSequence();
            Cubic[] buffer = new Cubic[1024];
            double now = 0;

            for (int copy = 1; copy <= 80; copy++)
            {
                RoundedPolygon target = sequence.Advance(out double rotation);
                animator.Trigger(target, rotation, now);
                for (int frame = 0; frame < 6; frame++)
                {
                    animator.Advance(now);
                    int count = animator.Sample(buffer);
                    Assert.True(count > 0, "empty frame at copy " + copy + " interval " + intervalMs);
                    now += 16.666;
                }
                now = copy * intervalMs;
            }

            for (int frame = 0; frame < 120; frame++)
            {
                animator.Advance(now);
                animator.Sample(buffer);
                now += 16.666;
            }
            Assert.False(animator.IsRunning, "animation did not settle at interval " + intervalMs);
        }
    }
}
