#pragma warning disable CS0659

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
        public static KdlBool True { get; } = new KdlBool(true, null);
        public static KdlBool False { get; } = new KdlBool(false, null);
        
        public KdlBool(bool value, string? type = null) : base(value, type) {
        }

        public override void Write(TextWriter writer, KdlPrintOptions options) {
            WriteType(writer, options);
            WriteValue(writer, options);
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            writer.Write((Value) ? "true" : "false");
        }

        public override bool Equals(object? obj) {
            return obj is KdlBool other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}