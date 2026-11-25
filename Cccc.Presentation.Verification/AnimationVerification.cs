using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Cccc.Adapters.Outbound.Toast.Wpf;
using Cccc.Application;
using Cccc.Domain.Feedback;
using Cccc.Domain.Preferences;
using Cccc.Platform.Windows;

namespace Cccc.Presentation.Verification
{
    // Only the changed fade/pulse behavior; no clipboard, tray, settings writes,
    // screenshots, material benchmarking or full regression run.
    internal static class AnimationVerification
    {
        private static readonly List<string> results = new List<string>();
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static int passed;
        private static T Field<T>(object value, string name) { return (T)value.GetType().GetField(name, Private).GetValue(value); }
        private static void Frame(ToastWindow toast)
        { typeof(ToastWindow).GetMethod("DrawFrame", Private).Invoke(toast, new object[] { null, EventArgs.Empty }); }
        private static void Verify(bool value, string name)
        { if (!value) throw new Exception(name); passed++; results.Add("PASS," + name); }
        private static bool Near(double left, double right) { return Math.Abs(left - right) < .000001; }
        private static void Pump(int milliseconds)
        {
            DispatcherFrame frame = new DispatcherFrame();
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
            timer.Tick += delegate { timer.Stop(); frame.Continue = false; }; timer.Start(); Dispatcher.PushFrame(frame);
        }
        private static FeedbackView Sample(int count)
        { return new FeedbackView { Kind = FeedbackViewKind.Confirmed, Count = count, Title = FeedbackMapper.ResolveTitle(FeedbackEngine.SuccessTitleKey(count), null), Preview = "Animation check g/y", Foreground = new WindowTarget() }; }
        private static void Fade()
        {
            WindowFade fade = new WindowFade(); fade.BeginIn(0, 1000);
            fade.Advance(1050); Verify(Near(fade.Opacity, .15625), "fadeIn200SmoothstepQuarter");
            fade.Advance(1100); Verify(Near(fade.Opacity, .5), "fadeInMidpoint");
            fade.Advance(1150); Verify(Near(fade.Opacity, .84375), "fadeInSoftFinish");
            fade.Advance(1200); Verify(fade.Opacity == 1 && !fade.Running, "fadeInFinishesAt200");
            fade.BeginOut(1, 2000); fade.Advance(2055);
            Verify(Near(fade.Opacity, .84375), "fadeOut220SmoothstepQuarter");
            fade.Advance(2110); Verify(Near(fade.Opacity, .5), "fadeOutMidpoint");
            fade.Advance(2220); Verify(fade.Opacity == 0 && !fade.Running, "fadeOutFinishesAt220");
            fade.BeginOut(1, 3000); fade.Advance(3110); fade.BeginIn(fade.Opacity, 3110);
            Verify(fade.Opacity == .5 && !fade.FadingOut, "reversalPreservesCurrentOpacity");
            fade.Advance(3160); Verify(Near(fade.Opacity, .75), "reversalSmoothstepRemainingRange");
            fade.Advance(3210); Verify(fade.Opacity == 1 && !fade.Running, "reversalScalesRemainingDuration");
            fade.BeginIn(0, 4000); double prior = 0; bool monotone = true;
            for (int offset = 0; offset <= 200; offset += 10)
            { fade.Advance(4000 + offset); monotone &= fade.Opacity >= prior && fade.Opacity <= 1; prior = fade.Opacity; }
            Verify(monotone, "fadeInMonotoneBounded");
            fade.BeginOut(1, 5000); prior = 1; monotone = true;
            for (int offset = 0; offset <= 220; offset += 10)
            { fade.Advance(5000 + offset); monotone &= fade.Opacity <= prior && fade.Opacity >= 0; prior = fade.Opacity; }
            Verify(monotone, "fadeOutMonotoneBounded");
            CopyMotion motion = new CopyMotion(); motion.Pulse(0);
            Verify(motion.Value(55) == 1 && CopyMotion.CardAmount == 0 && CopyMotion.CounterAmount == .18 && CopyMotion.Return == 155, "repeatPulseCounterOnlyAndTimingUnchanged");
            Verify(motion.Value(210) == 0 && !motion.Running, "repeatPulseFinishesAt210");

            ToastWindowMotion winMotion = new ToastWindowMotion();
            winMotion.SetDirection(ToastPosition.BottomLeft, 1.0, false);
            winMotion.SnapPosition(100, 200);
            winMotion.Reset(0);
            Verify(winMotion.RenderY > 200 && winMotion.Scale < 1.0, "popOutStartsOffsetAndScaled");
            winMotion.BeginIn(0, 1000);
            winMotion.Advance(1400);
            Verify(winMotion.Opacity == 1 && Near(winMotion.Scale, 1.0) && Near(winMotion.RenderY, 200), "popInSettlesToTarget");
            winMotion.SetTargetPosition(100, 150, 2000);
            winMotion.Advance(2050);
            Verify(winMotion.CurrentY < 200 && winMotion.CurrentY > 150, "positionSpringGlidesSmoothly");
            winMotion.Advance(2600);
            Verify(Near(winMotion.CurrentY, 150) && !winMotion.Running, "positionSpringSettles");
        }
        private static void EntryAndRepeat()
        {
            Themes.Current = new Appearance { Transparent = false }; Themes.MaterialAvailable = false;
            ToastWindow toast = new ToastWindow(false);
            Field<Grid>(toast, "surface").Opacity = 0;
            try
            {
                toast.CopyPressed(1, IntPtr.Zero, true); toast.Present(Sample(1), true);
                Verify(toast.TestPendingEntrance && !toast.TestPendingHit && !toast.TestMotionRunning && toast.Opacity == 0, "hiddenFirstCopyQueuesFadeOnly");
                Verify(toast.TestBadgeSpinRunning, "badgeSpinStartsWithCard");
                Verify(Field<SampledText>(toast, "title").BuildCount > 0, "firstTitlePreparedBeforeFade");
                Frame(toast);
                Verify(!toast.TestMotionRunning && Field<ToastWindowMotion>(toast, "windowMotion").Running, "firstFrameStartsEntranceMotion");
                Pump(450); Verify(toast.Opacity == 1 && !toast.TestFramesRunning, "firstFadeStopsCallbacks");
                Verify(toast.TestBadgeSpinAngle > 2, "badgeSpinsWithoutCopy angle=" + toast.TestBadgeSpinAngle.ToString("F1"));
                double spinBeforeCopy = toast.TestBadgeSpinAngle;
                toast.CopyPressed(2, IntPtr.Zero, true);
                Verify(toast.TestPendingHit && Field<SampledText>(toast, "times").Text == "×2", "secondKeyQueuesPulseAndPreparesCount");
                Frame(toast); Verify(toast.TestMotionRunning && toast.Opacity == 1, "secondKeyStartsPulse");
                Pump(40); Verify(toast.TestScale == 1 && toast.TestBadgeMotionRunning && toast.TestBadgeRotation > 225, "secondKeyMorphsBadgeWithoutScalingCard");
                toast.CopyPressed(3, IntPtr.Zero, true); toast.Present(Sample(3), true);
                Verify(toast.TestPendingHit && toast.Current.Count == 3, "thirdKeyKeepsRepeatPulse");
                Frame(toast); Pump(600);
                Verify(toast.TestScale == 1 && !toast.TestBadgeMotionRunning && !toast.TestFramesRunning, "repeatSettlesAndStopsCallbacks");
                Verify(toast.TestBadgeSpinAngle - spinBeforeCopy > 15, "badgeSpinSurvivesRepeatedCopies delta=" + (toast.TestBadgeSpinAngle - spinBeforeCopy).ToString("F1"));

                toast.HideAndClear();
                Verify(!toast.TestBadgeSpinRunning && toast.TestBadgeSpinAngle == 0, "hideStopsBadgeSpin");
                toast.CopyPressed(4, IntPtr.Zero, true); toast.Present(Sample(4), true); Frame(toast);
                Verify(toast.TestBadgeSpinRunning, "badgeSpinRestartsWithCard");
                Verify(!toast.TestMotionRunning && toast.TestScale == 1 && !toast.TestBadgeMotionRunning, "hiddenReappearanceFadeOnlyEvenWhenCountCarries");
                toast.HideAndClear();
                toast.CopyPressed(1, IntPtr.Zero, true); toast.Present(Sample(1), true);
                toast.CopyPressed(2, IntPtr.Zero, true); toast.Present(Sample(2), true);
                Verify(toast.TestPendingHit && toast.Current.Count == 2, "secondKeyBeforeFirstFrameMayPulse");
                toast.HideAndClear(); Frame(toast);
                Verify(!toast.TestPendingHit && !toast.TestPendingEntrance && !toast.TestFramesRunning, "hideCancelsQueuedRepeat");

                toast.Present(new FeedbackView { Kind = FeedbackViewKind.Status, Title = "Cccc 已启动", Preview = "状态提示" }, true); Frame(toast);
                toast.CopyPressed(1, IntPtr.Zero, true); toast.Present(Sample(1), true);
                Verify(!toast.TestPendingHit && !toast.TestMotionRunning, "firstKeyOverStatusDoesNotPulse");
                Frame(toast); Pump(240);
                ToastWindowMotion winMotion = Field<ToastWindowMotion>(toast, "windowMotion");
                toast.Opacity = .4; winMotion.BeginOut(.4, WindowsNativeMethods.MonotonicMilliseconds);
                toast.CopyPressed(2, IntPtr.Zero, true); Frame(toast);
                Verify(!winMotion.FadingOut && Near(toast.Opacity, .4) && toast.TestMotionRunning, "repeatDuringFadeOutRestoresWithoutJump");
                toast.HideAndClear();
            }
            finally { toast.Close(); }
        }
        private static void StatusHandoff()
        {
            Themes.Current = new Appearance { Transparent = false }; Themes.MaterialAvailable = false;
            ToastWindow toast = new ToastWindow(false);
            Field<Grid>(toast, "surface").Opacity = 0;
            try
            {
                FeedbackViewKind[] kinds = { FeedbackViewKind.Confirmed, FeedbackViewKind.Retained, FeedbackViewKind.Unconfirmed, FeedbackViewKind.Updated };
                foreach (FeedbackViewKind kind in kinds)
                {
                    toast.HideAndClear();
                    toast.Present(new FeedbackView { Kind = FeedbackViewKind.Status, Title = "高级材质已开启", Preview = "设置提示" }, true);
                    Frame(toast);
                    for (int count = 1; count <= 3; count++)
                    {
                        toast.CopyPressed(count, IntPtr.Zero, true);
                        Frame(toast);
                        Verify(toast.Current.Kind == FeedbackViewKind.Status && toast.Current.Count == 0 &&
                            Field<SampledText>(toast, "title").Text == "高级材质已开启" &&
                            Field<SampledText>(toast, "detail").Text == "设置提示" &&
                            Field<Border>(toast, "counter").Visibility == Visibility.Hidden && !toast.TestPendingHit && !toast.TestMotionRunning,
                            "statusUnchangedWhileWaitingForCopyResult count=" + count + " kind=" + kind +
                            " actualCount=" + toast.Current.Count + " title=" + Field<SampledText>(toast, "title").Text);
                    }
                    FeedbackView result = new FeedbackView { Kind = kind, Count = 3, Title = "复制结果 " + kind, Preview = "真实预览" };
                    toast.Present(result, true);
                    Verify(toast.Current.Kind == kind && toast.Current.Count == 3 &&
                        Field<SampledText>(toast, "title").Text == result.Title &&
                        Field<SampledText>(toast, "detail").Text == result.Preview &&
                        Field<SampledText>(toast, "times").Text == "×3" &&
                        Field<Border>(toast, "counter").Visibility == Visibility.Visible && !toast.TestPendingHit,
                        "copyResultImmediatelyReplacesStatus kind=" + kind + " title=" + Field<SampledText>(toast, "title").Text);
                    Frame(toast);
                    Verify(!toast.TestMotionRunning, "firstCopyResultOverStatusDoesNotPulse kind=" + kind);
                    toast.CopyPressed(4, IntPtr.Zero, true);
                    Verify(toast.TestPendingHit && toast.Current.Count == 4, "subsequentCopyPulses kind=" + kind);
                }
            }
            finally { toast.Close(); }
        }
        private static void RapidBurst()
        {
            Themes.Current = new Appearance { Transparent = false }; Themes.MaterialAvailable = false;
            ToastWindow toast = new ToastWindow(false);
            try
            {
                toast.CopyPressed(1, IntPtr.Zero, true); toast.Present(Sample(1), true);
                Frame(toast); Pump(240);
                for (int count = 2; count <= 20; count++)
                {
                    toast.CopyPressed(count, IntPtr.Zero, true);
                    toast.Present(Sample(count), true);
                    Frame(toast);
                    Pump(100);
                }
                Verify(toast.TestScale == 1, "rapidBurstKeepsCardUnscaled");
                Verify(toast.TestBadgeRotation > 225, "rapidBurstAdvancesBadge");
                Verify(toast.TestBadgeCubics > 0, "rapidBurstKeepsBadgeGeometry");
                Verify(toast.TestBadgeSpinRunning && toast.TestBadgeSpinAngle > 0, "rapidBurstKeepsSpin angle=" + toast.TestBadgeSpinAngle.ToString("F1"));
                Pump(600);
                Verify(!toast.TestFramesRunning, "rapidBurstSettles");
                toast.HideAndClear();
                Verify(toast.TestBadgeRotation == 225, "rapidBurstResetsBadge");
                Verify(!toast.TestBadgeSpinRunning && toast.TestBadgeSpinAngle == 0, "rapidBurstStopsSpin");
            }
            finally { toast.Close(); }
        }

        private static void PreviewHooks()
        {
            Themes.Current = new Appearance { Transparent = false }; Themes.MaterialAvailable = false;
            ToastWindow toast = new ToastWindow(false);
            try
            {
                Point requested = new Point(160, 180);
                toast.PreviewPositionProvider = size => requested;
                toast.PreviewKeepVisible = true;
                toast.Present(Sample(1), true);
                Frame(toast);
                Pump(100);
                WindowsNativeMethods.WindowRectangle bounds;
                int dpi = WindowsNativeMethods.GetWindowDpi(toast.Handle);
                double scale = dpi / 96.0;
                Verify(WindowsNativeMethods.GetWindowRect(toast.Handle, out bounds) &&
                    Math.Abs(bounds.Left - Math.Round(requested.X * scale)) <= 1 &&
                    Math.Abs(bounds.Top - Math.Round(requested.Y * scale)) <= 1,
                    "previewPositionProviderOverridesToastLocation");
                toast.PreviewPositionProvider = null;
                Pump(2300);
                Verify(toast.IsVisible && toast.Opacity > .99, "previewKeepVisibleSuppressesAutoHide");
                toast.PreviewKeepVisible = false;
                toast.Present(Sample(1), true);
                Pump(2300);
                Verify(!toast.IsVisible, "previewAutoHideRestartsAfterKeepVisibleDisabled");
            }
            finally { toast.Close(); }
        }

        private static void CounterExpand()
        {
            CounterExpandMotion expand = new CounterExpandMotion();
            expand.Snap(0);
            Verify(expand.Progress == 0 && !expand.Running, "counterExpandStartsCollapsed");
            expand.AnimateTo(1, 1000);
            Verify(expand.Running, "counterExpandStartsOnRepeat");
            expand.Advance(1000 + CounterExpandMotion.Duration / 2);
            Verify(expand.Progress > 0.5 && expand.Progress < 1, "counterExpandEaseOutMidpoint");
            expand.Advance(1000 + CounterExpandMotion.Duration);
            Verify(expand.Progress == 1 && !expand.Running, "counterExpandFinishesAtDuration");

            Themes.Current = new Appearance { Transparent = false }; Themes.MaterialAvailable = false;
            ToastProfile.SetCompact(false);
            ToastWindow toast = new ToastWindow(false);
            Field<Grid>(toast, "surface").Opacity = 0;
            try
            {
                toast.CopyPressed(1, IntPtr.Zero, true);
                toast.Present(Sample(1), true);
                Frame(toast);
                Border card = Field<Border>(toast, "card");
                double singleWidth = card.Width;
                Verify(singleWidth < 310, "singleCopyLeavesNoCounterSpace width=" + singleWidth);

                Pump(220);
                toast.CopyPressed(2, IntPtr.Zero, true);
                Frame(toast);
                Pump(80);
                Verify(card.Width > singleWidth && card.Width < 360, "repeatCopyExpandsHorizontally width=" + card.Width);
                Pump(250);
                Verify(Near(card.Width, 360) && Field<Border>(toast, "counter").Opacity == 1, "repeatCopySettlesExpanded");
            }
            finally { toast.Close(); }

            ToastWindow rightToast = new ToastWindow(false);
            Field<Grid>(rightToast, "surface").Opacity = 0;
            try
            {
                rightToast.ApplyPersonalization(new PreferencesModel { Position = WindowPosition.BottomRight });
                rightToast.CopyPressed(1, IntPtr.Zero, true);
                rightToast.Present(Sample(1), true);
                Frame(rightToast);
                Pump(500);
                IntPtr handle = Field<IntPtr>(rightToast, "windowHandle");
                WindowsNativeMethods.WindowRectangle singleBounds;
                Verify(WindowsNativeMethods.GetWindowRect(handle, out singleBounds), "rightAlignedSingleBoundsAvailable");

                rightToast.CopyPressed(2, IntPtr.Zero, true);
                Frame(rightToast);
                Pump(350);
                WindowsNativeMethods.WindowRectangle expandedBounds;
                Verify(WindowsNativeMethods.GetWindowRect(handle, out expandedBounds), "rightAlignedExpandedBoundsAvailable");
                Verify(expandedBounds.Left < singleBounds.Left, "rightAlignedExpandShiftsWindowLeft singleLeft=" + singleBounds.Left + " expandedLeft=" + expandedBounds.Left);
            }
            finally { rightToast.Close(); }
        }

        internal static void Run(string outputDirectory)
        {
            Fade();
            EntryAndRepeat();
            RapidBurst();
            StatusHandoff();
            PreviewHooks();
            CounterExpand();
            results.Add("TOTAL," + passed);
            File.WriteAllLines(Path.Combine(outputDirectory, "animation-check-results.csv"), results.ToArray());
            Console.WriteLine("Animation checks passed: " + passed);
        }
    }
}
