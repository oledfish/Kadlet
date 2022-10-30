#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="ulong"/>.
    /// </summary>
    public class KdlUInt64 : KdlNumber<ulong> 
    {
        public KdlUInt64(ulong value, string source, string? type = null) : base(value, source, type) {}
        public KdlUInt64(ulong value, string? type = null) : base(value, type) {}

        public override bool Equals(object? obj) {
            return obj is KdlUInt64 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}