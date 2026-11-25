using System.Reflection;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal static class ToastProfile
    {
        // The file name predates the feature shipping; keep it so existing
        // preferences survive an upgrade.
        internal const string SettingsFile = "settings-taskbar-experiment.txt";
        internal static string Label { get; } = ResolveVersion();
        internal static bool Compact { get; private set; }
        internal static void SetCompact(bool value) { Compact = value; }

        private static string ResolveVersion()
        {
            var info = typeof(ToastProfile).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(info))
            {
                int plus = info.IndexOf('+');
                return plus >= 0 ? info[..plus] : info;
            }
            return typeof(ToastProfile).Assembly.GetName().Version?.ToString(3)
                ?? "0.0.1-20261125.a";
        }
    }
}
