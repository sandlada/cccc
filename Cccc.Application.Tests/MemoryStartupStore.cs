using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class MemoryStartupStore : IStartupStore
    {
        public string Value;
        public bool FailRead;
        public bool FailWrite;
        public bool FailDelete;

        public string Read()
        {
            if (FailRead) throw new IOException("injected");
            return Value;
        }

        public void Write(string command)
        {
            if (FailWrite) throw new IOException("injected");
            Value = command;
        }

        public void Delete()
        {
            if (FailDelete) throw new IOException("injected");
            Value = null;
        }
    }
}
