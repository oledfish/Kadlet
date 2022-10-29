#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="uint"/>.
    /// </summary>
    public class KdlUInt32 : KdlNumber<uint> 
    {
        public KdlUInt32(uint value, string source, string? type = null) : base(value, source, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlUInt32 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}