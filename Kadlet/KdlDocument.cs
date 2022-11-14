using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Kadlet
{
    /// <summary>
    /// A class representing a KDL document.
    /// </summary>
    public class KdlDocument : IKdlObject
    {
        /// <value>A list of all the <see cref="KdlNode">s found in the document.</value>
        public IReadOnlyList<KdlNode> Nodes { get; }

        /// <value>A <see cref="SourceSpan"> indicating the start and end of the document.</value>
        public SourceSpan? SourceSpan { get; internal set; }

        public KdlDocument(IReadOnlyList<KdlNode> nodes) {
            Nodes = nodes;
        }

        public string ToKdlString() {
            return ToKdlString(KdlPrintOptions.PrettyPrint);
        }

        public string ToKdlString(KdlPrintOptions options) {
            StringWriter writer = new StringWriter();
            Write(writer, options);

            return writer.ToString();
        }

        public void Write(TextWriter writer) {
            Write(writer, KdlPrintOptions.PrettyPrint);
        }

        public void Write(TextWriter writer, KdlPrintOptions options) {
            if (Nodes.Count == 0) {
                writer.Write(options.Newline);
                return;
            } 

            foreach (KdlNode node in Nodes) {
                node.Write(writer, options);
                writer.Write(options.Newline);
            }
        }

        public override string ToString() {
            return $"KdlDocument {{ Nodes: {{ {string.Join(", ", Nodes)} }} }}";
        }

        public override bool Equals(object? obj) {
            if (!(obj is KdlDocument other)) {
                return false;
            }

            return Nodes.SequenceEqual(other.Nodes);
        }

        public override int GetHashCode() {
            HashCode hash = new HashCode();

            foreach (KdlNode node in Nodes)
                hash.Add(node.GetHashCode());

            return hash.ToHashCode();
        }
    }
}