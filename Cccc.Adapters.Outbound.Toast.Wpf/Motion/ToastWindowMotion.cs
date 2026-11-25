using System;
using Cccc.Domain.Preferences;
using Cccc.MaterialShape;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal sealed class ToastWindowMotion
    {
        private const double SlideDistanceDip = 18.0;
        private const double MinScale = 0.94;
        private const double PositionSettleEpsilon = 0.25;
        private const double PopSettleEpsilon = 0.002;

        private static readonly SpringParameters PositionSpring = new(420, 0.95);
        private static readonly SpringParameters PopSpring = new(560, 0.88);

        private readonly WindowFade fade = new();

        private double posStartX, posVelX, posTargetX, currentX;
        private double posStartY, posVelY, posTargetY, currentY;
        private long posStartedAt;
        private bool posRunning;

        private double popStart, popVel, popTarget, popProgress;
        private long popStartedAt;
        private bool popRunning;

        private double slideDirX, slideDirY = 1;
        private double currentScale = 1.0;

        internal double Opacity => fade.Opacity;
        internal bool FadingOut => fade.FadingOut;
        internal bool FadeRunning => fade.Running;
        internal double CurrentX => currentX;
        internal double CurrentY => currentY;
        internal double TargetX => posTargetX;
        internal double TargetY => posTargetY;
        internal double PopProgress => popProgress;
        internal double Scale { get; private set; } = 1.0;
        internal double RenderX => currentX + (1.0 - popProgress) * SlideDistanceDip * currentScale * slideDirX;
        internal double RenderY => currentY + (1.0 - popProgress) * SlideDistanceDip * currentScale * slideDirY;
        internal bool Running => fade.Running || posRunning || popRunning;

        internal void SetDirection(ToastPosition position, double scale, bool compact)
        {
            currentScale = scale > 0 ? scale : 1.0;
            if (compact)
            {
                slideDirX = 0;
                slideDirY = 0;
                return;
            }
            ScreenPoint vec = ToastPlacementPolicy.GetEntranceSlideVector(position);
            slideDirX = vec.X;
            slideDirY = vec.Y;
        }

        internal void SnapPosition(double x, double y)
        {
            currentX = posStartX = posTargetX = x;
            currentY = posStartY = posTargetY = y;
            posVelX = posVelY = 0;
            posRunning = false;
        }

        internal void SetTargetPosition(double x, double y, long now)
        {
            if (Math.Abs(posTargetX - x) < 0.25 && Math.Abs(posTargetY - y) < 0.25)
                return;

            posStartX = currentX;
            posStartY = currentY;
            posTargetX = x;
            posTargetY = y;
            posStartedAt = now;
            posRunning = true;
        }

        internal void Reset(double opacityValue)
        {
            fade.Reset(opacityValue);
            popProgress = popStart = popTarget = opacityValue > 0.5 ? 1.0 : 0.0;
            popVel = 0;
            popRunning = false;
            Scale = MinScale + (1.0 - MinScale) * popProgress;
        }

        internal void BeginIn(double currentOpacity, long now)
        {
            fade.BeginIn(currentOpacity, now);
            popStart = popProgress;
            popTarget = 1.0;
            popStartedAt = now;
            popRunning = Math.Abs(popTarget - popStart) > PopSettleEpsilon || Math.Abs(popVel) > PopSettleEpsilon;
            if (!popRunning)
            {
                popProgress = 1.0;
                Scale = 1.0;
            }
        }

        internal void BeginOut(double currentOpacity, long now)
        {
            fade.BeginOut(currentOpacity, now);
            popStart = popProgress;
            popTarget = 0.0;
            popStartedAt = now;
            popRunning = Math.Abs(popTarget - popStart) > PopSettleEpsilon || Math.Abs(popVel) > PopSettleEpsilon;
            if (!popRunning)
            {
                popProgress = 0.0;
                Scale = MinScale;
            }
        }

        internal void Advance(long now)
        {
            fade.Advance(now);

            if (posRunning)
            {
                double elapsedMs = Math.Max(0, now - posStartedAt);
                SpringState sx = Spring.MotionAt(PositionSpring, posStartX, posVelX, elapsedMs, posTargetX);
                SpringState sy = Spring.MotionAt(PositionSpring, posStartY, posVelY, elapsedMs, posTargetY);
                currentX = sx.Value;
                posVelX = sx.Velocity;
                currentY = sy.Value;
                posVelY = sy.Velocity;

                if (Math.Abs(posTargetX - currentX) <= PositionSettleEpsilon &&
                    Math.Abs(posTargetY - currentY) <= PositionSettleEpsilon &&
                    Math.Abs(posVelX) <= 2.0 && Math.Abs(posVelY) <= 2.0)
                {
                    currentX = posTargetX;
                    currentY = posTargetY;
                    posVelX = posVelY = 0;
                    posRunning = false;
                }
            }

            if (popRunning)
            {
                double elapsedMs = Math.Max(0, now - popStartedAt);
                SpringState sp = Spring.MotionAt(PopSpring, popStart, popVel, elapsedMs, popTarget);
                popProgress = Math.Clamp(sp.Value, -0.05, 1.08);
                popVel = sp.Velocity;

                if (Math.Abs(popTarget - sp.Value) <= PopSettleEpsilon && Math.Abs(popVel) <= 0.05)
                {
                    popProgress = popTarget;
                    popVel = 0;
                    popRunning = false;
                }
            }

            Scale = MinScale + (1.0 - MinScale) * popProgress;
        }
    }
}
