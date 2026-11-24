# Cccc.MaterialShape

Pure C# port of the Material Design 3 Expressive shape engine used by Cccc's
copy-feedback badge.

## Provenance

- TypeScript reference:
  `https://github.com/sandlada/mdc/tree/main/packages/mdc/src/components/shape`
  at commit `85398d36aaa9e0e48f589358735f27b741028813`.
- Geometry algorithms originate in AndroidX
  `androidx.graphics.shapes` (`RoundedPolygon`, `Morph`, `Shapes.kt`) and
  `androidx.compose.material3.MaterialShapes` / `MotionScheme`.
- The AndroidX-derived algorithms retain their Apache-2.0 notices; the C# port
  and integration code are MIT, matching this repository.

## Included

- `ShapePoint`, `Cubic`, `Feature`, `RoundedPolygon`, the full `Shapes.kt`
  parametric builders, all 35 `MaterialShapes`, `ShapeResolver`.
- `MeasuredPolygon` / `LengthMeasurer`, `DoubleMapper` / feature mapping,
  `Morph` including `ToPolygon` mid-flight snapshots.
- Analytic `Spring`, `MotionScheme` presets and duration estimation.
- `MorphAnimator` (interruptible spring morph + rotation).
- `MaterialShapeSequence` (the 8-shape copy cycle).
- `FreelyDistributableMathLibrary` (ported from fdlibm; sin / cos / atan / atan2) to match V8 bit-for-bit where .NET's
  UCRT differs by one ulp; this keeps the TypeScript golden comparisons exact.

## Not ported

The Lit element surface: DOM attributes, `clip-path`, ripple/focus mixins,
events, autoplay timers, variants/colors/size tokens and CSS. Those belong to
the web component; Cccc only needs the geometry and motion.

## Copy-feedback defaults

These mirror the updated `design.html`:

- Sequence: `circle → soft-burst → cookie-9-sided → pentagon → pill → sunny →
  cookie-4-sided → heart`
- Rotation: `+45°` per copy, blob only; the check mark stays upright.
- Motion: `expressive` / `fast` (`stiffness 800`, `dampingRatio 0.6`).
- The web preview uses `size="large"` (72 px). The WPF badge keeps its existing
  32 px (18 px in the compact taskbar style) so the card layout does not change.
- The card itself never scales. Only the badge outline and the counter animate.

## Build and test

```powershell
dotnet build Cccc.MaterialShape\Cccc.MaterialShape.csproj
dotnet test Cccc.MaterialShape.Tests\Cccc.MaterialShape.Tests.csproj
```

## Golden conformance

`Cccc.MaterialShape.Tests/Goldens/golden.json` is generated from the TypeScript
reference and pinned to the commit above. Regenerate with:

```powershell
node tools\generate-shape-goldens.mjs `
  --source C:\path\to\mdc\packages\mdc\src\components\shape `
  --out Cccc.MaterialShape.Tests\Goldens\golden.json `
  --commit 85398d36aaa9e0e48f589358735f27b741028813
```

Node 26 can execute the TypeScript sources directly; the generator copies them
to a temporary directory, adds `.ts` extensions to relative imports, and runs
them with no npm dependencies.
