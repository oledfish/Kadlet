#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="short"/>.
    /// </summary>
    public class KdlInt16 : KdlNumber<short> 
    {
        public KdlInt16(short value, string source, string? type = null) : base(value, source, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlInt16 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}