namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="uint"/>.
    /// </summary>
    public class KdlUInt32 : KdlNumber<uint> 
    {
        public KdlUInt32(uint value, string? type = null) : base(value, type) {
        }
    }
}