#pragma warning disable CS8618
#pragma warning disable CS8601

using System;
using System.IO;

namespace Kadlet
{
    /// <summary>
    /// An utility wrapper for C# object types, including a type annotation.
    /// </summary>
    public abstract class KdlValue : IKdlObject
    {
        /// <value>The type annotation for this value.</value>
        public string? Type;
        protected KdlValue(string? type) {
            Type = type;
        }

        /// <summary>
        /// Returns a string representation of the underlying value.
        /// </summary>
        public virtual string ToKdlString() {
            return string.Empty;
        }

        /// <summary>
        /// Returns a string representation of the underlying value.
        /// </summary>
        public virtual string ToKdlString(KdlPrintOptions options) {
            return string.Empty;
        }

        /// <summary>
        /// Writes the type annotation followed by the value as valid KDL.
        /// </summary>
        public virtual void Write(TextWriter writer, KdlPrintOptions options) {

        }

        /// <summary>
        /// Writes only the underlying value as valid KDL.
        /// </summary>
        public virtual void WriteValue(TextWriter writer, KdlPrintOptions options) {

        }
    }

    /// <summary>
    /// An utility wrapper for C# object types, including a type annotation.
    /// </summary>
    public class KdlValue<T> : KdlValue
    {
        public T Value;

        public KdlValue(T value, string? type) : base (type) {
            Value = value;
        }

        public override sealed string ToKdlString() {
            return ToKdlString(KdlPrintOptions.PrettyPrint);
        }

        public override sealed string ToKdlString(KdlPrintOptions options) {
            StringWriter writer = new StringWriter();
            Write(writer, options);

            return writer.ToString();
        }

        // Overriden in KdlNumber, KdlBool and KdlNull to not use quotes
        // There's a lot more derived types that need to be printed as strings,
        // so this avoids some repetition by not having to always define them
        // as being written with quotes.
        public override void Write(TextWriter writer, KdlPrintOptions options) {
            if (Type != null) {
                writer.Write('(');
                Util.WriteQuotedIdentifier(writer, Type, options);
                writer.Write(')');
            }

            writer.Write('"');
            WriteValue(writer, options);
            writer.Write('"');
        }

        public override void WriteValue(TextWriter writer, KdlPrintOptions options) {
            writer.Write(Convert.ToString(Value) ?? string.Empty);
        }

        public override string ToString() {
            return $"KdlValue {{ Value={Value}, Type={Type ?? "null"} }}";
        }

        public static implicit operator T(KdlValue<T> v) => v.Value;
    }
}