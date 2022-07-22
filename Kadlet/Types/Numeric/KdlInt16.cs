namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="short"/>.
    /// </summary>
    public class KdlInt16 : KdlNumber<short> 
    {
        public KdlInt16(short value, string? type = null) : base(value, type) {
        }
    }
}