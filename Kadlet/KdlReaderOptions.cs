using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kadlet
{
    /// <summary>
    /// An utility class to customize the behavior of <see cref="KdlReader"/>.
    /// </summary>
    public class KdlReaderOptions 
    {
        /// <value>A map of type annotations and their custom converters.</value>
        public Dictionary<string, KdlTypeConverter> TypeConverters = new Dictionary<string, KdlTypeConverter>();

        /// <value>If set, floating point numbers will be returned as <see cref="KdlFloat32"/> instead of <see cref="KdlFloat64"/>.</value>
        public bool PreferSingle = false;

        /// <value>If set, floating point numbers will be returned as <see cref="KdlDecimal"/> instead of <see cref="KdlFloat64"/>.</value>
        public bool PreferDecimal = false;

        /// <value>If unset, the converters for type annotations will be ignored.</value>
        public bool UseTypeAnnotations = true;

        /// <value>When reading a (<see cref="regex"/>) annotated string and with converters enabled, these will be passed to the constructor.</value>
        public RegexOptions RegexOptions = RegexOptions.None;
    }
}