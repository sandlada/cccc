using Cccc.Application;
using Xunit;

namespace Cccc.Application.Tests
{
    public sealed class InputAndFormattingTests
    {
        [Fact]
        public void Copy_key_detector_handles_repeat_and_modifiers()
        {
            var keys = new CopyKeyDetector();
            Assert.True(keys.Feed(0x43, true, true, false));
            Assert.False(keys.Feed(0x43, true, true, false));
            keys.Reset();
            Assert.True(keys.Feed(0x43, true, true, false));
            keys.Feed(0x43, false, false, false);
            Assert.False(keys.Feed(0x43, true, true, true));
        }

        [Fact]
        public void Preview_text_sanitizes_and_bounds_untrusted_clipboard_text()
        {
            Assert.Equal("", PreviewText.Shorten(null));
            Assert.Equal("", PreviewText.Shorten("   \r\n\t "));
            Assert.Equal("a b", PreviewText.Shorten(" a\r\n b "));
            Assert.Equal("ab", PreviewText.Shorten("a\u202eb"));
            Assert.Equal(new string('x', 36) + "…", PreviewText.Shorten(new string('x', 37)));
        }

        [Fact]
        public void Feedback_session_tracks_pause_generation_and_count()
        {
            var session = new FeedbackSession();
            Assert.Equal(0, session.Generation);
            session.Reset(4);
            Assert.Equal(1, session.Generation);
            session.SetPaused(true, 4);
            Assert.True(session.Paused);
            Assert.Equal(2, session.Generation);
            session.SetPaused(false, 5);
            Assert.False(session.Paused);
            Assert.Equal(3, session.Generation);
        }
    }
}
