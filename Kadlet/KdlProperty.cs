using System.IO;

namespace Kadlet
{
    /// <summary>
    /// A class representing a property in a node. Only used internally, exposed classes always represent
    /// properties with a KeyValuePair&lt;string, KdlValue&gt;>.
    /// </summary>
    internal class KdlProperty : IKdlObject
    {
        /// <value>The identifier for this property.</value>
        public string Key;

        /// <value>The value for this property.</value>
        public KdlValue Value;

        public KdlProperty(string key, KdlValue value) {
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