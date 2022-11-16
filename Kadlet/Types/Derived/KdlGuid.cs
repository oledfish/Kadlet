#pragma warning disable CS0659

using System;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="Guid"/>.
    /// </summary>
    public class KdlGuid : KdlValue<Guid>
    {
        public KdlGuid(Guid value, string? type = null) : base(value, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlGuid other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}