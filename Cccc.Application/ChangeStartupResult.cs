using System;

namespace Cccc.Application
{
    public sealed class ChangeStartupResult
    {
        public StartupState State;
        public bool Success;
        public string Error = "";
    }
}
