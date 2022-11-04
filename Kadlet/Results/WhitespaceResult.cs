namespace Kadlet
{
    /// <summary>
    /// A struct returning information after parsing whitespace.
    /// </summary>
    internal struct WhitespaceResult
    {
        /// <value>Whether any space was found or not.</value>
        public bool Found;

        /// <value>Whether a single-line comment or an escape was found.</value>
        public bool StoppedAtLine;
    }
}