#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="ushort"/>.
    /// </summary>
    public class KdlUInt16 : KdlNumber<ushort> 
    {
        public KdlUInt16(ushort value, string source, string? type = null) : base(value, source, type) {}
        public KdlUInt16(ushort value, string? type = null) : base(value, type) {}

        public override bool Equals(object? obj) {
            return obj is KdlUInt16 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}