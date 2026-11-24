using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Cached, normalized <see cref="RoundedPolygon"/> instances for every
    /// Material Design 3 Expressive shape.
    /// </summary>
    public static class MaterialShapes
    {
        private static readonly MaterialShapeKind[] ShapesInOrder = (MaterialShapeKind[])Enum.GetValues(typeof(MaterialShapeKind));
        private static readonly Lazy<RoundedPolygon>[] Cache = CreateCache();

        public static IReadOnlyList<MaterialShapeKind> All { get { return ShapesInOrder; } }

        public static RoundedPolygon Get(MaterialShapeKind shape)
        {
            int index = (int)shape;
            if (index < 0 || index >= Cache.Length) throw new ArgumentOutOfRangeException(nameof(shape));
            return Cache[index].Value;
        }

        public static RoundedPolygon Circle { get { return Get(MaterialShapeKind.Circle); } }

        public static RoundedPolygon Heart { get { return Get(MaterialShapeKind.Heart); } }

        private static Lazy<RoundedPolygon>[] CreateCache()
        {
            Lazy<RoundedPolygon>[] cache = new Lazy<RoundedPolygon>[ShapesInOrder.Length];
            foreach (MaterialShapeKind shape in ShapesInOrder)
            {
                MaterialShapeKind captured = shape;
                cache[(int)shape] = new Lazy<RoundedPolygon>(() => Create(captured).Normalized());
            }
            return cache;
        }

        private static readonly ShapePoint ShapeCenter = new ShapePoint(0.5, 0.5);
        private static readonly CornerRounding CornerRound15 = new CornerRounding(0.15);
        private static readonly CornerRounding CornerRound20 = new CornerRounding(0.2);
        private static readonly CornerRounding CornerRound30 = new CornerRounding(0.3);
        private static readonly CornerRounding CornerRound50 = new CornerRounding(0.5);
        private static readonly CornerRounding CornerRound100 = new CornerRounding(1);

        private static readonly Func<ShapePoint, ShapePoint> RotateNeg45 =
            point => GeometryMathematics.RotateDegrees(point, -45, new ShapePoint(0, 0));
        private static readonly Func<ShapePoint, ShapePoint> RotateNeg90 =
            point => GeometryMathematics.RotateDegrees(point, -90, new ShapePoint(0, 0));
        private static readonly Func<ShapePoint, ShapePoint> RotateNeg135 =
            point => GeometryMathematics.RotateDegrees(point, -135, new ShapePoint(0, 0));

        private static RoundedPolygon Create(MaterialShapeKind shape)
        {
            switch (shape)
            {
                case MaterialShapeKind.Circle:
                    return RoundedPolygonFactory.Circle(10);
                case MaterialShapeKind.Square:
                    return RoundedPolygonFactory.Rectangle(1, 1, CornerRound30);
                case MaterialShapeKind.Slanted:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.926, 0.97, new CornerRounding(0.189, 0.811)),
                            P(-0.021, 0.967, new CornerRounding(0.187, 0.057)),
                        },
                        2);
                case MaterialShapeKind.Arch:
                    return RoundedPolygonFactory.FromVertexCount(
                            4,
                            1,
                            0,
                            0,
                            CornerRounding.Unrounded,
                            new[] { CornerRound100, CornerRound100, CornerRound20, CornerRound20 })
                        .Transformed(RotateNeg135);
                case MaterialShapeKind.Fan:
                    return CustomPolygon(
                        new[]
                        {
                            P(1.004, 1.0, new CornerRounding(0.148, 0.417)),
                            P(0.0, 1.0, new CornerRounding(0.151)),
                            P(0.0, -0.003, new CornerRounding(0.148)),
                            P(0.978, 0.02, new CornerRounding(0.803)),
                        },
                        1);
                case MaterialShapeKind.Arrow:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 0.892, new CornerRounding(0.313)),
                            P(-0.216, 1.05, new CornerRounding(0.207)),
                            P(0.499, -0.16, new CornerRounding(0.215, 1.0)),
                            P(1.225, 1.06, new CornerRounding(0.211)),
                        },
                        1);
                case MaterialShapeKind.SemiCircle:
                    return RoundedPolygonFactory.Rectangle(
                        1.6,
                        1,
                        CornerRounding.Unrounded,
                        new[] { CornerRound20, CornerRound20, CornerRound100, CornerRound100 });
                case MaterialShapeKind.Oval:
                    return RoundedPolygonFactory.Circle()
                        .Transformed(point => new ShapePoint(point.X, point.Y * 0.64))
                        .Transformed(RotateNeg45);
                case MaterialShapeKind.Pill:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.961, 0.039, new CornerRounding(0.426)),
                            P(1.001, 0.428),
                            P(1.0, 0.609, new CornerRounding(1.0)),
                        },
                        2,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Triangle:
                    return RoundedPolygonFactory.FromVertexCount(3, 1, 0, 0, CornerRound20).Transformed(RotateNeg90);
                case MaterialShapeKind.Diamond:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 1.096, new CornerRounding(0.151, 0.524)),
                            P(0.04, 0.5, new CornerRounding(0.159)),
                        },
                        2);
                case MaterialShapeKind.ClamShell:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.171, 0.841, new CornerRounding(0.159)),
                            P(-0.02, 0.5, new CornerRounding(0.14)),
                            P(0.17, 0.159, new CornerRounding(0.159)),
                        },
                        2);
                case MaterialShapeKind.Pentagon:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, -0.009, new CornerRounding(0.172)),
                            P(1.03, 0.365, new CornerRounding(0.164)),
                            P(0.828, 0.97, new CornerRounding(0.169)),
                        },
                        1,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Gem:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.499, 1.023, new CornerRounding(0.241, 0.778)),
                            P(-0.005, 0.792, new CornerRounding(0.208)),
                            P(0.073, 0.258, new CornerRounding(0.228)),
                            P(0.433, -0.0, new CornerRounding(0.491)),
                        },
                        1,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Sunny:
                    return RoundedPolygonFactory.Star(8, 1, 0.8, CornerRound15);
                case MaterialShapeKind.VerySunny:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 1.08, new CornerRounding(0.085)),
                            P(0.358, 0.843, new CornerRounding(0.085)),
                        },
                        8);
                case MaterialShapeKind.Cookie4Sided:
                    return CustomPolygon(
                        new[]
                        {
                            P(1.237, 1.236, new CornerRounding(0.258)),
                            P(0.5, 0.918, new CornerRounding(0.233)),
                        },
                        4);
                case MaterialShapeKind.Cookie6Sided:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.723, 0.884, new CornerRounding(0.394)),
                            P(0.5, 1.099, new CornerRounding(0.398)),
                        },
                        6);
                case MaterialShapeKind.Cookie7Sided:
                    return RoundedPolygonFactory.Star(7, 1, 0.75, CornerRound50).Transformed(RotateNeg90);
                case MaterialShapeKind.Cookie9Sided:
                    return RoundedPolygonFactory.Star(9, 1, 0.8, CornerRound50).Transformed(RotateNeg90);
                case MaterialShapeKind.Cookie12Sided:
                    return RoundedPolygonFactory.Star(12, 1, 0.8, CornerRound50).Transformed(RotateNeg90);
                case MaterialShapeKind.Ghostish:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 0, new CornerRounding(1.0)),
                            P(1, 0, new CornerRounding(1.0)),
                            P(1, 1.14, new CornerRounding(0.254, 0.106)),
                            P(0.575, 0.906, new CornerRounding(0.253)),
                        },
                        1,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Clover4Leaf:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 0.074),
                            P(0.725, -0.099, new CornerRounding(0.476)),
                        },
                        4,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Clover8Leaf:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 0.036),
                            P(0.758, -0.101, new CornerRounding(0.209)),
                        },
                        8);
                case MaterialShapeKind.Burst:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, -0.006, new CornerRounding(0.006)),
                            P(0.592, 0.158, new CornerRounding(0.006)),
                        },
                        12);
                case MaterialShapeKind.SoftBurst:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.193, 0.277, new CornerRounding(0.053)),
                            P(0.176, 0.055, new CornerRounding(0.053)),
                        },
                        10);
                case MaterialShapeKind.Boom:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.457, 0.296, new CornerRounding(0.007)),
                            P(0.5, -0.051, new CornerRounding(0.007)),
                        },
                        15);
                case MaterialShapeKind.SoftBoom:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.733, 0.454),
                            P(0.839, 0.437, new CornerRounding(0.532)),
                            P(0.949, 0.449, new CornerRounding(0.439, 1.0)),
                            P(0.998, 0.478, new CornerRounding(0.174)),
                        },
                        16,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Flower:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.37, 0.187),
                            P(0.416, 0.049, new CornerRounding(0.381)),
                            P(0.479, 0.001, new CornerRounding(0.095)),
                        },
                        8,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Puffy:
                    return CustomPolygon(
                            new[]
                            {
                                P(0.5, 0.053),
                                P(0.545, -0.04, new CornerRounding(0.405)),
                                P(0.67, -0.035, new CornerRounding(0.426)),
                                P(0.717, 0.066, new CornerRounding(0.574)),
                                P(0.722, 0.128),
                                P(0.777, 0.002, new CornerRounding(0.36)),
                                P(0.914, 0.149, new CornerRounding(0.66)),
                                P(0.926, 0.289, new CornerRounding(0.66)),
                                P(0.881, 0.346),
                                P(0.94, 0.344, new CornerRounding(0.126)),
                                P(1.003, 0.437, new CornerRounding(0.255)),
                            },
                            2,
                            ShapeCenter,
                            true)
                        .Transformed(point => new ShapePoint(point.X, point.Y * 0.742));
                case MaterialShapeKind.PuffyDiamond:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.87, 0.13, new CornerRounding(0.146)),
                            P(0.818, 0.357),
                            P(1.0, 0.332, new CornerRounding(0.853)),
                        },
                        4,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.PixelCircle:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 0.0),
                            P(0.704, 0.0),
                            P(0.704, 0.065),
                            P(0.843, 0.065),
                            P(0.843, 0.148),
                            P(0.926, 0.148),
                            P(0.926, 0.296),
                            P(1.0, 0.296),
                        },
                        2,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.PixelTriangle:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.11, 0.5),
                            P(0.113, 0.0),
                            P(0.287, 0.0),
                            P(0.287, 0.087),
                            P(0.421, 0.087),
                            P(0.421, 0.17),
                            P(0.56, 0.17),
                            P(0.56, 0.265),
                            P(0.674, 0.265),
                            P(0.675, 0.344),
                            P(0.789, 0.344),
                            P(0.789, 0.439),
                            P(0.888, 0.439),
                        },
                        1,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Bun:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.796, 0.5),
                            P(0.853, 0.518, new CornerRounding(1)),
                            P(0.992, 0.631, new CornerRounding(1)),
                            P(0.968, 1.0, new CornerRounding(1)),
                        },
                        2,
                        ShapeCenter,
                        true);
                case MaterialShapeKind.Heart:
                    return CustomPolygon(
                        new[]
                        {
                            P(0.5, 0.268, new CornerRounding(0.016)),
                            P(0.792, -0.066, new CornerRounding(0.958)),
                            P(1.064, 0.276, new CornerRounding(1.0)),
                            P(0.501, 0.946, new CornerRounding(0.129)),
                        },
                        1,
                        ShapeCenter,
                        true);
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape));
            }
        }

        private static ShapePointSpec P(double x, double y, CornerRounding rounding = default)
        {
            return new ShapePointSpec(new ShapePoint(x, y), rounding);
        }

        private static RoundedPolygon CustomPolygon(
            ShapePointSpec[] shapePoints,
            int reps,
            ShapePoint? center = null,
            bool mirroring = false)
        {
            ShapePoint resolvedCenter = center ?? ShapeCenter;
            ShapePointSpec[] actualPoints = DoRepeat(shapePoints, reps, resolvedCenter, mirroring);
            double[] vertices = new double[actualPoints.Length * 2];
            CornerRounding[] perVertexRounding = new CornerRounding[actualPoints.Length];
            for (int i = 0; i < actualPoints.Length; i++)
            {
                vertices[i * 2] = actualPoints[i].Point.X;
                vertices[i * 2 + 1] = actualPoints[i].Point.Y;
                perVertexRounding[i] = actualPoints[i].Rounding;
            }
            return RoundedPolygonFactory.FromVertices(
                vertices,
                CornerRounding.Unrounded,
                perVertexRounding,
                resolvedCenter.X,
                resolvedCenter.Y);
        }

        private static ShapePointSpec[] DoRepeat(
            ShapePointSpec[] shapePoints,
            int reps,
            ShapePoint center,
            bool mirroring)
        {
            if (mirroring)
            {
                double[] angles = new double[shapePoints.Length];
                double[] distances = new double[shapePoints.Length];
                for (int i = 0; i < shapePoints.Length; i++)
                {
                    angles[i] = GeometryMathematics.AngleDegrees(shapePoints[i].Point - center);
                    distances[i] = GeometryMathematics.PointDistance(shapePoints[i].Point, center);
                }
                int actualReps = reps * 2;
                double sectionAngle = 360.0 / actualReps;
                List<ShapePointSpec> mirrored = new List<ShapePointSpec>();
                for (int rep = 0; rep < actualReps; rep++)
                {
                    for (int index = 0; index < shapePoints.Length; index++)
                    {
                        int i = rep % 2 == 0 ? index : shapePoints.Length - 1 - index;
                        if (i > 0 || rep % 2 == 0)
                        {
                            double angle = sectionAngle * rep +
                                (rep % 2 == 0
                                    ? angles[i]
                                    : sectionAngle - angles[i] + 2 * angles[0]);
                            ShapePoint direction = new ShapePoint(
                                FreelyDistributableMathLibrary.Cos(GeometryMathematics.ToRadians(angle)),
                                FreelyDistributableMathLibrary.Sin(GeometryMathematics.ToRadians(angle)));
                            ShapePoint finalPoint = direction * distances[i] + center;
                            mirrored.Add(new ShapePointSpec(finalPoint, shapePoints[i].Rounding));
                        }
                    }
                }
                return mirrored.ToArray();
            }

            ShapePointSpec[] result = new ShapePointSpec[shapePoints.Length * reps];
            int count = shapePoints.Length;
            for (int iteration = 0; iteration < result.Length; iteration++)
            {
                ShapePointSpec source = shapePoints[iteration % count];
                result[iteration] = new ShapePointSpec(
                    GeometryMathematics.RotateDegrees(
                        source.Point,
                        (Math.Floor((double)iteration / count) * 360) / reps,
                        center),
                    source.Rounding);
            }
            return result;
        }

        private readonly record struct ShapePointSpec(ShapePoint Point, CornerRounding Rounding);
    }
}
