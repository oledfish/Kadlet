using System;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kadlet
{
    /// <summary>
    /// Internal utility class meant to facilitate converting from strings into appropriate <see cref="KdlValue"/>s.
    /// </summary>
    internal static class KdlConvert
    {
        public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");

        public static KdlValue ToBoolOrNull(string input, string? type) {
            switch (input) {
                case "true":
                    return new KdlBool(true, type);
                case "false":
                    return new KdlBool(false, type);
                case "null":
                    return new KdlNull(type);
                default:
                    throw new KdlException($"Invalid literal '{input}', must be 'true', 'false', or 'null'", null);
            }
        }

        #region Numeric
        public static KdlFloat32 ToFloat32(string input, int radix, DecimalResult result, string? type) {
            if (radix != 10) {
                throw new KdlException($"Floating point numbers must be in base 10, received {input}.", null);
            }

            return new KdlFloat32(Convert.ToSingle(input, Culture), input, result.HasPoint, result.HasExponent, result.OnlyZeroes, type);
        }
        
        public static KdlFloat64 ToFloat64(string input, int radix, DecimalResult result, string? type) {
            if (radix != 10) {
                throw new KdlException($"Floating point numbers must be in base 10, received {input}.", null);
            }

            return new KdlFloat64(Convert.ToDouble(input, Culture), input, result.HasPoint, result.HasExponent, result.OnlyZeroes, type);
        }

        public static KdlDecimal ToDecimal(string input, int radix, DecimalResult result, string? type) {
            if (radix != 10) {
                throw new KdlException($"Floating point numbers must be in base 10, received {input}.", null);
            }

            NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;
            decimal number = Decimal.Parse(input, styles, Culture);
            return new KdlDecimal(number, input, result.HasPoint, result.HasExponent, result.OnlyZeroes, type);
        }

        public static KdlUInt8 ToUInt8(string input, int radix, string? type = null) {
            return new KdlUInt8(Convert.ToByte(input, radix), input, type);
        }
        
        public static KdlUInt16 ToUInt16(string input, int radix, string? type = null) {
            return new KdlUInt16(Convert.ToUInt16(input, radix), input, type);
        }
        
        public static KdlUInt32 ToUInt32(string input, int radix, string? type = null) {
            return new KdlUInt32(Convert.ToUInt16(input, radix), input, type);
        }
        
        public static KdlUInt64 ToUInt64(string input, int radix, string? type = null) {
            return new KdlUInt64(Convert.ToByte(input, radix), input, type);
        }
        
        public static KdlInt8 ToInt8(string input, sbyte sign, int radix, string? type = null) {
            return new KdlInt8((sbyte) (Convert.ToSByte(input, radix) * sign), input, type);
        }
        
        public static KdlInt16 ToInt16(string input, short sign, int radix, string? type = null) {
            return new KdlInt16((short) (Convert.ToInt16(input, radix) * sign), input, type);
        }
        
        public static KdlInt32 ToInt32(string input, int sign, int radix, string? type = null) {
            return new KdlInt32(Convert.ToInt16(input, radix) * sign, input, type);
        }
        
        public static KdlInt64 ToInt64(string input, int sign, int radix, string? type = null) {
            return new KdlInt64(Convert.ToInt64(input, radix) * sign, input, type);
        }

        #endregion

        #region Derived

        private static DateTime ParseDateTime(string input) {
            return DateTime.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static KdlValue ToDateTime(string input, string type, KdlReaderOptions options) {
            DateTime datetime = ParseDateTime(input);
            return new KdlDateTime(datetime, type);
        }

        public static KdlValue ToTime(string input, string type, KdlReaderOptions options) {
            DateTime datetime = ParseDateTime(input);
            return new KdlTimeSpan(datetime.TimeOfDay, type);
        }

        public static KdlValue ToDate(string input, string type, KdlReaderOptions options) {
            DateTime datetime = ParseDateTime(input);
            return new KdlDateTime(datetime.Date, type);
        }

        public static KdlValue ToDecimal(string input, string type, KdlReaderOptions options) {
            bool point = input.Contains(".");
            bool exponent = input.Contains("e") || input.Contains("E");

            NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;
            decimal number = Decimal.Parse(input, styles, Culture);
            return new KdlDecimal(number, input, type);
        }

        public static KdlValue ToIpAddress(string input, string type, KdlReaderOptions options) {
            IPAddress ip = IPAddress.Parse(input);

            // IPv6 has ScopeId, IPv4 doesn't, we try this to verify we didn't parse the wrong type
            try {
                long scope = ip.ScopeId; // This throws if it's IPv4
                
                if (type == "ipv4") {
                    throw new KdlException($"Tried to parse an IPv4 value but found IPv6 '{input}'.", null);
                }
            } catch (SocketException) {
                if (type == "ipv6") {
                    throw new KdlException($"Tried to parse an IPv6 value but found IPv4 '{input}'.", null);
                }
            }

            return new KdlIp(ip, type);
        }

        public static KdlValue ToRegex(string input, string type, KdlReaderOptions options) {
            Regex regex = new Regex(input, options.RegexOptions);
            return new KdlRegex(regex, type);
        }

        public static KdlValue ToBase64(string input, string type, KdlReaderOptions options) {
            byte[] array = Convert.FromBase64String(input);
            return new KdlByteArray(array, type);
        }

        #endregion
    }
}