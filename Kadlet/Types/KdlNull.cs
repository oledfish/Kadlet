using System.IO;
using System.Diagnostics;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> representing a null value.
    /// </summary>
    [DebuggerDisplay("null")]
    public class KdlNull : KdlValue {
        public KdlNull(string? type = null) : base(type) {
        }

        public override void Write(TextWriter writer, KdlPrintOptions options) {
            if (Type != null) {
                writer.Write('(');
                Util.WriteQuotedIdentifier(writer, Type, options);
                writer.Write(')');
            }

            WriteValue(writer, options);
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            writer.Write("null");
        }
    }
}