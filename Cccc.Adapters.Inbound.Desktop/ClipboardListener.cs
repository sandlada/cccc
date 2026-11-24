using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Threading;
using Cccc.Application;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Inbound.Desktop
{
    public sealed class ClipboardListener : IDisposable
    {
        private readonly ObserveClipboardHandler handler;
        private readonly ISystemClock clock;
        private readonly Dispatcher dispatcher;
        private readonly CancellationToken cancellationToken;
        private readonly HwndSourceHook hook;
        private HwndSource source;

        public ClipboardListener(ObserveClipboardHandler handler, ISystemClock clock, Dispatcher dispatcher,
            CancellationToken cancellationToken)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (clock == null) throw new ArgumentNullException("clock");
            this.handler = handler;
            this.clock = clock;
            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
            this.cancellationToken = cancellationToken;
            hook = WindowMessage;
        }

        public void Attach(HwndSource value)
        {
            if (value == null) throw new ArgumentNullException("value");
            source = value;
            source.AddHook(hook);
        }

        private IntPtr WindowMessage(IntPtr windowHandle, int message, IntPtr wordParameter, IntPtr longParameter, ref bool handled)
        {
            if (message == WindowsNativeMethods.WindowMessageClipboardUpdate && !cancellationToken.IsCancellationRequested)
            {
                long eventTime = clock.CurrentTimeMilliseconds;
                dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async delegate
                {
                    try
                    {
                        await handler.HandleAsync(eventTime, cancellationToken).ConfigureAwait(true);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception error)
                    {
                        Trace.TraceError("Cccc clipboard observation: {0} (0x{1:X8})",
                            error.GetType().Name, error.HResult);
                    }
                }));
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (source != null)
            {
                source.RemoveHook(hook);
                source = null;
            }
        }
    }
}
