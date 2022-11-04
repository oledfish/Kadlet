namespace Kadlet
{
    /// <summary>
    /// A struct returning information after parsing nodespace.
    /// </summary>
    internal struct EscapeNodespaceResult
    {
        /// <value>Whether any space was found or not.</value>
        public bool Found;

        /// <value>Whether an escape was found.</value>
        public bool Escape;
    }
}