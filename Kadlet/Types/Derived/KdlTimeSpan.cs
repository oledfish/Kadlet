using System;
using System.IO;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="TimeSpan"/>.
    /// </summary>
    public class KdlTimeSpan : KdlValue<TimeSpan>
    {
        public KdlTimeSpan(TimeSpan value, string? type = null) : base(value, type) {
        }
    }
}