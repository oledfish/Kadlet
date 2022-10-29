#pragma warning disable CS0659

using System;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an enum.
    public class KdlEnum : KdlValue<Enum>
    {
        public KdlEnum(Enum value, string? type = null) : base(value, type) {
        }

        public override bool Equals(object? obj) {
            return obj is KdlEnum other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}