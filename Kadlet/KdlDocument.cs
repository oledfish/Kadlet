using System.IO;
using System.Collections.Generic;

namespace Kadlet
{
    /// <summary>
    /// A class representing a KDL document.
    /// </summary>
    public class KdlDocument : IKdlObject
    {
        public List<KdlNode> Nodes;

        public KdlDocument(List<KdlNode> nodes) {
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
    }
}