#pragma warning disable CS0659

using System;
using System.IO;
using System.Globalization;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="float"/>.
    /// </summary>
    public class KdlFloat32 : KdlNumber<float> 
    {
        internal KdlFloat32(float value, string source, KdlDecimalFormat format, string? type = null)
            : base(value, source, type)
        {
            Format = format;
        }

        public KdlFloat32(float value, string? type = null) : base(value, type) { }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            // Edge cases where a value too large or too small was rounded to infinity or zero,
            // we rely on the original string to round-trip
            if (SourceString != null && (Single.IsInfinity(Value) || Value == 0 && !Format.HasFlag(KdlDecimalFormat.OnlyZeroes))) {
                writer.Write(SourceString
                    .Replace('E', options.ExponentChar)
                    .Replace('e', options.ExponentChar)
                );

                return;
            }

            // Safeguards for values constructed manually, so that a round-trip can work
            if (SourceString == null) {
                if (Single.IsPositiveInfinity(Value)) {
                    writer.Write(Single.MaxValue.ToString(CultureInfo.GetCultureInfo("en-US"))
                        .Replace('E', options.ExponentChar)
                        .Replace('e', options.ExponentChar)
                    );

                    return;
                }

                if (Single.IsNegativeInfinity(Value)) {
                    writer.Write(Single.MinValue.ToString(CultureInfo.GetCultureInfo("en-US"))
                        .Replace('E', options.ExponentChar)
                        .Replace('e', options.ExponentChar)
                    );

                    return;
                }

                if (Single.IsNaN(Value)) {
                    writer.Write("0.0");
                    return;
                }
            }

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
            return obj is KdlFloat32 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}