using System.IO;
using System.Diagnostics;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> with some extra fields for numbers.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public abstract class KdlNumber<T> : KdlValue<T>
    {   
        public string? SourceString { get; } 
        protected bool HasPoint = false;
        protected bool HasExponent = false;
        protected bool OnlyZeroes = false;
        
        public KdlNumber(T value, string source, string? type = null) : base(value, type) {
            SourceString = source;
        }

        public KdlNumber(T value, string? type = null) : base(value, type) {
            SourceString = null;
        }

        public override void Write(TextWriter writer, KdlPrintOptions options) {
            WriteType(writer, options);
            WriteValue(writer, options);
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            if (options.KeepRadix && SourceString != null) {
                if (SourceString.StartsWith("0x") || SourceString.StartsWith("0b") || SourceString.StartsWith("0o")) {
                    writer.Write(SourceString);
                    return;
                }
            }

            writer.Write(Value?.ToString() ?? string.Empty);
        }
    }
}