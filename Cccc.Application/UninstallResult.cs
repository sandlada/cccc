using System;

namespace Cccc.Application
{
    public sealed class UninstallResult
    {
        public bool Success;
        public bool StartupCleared;
        public bool SettingsDeleted;
        public bool LegacySettingsDeleted;
        public bool DirectoryDeleted;
        public string Error = "";
    }
}
