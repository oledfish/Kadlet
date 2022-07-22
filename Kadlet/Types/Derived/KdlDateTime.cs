using System;
using System.IO;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="DateTime"/>.
    /// </summary>
    public class KdlDateTime : KdlValue<DateTime>
    {
        public KdlDateTime(DateTime value, string? type = null) : base(value, type) {
        }
    }
}