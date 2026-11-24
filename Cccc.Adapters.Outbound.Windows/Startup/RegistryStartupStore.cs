using System;
using System.IO;
using Microsoft.Win32;
using Cccc.Application;

namespace Cccc.Adapters.Outbound.Windows
{
    public sealed class RegistryStartupStore : IStartupStore
    {
        public const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        // Must match Cccc.Desktop/ProductIdentity.StartupEntry; a case-only
        // difference from the upstream "CCCC" value would still collide,
        // because registry value names are case-insensitive.
        public const string EntryName = "Sandlada.Cccc";
        private readonly string subKey;

        public RegistryStartupStore() { subKey = RunKey; }

        internal RegistryStartupStore(string isolatedKey)
        {
            if (!isolatedKey.StartsWith("Software\\Cccc\\StartupTests\\", StringComparison.Ordinal))
                throw new ArgumentException("Only an isolated startup test key is allowed.");
            subKey = isolatedKey;
        }

        public string Read()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, false);
            if (key is null) return null;
            object value = key.GetValue(EntryName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
            return value is null ? null : (value as string ?? string.Empty);
        }

        public void Write(string command)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(subKey);
            if (key is null) throw new IOException("Cannot open the startup registration.");
            key.SetValue(EntryName, command, RegistryValueKind.String);
        }

        public void Delete()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, true);
            if (key is not null) key.DeleteValue(EntryName, false);
        }
    }
}
