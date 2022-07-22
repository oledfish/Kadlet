namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="ushort"/>.
    /// </summary>
    public class KdlUInt16 : KdlNumber<ushort> 
    {
        public KdlUInt16(ushort value, string source, string? type = null) : base(value, source, type) {
        }
    }
}