namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="long"/>.
    /// </summary>
    public class KdlInt64 : KdlNumber<long> 
    {
        public KdlInt64(long value, string source, string? type = null) : base(value, source, type) {
        }
    }
}