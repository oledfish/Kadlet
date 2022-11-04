using System.IO;

namespace Kadlet
{
    /// <summary>
    /// An internal utility class providing range checks for characters and string manipulation methods.
    /// </summary>
    internal static class Util 
    {
        internal static readonly int EOF = -1;
        internal static readonly int BOM = 0xFEFF;
        internal static readonly int MinUnicode = 0x20;
        internal static readonly int MaxUnicode = 0x10FFFF;

        /// <summary>
        /// Checks whether a string is "true", "false", or "null".
        /// </summary>
        internal static bool IsKeyword(string str) {
            return string.Equals(str, "null") || string.Equals(str, "true") || string.Equals(str, "false");
        }

        /// <summary>
        /// Checks whether a character is found in "true", "false", or "null"
        /// </summary>
        internal static bool IsKeywordCharacter(int c) {
            switch (c) {
                case 'n':
                case 'u':
                case 'l':
                case 't':
                case 'r':
                case 'e':
                case 'f':
                case 'a':
                case 's':
                    return true;
                default:
                    return false;
            }
        }


        /// <summary>
        /// Checks whether a character is a digit between 0 and 9.
        /// </summary>
        internal static bool IsDecimalDigit(int c) {
            return (c >= '0' && c <= '9');
        }

        /// <summary>
        /// Checks whether a character is 0 or 1.
        /// </summary>
        internal static bool IsBinaryDigit(int c) {
            return (c == '0') || (c == '1');
        }

        /// <summary>
        /// Checks whether a character is a digit between 0 and 7.
        /// </summary>
        internal static bool IsOctalDigit(int c) {
            return (c >= '0' && c <= '7');
        }

        /// <summary>
        /// Checks whether a character is a digit between 0 and 9, or letters from a-f or A-F.
        /// </summary>
        internal static bool IsHexadecimalDigit(int c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        /// <summary>
        /// Checks whether a character is a digit between 0 and 9, an exponent marker ('E' or 'e'), or a plus or minus sign.
        /// </summary>
        internal static bool IsFloatingDigit(int c) {
            return (c == 'e' || c == 'E') || (c == '+' || c == '-') || (c == '.');
        }

        /// <summary>
        /// Checks whether a character is a digit between 0 and 9 or a plus or minus sign.
        /// </summary>
        internal static bool IsValidInitialNumeric(int c) {
            return (c >= '0' && c <= '9') || (c == '+' || c == '-');
        }

        /// <summary>
        /// Checks whether a character can start a bare identifier.
        /// </summary>
        internal static bool IsValidInitialCharacter(int c) {
            return !IsDecimalDigit(c) && IsValidIdentifierCharacter(c);
        }

        /// <summary>
        /// Checks whether a character is valid for a bare identifier.
        /// </summary>
        internal static bool IsValidIdentifierCharacter(int c) {
            if (c <= MinUnicode || c > MaxUnicode) {
                return false;
            }

            switch (c) {
                case '\\':
                case '/':
                case '(':
                case ')':
                case '{':
                case '}':
                case '<':
                case '>':
                case ';':
                case '[':
                case ']':
                case '=':
                case ',':
                case '"':
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Checks whether a character represents empty space.
        /// </summary>
        internal static bool IsWhitespace(int c) {
            switch (c) {
                case '\u0009': // Character Tabulation
                case '\u0020': // Space
                case '\u00A0': // No-Break Space
                case '\u1680': // Ogham Space Mark
                case '\u2000': // En Quad
                case '\u2001': // Em Quad
                case '\u2002': // En Space
                case '\u2003': // Em Space
                case '\u2004': // Three-Per-Em Space
                case '\u2005': // Four-Per-Em Space
                case '\u2006': // Six-Per-Em Space
                case '\u2007': // Figure Space
                case '\u2008': // Punctuation Space
                case '\u2009': // Thin Space
                case '\u200A': // Hair Space
                case '\u202F': // Narrow No-Break Space
                case '\u205F': // Medium Mathematical Space
                case '\u3000': // Ideographic Space
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether a character represents a newline.
        /// </summary>
        internal static bool IsNewline(int c) {
            switch (c) {
                case '\u000D': // Carriage Return
                case '\u000A': // Line Feed
                case '\u0085': // Next Line
                case '\u000C': // Form Feed
                case '\u2028': // Line Separator
                case '\u2029': // Paragaph Separator
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether a character can end a node, which can be a semicolon, a newline, or EOF.
        /// </summary>
        internal static bool IsNodeTerminator(int c) {
            return c == EOF || c == ';' || IsNewline(c);
        }

        /// <summary>
        /// Checks whether a string is a valid bare identifier.
        /// </summary>
        internal static bool IsBareIdentifier(string id) {
            if (id.Length == 0) {
                return false;
            }

            if (!IsValidInitialCharacter(id[0])) {
                return false;
            }

            if (id.Length > 1) {
                if (id[0] == '+' || id[0] == '-') {
                    return !IsDecimalDigit(id[1]);
                }

                foreach (char c in id.Substring(1)) {
                    if (!IsValidIdentifierCharacter(c)) {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Writes a string escaping some characters according to the settings in KdlPrintOptions.
        /// </summary>
        internal static void WriteEscapedString(TextWriter writer, string str, KdlPrintOptions options) {
            foreach (int c in str) {
                writer.Write(EscapeCharacter(c, options));
            }
        }

        /// <summary>
        /// Returns the escaped representation of a character, if needed, otherwise the same character is returned.
        /// </summary>
        internal static string EscapeCharacter(int c, KdlPrintOptions options) {
            bool escape = false;
            if (c == '\\' || c == '\"') {
                escape = true;
            } else if (options.EscapeLinespace && IsNewline(c)) {
                escape = true;
            } else if (options.EscapeCommon && IsCommonEscape(c)) {
                escape = true;
            } else if (options.EscapeNonPrintableAscii && c < 32) {
                escape = true;
            } else if (options.EscapeNonAscii && c > 127) {
                escape = true;
            } else if (options.EscapePotentiallyDangerousUnicode && IsPotentiallyDangerousUnicode(c)) {
                escape = true;
            }

            if (escape) {
                switch (c) {
                    case '\\':
                        return "\\\\";
                    case '\"':
                        return "\\\"";
                    case '\r':
                        return "\\r";
                    case '\n':
                        return "\\n";
                    case '\t':
                        return "\\t";
                    case '\b':
                        return "\\b";
                    case '\f':
                        return "\\f";
                    default:
                        return $"\\u{'{'}{c:x}{'}'}";
                }
            }

            return ((char) c).ToString();
        }

        /// <summary>
        /// Checks whether the character is CR, LF, a backspace, a tab or a form feed.
        /// </summary>
        internal static bool IsCommonEscape(int c) {
            switch (c) {
                case '\\':
                case '\r':
                case '\n':
                case '\t':
                case '\b':
                case '\f':
                case '"':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the character is Unicode values U+202A to U+202E, or U+2066 to +U2069.
        /// </summary>
        internal static bool IsPotentiallyDangerousUnicode(int c) {
            switch (c) {
                case '\u202A':
                case '\u202B':
                case '\u202C':
                case '\u202D':
                case '\u202E':
                case '\u2066':
                case '\u2067':
                case '\u2068':
                case '\u2069':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Writes an identifier. If it's a valid bare identifier it is printed as-is, otherwise
        /// it's printed as a quoted escaped string.
        /// </summary>
        internal static void WriteQuotedIdentifier(TextWriter writer, string id, KdlPrintOptions options) {
            if (IsBareIdentifier(id)) {
                writer.Write(id);
            } else {
                writer.Write('"');
                WriteEscapedString(writer, id, options);
                writer.Write('"');
            }
        }
    }
}