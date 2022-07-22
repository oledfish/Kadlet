namespace Kadlet
{
    /// <summary>
    /// A struct returning information after parsing a decimal number.
    /// </summary>
    internal struct DecimalResult
    {
        /// <value>The KDL representation of the number found.</value>
        public string String;

        /// <value>Whether the number has a fractional part.</value>
        public bool HasPoint;

        /// <value>Whether the number has an exponent part.</value>
        public bool HasExponent;

        /// <value>Whether the integer and fractional part are composed only of zeroes.</value>
        public bool OnlyZeroes;
    }
}