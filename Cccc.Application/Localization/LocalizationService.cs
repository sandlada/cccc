using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Cccc.Domain.Localization;

namespace Cccc.Application
{
    public sealed class LocalizationService : ILocalizer
    {
        private readonly ISystemLanguageProvider systemLanguageProvider;
        private readonly Dictionary<string, LocalizationCatalog> catalogsByTag;
        private readonly Dictionary<string, LanguageOption> optionsByTag;
        private readonly LocalizationCatalog fallbackCatalog;
        private readonly List<LanguageDescriptor> descriptors;
        private readonly List<LanguageOption> availableLanguages;
        private LocalizationCatalog currentCatalog;
        private LanguageOption currentOption;
        private CultureInfo currentCulture;
        private LanguageTag preference;

        public LocalizationService(
            ISystemLanguageProvider systemLanguageProvider = null,
            IReadOnlyList<LocalizationCatalog> catalogs = null)
        {
            this.systemLanguageProvider = systemLanguageProvider;
            IReadOnlyList<LocalizationCatalog> loaded = catalogs ?? LocalizationCatalogLoader.LoadEmbeddedCatalogs();

            catalogsByTag = new(StringComparer.OrdinalIgnoreCase);
            optionsByTag = new(StringComparer.OrdinalIgnoreCase);
            descriptors = new();
            availableLanguages = new();
            LocalizationCatalog english = null;

            foreach (LocalizationCatalog catalog in loaded)
            {
                if (catalog is null) continue;
                string tag = catalog.Descriptor.Tag.Value;
                if (!catalogsByTag.ContainsKey(tag))
                {
                    catalogsByTag[tag] = catalog;
                    descriptors.Add(catalog.Descriptor);
                    LanguageOption option = ToOption(catalog.Descriptor);
                    optionsByTag[tag] = option;
                    availableLanguages.Add(option);
                }
                if (catalog.Descriptor.Tag == LanguageTag.English)
                    english = catalog;
            }

            if (english is null)
            {
                LanguageDescriptor fallbackDescriptor = new(LanguageTag.English, "English", null, "en");
                english = new LocalizationCatalog(fallbackDescriptor, new Dictionary<string, string>(StringComparer.Ordinal));
                catalogsByTag[LanguageTag.EnglishValue] = english;
                descriptors.Insert(0, fallbackDescriptor);
                LanguageOption fallbackOption = ToOption(fallbackDescriptor);
                optionsByTag[LanguageTag.EnglishValue] = fallbackOption;
                availableLanguages.Insert(0, fallbackOption);
            }

            fallbackCatalog = english;
            currentCatalog = fallbackCatalog;
            currentOption = optionsByTag[fallbackCatalog.Descriptor.Tag.Value];
            currentCulture = ResolveCulture(fallbackCatalog.Descriptor.CultureName);
            preference = LanguageTag.System;
            ApplyPreference(LanguageTag.SystemValue);
        }

        public string PreferenceTag => preference.Value;
        public LanguageOption CurrentLanguage => currentOption;
        public CultureInfo CurrentCulture => currentCulture;
        public IReadOnlyList<LanguageOption> AvailableLanguages => availableLanguages;

        public event Action<LanguageOption> LanguageChanged;

        public void ApplyPreference(string preferenceTag)
        {
            LanguageTag normalized = LanguageTag.NormalizePreference(preferenceTag);
            preference = normalized;

            string requestedTag = normalized.IsSystem
                ? TryReadSystemLanguageTag()
                : normalized.Value;

            LanguageDescriptor resolved = LanguageResolver.Resolve(
                requestedTag,
                descriptors,
                fallbackCatalog.Descriptor);

            LocalizationCatalog nextCatalog = catalogsByTag.TryGetValue(resolved.Tag.Value, out LocalizationCatalog found)
                ? found
                : fallbackCatalog;

            bool changed = !ReferenceEquals(currentCatalog, nextCatalog);
            currentCatalog = nextCatalog;
            currentOption = optionsByTag[currentCatalog.Descriptor.Tag.Value];
            currentCulture = ResolveCulture(currentCatalog.Descriptor.CultureName);

            if (changed)
                LanguageChanged?.Invoke(currentOption);
        }

        public void RefreshSystemLanguage()
        {
            if (preference.IsSystem)
                ApplyPreference(LanguageTag.SystemValue);
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            if (currentCatalog.Strings.TryGetValue(key, out string value) && !string.IsNullOrEmpty(value))
                return value;
            if (!ReferenceEquals(currentCatalog, fallbackCatalog) &&
                fallbackCatalog.Strings.TryGetValue(key, out string fallbackValue) &&
                !string.IsNullOrEmpty(fallbackValue))
                return fallbackValue;
            return key;
        }

        public string Format(string key, params object[] args)
        {
            string template = Get(key);
            if (args is null || args.Length == 0) return template;
            try
            {
                return string.Format(currentCulture, template, args);
            }
            catch (FormatException)
            {
                if (!ReferenceEquals(currentCatalog, fallbackCatalog) &&
                    fallbackCatalog.Strings.TryGetValue(key, out string fallbackTemplate) &&
                    !string.IsNullOrEmpty(fallbackTemplate))
                {
                    try { return string.Format(CultureInfo.InvariantCulture, fallbackTemplate, args); }
                    catch (FormatException) { }
                }
                return template;
            }
        }

        public IReadOnlyList<LocalizationCatalog> GetLoadedCatalogs()
        {
            List<LocalizationCatalog> result = new(descriptors.Count);
            foreach (LanguageDescriptor descriptor in descriptors)
            {
                if (catalogsByTag.TryGetValue(descriptor.Tag.Value, out LocalizationCatalog catalog))
                    result.Add(catalog);
            }
            return result;
        }

        private string TryReadSystemLanguageTag()
        {
            if (systemLanguageProvider is null) return null;
            try
            {
                return systemLanguageProvider.GetSystemLanguageTag();
            }
            catch (Exception error)
            {
                Trace.TraceWarning("Cccc system language read failed, falling back to English: {0}", error.GetType().Name);
                return null;
            }
        }

        private static LanguageOption ToOption(LanguageDescriptor descriptor) =>
            new(descriptor.Tag.Value, descriptor.NativeName, descriptor.CultureName, descriptor.FontFamily);

        private static CultureInfo ResolveCulture(string cultureName)
        {
            if (!string.IsNullOrWhiteSpace(cultureName))
            {
                try { return CultureInfo.GetCultureInfo(cultureName); }
                catch (CultureNotFoundException) { }
                catch (ArgumentException) { }
            }
            try { return CultureInfo.GetCultureInfo("en"); }
            catch (CultureNotFoundException) { return CultureInfo.InvariantCulture; }
        }
    }
}
