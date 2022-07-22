namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="ulong"/>.
    /// </summary>
    public class KdlUInt64 : KdlNumber<ulong> 
    {
        public KdlUInt64(ulong value, string? type = null) : base(value, type) {
        }
    }
}