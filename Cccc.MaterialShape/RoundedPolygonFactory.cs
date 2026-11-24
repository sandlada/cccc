using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Parametric shape builders ported from <c>androidx.graphics.shapes.RoundedPolygonFactory.kt</c>.
    /// </summary>
    public static class RoundedPolygonFactory
    {
        /// <summary>
        /// Builds a polygon from an ordered list of x/y vertex pairs and shared or
        /// per-vertex rounding parameters.
        /// </summary>
        public static RoundedPolygon FromVertices(
            ReadOnlySpan<double> vertices,
            CornerRounding rounding = default,
            IReadOnlyList<CornerRounding> perVertexRounding = null,
            double? centerX = null,
            double? centerY = null)
        {
            if (vertices.Length < 6) throw new ArgumentException("Polygons must have at least 3 vertices", nameof(vertices));
            if (vertices.Length % 2 == 1) throw new ArgumentException("The vertices array should have even size", nameof(vertices));
            if (perVertexRounding != null && perVertexRounding.Count * 2 != vertices.Length)
            {
                throw new ArgumentException(
                    "perVertexRounding list should be either null or the same size as the number of vertices (vertices.size / 2)",
                    nameof(perVertexRounding));
            }

            int vertexCount = vertices.Length / 2;
            RoundedCorner[] roundedCorners = new RoundedCorner[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                CornerRounding vertexRounding = perVertexRounding != null ? perVertexRounding[i] : rounding;
                int previousIndex = ((i + vertexCount - 1) % vertexCount) * 2;
                int nextIndex = ((i + 1) % vertexCount) * 2;
                roundedCorners[i] = new RoundedCorner(
                    new ShapePoint(vertices[previousIndex], vertices[previousIndex + 1]),
                    new ShapePoint(vertices[i * 2], vertices[i * 2 + 1]),
                    new ShapePoint(vertices[nextIndex], vertices[nextIndex + 1]),
                    vertexRounding);
            }

            (double roundCutRatio, double cutRatio)[] cutAdjusts = new (double, double)[vertexCount];
            for (int ix = 0; ix < vertexCount; ix++)
            {
                RoundedCorner current = roundedCorners[ix];
                RoundedCorner next = roundedCorners[(ix + 1) % vertexCount];
                double expectedRoundCut = current.ExpectedRoundCut + next.ExpectedRoundCut;
                double expectedCut = current.ExpectedCut + next.ExpectedCut;
                double vertexX = vertices[ix * 2];
                double vertexY = vertices[ix * 2 + 1];
                double nextVertexX = vertices[((ix + 1) % vertexCount) * 2];
                double nextVertexY = vertices[((ix + 1) % vertexCount) * 2 + 1];
                double sideSize = GeometryMathematics.PointDistance(
                    new ShapePoint(vertexX, vertexY),
                    new ShapePoint(nextVertexX, nextVertexY));
                if (expectedRoundCut > sideSize)
                {
                    cutAdjusts[ix] = (sideSize / expectedRoundCut, 0);
                }
                else if (expectedCut > sideSize)
                {
                    cutAdjusts[ix] = (1, (sideSize - expectedRoundCut) / (expectedCut - expectedRoundCut));
                }
                else
                {
                    cutAdjusts[ix] = (1, 1);
                }
            }

            Cubic[][] cornerCubics = new Cubic[vertexCount][];
            for (int i = 0; i < vertexCount; i++)
            {
                double[] allowedCuts = new double[2];
                for (int delta = 0; delta <= 1; delta++)
                {
                    (double roundCutRatio, double cutRatio) = cutAdjusts[(i + vertexCount - 1 + delta) % vertexCount];
                    RoundedCorner corner = roundedCorners[i];
                    allowedCuts[delta] =
                        corner.ExpectedRoundCut * roundCutRatio +
                        (corner.ExpectedCut - corner.ExpectedRoundCut) * cutRatio;
                }
                cornerCubics[i] = roundedCorners[i].GetCubics(allowedCuts[0], allowedCuts[1]);
            }

            List<Feature> features = new List<Feature>(vertexCount * 2);
            for (int i = 0; i < vertexCount; i++)
            {
                int previousVertexIndex = (i + vertexCount - 1) % vertexCount;
                int nextVertexIndex = (i + 1) % vertexCount;
                ShapePoint currentVertex = new ShapePoint(vertices[i * 2], vertices[i * 2 + 1]);
                ShapePoint previousVertex = new ShapePoint(
                    vertices[previousVertexIndex * 2],
                    vertices[previousVertexIndex * 2 + 1]);
                ShapePoint nextVertex = new ShapePoint(
                    vertices[nextVertexIndex * 2],
                    vertices[nextVertexIndex * 2 + 1]);
                bool isConvex = GeometryMathematics.IsConvex(previousVertex, currentVertex, nextVertex);
                features.Add(new FeatureCorner(cornerCubics[i], isConvex));
                Cubic cornerEnd = cornerCubics[i][cornerCubics[i].Length - 1];
                Cubic nextCornerStart = cornerCubics[(i + 1) % vertexCount][0];
                features.Add(new FeatureEdge(new Cubic[]
                {
                    Cubic.StraightLine(
                        cornerEnd.Anchor1X,
                        cornerEnd.Anchor1Y,
                        nextCornerStart.Anchor0X,
                        nextCornerStart.Anchor0Y),
                }));
            }

            (double centerXValue, double centerYValue) = CalculateCenter(vertices);
            ShapePoint center = centerX.HasValue && centerY.HasValue
                ? new ShapePoint(centerX.Value, centerY.Value)
                : new ShapePoint(centerXValue, centerYValue);
            return new RoundedPolygon(features, center);
        }

        /// <summary>Builds a regular polygon with numVertices vertices on a circle.</summary>
        public static RoundedPolygon FromVertexCount(
            int numVertices,
            double radius = 1,
            double centerX = 0,
            double centerY = 0,
            CornerRounding rounding = default,
            IReadOnlyList<CornerRounding> perVertexRounding = null)
        {
            if (numVertices < 3) throw new ArgumentException("Polygons must have at least 3 vertices", nameof(numVertices));
            double[] vertices = new double[numVertices * 2];
            for (int i = 0; i < numVertices; i++)
            {
                ShapePoint vertex = GeometryMathematics.RadialToCartesian(
                    radius,
                    (Math.PI / numVertices) * 2 * i,
                    new ShapePoint(centerX, centerY));
                vertices[i * 2] = vertex.X;
                vertices[i * 2 + 1] = vertex.Y;
            }
            return FromVertices(vertices, rounding, perVertexRounding, centerX, centerY);
        }

        /// <summary>
        /// Circular shape approximating the rounding of the shape around the
        /// underlying polygon vertices (mirrors RoundedPolygon.circle).
        /// </summary>
        public static RoundedPolygon Circle(
            int numVertices = 8,
            double radius = 1,
            double centerX = 0,
            double centerY = 0)
        {
            if (numVertices < 3) throw new ArgumentException("Circle must have at least three vertices", nameof(numVertices));
            double theta = Math.PI / numVertices;
            double polygonRadius = radius / FreelyDistributableMathLibrary.Cos(theta);
            return FromVertexCount(
                numVertices,
                polygonRadius,
                centerX,
                centerY,
                new CornerRounding(radius));
        }

        /// <summary>Rectangle with the given width/height around (centerX, centerY).</summary>
        public static RoundedPolygon Rectangle(
            double width = 2,
            double height = 2,
            CornerRounding rounding = default,
            IReadOnlyList<CornerRounding> perVertexRounding = null,
            double centerX = 0,
            double centerY = 0)
        {
            if (!(width > 0 && height > 0)) throw new ArgumentException("Rectangle shapes must have positive width and height");
            double left = centerX - width / 2;
            double top = centerY - height / 2;
            double right = centerX + width / 2;
            double bottom = centerY + height / 2;
            return FromVertices(
                new double[] { right, bottom, left, bottom, left, top, right, top },
                rounding,
                perVertexRounding,
                centerX,
                centerY);
        }

        /// <summary>Star polygon: every other vertex lies on the inner radius.</summary>
        public static RoundedPolygon Star(
            int numVerticesPerRadius,
            double radius = 1,
            double innerRadius = 0.5,
            CornerRounding rounding = default,
            CornerRounding? innerRounding = null,
            IReadOnlyList<CornerRounding> perVertexRounding = null,
            double centerX = 0,
            double centerY = 0)
        {
            if (radius <= 0 || innerRadius <= 0) throw new ArgumentException("Star radii must both be greater than 0");
            if (innerRadius >= radius) throw new ArgumentException("innerRadius must be less than radius");
            if (numVerticesPerRadius < 2) throw new ArgumentException("Star must have at least 2 vertices per radius");

            IReadOnlyList<CornerRounding> pvRounding = perVertexRounding;
            if (pvRounding == null && innerRounding.HasValue)
            {
                CornerRounding[] list = new CornerRounding[numVerticesPerRadius * 2];
                for (int i = 0; i < numVerticesPerRadius; i++)
                {
                    list[i * 2] = rounding;
                    list[i * 2 + 1] = innerRounding.Value;
                }
                pvRounding = list;
            }

            double[] vertices = new double[numVerticesPerRadius * 4];
            for (int i = 0; i < numVerticesPerRadius; i++)
            {
                ShapePoint outer = GeometryMathematics.RadialToCartesian(
                    radius,
                    (Math.PI / numVerticesPerRadius) * 2 * i,
                    new ShapePoint(centerX, centerY));
                vertices[i * 4] = outer.X;
                vertices[i * 4 + 1] = outer.Y;
                ShapePoint inner = GeometryMathematics.RadialToCartesian(
                    innerRadius,
                    (Math.PI / numVerticesPerRadius) * (2 * i + 1),
                    new ShapePoint(centerX, centerY));
                vertices[i * 4 + 2] = inner.X;
                vertices[i * 4 + 3] = inner.Y;
            }
            return FromVertices(vertices, rounding, pvRounding, centerX, centerY);
        }

        /// <summary>Capsule/pill polygon (mirrors RoundedPolygon.pill).</summary>
        public static RoundedPolygon Pill(
            double width = 2,
            double height = 1,
            double smoothing = 0,
            double centerX = 0,
            double centerY = 0)
        {
            if (!(width > 0 && height > 0)) throw new ArgumentException("Pill shapes must have positive width and height");
            double wHalf = width / 2;
            double hHalf = height / 2;
            return FromVertices(
                new double[]
                {
                    wHalf + centerX,
                    hHalf + centerY,
                    -wHalf + centerX,
                    hHalf + centerY,
                    -wHalf + centerX,
                    -hHalf + centerY,
                    wHalf + centerX,
                    -hHalf + centerY,
                },
                new CornerRounding(Math.Min(wHalf, hHalf), smoothing),
                null,
                centerX,
                centerY);
        }

        /// <summary>Pill-shaped star polygon (mirrors RoundedPolygon.pillStar).</summary>
        public static RoundedPolygon PillStar(
            double width = 2,
            double height = 1,
            int numVerticesPerRadius = 8,
            double innerRadiusRatio = 0.5,
            CornerRounding rounding = default,
            CornerRounding? innerRounding = null,
            IReadOnlyList<CornerRounding> perVertexRounding = null,
            double vertexSpacing = 0.5,
            double startLocation = 0,
            double centerX = 0,
            double centerY = 0)
        {
            if (!(width > 0 && height > 0)) throw new ArgumentException("Pill shapes must have positive width and height");
            if (!(innerRadiusRatio > 0 && innerRadiusRatio <= 1))
            {
                throw new ArgumentException("innerRadius must be between 0 and 1", nameof(innerRadiusRatio));
            }
            if (numVerticesPerRadius < 2) throw new ArgumentException("pillStar must have at least 2 vertices per radius", nameof(numVerticesPerRadius));

            IReadOnlyList<CornerRounding> pvRounding = perVertexRounding;
            if (pvRounding == null && innerRounding.HasValue)
            {
                CornerRounding[] list = new CornerRounding[numVerticesPerRadius * 2];
                for (int i = 0; i < numVerticesPerRadius; i++)
                {
                    list[i * 2] = rounding;
                    list[i * 2 + 1] = innerRounding.Value;
                }
                pvRounding = list;
            }

            double[] vertices = PillStarVerticesFromVertexCount(
                numVerticesPerRadius,
                width,
                height,
                innerRadiusRatio,
                vertexSpacing,
                startLocation,
                centerX,
                centerY);
            return FromVertices(vertices, rounding, pvRounding, centerX, centerY);
        }

        private static double[] PillStarVerticesFromVertexCount(
            int numVerticesPerRadius,
            double width,
            double height,
            double innerRadius,
            double vertexSpacing,
            double startLocation,
            double centerX,
            double centerY)
        {
            double endcapRadius = Math.Min(width, height);
            double vSegLen = Math.Max(0, height - width);
            double hSegLen = Math.Max(0, width - height);
            double vSegHalf = vSegLen / 2;
            double hSegHalf = hSegLen / 2;
            double circlePerimeter = 2 * Math.PI * endcapRadius * GeometryMathematics.Interpolate(innerRadius, 1, vertexSpacing);
            double perimeter = 2 * hSegLen + 2 * vSegLen + circlePerimeter;

            double[] sections = new double[11];
            sections[0] = 0;
            sections[1] = vSegLen / 2;
            sections[2] = sections[1] + circlePerimeter / 4;
            sections[3] = sections[2] + hSegLen;
            sections[4] = sections[3] + circlePerimeter / 4;
            sections[5] = sections[4] + vSegLen;
            sections[6] = sections[5] + circlePerimeter / 4;
            sections[7] = sections[6] + hSegLen;
            sections[8] = sections[7] + circlePerimeter / 4;
            sections[9] = sections[8] + vSegLen / 2;
            sections[10] = perimeter;

            double tPerVertex = perimeter / (2 * numVerticesPerRadius);
            bool inner = false;
            int currSecIndex = 0;
            double secStart = 0;
            double secEnd = sections[1];
            double t = startLocation * perimeter;
            double[] result = new double[numVerticesPerRadius * 4];

            ShapePoint rectBR = new ShapePoint(hSegHalf, vSegHalf);
            ShapePoint rectBL = new ShapePoint(-hSegHalf, vSegHalf);
            ShapePoint rectTL = new ShapePoint(-hSegHalf, -vSegHalf);
            ShapePoint rectTR = new ShapePoint(hSegHalf, -vSegHalf);

            for (int i = 0; i < numVerticesPerRadius * 2; i++)
            {
                double boundedT = t % perimeter;
                if (boundedT < secStart) currSecIndex = 0;
                while (boundedT >= sections[(currSecIndex + 1) % sections.Length])
                {
                    currSecIndex = (currSecIndex + 1) % sections.Length;
                    secStart = sections[currSecIndex];
                    secEnd = sections[(currSecIndex + 1) % sections.Length];
                }

                double tInSection = boundedT - secStart;
                double tProportion = secEnd > secStart ? tInSection / (secEnd - secStart) : 0;
                double currRadius = inner ? endcapRadius * innerRadius : endcapRadius;

                ShapePoint vertex;
                switch (currSecIndex)
                {
                    case 0:
                        vertex = new ShapePoint(currRadius, tProportion * vSegHalf);
                        break;
                    case 1:
                        vertex = GeometryMathematics.RadialToCartesian(
                            currRadius,
                            (tProportion * Math.PI) / 2,
                            rectBR);
                        break;
                    case 2:
                        vertex = new ShapePoint(hSegHalf - tProportion * hSegLen, currRadius);
                        break;
                    case 3:
                        vertex = GeometryMathematics.RadialToCartesian(
                            currRadius,
                            Math.PI / 2 + (tProportion * Math.PI) / 2,
                            rectBL);
                        break;
                    case 4:
                        vertex = new ShapePoint(-currRadius, vSegHalf - tProportion * vSegLen);
                        break;
                    case 5:
                        vertex = GeometryMathematics.RadialToCartesian(
                            currRadius,
                            Math.PI + (tProportion * Math.PI) / 2,
                            rectTL);
                        break;
                    case 6:
                        vertex = new ShapePoint(-hSegHalf + tProportion * hSegLen, -currRadius);
                        break;
                    case 7:
                        vertex = GeometryMathematics.RadialToCartesian(
                            currRadius,
                            Math.PI * 1.5 + (tProportion * Math.PI) / 2,
                            rectTR);
                        break;
                    default:
                        vertex = new ShapePoint(currRadius, -vSegHalf + tProportion * vSegHalf);
                        break;
                }

                result[i * 2] = vertex.X + centerX;
                result[i * 2 + 1] = vertex.Y + centerY;
                t += tPerVertex;
                inner = !inner;
            }

            return result;
        }

        private static (double X, double Y) CalculateCenter(ReadOnlySpan<double> vertices)
        {
            double cumulativeX = 0;
            double cumulativeY = 0;
            int index = 0;
            while (index < vertices.Length)
            {
                cumulativeX += vertices[index++];
                cumulativeY += vertices[index++];
            }
            return (cumulativeX / (vertices.Length / 2), cumulativeY / (vertices.Length / 2));
        }

        private sealed class RoundedCorner
        {
            private readonly ShapePoint p0;
            private readonly ShapePoint p1;
            private readonly ShapePoint p2;
            private readonly ShapePoint d1;
            private readonly ShapePoint d2;
            private readonly double cornerRadius;
            private readonly double smoothing;
            private readonly double cosAngle;
            private readonly double sinAngle;
            private ShapePoint center;

            internal RoundedCorner(ShapePoint p0, ShapePoint p1, ShapePoint p2, CornerRounding rounding)
            {
                this.p0 = p0;
                this.p1 = p1;
                this.p2 = p2;
                ShapePoint v01 = p0 - p1;
                ShapePoint v21 = p2 - p1;
                double d01 = GeometryMathematics.PointDistance(p0, p1);
                double d21 = GeometryMathematics.PointDistance(p2, p1);
                if (d01 > 0 && d21 > 0)
                {
                    double inverseD01 = 1 / d01;
                    double inverseD21 = 1 / d21;
                    d1 = new ShapePoint(v01.X * inverseD01, v01.Y * inverseD01);
                    d2 = new ShapePoint(v21.X * inverseD21, v21.Y * inverseD21);
                    cornerRadius = rounding.Radius;
                    smoothing = rounding.Smoothing;
                    cosAngle = GeometryMathematics.DotProduct(d1, d2);
                    sinAngle = Math.Sqrt(Math.Max(0, 1 - GeometryMathematics.Square(cosAngle)));
                    ExpectedRoundCut = sinAngle > 1e-3
                        ? (cornerRadius * (cosAngle + 1)) / sinAngle
                        : 0;
                }
                else
                {
                    d1 = new ShapePoint(0, 0);
                    d2 = new ShapePoint(0, 0);
                    cornerRadius = 0;
                    smoothing = 0;
                    cosAngle = 0;
                    sinAngle = 0;
                    ExpectedRoundCut = 0;
                }
                center = new ShapePoint(0, 0);
            }

            internal double ExpectedRoundCut { get; }

            internal double ExpectedCut { get { return (1 + smoothing) * ExpectedRoundCut; } }

            internal Cubic[] GetCubics(double allowedCut0, double? allowedCut1 = null)
            {
                double secondCut = allowedCut1.HasValue ? allowedCut1.Value : allowedCut0;
                double allowedCut = Math.Min(allowedCut0, secondCut);
                if (ExpectedRoundCut < GeometryMathematics.DistanceEpsilon ||
                    allowedCut < GeometryMathematics.DistanceEpsilon ||
                    cornerRadius < GeometryMathematics.DistanceEpsilon)
                {
                    center = p1;
                    return new Cubic[] { Cubic.StraightLine(p1.X, p1.Y, p1.X, p1.Y) };
                }
                double actualRoundCut = Math.Min(allowedCut, ExpectedRoundCut);
                double actualSmoothing0 = CalculateActualSmoothingValue(allowedCut0);
                double actualSmoothing1 = CalculateActualSmoothingValue(secondCut);
                double actualRadius = (cornerRadius * actualRoundCut) / ExpectedRoundCut;
                double centerDistance = Math.Sqrt(
                    GeometryMathematics.Square(actualRadius) + GeometryMathematics.Square(actualRoundCut));
                ShapePoint midDirection = new ShapePoint((d1.X + d2.X) / 2, (d1.Y + d2.Y) / 2);
                center = p1 + GeometryMathematics.DirectionVector(midDirection.X, midDirection.Y) * centerDistance;
                ShapePoint circleIntersection0 = p1 + d1 * actualRoundCut;
                ShapePoint circleIntersection2 = p1 + d2 * actualRoundCut;
                Cubic flanking0 = ComputeFlankingCurve(
                    actualRoundCut,
                    actualSmoothing0,
                    p1,
                    p0,
                    circleIntersection0,
                    circleIntersection2,
                    center,
                    actualRadius);
                Cubic flanking2 = ComputeFlankingCurve(
                    actualRoundCut,
                    actualSmoothing1,
                    p1,
                    p2,
                    circleIntersection2,
                    circleIntersection0,
                    center,
                    actualRadius).Reverse();
                return new Cubic[]
                {
                    flanking0,
                    Cubic.CircularArc(
                        center.X,
                        center.Y,
                        flanking0.Anchor1X,
                        flanking0.Anchor1Y,
                        flanking2.Anchor0X,
                        flanking2.Anchor0Y),
                    flanking2,
                };
            }

            private double CalculateActualSmoothingValue(double allowedCut)
            {
                if (allowedCut > ExpectedCut) return smoothing;
                if (allowedCut > ExpectedRoundCut)
                {
                    return (smoothing * (allowedCut - ExpectedRoundCut)) / (ExpectedCut - ExpectedRoundCut);
                }
                return 0;
            }

            private Cubic ComputeFlankingCurve(
                double actualRoundCut,
                double actualSmoothing,
                ShapePoint corner,
                ShapePoint sideStart,
                ShapePoint circleSegmentIntersection,
                ShapePoint otherCircleSegmentIntersection,
                ShapePoint circleCenter,
                double actualRadius)
            {
                ShapePoint sideDirection = GeometryMathematics.DirectionVector(
                    sideStart.X - corner.X,
                    sideStart.Y - corner.Y);
                ShapePoint curveStart = corner + sideDirection * (actualRoundCut * (1 + actualSmoothing));
                ShapePoint p = GeometryMathematics.InterpolatePoints(
                    circleSegmentIntersection,
                    new ShapePoint(
                        (circleSegmentIntersection.X + otherCircleSegmentIntersection.X) / 2,
                        (circleSegmentIntersection.Y + otherCircleSegmentIntersection.Y) / 2),
                    actualSmoothing);
                ShapePoint curveEnd = circleCenter +
                    GeometryMathematics.DirectionVector(p.X - circleCenter.X, p.Y - circleCenter.Y) * actualRadius;
                ShapePoint circleTangent = GeometryMathematics.Rotate90(curveEnd - circleCenter);
                ShapePoint anchorEnd =
                    LineIntersection(sideStart, sideDirection, curveEnd, circleTangent) ??
                    circleSegmentIntersection;
                ShapePoint anchorStart = (curveStart + anchorEnd * 2) * (1.0 / 3.0);
                return new Cubic(
                    curveStart.X,
                    curveStart.Y,
                    anchorStart.X,
                    anchorStart.Y,
                    anchorEnd.X,
                    anchorEnd.Y,
                    curveEnd.X,
                    curveEnd.Y);
            }

            private static ShapePoint? LineIntersection(ShapePoint p0, ShapePoint d0, ShapePoint p1, ShapePoint d1)
            {
                ShapePoint rotatedD1 = GeometryMathematics.Rotate90(d1);
                double den = GeometryMathematics.DotProduct(d0, rotatedD1);
                if (Math.Abs(den) < GeometryMathematics.DistanceEpsilon) return null;
                double num = GeometryMathematics.DotProduct(p1 - p0, rotatedD1);
                if (Math.Abs(den) < GeometryMathematics.DistanceEpsilon * Math.Abs(num)) return null;
                double k = num / den;
                return p0 + d0 * k;
            }
        }
    }
}
