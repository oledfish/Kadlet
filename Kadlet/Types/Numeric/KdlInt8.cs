namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="sbyte"/>.
    /// </summary>
    public class KdlInt8 : KdlNumber<sbyte> 
    {
        public KdlInt8(sbyte value, string source, string? type = null) : base(value, source, type) {
        }
    }
}