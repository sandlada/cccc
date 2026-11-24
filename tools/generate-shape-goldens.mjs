#!/usr/bin/env node
/**
 * Regenerates the C# conformance fixture from the TypeScript reference.
 *
 * Usage:
 *   node tools/generate-shape-goldens.mjs \
 *     --source <path-to-mdc>/packages/mdc/src/components/shape \
 *     --out Cccc.MaterialShape.Tests/Goldens/golden.json \
 *     --commit <sha>
 *
 * Node 26 executes the reference TypeScript directly (type stripping). The
 * script copies the sources into a temporary folder, appends `.ts` to relative
 * imports, and runs the generator with no npm dependencies.
 */
import fs from 'node:fs'
import os from 'node:os'
import path from 'node:path'
import { spawnSync } from 'node:child_process'

const args = process.argv.slice(2)
function option(name, fallback) {
  const index = args.indexOf(name)
  return index >= 0 && index + 1 < args.length ? args[index + 1] : fallback
}

const source = option('--source', '')
const output = option('--out', path.join('Cccc.MaterialShape.Tests', 'Goldens', 'golden.json'))
const commit = option('--commit', 'unknown')

if (!source || !fs.existsSync(source)) {
  console.error('Missing --source <path-to-shape-directory>.')
  process.exit(1)
}

const temporary = fs.mkdtempSync(path.join(os.tmpdir(), 'cccc-material-shape-goldens-'))
fs.cpSync(source, temporary, { recursive: true })

const importPattern = /from\s+(['"])(\.[^'"]+)\1/g
function rewriteImports(directory) {
  for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
    const full = path.join(directory, entry.name)
    if (entry.isDirectory()) {
      rewriteImports(full)
      continue
    }
    if (!entry.name.endsWith('.ts')) continue
    const before = fs.readFileSync(full, 'utf8')
    const after = before.replace(importPattern, (match, quote, specifier) => {
      if (/\.(ts|js|mts|cts)$/.test(specifier)) return match
      return `from ${quote}${specifier}.ts${quote}`
    })
    if (after !== before) fs.writeFileSync(full, after)
  }
}
rewriteImports(temporary)
fs.writeFileSync(path.join(temporary, 'package.json'), '{"type":"module"}')

const generator = `
import fs from 'node:fs'
import { AllMaterialShapeNames, getMaterialShapePolygon } from './internal/material-shapes.ts'
import { Morph } from './internal/morph.ts'
import { MotionScheme, springMotionAt, estimateAnimationDurationMillis } from './internal/spring.ts'

const names = AllMaterialShapeNames
const shapes = {}
for (const name of names) {
  const polygon = getMaterialShapePolygon(name)
  const bounds = polygon.calculateBounds(new Float64Array(4))
  const maxBounds = polygon.calculateMaxBounds(new Float64Array(4))
  shapes[name] = {
    cubics: polygon.cubics.map(cubic => Array.from(cubic.points)),
    bounds: Array.from(bounds),
    maxBounds: Array.from(maxBounds),
    center: [polygon.center.x, polygon.center.y],
  }
}

const morphs = {}
for (const first of names) {
  for (const second of names) {
    const morph = new Morph(getMaterialShapePolygon(first), getMaterialShapePolygon(second))
    morphs[first + '->' + second] = { curveCount: morph.curveCount }
  }
}

const samplePairs = [
  ['circle', 'heart'],
  ['circle', 'soft-burst'],
  ['heart', 'cookie-4-sided'],
  ['circle', 'square'],
  ['square', 'cookie-12-sided'],
  ['pixel-circle', 'sunny'],
  ['triangle', 'clover-4-leaf'],
  ['pill', 'square'],
]
for (const [first, second] of samplePairs) {
  const morph = new Morph(getMaterialShapePolygon(first), getMaterialShapePolygon(second))
  const samples = {}
  for (const progress of [0, 0.1, 0.5, 0.9, 1, 1.05]) {
    samples[String(progress)] = morph.asCubics(progress).map(cubic => Array.from(cubic.points))
  }
  morphs[first + '->' + second].samples = samples
  morphs[first + '->' + second].centerAtHalf = [morph.centerAt(0.5).x, morph.centerAt(0.5).y]
  morphs[first + '->' + second].bounds = Array.from(morph.calculateBounds(new Float64Array(4)))
}

const springs = {}
for (const [scheme, group] of [['expressive', MotionScheme.Expressive], ['standard', MotionScheme.Standard]]) {
  for (const [speed, spring] of Object.entries(group)) {
    const motion = springMotionAt(spring)
    springs[scheme + '.' + speed] = {
      spring,
      values: [0, 16, 50, 100, 250, 500, 1000].map(t => {
        const result = motion(0, 0, t, 1)
        return [result.value, result.velocity]
      }),
    }
  }
}
springs.durations = {
  expressiveFast: estimateAnimationDurationMillis({
    ...MotionScheme.Expressive.FastSpatial,
    initialVelocity: 0,
    initialDisplacement: -10,
    delta: 1,
  }),
  expressiveDefault: estimateAnimationDurationMillis({
    ...MotionScheme.Expressive.DefaultSpatial,
    initialVelocity: 0,
    initialDisplacement: -10,
    delta: 1,
  }),
  standardFast: estimateAnimationDurationMillis({
    ...MotionScheme.Standard.FastSpatial,
    initialVelocity: 0,
    initialDisplacement: -10,
    delta: 1,
  }),
  critical: estimateAnimationDurationMillis({
    stiffness: 1600,
    dampingRatio: 1,
    initialVelocity: 0,
    initialDisplacement: -10,
    delta: 1,
  }),
  overDamped: estimateAnimationDurationMillis({
    stiffness: 200,
    dampingRatio: 1.4,
    initialVelocity: 0,
    initialDisplacement: -10,
    delta: 1,
  }),
}

fs.writeFileSync('golden.json', JSON.stringify({
  commit: ${JSON.stringify(commit)},
  shapes,
  morphs,
  springs,
}))
`
fs.writeFileSync(path.join(temporary, 'generate.mts'), generator)

const result = spawnSync(process.execPath, ['generate.mts'], {
  cwd: temporary,
  stdio: 'inherit',
})
if (result.status !== 0) {
  console.error('Golden generation failed; temporary sources kept at ' + temporary)
  process.exit(result.status ?? 1)
}

const generated = path.join(temporary, 'golden.json')
const outputPath = path.resolve(output)
fs.mkdirSync(path.dirname(outputPath), { recursive: true })
fs.copyFileSync(generated, outputPath)
fs.rmSync(temporary, { recursive: true, force: true })
console.log('Wrote ' + outputPath + ' (' + fs.statSync(outputPath).size + ' bytes)')
