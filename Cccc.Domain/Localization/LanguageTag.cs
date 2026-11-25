using System;

namespace Cccc.Domain.Localization
{
    public readonly struct LanguageTag : IEquatable<LanguageTag>
    {
        public const string SystemValue = "system";
        public const string EnglishValue = "en";
        public const int MaxLength = 35;

        public static readonly LanguageTag System = new(SystemValue);
        public static readonly LanguageTag English = new(EnglishValue);

        private readonly string value;

        public LanguageTag(string value)
        {
            string normalized = Normalize(value);
            this.value = normalized ?? SystemValue;
        }

        public string Value => value ?? SystemValue;

        public bool IsSystem => string.Equals(Value, SystemValue, StringComparison.OrdinalIgnoreCase);

        public static string Normalize(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            string trimmed = raw.Trim().Replace('_', '-');
            if (trimmed.Length == 0 || trimmed.Length > MaxLength) return null;

            string[] segments = trimmed.Split('-');
            foreach (string segment in segments)
            {
                if (segment.Length == 0) return null;
                foreach (char c in segment)
                {
                    bool valid = (c >= 'a' && c <= 'z') ||
                        (c >= 'A' && c <= 'Z') ||
                        (c >= '0' && c <= '9');
                    if (!valid) return null;
                }
            }
            return trimmed;
        }

        public static LanguageTag NormalizePreference(string raw)
        {
            string normalized = Normalize(raw);
            if (normalized is null || string.Equals(normalized, SystemValue, StringComparison.OrdinalIgnoreCase))
                return System;
            return new LanguageTag(normalized);
        }

        public bool Equals(LanguageTag other) =>
            string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is LanguageTag other && Equals(other);

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        public override string ToString() => Value;

        public static bool operator ==(LanguageTag left, LanguageTag right) => left.Equals(right);

        public static bool operator !=(LanguageTag left, LanguageTag right) => !left.Equals(right);
    }
}
