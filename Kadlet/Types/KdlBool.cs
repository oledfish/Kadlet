using System.IO;
using System.Diagnostics;

namespace Kadlet
{

    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="bool"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class KdlBool : KdlValue<bool>
    {
        public KdlBool(bool value, string? type = null) : base(value, type) {
        }

        public override void Write(TextWriter writer, KdlPrintOptions options)
        {
            if (Type != null) {
                writer.Write('(');
                Util.WriteQuotedIdentifier(writer, Type, options);
                writer.Write(')');
            }

            WriteValue(writer, options);
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            writer.Write((Value) ? "true" : "false");
        }
    }
}