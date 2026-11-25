using System;
using System.Collections.Generic;
using System.Globalization;
using Cccc.Domain.Localization;

namespace Cccc.Application
{
    public sealed class LanguageOption
    {
        public LanguageOption(string tag, string nativeName, string cultureName, string fontFamily)
        {
            Tag = tag ?? LanguageTag.EnglishValue;
            NativeName = nativeName ?? "English";
            CultureName = cultureName ?? Tag;
            FontFamily = fontFamily;
        }

        public string Tag { get; }
        public string NativeName { get; }
        public string CultureName { get; }
        public string FontFamily { get; }
    }

    public static class LanguageSettings
    {
        public const string SystemTag = LanguageTag.SystemValue;
        public const string EnglishTag = LanguageTag.EnglishValue;

        public static string NormalizePreference(string raw) =>
            LanguageTag.NormalizePreference(raw).Value;

        public static bool IsSystem(string tag) =>
            LanguageTag.NormalizePreference(tag).IsSystem;
    }

    public interface ILocalizer
    {
        string PreferenceTag { get; }
        LanguageOption CurrentLanguage { get; }
        CultureInfo CurrentCulture { get; }
        IReadOnlyList<LanguageOption> AvailableLanguages { get; }

        event Action<LanguageOption> LanguageChanged;

        void ApplyPreference(string preferenceTag);
        void RefreshSystemLanguage();
        string Get(string key);
        string Format(string key, params object[] args);
    }
}
