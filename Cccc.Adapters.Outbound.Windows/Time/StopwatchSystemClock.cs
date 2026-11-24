using System;
using System.Diagnostics;
using Cccc.Application;

namespace Cccc.Adapters.Outbound.Windows
{
    public sealed class StopwatchSystemClock : ISystemClock
    {
        public long CurrentTimeMilliseconds => (long)(Stopwatch.GetTimestamp() * (1000.0 / Stopwatch.Frequency));
    }
}
