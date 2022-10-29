#pragma warning disable CS0659

using System;
using System.IO;
using System.Linq;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a byte array.
    /// This can be used to hold arbitrary binary data that, if written to a document,
    /// will be represented as a Base64 encoded string.
    /// </summary>
    public class KdlByteArray : KdlValue<byte[]>
    {
        public KdlByteArray(byte[] value, string? type = null) : base(value, type) {
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            writer.Write(Convert.ToBase64String(Value));
        }

        public override bool Equals(object? obj) {
            return obj is KdlByteArray other && Value.SequenceEqual(other.Value) && Type == other.Type;
        }
    }
}