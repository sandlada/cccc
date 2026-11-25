using System.Reflection;

namespace Cccc.Desktop
{
    internal static class ProductProfile
    {
        // Version comes from Directory.Build.props so the csproj, the tray label
        // and the toast window title cannot drift apart.
        internal static string Label { get; } = ResolveVersion();

        private static string ResolveVersion()
        {
            var info = typeof(ProductProfile).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(info))
            {
                int plus = info.IndexOf('+');
                return plus >= 0 ? info[..plus] : info;
            }
            return typeof(ProductProfile).Assembly.GetName().Version?.ToString(3)
                ?? "0.0.1-20261125.a";
        }
    }
}
