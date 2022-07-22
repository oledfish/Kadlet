namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="byte"/>.
    /// </summary>
    public class KdlUInt8 : KdlNumber<byte> 
    {
        public KdlUInt8(byte value, string source, string? type = null) : base(value, source, type) {
        }
    }
}