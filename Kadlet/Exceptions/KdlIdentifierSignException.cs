using System;

namespace Kadlet 
{
    /// <summary>
    /// Exception used internally to halt parsing a value as a bare identifier.
    /// Whatever follows is then parsed as a number.
    /// </summary>
    internal class KdlIdentifierSignException : Exception {
        public char Sign;

        public KdlIdentifierSignException(string message, char sign) : base(message) {
            Sign = sign;
        }

        public KdlIdentifierSignException(string message, Exception innerException, char sign) : base(message, innerException) {
            Sign = sign;
        }
    }
}