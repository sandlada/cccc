using System;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Interruptible shape-morph state machine driven by an analytic spring.
    /// Port of the morph / rotation portions of the TypeScript
    /// <c>BaseShape.stepAnimation</c>, <c>startOrRetargetMorph</c> and
    /// <c>handleRotationTargetChange</c> logic.
    /// </summary>
    /// <remarks>
    /// Time is supplied by the caller in milliseconds and must be monotonic.
    /// The animator is not thread-safe; it is intended to be driven from the
    /// UI thread only.
    /// </remarks>
    public sealed class MorphAnimator
    {
        private const double MorphSettlePositionThreshold = 0.0008;
        private const double MorphSettleVelocityThreshold = 0.008;
        private const double RotationSettlePositionThreshold = 0.05;
        private const double RotationSettleVelocityThreshold = 0.5;

        private readonly SpringParameters spring;

        private RoundedPolygon settledShape;
        private Morph activeMorph;
        private int morphSpringTarget;
        private double morphStartValue;
        private double morphStartVelocity;
        private double morphValue;
        private double morphVelocity;
        private double morphStartTime;

        private double currentRotation;
        private double rotationTarget;
        private double rotationStartValue;
        private double rotationStartVelocity;
        private double rotationVelocity;
        private double rotationStartTime;
        private bool isRotationRunning;

        public MorphAnimator(RoundedPolygon restShape, SpringParameters spring, double rotationDegrees = 0)
        {
            settledShape = restShape ?? throw new ArgumentNullException(nameof(restShape));
            this.spring = spring;
            currentRotation = rotationDegrees;
            rotationTarget = rotationDegrees;
            morphValue = 1;
        }

        /// <summary>The polygon currently at rest (never a mid-morph snapshot).</summary>
        public RoundedPolygon SettledShape { get { return settledShape; } }

        /// <summary>
        /// The endpoint the active spring is heading for, or
        /// <see cref="SettledShape"/> when no morph is running.
        /// </summary>
        public RoundedPolygon CurrentTarget
        {
            get
            {
                if (activeMorph == null) return settledShape;
                return morphSpringTarget == 1 ? activeMorph.EndPolygon : activeMorph.StartPolygon;
            }
        }

        /// <summary>Current animated rotation in degrees.</summary>
        public double RotationDegrees { get { return currentRotation; } }

        /// <summary>
        /// Safety budget for snapshot retargeting. A rapid third-shape trigger
        /// can otherwise grow the curve count without bound; above this value the
        /// animator snaps to the target instead of keeping the intermediate
        /// snapshot. The caller's sample buffer must be at least this large plus
        /// one (the badge view uses 1024).
        /// </summary>
        public int MaxCurveCount { get; set; } = 512;

        /// <summary>Whether the morph or the rotation spring still has work to do.</summary>
        public bool IsRunning { get { return activeMorph != null || isRotationRunning; } }

        /// <summary>
        /// Immediately places the animator at rest on <paramref name="shape"/>
        /// with the given rotation, cancelling any in-flight animation.
        /// </summary>
        public void SnapTo(RoundedPolygon shape, double rotationDegrees = 0)
        {
            if (shape == null) throw new ArgumentNullException(nameof(shape));

            settledShape = shape;
            activeMorph = null;
            morphSpringTarget = 1;
            morphStartValue = 1;
            morphStartVelocity = 0;
            morphValue = 1;
            morphVelocity = 0;
            morphStartTime = 0;

            currentRotation = rotationDegrees;
            rotationTarget = rotationDegrees;
            rotationStartValue = rotationDegrees;
            rotationStartVelocity = 0;
            rotationVelocity = 0;
            rotationStartTime = 0;
            isRotationRunning = false;
        }

        /// <summary>
        /// Starts, reverses, or retargets the morphology towards
        /// <paramref name="target"/> and adds <paramref name="rotationDelta"/>
        /// degrees to the rotation target.
        /// </summary>
        public void Trigger(RoundedPolygon target, double rotationDelta, double nowMs)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (activeMorph == null)
            {
                if (!ReferenceEquals(target, settledShape))
                {
                    Morph next = new Morph(settledShape, target);
                    if (next.CurveCount > MaxCurveCount)
                    {
                        SnapTo(target, rotationTarget + rotationDelta);
                        return;
                    }
                    activeMorph = next;
                    morphSpringTarget = 1;
                    morphStartValue = 0;
                    morphStartVelocity = 0;
                    morphValue = 0;
                    morphVelocity = 0;
                    morphStartTime = nowMs;
                }
            }
            else if (ReferenceEquals(target, activeMorph.StartPolygon))
            {
                if (morphSpringTarget != 0)
                {
                    // Reverse back to the start of the active morph. Resolving at
                    // nowMs keeps both position and velocity continuous even when
                    // the caller has not advanced to this timestamp yet.
                    ResolveMorphAt(nowMs);
                    morphStartValue = morphValue;
                    morphStartVelocity = morphVelocity;
                    morphSpringTarget = 0;
                    morphStartTime = nowMs;
                }
            }
            else if (!ReferenceEquals(target, activeMorph.EndPolygon))
            {
                // Third shape mid-flight: snapshot the exact intermediate
                // outline and morph from it, restarting the spring at zero.
                ResolveMorphAt(nowMs);
                RoundedPolygon snapshot = activeMorph.ToPolygon(morphValue);
                Morph next = new Morph(snapshot, target);
                if (next.CurveCount > MaxCurveCount)
                {
                    SnapTo(target, rotationTarget + rotationDelta);
                    return;
                }
                activeMorph = next;
                morphSpringTarget = 1;
                morphStartValue = 0;
                morphStartVelocity = 0;
                morphValue = 0;
                morphVelocity = 0;
                morphStartTime = nowMs;
            }
            else if (morphSpringTarget != 1)
            {
                // The caller flipped back to the original end while the morph
                // was still reversing; continue forward from the live state.
                ResolveMorphAt(nowMs);
                morphStartValue = morphValue;
                morphStartVelocity = morphVelocity;
                morphSpringTarget = 1;
                morphStartTime = nowMs;
            }

            RetargetRotation(rotationDelta, nowMs);
        }

        /// <summary>
        /// Evaluates both springs at <paramref name="nowMs"/> and snaps them
        /// exactly onto their targets once they are within threshold.
        /// Returns whether either spring is still running.
        /// </summary>
        public bool Advance(double nowMs)
        {
            if (activeMorph != null)
            {
                SpringState state = Spring.MotionAt(
                    spring,
                    morphStartValue,
                    morphStartVelocity,
                    nowMs - morphStartTime,
                    morphSpringTarget);
                morphValue = state.Value;
                morphVelocity = state.Velocity;

                if (Math.Abs(morphValue - morphSpringTarget) < MorphSettlePositionThreshold &&
                    Math.Abs(morphVelocity) < MorphSettleVelocityThreshold)
                {
                    settledShape = morphSpringTarget == 1 ? activeMorph.EndPolygon : activeMorph.StartPolygon;
                    activeMorph = null;
                    morphValue = morphSpringTarget;
                    morphVelocity = 0;
                }
            }

            if (isRotationRunning)
            {
                SpringState state = Spring.MotionAt(
                    spring,
                    rotationStartValue,
                    rotationStartVelocity,
                    nowMs - rotationStartTime,
                    rotationTarget);
                currentRotation = state.Value;
                rotationVelocity = state.Velocity;

                if (Math.Abs(currentRotation - rotationTarget) < RotationSettlePositionThreshold &&
                    Math.Abs(rotationVelocity) < RotationSettleVelocityThreshold)
                {
                    currentRotation = rotationTarget;
                    rotationVelocity = 0;
                    isRotationRunning = false;
                }
            }

            return IsRunning;
        }

        /// <summary>
        /// Writes the current outline into <paramref name="destination"/> and
        /// returns the number of cubics written. Allocation-free after the
        /// animator has been constructed.
        /// </summary>
        public int Sample(Span<Cubic> destination)
        {
            if (activeMorph != null)
                return activeMorph.Sample(morphValue, destination);

            Cubic[] cubics = settledShape.Cubics;
            if (destination.Length < cubics.Length)
                throw new ArgumentException(
                    "Destination is smaller than the settled shape's curve count.",
                    nameof(destination));
            cubics.AsSpan().CopyTo(destination);
            return cubics.Length;
        }

        private void ResolveMorphAt(double nowMs)
        {
            if (activeMorph == null) return;
            SpringState state = Spring.MotionAt(
                spring,
                morphStartValue,
                morphStartVelocity,
                nowMs - morphStartTime,
                morphSpringTarget);
            morphValue = state.Value;
            morphVelocity = state.Velocity;
        }

        private void RetargetRotation(double rotationDelta, double nowMs)
        {
            if (rotationDelta == 0 && !isRotationRunning) return;

            if (isRotationRunning)
            {
                // Resolve the current spring position before changing the
                // target so the rotation stays C1 continuous even when the
                // caller has not advanced to nowMs yet.
                SpringState state = Spring.MotionAt(
                    spring,
                    rotationStartValue,
                    rotationStartVelocity,
                    nowMs - rotationStartTime,
                    rotationTarget);
                currentRotation = state.Value;
                rotationVelocity = state.Velocity;
            }

            rotationTarget += rotationDelta;
            rotationStartValue = currentRotation;
            rotationStartVelocity = rotationVelocity;
            rotationStartTime = nowMs;
            isRotationRunning = true;
        }
    }
}
