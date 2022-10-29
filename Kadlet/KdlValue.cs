using System;
using System.IO;
using System.Numerics;

namespace Kadlet
{
    /// <summary>
    /// An utility wrapper for C# object types, including a type annotation.
    /// </summary>
    public abstract class KdlValue : IKdlObject
    {
        /// <value>The type annotation for this value.</value>
        public string? Type { get; internal set; }
        
        protected KdlValue(string? type) {
            Type = type;
        }

        /// <summary>
        /// Returns a string representation of the underlying value.
        /// </summary>
        public abstract string ToKdlString();

        /// <summary>
        /// Returns a string representation of the underlying value.
        /// </summary>
        public abstract string ToKdlString(KdlPrintOptions options);

        /// <summary>
        /// Writes the type annotation followed by the value as valid KDL.
        /// </summary>
        public abstract void Write(TextWriter writer, KdlPrintOptions options);

        /// <summary>
        /// Writes only the underlying value as valid KDL.
        /// </summary>
        public abstract void WriteValue(TextWriter writer, KdlPrintOptions options);

        /// <summary>
        /// Writes only the type annotation.
        /// </summary>
        protected void WriteType(TextWriter writer, KdlPrintOptions options) {
            if (Type != null) {
                writer.Write('(');
                Util.WriteQuotedIdentifier(writer, Type, options);
                writer.Write(')');
            }
        }

        public abstract override bool Equals(object? obj);
        public abstract override int GetHashCode();
    }

    /// <summary>
    /// An utility wrapper for C# object types, including a type annotation.
    /// </summary>
    public abstract class KdlValue<T> : KdlValue
    {
        public T Value { get; }

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
            WriteType(writer, options);
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

        public override bool Equals(object? obj) {
            if (Value == null || obj == null)
                return false;

            return obj is KdlNumber<T> other && other.Value != null && Value.Equals(other.Value) && Type == other.Type;
        }

        public override int GetHashCode() {
            HashCode hash = new HashCode();

            if (Value != null)
                hash.Add(Value.GetHashCode());

            if (Type != null)
                hash.Add(Type.GetHashCode());

            return hash.ToHashCode();
        }

        public static implicit operator T(KdlValue<T> v) => v.Value;
    }
}