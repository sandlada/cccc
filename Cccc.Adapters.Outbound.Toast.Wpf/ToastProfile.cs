namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal static class ToastProfile
    {
        // The file name predates the feature shipping; keep it so existing
        // preferences survive an upgrade.
        internal const string SettingsFile = "settings-taskbar-experiment.txt";
        internal static string Label { get; } =
            typeof(ToastProfile).Assembly.GetName().Version?.ToString(3) ?? "2.5.0";
        internal static bool Compact { get; private set; }
        internal static void SetCompact(bool value) { Compact = value; }
    }
}
