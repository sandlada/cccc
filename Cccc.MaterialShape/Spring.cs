using System;

namespace Cccc.MaterialShape
{
    /// <summary>
    /// Analytic one-mass spring solution and Compose-compatible duration
    /// estimation. Ported from <c>SpringSimulation.kt</c>.
    /// </summary>
    public static class Spring
    {
        private const double MaxLongMillis = 9007199254740991.0;

        private static void AssertParameters(SpringParameters spring)
        {
            if (!double.IsFinite(spring.Stiffness) || !(spring.Stiffness > 0))
                throw new ArgumentException("Spring stiffness constant must be positive.", nameof(spring));
            if (!double.IsFinite(spring.DampingRatio) || !(spring.DampingRatio >= 0))
                throw new ArgumentException("Damping ratio must be non-negative", nameof(spring));
        }

        /// <summary>Evaluates the analytic solution of a 1-mass spring.</summary>
        public static SpringState MotionAt(
            SpringParameters spring,
            double lastValue,
            double lastVelocity,
            double timeElapsedMs,
            double finalPosition)
        {
            AssertParameters(spring);
            double naturalFrequency = Math.Sqrt(spring.Stiffness);
            double dampingRatio = spring.DampingRatio;
            double adjustedDisplacement = lastValue - finalPosition;
            double deltaT = Math.Max(0, timeElapsedMs) / 1000;
            double dampingRatioSquared = dampingRatio * dampingRatio;
            double r = -dampingRatio * naturalFrequency;

            double displacement;
            double currentVelocity;

            if (dampingRatio > 1)
            {
                double s = naturalFrequency * Math.Sqrt(dampingRatioSquared - 1);
                double gammaPlus = r + s;
                double gammaMinus = r - s;
                double coeffB = (gammaMinus * adjustedDisplacement - lastVelocity) / (gammaMinus - gammaPlus);
                double coeffA = adjustedDisplacement - coeffB;
                displacement = coeffA * Math.Exp(gammaMinus * deltaT) + coeffB * Math.Exp(gammaPlus * deltaT);
                currentVelocity =
                    coeffA * gammaMinus * Math.Exp(gammaMinus * deltaT) +
                    coeffB * gammaPlus * Math.Exp(gammaPlus * deltaT);
            }
            else if (dampingRatio == 1)
            {
                double coeffA = adjustedDisplacement;
                double coeffB = lastVelocity + naturalFrequency * adjustedDisplacement;
                double nFdT = -naturalFrequency * deltaT;
                displacement = (coeffA + coeffB * deltaT) * Math.Exp(nFdT);
                currentVelocity =
                    (coeffA + coeffB * deltaT) * Math.Exp(nFdT) * -naturalFrequency +
                    coeffB * Math.Exp(nFdT);
            }
            else
            {
                double dampedFrequency = naturalFrequency * Math.Sqrt(1 - dampingRatioSquared);
                double cosCoeff = adjustedDisplacement;
                double sinCoeff = (1 / dampedFrequency) * (-r * adjustedDisplacement + lastVelocity);
                double dFdT = dampedFrequency * deltaT;
                displacement = Math.Exp(r * deltaT) *
                    (cosCoeff * Math.Cos(dFdT) + sinCoeff * Math.Sin(dFdT));
                currentVelocity =
                    displacement * r +
                    Math.Exp(r * deltaT) *
                        (-dampedFrequency * cosCoeff * Math.Sin(dFdT) +
                         dampedFrequency * sinCoeff * Math.Cos(dFdT));
            }

            return new SpringState(displacement + finalPosition, currentVelocity);
        }

        /// <summary>
        /// Estimated time (whole milliseconds, truncated) the spring lasts until
        /// it stays within <paramref name="delta"/> of the final position.
        /// </summary>
        public static double EstimateAnimationDurationMilliseconds(
            SpringParameters spring,
            double initialVelocity,
            double initialDisplacement,
            double delta)
        {
            AssertParameters(spring);
            if (spring.DampingRatio == 0) return MaxLongMillis;

            double dampingCoefficient = 2 * spring.DampingRatio * Math.Sqrt(spring.Stiffness);
            double partialRoot = dampingCoefficient * dampingCoefficient - 4 * spring.Stiffness;
            double partialRootReal = partialRoot < 0 ? 0 : Math.Sqrt(partialRoot);
            double partialRootImaginary = partialRoot < 0 ? Math.Sqrt(Math.Abs(partialRoot)) : 0;
            double firstRootReal = (-dampingCoefficient + partialRootReal) * 0.5;
            double firstRootImaginary = partialRootImaginary * 0.5;
            double secondRootReal = (-dampingCoefficient - partialRootReal) * 0.5;

            return EstimateDurationInternal(
                firstRootReal,
                firstRootImaginary,
                secondRootReal,
                spring.DampingRatio,
                initialVelocity,
                initialDisplacement,
                delta);
        }

        private static double EstimateDurationInternal(
            double firstRootReal,
            double firstRootImaginary,
            double secondRootReal,
            double dampingRatio,
            double initialVelocity,
            double initialPosition,
            double delta)
        {
            if (initialPosition == 0 && initialVelocity == 0) return 0;
            double v0 = initialPosition < 0 ? -initialVelocity : initialVelocity;
            double p0 = Math.Abs(initialPosition);

            double seconds =
                dampingRatio > 1
                    ? EstimateOverDamped(firstRootReal, secondRootReal, p0, v0, delta)
                    : dampingRatio < 1
                        ? EstimateUnderDamped(firstRootReal, firstRootImaginary, p0, v0, delta)
                        : EstimateCriticallyDamped(firstRootReal, p0, v0, delta);

            return Math.Truncate(seconds * 1000);
        }

        private static double IterateNewtonsMethod(double x, Func<double, double> function, Func<double, double> derivative)
        {
            return x - function(x) / derivative(x);
        }

        private static double EstimateUnderDamped(
            double firstRootReal,
            double firstRootImaginary,
            double p0,
            double v0,
            double delta)
        {
            double r = firstRootReal;
            double c1 = p0;
            double c2 = (v0 - r * c1) / firstRootImaginary;
            double c = Math.Sqrt(c1 * c1 + c2 * c2);
            return Math.Log(delta / c) / r;
        }

        private static double EstimateCriticallyDamped(
            double firstRootReal,
            double p0,
            double v0,
            double delta)
        {
            double r = firstRootReal;
            double c1 = p0;
            double c2 = v0 - r * c1;

            double t1 = Math.Log(Math.Abs(delta / c1)) / r;
            double guess = Math.Log(Math.Abs(delta / c2));
            double t = guess;
            for (int i = 0; i <= 5; i++) t = guess - Math.Log(Math.Abs(t / r));
            double t2 = t / r;

            double tCurrent = double.IsFinite(t1)
                ? (double.IsFinite(t2) ? Math.Max(t1, t2) : t1)
                : t2;

            double tInflection = -(r * c1 + c2) / (r * c2);
            double xInflection = c1 * Math.Exp(r * tInflection) + c2 * tInflection * Math.Exp(r * tInflection);

            double signedDelta;
            if (double.IsNaN(tInflection) || tInflection <= 0)
            {
                signedDelta = -delta;
            }
            else if (-xInflection < delta)
            {
                if (c2 < 0 && c1 > 0) tCurrent = 0;
                signedDelta = -delta;
            }
            else
            {
                tCurrent = -(2 / r) - c1 / c2;
                signedDelta = delta;
            }

            double tDelta = double.MaxValue;
            int iterations = 0;
            while (tDelta > 0.001 && iterations < 100)
            {
                iterations++;
                double tLast = tCurrent;
                tCurrent = IterateNewtonsMethod(
                    tCurrent,
                    time => (c1 + c2 * time) * Math.Exp(r * time) + signedDelta,
                    time => (c2 * (r * time + 1) + c1 * r) * Math.Exp(r * time));
                tDelta = Math.Abs(tLast - tCurrent);
            }
            return tCurrent;
        }

        private static double EstimateOverDamped(
            double firstRootReal,
            double secondRootReal,
            double p0,
            double v0,
            double delta)
        {
            double r1 = firstRootReal;
            double r2 = secondRootReal;
            double c2 = (r1 * p0 - v0) / (r1 - r2);
            double c1 = p0 - c2;

            double t1 = Math.Log(Math.Abs(delta / c1)) / r1;
            double t2 = Math.Log(Math.Abs(delta / c2)) / r2;
            double tCurrent = double.IsFinite(t1)
                ? (double.IsFinite(t2) ? Math.Max(t1, t2) : t1)
                : t2;

            double tInflection = Math.Log((c1 * r1) / (-c2 * r2)) / (r2 - r1);
            double InflectionValue()
            {
                return c1 * Math.Exp(r1 * tInflection) + c2 * Math.Exp(r2 * tInflection);
            }

            double signedDelta;
            if (double.IsNaN(tInflection) || tInflection <= 0)
            {
                signedDelta = -delta;
            }
            else if (-InflectionValue() < delta)
            {
                if (c2 > 0 && c1 < 0) tCurrent = 0;
                signedDelta = -delta;
            }
            else
            {
                tCurrent = Math.Log(-(c2 * r2 * r2) / (c1 * r1 * r1)) / (r1 - r2);
                signedDelta = delta;
            }

            if (Math.Abs(c1 * r1 * Math.Exp(r1 * tCurrent) + c2 * r2 * Math.Exp(r2 * tCurrent)) < 0.0001)
                return tCurrent;

            double tDelta = double.MaxValue;
            int iterations = 0;
            while (tDelta > 0.001 && iterations < 100)
            {
                iterations++;
                double tLast = tCurrent;
                tCurrent = IterateNewtonsMethod(
                    tCurrent,
                    time => c1 * Math.Exp(r1 * time) + c2 * Math.Exp(r2 * time) + signedDelta,
                    time => c1 * r1 * Math.Exp(r1 * time) + c2 * r2 * Math.Exp(r2 * time));
                tDelta = Math.Abs(tLast - tCurrent);
            }
            return tCurrent;
        }
    }
}
