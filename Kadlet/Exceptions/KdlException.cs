using System;

namespace Kadlet 
{
    /// <summary>
    /// A generic exception thrown when <see cref="KdlReader"/> is halted due to a parsing error.
    /// </summary>
    public class KdlException : Exception {
        private static string AppendContext(string message, KdlParseContext? context) {
            if (context != null) {
                message += context.Abort();
            }

            return message;
        }

        public KdlException(string message, KdlParseContext? context) : base(AppendContext(message, context)) {}
        public KdlException(string message, Exception innerException, KdlParseContext? context) : base(AppendContext(message, context)) {}
    }
}