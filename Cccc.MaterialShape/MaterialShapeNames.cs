using System;
using System.Collections.Generic;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Resolves the canonical string slug used by the Material Design 3
    /// documentation and by the TypeScript reference implementation.
    /// </summary>
    public static class MaterialShapeNames
    {
        private static readonly Dictionary<string, MaterialShapeKind> BySlug =
            new Dictionary<string, MaterialShapeKind>(StringComparer.Ordinal);

        static MaterialShapeNames()
        {
            foreach (MaterialShapeKind shape in (MaterialShapeKind[])Enum.GetValues(typeof(MaterialShapeKind)))
            {
                BySlug[ToSlug(shape)] = shape;
            }
        }

        public static string ToSlug(MaterialShapeKind shape)
        {
            switch (shape)
            {
                case MaterialShapeKind.Circle: return "circle";
                case MaterialShapeKind.Square: return "square";
                case MaterialShapeKind.Slanted: return "slanted";
                case MaterialShapeKind.Arch: return "arch";
                case MaterialShapeKind.Fan: return "fan";
                case MaterialShapeKind.Arrow: return "arrow";
                case MaterialShapeKind.SemiCircle: return "semi-circle";
                case MaterialShapeKind.Oval: return "oval";
                case MaterialShapeKind.Pill: return "pill";
                case MaterialShapeKind.Triangle: return "triangle";
                case MaterialShapeKind.Diamond: return "diamond";
                case MaterialShapeKind.ClamShell: return "clam-shell";
                case MaterialShapeKind.Pentagon: return "pentagon";
                case MaterialShapeKind.Gem: return "gem";
                case MaterialShapeKind.Sunny: return "sunny";
                case MaterialShapeKind.VerySunny: return "very-sunny";
                case MaterialShapeKind.Cookie4Sided: return "cookie-4-sided";
                case MaterialShapeKind.Cookie6Sided: return "cookie-6-sided";
                case MaterialShapeKind.Cookie7Sided: return "cookie-7-sided";
                case MaterialShapeKind.Cookie9Sided: return "cookie-9-sided";
                case MaterialShapeKind.Cookie12Sided: return "cookie-12-sided";
                case MaterialShapeKind.Ghostish: return "ghostish";
                case MaterialShapeKind.Clover4Leaf: return "clover-4-leaf";
                case MaterialShapeKind.Clover8Leaf: return "clover-8-leaf";
                case MaterialShapeKind.Burst: return "burst";
                case MaterialShapeKind.SoftBurst: return "soft-burst";
                case MaterialShapeKind.Boom: return "boom";
                case MaterialShapeKind.SoftBoom: return "soft-boom";
                case MaterialShapeKind.Flower: return "flower";
                case MaterialShapeKind.Puffy: return "puffy";
                case MaterialShapeKind.PuffyDiamond: return "puffy-diamond";
                case MaterialShapeKind.PixelCircle: return "pixel-circle";
                case MaterialShapeKind.PixelTriangle: return "pixel-triangle";
                case MaterialShapeKind.Bun: return "bun";
                case MaterialShapeKind.Heart: return "heart";
                default: throw new ArgumentOutOfRangeException(nameof(shape));
            }
        }

        public static bool TryParse(string slug, out MaterialShapeKind shape)
        {
            if (slug == null) { shape = default; return false; }
            return BySlug.TryGetValue(slug, out shape);
        }
    }
}
