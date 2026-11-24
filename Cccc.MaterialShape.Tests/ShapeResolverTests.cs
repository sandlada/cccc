using System;
using System.Collections.Generic;
using Cccc.MaterialShape;
using Xunit;

namespace Cccc.MaterialShape.Tests
{
    public sealed class ShapeResolverTests
    {
        [Fact]
        public void Material_shape_resolves_to_the_cached_polygon()
        {
            RoundedPolygon polygon = ShapeResolver.Resolve(new ShapeResolutionParameters
            {
                Material = MaterialShapeKind.Heart,
            });

            Assert.Same(MaterialShapes.Get(MaterialShapeKind.Heart), polygon);
        }

        [Fact]
        public void Custom_polygon_is_normalized()
        {
            RoundedPolygon source = RoundedPolygonFactory.Circle(3, 2).Transformed(point => new ShapePoint(point.X + 4, point.Y - 1));
            RoundedPolygon polygon = ShapeResolver.Resolve(new ShapeResolutionParameters { Polygon = source });
            AssertNormalized(polygon);
        }

        [Fact]
        public void Custom_vertices_are_normalized()
        {
            RoundedPolygon polygon = ShapeResolver.Resolve(new ShapeResolutionParameters
            {
                Vertices = new[] { 0.0, 0.0, 2.0, 0.0, 2.0, 2.0, 0.0, 2.0 },
                CornerRadius = 0.2,
            });
            AssertNormalized(polygon);
        }

        [Fact]
        public void Parametric_shapes_are_normalized()
        {
            foreach (ParametricShape kind in (ParametricShape[])Enum.GetValues(typeof(ParametricShape)))
            {
                RoundedPolygon polygon = ShapeResolver.Resolve(new ShapeResolutionParameters
                {
                    Parametric = kind,
                    Sides = 6,
                    InnerRadius = 0.6,
                    CornerRadius = 0.2,
                    InnerCornerRadius = 0.05,
                    Smoothing = 0.1,
                    AspectRatio = 0.7,
                    VertexSpacing = 0.4,
                });
                AssertNormalized(polygon);
            }
        }

        [Fact]
        public void Invalid_vertices_throw()
        {
            Assert.Throws<ArgumentException>(() => ShapeResolver.Resolve(new ShapeResolutionParameters
            {
                Vertices = new[] { 0.0, 0.0, 1.0, 1.0 },
            }));
        }

        [Fact]
        public void Sides_are_clamped_to_at_least_three_for_polygons()
        {
            RoundedPolygon polygon = ShapeResolver.Resolve(new ShapeResolutionParameters
            {
                Parametric = ParametricShape.Polygon,
                Sides = 1,
            });
            Assert.True(polygon.Cubics.Length >= 3);
        }

        private static void AssertNormalized(RoundedPolygon polygon)
        {
            ShapeBounds bounds = polygon.CalculateBounds();
            Assert.Equal(1, Math.Max(bounds.Width, bounds.Height), 4);
            Assert.Equal(0.5, (bounds.Left + bounds.Right) / 2, 4);
            Assert.Equal(0.5, (bounds.Top + bounds.Bottom) / 2, 4);
            Assert.True(polygon.Cubics.Length > 0);
        }
    }
}
