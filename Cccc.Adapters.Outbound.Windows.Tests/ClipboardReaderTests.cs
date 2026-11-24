using System;
using System.IO;
using Cccc.Adapters.Outbound.Windows;
using Xunit;

namespace Cccc.Adapters.Outbound.Windows.Tests
{
    public sealed class ClipboardReaderTests
    {
        private sealed class FakeClipboardNativeAccess : IClipboardNativeAccess
        {
            public bool Opens = true;
            public bool Fail;
            public int Closed;
            public uint SequenceValue = 42;
            public uint Owner = 1;
            public int Formats = 1;

            public bool Open(IntPtr window) => Opens;
            public void Close() { Closed++; }
            public uint Sequence => SequenceValue;
            public uint OwnerProcess => Owner;
            public bool Available(uint format) { if (Fail) throw new IOException("injected"); return false; }
            public IntPtr Data(uint format) => IntPtr.Zero;
            public int FormatCount => Formats;
        }

        [Fact]
        public void Busy_clipboard_is_not_closed_by_non_owner()
        {
            var access = new FakeClipboardNativeAccess { Opens = false };
            Assert.Null(ClipboardReader.Read(IntPtr.Zero, 42, access));
            Assert.Equal(0, access.Closed);
        }

        [Fact]
        public void Stale_sequence_is_rejected_but_handle_is_closed()
        {
            var access = new FakeClipboardNativeAccess { SequenceValue = 43 };
            Assert.Null(ClipboardReader.Read(IntPtr.Zero, 42, access));
            Assert.Equal(1, access.Closed);
        }

        [Fact]
        public void Unsupported_format_returns_unclaimed_snapshot_and_closes_handle()
        {
            var access = new FakeClipboardNativeAccess();
            var result = ClipboardReader.Read(IntPtr.Zero, 42, access);
            Assert.NotNull(result);
            Assert.False(result.Supported);
            Assert.Equal(1, access.Closed);
        }

        [Fact]
        public void Exception_still_releases_clipboard()
        {
            var access = new FakeClipboardNativeAccess { Fail = true };
            Assert.Throws<IOException>(() => ClipboardReader.Read(IntPtr.Zero, 42, access));
            Assert.Equal(1, access.Closed);
        }
    }
}
