namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="int"/>.
    /// </summary>
    public class KdlInt32 : KdlNumber<int> 
    {
        public KdlInt32(int value, string? type = null) : base(value, type) {
        }
    }
}