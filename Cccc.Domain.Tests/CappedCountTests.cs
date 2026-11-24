using System;
using Cccc.Domain.Feedback;
using Xunit;

namespace Cccc.Domain.Tests
{
    public sealed class CappedCountTests
    {
        [Theory]
        [InlineData(0, 9, "0", false, 0)]
        [InlineData(9, 9, "9", false, 9)]
        [InlineData(10, 9, "9+", true, 9)]
        [InlineData(99, 99, "99", false, 99)]
        [InlineData(100, 99, "99+", true, 99)]
        [InlineData(999, 999, "999", false, 999)]
        [InlineData(1000, 999, "999+", true, 999)]
        [InlineData(3, 3, "3", false, 3)]
        [InlineData(4, 3, "3+", true, 3)]
        [InlineData(33, 33, "33", false, 33)]
        [InlineData(34, 33, "33+", true, 33)]
        [InlineData(333, 333, "333", false, 333)]
        [InlineData(334, 333, "333+", true, 333)]
        [InlineData(0, 0, "0", false, 0)]
        [InlineData(1, 0, "0+", true, 0)]
        public void Formats_and_clamps_arbitrary_limits(int value, int max, string expected, bool expectedOverflow, int expectedClamped)
        {
            var count = new CappedCount(value, max);

            Assert.Equal(value, count.Value);
            Assert.Equal(max, count.Max);
            Assert.Equal(expectedOverflow, count.IsOverflow);
            Assert.Equal(expectedClamped, count.ClampedValue);
            Assert.Equal(expected, count.Format());
            Assert.Equal(expected, count.ToString());
            Assert.Equal("×" + expected, count.Format("×"));
        }

        [Fact]
        public void Uses_default_limit_of_99()
        {
            Assert.Equal(99, CappedCount.DefaultMax);
            Assert.Equal("99", new CappedCount(99).Format());
            Assert.Equal("99+", new CappedCount(100).Format());
            Assert.True(new CappedCount(100).IsOverflow);
            Assert.False(new CappedCount(99).IsOverflow);
        }

        [Fact]
        public void Negative_value_is_normalized_to_zero()
        {
            var count = new CappedCount(-5, 9);

            Assert.Equal(0, count.Value);
            Assert.Equal(0, count.ClampedValue);
            Assert.False(count.IsOverflow);
            Assert.Equal("0", count.Format());
        }

        [Fact]
        public void Negative_limit_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CappedCount(1, -1));
        }

        [Fact]
        public void Equality_compares_value_and_limit()
        {
            Assert.Equal(new CappedCount(10, 9), new CappedCount(10, 9));
            Assert.NotEqual(new CappedCount(10, 9), new CappedCount(10, 99));
            Assert.NotEqual(new CappedCount(10, 9), new CappedCount(11, 9));
        }
    }
}
