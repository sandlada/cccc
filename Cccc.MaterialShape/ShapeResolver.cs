using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>Parametric shape builders exposed by the reference resolver.</summary>
    public enum ParametricShape
    {
        Polygon,
        Star,
        Rectangle,
        PillStar,
    }

    /// <summary>
    /// Inputs for <see cref="ShapeResolver.Resolve"/>. A custom polygon takes
    /// precedence over custom vertices, which take precedence over a material
    /// shape, which takes precedence over a parametric shape.
    /// </summary>
    public sealed class ShapeResolutionParameters
    {
        public MaterialShapeKind? Material { get; set; }

        public ParametricShape? Parametric { get; set; }

        public RoundedPolygon Polygon { get; set; }

        public IReadOnlyList<double> Vertices { get; set; }

        public int Sides { get; set; } = 5;

        public double InnerRadius { get; set; } = 0.5;

        public double CornerRadius { get; set; } = 0.15;

        public double? InnerCornerRadius { get; set; }

        public double Smoothing { get; set; }

        public double AspectRatio { get; set; } = 1;

        public double VertexSpacing { get; set; } = 0.5;
    }

    /// <summary>
    /// Resolves named Material shapes, custom polygons / vertices and the
    /// <c>RoundedPolygonFactory.kt</c> parametric builders into normalized polygons. Ported from
    /// the reference <c>shape-resolver.ts</c>.
    /// </summary>
    public static class ShapeResolver
    {
        public static RoundedPolygon Resolve(ShapeResolutionParameters parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (parameters.Polygon != null) return parameters.Polygon.Normalized();

            CornerRounding rounding = new CornerRounding(
                ClampPositive(parameters.CornerRadius, 0, 0),
                ClampUnitRange(parameters.Smoothing, 0));

            if (parameters.Vertices != null)
            {
                double[] vertices = new double[parameters.Vertices.Count];
                for (int i = 0; i < vertices.Length; i++) vertices[i] = parameters.Vertices[i];
                if (vertices.Length < 6 || vertices.Length % 2 != 0)
                    throw new ArgumentException("Custom vertices must contain an even number of at least 6 numbers.");
                return RoundedPolygonFactory.FromVertices(vertices, rounding).Normalized();
            }

            if (parameters.Material.HasValue) return MaterialShapes.Get(parameters.Material.Value);

            int sides = Math.Max(2, (int)GeometryMathematics.RoundLikeJavaScript(ClampPositive(parameters.Sides, 2, 5)));
            double innerRadius = ClampInnerRadius(parameters.InnerRadius);
            CornerRounding? innerRounding = parameters.InnerCornerRadius.HasValue
                ? new CornerRounding(
                    ClampPositive(parameters.InnerCornerRadius.Value, 0, 0),
                    ClampUnitRange(parameters.Smoothing, 0))
                : (CornerRounding?)null;
            double aspectRatio = ClampPositive(parameters.AspectRatio, 0.05, 1);
            double vertexSpacing = ClampUnitRange(parameters.VertexSpacing, 0.5);

            switch (parameters.Parametric)
            {
                case ParametricShape.Polygon:
                    return RoundedPolygonFactory.FromVertexCount(Math.Max(3, sides), 1, 0, 0, rounding).Normalized();
                case ParametricShape.Star:
                    return RoundedPolygonFactory.Star(sides, 1, innerRadius, rounding, innerRounding).Normalized();
                case ParametricShape.Rectangle:
                    return RoundedPolygonFactory.Rectangle(aspectRatio, 1, rounding).Normalized();
                case ParametricShape.PillStar:
                    return RoundedPolygonFactory.PillStar(
                        aspectRatio,
                        1,
                        sides,
                        innerRadius,
                        rounding,
                        innerRounding,
                        null,
                        vertexSpacing).Normalized();
                default:
                    throw new ArgumentException("Unknown shape kind.", nameof(parameters));
            }
        }

        private static double ClampPositive(double value, double minimum, double fallback)
        {
            return double.IsFinite(value) && value >= minimum ? value : fallback;
        }

        private static double ClampUnitRange(double value, double fallback)
        {
            return double.IsFinite(value) ? Math.Min(1, Math.Max(0, value)) : fallback;
        }

        private static double ClampInnerRadius(double value)
        {
            if (!double.IsFinite(value)) return 0.5;
            return Math.Min(0.99, Math.Max(0.01, value));
        }
    }
}
