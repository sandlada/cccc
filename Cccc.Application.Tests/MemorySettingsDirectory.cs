using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class MemorySettingsDirectory : ISettingsDirectory
    {
        public DeleteResult DeleteResult = new() { Success = true };
        public int DeleteCount;

        public DeleteResult Delete()
        {
            DeleteCount++;
            return DeleteResult;
        }
    }
}
