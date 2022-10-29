#pragma warning disable CS0659

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

        public override bool Equals(object? obj) {
            return obj is KdlIp other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}