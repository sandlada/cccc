using System;

namespace Cccc.Domain.Feedback
{
    public readonly struct ClipboardSequence : IEquatable<ClipboardSequence>
    {
        public uint Value { get; }

        public ClipboardSequence(uint value)
        {
            Value = value;
        }

        public bool Equals(ClipboardSequence other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ClipboardSequence other && Equals(other);
        public override int GetHashCode() => (int)Value;
        public override string ToString() => Value.ToString();

        public static bool operator ==(ClipboardSequence left, ClipboardSequence right) => left.Equals(right);
        public static bool operator !=(ClipboardSequence left, ClipboardSequence right) => !left.Equals(right);
    }
}
