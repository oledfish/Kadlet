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
        public KdlDecimal(decimal value, string source, bool point, bool exponent, bool onlyZeroes, string? type = null)
            : base(value, source, type)
        {
            HasPoint = point;
            HasExponent = exponent;
            OnlyZeroes = onlyZeroes;
        }

        public KdlDecimal(decimal value, string source, string? type = null)
            : base(value, source, type)
        {
            HasPoint = source.Contains(".");
            HasExponent = source.Contains("E") || source.Contains("e");
            OnlyZeroes = false;
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            if (HasExponent) {
                string format = HasPoint ? "E1" : "E0";

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
    }
}