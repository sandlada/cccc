using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class FakeSystemClock : ISystemClock
    {
        public long CurrentTimeMilliseconds { get; set; }
    }
}
