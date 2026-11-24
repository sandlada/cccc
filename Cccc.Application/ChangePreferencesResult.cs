using System;

namespace Cccc.Application
{
    public sealed class ChangePreferencesResult
    {
        public PreferencesModel Preferences = new();
        public bool Success;
        public string Error = "";
    }
}
