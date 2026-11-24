using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Adapters.Outbound.Windows;
using Cccc.Application;
using Xunit;

namespace Cccc.Adapters.Outbound.Windows.Tests
{
    public sealed class ClipboardWorkerTests
    {
        [Fact]
        public void Dispose_is_idempotent_and_stops_the_idle_thread()
        {
            var worker = new ClipboardWorker(IntPtr.Zero, new StopwatchSystemClock());
            var field = typeof(ClipboardWorker).GetField("thread",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var thread = (Thread)field.GetValue(worker);

            worker.Dispose();
            worker.Dispose();

            Assert.True(thread.Join(2000));
        }

        [Fact]
        public async Task Read_after_dispose_returns_null_without_throwing()
        {
            var worker = new ClipboardWorker(IntPtr.Zero, new StopwatchSystemClock());
            worker.Dispose();

            ClipboardContent content = await worker.ReadAsync(0, 1000, CancellationToken.None);

            Assert.Null(content);
        }

        [Fact]
        public void Clock_is_monotonic()
        {
            var clock = new StopwatchSystemClock();
            long first = clock.CurrentTimeMilliseconds;
            long second = clock.CurrentTimeMilliseconds;
            Assert.True(second >= first);
        }
    }
}
