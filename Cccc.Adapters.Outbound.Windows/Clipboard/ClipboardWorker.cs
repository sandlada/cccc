using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Windows
{
    // A single background STA owns potentially delayed clipboard reads. It keeps only
    // the newest queued request, and its results never block the UI or keyboard hook.
    public sealed class ClipboardWorker : IClipboardReader, IDisposable
    {
        private sealed class Request
        {
            internal uint ExpectedSequence;
            internal long DeadlineMilliseconds;
            internal CancellationToken CancellationToken;
            internal TaskCompletionSource<ClipboardContent> Completion;
        }

        private readonly object gate = new();
        private readonly AutoResetEvent wake;
        private readonly IntPtr windowHandle;
        private readonly ISystemClock clock;
        private readonly Thread thread;
        private Request latest;
        private bool stopped;

        public ClipboardWorker(IntPtr windowHandle, ISystemClock clock)
        {
            ArgumentNullException.ThrowIfNull(clock);
            wake = new AutoResetEvent(false);
            this.windowHandle = windowHandle;
            this.clock = clock;
            thread = new Thread(Work) { IsBackground = true, Name = "Cccc clipboard reader" };
            thread.SetApartmentState(ApartmentState.STA);
            try { thread.Start(); }
            catch { wake.Dispose(); throw; }
        }

        public uint CurrentSequence => WindowsNativeMethods.GetClipboardSequenceNumber();

        public Task<ClipboardContent> ReadAsync(uint expectedSequence, long deadlineMilliseconds,
            CancellationToken cancellationToken)
        {
            TaskCompletionSource<ClipboardContent> completion = new(
                TaskCreationOptions.RunContinuationsAsynchronously);
            if (cancellationToken.IsCancellationRequested)
            {
                completion.TrySetCanceled();
                return completion.Task;
            }

            Request request = new()
            {
                ExpectedSequence = expectedSequence,
                DeadlineMilliseconds = deadlineMilliseconds,
                CancellationToken = cancellationToken,
                Completion = completion
            };

            lock (gate)
            {
                if (stopped)
                {
                    completion.TrySetResult(null);
                    return completion.Task;
                }
                if (latest is not null) latest.Completion.TrySetResult(null);
                latest = request;
                wake.Set();
            }
            return completion.Task;
        }

        private void Work()
        {
            try
            {
                int[] delays = [0, 40, 80, 160, 240, 320, 400];
                for (;;)
                {
                    Request request;
                    lock (gate)
                    {
                        if (stopped) return;
                        request = latest;
                        latest = null;
                        if (request is not null) wake.Reset();
                    }
                    if (request is null) { wake.WaitOne(); continue; }

                    ClipboardContent result = null;
                    foreach (int delay in delays)
                    {
                        if (delay > 0)
                        {
                            if (request.CancellationToken.WaitHandle.WaitOne(delay)) break;
                        }
                        lock (gate) { if (stopped) return; if (latest is not null) break; }
                        if (clock.CurrentTimeMilliseconds >= request.DeadlineMilliseconds ||
                            WindowsNativeMethods.GetClipboardSequenceNumber() != request.ExpectedSequence) break;
                        try { result = ClipboardReader.Read(windowHandle, request.ExpectedSequence); }
                        catch (Exception error)
                        {
                            Trace.TraceError("Cccc clipboard read: {0} (0x{1:X8})",
                                error.GetType().Name, error.HResult);
                            result = null;
                        }
                        if (result is not null) break;
                    }

                    lock (gate) { if (stopped) return; }
                    if (result is not null && clock.CurrentTimeMilliseconds < request.DeadlineMilliseconds &&
                        WindowsNativeMethods.GetClipboardSequenceNumber() == request.ExpectedSequence)
                        request.Completion.TrySetResult(result);
                    else if (request.CancellationToken.IsCancellationRequested)
                        request.Completion.TrySetCanceled();
                    else
                        request.Completion.TrySetResult(null);
                }
            }
            finally
            {
                lock (gate) { stopped = true; latest = null; wake.Dispose(); }
            }
        }

        public void Dispose()
        {
            lock (gate)
            {
                if (stopped) return;
                stopped = true;
                if (latest is not null) latest.Completion.TrySetResult(null);
                latest = null;
                wake.Set();
            }
            // A background read may be waiting on a source app's delayed rendering.
            // Do not join it on the UI thread. Work owns and closes the event on exit.
        }
    }
}
