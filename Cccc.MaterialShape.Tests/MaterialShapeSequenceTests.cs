using Cccc.MaterialShape;
using Xunit;

namespace Cccc.MaterialShape.Tests
{
    public sealed class MaterialShapeSequenceTests
    {
        [Fact]
        public void Default_sequence_matches_the_reference_web_component()
        {
            Assert.Equal(8, MaterialShapeSequence.Default.Count);
            Assert.Equal(MaterialShapeKind.Circle, MaterialShapeSequence.Default[0]);
            Assert.Equal(MaterialShapeKind.SoftBurst, MaterialShapeSequence.Default[1]);
            Assert.Equal(MaterialShapeKind.Cookie9Sided, MaterialShapeSequence.Default[2]);
            Assert.Equal(MaterialShapeKind.Pentagon, MaterialShapeSequence.Default[3]);
            Assert.Equal(MaterialShapeKind.Pill, MaterialShapeSequence.Default[4]);
            Assert.Equal(MaterialShapeKind.Sunny, MaterialShapeSequence.Default[5]);
            Assert.Equal(MaterialShapeKind.Cookie4Sided, MaterialShapeSequence.Default[6]);
            Assert.Equal(MaterialShapeKind.Heart, MaterialShapeSequence.Default[7]);
        }

        [Fact]
        public void Advance_cycles_shapes_and_rotation_delta()
        {
            MaterialShapeSequence sequence = new MaterialShapeSequence();
            Assert.Equal(MaterialShapeKind.Circle, sequence.Current);

            RoundedPolygon first = sequence.Advance(out double rotation);
            Assert.Equal(MaterialShapeKind.SoftBurst, sequence.Current);
            Assert.Equal(45, rotation);
            Assert.Same(MaterialShapes.Get(MaterialShapeKind.SoftBurst), first);

            for (int i = 0; i < 6; i++) sequence.Advance(out _);
            Assert.Equal(MaterialShapeKind.Heart, sequence.Current);

            sequence.Advance(out rotation);
            Assert.Equal(MaterialShapeKind.Circle, sequence.Current);
            Assert.Equal(45, rotation);

            sequence.Reset();
            Assert.Equal(MaterialShapeKind.Circle, sequence.Current);
        }

        [Fact]
        public void Custom_sequence_is_respected()
        {
            MaterialShapeSequence sequence = new MaterialShapeSequence(
                new[] { MaterialShapeKind.Square, MaterialShapeKind.Heart },
                0);

            Assert.Equal(MaterialShapeKind.Square, sequence.Current);
            sequence.Advance(out double rotation);
            Assert.Equal(MaterialShapeKind.Heart, sequence.Current);
            Assert.Equal(0, rotation);
        }
    }
}
