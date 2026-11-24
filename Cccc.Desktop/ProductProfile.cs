namespace Cccc.Desktop
{
    internal static class ProductProfile
    {
        // Version comes from Directory.Build.props so the csproj, the tray label
        // and the toast window title cannot drift apart.
        internal static string Label { get; } =
            typeof(ProductProfile).Assembly.GetName().Version?.ToString(3) ?? "2.5.0";
    }
}
