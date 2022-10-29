#pragma warning disable CS0659

using System;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="TimeSpan"/>.
    /// </summary>
    public class KdlTimeSpan : KdlValue<TimeSpan>
    {
        public KdlTimeSpan(TimeSpan value, string? type = null) : base(value, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlTimeSpan other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}