#pragma warning disable CS0659

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="int"/>.
    /// </summary>
    public class KdlInt32 : KdlNumber<int> 
    {
        public KdlInt32(int value, string source, string? type = null) : base(value, source, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlInt32 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}