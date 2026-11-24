using System;

namespace Cccc.Application
{
    public sealed class PreferencesLoadResult
    {
        public PreferencesLoadState State;
        public PreferencesModel Model = new();
        public string Error = "";
    }
}
