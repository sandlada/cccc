using System;
using System.Globalization;

namespace Cccc.Domain.Feedback
{
    public readonly struct CappedCount : IEquatable<CappedCount>
    {
        public const int DefaultMax = 99;

        public int Value { get; }
        public int Max { get; }
        public bool IsOverflow => Value > Max;
        public int ClampedValue => IsOverflow ? Max : Value;

        public CappedCount(int value, int max = DefaultMax)
        {
            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "Upper limit must be non-negative.");
            }

            Value = value < 0 ? 0 : value;
            Max = max;
        }

        public string Format() =>
            IsOverflow
                ? Max.ToString(CultureInfo.InvariantCulture) + "+"
                : Value.ToString(CultureInfo.InvariantCulture);

        public string Format(string prefix) =>
            string.IsNullOrEmpty(prefix) ? Format() : prefix + Format();

        public override string ToString() => Format();

        public bool Equals(CappedCount other) => Value == other.Value && Max == other.Max;
        public override bool Equals(object obj) => obj is CappedCount other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Value, Max);

        public static bool operator ==(CappedCount left, CappedCount right) => left.Equals(right);
        public static bool operator !=(CappedCount left, CappedCount right) => !left.Equals(right);
    }
}
