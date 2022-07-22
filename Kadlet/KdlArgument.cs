using System.IO;

namespace Kadlet
{
    /// <summary>
    /// A class representing an argument in a node. Only used internally, exposed classes always represent
    /// arguments with a KeyValuePair&lt;string, KdlValue&gt;>.
    /// </summary>
    internal class KdlArgument : IKdlObject
    {
        /// <value>The identifier for this argument.</value>
        public string Key;

        /// <value>The value for this argument.</value>
        public KdlValue Value;

        public KdlArgument(string key, KdlValue value) {
            Key = key;
            Value = value;
        }

        public void Write(TextWriter writer, KdlPrintOptions options) {
        }

        public string ToKdlString(KdlPrintOptions options) {
            return string.Empty;
        }
    }
}