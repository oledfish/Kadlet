#pragma warning disable CS0659

using System.Numerics;
using System.Diagnostics;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="BigInteger"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class KdlBigInteger : KdlNumber<BigInteger> 
    {
        public KdlBigInteger(BigInteger value, string source, string? type = null) : base(value, source, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlBigInteger other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}