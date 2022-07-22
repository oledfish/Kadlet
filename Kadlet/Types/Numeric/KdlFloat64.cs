using System;
using System.IO;
using System.Globalization;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="double"/>.
    /// </summary>
    public class KdlFloat64 : KdlNumber<double> 
    {
        public KdlFloat64(double value, bool point, bool exponent, string? type = null) : base(value, type) {
            HasPoint = point;
            HasExponent = exponent;
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