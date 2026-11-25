using Cccc.Domain.Preferences;
using Xunit;

namespace Cccc.Domain.Tests
{
    public sealed class ToastAppearancePolicyTests
    {
        [Theory]
        [InlineData(ToastDisplayMode.Full, true, true, true)]
        [InlineData(ToastDisplayMode.IconOnly, true, false, false)]
        [InlineData(ToastDisplayMode.IconAndLabel, true, true, false)]
        [InlineData(ToastDisplayMode.IconAndContent, true, false, true)]
        [InlineData(ToastDisplayMode.LabelOnly, false, true, false)]
        [InlineData(ToastDisplayMode.ContentOnly, false, false, true)]
        [InlineData(ToastDisplayMode.LabelAndContent, false, true, true)]
        public void Display_mode_visibility_flags_match_expected(
            ToastDisplayMode mode, bool expectedIcon, bool expectedLabel, bool expectedContent)
        {
            Assert.Equal(expectedIcon, ToastDisplayPolicy.ShowsIcon(mode));
            Assert.Equal(expectedLabel, ToastDisplayPolicy.ShowsLabel(mode));
            Assert.Equal(expectedContent, ToastDisplayPolicy.ShowsContent(mode));
        }

        [Theory]
        [InlineData(-10, 0)]
        [InlineData(0, 0)]
        [InlineData(26, 26)]
        [InlineData(64, 64)]
        [InlineData(999, 64)]
        public void Corner_radius_clamps_within_bounds(int input, int expected)
        {
            Assert.Equal(expected, ToastAppearancePolicy.ClampCornerRadius(input));
        }

        [Theory]
        [InlineData(0, 4)]
        [InlineData(4, 4)]
        [InlineData(20, 20)]
        [InlineData(36, 36)]
        [InlineData(100, 36)]
        public void Padding_clamps_within_bounds(int input, int expected)
        {
            Assert.Equal(expected, ToastAppearancePolicy.ClampPadding(input));
        }

        [Fact]
        public void Full_corner_radius_resolves_to_half_min_dimension()
        {
            Assert.Equal(40, ToastAppearancePolicy.ResolveCornerRadius(10, true, 360, 80));
            Assert.Equal(30, ToastAppearancePolicy.ResolveCornerRadius(40, false, 120, 60));
            Assert.Equal(0, ToastAppearancePolicy.ResolveCornerRadius(0, false, 360, 80));
        }

        [Fact]
        public void Font_family_normalization_falls_back_trims_and_clamps()
        {
            Assert.Equal(ToastAppearancePolicy.DefaultFontFamily, ToastAppearancePolicy.NormalizeFontFamily(null));
            Assert.Equal(ToastAppearancePolicy.DefaultFontFamily, ToastAppearancePolicy.NormalizeFontFamily("   "));
            Assert.Equal("Consolas, Segoe UI", ToastAppearancePolicy.NormalizeFontFamily("  Consolas, Segoe UI\r\n"));

            string tooLong = new string('A', ToastAppearancePolicy.MaxFontFamilyLength + 20);
            string normalized = ToastAppearancePolicy.NormalizeFontFamily(tooLong);
            Assert.Equal(ToastAppearancePolicy.MaxFontFamilyLength, normalized.Length);
        }
    }
}
