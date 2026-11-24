# Cccc copy badge — `@sandlada/mdc` shape preview

Standalone HTML preview of the **published** `@sandlada/mdc` `<mdc-shape>` component, showing the
copy-badge feedback used by Cccc. It is intentionally outside the .NET solution and does not touch
`Cccc.sln` or `design.html`.

## Quick start

```powershell
cd preview/shape
npm install --no-audit --no-fund
npm run build
npm run serve
```

Open <http://127.0.0.1:8792/>.

`shape.bundle.js` is committed, so after the first install you can serve the folder with any static
server. Do **not** open `index.html` through `file://` — the page uses an ES module and browsers block
module loads from the file system. Either `npm run serve` (Python's `http.server` on
`127.0.0.1:8792`) or another local static server works.

## What the preview shows

| Element | Size | Notes |
| --- | --- | --- |
| Cccc copy-card badge | **72px** | `size="large"`, matching `design.html`; white check is slotted content and stays upright |
| Actual-size badge | **32px** | The real Cccc badge size; shown inside a dashed 72px frame for scale |
| Auto-demo intervals | **1000 / 500 / 100 ms** | Manual tick loop, not the component's `autoplay` |
| Shape step | **+45°** | `rotation` starts at **20745** (= 225° phase), then `+45` per copy |
| Ambient spin | **45°/s** | While the card is up, the badge idles at 45°/s (8 s per revolution); toggle in the controls |
| Card animation | **none** | The card is never scaled or transformed; only the component's SVG path/rotation animates |

The telemetry panel reports the active shape, target rotation, live spring angle, rotation phase,
`pathData` length, motion mode, card transform, and card size in real time.

## Component API used

The real `MDCShape` from
`@sandlada/mdc/build/components/shape/index.js` (exports `MDCShape` and registers
`<mdc-shape>` through `shape.js`) is bundled by esbuild:

```powershell
esbuild node_modules/@sandlada/mdc/build/components/shape/index.js `
  --bundle --format=esm --minify --outfile=shape.bundle.js
```

Each simulated copy does exactly this for both live instances:

```js
badge.rotation += 45;   // reactive Number property; reflects to [rotation]
badge.next();           // advances the Material shape sequence
```

The ambient spin adds `45 * dt` degrees to the same `rotation` target every 50 ms while the
"徽章持續自轉 45°/s" toggle is on. The component springs to the new target; the slotted white
check stays upright because only the shape path rotates.

The explicit `sequence` attribute is required because this preview drives ticks itself. In the
published build, `next()` advances `sequenceIndex`, but the active shape only resolves from the
sequence when `sequence` is non-empty **or** `autoplay` is on. Since `autoplay` is deliberately not
used, the default sequence is set explicitly (in `DefaultAutoplaySequence` order):

```
circle → soft-burst → cookie-9-sided → pentagon → pill → sunny → cookie-4-sided → heart
```

The initial `rotation="20745"` (225° phase) matches `design.html`. Shape rotation and card scale are
independent: `<mdc-shape>` rotates its own path data; the card has `transform: none` for the whole
demo and is verified live in the telemetry panel.

## 100ms fast mode caveat

While validating the published `0.3.0-20261123.a` build, running the real spring at a 100ms tick
produced repeated console exceptions:

```
Error: FloatMapping - Progress repeats a value: …
  at BaseShape.startOrRetargetMorph ...
```

The component retargets a morph every 100ms while the previous morph is still in flight, and the
published FeatureMapping code throws for certain intermediate polygons. To keep the preview's
console clean, **100ms mode sets the public `disable-animation` property** (seen as `motion mode:
snap`). Shape cycling and `rotation += 45` still run through `next()`/`rotation` on every tick; only
the spring interpolation is skipped. The 500ms and 1000ms modes use the real expressive spring.

This is a component/package issue, not a preview shortcut: a page using `autoplay` with
`interval="100"` (as in `design.html`) can hit the same exception.

## Verify by hand

With the page open, run in DevTools:

```js
const badge = document.querySelector("#badge");
({
  shape: badge.activeShape,        // circle, soft-burst, …
  sequenceIndex: badge.sequenceIndex,
  rotation: badge.rotation,        // grows continuously: +45 per copy and +45°/s ambient
  phase: badge.rotation % 360,     // arbitrary phase once the ambient spin runs
  pathLength: badge.pathData.length,
  morphing: badge.isMorphing,
  cardTransform: getComputedStyle(document.querySelector("#copy-card")).transform, // "none"
});
```

Click **100 ms**, wait 1-2 seconds, and the count increases while `pathData.length` and `rotation`
change. The badge keeps idling at 45°/s; uncheck the toggle to compare against a static badge.
A screenshot is kept at [`preview.png`](preview.png).

## Files

```
preview/shape/
├── index.html        # standalone preview (Cccc card, 72px badge, 32px comparison, controls)
├── package.json      # @sandlada/mdc + esbuild, build/serve scripts
├── shape.bundle.js   # prebuilt ESM bundle (commit this)
├── preview.png       # Playwright screenshot evidence
└── README.md
```

`node_modules/` is already ignored by the repository root `.gitignore`.
