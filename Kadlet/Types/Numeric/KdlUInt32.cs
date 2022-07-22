namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="uint"/>.
    /// </summary>
    public class KdlUInt32 : KdlNumber<uint> 
    {
        public KdlUInt32(uint value, string source, string? type = null) : base(value, source, type) {
        }
    }
}