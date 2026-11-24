using Cccc.Domain.Feedback;
using Xunit;

namespace Cccc.Domain.Tests
{
    public sealed class FeedbackEngineTests
    {
        [Fact]
        public void Null_observation_is_ignored()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(1));
            Assert.Null(engine.Observe(null, 1, 1, new WindowIdentity(1, 9)));
        }

        [Fact]
        public void Verified_copy_is_confirmed()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(1));
            engine.CopyPressed(0, new WindowIdentity(1, 9), new ClipboardSequence(1));

            var result = engine.Observe(new()
            {
                Sequence = new ClipboardSequence(2),
                OwnerProcess = 9,
                Supported = true,
                Preview = "text",
                TypeLabel = "text"
            }, 10, 10, new WindowIdentity(1, 9));

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Equal(FeedbackKind.Confirmed, result.Kind);
        }

        [Fact]
        public void Duplicate_sequence_is_ignored()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(1));
            engine.CopyPressed(0, new WindowIdentity(1, 9), new ClipboardSequence(1));
            var snapshot = new ClipboardSnapshot
            {
                Sequence = new ClipboardSequence(2),
                OwnerProcess = 9,
                Supported = true
            };
            engine.Observe(snapshot, 10, 10, new WindowIdentity(1, 9));
            Assert.Null(engine.Observe(snapshot, 11, 11, new WindowIdentity(1, 9)));
        }

        [Fact]
        public void Confirmation_deadline_is_exact()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(1));
            engine.CopyPressed(0, new WindowIdentity(1, 9), new ClipboardSequence(1));
            engine.Observe(new()
            {
                Sequence = new ClipboardSequence(2),
                OwnerProcess = 9,
                Supported = true
            }, 10, 10, new WindowIdentity(1, 9));
            engine.CopyPressed(100, new WindowIdentity(1, 9), new ClipboardSequence(2));

            Assert.Null(engine.Expire(1599, new ClipboardSequence(2)));
            var result = engine.Expire(1600, new ClipboardSequence(2));
            Assert.NotNull(result);
            Assert.Equal(FeedbackKind.Retained, result.Kind);
        }

        [Fact]
        public void Changed_clipboard_expires_unconfirmed()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(2));
            engine.CopyPressed(2000, new WindowIdentity(1, 9), new ClipboardSequence(2));

            var result = engine.Expire(3500, new ClipboardSequence(2));
            Assert.NotNull(result);
            Assert.Equal(FeedbackKind.Unconfirmed, result.Kind);
        }

        [Fact]
        public void Late_read_remembers_earlier_confirmation_without_claiming_latest_press()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(1));
            engine.CopyPressed(100, new WindowIdentity(1, 9), new ClipboardSequence(1));
            engine.CopyPressed(120, new WindowIdentity(1, 9), new ClipboardSequence(1));

            var late = engine.Observe(new()
            {
                Sequence = new ClipboardSequence(2),
                OwnerProcess = 9,
                Supported = true
            }, 50, 130, new WindowIdentity(1, 9));

            Assert.Null(late);
        }

        [Fact]
        public void Burst_resets_after_timeout_or_window_change()
        {
            FeedbackEngine engine = new();
            engine.Reset(new ClipboardSequence(1));
            engine.CopyPressed(0, new WindowIdentity(1, 9), new ClipboardSequence(1));
            engine.CopyPressed(100, new WindowIdentity(1, 9), new ClipboardSequence(1));
            Assert.Equal(2, engine.Count);

            engine.CopyPressed(4000, new WindowIdentity(1, 9), new ClipboardSequence(1));
            Assert.Equal(1, engine.Count);

            engine.CopyPressed(4100, new WindowIdentity(2, 9), new ClipboardSequence(1));
            Assert.Equal(1, engine.Count);
        }

        [Theory]
        [InlineData(1, "已复制")]
        [InlineData(2, "还在呢")]
        [InlineData(3, "真的复制好了")]
        [InlineData(4, "千真万确复制好了")]
        [InlineData(5, "去粘贴吧")]
        [InlineData(9, "去粘贴吧")]
        public void Success_titles_are_stable(int count, string expected)
        {
            Assert.Equal(expected, FeedbackEngine.SuccessTitle(count));
        }

        [Fact]
        public void Window_identity_compares_handle_and_process()
        {
            var left = new WindowIdentity(10, 20);
            var right = new WindowIdentity(10, 20);
            var other = new WindowIdentity(10, 21);
            Assert.True(left == right);
            Assert.True(left != other);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Clipboard_sequence_compares_value()
        {
            Assert.True(new ClipboardSequence(4) == new ClipboardSequence(4));
            Assert.True(new ClipboardSequence(4) != new ClipboardSequence(5));
        }
    }
}
