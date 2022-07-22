using System.IO;
using System.Collections.Generic;

namespace Kadlet 
{
    /// <summary>
    /// A class representing a KDL node.
    /// </summary>
    public class KdlNode : IKdlObject 
    {
        /// <value>The name for this node.</value>
        public string Identifier;

        /// <value>The type annotation for this node.</value>
        public string? Type;

        /// <value>The unnamed values for this node.</value>
        public List<KdlValue> Properties;

        /// <value>The named values for this node.</value>
        public Dictionary<string, KdlValue> Arguments;

        /// <value>A <see cref="KdlDocument"/> containing any children nodes.</value>
        public KdlDocument? Children;

        /// <value>The depth of this node within the *root* document.</value>
        internal int HierarchyLevel; 

        public KdlNode(string identifier, string? type) {
            Identifier = identifier;
            Type = type;
            Properties = new List<KdlValue>();
            Arguments = new Dictionary<string, KdlValue>();
        }

        public KdlNode(string identifier, string? type, List<KdlValue> properties, Dictionary<string, KdlValue> arguments, KdlDocument? children, int hierarchyLevel) {
            Identifier = identifier;
            Type = type;
            Arguments = arguments;
            Properties = properties;
            Children = children;
            HierarchyLevel = hierarchyLevel;
        }

        public string ToKdlString() {
            return ToKdlString(KdlPrintOptions.PrettyPrint);
        }

        public string ToKdlString(KdlPrintOptions options) {
            StringWriter writer = new StringWriter();
            Write(writer, options);

            return writer.ToString();
        }

        public void Write(TextWriter writer, KdlPrintOptions options) {
            for (int i = 0; i < HierarchyLevel; i++)
                writer.Write(new string(options.IndentChar, options.IndentSize));

            if (Type != null) {
                writer.Write('(');
                Util.WriteQuotedIdentifier(writer, Type, options);
                writer.Write(')');
            }

            Util.WriteQuotedIdentifier(writer, Identifier, options);

            foreach (KdlValue property in Properties) {
                if (property is KdlNull && !options.PrintNullProperties)
                    continue;

                writer.Write(' ');
                property.Write(writer, options);
            }

            foreach (KeyValuePair<string, KdlValue> argument in Arguments) {
                if (argument.Value is KdlNull && !options.PrintNullArguments)
                    continue;
    
                writer.Write(' ');
                Util.WriteQuotedIdentifier(writer, argument.Key, options);
                writer.Write('=');
                argument.Value.Write(writer, options);
            }

            if (Children != null && Children.Nodes.Count > 0) {
                writer.Write(" {");
                writer.Write(options.Newline);
                Children.Write(writer, options);

                for (int i = 0; i < HierarchyLevel; i++)
                    writer.Write(new string(options.IndentChar, options.IndentSize));

                writer.Write("}");
            }
            
            if (Children != null && Children.Nodes.Count == 0 && options.PrintEmptyChildren) {
                writer.Write(" {}");
            }

            if (options.TerminateNodesWithSemicolon) {
                writer.Write(";");
            }
        }

        public override string ToString() => $"KdlNode {{ Identifier: {Identifier} }}";
    } 
}