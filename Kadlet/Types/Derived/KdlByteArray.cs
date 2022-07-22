using System;
using System.IO;

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
    }
}