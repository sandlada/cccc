using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Cccc.Application;
using Cccc.Domain.Localization;
using Xunit;

namespace Cccc.Application.Tests
{
    public sealed class LocalizationServiceTests
    {
        private sealed class FakeSystemLanguageProvider : ISystemLanguageProvider
        {
            public string Tag;
            public Exception Error;

            public string GetSystemLanguageTag()
            {
                if (Error is not null) throw Error;
                return Tag;
            }
        }

        [Fact]
        public void System_language_error_or_null_falls_back_to_english()
        {
            var throwing = new LocalizationService(new FakeSystemLanguageProvider
            {
                Error = new InvalidOperationException("registry read failed")
            });
            Assert.Equal("en", throwing.CurrentLanguage.Tag);
            Assert.Equal("Copied", throwing.Get("feedback.title.copied"));

            var empty = new LocalizationService(new FakeSystemLanguageProvider { Tag = null });
            Assert.Equal("en", empty.CurrentLanguage.Tag);

            var unsupported = new LocalizationService(new FakeSystemLanguageProvider { Tag = "sw-KE" });
            Assert.Equal("en", unsupported.CurrentLanguage.Tag);
        }

        [Theory]
        [InlineData("zh-CN", "zh-Hans", "已复制")]
        [InlineData("zh-TW", "zh-Hant", "已複製")]
        [InlineData("zh-HK", "zh-Hant", "已複製")]
        [InlineData("ja-JP", "ja", "コピーしました")]
        [InlineData("ko-KR", "ko", "복사됨")]
        [InlineData("en-GB", "en", "Copied")]
        public void System_language_maps_to_expected_discovered_catalog(string systemTag, string expectedTag, string expectedCopied)
        {
            var service = new LocalizationService(new FakeSystemLanguageProvider { Tag = systemTag });
            Assert.Equal(expectedTag, service.CurrentLanguage.Tag);
            Assert.Equal(expectedCopied, service.Get("feedback.title.copied"));
        }

        [Fact]
        public void Manual_switch_fires_language_changed_and_updates_culture()
        {
            var service = new LocalizationService(new FakeSystemLanguageProvider { Tag = "en-US" });
            List<string> events = new();
            service.LanguageChanged += lang => events.Add(lang.Tag);

            service.ApplyPreference("ja");
            Assert.Equal("ja", service.CurrentLanguage.Tag);
            Assert.Equal("ja", service.CurrentCulture.Name);
            Assert.Equal(new[] { "ja" }, events);

            // Re-applying same preference should not fire duplicate event
            service.ApplyPreference("ja");
            Assert.Single(events);

            service.ApplyPreference("system");
            Assert.Equal("en", service.CurrentLanguage.Tag);
            Assert.Equal(new[] { "ja", "en" }, events);
        }

        [Fact]
        public void Missing_key_in_non_english_catalog_falls_back_to_english()
        {
            var en = new LocalizationCatalog(
                new LanguageDescriptor(LanguageTag.English, "English"),
                new Dictionary<string, string> { ["greeting"] = "Hello", ["only_en"] = "Fallback" });
            var fr = new LocalizationCatalog(
                new LanguageDescriptor(new LanguageTag("fr"), "Français"),
                new Dictionary<string, string> { ["greeting"] = "Bonjour" });

            var service = new LocalizationService(
                new FakeSystemLanguageProvider { Tag = "fr-CA" },
                new[] { en, fr });

            Assert.Equal("fr", service.CurrentLanguage.Tag);
            Assert.Equal("Bonjour", service.Get("greeting"));
            Assert.Equal("Fallback", service.Get("only_en"));
            Assert.Equal("unknown.key", service.Get("unknown.key"));
        }

        [Fact]
        public void All_discovered_embedded_catalogs_have_full_key_parity_placeholders_and_safe_tooltips()
        {
            var service = new LocalizationService(new FakeSystemLanguageProvider { Tag = "en" });
            IReadOnlyList<LocalizationCatalog> catalogs = service.GetLoadedCatalogs();

            Assert.True(catalogs.Count >= 5, "Expected at least 5 embedded catalogs (en, zh-Hans, zh-Hant, ja, ko).");
            LocalizationCatalog baseline = catalogs.First(c => c.Descriptor.Tag == LanguageTag.English);
            Assert.NotEmpty(baseline.Strings);

            Regex placeholderRegex = new(@"\{(\d+)\}", RegexOptions.Compiled);

            foreach (LocalizationCatalog catalog in catalogs)
            {
                string tag = catalog.Descriptor.Tag.Value;
                Assert.False(string.IsNullOrWhiteSpace(catalog.Descriptor.NativeName), $"Catalog '{tag}' missing nativeName.");

                // Valid CultureInfo
                CultureInfo culture = CultureInfo.GetCultureInfo(catalog.Descriptor.CultureName);
                Assert.NotNull(culture);

                // No missing or empty keys compared to en.json
                foreach (var pair in baseline.Strings)
                {
                    Assert.True(catalog.Strings.TryGetValue(pair.Key, out string value),
                        $"Catalog '{tag}' is missing key '{pair.Key}'.");
                    Assert.False(string.IsNullOrWhiteSpace(value),
                        $"Catalog '{tag}' has empty value for key '{pair.Key}'.");

                    // Placeholder indices must match baseline
                    string[] expectedPlaceholders = placeholderRegex.Matches(pair.Value)
                        .Select(m => m.Value).Distinct().OrderBy(x => x).ToArray();
                    string[] actualPlaceholders = placeholderRegex.Matches(value)
                        .Select(m => m.Value).Distinct().OrderBy(x => x).ToArray();
                    Assert.Equal(expectedPlaceholders, actualPlaceholders);
                }

                // No orphan keys unknown to en.json
                foreach (string key in catalog.Strings.Keys)
                {
                    Assert.True(baseline.Strings.ContainsKey(key),
                        $"Catalog '{tag}' contains unknown key '{key}' not present in en.json.");
                }

                // Win32 NotifyIcon.Text limit is 63 characters
                Assert.True(catalog.Strings["tray.tooltip.active"].Length <= 63,
                    $"Catalog '{tag}' tray.tooltip.active exceeds 63 chars.");
                Assert.True(catalog.Strings["tray.tooltip.paused"].Length <= 63,
                    $"Catalog '{tag}' tray.tooltip.paused exceeds 63 chars.");
            }
        }
    }
}
