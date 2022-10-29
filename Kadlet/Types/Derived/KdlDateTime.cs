#pragma warning disable CS0659

using System;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="DateTime"/>.
    /// </summary>
    public class KdlDateTime : KdlValue<DateTime>
    {
        public KdlDateTime(DateTime value, string? type = null) : base(value, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlDateTime other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}