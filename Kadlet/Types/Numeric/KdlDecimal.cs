#pragma warning disable CS0659

using System;
using System.IO;
using System.Globalization;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="decimal"/>.
    /// </summary>
    public class KdlDecimal : KdlNumber<decimal> 
    {
        internal KdlDecimal(decimal value, string source, KdlDecimalFormat format, string? type = null)
            : base(value, source, type)
        {
            Format = format;
        }

        public KdlDecimal(decimal value, string? type = null) : base(value, type) { }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            if (Format.HasFlag(KdlDecimalFormat.HasExponent)) {
                string format = Format.HasFlag(KdlDecimalFormat.HasPoint) ? "E1" : "E0";

                writer.Write(Value
                    .ToString(format, CultureInfo.GetCultureInfo("en-US"))
                    .Replace("E+0", "E+")
                    .Replace("E-0", "E-")
                    .Replace('E', options.ExponentChar)
                );
            } else {
                if (Math.Truncate(Value) != Value) {
                    writer.Write(Value.ToString("G", CultureInfo.GetCultureInfo("en-US")));
                } else {
                    writer.Write(Value.ToString("0.0", CultureInfo.GetCultureInfo("en-US")));
                }
            }
        }

        public override bool Equals(object? obj) {
            return obj is KdlDecimal other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}