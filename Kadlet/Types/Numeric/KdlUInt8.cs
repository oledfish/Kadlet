#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="byte"/>.
    /// </summary>
    public class KdlUInt8 : KdlNumber<byte> 
    {
        public KdlUInt8(byte value, string source, string? type = null) : base(value, source, type) {}
        public KdlUInt8(byte value, string? type = null) : base(value, type) {}

        public override bool Equals(object? obj) {
            return obj is KdlUInt8 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}