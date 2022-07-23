using System.IO;

namespace Kadlet
{
    /// <summary>
    /// A class representing an identifier. Only used internally, exposed classes always represent
    /// identifiers with a <see cref="string"/>.
    /// </summary>
    internal class KdlIdentifier : IKdlObject
    {
        /// <value>The name for this identifier.</value>
        public string Name { get; }

        /// <value>Whether this identifier was parsed as a string or raw string, or as a bare identifier.</value>
        public bool IsExplicitString { get; }

        public KdlIdentifier(string name, bool isExplicitString = false) {
            Name = name;
            IsExplicitString = isExplicitString;
        }

        public void Write(TextWriter writer, KdlPrintOptions options) {
        }

        public string ToKdlString(KdlPrintOptions options) {
            return string.Empty;
        }
    }
}