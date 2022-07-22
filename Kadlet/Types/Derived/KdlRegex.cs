using System.IO;
using System.Text.RegularExpressions;

namespace Kadlet
{
    /// <summary>
    /// A <see cref="KdlValue"/> wrapping a <see cref="Regex"/>.
    /// </summary>
    public class KdlRegex : KdlValue<Regex>
    {
        public KdlRegex(Regex value, string? type = null) : base(value, type) {
        }
    }
}