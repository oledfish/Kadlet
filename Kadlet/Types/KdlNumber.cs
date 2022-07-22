using System.IO;
using System.Diagnostics;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> with some extra fields for numbers.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class KdlNumber<T> : KdlValue<T>
    {   
        protected bool HasPoint = false;
        protected bool HasExponent = false;
        public KdlNumber(T value, string? type = null) : base(value, type) {
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
            writer.Write(Value?.ToString() ?? string.Empty);
        }
    }
}