using System;

namespace Cccc.Domain.Feedback
{
    public readonly struct WindowIdentity : IEquatable<WindowIdentity>
    {
        public long Handle { get; }
        public uint ProcessId { get; }

        public WindowIdentity(long handle, uint processId)
        {
            Handle = handle;
            ProcessId = processId;
        }

        public bool IsEmpty => Handle == 0 && ProcessId == 0;

        public bool Equals(WindowIdentity other) => Handle == other.Handle && ProcessId == other.ProcessId;

        public override bool Equals(object obj) => obj is WindowIdentity other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle, ProcessId);

        public static bool operator ==(WindowIdentity left, WindowIdentity right) => left.Equals(right);
        public static bool operator !=(WindowIdentity left, WindowIdentity right) => !left.Equals(right);
    }
}
