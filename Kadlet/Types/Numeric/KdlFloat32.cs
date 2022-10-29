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
        public KdlFloat32(float value, string source, bool point, bool exponent, bool onlyZeroes, string? type = null)
            : base(value, source, type)
        {
            HasPoint = point;
            HasExponent = exponent;
            OnlyZeroes = onlyZeroes;
        }

        public KdlFloat32(float value, string source, string? type = null)
            : base(value, source, type)
        {
            HasPoint = source.Contains(".");
            HasExponent = source.Contains("E") || source.Contains("e");
            OnlyZeroes = false;
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            // Edge cases where a value too large or too small was rounded to infinity or zero
            if (Single.IsInfinity(Value) || Value == 0 && !OnlyZeroes) {
                writer.Write(SourceString
                    .Replace('E', options.ExponentChar)
                    .Replace('e', options.ExponentChar)
                );

                return;
            }

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

        public override bool Equals(object? obj) {
            return obj is KdlFloat32 other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}