using System;
using System.Windows;
using System.Windows.Media;
using Cccc.Domain.Preferences;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // Toast card typography uses user-configured system font families with a default fallback stack,
    // avoiding large embedded font binaries in the application package.
    internal static class EmbeddedFonts
    {
        static EmbeddedFonts() { }
        internal static readonly FontWeight TitleWeight = FontWeight.FromOpenTypeWeight(650);
        internal static readonly FontWeight BodyWeight = FontWeight.FromOpenTypeWeight(500);
        internal static readonly FontFamily Family = new FontFamily(ToastAppearancePolicy.DefaultFontFamily);

        internal static FontFamily ResolveFontFamily(string fontFamilyName, string languageDefaultFontFamily = null)
        {
            string effective = ToastAppearancePolicy.ResolveEffectiveFontFamily(fontFamilyName, languageDefaultFontFamily);
            try
            {
                return new FontFamily(effective);
            }
            catch (ArgumentException)
            {
                return Family;
            }
            catch (NotSupportedException)
            {
                return Family;
            }
        }
    }
}
