using System;
using System.IO;
using System.Diagnostics;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> representing a null value.
    /// </summary>
    [DebuggerDisplay("null")]
    public class KdlNull : KdlValue 
    {
        public static KdlNull Instance { get; } = new KdlNull(null);

        public KdlNull(string? type = null) : base(type) {
        }

        public override void Write(TextWriter writer, KdlPrintOptions options) {
            WriteType(writer, options);
            WriteValue(writer, options);
        }

        public override string ToKdlString() => "null";
        public override string ToKdlString(KdlPrintOptions options) => "null";

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            writer.Write("null");
        }

        public override bool Equals(object? obj) {
            return obj is KdlNull other && Type == other.Type;
        }

        public override int GetHashCode() {
            HashCode hash = new HashCode();

            if (Type != null)
                hash.Add(Type.GetHashCode());

            return hash.ToHashCode();
        }
    }
}