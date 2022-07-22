using System;

namespace Kadlet 
{
    /// <summary>
    /// Exception used internally to halt parsing a value as a raw string.
    /// Whatever follows is then parsed as a bare identifier.
    /// </summary>
    internal class KdlRawStringException : Exception 
    {
        public int Hashes;

        public KdlRawStringException(string message, int hashes = 0) : base(message) {
            Hashes = hashes;
        }

        public KdlRawStringException(string message, Exception innerException, int hashes = 0) : base(message, innerException) {
            Hashes = hashes;
        }
    }
}