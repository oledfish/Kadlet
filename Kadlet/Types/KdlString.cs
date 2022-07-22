using System;
using System.IO;
using System.Diagnostics;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="string"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class KdlString : KdlValue<string>
    {
        public KdlString(string value, string? type = null) : base(value, type) {
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            Util.WriteEscapedString(writer, Value, options);
        }
    }
}