using System.Collections.Generic;
using Cccc.Domain.Localization;
using Cccc.Domain.Preferences;
using Xunit;

namespace Cccc.Domain.Tests
{
    public sealed class LanguageResolverTests
    {
        private static readonly LanguageDescriptor En = new(LanguageTag.English, "English", null, "en");
        private static readonly LanguageDescriptor ZhHans = new(new LanguageTag("zh-Hans"), "简体中文", new[] { "zh", "zh-CN", "zh-SG" }, "zh-Hans");
        private static readonly LanguageDescriptor ZhHant = new(new LanguageTag("zh-Hant"), "繁體中文", new[] { "zh-TW", "zh-HK", "zh-MO" }, "zh-Hant");
        private static readonly LanguageDescriptor Ja = new(new LanguageTag("ja"), "日本語", null, "ja");
        private static readonly LanguageDescriptor Ko = new(new LanguageTag("ko"), "한국어", null, "ko");
        private static readonly LanguageDescriptor Fr = new(new LanguageTag("fr"), "Français", null, "fr");

        private static readonly IReadOnlyList<LanguageDescriptor> StandardCatalog = new[] { En, ZhHans, ZhHant, Ja, Ko };

        [Theory]
        [InlineData(null, "en")]
        [InlineData("", "en")]
        [InlineData("   ", "en")]
        [InlineData("system", "en")]
        [InlineData("invalid tag!!", "en")]
        [InlineData("en", "en")]
        [InlineData("en-US", "en")]
        [InlineData("en-GB", "en")]
        [InlineData("zh", "zh-Hans")]
        [InlineData("zh-CN", "zh-Hans")]
        [InlineData("zh_CN", "zh-Hans")]
        [InlineData("zh-SG", "zh-Hans")]
        [InlineData("zh-Hans", "zh-Hans")]
        [InlineData("zh-Hans-CN", "zh-Hans")]
        [InlineData("zh-Hans-SG", "zh-Hans")]
        [InlineData("zh-TW", "zh-Hant")]
        [InlineData("zh-HK", "zh-Hant")]
        [InlineData("zh-MO", "zh-Hant")]
        [InlineData("zh-Hant", "zh-Hant")]
        [InlineData("zh-Hant-TW", "zh-Hant")]
        [InlineData("zh-Hant-HK", "zh-Hant")]
        [InlineData("ja", "ja")]
        [InlineData("ja-JP", "ja")]
        [InlineData("ko", "ko")]
        [InlineData("ko-KR", "ko")]
        [InlineData("fr-FR", "en")]
        [InlineData("de-DE", "en")]
        public void Resolve_maps_bcp47_and_falls_back_to_english(string requested, string expectedTag)
        {
            LanguageDescriptor resolved = LanguageResolver.Resolve(requested, StandardCatalog, En);
            Assert.Equal(expectedTag, resolved.Tag.Value);
        }

        [Fact]
        public void Resolve_automatically_matches_newly_added_community_language_via_parent_walk()
        {
            var extended = new[] { En, ZhHans, ZhHant, Ja, Ko, Fr };
            Assert.Equal("fr", LanguageResolver.Resolve("fr-CA", extended, En).Tag.Value);
            Assert.Equal("fr", LanguageResolver.Resolve("fr-FR", extended, En).Tag.Value);
        }

        [Theory]
        [InlineData(null, "system")]
        [InlineData("", "system")]
        [InlineData("system", "system")]
        [InlineData("SYSTEM", "system")]
        [InlineData("bad/tag", "system")]
        [InlineData("zh_Hans", "zh-Hans")]
        [InlineData("pt-BR", "pt-BR")]
        public void Language_tag_normalizes_preferences_safely(string raw, string expected)
        {
            Assert.Equal(expected, LanguageTag.NormalizePreference(raw).Value);
        }

        [Fact]
        public void Effective_font_family_prefers_user_custom_font_and_uses_language_default_otherwise()
        {
            const string jaDefault = "Noto Sans, Noto Sans JP, Yu Gothic UI";
            Assert.Equal(jaDefault, ToastAppearancePolicy.ResolveEffectiveFontFamily("", jaDefault));
            Assert.Equal(jaDefault, ToastAppearancePolicy.ResolveEffectiveFontFamily(ToastAppearancePolicy.DefaultFontFamily, jaDefault));
            Assert.Equal("Cascadia Code", ToastAppearancePolicy.ResolveEffectiveFontFamily("Cascadia Code", jaDefault));
            Assert.Equal(ToastAppearancePolicy.DefaultFontFamily, ToastAppearancePolicy.ResolveEffectiveFontFamily("", null));
        }
    }
}
