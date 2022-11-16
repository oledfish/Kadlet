namespace Kadlet
{
    /// <summary>
    /// A struct returning information after parsing a decimal number.
    /// </summary>
    internal struct DecimalResult
    {
        /// <value>The KDL representation of the number found.</value>
        public string String;

        /// <value>The format of the number, indicating if it has a fractional part, an exponent part, and/or is zero.</value>
        public KdlDecimalFormat Format;
    }
}