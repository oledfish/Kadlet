#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="sbyte"/>.
    /// </summary>
    public class KdlInt8 : KdlNumber<sbyte> 
    {
        public KdlInt8(sbyte value, string source, string? type = null) : base(value, source, type) {}
        public KdlInt8(sbyte value, string? type = null) : base(value, type) {}

        public override bool Equals(object? obj) {
            return obj is KdlInt8 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}