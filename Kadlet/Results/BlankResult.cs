namespace Kadlet
{
    /// <summary>
    /// A struct returning information after parsing blank space.
    /// </summary>
    internal struct BlankResult
    {
        /// <value>If a <see cref="/-"/> escape was found, indicating that the next node, argument, property or children must be ignored.</value>
        public bool EscapeNext;

        /// <value>Whether any space was found or not.</value>
        public bool Empty;

        /// <value>Whether a single-line comment was found.</value>
        public bool LineComment;
    }
}