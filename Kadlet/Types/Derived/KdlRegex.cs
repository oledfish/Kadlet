#pragma warning disable CS0659

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

        public override bool Equals(object? obj) {
            return obj is KdlRegex other && Value.Equals(other.Value) && Type == other.Type;
        }
    }
}