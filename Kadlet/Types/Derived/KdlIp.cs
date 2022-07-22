using System.IO;
using System.Net;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an <see cref="IPAddress"/>.
    /// </summary>
    public class KdlIp : KdlValue<IPAddress>
    {
        public KdlIp(IPAddress value, string? type = null) : base(value, type) {
        }
    }
}