#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="long"/>.
    /// </summary>
    public class KdlInt64 : KdlNumber<long> 
    {
        public KdlInt64(long value, string source, string? type = null) : base(value, source, type) {}
        public KdlInt64(long value, string? type = null) : base(value, type) {}

        public override bool Equals(object? obj) {
            return obj is KdlInt64 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}