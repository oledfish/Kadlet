using System;
using System.IO;
using System.Text;
using System.Numerics;
using System.Globalization;
using System.Collections.Generic;

namespace Kadlet
{
    /// <summary>
    /// A object used to transform valid KDL into a <see cref="KdlDocument"/>.
    /// </summary>
    public class KdlReader
    {
        private readonly KdlReaderOptions _options = new KdlReaderOptions{};

        public KdlReader() {
            _options.TypeConverters = new Dictionary<string, KdlTypeConverter>();
            _options.TypeConverters["date-time"] = KdlConvert.ToDateTime;
            _options.TypeConverters["time"] = KdlConvert.ToTime;
            _options.TypeConverters["date"] = KdlConvert.ToDate;
            _options.TypeConverters["decimal"] = KdlConvert.ToDecimal;
            _options.TypeConverters["ipv4"] = KdlConvert.ToIpAddress;
            _options.TypeConverters["ipv6"] = KdlConvert.ToIpAddress;
            _options.TypeConverters["regex"] = KdlConvert.ToRegex;
            _options.TypeConverters["base64"] = KdlConvert.ToBase64;
        }

        public KdlReader(KdlReaderOptions options) : base() {
            if (options.UseTypeAnnotations) {
                foreach (KeyValuePair<string, KdlTypeConverter> kv in options.TypeConverters) {
                    _options.TypeConverters[kv.Key] = kv.Value;
                }
            } else {
                _options.TypeConverters.Clear();
            }

            _options = options;
        }

        /// <summary>
        /// Parses the source into a KdlDocument.
        /// </summary>
        /// <exception cref="KdlException"/>
        public KdlDocument Parse(TextReader reader) {
            return Parse(new KdlParseContext(reader));
        }
        
        /// <summary>
        /// Parses the source into a KdlDocument. The Stream is read in its entirety.
        /// </summary>
        /// <exception cref="KdlException"/>
        public KdlDocument Parse(Stream stream) {
            return Parse(new StreamReader(stream));
        }

        /// <summary>
        /// Parses the source into a KdlDocument.
        /// </summary>
        /// <exception cref="KdlException"/>
        public KdlDocument Parse(string str) {
            return Parse(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(str)), Encoding.UTF8));
        }

        /// <summary>
        /// Parses the source into a KdlDocument. Called internally too when parsing child nodes.
        /// </summary>
        /// <exception cref="KdlException"/>
        internal KdlDocument Parse(KdlParseContext context, int level = 0) {
            List<KdlNode> nodes = new List<KdlNode>();
            SourceOffset spanStart = context.Offset;
            SourceOffset spanEnd = context.Offset;

            int c;
            while (true) {
                ParseLinespace(context);
                
                c = context.Peek();
                if (c == Util.EOF) {
                    // EOF is only an error if we expect a '}'.
                    if (level > 0) {
                        throw new KdlException("Expected to close child node but found EOF instead.", context);
                    }

                    c = context.Read();
                    spanEnd = context.Offset;

                    break;
                }

                // In case we're parsing child nodes
                if (c == '}') {
                    if (level > 0) {
                        spanEnd = context.Offset;
                        context.Read();
                        break;
                    } else {
                        throw new KdlException("Unexpected stray '}'.", context);
                    }
                }

                EscapeNodespaceResult result = ParseEscapeAndNodespace(context);
                KdlNode node = ParseNode(context, level);

                // Ignore if we have an escape.
                if (!result.Escape && node != null) {
                    nodes.Add(node);
                }
            };

            KdlDocument document = new KdlDocument(nodes);
            document.SourceSpan = new SourceSpan(spanStart, spanEnd);

            return document;
        }

        /// <summary>
        /// Parses a node, including its arguments, properties and children.
        /// </summary>
        /// <exception cref="KdlException"/>
        internal KdlNode ParseNode(KdlParseContext context, int level = 0) {
            SourceOffset nodeSpanStart = context.Offset;

            string? type = ParseType(context);
            string identifier = ParseIdentifier(context).Name;

            if (Util.IsKeyword(identifier)) {
                throw new KdlException("A node name cannot be confused by a true, false or null value.", context);
            }

            List<KdlValue> args = new List<KdlValue>();
            Dictionary<string, KdlValue> props = new Dictionary<string, KdlValue>();
            KdlDocument? children = null;

            while (true) {
                // Look for nodespace following the identifier
                bool nodespace = ParseNodespace(context);

                int c = context.Peek();

                // Check for a semicolon, a newline, or EOF
                if (Util.IsNodeTerminator(c)) {
                    if (c == ';') {
                        context.Read();
                    }

                    break;
                } else if (c == '/') {
                    c = context.Read();
                    c = context.Peek();

                    if (c == '/') {
                        context.Read();
                        ParseLineComment(context);
                        break;
                    } else if (c == '-') {
                        context.Unread('/');
                    } else {
                        throw new KdlException($"Unexpected character after node: {(char)c}", context);
                    }
                }

                // Look for an escape and any following nodespace
                EscapeNodespaceResult result = ParseEscapeAndNodespace(context);
                nodespace |= result.Found;

                // We try parsing a child node
                c = context.Peek();
                if (c == '{') {
                    context.Read();
                    KdlDocument document = Parse(context, level + 1);

                    // Ignore if there was an escape immediately behind
                    if (!result.Escape) {
                        children = document;
                    }
                    
                    // Optionally, there may be whitespace after the closing '}', followed by possible nodespace, or a terminator
                    ParseWhitespace(context);
                    ParseNodespace(context);

                    c = context.Peek();
                    if (Util.IsNodeTerminator(c)) {
                        if (c == ';') {
                            context.Read();
                        }
                    }

                    break;
                }

                // In some edge cases, a value will be returned before it becomes invalid,
                // but the tokens following immediately compose a valid value.
                // This must throw, and we do it by checking if there's no space between.
                if (!nodespace) {
                    c = context.Read();
                    throw new KdlException("Arguments or properties must have space between them.", context);
                }

                SourceOffset spanStart = context.Offset;
                IKdlObject obj = ParseArgumentOrProperty(context);
                SourceOffset spanEnd = context.Offset;

                // Ignore if we have an escape
                if (!result.Escape && obj != null) {
                    if (obj is KdlProperty property) {
                        props[property.Key] = property.Value;
                        props[property.Key].SourceSpan = new SourceSpan(spanStart, spanEnd);
                    }

                    if (obj is KdlValue argument) {
                        argument.SourceSpan = new SourceSpan(spanStart, spanEnd);
                        args.Add(argument);
                    }
                }
            }

            SourceOffset nodeSpanEnd = context.Offset;

            KdlNode node = new KdlNode(identifier, type, args, props, children, level);
            node.SourceSpan = new SourceSpan(nodeSpanStart, nodeSpanEnd);
            return node;
        }

        /// <summary>
        /// Parses an identifier. Used by ParseNode to identify node names, string values and argument names.
        /// Since identifiers can be strings, raw strings, or bare identifiers, it'll throw an error
        /// if a bare identifier is thought to be used as a value, while it will be turned into a KdlString otherwise.
        /// </summary>
        /// <exception cref="KdlException"/>
        internal KdlIdentifier ParseIdentifier(KdlParseContext context) {
            StringBuilder builder = new StringBuilder();
            int c = context.Peek();

            // First try to parse an escaped string
            if (c == '"') {
                return new KdlIdentifier(ParseString(context, null).Value, true);
            }

            // Otherwise, we check for the start of a bare identifier
            if (!Util.IsValidInitialCharacter(c)) {
                throw new KdlException("Identifiers cannot start with a digit or an invalid identifier character (\\/(){}<>;[]=,), found " + $"({(char) c}).", context);
            }

            // An 'r' can continue into a raw string or a bare identifier, we try a raw string first
            if (c == 'r') {
                context.Read();
                c = context.Peek();

                if (c == '#' || c == '"') {
                    try {
                        return new KdlIdentifier(ParseRawString(context, null).Value, true);
                    } catch (KdlRawStringException exception) {
                        // Edge case in which we try parsing as a raw string but suddenly
                        // it turns into an invalid one, we try parsing it again as a
                        // bare identifier.
                        builder.Append('r');
                        builder.Append('#', exception.Hashes);
                    }
                } else {
                    // Nothing suggesting a raw string, we continue as bare identifier
                    builder.Append('r');
                }
            }

            // If it starts with a sign, we must check the next number isn't a digit,
            // otherwise it would be confused by a decimal
            if (c == '+' || c == '-') {
                char sign = (char) context.Read();
                builder.Append(sign);
                
                c = context.Peek();

                if (Util.IsDecimalDigit(c)) {
                    throw new KdlIdentifierSignException("An identifier that starts with a sign cannot have a digit as the second character.", sign);
                }
            }

            // Parse out the rest of the contents
            while (true) {
                c = context.Peek();

                if (c == Util.EOF || Util.IsWhitespace(c) || c == Util.BOM) {
                    break;
                }

                // Feed characters until we find an invalid identifier character.
                if (!Util.IsValidIdentifierCharacter(c)) {
                    break;
                }

                c = context.Read();
                builder.Append((char) c);
            }

            return new KdlIdentifier(builder.ToString(), false);
        }

        /// <summary>
        /// Parses a type annotation (an identifier between parenthesis). Called at the start of parsing
        /// a node, an argument or a value.
        /// </summary>
        /// <exception cref="KdlException"/>
        internal string? ParseType(KdlParseContext context) {
            int c = context.Peek();

            // Types always start with a left parenthesis
            if (c == '(') {
                context.Read();
                string type = ParseIdentifier(context).Name;
                c = context.Read();

                if (c == ')') {
                    return type;
                } else {
                    throw new KdlException($"Expected ) to close type annotation, found '{(char) c}'.", context);
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a line comment. The stream should be after '//'. This advances the stream until a newline is found.
        /// </summary>
        internal void ParseLineComment(KdlParseContext context) {
            int c = context.Peek();
            int length = 0;

            while (true) {
                c = context.Read();

                if (Util.IsNewline(c) || c == Util.EOF) {
                    if (length == 0) {
                        throw new KdlException("Single line comments must have be at least one character long.", context);
                    }

                    break;
                }

                length++;
            }
        }

        /// <summary>
        /// Parses a block comment, including nested ones. The stream should be after the first '/*'.
        /// </summary>
        /// <exception cref="KdlException"/>
        internal void ParseBlockComment(KdlParseContext context) {
            int c = context.Read();
            int start = 1; // We already parsed the first '/*' before calling this function
            int end = 0;

            do {
                if (c == '/') {
                    c = context.Peek();
                    if (c == '*') {
                        context.Read();
                        start++;
                    }
                } else if (c == '*') {
                    c = context.Peek();
                    if (c == '/') {
                        context.Read();
                        end++;
                    }
                }

                if (start == end) {
                    break;
                }

                c = context.Read();

                if (c == Util.EOF) {
                    if (start != end) {
                        throw new KdlException("Expected to close block comment but found EOF instead.", context);
                    }
                }
            } while (start != end && c != Util.EOF);
        }

        /// <summary>
        /// Parses a value, which can be a string, a number, a boolean or null. The stream is advanced
        /// until a separator is found or the value would become invalid, in which case the following
        /// parse would throw. 
        /// </summary>
        /// <returns>
        /// A <see cref="KdlString"/>, a <see cref="KdlNumber"/>, a <see cref="KdlBool"/>, or a <see cref="KdlNull"/>. 
        /// </returns>
        /// <exception cref="KdlException"/>
        internal KdlValue? ParseValue(KdlParseContext context) {
            StringBuilder builder = new StringBuilder();
            string? type = ParseType(context);
            int c = context.Peek();


            if (c == '\"' || c == 'r') {
                KdlString str = new KdlString(string.Empty);

                // First try to parse an escaped string
                if (c == '\"') {
                    str = ParseString(context, type);
                }

                // If we have an 'r' we can safely try to parse a raw string,
                // as we know we're parsing a value and don't have to account
                // for this possibly being a bare identifier
                if (c == 'r') {
                    context.Read();
                    c = context.Peek();

                    if (c == '#' || c == '"') {
                        str = ParseRawString(context, type);
                    } else {
                        throw new KdlException($"Unexpected character '{(char) c}' when trying to parse a raw string.", context);
                    }
                }

                // If we have a type converter, return this value instead of a KdlString
                if (type != null && _options.TypeConverters.ContainsKey(type)) {
                    return _options.TypeConverters[type](str.Value, type, _options);
                } else {
                    return str;
                }
            }

            // If the value starts with a digit, or plus or minus sign, interpret it as a number
            if (Util.IsValidInitialNumeric(c)) {
                return ParseNumber(context, null, type);
            }

            // Value starts with t (possibly 'true'), f (possibly 'f'), or n (possibly 'null')
            // so we try to interpret a boolean or a null
            if (c == 't' || c == 'f'|| c == 'n') {
                while (true) {
                    c = context.Peek();

                    if (Util.IsKeywordCharacter(c)) {
                        c = context.Read();
                        builder.Append((char) c);
                    } else {
                        break;
                    }
                }

                string value = builder.ToString();
                return KdlConvert.ToBoolOrNull(value, type);
            }

            return null;
        }

        /// <summary>
        /// Parses an escaped string, delimited by quotes. Escape literals are turned into their Unicode representation.
        /// </summary>
        /// <exception cref="KdlException"/>
        internal KdlString ParseString(KdlParseContext context, string? type) {
            StringBuilder builder = new StringBuilder();
            int c = context.Peek();

            // This is likely unreachable code, but, just in case
            if (c != '\"') {
                throw new KdlException("Tried to parse string without a quotation mark.", context);
            }

            // Consume the quotation mark
            context.Read();

            // Consume the contents
            while (true) {
                c = context.Peek();

                if (c == Util.EOF) {
                    throw new KdlException("Unexpected EOF when parsing string.", context);
                }

                // Another quotation mark, end of the string
                if (c == '"') {
                    context.Read();
                    break;
                }

                // Possibly escaped character
                if (c == '\\') {
                    context.Read();
                    c = context.Peek();

                    switch (c) {
                        case 'n':
                            context.Read();
                            builder.Append('\u000A');
                            break;
                        case 'r':
                            context.Read();
                            builder.Append('\u000D');
                            break;
                        case 't':
                            context.Read();
                            builder.Append('\u0009');
                            break;
                        case '\\':
                            context.Read();
                            builder.Append('\u005C');
                            break;
                        case '/':
                            context.Read();
                            builder.Append('\u002F');
                            break;
                        case '"': 
                            context.Read();
                            builder.Append('\u0022');
                            break;
                        case 'b':
                            context.Read();
                            builder.Append('\u0008');
                            break;
                        case 'f':
                            context.Read();
                            builder.Append('\u000C');
                            break;
                        case 'u':
                            context.Read();
                            if (context.Peek() == '{') {
                                StringBuilder hex = new StringBuilder(6);
                                context.Read();

                                while (true) {
                                    c = context.Read();

                                    if (Util.IsHexadecimalDigit(c)) {
                                        hex.Append((char) c);
                                    } else if (c == '}') {
                                        break;
                                    } else {
                                        throw new KdlException("Invalid Unicode escape.", context);
                                    }
                                }

                                if (hex.Length == 0 || hex.Length > 6) {
                                    throw new KdlException($"Unicode escape must have 1 to 6 characters, got '{hex}'", context);
                                }

                                int code = Convert.ToInt32(hex.ToString(), 16);

                                if (code < 0 || code > Util.MaxUnicode) {
                                    throw new KdlException($"Unicode codepoint is not accepted, got '{code:x}'", context);
                                }

                                builder.Append((char) code);
                            } else {
                                throw new KdlException("Invalid Unicode escape.", context);
                            }
                            break;
                        default:
                            // TODO: this will be an error on KDL v2
                            builder.Append('\\');
                            break;
                    }
                } else {
                    c = context.Read();
                    builder.Append((char) c);
                }
            }

            return new KdlString(builder.ToString(), type);
        }

        /// <summary>
        /// Parses a raw string, starting with 'r', and delimited by quotes with a matching number of hashes ('#')
        /// around them. 
        /// <br/>
        /// When called, the stream should have already consumed the initial 'r'.
        /// </summary>
        /// <exception cref="KdlException"/>
        /// <exception cref="KdlIdentifierSignException">
        /// Used internally when a raw string turns out not to be valid, but can be a bare identifier.
        /// Always catched by ParseNode, in which case it's interpreted as a bare identifier.
        /// </exception>
        internal KdlString ParseRawString(KdlParseContext context, string? type) {
            StringBuilder builder = new StringBuilder();
            int c = context.Peek();
            int hashes = 0;

            // Count the number of hashes at the start
            while (true) {
                c = context.Peek();
                if (c == '#') {
                    context.Read();
                    hashes++;
                } else if (c == '"') {
                    context.Read();
                    break;
                } else {
                    // The raw string is no longer valid, but this can still be a bare identifier.
                    // This exception is always catched by ParseNode
                    throw new KdlRawStringException("Malformed raw string, expected '#' or '\"'.", hashes);
                }
            }

            // Read out the contents
            while (true) {
                c = context.Read();

                if (c == Util.EOF) {
                    throw new KdlException("Unexpected EOF when parsing raw string.", context);
                }

                // Another quotation mark, *possible* end of the string
                if (c == '"') {
                    // We check if we have any hashes following the quote, if so,
                    // we count them and see if they match what we have at the start.
                    // If not, we cancel and append everything we've read.
                    if (hashes > 0) {
                        int endHashes = 0;

                        while (endHashes < hashes && c != Util.EOF) {
                            c = context.Peek();

                            if (c == '#') {
                                context.Read();
                                endHashes++;
                            } else {
                                builder.Append('"');
                                builder.Append('#', endHashes);
                                break;
                            }
                        }

                        // Hashes match, string ends here
                        if (hashes == endHashes) {
                            return new KdlString(builder.ToString(), type);   
                        }
                    } else {
                        break;
                    }
                } else {
                    builder.Append((char) c);
                }
            }

            // Count the number of hashes at the end
            while (true) {
                c = context.Peek();
                if (c == '#') {
                    context.Read();
                    hashes--;
                } else {
                    break;
                }
            }

            // If the number of hashes don't match, throw an error
            if (hashes == 0) {
                return new KdlString(builder.ToString(), type);
            } else {
                throw new KdlException("Unequal number of hashes in raw string.", context);
            }
        }

        /// <summary>
        /// Parses a number, which can take several formats: decimal, binary, octal and hex integers as well
        /// as decimal floats.
        /// <br/>
        /// The stream can start at the sign or it can be passed as a parameter, in which case it'll start by seeking a digit.
        /// </summary>
        /// <exception cref="KdlException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        internal KdlValue ParseNumber(KdlParseContext context, char? signChar, string? type) {
            StringBuilder builder = new StringBuilder();
            int c = context.Peek();

            DecimalResult result = new DecimalResult{};

            int radix = 10;
            int sign = 1;
            bool explicitSign = false;

            // Without an explicit sign argument, we try looking for one
            // and saving it appropriately
            if (!signChar.HasValue) {
                if (c == '+' || c == '-') {
                    context.Read();

                    explicitSign = true;
                    sign = (c == '+') ? 1 : -1;
                    builder.Append((char) c);
                }
            } else {
                explicitSign = true;
                sign = (signChar == '+') ? 1 : -1;
                builder.Append((char) signChar);
            }

            // We look for a digit to start out with
            c = context.Read();
            if (Util.IsDecimalDigit(c)) {
                builder.Append((char) c);

                // If 0, we look to see if we have a radix annotation
                if (c == '0') {
                    c = context.Peek();

                    switch (c) {
                        case 'b':
                            radix = 2;
                            builder.Append((char) c);
                            context.Read();
                            break;
                        case 'o':
                            radix = 8;
                            builder.Append((char) c);
                            context.Read();
                            break;
                        case 'x':
                            radix = 16;
                            builder.Append((char) c);
                            context.Read();
                            break;
                    }
                }
            } else {
                throw new KdlException("Numbers must have at least one digit.", context);
            }

            // Binary, octal and hexadecimal parsing is rather simple
            if (radix != 10) {
                while (true) {
                    c = context.Peek();
                    if ((radix == 2 && Util.IsBinaryDigit(c)) ||
                        (radix == 8 && Util.IsOctalDigit(c)) ||
                        (radix == 16 && Util.IsHexadecimalDigit(c)) || 
                        c == '_') 
                    {
                        if (builder.Length == 2 && c == '_') {
                            throw new KdlException("Numbers cannot start with an underscore.", context);
                        }

                        c = context.Read();
                        if (c != '_') {
                            builder.Append((char) c);
                        }
                    } else {
                        break;
                    }
                }

                if (builder.Length == 1) {
                    throw new KdlException("Numbers must have at least one digit.", context);
                }
            }

            // Decimal numbers are relegated to its own method, since it can be 
            // an integer, an integer with exponent, a float, or a float with exponent
            if (radix == 10) {
                result = ParseDecimal(context);
                builder.Append(result.String);
            }

            string data = builder.ToString();
            string trim = (explicitSign) ? data.Substring(1) : data;
            trim = (radix == 10 ? trim : trim.Substring(2));

            // Type overrides, these can throw if the number isn't in a suitable format.
            if (_options.UseTypeAnnotations) {
                switch (type) {
                    case "u8":
                        return KdlConvert.ToUInt8(data, radix, type);
                    case "u16":
                        return KdlConvert.ToUInt16(data, radix, type);
                    case "u32":
                        return KdlConvert.ToUInt32(data, radix, type);
                    case "u64":
                        return KdlConvert.ToUInt64(data, radix, type);
                    case "i8":
                        return KdlConvert.ToInt8(data, (sbyte) sign, radix, type);
                    case "i16":
                        return KdlConvert.ToInt16(data, (short) sign, radix, type);
                    case "i32":
                        return KdlConvert.ToInt32(data, sign, radix, type);
                    case "i64":
                        return KdlConvert.ToInt64(data, sign, radix, type);
                    case "f32":
                        return KdlConvert.ToFloat32(data, radix, result.Format, type);
                    case "f64":
                    case "decimal64":
                        return KdlConvert.ToFloat64(data, radix, result.Format, type);
                    case "decimal":
                    case "decimal128":
                        return KdlConvert.ToDecimal(data, radix, result.Format, type);
                    // case "isize":
                    // case "usize":
                }
            }

            // Only try parsing as float if we know it has a dot or an exponent
            // We use 64-bits regardless of size
            if (result.Format.HasFlag(KdlDecimalFormat.HasPoint) || result.Format.HasFlag(KdlDecimalFormat.HasExponent)) {
                if (_options.PreferSingle) {
                    return KdlConvert.ToFloat32(data, radix, result.Format, type);
                } else if (_options.PreferDecimal) {
                    return KdlConvert.ToDecimal(data, radix, result.Format, type);
                }

                return KdlConvert.ToFloat64(data, radix, result.Format, type);
            }

            // Otherwise, the number must be an integer
            // First try 32-bit, then 64-bit, then BigInteger
            try {
                int number = Convert.ToInt32(trim, radix);
                if (number < 0) {
                    throw new OverflowException();
                }

                number *= sign;
                return new KdlInt32(number, data, type);
            } catch (OverflowException) {
                try {
                    long number = Convert.ToInt64(trim, radix);
                    if (number < 0) {
                        throw new OverflowException();
                    }

                    number *= sign;
                    return new KdlInt64(number, data, type);
                } catch (OverflowException) {
                    if (radix == 8 || radix == 2) {
                        throw new NotSupportedException("Very large octal or binary numbers are not supported.");
                    }

                    NumberStyles styles = (radix == 10) ? NumberStyles.Number : NumberStyles.HexNumber;
                    BigInteger number = BigInteger.Parse('0' + trim, styles);
                    number *= sign;

                    return new KdlBigInteger(number, data, type);
                }
            }
        }

        /// <summary>
        /// Parses a decimal number, which can be a float or an integer.
        /// </summary>
        /// <returns>
        /// A <see cref="DecimalResult"/> struct, containing the string and indicating if it has 
        /// a decimal fraction and/or an exponent.
        /// </returns>
        /// <exception cref="KdlException"/>
        internal DecimalResult ParseDecimal(KdlParseContext context) {
            StringBuilder builder = new StringBuilder();
            int c = context.Peek();

            KdlDecimalFormat format = KdlDecimalFormat.OnlyZeroes;
            
            // Integer part
            while (true) {
                c = context.Peek();

                if (Util.IsDecimalDigit(c)) {
                    if (c != '0') {
                        format &= ~KdlDecimalFormat.OnlyZeroes;
                    }

                    c = context.Read();
                    builder.Append((char) c);
                } else if (c == '_') {
                    context.Read();
                    continue;
                } else {
                    break;
                }
            }

            // Fractional part
            if (c == '.') {
                c = context.Read();
                builder.Append((char) c);

                c = context.Peek();
                if (!Util.IsDecimalDigit(c)) {
                    throw new KdlException("Decimal fractions must have at least one digit.", context);
                }

                while (true) {
                    c = context.Peek();

                    if (Util.IsDecimalDigit(c)) {
                        if (c != '0') {
                            format &= ~KdlDecimalFormat.OnlyZeroes;
                        }

                        c = context.Read();
                        builder.Append((char) c);
                    } else if (c == '_') {
                        context.Read();
                    } else {
                        break;
                    }
                }

                format |= KdlDecimalFormat.HasPoint;
            }

            // Exponent part
            // If neither this one or the fraction is present, we have an integer
            if (c == 'e' || c == 'E') {
                c = context.Read();
                builder.Append((char) c);

                c = context.Peek();
                if (c == '+' || c == '-') {
                    context.Read();
                    builder.Append((char) c);

                    c = context.Peek();
                }
                
                if (!Util.IsDecimalDigit(c)) {
                    throw new KdlException($"Unexpected character when parsing decimal exponent: {(char) c}", context);
                }

                while (true) {
                    c = context.Peek();

                    if (Util.IsDecimalDigit(c)) {
                        c = context.Read();
                        builder.Append((char) c);
                    } else if (c == '_') {
                        context.Read();
                    } else {
                        break;
                    }
                }

                format |= KdlDecimalFormat.HasExponent;
            }

            return new DecimalResult {
                String = builder.ToString(),
                Format = format
            };
        }

        /// <summary>
        /// Parses a property ('name'=value) or an argument (an unnamed value). 
        /// Called in ParseNode after its identifier is obtained.
        /// </summary>
        /// <returns>
        /// A <see cref="KdlProperty"/> or a <see cref="KdlValue"/>.
        /// </returns>
        /// <exception cref="KdlException"/>
        internal IKdlObject ParseArgumentOrProperty(KdlParseContext context) {
            IKdlObject obj = KdlNull.Instance;
            StringBuilder builder = new StringBuilder();

            // If we have a type in the property name, that's an error, but
            // we don't know if we'll have a property or argument yet
            string? type = ParseType(context);
            int c = context.Peek();

            /*
                This function first tries finding an escaped string,
                then a number *without a sign*, then an identifier.

                The identifier can turn out to be a few different things:
                If it's 'true', 'false', or 'null', we'll get a KdlBool or KdlNull.
                If it can produce a number, it'll throw an exception which will be catched
                and then it will attempt to parse a number. 

                Later, if we don't treat what we found as a property, those become valid
                values, otherwise they'll throw.

                If it's still an identifier, it'll be valid as a property always. If we
                have a property, it'll check if it's a string, otherwise it'll throw.
            */

            // First we try parsing an escaped string
            if (c == '\"') {
                obj = ParseString(context, type);
            // Otherwise, a number *without a sign*
            } else if (Util.IsDecimalDigit(c)) {
                obj = ParseNumber(context, null, type);
            // Otherwise, an identifier, with the behaviour explained above
            } else if (Util.IsValidInitialCharacter(c)) {
                try {
                    KdlIdentifier identifier = ParseIdentifier(context);

                    // Keywords, turn them into its correct types
                    if (Util.IsKeyword(identifier.Name)) {
                        obj = KdlConvert.ToBoolOrNull(identifier.Name, type);
                    } else {
                        obj = identifier;
                    }
                } catch (KdlIdentifierSignException e) {
                    // This looks like a number, so we try parsing one instead
                    obj = ParseNumber(context, e.Sign, type);
                }
            } else {
                throw new KdlException($"Unexpected character: {(char) c}", context);
            }

            // Now we check if this is an argument or a property
            c = context.Peek();
            if (c == '=') {
                // Either of these can serve as a property name
                if (obj is KdlIdentifier || obj is KdlString) {
                    if (type != null) {
                        throw new KdlException("Property types must come before the value, not before the name.", context);
                    }

                    // Consume the '='
                    context.Read();

                    string identifier = string.Empty;
                    if (obj is KdlIdentifier id) {
                        identifier = id.Name;
                    } else if (obj is KdlString str) {
                        identifier = str.Value;
                    }

                    KdlValue? value = ParseValue(context);

                    if (value != null) {
                        return new KdlProperty(identifier, value);
                    } else {
                        throw new KdlException("Could not parse a valid property value.", context);
                    }
                } else {
                    throw new KdlException("Property name must be a valid identifier or a string.", context);
                }
            } else {
                if (obj is KdlValue value) {
                    value.Type = type;
                }

                // Arguments that are strings or raw strings are returned as KdlIdentifier,
                // which can be bare (invalid) or a string or raw string, in which case
                // we wrap them into a KdlString, otherwise we throw
                if (obj is KdlIdentifier identifier) {
                    if (!identifier.IsExplicitString) {
                        throw new KdlException("Arguments can't be bare identifiers.", context);
                    } else {
                        obj = new KdlString(identifier.Name, type);
                    }
                }
                
                // Finally, if we have a string but a type converter, we run it through
                if (obj is KdlString strObj && type != null && _options.TypeConverters.ContainsKey(type)) {
                    obj = _options.TypeConverters[type](strObj.Value, type, _options);
                }
            }

            return obj;
        }

        /// <summary>
        /// Consumes all whitespace, leaving the stream where it's over.
        /// </summary>
        internal WhitespaceResult ParseWhitespace(KdlParseContext context) {
            WhitespaceResult result = new WhitespaceResult {
                Found = false,
                StoppedAtLine = false
            };

            while (true) {
                int c = context.Peek();

                if (Util.IsWhitespace(c) || c == Util.BOM) {
                    context.Read();
                    result.Found = true;
                } else if (c == '/') {
                    result.Found = true;
                    c = context.Read();
                    c = context.Peek();
                    
                    if (c == '*') {
                        c = context.Read();
                        ParseBlockComment(context);
                    } else if (c == '-' || c == '/') {
                        context.Unread('/');
                        result.StoppedAtLine = true;

                        return result;
                    } else {
                        throw new KdlException("Found stray / where whitespace was expected.", context);
                    }
                } else {
                    return result;
                }
            }
        }

        /// <summary>
        /// Consumes all space between nodes, including whitespace, newlines and comments.
        /// </summary>
        internal void ParseLinespace(KdlParseContext context) {
            int c = context.Peek();

            while (true) {
                c = context.Peek();

                if (Util.IsWhitespace(c) || Util.IsNewline(c) || c == Util.BOM) {
                    context.Read();
                } else if (c == '/') {
                    c = context.Read();
                    c = context.Peek();

                    if (c == '/') {
                        context.Read();
                        ParseLineComment(context);
                    } else if (c == '*') {
                        context.Read();
                        ParseBlockComment(context);
                    } else if (c == '-') {
                        context.Unread('/');
                        return;
                    } else {
                        throw new KdlException("Found stray / where whitespace was expected.", context);
                    }
                } else {
                    return;
                }
            }
        }

        /// <summary>
        /// Consumes a line continuation, including whitespace and single-line comments.
        /// </summary>
        internal bool ParseEscline(KdlParseContext context) {
            int c = context.Peek();

            if (c == '\\') {
                c = context.Read();
                ParseWhitespace(context);
                c = context.Peek();

                if (c == '/') {
                    c = context.Read();
                    c = context.Read();

                    if (c == '/') {
                        ParseLineComment(context);
                        return true;
                    } else {
                        throw new KdlException($"Found stray ({(char) c}) where a comment or newline was expected.", context);
                    }
                } else if (Util.IsNewline(c)) {
                    c = context.Read(); 
                    return true;
                } else {
                    throw new KdlException($"Found stray ({(char) c}) where a comment or newline was expected.", context);
                }
            }

            return false;
        }

        /// <summary>
        /// Consumes an escape (/-), and if found, all following nodespace.
        /// </summary>
        internal EscapeNodespaceResult ParseEscapeAndNodespace(KdlParseContext context) {
            EscapeNodespaceResult result = new EscapeNodespaceResult{
                Found = false,
                Escape = false,
            };

            int c = context.Peek();

            if (c == '/') {
                c = context.Read();
                c = context.Read();

                if (c == '-') {
                    result.Found = ParseNodespace(context);
                    result.Escape = true;
                    return result;
                } else {
                    throw new KdlException($"Found stray ({(char) c}) where a node/property/argument/children escape was expected.", context);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Consumes space between node contents.
        /// </summary>
        internal bool ParseNodespace(KdlParseContext context) {
            bool nodespace = false;

            while (true) {
                WhitespaceResult res1 = ParseWhitespace(context);
                bool escline = ParseEscline(context);
                WhitespaceResult res2 = ParseWhitespace(context);

                if (res1.Found || escline || res2.Found) {
                    nodespace = true;
                }

                char c = (char) context.Peek();
                if ((!Util.IsWhitespace(c) && c != Util.BOM && c != '\\') || res1.StoppedAtLine || res2.StoppedAtLine) {
                    return nodespace;
                }
            }
        }
    }
}