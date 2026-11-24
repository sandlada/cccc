using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class FakeForegroundWindowQuery : IForegroundWindowQuery
    {
        public WindowTarget Current { get; set; } = new(7, 9);
    }
}
