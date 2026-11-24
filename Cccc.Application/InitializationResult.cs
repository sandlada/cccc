using System;

namespace Cccc.Application
{
    public sealed class InitializationResult
    {
        public bool Initialized;
        public bool NeedsWarning;
        public PreferencesModel Preferences = new();
        public StartupState Startup;
        public string Error = "";
    }
}
