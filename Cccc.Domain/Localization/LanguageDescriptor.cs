using System;
using System.Collections.Generic;
using Cccc.Domain.Preferences;

namespace Cccc.Domain.Localization
{
    public sealed class LanguageDescriptor
    {
        public LanguageDescriptor(
            LanguageTag tag,
            string nativeName,
            IReadOnlyList<string> aliases = null,
            string cultureName = null,
            string fontFamily = null)
        {
            if (tag.IsSystem) throw new ArgumentException("A concrete language descriptor cannot use 'system'.", nameof(tag));
            if (string.IsNullOrWhiteSpace(nativeName)) throw new ArgumentException("NativeName is required.", nameof(nativeName));

            Tag = tag;
            NativeName = nativeName.Trim();
            Aliases = NormalizeAliases(aliases);
            CultureName = LanguageTag.Normalize(cultureName) ?? tag.Value;
            FontFamily = string.IsNullOrWhiteSpace(fontFamily)
                ? null
                : ToastAppearancePolicy.NormalizeFontFamily(fontFamily);
        }

        public LanguageTag Tag { get; }
        public string NativeName { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string CultureName { get; }
        public string FontFamily { get; }

        private static IReadOnlyList<string> NormalizeAliases(IReadOnlyList<string> aliases)
        {
            if (aliases is null || aliases.Count == 0) return Array.Empty<string>();
            List<string> result = new(aliases.Count);
            foreach (string raw in aliases)
            {
                string normalized = LanguageTag.Normalize(raw);
                if (normalized is not null && !string.Equals(normalized, LanguageTag.SystemValue, StringComparison.OrdinalIgnoreCase))
                    result.Add(normalized);
            }
            return result;
        }
    }
}
