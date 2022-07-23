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
        public string Identifier { get; }

        /// <value>The type annotation for this node.</value>
        public string? Type { get; }

        /// <value>The unnamed values for this node.</value>
        public IReadOnlyList<KdlValue> Arguments { get; }

        /// <value>The named values for this node.</value>
        public IReadOnlyDictionary<string, KdlValue> Properties { get; }

        /// <value>A <see cref="KdlDocument"/> containing any children nodes.</value>
        public KdlDocument? Children { get; }

        /// <value>The depth of this node within the *root* document.</value>
        internal int HierarchyLevel { get; }

        public KdlNode(string identifier, string? type) {
            Identifier = identifier;
            Type = type;
            Arguments = new List<KdlValue>();
            Properties = new Dictionary<string, KdlValue>();
        }

        public KdlNode(string identifier, string? type, IReadOnlyList<KdlValue> arguments, IReadOnlyDictionary<string, KdlValue> properties, KdlDocument? children, int hierarchyLevel) {
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

            foreach (KdlValue argument in Arguments) {
                if (argument is KdlNull && !options.PrintNullArguments)
                    continue;

                writer.Write(' ');
                argument.Write(writer, options);
            }

            foreach (KeyValuePair<string, KdlValue> property in Properties) {
                if (property.Value is KdlNull && !options.PrintNullProperties)
                    continue;
    
                writer.Write(' ');
                Util.WriteQuotedIdentifier(writer, property.Key, options);
                writer.Write('=');
                property.Value.Write(writer, options);
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

        public override string ToString() => 
            $"KdlNode {{ Identifier: {Identifier}, Type: {Type ?? "null"}, Properties: [{string.Join(", ", Properties) }], Arguments: [{string.Join(", ", Arguments)}], Children: {Children}";
    } 
}