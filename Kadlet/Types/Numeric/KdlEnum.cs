using System;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping an enum.
    public class KdlEnum : KdlValue<Enum> 
    {
        public KdlEnum(Enum value, string? type = null) : base(value, type) {
        }
    }
}