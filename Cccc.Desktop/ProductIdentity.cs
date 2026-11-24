namespace Cccc.Desktop
{
    // Identity strings that keep this fork's runtime state separate from the
    // upstream CCCC application on the same machine. Windows treats file paths
    // and registry value names case-insensitively, so these must differ by more
    // than letter case. Keep build.ps1 and RegistryStartupStore.EntryName in
    // sync; ProductIdentityTests guards the three copies.
    internal static class ProductIdentity
    {
        internal const string Name = "Sandlada.Cccc";
        internal const string StartupEntry = "Sandlada.Cccc";
        internal const string Mutex = "Local\\Sandlada.Cccc.CopyFeedback.v1";
    }
}
