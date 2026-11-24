using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class FakeClipboardReader : IClipboardReader
    {
        public uint Sequence = 1;
        public ClipboardContent Result;
        public TaskCompletionSource<ClipboardContent> Pending;
        public uint? ExpectedSequence;
        public long? Deadline;
        public int Reads;

        public uint CurrentSequence => Sequence;

        public Task<ClipboardContent> ReadAsync(uint expectedSequence, long deadlineMilliseconds,
            CancellationToken cancellationToken)
        {
            Reads++;
            ExpectedSequence = expectedSequence;
            Deadline = deadlineMilliseconds;
            return Pending is not null ? Pending.Task : Task.FromResult(Result);
        }
    }
}
