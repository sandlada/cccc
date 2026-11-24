using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Cccc.MaterialShape;
using Xunit;

namespace Cccc.MaterialShape.Tests
{
    /// <summary>
    /// Numeric and topological conformance against the TypeScript reference
    /// implementation at commit 85398d36aaa9e0e48f589358735f27b741028813.
    /// </summary>
    public sealed class GoldenTests
    {
        private static readonly JsonDocument Golden = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Goldens", "golden.json")));

        private static readonly KeyValuePair<string, SpringParameters>[] SpringPresets =
        {
            Pair("expressive.FastSpatial", MotionScheme.Expressive.FastSpatial),
            Pair("expressive.DefaultSpatial", MotionScheme.Expressive.DefaultSpatial),
            Pair("expressive.SlowSpatial", MotionScheme.Expressive.SlowSpatial),
            Pair("expressive.FastEffects", MotionScheme.Expressive.FastEffects),
            Pair("expressive.DefaultEffects", MotionScheme.Expressive.DefaultEffects),
            Pair("expressive.SlowEffects", MotionScheme.Expressive.SlowEffects),
            Pair("standard.FastSpatial", MotionScheme.Standard.FastSpatial),
            Pair("standard.DefaultSpatial", MotionScheme.Standard.DefaultSpatial),
            Pair("standard.SlowSpatial", MotionScheme.Standard.SlowSpatial),
            Pair("standard.FastEffects", MotionScheme.Standard.FastEffects),
            Pair("standard.DefaultEffects", MotionScheme.Standard.DefaultEffects),
            Pair("standard.SlowEffects", MotionScheme.Standard.SlowEffects),
        };

        private static readonly (string First, string Second)[] SampledMorphs =
        {
            ("circle", "heart"),
            ("circle", "soft-burst"),
            ("heart", "cookie-4-sided"),
            ("circle", "square"),
            ("square", "cookie-12-sided"),
            ("pixel-circle", "sunny"),
            ("triangle", "clover-4-leaf"),
            ("pill", "square"),
        };

        private static readonly double[] SampleProgress = { 0, 0.1, 0.5, 0.9, 1, 1.05 };

        private static KeyValuePair<string, SpringParameters> Pair(string name, SpringParameters spring)
        {
            return new KeyValuePair<string, SpringParameters>(name, spring);
        }

        [Fact]
        public void MaterialShapes_match_type_script_golden()
        {
            JsonElement shapes = Golden.RootElement.GetProperty("shapes");
            foreach (MaterialShapeKind shape in MaterialShapes.All)
            {
                string slug = MaterialShapeNames.ToSlug(shape);
                JsonElement expected = shapes.GetProperty(slug);
                RoundedPolygon polygon = MaterialShapes.Get(shape);

                JsonElement cubics = expected.GetProperty("cubics");
                Assert.Equal(cubics.GetArrayLength(), polygon.Cubics.Length);
                for (int index = 0; index < polygon.Cubics.Length; index++)
                {
                    JsonElement values = cubics[index];
                    Cubic cubic = polygon.Cubics[index];
                    AssertClose(values[0].GetDouble(), cubic.Anchor0X, slug + " cubic " + index + " a0x");
                    AssertClose(values[1].GetDouble(), cubic.Anchor0Y, slug + " cubic " + index + " a0y");
                    AssertClose(values[2].GetDouble(), cubic.Control0X, slug + " cubic " + index + " c0x");
                    AssertClose(values[3].GetDouble(), cubic.Control0Y, slug + " cubic " + index + " c0y");
                    AssertClose(values[4].GetDouble(), cubic.Control1X, slug + " cubic " + index + " c1x");
                    AssertClose(values[5].GetDouble(), cubic.Control1Y, slug + " cubic " + index + " c1y");
                    AssertClose(values[6].GetDouble(), cubic.Anchor1X, slug + " cubic " + index + " a1x");
                    AssertClose(values[7].GetDouble(), cubic.Anchor1Y, slug + " cubic " + index + " a1y");
                }

                JsonElement bounds = expected.GetProperty("bounds");
                ShapeBounds actualBounds = polygon.CalculateBounds();
                AssertClose(bounds[0].GetDouble(), actualBounds.Left, slug + " bounds left");
                AssertClose(bounds[1].GetDouble(), actualBounds.Top, slug + " bounds top");
                AssertClose(bounds[2].GetDouble(), actualBounds.Right, slug + " bounds right");
                AssertClose(bounds[3].GetDouble(), actualBounds.Bottom, slug + " bounds bottom");

                JsonElement maxBounds = expected.GetProperty("maxBounds");
                ShapeBounds actualMaxBounds = polygon.CalculateMaxBounds();
                AssertClose(maxBounds[0].GetDouble(), actualMaxBounds.Left, slug + " max left");
                AssertClose(maxBounds[1].GetDouble(), actualMaxBounds.Top, slug + " max top");
                AssertClose(maxBounds[2].GetDouble(), actualMaxBounds.Right, slug + " max right");
                AssertClose(maxBounds[3].GetDouble(), actualMaxBounds.Bottom, slug + " max bottom");

                JsonElement center = expected.GetProperty("center");
                AssertClose(center[0].GetDouble(), polygon.Center.X, slug + " center x");
                AssertClose(center[1].GetDouble(), polygon.Center.Y, slug + " center y");
            }
        }

        [Fact]
        public void All_material_shapes_are_closed_and_normalized()
        {
            foreach (MaterialShapeKind shape in MaterialShapes.All)
            {
                string slug = MaterialShapeNames.ToSlug(shape);
                RoundedPolygon polygon = MaterialShapes.Get(shape);
                ShapeBounds bounds = polygon.CalculateBounds();
                AssertClose(1, Math.Max(bounds.Width, bounds.Height), 1e-4, slug + " normalized side");
                AssertClose(0.5, (bounds.Left + bounds.Right) / 2, 1e-4, slug + " centered x");
                AssertClose(0.5, (bounds.Top + bounds.Bottom) / 2, 1e-4, slug + " centered y");
                for (int i = 1; i < polygon.Cubics.Length; i++)
                {
                    AssertClose(polygon.Cubics[i - 1].Anchor1X, polygon.Cubics[i].Anchor0X, 1e-4, slug + " contiguous x");
                    AssertClose(polygon.Cubics[i - 1].Anchor1Y, polygon.Cubics[i].Anchor0Y, 1e-4, slug + " contiguous y");
                }
                Cubic last = polygon.Cubics[polygon.Cubics.Length - 1];
                Cubic first = polygon.Cubics[0];
                AssertClose(last.Anchor1X, first.Anchor0X, 1e-4, slug + " closed x");
                AssertClose(last.Anchor1Y, first.Anchor0Y, 1e-4, slug + " closed y");
            }
        }

        [Fact]
        public void All_morph_pairs_match_type_script_curve_counts()
        {
            JsonElement morphs = Golden.RootElement.GetProperty("morphs");
            foreach (MaterialShapeKind first in MaterialShapes.All)
            {
                foreach (MaterialShapeKind second in MaterialShapes.All)
                {
                    string key = MaterialShapeNames.ToSlug(first) + "->" + MaterialShapeNames.ToSlug(second);
                    int expected = morphs.GetProperty(key).GetProperty("curveCount").GetInt32();
                    Morph morph = new Morph(MaterialShapes.Get(first), MaterialShapes.Get(second));
                    Assert.True(
                        expected == morph.CurveCount,
                        key + ": expected " + expected + " curves, got " + morph.CurveCount);
                }
            }
        }

        [Fact]
        public void Morph_samples_match_type_script_golden()
        {
            JsonElement morphs = Golden.RootElement.GetProperty("morphs");
            foreach ((string first, string second) in SampledMorphs)
            {
                MaterialShapeNames.TryParse(first, out MaterialShapeKind firstShape);
                MaterialShapeNames.TryParse(second, out MaterialShapeKind secondShape);
                string key = first + "->" + second;
                JsonElement expected = morphs.GetProperty(key);
                Morph morph = new Morph(MaterialShapes.Get(firstShape), MaterialShapes.Get(secondShape));

                JsonElement center = expected.GetProperty("centerAtHalf");
                ShapePoint actualCenter = morph.CenterAt(0.5);
                AssertClose(center[0].GetDouble(), actualCenter.X, key + " center x");
                AssertClose(center[1].GetDouble(), actualCenter.Y, key + " center y");

                JsonElement bounds = expected.GetProperty("bounds");
                ShapeBounds actualBounds = morph.CalculateBounds();
                AssertClose(bounds[0].GetDouble(), actualBounds.Left, key + " bounds left");
                AssertClose(bounds[1].GetDouble(), actualBounds.Top, key + " bounds top");
                AssertClose(bounds[2].GetDouble(), actualBounds.Right, key + " bounds right");
                AssertClose(bounds[3].GetDouble(), actualBounds.Bottom, key + " bounds bottom");

                JsonElement samples = expected.GetProperty("samples");
                foreach (double progress in SampleProgress)
                {
                    JsonElement expectedCubics = samples.GetProperty(
                        progress.ToString(CultureInfo.InvariantCulture));
                    Cubic[] actualCubics = morph.AsCubics(progress);
                    Assert.Equal(expectedCubics.GetArrayLength(), actualCubics.Length);
                    for (int index = 0; index < actualCubics.Length; index++)
                    {
                        JsonElement values = expectedCubics[index];
                        Cubic cubic = actualCubics[index];
                        string label = key + " p=" + progress + " cubic " + index;
                        AssertClose(values[0].GetDouble(), cubic.Anchor0X, label + " a0x");
                        AssertClose(values[1].GetDouble(), cubic.Anchor0Y, label + " a0y");
                        AssertClose(values[2].GetDouble(), cubic.Control0X, label + " c0x");
                        AssertClose(values[3].GetDouble(), cubic.Control0Y, label + " c0y");
                        AssertClose(values[4].GetDouble(), cubic.Control1X, label + " c1x");
                        AssertClose(values[5].GetDouble(), cubic.Control1Y, label + " c1y");
                        AssertClose(values[6].GetDouble(), cubic.Anchor1X, label + " a1x");
                        AssertClose(values[7].GetDouble(), cubic.Anchor1Y, label + " a1y");
                    }
                }
            }
        }

        [Fact]
        public void Springs_match_type_script_golden()
        {
            JsonElement springs = Golden.RootElement.GetProperty("springs");
            double[] times = { 0, 16, 50, 100, 250, 500, 1000 };
            foreach ((string name, SpringParameters spring) in SpringPresets)
            {
                JsonElement expected = springs.GetProperty(name).GetProperty("values");
                Assert.Equal(times.Length, expected.GetArrayLength());
                for (int index = 0; index < times.Length; index++)
                {
                    SpringState state = Spring.MotionAt(spring, 0, 0, times[index], 1);
                    AssertClose(expected[index][0].GetDouble(), state.Value, name + " t=" + times[index] + " value");
                    AssertClose(expected[index][1].GetDouble(), state.Velocity, name + " t=" + times[index] + " velocity");
                }
            }

            JsonElement durations = springs.GetProperty("durations");
            AssertClose(
                durations.GetProperty("expressiveFast").GetDouble(),
                Spring.EstimateAnimationDurationMilliseconds(
                    MotionScheme.Expressive.FastSpatial, 0, -10, 1), 1.0, "expressiveFast duration");
            AssertClose(
                durations.GetProperty("expressiveDefault").GetDouble(),
                Spring.EstimateAnimationDurationMilliseconds(
                    MotionScheme.Expressive.DefaultSpatial, 0, -10, 1), 1.0, "expressiveDefault duration");
            AssertClose(
                durations.GetProperty("standardFast").GetDouble(),
                Spring.EstimateAnimationDurationMilliseconds(
                    MotionScheme.Standard.FastSpatial, 0, -10, 1), 1.0, "standardFast duration");
            AssertClose(
                durations.GetProperty("critical").GetDouble(),
                Spring.EstimateAnimationDurationMilliseconds(
                    new SpringParameters(1600, 1), 0, -10, 1), 1.0, "critical duration");
            AssertClose(
                durations.GetProperty("overDamped").GetDouble(),
                Spring.EstimateAnimationDurationMilliseconds(
                    new SpringParameters(200, 1.4), 0, -10, 1), 1.0, "overDamped duration");
        }

        [Fact]
        public void Morph_to_polygon_supports_mid_flight_interruption()
        {
            (string First, string Second)[] pairs =
            {
                ("circle", "heart"),
                ("pixel-circle", "sunny"),
                ("square", "cookie-12-sided"),
                ("triangle", "clover-4-leaf"),
            };
            foreach ((string first, string second) in pairs)
            {
                MaterialShapeNames.TryParse(first, out MaterialShapeKind firstShape);
                MaterialShapeNames.TryParse(second, out MaterialShapeKind secondShape);
                Morph morph = new Morph(MaterialShapes.Get(firstShape), MaterialShapes.Get(secondShape));
                foreach (double progress in new[] { 0.1, 0.25, 0.5, 0.75, 0.9, 1.05 })
                {
                    RoundedPolygon snapshot = morph.ToPolygon(progress);
                    Assert.True(snapshot.Cubics.Length > 0, first + "->" + second + " p=" + progress + " empty snapshot");
                    Morph next = new Morph(snapshot, MaterialShapes.Get(MaterialShapeKind.PuffyDiamond));
                    Cubic[] cubics = next.AsCubics(0.5);
                    Assert.True(cubics.Length > 0);
                    AssertClose(cubics[0].Anchor0X, cubics[cubics.Length - 1].Anchor1X, 1e-6, "interruption closure x");
                    AssertClose(cubics[0].Anchor0Y, cubics[cubics.Length - 1].Anchor1Y, 1e-6, "interruption closure y");
                }
            }
        }

        private static void AssertClose(double expected, double actual, string message, double tolerance = 1e-9)
        {
            AssertClose(expected, actual, tolerance, message);
        }

        private static void AssertClose(double expected, double actual, double tolerance, string message)
        {
            double delta = Math.Abs(expected - actual);
            Assert.True(delta <= tolerance, message + ": expected " + expected.ToString("R", CultureInfo.InvariantCulture) +
                ", actual " + actual.ToString("R", CultureInfo.InvariantCulture) +
                ", delta " + delta.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
